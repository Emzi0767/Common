// This file is part of Emzi0767.Common project
//
// Copyright 2019 Emzi0767
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace System.Threading.Tasks
{
    /// <summary>
    /// <para>Thrown whenever execution of an <see cref="AsyncEventHandler{TSender, TArgs}"/> exceeds maximum time allowed.</para>
    /// <para>This is a non-fatal exception, used primarily to inform users that their code is taking too long to execute.</para>
    /// </summary>
    /// <typeparam name="TSender">Type of sender that dispatched this asynchronous event.</typeparam>
    /// <typeparam name="TArgs">Type of event arguments for the asynchronous event.</typeparam>
    public class AsyncEventTimeoutException<TSender, TArgs> : Exception where TArgs : AsyncEventArgs
    {
        /// <summary>
        /// Gets the event the invokation of which caused the timeout.
        /// </summary>
        public AsyncEvent<TSender, TArgs> Event { get; }

        /// <summary>
        /// Gets the handler which caused the timeout.
        /// </summary>
        public AsyncEventHandler<TSender, TArgs> Handler { get; }

        /// <summary>
        /// Creates a new timeout exception for specified event and handler.
        /// </summary>
        /// <param name="asyncEvent">Event the execution of which timed out.</param>
        /// <param name="eventHandler">Handler which timed out.</param>
        public AsyncEventTimeoutException(AsyncEvent<TSender, TArgs> asyncEvent, AsyncEventHandler<TSender, TArgs> eventHandler)
            : base("An event handler caused the invokation of asynchronous event to time out.")
        {
            this.Event = asyncEvent;
            this.Handler = eventHandler;
        }
    }
}
