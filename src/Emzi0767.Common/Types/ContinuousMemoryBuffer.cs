// This file is part of Emzi0767.Common project.
//
// Copyright © 2020-2025 Emzi0767
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Emzi0767.Utilities;

namespace Emzi0767.Types;

/// <summary>
/// Provides a resizable memory buffer analogous to <see cref="MemoryBuffer{T}"/>, using a single continuous memory region instead.
/// </summary>
/// <typeparam name="T">Type of item to hold in the buffer.</typeparam>
public sealed class ContinuousMemoryBuffer<T> : IMemoryBuffer<T>
    where T : unmanaged
{
    /// <inheritdoc />
    public ulong Capacity => (ulong)this._buff.Length;

    /// <inheritdoc />
    public ulong Length => (ulong)this._pos;

    /// <inheritdoc />
    public ulong Count => (ulong)(this._pos / this._itemSize);

    /// <summary>
    /// Gets a typed view of the underlying buffer, bounded by total number of items written so far.
    /// </summary>
    public ReadOnlySpan<T> Span => MemoryMarshal.Cast<byte, T>(this._buff.Span[..(int)this.Length]);

    /// <summary>
    /// Gets a byte view of the underlying buffer, bounded by total number of bytes written so far.
    /// </summary>
    public ReadOnlySpan<byte> ByteSpan => this._buff.Span[..(int)this.Length];

    /// <summary>
    /// Gets a byte view of the underlying buffer, bounded by total number of bytes written so far.
    /// </summary>
    public ReadOnlyMemory<byte> ByteMemory => this._buff[..(int)this.Length];

    private readonly MemoryPool<byte> _pool;
    private IMemoryOwner<byte> _buffOwner;
    private Memory<byte> _buff;
    private readonly bool _clear;
    private int _pos;
    private readonly int _itemSize;
    private bool _isDisposed;

    /// <summary>
    /// Creates a new buffer with a specified segment size, specified number of initially-allocated segments, and supplied memory pool.
    /// </summary>
    /// <param name="initialSize">Initial size of the buffer in bytes. Defaults to 64KiB.</param>
    /// <param name="memPool">Memory pool to use for renting buffers. Defaults to <see cref="MemoryPool{T}.Shared"/>.</param>
    /// <param name="clearOnDispose">Determines whether the underlying buffers should be cleared on exit. If dealing with sensitive data, it might be a good idea to set this option to true.</param>
    public ContinuousMemoryBuffer(int initialSize = 65536, MemoryPool<byte> memPool = default, bool clearOnDispose = false)
    {
        this._itemSize = Unsafe.SizeOf<T>();
        this._pool = memPool ?? MemoryPool<byte>.Shared;
        this._clear = clearOnDispose;

        this._buffOwner = this._pool.Rent(initialSize);
        this._buff = this._buffOwner.Memory;

        this._isDisposed = false;
    }

    /// <inheritdoc />
    public void Write(ReadOnlySpan<T> data)
    {
        if (this._isDisposed)
            ThrowHelper.BufferDisposed();

        var bytes = MemoryMarshal.AsBytes(data);
        this.EnsureSize(this._pos + bytes.Length);

        bytes.CopyTo(this._buff[this._pos..].Span);
        this._pos += bytes.Length;
    }

    /// <inheritdoc />
    public void Write(T[] data, int start, int count)
        => this.Write(data.AsSpan(start, count));

    /// <inheritdoc />
    public void Write(ArraySegment<T> data)
        => this.Write(data.AsSpan());

    /// <inheritdoc />
    public void Write(Stream stream)
    {
        if (this._isDisposed)
            ThrowHelper.BufferDisposed();

        if (stream.CanSeek)
            this.WriteStreamSeekable(stream);
        else
            this.WriteStreamUnseekable(stream);
    }

    /// <inheritdoc />
    public void Write(Stream stream, ulong limit)
    {
        if (this._isDisposed)
            ThrowHelper.BufferDisposed();

        if (stream.CanSeek)
            this.WriteStreamSeekable(stream, limit);
        else
            this.WriteStreamUnseekable(stream, limit);
    }

    private void WriteStreamSeekable(Stream stream, ulong? limit = null)
    {
        if (limit is not null)
            limit -= limit % (ulong)this._itemSize;

        var readLength = stream.Length - stream.Position;
        readLength = limit is not null && (ulong)readLength > limit ? (int)limit : readLength;
        readLength -= readLength % this._itemSize;

        if (this._pos + readLength > int.MaxValue)
            ThrowHelper.Argument(nameof(stream), "Stream is too long.");

        this.EnsureSize(this._pos + (int)readLength);
        while (readLength > 0)
        {
            var br = stream.Read(this._buff.Slice(this._pos, (int)readLength).Span);

            this._pos += br;
            readLength -= br;
        }
    }

    private void WriteStreamUnseekable(Stream stream, ulong? limit = null)
    {
        if (limit is not null)
            limit -= limit % (ulong)this._itemSize;

        var readLength = 4096;
        readLength -= readLength % this._itemSize;

        var br = 0;
        do
        {
            var read = (int)Math.Min(limit ?? (ulong)readLength, (ulong)readLength);
            this.EnsureSize(this._pos + read);
            br = stream.Read(this._buff.Slice(this._pos, read).Span);
            this._pos += br;

            if (limit is not null)
                limit -= (ulong)br;
        }
        while (br != 0 && limit is not 0);
    }

    /// <inheritdoc />
    public ValueTask WriteAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        if (this._isDisposed)
            ThrowHelper.BufferDisposed();

        return stream.CanSeek
            ? this.WriteStreamSeekableAsync(stream, null, cancellationToken)
            : this.WriteStreamUnseekableAsync(stream, null, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask WriteAsync(Stream stream, ulong limit, CancellationToken cancellationToken = default)
    {
        if (this._isDisposed)
            ThrowHelper.BufferDisposed();

        return stream.CanSeek
            ? this.WriteStreamSeekableAsync(stream, limit, cancellationToken)
            : this.WriteStreamUnseekableAsync(stream, limit, cancellationToken);
    }

    private async ValueTask WriteStreamSeekableAsync(Stream stream, ulong? limit, CancellationToken cancellationToken)
    {
        if (limit is not null)
            limit -= limit % (ulong)this._itemSize;

        var readLength = stream.Length - stream.Position;
        readLength = limit is not null && (ulong)readLength > limit ? (int)limit : readLength;
        readLength -= readLength % this._itemSize;

        if (this._pos + readLength > int.MaxValue)
            ThrowHelper.Argument(nameof(stream), "Stream is too long.");

        this.EnsureSize(this._pos + (int)readLength);
        while (readLength > 0)
        {
            var br = await stream.ReadAsync(this._buff.Slice(this._pos, (int)readLength), cancellationToken).ConfigureAwait(false);

            this._pos += br;
            readLength -= br;
        }
    }

    private async ValueTask WriteStreamUnseekableAsync(Stream stream, ulong? limit, CancellationToken cancellationToken)
    {
        if (limit is not null)
            limit -= limit % (ulong)this._itemSize;

        var readLength = 4096;
        readLength -= readLength % this._itemSize;

        var br = 0;
        do
        {
            var read = (int)Math.Min(limit ?? (ulong)readLength, (ulong)readLength);
            this.EnsureSize(this._pos + read);
            br = await stream.ReadAsync(this._buff.Slice(this._pos, read), cancellationToken).ConfigureAwait(false);
            this._pos += br;

            if (limit is not null)
                limit -= (ulong)br;
        }
        while (br != 0);
    }

    /// <inheritdoc />
    public bool Read(Span<T> destination, ulong source, out int itemsWritten)
    {
        itemsWritten = 0;
        if (this._isDisposed)
            ThrowHelper.BufferDisposed();

        source *= (ulong)this._itemSize;
        if (source > this.Count)
            ThrowHelper.ArgumentOutOfRange(nameof(source), "Cannot copy data from beyond the buffer.");

        var start = (int)source;
        var sbuff = this._buff[start..this._pos].Span;
        var dbuff = MemoryMarshal.AsBytes(destination);
        if (sbuff.Length > dbuff.Length)
            sbuff = sbuff[..dbuff.Length];

        itemsWritten = sbuff.Length / this._itemSize;
        sbuff.CopyTo(dbuff);

        return this.Length - source != (ulong)itemsWritten;
    }

    /// <inheritdoc />
    public bool Read(T[] data, int start, int count, ulong source, out int itemsWritten)
        => this.Read(data.AsSpan(start, count), source, out itemsWritten);

    /// <inheritdoc />
    public bool Read(ArraySegment<T> data, ulong source, out int itemsWritten)
        => this.Read(data.AsSpan(), source, out itemsWritten);

    /// <inheritdoc />
    public ulong Read(Stream stream, ulong source)
        => this.Read(stream, source, this.Count - source);

    /// <inheritdoc />
    public ulong Read(Stream stream, ulong source, ulong count)
    {
        if (this._isDisposed)
            ThrowHelper.BufferDisposed();

        source *= (ulong)this._itemSize;
        count *= (ulong)this._itemSize;
        if (source > this.Count || source + count > this.Count)
            ThrowHelper.ArgumentOutOfRange(nameof(source), "Cannot copy data from beyond the buffer.");

        var start = (int)source;
        var len = (int)count;
        var sbuff = this._buff.Slice(start, len).Span;
        stream.Write(sbuff);

        return (ulong)(sbuff.Length / this._itemSize);
    }

    /// <inheritdoc />
    public ValueTask<ulong> ReadAsync(Stream stream, ulong source, CancellationToken cancellationToken = default)
        => this.ReadAsync(stream, source, this.Count - source, cancellationToken);

    /// <inheritdoc />
    public async ValueTask<ulong> ReadAsync(Stream stream, ulong source, ulong count, CancellationToken cancellationToken = default)
    {
        if (this._isDisposed)
            ThrowHelper.BufferDisposed();

        source *= (ulong)this._itemSize;
        count *= (ulong)this._itemSize;
        if (source > this.Count || source + count > this.Count)
            ThrowHelper.ArgumentOutOfRange(nameof(source), "Cannot copy data from beyond the buffer.");

        var start = (int)source;
        var len = (int)count;
        var sbuff = this._buff.Slice(start, len);
        await stream.WriteAsync(sbuff, cancellationToken).ConfigureAwait(false);

        return (ulong)(sbuff.Length / this._itemSize);
    }

    /// <inheritdoc />
    public T[] ToArray()
    {
        if (this._isDisposed)
            ThrowHelper.BufferDisposed();

        return MemoryMarshal.Cast<byte, T>(this._buff[..this._pos].Span).ToArray();
    }

    /// <inheritdoc />
    public ReadOnlySequence<T> AsSequence()
    {
        var seg = new MemoryBufferSegment<T>(this._buff[..this._pos].Cast<byte, T>());
        return new ReadOnlySequence<T>(seg, 0, seg, seg.Memory.Length);
    }

    /// <inheritdoc />
    public ReadOnlySequence<byte> AsByteSequence()
    {
        var seg = new MemoryBufferSegment<byte>(this._buff[..this._pos]);
        return new ReadOnlySequence<byte>(seg, 0, seg, seg.Memory.Length);
    }

    /// <inheritdoc />
    public void CopyTo(Stream destination)
    {
        if (this._isDisposed)
            ThrowHelper.BufferDisposed();

        destination.Write(this._buff[..this._pos].Span);
    }

    /// <inheritdoc />
    public async ValueTask CopyToAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        if (this._isDisposed)
            ThrowHelper.BufferDisposed();

        await destination.WriteAsync(this._buff[..this._pos], cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Clear()
    {
        if (this._isDisposed)
            ThrowHelper.BufferDisposed();

        this._pos = 0;
    }

    /// <summary>
    /// Disposes of any resources claimed by this buffer.
    /// </summary>
    public void Dispose()
    {
        if (this._isDisposed)
            return;

        this._isDisposed = true;
        if (this._clear)
            this._buff.Span.Clear();

        this._buffOwner.Dispose();
        this._buff = default;
    }

    void IBufferWriter<T>.Advance(int count)
        => this._pos += count * this._itemSize;

    Memory<T> IBufferWriter<T>.GetMemory(int sizeHint)
    {
        this.EnsureSize(this._pos + (sizeHint * this._itemSize));
        return this._buff[this._pos..].Cast<byte, T>();
    }

    Span<T> IBufferWriter<T>.GetSpan(int sizeHint)
    {
        this.EnsureSize(this._pos + (sizeHint * this._itemSize));
        return MemoryMarshal.Cast<byte, T>(this._buff[this._pos..].Span);
    }

    private void EnsureSize(int newCapacity)
    {
        var cap = this._buff.Length;
        if (cap >= newCapacity)
            return;

        var factor = newCapacity / cap;
        if (newCapacity % cap != 0)
            ++factor;

        var newActualCapacity = cap * factor;

        var newBuffOwner = this._pool.Rent(newActualCapacity);
        var newBuff = newBuffOwner.Memory;

        this._buff.Span.CopyTo(newBuff.Span);
        if (this._clear)
            this._buff.Span.Clear();

        this._buffOwner.Dispose();
        this._buffOwner = newBuffOwner;
        this._buff = newBuff;
    }
}
