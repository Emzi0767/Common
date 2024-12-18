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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Emzi0767.Serialization;

/// <summary>
/// Decomposes <see cref="Complex"/> numbers into tuples (arrays of 2).
/// </summary>
public sealed class ComplexDecomposer : IDecomposer
{
    private static Type TComplex { get; } = typeof(Complex);
    private static Type TDoubleArray { get; } = typeof(double[]);
    private static Type TDoubleEnumerable { get; } = typeof(IEnumerable<double>);
    private static Type TObjectArray { get; } = typeof(object[]);
    private static Type TObjectEnumerable { get; } = typeof(IEnumerable<object>);

    /// <inheritdoc />
    public bool CanDecompose(Type t)
        => t == TComplex;

    /// <inheritdoc />
    public bool CanRecompose(Type t)
        => t == TDoubleArray
        || t == TObjectArray
        || TDoubleEnumerable.IsAssignableFrom(t)
        || TObjectEnumerable.IsAssignableFrom(t);

    /// <inheritdoc />
    public bool TryDecompose(object obj, Type tobj, out object decomposed, out Type tdecomposed)
    {
        decomposed = null;
        tdecomposed = TDoubleArray;

        if (tobj != TComplex || obj is not Complex c)
            return false;

        decomposed = new[] { c.Real, c.Imaginary };
        return true;
    }

    /// <inheritdoc />
    public bool TryRecompose(object obj, Type tobj, Type trecomposed, out object recomposed)
    {
        recomposed = null;

        if (trecomposed != TComplex)
            return false;

        // ie<double>
        if (TDoubleEnumerable.IsAssignableFrom(tobj) && obj is IEnumerable<double> ied)
        {
            if (ied.Count() < 2)
                return false;

            var (real, imag) = ied.FirstTwoOrDefault();
            recomposed = new Complex(real, imag);
            return true;
        }

        // ie<obj>
        if (TObjectEnumerable.IsAssignableFrom(tobj) && obj is IEnumerable<object> ieo)
        {
            if (ieo.Count() < 2)
                return false;

            var (real, imag) = ieo.FirstTwoOrDefault();
            if (real is not double dreal || imag is not double dimag)
                return false;

            recomposed = new Complex(dreal, dimag);
            return true;
        }

        return false;
    }
}
