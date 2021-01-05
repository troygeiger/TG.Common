using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TG.Common
{
    public class DelayedMethodInvoker : IDisposable
    {

        private Delegate Exe;
        private System.Threading.Timer tmer;
        private int Delay = 0;
        private bool isRunning = false;

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="del">The delegate to execute once the delay time has elapsed.</param>
        /// <param name="DelayTime">The time, in milliseconds to wait before executing the delegate.</param>
        /// <param name="args">The arguments to be passed to the delegate when executed.</param>
        public DelayedMethodInvoker(Delegate del, int DelayTime, params object[] args)
        {
            Exe = del;
            Delay = DelayTime;
            tmer = new System.Threading.Timer(new System.Threading.TimerCallback(TimesUp)
                                              , args
                                              , System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

        }

#if !NET20
        /// <summary>
        /// Set this property if you would like to provide what <see cref="System.Threading.Thread"/> should be returned to when invoking the delegate.
        /// </summary>
        public Thread ReturnThread { get; set; }
#endif

        /// <summary>
        /// Starts the delay timer and then invokes the method once the delay has elapsed.
        /// </summary>
        public void Invoke()
        {
            tmer.Change(Delay, System.Threading.Timeout.Infinite);
            isRunning = true;
        }

        /// <summary>
        /// Starts the delay timer and then invokes the method once the delay has elapsed.
        /// </summary>
        /// <param name="args">The arguments to pass to the method.</param>
        public void Invoke(params object[] args)
        {
            tmer.Dispose();
            tmer = new System.Threading.Timer(new System.Threading.TimerCallback(TimesUp)
                                              , args
                                              , System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            this.Invoke();
        }

        private void TimesUp(object args)
        {

            if (Exe != null)
            {
                try
                {
                    Delegate[] Dels = Exe.GetInvocationList();
                    System.ComponentModel.ISynchronizeInvoke Synchronizer = null;

                    if (Exe.Target != null && typeof(System.ComponentModel.ISynchronizeInvoke).IsAssignableFrom(Exe.Target.GetType()))
                    {
                        Synchronizer = (System.ComponentModel.ISynchronizeInvoke)Exe.Target;
                    }
#if NET45
                    else if (ReturnThread != null)
                    {
                        var dispatcher = System.Windows.Threading.Dispatcher.FromThread(ReturnThread);
                        dispatcher.Invoke(Exe);
                        return;
                    }
#endif
                    else
                    {
                        Exe.DynamicInvoke((object[])args);
                        return;
                    }

                    foreach (Delegate sync in Dels)
                    {
                        if (Synchronizer != null)
                        {
                            if (Synchronizer.InvokeRequired)
                            {
                                Synchronizer.Invoke(sync, (object[])args);
                            }
                            else
                            {
                                sync.DynamicInvoke((object[])args);
                            }
                        }
                        else
                        {
                            sync.DynamicInvoke((object[])args);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    isRunning = false;
                }

            }

        }

        /// <summary>
        /// Create a new instance of <see cref="DelayedMethodInvoker"/>.
        /// </summary>
        /// <param name="del">The delegate to execute once the delay time has elapsed.</param>
        /// <param name="DelayTime">The time, in milliseconds to wait before executing the delegate.</param>
        /// <param name="args">The arguments to be passed to the delegate when executed.</param>
        /// <returns>A new instance of <see cref="DelayedMethodInvoker"/>.</returns>
        public static DelayedMethodInvoker CreateMethodInvoker(Delegate del, int DelayTime, params object[] args)
        {
            return new DelayedMethodInvoker(del, DelayTime, args);
        }

        /// <summary>
        /// Starts or Restarts the timer.
        /// </summary>
        public void RestartTimer()
        {
            Invoke();
        }

        /// <summary>
        /// Cancels the delay timer.
        /// </summary>
        public void Cancel()
        {
            tmer.Change(System.Threading.Timeout.Infinite
                        , System.Threading.Timeout.Infinite);
            isRunning = false;
        }

        /// <summary>
        /// Determines if the timer is set to invoke.
        /// </summary>
        public bool IsRunning { get { return isRunning; } }

        public void Dispose()
        {
            tmer.Dispose();
            isRunning = false;
        }
    }
}
