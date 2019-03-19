// This file is a part of Emzi0767.Common project.
// 
// Copyright 2019 Emzi0767
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests
{
    [TestClass]
    public sealed class NumericTests
    {
        [TestMethod]
        public void NumberLengthTests()
        {
            Assert.AreEqual(3, ((byte)255).CalculateLength());
            Assert.AreEqual(1, 0.CalculateLength());
            Assert.AreEqual(3, (-42).CalculateLength());
            Assert.AreEqual(2, (-1).CalculateLength());
            Assert.AreEqual(20, ulong.MaxValue.CalculateLength());
            Assert.AreEqual(20, long.MinValue.CalculateLength());
            Assert.AreEqual(19, long.MaxValue.CalculateLength());
        }

        [TestMethod]
        public void NumberRangeTests()
        {
            Assert.IsTrue(42.IsInRange(-5, 69));
            Assert.IsTrue(42.IsInRange(0, 42));
            Assert.IsFalse(42.IsInRange(0, 42, false));
            Assert.IsTrue(1.5F.IsInRange(0.5F, 2F));
            Assert.IsTrue(4.2.IsInRange(0, 4.2));
            Assert.IsFalse(4.2.IsInRange(0, 4.2, false));
            Assert.IsTrue(42ul.IsInRange(42ul, 0ul));
            Assert.IsFalse(42ul.IsInRange(42ul, 0ul, false));
        }
    }
}
