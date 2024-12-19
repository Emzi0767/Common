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

namespace Emzi0767.Utilities;

/// <summary>
/// Defines the behaviour for throwing exceptions from <see cref="AsyncEvent{TSender, TArgs}.InvokeAsync(TSender, TArgs, AsyncEventExceptionMode)"/>.
/// </summary>
[Flags]
public enum AsyncEventExceptionMode : int
{
    /// <summary>
    /// Defines that no exceptions should be thrown. Only exception handlers will be used.
    /// </summary>
    IgnoreAll = 0,

    /// <summary>
    /// Defines that only fatal (i.e. non-<see cref="AsyncEventTimeoutException{TSender, TArgs}"/>) exceptions
    /// should be thrown.
    /// </summary>
    ThrowFatal = 1,

    /// <summary>
    /// Defines that only non-fatal (i.e. <see cref="AsyncEventTimeoutException{TSender, TArgs}"/>) exceptions
    /// should be thrown.
    /// </summary>
    ThrowNonFatal = 2,

    /// <summary>
    /// Defines that all exceptions should be thrown. This is equivalent to combining <see cref="ThrowFatal"/> and
    /// <see cref="ThrowNonFatal"/> flags.
    /// </summary>
    ThrowAll = ThrowFatal | ThrowNonFatal,

    /// <summary>
    /// Defines that only fatal (i.e. non-<see cref="AsyncEventTimeoutException{TSender, TArgs}"/>) exceptions
    /// should be handled by the specified exception handler.
    /// </summary>
    HandleFatal = 4,

    /// <summary>
    /// Defines that only non-fatal (i.e. <see cref="AsyncEventTimeoutException{TSender, TArgs}"/>) exceptions
    /// should be handled by the specified exception handler.
    /// </summary>
    HandleNonFatal = 8,

    /// <summary>
    /// Defines that all exceptions should be handled by the specified exception handler. This is equivalent to
    /// combining <see cref="HandleFatal"/> and <see cref="HandleNonFatal"/> flags.
    /// </summary>
    HandleAll = HandleFatal | HandleNonFatal,

    /// <summary>
    /// Defines that all exceptions should be thrown and handled by the specified exception handler. This is
    /// equivalent to combinind <see cref="HandleAll"/> and <see cref="ThrowAll"/> flags.
    /// </summary>
    ThrowAllHandleAll = ThrowAll | HandleAll,

    /// <summary>
    /// Default mode, equivalent to <see cref="HandleAll"/>.
    /// </summary>
    Default = HandleAll
}
