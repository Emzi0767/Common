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

using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// Assortment of various extension and utility methods, designed to make working with various types a little easier.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// <para>Deconstructs a <see cref="Dictionary{TKey, TValue}"/> key-value pair item (<see cref="KeyValuePair{TKey, TValue}"/>) into 2 separate variables.</para>
        /// <para>This allows for enumerating over dictionaries in foreach blocks by using a (k, v) tuple as the enumerator variable, instead of having to use a <see cref="KeyValuePair{TKey, TValue}"/> directly.</para>
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary item key.</typeparam>
        /// <typeparam name="TValue">Type of dictionary item value.</typeparam>
        /// <param name="kvp">Key-value pair to deconstruct.</param>
        /// <param name="key">Deconstructed key.</param>
        /// <param name="value">Deconstructed value.</param>
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }
}
