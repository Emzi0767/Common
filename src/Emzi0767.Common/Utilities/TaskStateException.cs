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
using System.Threading.Tasks;

namespace Emzi0767.Utilities;

/// <summary>
/// Used to indicate that a task found itself in an invalid or unexpected state.
/// </summary>
public sealed class TaskStateException : Exception
{
    /// <summary>
    /// Gets the faulted task.
    /// </summary>
    public Task Task { get; }

    /// <summary>
    /// Creates a new exception indicating that a task has reached an invalid or unexpected state.
    /// </summary>
    /// <param name="t">Faulted task.</param>
    /// <param name="message">Exception message.</param>
    public TaskStateException(Task t, string message)
        : base(message)
    {
        this.Task = t;
    }
}
