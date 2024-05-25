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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.Utilities;

/// <summary>
/// Provides a simplified way of executing asynchronous code synchronously.
/// </summary>
public class AsyncExecutor
{
    /// <summary>
    /// Creates a new instance of asynchronous executor.
    /// </summary>
    public AsyncExecutor()
    { }

    /// <summary>
    /// Executes a specified task in an asynchronous manner, waiting for its completion.
    /// </summary>
    /// <param name="task">Task to execute.</param>
    public void Execute(Task task)
    {
        // create state object
        var taskState = new StateRef<object>(new AutoResetEvent(false));

        // queue a task and wait for it to finish executing
        task.ContinueWith(TaskCompletionHandler, taskState);
        taskState.Lock.WaitOne();

        // check for and rethrow any exceptions
        if (taskState.Exception != null)
            throw taskState.Exception;

        // completion method
        void TaskCompletionHandler(Task t, object state)
        {
            // retrieve state data
            var stateRef = state as StateRef<object>;

            // retrieve any exceptions or cancellation status
            if (t.IsFaulted)
            {
                if (t.Exception.InnerExceptions.Count == 1) // unwrap if 1
                    stateRef.Exception = t.Exception.InnerException;
                else
                    stateRef.Exception = t.Exception;
            }
            else if (t.IsCanceled)
            {
                stateRef.Exception = new TaskCanceledException(t);
            }

            // signal that the execution is done
            stateRef.Lock.Set();
        }
    }

    /// <summary>
    /// Executes a specified task in an asynchronous manner, waiting for its completion, and returning the result.
    /// </summary>
    /// <typeparam name="T">Type of the Task's return value.</typeparam>
    /// <param name="task">Task to execute.</param>
    /// <returns>Task's result.</returns>
    public T Execute<T>(Task<T> task)
    {
        // create state object
        var taskState = new StateRef<T>(new AutoResetEvent(false));

        // queue a task and wait for it to finish executing
        task.ContinueWith(TaskCompletionHandler, taskState);
        taskState.Lock.WaitOne();

        // check for and rethrow any exceptions
        if (taskState.Exception != null)
            throw taskState.Exception;

        // return the result, if any
        if (taskState.HasResult)
            return taskState.Result;

        // throw exception if no result
        throw new Exception("Task returned no result.");

        // completion method
        void TaskCompletionHandler(Task<T> t, object state)
        {
            // retrieve state data
            var stateRef = state as StateRef<T>;

            // retrieve any exceptions or cancellation status
            if (t.IsFaulted)
            {
                if (t.Exception.InnerExceptions.Count == 1) // unwrap if 1
                    stateRef.Exception = t.Exception.InnerException;
                else
                    stateRef.Exception = t.Exception;
            }
            else if (t.IsCanceled)
            {
                stateRef.Exception = new TaskCanceledException(t);
            }

            // return the result from the task, if any
            if (t.IsCompleted && !t.IsFaulted)
            {
                stateRef.HasResult = true;
                stateRef.Result = t.Result;
            }

            // signal that the execution is done
            stateRef.Lock.Set();
        }
    }

    private sealed class StateRef<T>
    {
        /// <summary>
        /// Gets the lock used to wait for task's completion.
        /// </summary>
        public AutoResetEvent Lock { get; }

        /// <summary>
        /// Gets the exception that occured during task's execution, if any.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets the result returned by the task.
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// Gets whether the task returned a result.
        /// </summary>
        public bool HasResult { get; set; } = false;

        public StateRef(AutoResetEvent @lock)
        {
            this.Lock = @lock;
        }
    }
}
