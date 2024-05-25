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

using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.Utilities;

/// <summary>
/// Represents a thread synchronization event that, when signaled, must be reset manually. Unlike <see cref="ManualResetEventSlim"/>, this event is asynchronous.
/// </summary>
public sealed class AsyncManualResetEvent
{
    /// <summary>
    /// Gets whether this event has been signaled.
    /// </summary>
    public bool IsSet => this._resetTcs?.Task?.IsCompleted == true;

    private volatile TaskCompletionSource<bool> _resetTcs;

    /// <summary>
    /// Creates a new asynchronous synchronization event with initial state.
    /// </summary>
    /// <param name="initialState">Initial state of this event.</param>
    public AsyncManualResetEvent(bool initialState)
    {
        this._resetTcs = new TaskCompletionSource<bool>();
        if (initialState)
            this._resetTcs.TrySetResult(initialState);
    }

    // Spawn a threadpool thread instead of making a task
    // Maybe overkill, but I am less unsure of this than awaits and
    // potentially cross-scheduler interactions
    /// <summary>
    /// Asynchronously signal this event.
    /// </summary>
    /// <returns></returns>
    public Task SetAsync()
        => Task.Run(() => this._resetTcs.TrySetResult(true));

    /// <summary>
    /// Asynchronously wait for this event to be signaled.
    /// </summary>
    /// <returns></returns>
    public Task WaitAsync()
        => this._resetTcs.Task;

    /// <summary>
    /// Reset this event's signal state to unsignaled.
    /// </summary>
    public void Reset()
    {
        while (true)
        {
            var tcs = this._resetTcs;
            if (!tcs.Task.IsCompleted || Interlocked.CompareExchange(ref this._resetTcs, new TaskCompletionSource<bool>(), tcs) == tcs)
                return;
        }
    }
}
