using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace TG.Common
{
    /// <summary>
    /// Provides utility methods for working with asynchronous tasks, including executing tasks synchronously,
    /// handling task delays with cancellation support and executing tasks with SafeFireAndForget.
    /// </summary>
    /// <remarks>The <see cref="TaskHelpers"/> class includes methods for running asynchronous tasks
    /// synchronously on the calling thread, as well as methods for delaying execution with cancellation support. These
    /// methods are designed to simplify common task-related operations while ensuring proper handling of continuations
    /// and exceptions.</remarks>
    public static partial class TaskHelpers
    {
        /// <summary>
        /// Executes a task synchronously on the calling thread by installing a temporary synchronization context that queues continuations
        /// </summary>
        /// <param name="task">Callback for asynchronous task to run</param>
        public static void RunSync(Func<Task> task)
        {
            var currentContext = SynchronizationContext.Current;
            var customContext = new CustomSynchronizationContext(task);

            try
            {
                SynchronizationContext.SetSynchronizationContext(customContext);
                customContext.Run();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(currentContext);
            }
        }

        /// <summary>
        /// Executes a task synchronously on the calling thread by installing a temporary synchronization context that queues continuations
        /// </summary>
        /// <param name="task">Callback for asynchronous task to run</param>
        /// <typeparam name="T">Return type for the task</typeparam>
        /// <returns>Return value from the task</returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            T? result = default;
            RunSync(async () => { result = await task(); });
            return result!;
        }

        /// <summary>
        /// Delays for TimeSpan and returns true if there was an exception but does not throw exception if canceled.
        /// </summary>
        /// <param name="timeout">How long to wait.</param>
        /// <param name="cancellation">Cancellation token to cancel the wait.</param>
        /// <returns>Task of boolean.</returns>
        public static Task<bool> DelayWithoutException(TimeSpan timeout, CancellationToken cancellation) =>
            Task.Delay(timeout, cancellation).ContinueWith(tsk => tsk.Exception == default);

        /// <summary>
        /// Delays for TimeSpan and returns true if there was an exception but does not throw exception if canceled.
        /// </summary>
        /// <param name="timeoutMs">How long to wait in milliseconds.</param>
        /// <param name="cancellation">Cancellation token to cancel the wait.</param>
        /// <returns>Task of boolean.</returns>
        public static Task<bool> DelayWithoutException(int timeoutMs, CancellationToken cancellation) =>
            Task.Delay(timeoutMs, cancellation).ContinueWith(tsk => tsk.Exception == default);

        /// <summary>
        /// Synchronization context that can be "pumped" in order to have it execute continuations posted back to it
        /// </summary>
        private class CustomSynchronizationContext : SynchronizationContext
        {
            private readonly ConcurrentQueue<Tuple<SendOrPostCallback, object?>> _items = new ConcurrentQueue<Tuple<SendOrPostCallback, object?>>();
            private readonly AutoResetEvent _workItemsWaiting = new AutoResetEvent(false);
            private readonly Func<Task> _task;
            private ExceptionDispatchInfo? _caughtException = null;
            private bool _done;

            /// <summary>
            /// Constructor for the custom context
            /// </summary>
            /// <param name="task">Task to execute</param>
            public CustomSynchronizationContext(Func<Task> task) =>
                _task = task ?? throw new ArgumentNullException(nameof(task), @"Please remember to pass a Task to be executed");

            /// <summary>
            /// When overridden in a derived class, dispatches an asynchronous message to a synchronization context.
            /// </summary>
            /// <param name="function">Callback function</param>
            /// <param name="state">Callback state</param>
            public override void Post(SendOrPostCallback function, object? state)
            {
                _items.Enqueue(Tuple.Create(function, state));
                _workItemsWaiting.Set();
            }

            /// <summary>
            /// Enqueues the function to be executed and executes all resulting continuations until it is completely done
            /// </summary>
            public void Run()
            {
                async void PostCallback(object? e)
                {
                    try
                    {
                        await _task().ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        _caughtException = ExceptionDispatchInfo.Capture(exception);
                        throw;
                    }
                    finally
                    {
                        Post(_ => _done = true, null);
                    }
                }

                Post(PostCallback, null);

                while (!_done)
                {
                    if (_items.TryDequeue(out var task))
                    {
                        task.Item1(task.Item2);
                        if (_caughtException == null)
                        {
                            continue;
                        }

                        _caughtException.Throw();
                    }
                    else
                    {
                        _workItemsWaiting.WaitOne();
                    }
                }
            }

            /// <summary>
            /// When overridden in a derived class, dispatches a synchronous message to a synchronization context.
            /// </summary>
            /// <param name="function">Callback function</param>
            /// <param name="state">Callback state</param>
            public override void Send(SendOrPostCallback function, object? state) => throw new NotSupportedException("Cannot send to same thread");

            /// <summary>
            /// When overridden in a derived class, creates a copy of the synchronization context. Not needed, so just return ourselves.
            /// </summary>
            /// <returns>Copy of the context</returns>
            public override SynchronizationContext CreateCopy() => this;
        }
    }
}
