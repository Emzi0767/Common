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
/// Specifies a decomposer for a given type or property.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class DecomposerAttribute : SerializationAttribute
{
    /// <summary>
    /// Gets the type of the decomposer.
    /// </summary>
    public Type DecomposerType { get; }

    /// <summary>
    /// Specifies a decomposer for given type or property.
    /// </summary>
    /// <param name="type">Type of decomposer to use.</param>
    public DecomposerAttribute(Type type)
    {
        if (!typeof(IDecomposer).IsAssignableFrom(type) || !type.IsClass || type.IsAbstract) // abstract covers static - static = abstract + sealed
            throw new ArgumentException("Invalid type specified. Must be a non-abstract class which implements Emzi0767.Serialization.IDecomposer interface.", nameof(type));

        this.DecomposerType = type;
    }
}
