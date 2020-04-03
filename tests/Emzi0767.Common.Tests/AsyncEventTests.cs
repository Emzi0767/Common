// This file is a part of Emzi0767.Common project.
// 
// Copyright 2020 Emzi0767
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

using System;
using System.Threading.Tasks;
using Emzi0767.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests
{
    [TestClass]
    public sealed class AsyncEventTests
    {
        private AsyncExecutor Executor { get; } = new AsyncExecutor();
        private SecureRandom RNG { get; } = new SecureRandom();
        private AsyncEvent<AsyncEventTests, TestEventArgs> Event { get; }

        private event AsyncEventHandler<AsyncEventTests, TestEventArgs> TestEvent
        {
            add => this.Event.Register(value);
            remove => this.Event.Unregister(value);
        }

        public AsyncEventTests()
        {
            this.Event = new AsyncEvent<AsyncEventTests, TestEventArgs>("TEST_EVENT", TimeSpan.FromSeconds(3), this.EventExceptionHandler);
        }

        [TestMethod]
        public void TestEventArgPropagation()
        {
            var value = this.RNG.GetUInt64();

            Task Handler(AsyncEventTests sender, TestEventArgs e)
            {
                Assert.AreEqual(value, e.Value);
                return Task.CompletedTask;
            }

            this.TestEvent += Handler;
            this.Executor.Execute(this.Event.InvokeAsync(this, new TestEventArgs(value, null)));
            this.TestEvent -= Handler;
        }

        [TestMethod]
        public void TestEventHandling()
        {
            this.TestEvent += Handler1;
            this.TestEvent += Handler2;
            this.Executor.Execute(this.Event.InvokeAsync(this, new TestEventArgs(0, null)));
            this.TestEvent -= Handler2;
            this.TestEvent -= Handler1;

            Task Handler1(AsyncEventTests sender, TestEventArgs e)
            {
                e.Handled = true;
                return Task.CompletedTask;
            }

            Task Handler2(AsyncEventTests sender, TestEventArgs e)
            {
                Assert.Fail("This handler should not have been invoked.");
                return Task.CompletedTask;
            }
        }

        [TestMethod]
        public void TestEventTimeouts()
        {
            AsyncEventTimeoutException<AsyncEventTests, TestEventArgs> timeout = null;

            this.TestEvent += Handler;
            this.Executor.Execute(this.Event.InvokeAsync(this, new TestEventArgs(0, ex => timeout = ex as AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>)));
            this.TestEvent -= Handler;

            Assert.IsNotNull(timeout);
            Assert.IsInstanceOfType(timeout, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));

            async Task Handler(AsyncEventTests sender, TestEventArgs e)
                => await Task.Delay(TimeSpan.FromSeconds(5));
        }

        private void EventExceptionHandler(AsyncEvent<AsyncEventTests, TestEventArgs> @event, Exception exception, AsyncEventHandler<AsyncEventTests, TestEventArgs> faultingHandler, AsyncEventTests sender, TestEventArgs args)
        {
            if (exception is AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>)
            {
                Assert.IsNotNull(args.Return);
                args.Return(exception);
                return;
            }

            Assert.Fail(exception.Message);
        }

        private sealed class TestEventArgs : AsyncEventArgs
        {
            public ulong Value { get; }
            public Action<Exception> Return { get; }

            public TestEventArgs(ulong value, Action<Exception> @return)
            {
                this.Value = value;
                this.Return = @return;
            }
        }
    }
}
