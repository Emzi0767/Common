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
using System.IO;
using Emzi0767.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests
{
    [TestClass]
    public sealed class MemoryBufferTests
    {
        private SecureRandom RNG { get; } = new SecureRandom();

        [DataTestMethod]
        [DataRow(32 * 1024, 16 * 1024)]
        [DataRow(32 * 1024, 32 * 1024)]
        [DataRow(32 * 1024, 64 * 1024)]
        [DataRow(16 * 1024, 64 * 1024)]
        [DataRow(128 * 1024, 64 * 1024)]
        [DataRow(512 * 1024, 64 * 1024)]
        public void TestStraightWrite(int size, int segment)
        {
            var data = new byte[size];
            var datas = data.AsSpan();
            this.RNG.GetBytes(datas);

            using (var buff = new MemoryBuffer(segment))
            {
                buff.Write(datas);

                Assert.IsTrue(buff.Capacity >= (ulong)size);
                Assert.AreEqual((ulong)size, buff.Length);
            }
        }

        [DataTestMethod]
        [DataRow(128 * 1024, 16 * 1024, 16)]
        [DataRow(128 * 1024, 16 * 1024, 384)]
        [DataRow(128 * 1024, 16 * 1024, 512)]
        [DataRow(128 * 1024, 16 * 1024, 1024)]
        [DataRow(128 * 1024, 16 * 1024, 4096)]
        public void TestPartialWrite(int size, int segment, int chunk)
        {
            var data = new byte[size];
            var datas = data.AsSpan();
            this.RNG.GetBytes(datas);

            using (var buff = new MemoryBuffer(segment))
            {
                for (var i = 0; i < size; i += chunk)
                    buff.Write(datas.Slice(i, Math.Min(chunk, datas.Length - i)));

                Assert.IsTrue(buff.Capacity >= (ulong)size);
                Assert.AreEqual((ulong)size, buff.Length);
            }
        }

        [DataTestMethod]
        [DataRow(128 * 1024, 8 * 1024)]
        [DataRow(128 * 1024, 16 * 1024)]
        [DataRow(128 * 1024, 64 * 1024)]
        [DataRow(128 * 1024, 128 * 1024)]
        public void TestArrayConversion(int size, int segment)
        {
            var data = new byte[size];
            var datas = data.AsSpan();
            this.RNG.GetBytes(datas);

            using (var buff = new MemoryBuffer(segment))
            {
                buff.Write(datas);

                Assert.IsTrue(buff.Capacity >= (ulong)size);
                Assert.AreEqual((ulong)size, buff.Length);

                var readout = buff.ToArray();
                var readouts = readout.AsSpan();
                Assert.AreEqual(size, readout.Length);
                Assert.IsTrue(readouts.SequenceEqual(datas));
            }
        }

        [DataTestMethod]
        [DataRow(128 * 1024, 8 * 1024, 12 * 1024, 3UL * 1024)]
        [DataRow(128 * 1024, 16 * 1024, 24 * 1024, 32UL * 1024)]
        [DataRow(128 * 1024, 64 * 1024, 4 * 1024, 66UL * 1024)]
        [DataRow(128 * 1024, 128 * 1024, 1 * 1024, 1UL * 1024)]
        public void TestPartialReads(int size, int segment, int arraySize, ulong start)
        {
            var data = new byte[size];
            var datas = data.AsSpan();
            this.RNG.GetBytes(datas);

            using (var buff = new MemoryBuffer(segment))
            {
                buff.Write(datas);

                Assert.IsTrue(buff.Capacity >= (ulong)size);
                Assert.AreEqual((ulong)size, buff.Length);

                var readout = new byte[arraySize];
                var readouts = readout.AsSpan();
                buff.Read(readouts, start, out var written);
                Assert.AreEqual(arraySize, written);
                Assert.IsTrue(readouts.SequenceEqual(datas.Slice((int)start, arraySize)));
            }
        }

        [DataTestMethod]
        [DataRow(128 * 1024, 8 * 1024)]
        [DataRow(128 * 1024, 16 * 1024)]
        [DataRow(128 * 1024, 64 * 1024)]
        [DataRow(128 * 1024, 128 * 1024)]
        public void TestStreamWrite(int size, int segment)
        {
            var data = new byte[size];
            var datas = data.AsSpan();
            this.RNG.GetBytes(datas);

            using (var buff = new MemoryBuffer(segment))
            {
                buff.Write(datas);

                Assert.IsTrue(buff.Capacity >= (ulong)size);
                Assert.AreEqual((ulong)size, buff.Length);

                using (var ms = new MemoryStream())
                {
                    buff.CopyTo(ms);
                    Assert.AreEqual((long)buff.Length, ms.Length);

                    var readout = ms.ToArray();
                    var readouts = readout.AsSpan();
                    Assert.AreEqual(size, readout.Length);
                    Assert.IsTrue(readouts.SequenceEqual(datas));
                }
            }
        }
    }
}
