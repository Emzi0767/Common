// This file is part of Emzi0767.Common project.
//
// Copyright © 2020-2024 Emzi0767
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
/// Provides an interface to decompose an object into another object or combination of objects.
/// </summary>
public interface IDecomposer
{
    /// <summary>
    /// Checks whether the decomposer can decompose a specific type.
    /// </summary>
    /// <param name="t">Type to check.</param>
    /// <returns>Whether the decomposer can decompose a given type.</returns>
    bool CanDecompose(Type t);

    /// <summary>
    /// <para>Checks whether the decomposer can recompose a specific decomposed type.</para>
    /// <para>Note that while a type might be considered recomposable, other factors might prevent recomposing operation from being successful.</para>
    /// </summary>
    /// <param name="t">Decomposed type to check.</param>
    /// <returns>Whether the decomposer can decompose a given type.</returns>
    bool CanRecompose(Type t);

    /// <summary>
    /// Attempts to decompose a given object of specified source type. The operation produces the decomposed object and the type it got decomposed into.
    /// </summary>
    /// <param name="obj">Object to decompose.</param>
    /// <param name="tobj">Type to decompose.</param>
    /// <param name="decomposed">Decomposition result.</param>
    /// <param name="tdecomposed">Type of the result.</param>
    /// <returns>Whether the operation was successful.</returns>
    bool TryDecompose(object obj, Type tobj, out object decomposed, out Type tdecomposed);

    /// <summary>
    /// Attempts to recompose given object of specified source type, into specified target type. The operation produces the recomposed object.
    /// </summary>
    /// <param name="obj">Object to recompose from.</param>
    /// <param name="tobj">Type of data to recompose.</param>
    /// <param name="trecomposed">Type to recompose into.</param>
    /// <param name="recomposed">Recomposition result.</param>
    /// <returns>Whether the operation was successful.</returns>
    bool TryRecompose(object obj, Type tobj, Type trecomposed, out object recomposed);
}
