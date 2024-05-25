// This file is a part of Emzi0767.Common project.
// 
// Copyright (C) 2020-2021 Emzi0767
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using Emzi0767.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests
{
    [TestClass]
    public sealed class ReflectionTests
    {
        private class TestClass0
        {
            public bool IsInitialized { get; } = true;
        }

        private class TestClass1
        {
            public int Value { get; }

            public TestClass1(int value)
            {
                if (value == 0)
                    value = 1;

                if (value > 100)
                    value = 100;

                if (value < -100)
                    value = -100;

                this.Value = value + 1;
            }
        }

        [TestMethod]
        public void TestEmptyObjects()
        {
            // Create empty objects
            var o0 = ReflectionUtilities.CreateEmpty<TestClass0>();
            var o1 = ReflectionUtilities.CreateEmpty<TestClass1>();

            var p0 = typeof(TestClass0).CreateEmpty() as TestClass0;
            var p1 = typeof(TestClass1).CreateEmpty() as TestClass1;

            // Test that they are empty
            Assert.IsFalse(o0.IsInitialized);
            Assert.AreEqual(0, o1.Value);

            // Test that nongeneric variants created appropriate types
            Assert.IsNotNull(p0);
            Assert.IsInstanceOfType(p0, typeof(TestClass0));

            Assert.IsNotNull(p1);
            Assert.IsInstanceOfType(p1, typeof(TestClass1));

            // Test that the objects are empty
            Assert.IsFalse(p0.IsInitialized);
            Assert.AreEqual(0, p1.Value);
        }

        [TestMethod]
        public void TestDictionaryConversion()
        {
            var x = new { Text = "Behold, an anonymous object", Number = 42 };
            var y = new { Text = "Another anonymous object", Number = 69 };
            var z = new TestClass0();
            var w = new TestClass1(42);
            var v = ReflectionUtilities.CreateEmpty<TestClass0>();

            var dxf = x.ToDictionary(useCachedModel: true);
            var dyf = y.ToDictionary(useCachedModel: true);
            var dzf = z.ToDictionary(useCachedModel: true);
            var dwf = w.ToDictionary(useCachedModel: true);
            var dvf = v.ToDictionary(useCachedModel: true);

            var dxs = x.ToDictionary(useCachedModel: false);
            var dys = y.ToDictionary(useCachedModel: false);
            var dzs = z.ToDictionary(useCachedModel: false);
            var dws = w.ToDictionary(useCachedModel: false);
            var dvs = v.ToDictionary(useCachedModel: false);

            Assert.AreEqual(x.Text, dxf["Text"]);
            Assert.AreEqual(y.Text, dyf["Text"]);
            Assert.AreNotEqual(x.Text, dyf["Text"]);
            Assert.AreEqual(x.Number, dxf["Number"]);
            Assert.AreEqual(y.Number, dyf["Number"]);
            Assert.AreNotEqual(x.Number, dyf["Number"]);
            Assert.AreNotEqual(dxf["Text"], dxf["Number"]);
            Assert.AreEqual(true, dzf["IsInitialized"]);
            Assert.AreEqual(43, dwf["Value"]);
            Assert.AreEqual(false, dvf["IsInitialized"]);

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < 1000; ++i)
                x.ToDictionary(useCachedModel: true);
            sw.Stop();
            var tFast = sw.ElapsedTicks;

            sw.Restart();
            for (var i = 0; i < 1000; ++i)
                x.ToDictionary(useCachedModel: false);
            sw.Stop();
            var tSlow = sw.ElapsedTicks;

            Assert.IsTrue(tFast < tSlow);
        }

        [TestMethod]
        public void TestDictionaryAnonymousConversion()
        {
            var id = Guid.NewGuid();
            var x = new { id };
            var y = x.ToDictionary();

            Assert.AreEqual(id, y["id"]);
        }
    }
}
