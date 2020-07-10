// This file is part of Emzi0767.Common project
//
// Copyright 2020 Emzi0767
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace Emzi0767.Types
{
    /// <summary>
    /// Provides a resizable memory buffer, which can be read from and written to. It will automatically resize whenever required.
    /// </summary>
    public sealed class MemoryBuffer : IDisposable
    {
        /// <summary>
        /// Gets the total capacity of this buffer. The capacity is the number of segments allocated, multiplied by size of individual segment.
        /// </summary>
        public ulong Capacity => this._segments.Aggregate(0UL, (a, x) => a + (ulong)x.Memory.Length); // .Sum() does only int

        /// <summary>
        /// Gets the amount of bytes currently written to the buffer. This number is never greather than <see cref="Capacity"/>.
        /// </summary>
        public ulong Length { get; private set; }

        private readonly MemoryPool<byte> _pool;
        private readonly int _segmentSize;
        private int _lastSegmentLength;
        private int _segNo;
        private bool _clear;
        private List<IMemoryOwner<byte>> _segments;
        private bool _isDisposed;

        /// <summary>
        /// Creates a new buffer with a specified segment size, specified number of initially-allocated segments, and supplied memory pool.
        /// </summary>
        /// <param name="segmentSize">Byte size of an individual segment. Defaults to 64KiB.</param>
        /// <param name="initialSegmentCount">Number of segments to allocate. Defaults to 0.</param>
        /// <param name="memPool">Memory pool to use for renting buffers. Defaults to <see cref="MemoryPool{T}.Shared"/>.</param>
        /// <param name="clearOnDispose">Determines whether the underlying buffers should be cleared on exit. If dealing with sensitive data, it might be a good idea to set this option to true.</param>
        public MemoryBuffer(int segmentSize = 65536, int initialSegmentCount = 0, MemoryPool<byte> memPool = default, bool clearOnDispose = false)
        {
            this._pool = memPool ?? MemoryPool<byte>.Shared;

            this._segmentSize = segmentSize;
            this._segNo = 0;
            this._lastSegmentLength = 0;
            this._clear = clearOnDispose;
            this._segments = Enumerable.Range(0, initialSegmentCount)
                .Select(x => this._pool.Rent(this._segmentSize))
                .ToList();
            this.Length = 0;

            this._isDisposed = false;
        }

        /// <summary>
        /// Appends data from a supplied buffer to this buffer, growing it if necessary.
        /// </summary>
        /// <param name="data">Buffer containing data to write.</param>
        public void Write(ReadOnlySpan<byte> data)
        {
            if (this._isDisposed)
                throw new InvalidOperationException("This buffer is disposed.");

            this.Grow(data.Length);

            var src = data;
            while (this._segNo < this._segments.Count && src.Length > 0)
            {
                var seg = this._segments[this._segNo];
                var mem = seg.Memory;
                var avs = mem.Length - this._lastSegmentLength;
                avs = avs > src.Length
                    ? src.Length
                    : avs;

                src.Slice(0, avs).CopyTo(mem.Span);
                src = src.Slice(avs);

                this.Length += (ulong)avs;
                this._lastSegmentLength += avs;

                if (this._lastSegmentLength == mem.Length)
                {
                    this._segNo++;
                    this._lastSegmentLength = 0;
                }
            }
        }

        /// <summary>
        /// Appends data from a supplied array to this buffer, growing it if necessary.
        /// </summary>
        /// <param name="data">Array containing data to write.</param>
        /// <param name="start">Index from which to start reading the data.</param>
        /// <param name="count">Number of bytes to read from the source.</param>
        public void Write(byte[] data, int start, int count)
            => this.Write(data.AsSpan(start, count));

        /// <summary>
        /// Appends data from a supplied array slice to this buffer, growing it if necessary.
        /// </summary>
        /// <param name="data">Array slice containing data to write.</param>
        public void Write(ArraySegment<byte> data)
            => this.Write(data.AsSpan());

        /// <summary>
        /// Reads data from this buffer to the specified destination buffer. This method will write either as many 
        /// bytes as there are in the destination buffer, or however many bytes are available in this buffer, 
        /// whichever is less.
        /// </summary>
        /// <param name="destination">Buffer to read the data from this buffer into.</param>
        /// <param name="source">Starting position in this buffer to read from.</param>
        /// <returns>Whether more data is available in this buffer.</returns>
        public bool Read(Span<byte> destination, int source)
        {
            if (this._isDisposed)
                throw new InvalidOperationException("This buffer is disposed.");

            return false;
        }

        /// <summary>
        /// Reads data from this buffer to specified destination array. This method will write either as many bytes 
        /// as specified for the destination array, or however many bytes are available in this buffer, whichever is 
        /// less.
        /// </summary>
        /// <param name="data">Array to read the data from this buffer into.</param>
        /// <param name="start">Starting position in the target array to write to.</param>
        /// <param name="count">Maximum number of bytes to write to target array.</param>
        /// <param name="source">Starting position in this buffer to read from.</param>
        /// <returns>Whether more data is available in this buffer.</returns>
        public bool Read(byte[] data, int start, int count, int source)
            => this.Read(data.AsSpan(start, count), source);

        /// <summary>
        /// Reads data from this buffer to specified destination array slice. This method will write either as many 
        /// bytes as specified in the target slice, or however many bytes are available in this buffer, whichever is 
        /// less.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="source"></param>
        public bool Read(ArraySegment<byte> data, int source)
            => this.Read(data.AsSpan(), source);

        /// <summary>
        /// Disposes of any resources claimed by this buffer.
        /// </summary>
        public void Dispose()
        {
            if (this._isDisposed)
                return;

            this._isDisposed = true;
            foreach (var segment in this._segments)
            {
                if (this._clear)
                    segment.Memory.Span.Clear();

                segment.Dispose();
            }
        }

        private void Grow(int minAmount)
        {
            var capacity = this.Capacity;
            var length = this.Length;
            var totalAmt = (length + (ulong)minAmount);
            if (capacity >= totalAmt)
                return; // we're good

            var amt = (int)(totalAmt - capacity);
            var segCount = amt / this._segmentSize;
            if (amt % this._segmentSize != 0)
                segCount++;

            // Basically List<T>.EnsureCapacity
            // Default grow behaviour is minimum current*2
            var segCap = this._segments.Count + segCount;
            if (segCap > this._segments.Capacity)
                this._segments.Capacity = segCap < this._segments.Capacity * 2
                    ? this._segments.Capacity * 2
                    : segCap;

            for (var i = 0; i < segCount; i++)
                this._segments.Add(this._pool.Rent(this._segmentSize));
        }
    }
}
