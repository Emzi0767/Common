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
using System.Runtime.CompilerServices;

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

        /// <summary>
        /// Calculates the length of string representation of given number in base 10 (including sign, if present).
        /// </summary>
        /// <param name="num">Number to calculate the length of.</param>
        /// <returns>Calculated number length.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateLength(this sbyte num)
            => num == 0 ? 1 : (int)Math.Floor(Math.Log10(Math.Abs(num == sbyte.MinValue ? num + 1 : num))) + (num < 0 ? 2 /* include sign */ : 1);

        /// <summary>
        /// Calculates the length of string representation of given number in base 10 (including sign, if present).
        /// </summary>
        /// <param name="num">Number to calculate the length of.</param>
        /// <returns>Calculated nuembr length.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateLength(this byte num)
            => num == 0 ? 1 : (int)Math.Floor(Math.Log10(num)) + 1;

        /// <summary>
        /// Calculates the length of string representation of given number in base 10 (including sign, if present).
        /// </summary>
        /// <param name="num">Number to calculate the length of.</param>
        /// <returns>Calculated number length.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateLength(this short num)
            => num == 0 ? 1 : (int)Math.Floor(Math.Log10(Math.Abs(num == short.MinValue ? num + 1 : num))) + (num < 0 ? 2 /* include sign */ : 1);

        /// <summary>
        /// Calculates the length of string representation of given number in base 10 (including sign, if present).
        /// </summary>
        /// <param name="num">Number to calculate the length of.</param>
        /// <returns>Calculated nuembr length.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateLength(this ushort num)
            => num == 0 ? 1 : (int)Math.Floor(Math.Log10(num)) + 1;

        /// <summary>
        /// Calculates the length of string representation of given number in base 10 (including sign, if present).
        /// </summary>
        /// <param name="num">Number to calculate the length of.</param>
        /// <returns>Calculated number length.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateLength(this int num)
            => num == 0 ? 1 : (int)Math.Floor(Math.Log10(Math.Abs(num == int.MinValue ? num + 1 : num))) + (num < 0 ? 2 /* include sign */ : 1);

        /// <summary>
        /// Calculates the length of string representation of given number in base 10 (including sign, if present).
        /// </summary>
        /// <param name="num">Number to calculate the length of.</param>
        /// <returns>Calculated nuembr length.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateLength(this uint num)
            => num == 0 ? 1 : (int)Math.Floor(Math.Log10(num)) + 1;

        /// <summary>
        /// Calculates the length of string representation of given number in base 10 (including sign, if present).
        /// </summary>
        /// <param name="num">Number to calculate the length of.</param>
        /// <returns>Calculated number length.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateLength(this long num)
            => num == 0 ? 1 : (int)Math.Floor(Math.Log10(Math.Abs(num == long.MinValue ? num + 1 : num))) + (num < 0 ? 2 /* include sign */ : 1);

        /// <summary>
        /// Calculates the length of string representation of given number in base 10 (including sign, if present).
        /// </summary>
        /// <param name="num">Number to calculate the length of.</param>
        /// <returns>Calculated nuembr length.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateLength(this ulong num)
            => num == 0 ? 1 : (int)Math.Floor(Math.Log10(num)) + 1;

        /// <summary>
        /// Tests wheter given value is in supplied range, optionally allowing it to be an exclusive check.
        /// </summary>
        /// <param name="num">Number to test.</param>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="inclusive">Whether the check is to be inclusive.</param>
        /// <returns>Whether the value is in range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this sbyte num, sbyte min, sbyte max, bool inclusive = true)
        {
            if (min > max)
            {
                min ^= max;
                max ^= min;
                min ^= max;
            }

            return inclusive ? (num >= min && num <= max) : (num > min && num < max);
        }

        /// <summary>
        /// Tests wheter given value is in supplied range, optionally allowing it to be an exclusive check.
        /// </summary>
        /// <param name="num">Number to test.</param>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="inclusive">Whether the check is to be inclusive.</param>
        /// <returns>Whether the value is in range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this byte num, byte min, byte max, bool inclusive = true)
        {
            if (min > max)
            {
                min ^= max;
                max ^= min;
                min ^= max;
            }

            return inclusive ? (num >= min && num <= max) : (num > min && num < max);
        }

        /// <summary>
        /// Tests wheter given value is in supplied range, optionally allowing it to be an exclusive check.
        /// </summary>
        /// <param name="num">Number to test.</param>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="inclusive">Whether the check is to be inclusive.</param>
        /// <returns>Whether the value is in range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this short num, short min, short max, bool inclusive = true)
        {
            if (min > max)
            {
                min ^= max;
                max ^= min;
                min ^= max;
            }

            return inclusive ? (num >= min && num <= max) : (num > min && num < max);
        }
        
        /// <summary>
        /// Tests wheter given value is in supplied range, optionally allowing it to be an exclusive check.
        /// </summary>
        /// <param name="num">Number to test.</param>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="inclusive">Whether the check is to be inclusive.</param>
        /// <returns>Whether the value is in range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this ushort num, ushort min, ushort max, bool inclusive = true)
        {
            if (min > max)
            {
                min ^= max;
                max ^= min;
                min ^= max;
            }

            return inclusive ? (num >= min && num <= max) : (num > min && num < max);
        }

        /// <summary>
        /// Tests wheter given value is in supplied range, optionally allowing it to be an exclusive check.
        /// </summary>
        /// <param name="num">Number to test.</param>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="inclusive">Whether the check is to be inclusive.</param>
        /// <returns>Whether the value is in range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this int num, int min, int max, bool inclusive = true)
        {
            if (min > max)
            {
                min ^= max;
                max ^= min;
                min ^= max;
            }

            return inclusive ? (num >= min && num <= max) : (num > min && num < max);
        }

        /// <summary>
        /// Tests wheter given value is in supplied range, optionally allowing it to be an exclusive check.
        /// </summary>
        /// <param name="num">Number to test.</param>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="inclusive">Whether the check is to be inclusive.</param>
        /// <returns>Whether the value is in range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this uint num, uint min, uint max, bool inclusive = true)
        {
            if (min > max)
            {
                min ^= max;
                max ^= min;
                min ^= max;
            }

            return inclusive ? (num >= min && num <= max) : (num > min && num < max);
        }

        /// <summary>
        /// Tests wheter given value is in supplied range, optionally allowing it to be an exclusive check.
        /// </summary>
        /// <param name="num">Number to test.</param>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="inclusive">Whether the check is to be inclusive.</param>
        /// <returns>Whether the value is in range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this long num, long min, long max, bool inclusive = true)
        {
            if (min > max)
            {
                min ^= max;
                max ^= min;
                min ^= max;
            }

            return inclusive ? (num >= min && num <= max) : (num > min && num < max);
        }

        /// <summary>
        /// Tests wheter given value is in supplied range, optionally allowing it to be an exclusive check.
        /// </summary>
        /// <param name="num">Number to test.</param>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="inclusive">Whether the check is to be inclusive.</param>
        /// <returns>Whether the value is in range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this ulong num, ulong min, ulong max, bool inclusive = true)
        {
            if (min > max)
            {
                min ^= max;
                max ^= min;
                min ^= max;
            }

            return inclusive ? (num >= min && num <= max) : (num > min && num < max);
        }

        /// <summary>
        /// Tests wheter given value is in supplied range, optionally allowing it to be an exclusive check.
        /// </summary>
        /// <param name="num">Number to test.</param>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="inclusive">Whether the check is to be inclusive.</param>
        /// <returns>Whether the value is in range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this float num, float min, float max, bool inclusive = true)
        {
            if (min > max)
                return false;

            return inclusive ? (num >= min && num <= max) : (num > min && num < max);
        }

        /// <summary>
        /// Tests wheter given value is in supplied range, optionally allowing it to be an exclusive check.
        /// </summary>
        /// <param name="num">Number to test.</param>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="inclusive">Whether the check is to be inclusive.</param>
        /// <returns>Whether the value is in range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this double num, double min, double max, bool inclusive = true)
        {
            if (min > max)
                return false;

            return inclusive ? (num >= min && num <= max) : (num > min && num < max);
        }
    }
}
