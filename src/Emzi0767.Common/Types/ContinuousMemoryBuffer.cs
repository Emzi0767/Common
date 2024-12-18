// This file is part of Emzi0767.Common project.
//
// Copyright © 2020-2024 Emzi0767
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

namespace Emzi0767.Types;

/// <summary>
/// Provides a resizable memory buffer analogous to <see cref="MemoryBuffer{T}"/>, using a single continuous memory region instead.
/// </summary>
/// <typeparam name="T">Type of item to hold in the buffer.</typeparam>
public sealed class ContinuousMemoryBuffer<T> : IMemoryBuffer<T> where T : unmanaged
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
    public ReadOnlySpan<T> Span => MemoryMarshal.Cast<byte, T>(this._buff.Span[..this._buff.Length]);

    /// <summary>
    /// Gets a byte view of the underlying buffer, bounded by total number of bytes written so far.
    /// </summary>
    public ReadOnlySpan<byte> ByteSpan => this._buff.Span[..this._buff.Length];

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
            throw new ObjectDisposedException("This buffer is disposed.");

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
            throw new ObjectDisposedException("This buffer is disposed.");

        if (stream.CanSeek)
            this.WriteStreamSeekable(stream);
        else
            this.WriteStreamUnseekable(stream);
    }

    private void WriteStreamSeekable(Stream stream)
    {
        if (stream.Length > int.MaxValue)
            throw new ArgumentException("Stream is too long.", nameof(stream));

        this.EnsureSize(this._pos + (int)stream.Length);
        stream.Read(this._buff[this._pos..].Span);

        this._pos += (int)stream.Length;
    }

    private void WriteStreamUnseekable(Stream stream)
    {
        var br = 0;
        do
        {
            this.EnsureSize(this._pos + 4096);
            br = stream.Read(this._buff[this._pos..].Span);
            this._pos += br;
        }
        while (br != 0);
    }

    /// <inheritdoc />
    public bool Read(Span<T> destination, ulong source, out int itemsWritten)
    {
        itemsWritten = 0;
        if (this._isDisposed)
            throw new ObjectDisposedException("This buffer is disposed.");

        source *= (ulong)this._itemSize;
        if (source > this.Count)
            throw new ArgumentOutOfRangeException(nameof(source), "Cannot copy data from beyond the buffer.");

        var start = (int)source;
        var sbuff = this._buff[start..this._pos ].Span;
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
    public T[] ToArray()
    {
        if (this._isDisposed)
            throw new ObjectDisposedException("This buffer is disposed.");

        return MemoryMarshal.Cast<byte, T>(this._buff[..this._pos].Span).ToArray();
    }

    /// <inheritdoc />
    public void CopyTo(Stream destination)
    {
        if (this._isDisposed)
            throw new ObjectDisposedException("This buffer is disposed.");

        destination.Write(this._buff[..this._pos].Span);
    }

    /// <inheritdoc />
    public void Clear()
    {
        if (this._isDisposed)
            throw new ObjectDisposedException("This buffer is disposed.");

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
