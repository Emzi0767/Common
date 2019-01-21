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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests
{
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
}
