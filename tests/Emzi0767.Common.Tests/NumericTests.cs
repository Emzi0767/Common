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
