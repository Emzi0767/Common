// This file is part of Emzi0767.Common project
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
using System.Collections.Immutable;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests
{
    [TestClass]
    public sealed class SpanLookupRoDictTests
    {
        private IEnumerable<(string Key, int Value)> TestValues { get; } = new[] { ("42", 42), ("69", 69) };
        private IEnumerable<(string Key, int Value)> NonTestValues { get; } = new[] { ("144", 144), ("0", 0), ("-3", -3) };

        [TestMethod]
        public void TestCreationFromAnother()
        {
            var baseDict = new Dictionary<string, int>();
            foreach (var (k, v) in this.TestValues)
                baseDict[k] = v;
            Assert.AreEqual(this.TestValues.Count(), baseDict.Count);
            foreach (var (k, v) in this.TestValues)
                Assert.IsTrue(baseDict.ContainsKey(k));
            foreach (var (k, v) in this.NonTestValues)
                Assert.IsFalse(baseDict.ContainsKey(k));

            var dict = new CharSpanLookupReadOnlyDictionary<int>(baseDict as IDictionary<string, int>);
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
            {
                Assert.IsTrue(dict.TryGetValue(k, out var rv));
                Assert.AreEqual(v, rv);

                Assert.IsTrue(dict.TryGetValue(k.AsSpan(), out rv));
                Assert.AreEqual(v, rv);
            }

            foreach (var (k, v) in this.NonTestValues)
            {
                Assert.IsFalse(dict.TryGetValue(k, out var rv));
                Assert.IsFalse(dict.TryGetValue(k.AsSpan(), out rv));
            }

            dict = new CharSpanLookupReadOnlyDictionary<int>(baseDict as IReadOnlyDictionary<string, int>);
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
            {
                Assert.IsTrue(dict.TryGetValue(k, out var rv));
                Assert.AreEqual(v, rv);

                Assert.IsTrue(dict.TryGetValue(k.AsSpan(), out rv));
                Assert.AreEqual(v, rv);
            }

            foreach (var (k, v) in this.NonTestValues)
            {
                Assert.IsFalse(dict.TryGetValue(k, out var rv));
                Assert.IsFalse(dict.TryGetValue(k.AsSpan(), out rv));
            }

            dict = new CharSpanLookupReadOnlyDictionary<int>(baseDict as IEnumerable<KeyValuePair<string, int>>);
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
            {
                Assert.IsTrue(dict.TryGetValue(k, out var rv));
                Assert.AreEqual(v, rv);

                Assert.IsTrue(dict.TryGetValue(k.AsSpan(), out rv));
                Assert.AreEqual(v, rv);
            }

            foreach (var (k, v) in this.NonTestValues)
            {
                Assert.IsFalse(dict.TryGetValue(k, out var rv));
                Assert.IsFalse(dict.TryGetValue(k.AsSpan(), out rv));
            }

            dict = new CharSpanLookupReadOnlyDictionary<int>(this.TestValues.Select(x => new KeyValuePair<string, int>(x.Key, x.Value)));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
            {
                Assert.IsTrue(dict.TryGetValue(k, out var rv));
                Assert.AreEqual(v, rv);

                Assert.IsTrue(dict.TryGetValue(k.AsSpan(), out rv));
                Assert.AreEqual(v, rv);
            }

            foreach (var (k, v) in this.NonTestValues)
            {
                Assert.IsFalse(dict.TryGetValue(k, out var rv));
                Assert.IsFalse(dict.TryGetValue(k.AsSpan(), out rv));
            }
        }

        [TestMethod]
        public void TestEnumeration()
        {
            var dict = new CharSpanLookupReadOnlyDictionary<int>(this.TestValues.Select(x => new KeyValuePair<string, int>(x.Key, x.Value)));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            var i = 0;
            foreach (var (k, v) in dict)
            {
                Assert.IsTrue(this.TestValues.Any(x => x.Key == k && x.Value == v));
                i++;
            }
            Assert.AreEqual(this.TestValues.Count(), i);
        }

        [TestMethod]
        public void TestKeysValues()
        {
            var dict = new CharSpanLookupReadOnlyDictionary<int>(this.TestValues.Select(x => new KeyValuePair<string, int>(x.Key, x.Value)));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            var keys = this.TestValues.Select(x => x.Key).ToImmutableArray();
            var vals = this.TestValues.Select(x => x.Value).ToImmutableArray();

            var i = 0;
            var dkeys = dict.Keys;
            Assert.IsNotNull(dkeys);
            Assert.AreEqual(dict.Count, dkeys.Count());
            foreach (var k in dkeys)
            {
                Assert.IsTrue(keys.Contains(k));
                i++;
            }
            Assert.AreEqual(dict.Count, i);

            i = 0;
            var dvals = dict.Values;
            Assert.IsNotNull(dvals);
            Assert.AreEqual(dict.Count, dvals.Count());
            foreach (var v in dvals)
            {
                Assert.IsTrue(vals.Contains(v));
                i++;
            }
            Assert.AreEqual(dict.Count, i);
        }
    }
}
