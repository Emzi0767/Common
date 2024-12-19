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

namespace Emzi0767.Types;

/// <summary>
/// Thrown when an inserted key is already present in the dictionary.
/// </summary>
public sealed class DuplicateKeyException : Exception
{
    /// <summary>
    /// Creates a new duplicate key exception with default message.
    /// </summary>
    public DuplicateKeyException()
        : this("Given key is already present in the dictionary.")
    { }

    /// <summary>
    /// Creates a new duplicate key exception.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public DuplicateKeyException(string message)
        : base(message)
    { }
}
