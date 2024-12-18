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

namespace Emzi0767.Serialization;

/// <summary>
/// Declares name of a property in serialized data. This is used for mapping serialized data to object properties and fields.
/// </summary>
[Obsolete("Use [DataMember] with set Name instead.")]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class SerializedNameAttribute : SerializationAttribute
{
    /// <summary>
    /// Gets the serialized name of the field or property.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Declares name of a property in serialized data. This is used for mapping serialized data to object properties and fields.
    /// </summary>
    /// <param name="name">Name of the field or property in serialized data.</param>
    public SerializedNameAttribute(string name)
    {
        this.Name = name;
    }
}
