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
        private AsyncEvent<AsyncEventTests, TestEventArgs2> Event2 { get; }

        private event AsyncEventHandler<AsyncEventTests, TestEventArgs> TestEvent
        {
            add => this.Event.Register(value);
            remove => this.Event.Unregister(value);
        }

        private event AsyncEventHandler<AsyncEventTests, TestEventArgs2> TestEvent2
        {
            add => this.Event2.Register(value);
            remove => this.Event2.Unregister(value);
        }

        public AsyncEventTests()
        {
            this.Event = new AsyncEvent<AsyncEventTests, TestEventArgs>("TEST_EVENT", TimeSpan.FromSeconds(3), this.EventExceptionHandler);
            this.Event2 = new AsyncEvent<AsyncEventTests, TestEventArgs2>("TEST_EVENT_2", TimeSpan.FromSeconds(3), this.EventExceptionHandler);
        }

        [TestMethod]
        public void TestNullTasks()
        {
            this.TestEvent += Handler;
            this.Executor.Execute(this.Event.InvokeAsync(this, new TestEventArgs(0, null)));
            this.TestEvent -= Handler;

            Task Handler(AsyncEventTests sender, TestEventArgs e)
                => null;
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

        [TestMethod]
        public void TestVariance()
        {
            TestException test = null;

            this.TestEvent2 += Handler;
            this.Executor.Execute(this.Event2.InvokeAsync(this, new TestEventArgs2(ex => test = ex as TestException)));
            this.TestEvent2 -= Handler;

            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test, typeof(TestException));

            Task Handler(AsyncEventTests sender, TestEventArgs e)
                => throw new TestException();
        }

        [TestMethod]
        public void TestThrowing()
        {
            this.TestEvent += Handler;
            var aggregateEx = Assert.ThrowsException<AggregateException>(RunHandlers);
            this.TestEvent -= Handler;

            Assert.AreEqual(1, aggregateEx.InnerExceptions.Count);
            Assert.IsInstanceOfType(aggregateEx.InnerExceptions[0], typeof(TestException));

            Task Handler(AsyncEventTests sender, TestEventArgs e)
                => throw new TestException();

            void RunHandlers()
            {
                try
                {
                    this.Executor.Execute(this.Event.InvokeAsync(this, new TestEventArgs(420, ex => { }), AsyncEventExceptionMode.ThrowAllHandleAll));
                }
                catch
                {
                    throw;
                }
            }
        }

        [TestMethod]
        public void TestExceptionModes()
        {
            AggregateException ex;
            Exception ret;

            // Fatal first
            this.TestEvent += Handler1;

            ret = null;
            ex = Assert.ThrowsException<AggregateException>(() => RunHandlers(AsyncEventExceptionMode.ThrowAllHandleAll));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType(ex.InnerException, typeof(TestException));
            Assert.IsNotNull(ret);
            Assert.IsInstanceOfType(ret, typeof(TestException));

            ret = null;
            ex = Assert.ThrowsException<AggregateException>(() => RunHandlers(AsyncEventExceptionMode.ThrowAll));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType(ex.InnerException, typeof(TestException));
            Assert.IsNull(ret);

            ret = null;
            ex = Assert.ThrowsException<AggregateException>(() => RunHandlers(AsyncEventExceptionMode.ThrowFatal));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType(ex.InnerException, typeof(TestException));
            Assert.IsNull(ret);

            ret = null;
            ex = Assert.ThrowsException<AggregateException>(() => RunHandlers(AsyncEventExceptionMode.ThrowFatal | AsyncEventExceptionMode.HandleAll));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType(ex.InnerException, typeof(TestException));
            Assert.IsNotNull(ret);
            Assert.IsInstanceOfType(ret, typeof(TestException));

            ret = null;
            ex = Assert.ThrowsException<AggregateException>(() => RunHandlers(AsyncEventExceptionMode.ThrowFatal | AsyncEventExceptionMode.HandleFatal));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType(ex.InnerException, typeof(TestException));
            Assert.IsNotNull(ret);
            Assert.IsInstanceOfType(ret, typeof(TestException));

            ret = null;
            ex = Assert.ThrowsException<AggregateException>(() => RunHandlers(AsyncEventExceptionMode.ThrowFatal | AsyncEventExceptionMode.HandleNonFatal));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType(ex.InnerException, typeof(TestException));
            Assert.IsNull(ret);

            try
            {
                ret = null;
                RunHandlers(AsyncEventExceptionMode.Default);
                Assert.IsNotNull(ret);
                Assert.IsInstanceOfType(ret, typeof(TestException));

                ret = null;
                RunHandlers(AsyncEventExceptionMode.ThrowNonFatal);
                Assert.IsNull(ret);

                ret = null;
                RunHandlers(AsyncEventExceptionMode.ThrowNonFatal | AsyncEventExceptionMode.HandleAll);
                Assert.IsNotNull(ret);
                Assert.IsInstanceOfType(ret, typeof(TestException));

                ret = null;
                RunHandlers(AsyncEventExceptionMode.ThrowNonFatal | AsyncEventExceptionMode.HandleFatal);
                Assert.IsNotNull(ret);
                Assert.IsInstanceOfType(ret, typeof(TestException));

                ret = null;
                RunHandlers(AsyncEventExceptionMode.ThrowNonFatal | AsyncEventExceptionMode.HandleNonFatal);
                Assert.IsNull(ret);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch
            {
                Assert.Fail("Exception thrown where one should not have been thrown.");
                throw;
            }

            this.TestEvent -= Handler1;

            // Non-fatal next
            this.TestEvent += Handler2;

            ret = null;
            ex = Assert.ThrowsException<AggregateException>(() => RunHandlers(AsyncEventExceptionMode.ThrowAllHandleAll));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType(ex.InnerException, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));
            Assert.IsNotNull(ret);
            Assert.IsInstanceOfType(ret, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));

            ret = null;
            ex = Assert.ThrowsException<AggregateException>(() => RunHandlers(AsyncEventExceptionMode.ThrowAll));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType(ex.InnerException, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));
            Assert.IsNull(ret);

            ret = null;
            ex = Assert.ThrowsException<AggregateException>(() => RunHandlers(AsyncEventExceptionMode.ThrowNonFatal));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType(ex.InnerException, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));
            Assert.IsNull(ret);

            ret = null;
            ex = Assert.ThrowsException<AggregateException>(() => RunHandlers(AsyncEventExceptionMode.ThrowNonFatal | AsyncEventExceptionMode.HandleAll));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType(ex.InnerException, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));
            Assert.IsNotNull(ret);
            Assert.IsInstanceOfType(ret, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));

            ret = null;
            ex = Assert.ThrowsException<AggregateException>(() => RunHandlers(AsyncEventExceptionMode.ThrowNonFatal | AsyncEventExceptionMode.HandleFatal));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType(ex.InnerException, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));
            Assert.IsNull(ret);

            ret = null;
            ex = Assert.ThrowsException<AggregateException>(() => RunHandlers(AsyncEventExceptionMode.ThrowNonFatal | AsyncEventExceptionMode.HandleNonFatal));
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.IsInstanceOfType(ex.InnerException, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));
            Assert.IsNotNull(ret);
            Assert.IsInstanceOfType(ret, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));


            try
            {
                ret = null;
                RunHandlers(AsyncEventExceptionMode.Default);
                Assert.IsNotNull(ret);
                Assert.IsInstanceOfType(ret, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));

                ret = null;
                RunHandlers(AsyncEventExceptionMode.ThrowFatal);
                Assert.IsNull(ret);

                ret = null;
                RunHandlers(AsyncEventExceptionMode.ThrowFatal | AsyncEventExceptionMode.HandleAll);
                Assert.IsNotNull(ret);
                Assert.IsInstanceOfType(ret, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));

                ret = null;
                RunHandlers(AsyncEventExceptionMode.ThrowFatal | AsyncEventExceptionMode.HandleFatal);
                Assert.IsNull(ret);

                ret = null;
                RunHandlers(AsyncEventExceptionMode.ThrowFatal | AsyncEventExceptionMode.HandleNonFatal);
                Assert.IsNotNull(ret);
                Assert.IsInstanceOfType(ret, typeof(AsyncEventTimeoutException<AsyncEventTests, TestEventArgs>));
            }
            catch
            {
                Assert.Fail("Exception thrown where one should not have been thrown.");
                throw;
            }

            this.TestEvent -= Handler2;

            Task Handler1(AsyncEventTests sender, TestEventArgs e)
                => throw new TestException();

            Task Handler2(AsyncEventTests sender, TestEventArgs e)
                => Task.Delay(TimeSpan.FromSeconds(5));

            void RunHandlers(AsyncEventExceptionMode exceptionMode)
            {
                try
                {
                    this.Executor.Execute(this.Event.InvokeAsync(this, new TestEventArgs(420, ex => ret = ex), exceptionMode));
                }
                catch
                {
                    throw;
                }
            }
        }

        private void EventExceptionHandler<T1, T2>(AsyncEvent<T1, T2> @event, Exception exception, AsyncEventHandler<T1, T2> faultingHandler, T1 sender, T2 args)
            where T2 : TestEventArgs
        {
            switch (exception)
            {
                case AsyncEventTimeoutException<AsyncEventTests, TestEventArgs> timeout:
                    Assert.IsNotNull(args.Return);
                    args.Return(timeout);
                    return;

                case TestException test:
                    Assert.IsNotNull(test);
                    args.Return(test);
                    return;
            }

            Assert.Fail(exception.Message);
        }

        private class TestEventArgs : AsyncEventArgs
        {
            public virtual ulong Value { get; }
            public virtual Action<Exception> Return { get; }

            public TestEventArgs(ulong value, Action<Exception> @return)
            {
                this.Value = value;
                this.Return = @return;
            }
        }

        private class TestEventArgs2 : TestEventArgs
        {
            public override ulong Value => 42;

            public TestEventArgs2(Action<Exception> @return)
                : base(0, @return)
            { }
        }

        private class TestException : Exception
        { }
    }
}
