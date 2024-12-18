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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests;

[TestClass]
public sealed class ExtensionTests
{
    [TestMethod]
    public void KeyValueDeconstructionTests()
    {
        // Create a dictionary
        var dict = new Dictionary<int, string>()
        {
            [42] = "42",
            [69] = "69",
            [142] = "142",
            [169] = "169",
            [420] = "420"
        };

        // Create target key list and reference key list
        var keys = new List<int>();
        var keySeq = new[] { 42, 69, 142, 169, 420 };

        // Create target value list and reference value list
        var values = new List<string>();
        var valueSeq = new[] { "42", "69", "142", "169", "420" };

        // Iterate over deconstructed key-value pairs
        foreach (var (k, v) in dict)
        {
            keys.Add(k);
            values.Add(v);

            Assert.AreEqual(v, k.ToString());
        }

        // Test that sequences match
        Assert.IsTrue(keys.SequenceEqual(keySeq));
        Assert.IsTrue(values.SequenceEqual(valueSeq));
    }
}
