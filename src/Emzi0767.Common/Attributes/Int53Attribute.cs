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

namespace Emzi0767.Serialization
{
    /// <summary>
    /// <para>Specifies that this 64-bit integer uses no more than 53 bits to represent its value.</para>
    /// <para>This is used to indicate that large numbers are safe for direct serialization into formats which do support 64-bit integers natively (such as JSON).</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class Int53Attribute : SerializationAttribute
    {
        /// <summary>
        /// <para>Gets the maximum safe value representable as an integer by a IEEE754 64-bit binary floating point value.</para>
        /// <para>This value equals to 9007199254740991.</para>
        /// </summary>
        public const long MaxValue = (1L << 53) - 1;

        /// <summary>
        /// <para>Gets the minimum safe value representable as an integer by a IEEE754 64-bit binary floating point value.</para>
        /// <para>This value equals to -9007199254740991.</para>
        /// </summary>
        public const long MinValue = -MaxValue;
    }
}
