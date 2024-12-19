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
using Emzi0767.Types;

namespace Emzi0767;

internal static class ThrowHelper
{
    public static void BufferDisposed()
        => throw new ObjectDisposedException("This buffer is disposed.");

    public static void NotSupported()
        => throw new NotSupportedException();

    public static void Argument(string arg, string msg)
        => throw new ArgumentException(msg, arg);

    public static void ArgumentNull(string arg)
        => throw new ArgumentNullException(arg);

    public static void ArgumentNull(string arg, string msg)
        => throw new ArgumentNullException(arg, msg);

    public static void ArgumentOutOfRange(string arg)
        => throw new ArgumentOutOfRangeException(arg);

    public static void ArgumentOutOfRange(string arg, string msg)
        => throw new ArgumentOutOfRangeException(arg, msg);

    public static void NullReference()
        => throw new NullReferenceException();

    public static void Aggregate(string msg, IEnumerable<Exception> exceptions)
        => throw new AggregateException(msg, exceptions);

    public static void KeyNotFound()
        => throw new KeyNotFoundException();

    public static void DuplicateKey()
        => throw new DuplicateKeyException();

    public static void InvalidOperation(string msg)
        => throw new InvalidOperationException(msg);

    public static void Instance(Exception ex)
        => throw ex;
}
