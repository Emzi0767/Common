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
/// ABC for <see cref="AsyncEventHandler{TSender, TArgs}"/>, allowing for using instances thereof without knowing the underlying instance's type parameters.
/// </summary>
public abstract class AsyncEventTimeoutException : Exception
{
    /// <summary>
    /// Gets the event the invocation of which caused the timeout.
    /// </summary>
    public AsyncEvent Event { get; }

    /// <summary>
    /// Gets the handler which caused the timeout.
    /// </summary>
    public AsyncEventHandler<object, AsyncEventArgs> Handler { get; }

    private protected AsyncEventTimeoutException(AsyncEvent asyncEvent, AsyncEventHandler<object, AsyncEventArgs> eventHandler, string message)
        : base(message)
    {
        this.Event = asyncEvent;
        this.Handler = eventHandler;
    }
}

/// <summary>
/// <para>Thrown whenever execution of an <see cref="AsyncEventHandler{TSender, TArgs}"/> exceeds maximum time allowed.</para>
/// <para>This is a non-fatal exception, used primarily to inform users that their code is taking too long to execute.</para>
/// </summary>
/// <typeparam name="TSender">Type of sender that dispatched this asynchronous event.</typeparam>
/// <typeparam name="TArgs">Type of event arguments for the asynchronous event.</typeparam>
public class AsyncEventTimeoutException<TSender, TArgs> : AsyncEventTimeoutException
    where TArgs : AsyncEventArgs
{
    /// <summary>
    /// Gets the event the invocation of which caused the timeout.
    /// </summary>
    public new AsyncEvent<TSender, TArgs> Event => base.Event as AsyncEvent<TSender, TArgs>;

    /// <summary>
    /// Gets the handler which caused the timeout.
    /// </summary>
    public new AsyncEventHandler<TSender, TArgs> Handler => base.Handler as AsyncEventHandler<TSender, TArgs>;

    /// <summary>
    /// Creates a new timeout exception for specified event and handler.
    /// </summary>
    /// <param name="asyncEvent">Event the execution of which timed out.</param>
    /// <param name="eventHandler">Handler which timed out.</param>
    public AsyncEventTimeoutException(AsyncEvent<TSender, TArgs> asyncEvent, AsyncEventHandler<TSender, TArgs> eventHandler)
        : base(asyncEvent, eventHandler as AsyncEventHandler<object, AsyncEventArgs>, "An event handler caused the invocation of an asynchronous event to time out.")
    { }
}
