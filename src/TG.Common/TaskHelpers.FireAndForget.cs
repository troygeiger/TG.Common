using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Common
{
    public static partial class TaskHelpers
    {
        /// <summary>
        /// Gets or sets the default exception handler for unobserved exceptions in fire-and-forget operations.
        /// </summary>
        /// <remarks>This property allows you to specify a global exception handler for fire-and-forget
        /// operations. Use this to log or process exceptions that would otherwise go unobserved.</remarks>
        public static Action<Exception> DefaultSafeFireAndForgetExceptionHandler { get; set; }

        /// <summary>
        /// Executes a task without awaiting its completion, safely handling exceptions.
        /// </summary>
        /// <remarks>This method is intended for fire-and-forget scenarios where the caller does not need
        /// to await the task. It ensures that any exceptions thrown by the task are caught and handled, preventing
        /// unobserved exceptions. If no <paramref name="onException"/> callback is provided, the method will invoke the
        /// default exception handler, <see cref="DefaultSafeFireAndForgetExceptionHandler"/>, if it is set.</remarks>
        /// <param name="task">The task to execute without awaiting.</param>
        /// <param name="continueOnCapturedContext">A value indicating whether to continue on the captured synchronization context. Set to <see
        /// langword="true"/> to continue on the captured context; otherwise, <see langword="false"/>.</param>
        /// <param name="onException">An optional callback to handle exceptions thrown by the task. If provided, the callback is invoked with the
        /// exception as its argument. If <see langword="null"/>, exceptions are handled by the default exception
        /// handler, if configured.</param>
        public static async void SafeFireAndForget(this Task task, bool continueOnCapturedContext = false, Action<Exception> onException = null)
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
                DefaultSafeFireAndForgetExceptionHandler?.Invoke(ex);
            }
        }

        /// <summary>
        /// Executes the specified task without awaiting its completion, allowing the caller to safely ignore the
        /// result.
        /// </summary>
        /// <remarks>This method is intended for fire-and-forget scenarios where the caller does not need
        /// to await the task. Exceptions thrown by the task are handled by the provided <paramref name="onException"/>
        /// callback, if specified,  and the default exception handler, if configured.</remarks>
        /// <typeparam name="T">The type of the result produced by the task.</typeparam>
        /// <param name="task">The task to execute. Cannot be <see langword="null"/>.</param>
        /// <param name="continueOnCapturedContext">A value indicating whether to continue on the captured synchronization context. Set to <see
        /// langword="true"/> to preserve the synchronization context; otherwise, <see langword="false"/>.</param>
        /// <param name="onException">An optional callback to handle exceptions thrown by the task. If <see langword="null"/>, exceptions will
        /// only be passed to the default exception handler.</param>
        public static async void SafeFireAndForget<T>(this Task<T> task, bool continueOnCapturedContext = false, Action<Exception> onException = null)
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
                DefaultSafeFireAndForgetExceptionHandler?.Invoke(ex);
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

        /// <summary>
        /// Executes a <see cref="ValueTask"/> without awaiting its completion, handling any exceptions that occur.
        /// </summary>
        /// <remarks>This method is intended for fire-and-forget scenarios where the caller does not need
        /// to await the task. It ensures that any exceptions thrown during the execution of the task are caught and
        /// handled. If an <paramref name="onException"/> callback is provided, it will be invoked with the exception.
        /// Additionally, if a default exception handler is configured via
        /// <c>DefaultSafeFireAndForgetExceptionHandler</c>, it will also be invoked.</remarks>
        /// <param name="task">The <see cref="ValueTask"/> to execute.</param>
        /// <param name="continueOnCapturedContext">A value indicating whether to continue on the captured synchronization context. Pass <see langword="true"/>
        /// to preserve the synchronization context; otherwise, pass <see langword="false"/>.</param>
        /// <param name="onException">An optional callback to handle exceptions thrown during the execution of the task. If <see
        /// langword="null"/>, only the default exception handler will be invoked.</param>
        public static async void SafeFireAndForget(this ValueTask task, bool continueOnCapturedContext = false, Action<Exception> onException = null)
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
                DefaultSafeFireAndForgetExceptionHandler?.Invoke(ex);
            }
        }

        /// <summary>
        /// Executes the specified <see cref="ValueTask{T}"/> without awaiting its completion,  safely handling any
        /// exceptions that occur during its execution.
        /// </summary>
        /// <remarks>This method is designed for fire-and-forget scenarios where the caller does not need
        /// to await the task. It ensures that any exceptions thrown during the execution of the task are handled by the
        /// provided  <paramref name="onException"/> callback and the default exception handler, if
        /// configured.</remarks>
        /// <typeparam name="T">The type of the result produced by the <see cref="ValueTask{T}"/>.</typeparam>
        /// <param name="task">The <see cref="ValueTask{T}"/> to execute.</param>
        /// <param name="continueOnCapturedContext">A value indicating whether to continue on the captured synchronization context. Pass <see langword="true"/>
        /// to continue on the captured context; otherwise, <see langword="false"/>.</param>
        /// <param name="onException">An optional callback to invoke if an exception occurs during the execution of the task. If <see
        /// langword="null"/>, no custom exception handling is performed.</param>
        public static async void SafeFireAndForget<T>(this ValueTask<T> task, bool continueOnCapturedContext = false, Action<Exception> onException = null)
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
                DefaultSafeFireAndForgetExceptionHandler?.Invoke(ex);
            }
        }
#endif

    }
}