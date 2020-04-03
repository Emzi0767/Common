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

            var dx = x.ToDictionary();
            var dy = y.ToDictionary();
            var dz = z.ToDictionary();
            var dw = w.ToDictionary();
            var dv = v.ToDictionary();

            Assert.AreEqual(x.Text, dx["Text"]);
            Assert.AreEqual(y.Text, dy["Text"]);
            Assert.AreNotEqual(x.Text, dy["Text"]);
            Assert.AreEqual(x.Number, dx["Number"]);
            Assert.AreEqual(y.Number, dy["Number"]);
            Assert.AreNotEqual(x.Number, dy["Number"]);
            Assert.AreNotEqual(dx["Text"], dx["Number"]);
            Assert.AreEqual(true, dz["IsInitialized"]);
            Assert.AreEqual(43, dw["Value"]);
            Assert.AreEqual(false, dv["IsInitialized"]);
        }
    }
}
