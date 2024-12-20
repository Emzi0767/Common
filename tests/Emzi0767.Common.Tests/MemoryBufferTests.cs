﻿// This file is part of Emzi0767.Common project.
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
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Emzi0767.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests;

[TestClass]
public sealed class MemoryBufferTests
{
    private SecureRandom RNG { get; } = new SecureRandom();

    private const int _bufferStandard = 0;
    private const int _bufferContinuous = 1;

    private static IMemoryBuffer<T> CreateMemoryBuffer<T>(int type, int size)
        where T : unmanaged
        => type switch
        {
            _bufferStandard => new MemoryBuffer<T>(size),
            _bufferContinuous => new ContinuousMemoryBuffer<T>(size),

            _ => null
        };

    [DataTestMethod]
    [DataRow(32 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(32 * 1024, 32 * 1024, _bufferStandard)]
    [DataRow(32 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(16 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(512 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(32 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(32 * 1024, 32 * 1024, _bufferContinuous)]
    [DataRow(32 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(16 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(512 * 1024, 64 * 1024, _bufferContinuous)]
    public void TestStraightWrite(int size, int segment, int type)
    {
        var data = new byte[size];
        var datas = data.AsSpan();
        var datat = new byte[size];
        this.RNG.GetBytes(datas);

        using var buff = CreateMemoryBuffer<byte>(type, segment);
        buff.Write(datas);

        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Length);

        buff.Read(datat, 0, out var written);

        Assert.IsTrue(datat.AsSpan().SequenceEqual(datas));
        Assert.AreEqual((ulong)written, buff.Length);
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 16 * 1024, 16, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 384, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 512, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 4096, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 16, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, 384, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, 512, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, 4096, _bufferContinuous)]
    public void TestPartialWrite(int size, int segment, int chunk, int type)
    {
        var data = new byte[size];
        var datas = data.AsSpan();
        var datat = new byte[size];
        this.RNG.GetBytes(datas);

        using var buff = CreateMemoryBuffer<byte>(type, segment);
        for (var i = 0; i < size; i += chunk)
            buff.Write(datas.Slice(i, Math.Min(chunk, datas.Length - i)));

        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Length);

        buff.Read(datat, 0, out var written);

        Assert.IsTrue(datat.AsSpan().SequenceEqual(datas));
        Assert.AreEqual((ulong)written, buff.Length);
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, _bufferContinuous)]
    public void TestArrayConversion(int size, int segment, int type)
    {
        var data = new byte[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(datas);

        using var buff = CreateMemoryBuffer<byte>(type, segment);
        buff.Write(datas);

        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Length);

        var readout = buff.ToArray();
        var readouts = readout.AsSpan();
        Assert.AreEqual(size, readout.Length);
        Assert.IsTrue(readouts.SequenceEqual(datas));
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024, 12 * 1024, 3UL * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 24 * 1024, 32UL * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, 4 * 1024, 66UL * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, 1 * 1024, 1UL * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, 12 * 1024, 3UL * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, 24 * 1024, 32UL * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, 4 * 1024, 66UL * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, 1 * 1024, 1UL * 1024, _bufferContinuous)]
    public void TestPartialReads(int size, int segment, int arraySize, ulong start, int type)
    {
        var data = new byte[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(datas);

        using var buff = CreateMemoryBuffer<byte>(type, segment);
        buff.Write(datas);

        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Length);

        var readout = new byte[arraySize];
        var readouts = readout.AsSpan();
        buff.Read(readouts, start, out var written);
        Assert.AreEqual(arraySize, written);
        Assert.IsTrue(readouts.SequenceEqual(datas.Slice((int)start, arraySize)));
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, _bufferContinuous)]
    public void TestStreamCopy(int size, int segment, int type)
    {
        var data = new byte[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(datas);

        using var buff = CreateMemoryBuffer<byte>(type, segment);
        buff.Write(datas);

        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Length);

        using var ms = new MemoryStream();
        buff.CopyTo(ms);
        Assert.AreEqual((long)buff.Length, ms.Length);

        var readout = ms.ToArray();
        var readouts = readout.AsSpan();
        Assert.AreEqual(size, readout.Length);
        Assert.IsTrue(readouts.SequenceEqual(datas));
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, _bufferContinuous)]
    public void TestStreamWrite(int size, int segment, int type)
    {
        var data = new byte[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(datas);

        using var ms = new MemoryStream();
        ms.Write(data, 0, data.Length);
        ms.Position = 0;

        using var buff = CreateMemoryBuffer<byte>(type, segment);
        buff.Write(ms);

        Assert.AreEqual((long)buff.Length, ms.Length);

        var readout = buff.ToArray();
        var readouts = readout.AsSpan();
        Assert.AreEqual(size, readout.Length);
        Assert.IsTrue(readouts.SequenceEqual(datas));
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, _bufferContinuous)]
    public void TestUnseekableStreamWrite(int size, int segment, int type)
    {
        var data = new byte[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(datas);

        using var ms = new MemoryStream();
        using (var gzc = new GZipStream(ms, CompressionLevel.Optimal, true))
        {
            gzc.Write(data, 0, data.Length);
            gzc.Flush();
        }

        ms.Position = 0;

        using var gzd = new GZipStream(ms, CompressionMode.Decompress, true);
        using var buff = CreateMemoryBuffer<byte>(type, segment);
        buff.Write(gzd);

        var readout = buff.ToArray();
        var readouts = readout.AsSpan();
        Assert.AreEqual(size, readout.Length);
        Assert.IsTrue(readouts.SequenceEqual(datas));
    }

    [DataTestMethod]
    [DataRow(1024, 512, 1024, _bufferStandard)]
    [DataRow(1024, 512, 2048, _bufferStandard)]
    [DataRow(2048, 1024, 1024, _bufferStandard)]
    [DataRow(2048, 1536, 1024, _bufferStandard)]
    [DataRow(1024, 512, 1024, _bufferContinuous)]
    [DataRow(1024, 512, 2048, _bufferContinuous)]
    [DataRow(2048, 1024, 1024, _bufferContinuous)]
    [DataRow(2048, 1536, 1024, _bufferContinuous)]
    public void TestReuse(int size1, int size2, int segment, int type)
    {
        var data = new byte[size1];
        var datas = data.AsSpan();
        this.RNG.GetBytes(datas);

        var readout = new byte[size1];

        using var ms = new MemoryStream();
        ms.Write(data, 0, data.Length);
        ms.Position = 0;

        using (var buff = CreateMemoryBuffer<byte>(type, segment))
        {
            buff.Write(ms);

            Assert.AreEqual((long)buff.Length, ms.Length);

            var readouts = readout.AsSpan();
            buff.Read(readout.AsSpan(), 0, out var written);
            Assert.AreEqual(size1, written);
            Assert.IsTrue(readouts.SequenceEqual(datas));
        }

        ms.Position = 0;
        ms.SetLength(0);
        ms.Write(data, 0, size2);
        ms.Position = 0;

        using (var buff = CreateMemoryBuffer<byte>(type, segment))
        {
            buff.Write(ms);

            Assert.AreEqual((long)buff.Length, ms.Length);

            var readouts = readout.AsSpan();
            buff.Read(readout.AsSpan(), 0, out var written);
            Assert.AreEqual(size2, written);
            Assert.IsTrue(readouts[..size2].SequenceEqual(datas[..size2]));
        }
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, _bufferContinuous)]
    public void TestClearing(int size, int segment, int type)
    {
        var data = new byte[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(datas);

        using var buff = CreateMemoryBuffer<byte>(type, segment);
        buff.Write(datas);

        var oldcap = buff.Capacity;
        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Length);

        var readout = buff.ToArray();
        var readouts = readout.AsSpan();
        Assert.AreEqual(size, readout.Length);
        Assert.IsTrue(readouts.SequenceEqual(datas));

        buff.Clear();

        Assert.AreEqual(oldcap, buff.Capacity);
        Assert.AreEqual(0UL, buff.Length);

        readout = buff.ToArray();
        Assert.AreEqual(0, readout.Length);
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, _bufferContinuous)]
    public void TestArrayConversionI16(int size, int segment, int type)
    {
        var data = new short[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(MemoryMarshal.AsBytes(datas));

        using var buff = CreateMemoryBuffer<short>(type, segment);
        buff.Write(datas);

        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Count);

        var readout = buff.ToArray();
        var readouts = readout.AsSpan();
        Assert.AreEqual(size, readout.Length);
        Assert.IsTrue(readouts.SequenceEqual(datas));
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, _bufferContinuous)]
    public void TestArrayConversionI32(int size, int segment, int type)
    {
        var data = new int[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(MemoryMarshal.AsBytes(datas));

        using var buff = CreateMemoryBuffer<int>(type, segment);
        buff.Write(datas);

        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Count);

        var readout = buff.ToArray();
        var readouts = readout.AsSpan();
        Assert.AreEqual(size, readout.Length);
        Assert.IsTrue(readouts.SequenceEqual(datas));
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, _bufferContinuous)]
    public void TestArrayConversionI64(int size, int segment, int type)
    {
        var data = new long[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(MemoryMarshal.AsBytes(datas));

        using var buff = CreateMemoryBuffer<long>(type, segment);
        buff.Write(datas);

        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Count);

        var readout = buff.ToArray();
        var readouts = readout.AsSpan();
        Assert.AreEqual(size, readout.Length);
        Assert.IsTrue(readouts.SequenceEqual(datas));
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, _bufferContinuous)]
    public void TestArrayConversionLarge(int size, int segment, int type)
    {
        var data = new ComplexType[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(MemoryMarshal.AsBytes(datas));

        using var buff = CreateMemoryBuffer<ComplexType>(type, segment);
        buff.Write(datas);

        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Count);

        var readout = buff.ToArray();
        var readouts = readout.AsSpan();
        Assert.AreEqual(size, readout.Length);
        Assert.IsTrue(readouts.SequenceEqual(datas));
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024)]
    [DataRow(128 * 1024, 16 * 1024)]
    [DataRow(128 * 1024, 64 * 1024)]
    [DataRow(128 * 1024, 128 * 1024)]
    public void TestUnderlyingBufferSpan(int size, int segment)
    {
        var data = new ComplexType[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(MemoryMarshal.AsBytes(datas));

        using var buff = CreateMemoryBuffer<ComplexType>(_bufferContinuous, segment) as ContinuousMemoryBuffer<ComplexType>;
        buff.Write(datas);

        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Count);

        var typedReadout = buff.Span;
        var rawReadout = buff.ByteSpan;
        Assert.AreEqual((ulong)typedReadout.Length, buff.Count);
        Assert.AreEqual((ulong)rawReadout.Length, buff.Length);
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024, 4 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, 6 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 12 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 32 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, 32 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, 48 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, 32 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, 48 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, 4 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 8 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 8 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, 32 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, 32 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, 128 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, 32 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, 128 * 1024, _bufferContinuous)]
    public void TestBufferWriterInterfaceSpan(int size, int segment, int chunk, int type)
    {
        var data = new ComplexType[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(MemoryMarshal.AsBytes(datas));

        using var buff = CreateMemoryBuffer<ComplexType>(type, segment);
        for (var i = 0; i < size; i += chunk)
        {
            var actual = i + chunk <= datas.Length ? chunk : datas.Length - i;
            var span = buff.GetSpan(actual);
            datas.Slice(i, actual).CopyTo(span);
            buff.Advance(actual);
        }

        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Count);

        var readout = buff.ToArray();
        var readouts = readout.AsSpan();
        Assert.AreEqual(size, readout.Length);
        Assert.IsTrue(readouts.SequenceEqual(datas));
    }

    [DataTestMethod]
    [DataRow(128 * 1024, 8 * 1024, 4 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, 6 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 8 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 12 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 16 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 16 * 1024, 32 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, 32 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, 48 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 64 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, 32 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, 48 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, 64 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 128 * 1024, 128 * 1024, _bufferStandard)]
    [DataRow(128 * 1024, 8 * 1024, 4 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 8 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 8 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, 8 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, 16 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 16 * 1024, 32 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, 32 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 64 * 1024, 128 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, 32 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, 64 * 1024, _bufferContinuous)]
    [DataRow(128 * 1024, 128 * 1024, 128 * 1024, _bufferContinuous)]
    public void TextMixedModeWrites(int size, int segment, int chunk, int type)
    {
        var data = new ComplexType[size];
        var datas = data.AsSpan();
        this.RNG.GetBytes(MemoryMarshal.AsBytes(datas));

        using var buff = CreateMemoryBuffer<ComplexType>(type, segment);
        var counter = 0;
        for (var i = 0; i < size; i += chunk)
        {
            var actual = i + chunk <= datas.Length ? chunk : datas.Length - i;
            var src = datas.Slice(i, actual);
            if (counter % 2 == 0)
            {
                var span = buff.GetSpan(actual);
                src.CopyTo(span);
                buff.Advance(actual);
            }
            else
            {
                buff.Write(src);
            }

            ++counter;
        }

        Assert.IsTrue(buff.Capacity >= (ulong)size);
        Assert.AreEqual((ulong)size, buff.Count);

        var readout = buff.ToArray();
        var readouts = readout.AsSpan();
        Assert.AreEqual(size, readout.Length);
        Assert.IsTrue(readouts.SequenceEqual(datas));
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct ComplexType : IEquatable<ComplexType>
    {
        public double FirstValue { get; set; }
        public long SecondValue { get; set; }

        public string DebuggerDisplay => $"{this.FirstValue}; {this.SecondValue}";

        public bool Equals(ComplexType other)
            => ((double.IsNaN(this.FirstValue) && double.IsNaN(other.FirstValue)) || this.FirstValue == other.FirstValue) && this.SecondValue == other.SecondValue;
    }
}
