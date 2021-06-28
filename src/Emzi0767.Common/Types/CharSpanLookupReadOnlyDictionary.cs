// This file is part of Emzi0767.Common project
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Emzi0767
{
    /// <summary>
    /// Represents collection of string keys and <typeparamref name="TValue"/> values, allowing the use of <see cref="ReadOnlySpan{T}"/> for dictionary operations.
    /// </summary>
    /// <typeparam name="TValue">Type of items in this dictionary.</typeparam>
    public sealed class CharSpanLookupReadOnlyDictionary<TValue> : IReadOnlyDictionary<string, TValue>
    {
        /// <summary>
        /// Gets the collection of all keys present in this dictionary.
        /// </summary>
        public IEnumerable<string> Keys => this.GetKeysInternal();

        /// <summary>
        /// Gets the collection of all values present in this dictionary.
        /// </summary>
        public IEnumerable<TValue> Values => this.GetValuesInternal();

        /// <summary>
        /// Gets the total number of items in this dictionary.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets a value corresponding to given key in this dictionary.
        /// </summary>
        /// <param name="key">Key to get or set the value for.</param>
        /// <returns>Value matching the supplied key, if applicable.</returns>
        public TValue this[string key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                if (!this.TryRetrieveInternal(key.AsSpan(), out var value))
                    throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");

                return value;
            }
        }

        /// <summary>
        /// Gets a value corresponding to given key in this dictionary.
        /// </summary>
        /// <param name="key">Key to get or set the value for.</param>
        /// <returns>Value matching the supplied key, if applicable.</returns>
        public TValue this[ReadOnlySpan<char> key]
        {
            get
            {
                if (!this.TryRetrieveInternal(key, out var value))
                    throw new KeyNotFoundException($"The given key was not present in the dictionary.");

                return value;
            }
        }

        private IReadOnlyDictionary<ulong, KeyedValue> InternalBuckets { get; }

        /// <summary>
        /// Creates a new <see cref="CharSpanLookupReadOnlyDictionary{TValue}"/> with string keys and items of type <typeparamref name="TValue"/> and populates it with key-value pairs from supplied dictionary.
        /// </summary>
        /// <param name="values">Dictionary containing items to populate this dictionary with.</param>
        public CharSpanLookupReadOnlyDictionary(IDictionary<string, TValue> values)
            : this(values as IEnumerable<KeyValuePair<string, TValue>>)
        { }

        /// <summary>
        /// Creates a new <see cref="CharSpanLookupReadOnlyDictionary{TValue}"/> with string keys and items of type <typeparamref name="TValue"/> and populates it with key-value pairs from supplied dictionary.
        /// </summary>
        /// <param name="values">Dictionary containing items to populate this dictionary with.</param>
        public CharSpanLookupReadOnlyDictionary(IReadOnlyDictionary<string, TValue> values)
            : this(values as IEnumerable<KeyValuePair<string, TValue>>)
        { }

        /// <summary>
        /// Creates a new <see cref="CharSpanLookupReadOnlyDictionary{TValue}"/> with string keys and items of type <typeparamref name="TValue"/> and populates it with key-value pairs from supplied key-value collection.
        /// </summary>
        /// <param name="values">Dictionary containing items to populate this dictionary with.</param>
        public CharSpanLookupReadOnlyDictionary(IEnumerable<KeyValuePair<string, TValue>> values)
        {
            this.InternalBuckets = PrepareItems(values, out var count);
            this.Count = count;
        }

        /// <summary>
        /// Attempts to retrieve a value corresponding to the supplied key from this dictionary.
        /// </summary>
        /// <param name="key">Key to retrieve the value for.</param>
        /// <param name="value">Retrieved value.</param>
        /// <returns>Whether the operation was successful.</returns>
        public bool TryGetValue(string key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return this.TryRetrieveInternal(key.AsSpan(), out value);
        }

        /// <summary>
        /// Attempts to retrieve a value corresponding to the supplied key from this dictionary.
        /// </summary>
        /// <param name="key">Key to retrieve the value for.</param>
        /// <param name="value">Retrieved value.</param>
        /// <returns>Whether the operation was successful.</returns>
        public bool TryGetValue(ReadOnlySpan<char> key, out TValue value)
            => this.TryRetrieveInternal(key, out value);

        /// <summary>
        /// Checks whether this dictionary contains the specified key.
        /// </summary>
        /// <param name="key">Key to check for in this dictionary.</param>
        /// <returns>Whether the key was present in the dictionary.</returns>
        public bool ContainsKey(string key)
            => this.ContainsKeyInternal(key.AsSpan());

        /// <summary>
        /// Checks whether this dictionary contains the specified key.
        /// </summary>
        /// <param name="key">Key to check for in this dictionary.</param>
        /// <returns>Whether the key was present in the dictionary.</returns>
        public bool ContainsKey(ReadOnlySpan<char> key)
            => this.ContainsKeyInternal(key);

        /// <summary>
        /// Gets an enumerator over key-value pairs in this dictionary.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        private bool TryRetrieveInternal(ReadOnlySpan<char> key, out TValue value)
        {
            value = default;

            var hash = key.CalculateKnuthHash();
            if (!this.InternalBuckets.TryGetValue(hash, out var kdv))
                return false;

            while (kdv != null)
            {
                if (key.SequenceEqual(kdv.Key.AsSpan()))
                {
                    value = kdv.Value;
                    return true;
                }
            }

            return false;
        }

        private bool ContainsKeyInternal(ReadOnlySpan<char> key)
        {
            var hash = key.CalculateKnuthHash();
            if (!this.InternalBuckets.TryGetValue(hash, out var kdv))
                return false;

            while (kdv != null)
            {
                if (key.SequenceEqual(kdv.Key.AsSpan()))
                    return true;

                kdv = kdv.Next;
            }

            return false;
        }

        private ImmutableArray<string> GetKeysInternal()
        {
            var builder = ImmutableArray.CreateBuilder<string>(this.Count);
            foreach (var value in this.InternalBuckets.Values)
            {
                var kdv = value;
                while (kdv != null)
                {
                    builder.Add(kdv.Key);
                    kdv = kdv.Next;
                }
            }

            return builder.MoveToImmutable();
        }

        private ImmutableArray<TValue> GetValuesInternal()
        {
            var builder = ImmutableArray.CreateBuilder<TValue>(this.Count);
            foreach (var value in this.InternalBuckets.Values)
            {
                var kdv = value;
                while (kdv != null)
                {
                    builder.Add(kdv.Value);
                    kdv = kdv.Next;
                }
            }

            return builder.MoveToImmutable();
        }

        private static IReadOnlyDictionary<ulong, KeyedValue> PrepareItems(IEnumerable<KeyValuePair<string, TValue>> items, out int count)
        {
            count = 0;
            var dict = new Dictionary<ulong, KeyedValue>();
            foreach (var (k, v) in items)
            {
                if (k == null)
                    throw new ArgumentException("Keys cannot be null.", nameof(items));

                var hash = k.CalculateKnuthHash();
                if (!dict.ContainsKey(hash))
                {
                    dict.Add(hash, new KeyedValue(k, hash, v));
                    count++;
                    continue;
                }

                var kdv = dict[hash];
                var kdvLast = kdv;
                while (kdv != null)
                {
                    if (kdv.Key == k)
                        throw new ArgumentException("Given key is already present in the dictionary.", nameof(items));

                    kdvLast = kdv;
                    kdv = kdv.Next;
                }

                kdvLast.Next = new KeyedValue(k, hash, v);
                count++;
            }

            return new ReadOnlyDictionary<ulong, KeyedValue>(dict);
        }

        private class KeyedValue
        {
            public ulong KeyHash { get; }
            public string Key { get; }
            public TValue Value { get; set; }

            public KeyedValue Next { get; set; }

            public KeyedValue(string key, ulong keyHash, TValue value)
            {
                this.KeyHash = keyHash;
                this.Key = key;
                this.Value = value;
            }
        }

        private class Enumerator : IEnumerator<KeyValuePair<string, TValue>>
        {
            public KeyValuePair<string, TValue> Current { get; private set; }
            object IEnumerator.Current => this.Current;

            private CharSpanLookupReadOnlyDictionary<TValue> InternalDictionary { get; }
            private IEnumerator<KeyValuePair<ulong, KeyedValue>> InternalEnumerator { get; }
            private KeyedValue CurrentValue { get; set; } = null;

            public Enumerator(CharSpanLookupReadOnlyDictionary<TValue> spDict)
            {
                this.InternalDictionary = spDict;
                this.InternalEnumerator = this.InternalDictionary.InternalBuckets.GetEnumerator();
            }

            public bool MoveNext()
            {
                var kdv = this.CurrentValue;
                if (kdv == null)
                {
                    if (!this.InternalEnumerator.MoveNext())
                        return false;

                    kdv = this.InternalEnumerator.Current.Value;
                    this.Current = new KeyValuePair<string, TValue>(kdv.Key, kdv.Value);

                    this.CurrentValue = kdv.Next;
                    return true;
                }

                this.Current = new KeyValuePair<string, TValue>(kdv.Key, kdv.Value);
                this.CurrentValue = kdv.Next;
                return true;
            }

            public void Reset()
            {
                this.InternalEnumerator.Reset();
                this.Current = default;
                this.CurrentValue = null;
            }

            public void Dispose()
            {
                this.Reset();
            }
        }
    }
}
