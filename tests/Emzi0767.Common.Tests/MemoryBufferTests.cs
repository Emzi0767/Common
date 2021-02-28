// This file is a part of Emzi0767.Common project.
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
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using Emzi0767.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests
{
    [TestClass]
    public sealed class MemoryBufferTests
    {
        private SecureRandom RNG { get; } = new SecureRandom();

        private const int BufferStandard = 0;
        private const int BufferContinuous = 1;

        private static IMemoryBuffer<T> CreateMemoryBuffer<T>(int type, int size)
            where T : unmanaged
            => type switch
            {
                BufferStandard => new MemoryBuffer<T>(size),
                BufferContinuous => new ContinuousMemoryBuffer<T>(size),

                _ => null
            };

        [DataTestMethod]
        [DataRow(32 * 1024, 16 * 1024, BufferStandard)]
        [DataRow(32 * 1024, 32 * 1024, BufferStandard)]
        [DataRow(32 * 1024, 64 * 1024, BufferStandard)]
        [DataRow(16 * 1024, 64 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 64 * 1024, BufferStandard)]
        [DataRow(512 * 1024, 64 * 1024, BufferStandard)]
        [DataRow(32 * 1024, 16 * 1024, BufferContinuous)]
        [DataRow(32 * 1024, 32 * 1024, BufferContinuous)]
        [DataRow(32 * 1024, 64 * 1024, BufferContinuous)]
        [DataRow(16 * 1024, 64 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 64 * 1024, BufferContinuous)]
        [DataRow(512 * 1024, 64 * 1024, BufferContinuous)]
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
        [DataRow(128 * 1024, 16 * 1024, 16, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, 384, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, 512, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, 1024, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, 4096, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, 16, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, 384, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, 512, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, 1024, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, 4096, BufferContinuous)]
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
        [DataRow(128 * 1024, 8 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 64 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 128 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 8 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 64 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 128 * 1024, BufferContinuous)]
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
        [DataRow(128 * 1024, 8 * 1024, 12 * 1024, 3UL * 1024, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, 24 * 1024, 32UL * 1024, BufferStandard)]
        [DataRow(128 * 1024, 64 * 1024, 4 * 1024, 66UL * 1024, BufferStandard)]
        [DataRow(128 * 1024, 128 * 1024, 1 * 1024, 1UL * 1024, BufferStandard)]
        [DataRow(128 * 1024, 8 * 1024, 12 * 1024, 3UL * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, 24 * 1024, 32UL * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 64 * 1024, 4 * 1024, 66UL * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 128 * 1024, 1 * 1024, 1UL * 1024, BufferContinuous)]
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
        [DataRow(128 * 1024, 8 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 64 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 128 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 8 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 64 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 128 * 1024, BufferContinuous)]
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
        [DataRow(128 * 1024, 8 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 64 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 128 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 8 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 64 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 128 * 1024, BufferContinuous)]
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
        [DataRow(128 * 1024, 8 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 64 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 128 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 8 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 64 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 128 * 1024, BufferContinuous)]
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
        [DataRow(1024, 512, 1024, BufferStandard)]
        [DataRow(1024, 512, 2048, BufferStandard)]
        [DataRow(2048, 1024, 1024, BufferStandard)]
        [DataRow(2048, 1536, 1024, BufferStandard)]
        [DataRow(1024, 512, 1024, BufferContinuous)]
        [DataRow(1024, 512, 2048, BufferContinuous)]
        [DataRow(2048, 1024, 1024, BufferContinuous)]
        [DataRow(2048, 1536, 1024, BufferContinuous)]
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
                Assert.IsTrue(readouts.Slice(0, size2).SequenceEqual(datas.Slice(0, size2)));
            }
        }

        [DataTestMethod]
        [DataRow(128 * 1024, 8 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 64 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 128 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 8 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 64 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 128 * 1024, BufferContinuous)]
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
        [DataRow(128 * 1024, 8 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 64 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 128 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 8 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 64 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 128 * 1024, BufferContinuous)]
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
        [DataRow(128 * 1024, 8 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 64 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 128 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 8 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 64 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 128 * 1024, BufferContinuous)]
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
        [DataRow(128 * 1024, 8 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 16 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 64 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 128 * 1024, BufferStandard)]
        [DataRow(128 * 1024, 8 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 16 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 64 * 1024, BufferContinuous)]
        [DataRow(128 * 1024, 128 * 1024, BufferContinuous)]
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

        [DebuggerDisplay("{DebuggerDisplay,nq}")]
        public struct ComplexType : IEquatable<ComplexType>
        {
            public double FirstValue { get; set; }
            public long SecondValue { get; set; }

            public string DebuggerDisplay => $"{FirstValue}; {SecondValue}";

            public bool Equals(ComplexType other)
                => ((double.IsNaN(this.FirstValue) && double.IsNaN(other.FirstValue)) || this.FirstValue == other.FirstValue) && this.SecondValue == other.SecondValue;
        }
    }
}
