// This file is a part of Emzi0767.Common project.
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

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests
{
    [TestClass]
    public sealed class AsyncExecutorTests
    {
        private AsyncExecutor Executor { get; } = new AsyncExecutor();

        [TestMethod]
        public void TestVoidTasks()
        {
            // Create a value to keep track of
            ByRef<int> x = 1;

            // Run a non-faulting void and test that it does not throw
            this.Executor.Execute(this.NonFaultingVoidAsync(x));
            Assert.AreEqual(2, x.Instance);

            // Run a faulting void and test that it throws
            Assert.ThrowsException<Exception>(() => this.Executor.Execute(this.FaultingVoidAsync(x)));
            Assert.AreEqual(4, x.Instance);
        }

        [TestMethod]
        public void TestTypeTasks()
        {
            // Create a value to keep track of
            ByRef<int> x = 1;

            // Run a non-faulting type-returning task and test that it does not throw
            var y = this.Executor.Execute(this.NonFaultingTypeAsync(x));
            Assert.AreEqual(2, x.Instance);
            Assert.AreEqual(2, y.Instance);
            Assert.IsTrue(ReferenceEquals(x, y));

            // Run a faulting type-returning task and test that it throws
            Assert.ThrowsException<Exception>(() => y = this.Executor.Execute(this.FaultingTypeAsync(x)));
            Assert.AreEqual(4, x.Instance);
            Assert.AreEqual(4, y.Instance);
            Assert.IsTrue(ReferenceEquals(x, y));
        }

        [TestMethod]
        public void TestCancellationAsync()
        {
            using (var cts = new CancellationTokenSource())
            {
                _ = Task.Delay(100).ContinueWith(t => cts.Cancel());
                Assert.ThrowsException<Exception>(() => this.Executor.Execute(Task.Delay(500, cts.Token)));
            }

            // infinite wait?
            using (var cts = new CancellationTokenSource())
            {
                _ = Task.Delay(100).ContinueWith(t => cts.Cancel());
                Assert.ThrowsException<Exception>(() => this.Executor.Execute(Task.Delay(-1, cts.Token)));
            }

            Assert.IsTrue(true, "Reached this point.");
        }

        private sealed class ByRef<T>
        {
            public T Instance { get; set; }

            public ByRef(T inst)
            {
                this.Instance = inst;
            }

            public static implicit operator ByRef<T>(T inst)
                => new ByRef<T>(inst);
        }

        private async Task NonFaultingVoidAsync(ByRef<int> value)
        {
            await Task.Yield();

            await Task.Delay(500);
            value.Instance *= 2;
        }

        private async Task FaultingVoidAsync(ByRef<int> value)
        {
            await Task.Yield();

            await Task.Delay(500);
            value.Instance *= 2;

            await Task.Delay(500);
            throw new Exception("Test exception.");
#pragma warning disable 0162
            value.Instance *= 2;
#pragma warning restore 0162
        }

        private async Task<ByRef<int>> NonFaultingTypeAsync(ByRef<int> value)
        {
            await Task.Yield();

            await Task.Delay(500);
            value.Instance *= 2;

            return value;
        }

        private async Task<ByRef<int>> FaultingTypeAsync(ByRef<int> value)
        {
            await Task.Yield();

            await Task.Delay(500);
            value.Instance *= 2;

            await Task.Delay(500);
            throw new Exception("Test exception.");
#pragma warning disable 0162
            value.Instance *= 2;
            return value.Instance;
#pragma warning restore 0162
        }
    }
}
