// This file is part of Emzi0767.Common project
//
// Copyright (C) 2020-2021 Emzi0767
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;

namespace Emzi0767.Utilities
{
    /// <summary>
    /// Handles any exception raised by an <see cref="AsyncEvent{TSender, TArgs}"/> or its handlers.
    /// </summary>
    /// <typeparam name="TSender">Type of the object that dispatches this event.</typeparam>
    /// <typeparam name="TArgs">Type of the object which holds arguments for this event.</typeparam>
    /// <param name="asyncEvent">Asynchronous event which threw the exception.</param>
    /// <param name="exception">Exception that was thrown</param>
    /// <param name="handler">Handler which threw the exception.</param>
    /// <param name="sender">Object which dispatched the event.</param>
    /// <param name="eventArgs">Arguments with which the event was dispatched.</param>
    public delegate void AsyncEventExceptionHandler<TSender, TArgs>(AsyncEvent<TSender, TArgs> asyncEvent, Exception exception, AsyncEventHandler<TSender, TArgs> handler, TSender sender, TArgs eventArgs) 
        where TArgs : AsyncEventArgs;
}
