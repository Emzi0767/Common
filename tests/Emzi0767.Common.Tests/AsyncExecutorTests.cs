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
using System.Threading;
using System.Threading.Tasks;
using Emzi0767.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests;

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

        // Run another faulting void and test that it throws
        Assert.ThrowsException<InvalidOperationException>(() => this.Executor.Execute(this.FaultingVoid2Async()));
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

        // Run another faulting type-returning task and test that it throws
        Assert.ThrowsException<InvalidOperationException>(() => y = this.Executor.Execute(this.FaultingType2Async()));
    }

    [TestMethod]
    public void TestCancellationAsync()
    {
        using (var cts = new CancellationTokenSource())
        {
            _ = Task.Delay(100).ContinueWith(t => cts.Cancel());
            Assert.ThrowsException<TaskCanceledException>(() => this.Executor.Execute(Task.Delay(500, cts.Token)));
        }

        // infinite wait?
        using (var cts = new CancellationTokenSource())
        {
            _ = Task.Delay(100).ContinueWith(t => cts.Cancel());
            Assert.ThrowsException<TaskCanceledException>(() => this.Executor.Execute(Task.Delay(-1, cts.Token)));
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
            => new(inst);
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

    private async Task FaultingVoid2Async()
    {
        await Task.Yield();

        await Task.Delay(500);

        await Task.Delay(500);
        throw new InvalidOperationException("Test exception.");
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

    private async Task<int> FaultingType2Async()
    {
        await Task.Yield();

        await Task.Delay(1000);
        throw new InvalidOperationException("Test exception.");
    }
}
