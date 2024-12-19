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
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Emzi0767.Utilities;

/// <summary>
/// ABC for <see cref="AsyncEvent{TSender, TArgs}"/>, allowing for using instances thereof without knowing the underlying instance's type parameters.
/// </summary>
public abstract class AsyncEvent
{
    /// <summary>
    /// Gets the name of this event.
    /// </summary>
    public string Name { get; }

    private protected AsyncEvent(string name)
    {
        this.Name = name;
    }
}

/// <summary>
/// Implementation of asynchronous event. The handlers of such events are executed asynchronously, but sequentially.
/// </summary>
/// <typeparam name="TSender">Type of the object that dispatches this event.</typeparam>
/// <typeparam name="TArgs">Type of event argument object passed to this event's handlers.</typeparam>
public sealed class AsyncEvent<TSender, TArgs> : AsyncEvent
    where TArgs : AsyncEventArgs
{
    /// <summary>
    /// Gets the maximum alloted execution time for all handlers. Any event which causes the handler to time out 
    /// will raise a non-fatal <see cref="AsyncEventTimeoutException{TSender, TArgs}"/>.
    /// </summary>
    public TimeSpan MaximumExecutionTime { get; }

    private readonly object _lock = new();
    private ImmutableArray<AsyncEventHandler<TSender, TArgs>> _handlers;
    private readonly AsyncEventExceptionHandler<TSender, TArgs> _exceptionHandler;

    /// <summary>
    /// Creates a new asynchronous event with specified name and exception handler.
    /// </summary>
    /// <param name="name">Name of this event.</param>
    /// <param name="maxExecutionTime">Maximum handler execution time. A value of <see cref="TimeSpan.Zero"/> means infinite.</param>
    /// <param name="exceptionHandler">Delegate which handles exceptions caused by this event.</param>
    public AsyncEvent(string name, TimeSpan maxExecutionTime, AsyncEventExceptionHandler<TSender, TArgs> exceptionHandler)
        : base(name)
    {
        this._handlers = ImmutableArray<AsyncEventHandler<TSender, TArgs>>.Empty;
        this._exceptionHandler = exceptionHandler;

        this.MaximumExecutionTime = maxExecutionTime;
    }

    /// <summary>
    /// Registers a new handler for this event.
    /// </summary>
    /// <param name="handler">Handler to register for this event.</param>
    public void Register(AsyncEventHandler<TSender, TArgs> handler)
    {
        if (handler is null)
            ThrowHelper.ArgumentNull(nameof(handler));

        lock (this._lock)
            this._handlers = this._handlers.Add(handler);
    }

    /// <summary>
    /// Unregisters an existing handler from this event.
    /// </summary>
    /// <param name="handler">Handler to unregister from the event.</param>
    public void Unregister(AsyncEventHandler<TSender, TArgs> handler)
    {
        if (handler is null)
            ThrowHelper.ArgumentNull(nameof(handler));

        lock (this._lock)
            this._handlers = this._handlers.Remove(handler);
    }

    /// <summary>
    /// Unregisters all existing handlers from this event.
    /// </summary>
    public void UnregisterAll()
        => this._handlers = ImmutableArray<AsyncEventHandler<TSender, TArgs>>.Empty;

    /// <summary>
    /// <para>Raises this event by invoking all of its registered handlers, in order of registration.</para>
    /// <para>All exceptions throw during invocation will be handled by the event's registered exception handler.</para>
    /// </summary>
    /// <param name="sender">Object which raised this event.</param>
    /// <param name="e">Arguments for this event.</param>
    /// <param name="exceptionMode">Defines what to do with exceptions caught from handlers.</param>
    /// <returns></returns>
    public async Task InvokeAsync(TSender sender, TArgs e, AsyncEventExceptionMode exceptionMode = AsyncEventExceptionMode.Default)
    {
        var handlers = this._handlers;
        if (handlers.Length == 0)
            return;

        // Collect exceptions
        List<Exception> exceptions = null;
        if ((exceptionMode & AsyncEventExceptionMode.ThrowAll) != 0)
            exceptions = new List<Exception>(handlers.Length * 2 /* timeout + regular */);

        // If we have a timeout configured, start the timeout task
        var timeout = this.MaximumExecutionTime > TimeSpan.Zero ? Task.Delay(this.MaximumExecutionTime) : null;
        for (var i = 0; i < handlers.Length; i++)
        {
            var handler = handlers[i];
            try
            {
                // Start the handler execution
                var handlerTask = handler(sender, e);
                if (handlerTask is not null && timeout is not null)
                {
                    // If timeout is configured, wait for any task to finish
                    // If the timeout task finishes first, the handler is causing a timeout

                    var result = await Task.WhenAny(timeout, handlerTask).ConfigureAwait(false);
                    if (result == timeout)
                    {
                        timeout = null;
                        var timeoutEx = new AsyncEventTimeoutException<TSender, TArgs>(this, handler);

                        // Notify about the timeout and complete execution
                        if ((exceptionMode & AsyncEventExceptionMode.HandleNonFatal) == AsyncEventExceptionMode.HandleNonFatal)
                            this.HandleException(timeoutEx, handler, sender, e);

                        if ((exceptionMode & AsyncEventExceptionMode.ThrowNonFatal) == AsyncEventExceptionMode.ThrowNonFatal)
                            exceptions.Add(timeoutEx);
                    }
                }

                if (handlerTask is not null)
                {
                    // No timeout is configured, or timeout already expired, proceed as usual
                    await handlerTask.ConfigureAwait(false);
                }

                if (e.Handled)
                    break;
            }
            catch (Exception ex)
            {
                e.Handled = false;

                if ((exceptionMode & AsyncEventExceptionMode.HandleFatal) == AsyncEventExceptionMode.HandleFatal)
                    this.HandleException(ex, handler, sender, e);

                if ((exceptionMode & AsyncEventExceptionMode.ThrowFatal) == AsyncEventExceptionMode.ThrowFatal)
                    exceptions.Add(ex);
            }
        }

        if ((exceptionMode & AsyncEventExceptionMode.ThrowAll) != 0 && exceptions.Count > 0)
            ThrowHelper.Aggregate("Exceptions were thrown during execution of the event's handlers.", exceptions);
    }

    private void HandleException(Exception ex, AsyncEventHandler<TSender, TArgs> handler, TSender sender, TArgs args)
    {
        if (this._exceptionHandler is not null)
            this._exceptionHandler(this, ex, handler, sender, args);
    }
}
