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
    /// <para>Specifies that this enum should be serialized and deserialized as its underlying numeric type.</para>
    /// <para>This is used to change the behaviour of enum serialization.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NumericEnumAttribute : Attribute
    { }

    /// <summary>
    /// <para>Specifies that this enum should be serialized and deserialized as its string representation.</para>
    /// <para>This is used to change the behaviour of enum serialization.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class StringEnumAttribute : Attribute
    { }
}
