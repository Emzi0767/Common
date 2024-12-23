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
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.Types;

/// <summary>
/// An interface describing the API of resizable memory buffers, such as <see cref="MemoryBuffer{T}"/> and <see cref="ContinuousMemoryBuffer{T}"/>.
/// </summary>
/// <typeparam name="T">Type of item to hold in the buffer.</typeparam>
public interface IMemoryBuffer<T> : IBufferWriter<T>, IDisposable
    where T : unmanaged
{
    /// <summary>
    /// Gets the total capacity of this buffer. The capacity is the number of segments allocated, multiplied by size of individual segment.
    /// </summary>
    ulong Capacity { get; }

    /// <summary>
    /// Gets the amount of bytes currently written to the buffer. This number is never greather than <see cref="Capacity"/>.
    /// </summary>
    ulong Length { get; }

    /// <summary>
    /// Gets the number of items currently written to the buffer. This number is equal to <see cref="Length"/> divided by size of <typeparamref name="T"/>.
    /// </summary>
    ulong Count { get; }

    /// <summary>
    /// Appends data from a supplied buffer to this buffer, growing it if necessary.
    /// </summary>
    /// <param name="data">Buffer containing data to write.</param>
    void Write(ReadOnlySpan<T> data);

    /// <summary>
    /// Appends data from a supplied array to this buffer, growing it if necessary.
    /// </summary>
    /// <param name="data">Array containing data to write.</param>
    /// <param name="start">Index from which to start reading the data.</param>
    /// <param name="count">Number of bytes to read from the source.</param>
    void Write(T[] data, int start, int count);

    /// <summary>
    /// Appends data from a supplied array slice to this buffer, growing it if necessary.
    /// </summary>
    /// <param name="data">Array slice containing data to write.</param>
    void Write(ArraySegment<T> data);

    /// <summary>
    /// Appends data from a supplied stream to this buffer, growing it if necessary.
    /// </summary>
    /// <param name="stream">Stream to copy data from.</param>
    void Write(Stream stream);

    /// <summary>
    /// Appends data from a supplied stream to this buffer, growing it if necessary. This method will copy at most
    /// <paramref name="limit"/> bytes.
    /// </summary>
    /// <param name="stream">Stream to copy data from.</param>
    /// <param name="limit">Maximum number of bytes to copy.</param>
    void Write(Stream stream, ulong limit);

    /// <summary>
    /// Asynchronously appends data from a supplied stream to this buffer, growing it if necessary.
    /// </summary>
    /// <param name="stream">Stream to copy data from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the operation.</returns>
    ValueTask WriteAsync(Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously appends data from a supplied stream to this buffer, growing it if necessary. This method will
    /// copy at most <paramref name="limit"/> bytes.
    /// </summary>
    /// <param name="stream">Stream to copy data from.</param>
    /// <param name="limit">A token to cancel the operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the operation.</returns>
    ValueTask WriteAsync(Stream stream, ulong limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads data from this buffer to the specified destination buffer. This method will write either as many
    /// bytes as there are in the destination buffer, or however many bytes are available in this buffer,
    /// whichever is less.
    /// </summary>
    /// <param name="destination">Buffer to read the data from this buffer into.</param>
    /// <param name="source">Starting position in this buffer to read from.</param>
    /// <param name="itemsWritten">Number of items written to the destination buffer.</param>
    /// <returns>Whether more data is available in this buffer.</returns>
    bool Read(Span<T> destination, ulong source, out int itemsWritten);

    /// <summary>
    /// Reads data from this buffer to specified destination array. This method will write either as many bytes
    /// as specified for the destination array, or however many bytes are available in this buffer, whichever is
    /// less.
    /// </summary>
    /// <param name="data">Array to read the data from this buffer into.</param>
    /// <param name="start">Starting position in the target array to write to.</param>
    /// <param name="count">Maximum number of bytes to write to target array.</param>
    /// <param name="source">Starting position in this buffer to read from.</param>
    /// <param name="itemsWritten">Number of items written to the destination buffer.</param>
    /// <returns>Whether more data is available in this buffer.</returns>
    bool Read(T[] data, int start, int count, ulong source, out int itemsWritten);

    /// <summary>
    /// Reads data from this buffer to specified destination array slice. This method will write either as many
    /// bytes as specified in the target slice, or however many bytes are available in this buffer, whichever is
    /// less.
    /// </summary>
    /// <param name="data">Array segment to read the data from this buffer into.</param>
    /// <param name="source">Starting position in this buffer to read from.</param>
    /// <param name="itemsWritten">Number of items written to the destination buffer.</param>
    /// <returns>Whether more data is available in this buffer.</returns>
    bool Read(ArraySegment<T> data, ulong source, out int itemsWritten);

    /// <summary>
    /// Reads data from this buffer to specified stream. This method will write all bytes in the target slice.
    /// </summary>
    /// <param name="stream">Stream to write the data from the buffer slice to.</param>
    /// <param name="source">Starting position in this buffer to read from.</param>
    /// <returns>Number of bytes written into the stream.</returns>
    ulong Read(Stream stream, ulong source);

    /// <summary>
    /// Reads data from this buffer to specified stream. This method will write all bytes in the target slice.
    /// </summary>
    /// <param name="stream">Stream to write the data from the buffer slice to.</param>
    /// <param name="source">Starting position in this buffer to read from.</param>
    /// <param name="count">Number of items in this buffer to write to the stream. Must not exceed buffer's size.</param>
    /// <returns>Number of bytes written into the stream.</returns>
    ulong Read(Stream stream, ulong source, ulong count);

    /// <summary>
    /// Asynchronously reads data from this buffer to specified stream. This method will write all bytes in the target
    /// slice.
    /// </summary>
    /// <param name="stream">Stream to write the data from the buffer slice to.</param>
    /// <param name="source">Starting position in this buffer to read from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Number of bytes written into the stream.</returns>
    ValueTask<ulong> ReadAsync(Stream stream, ulong source, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously reads data from this buffer to specified stream. This method will write all bytes in the target
    /// slice.
    /// </summary>
    /// <param name="stream">Stream to write the data from the buffer slice to.</param>
    /// <param name="source">Starting position in this buffer to read from.</param>
    /// <param name="count">Number of items in this buffer to write to the stream. Must not exceed buffer's size.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Number of bytes written into the stream.</returns>
    ValueTask<ulong> ReadAsync(Stream stream, ulong source, ulong count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Converts this buffer into a single continuous byte array.
    /// </summary>
    /// <returns>Converted byte array.</returns>
    T[] ToArray();

    /// <summary>
    /// Returns a view over this buffer's memory as a <see cref="ReadOnlySequence{T}"/>.
    /// </summary>
    /// <returns>Sequence view of this buffer.</returns>
    ReadOnlySequence<T> AsSequence();

    /// <summary>
    /// Returns a view over this buffer's memory as a <see cref="ReadOnlySequence{T}"/>, where T is <see cref="byte"/>.
    /// </summary>
    /// <returns>Byte sequence view of this buffer.</returns>
    ReadOnlySequence<byte> AsByteSequence();

    /// <summary>
    /// Copies all the data from this buffer to a stream.
    /// </summary>
    /// <param name="destination">Stream to copy this buffer's data to.</param>
    void CopyTo(Stream destination);

    /// <summary>
    /// Asynchronously copies all the data from this buffer to a stream.
    /// </summary>
    /// <param name="destination">Stream to copy this buffer's data to.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the operation.</returns>
    ValueTask CopyToAsync(Stream destination, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the buffer's pointer to the beginning, allowing for reuse.
    /// </summary>
    void Clear();
}
