using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.Platform
{
    public sealed class SingleThreadSignalTaskScheduler : TaskScheduler, IDisposable
    {
        private readonly object _lock = new object();
        private readonly Queue<Task> _tasks = new Queue<Task>();
        private readonly Thread _thread;
        private bool _isDone;
        private bool _isSignaled;
        private Action _signalHandler;

        public override int MaximumConcurrencyLevel
        {
            get
            {
                return 1;
            }
        }

        public SingleThreadSignalTaskScheduler(string name, Action signalHandler)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (signalHandler == null)
                throw new ArgumentNullException("signalHandler");
            this._signalHandler = signalHandler;
            this._thread = new Thread(new ThreadStart(this.Run))
            {
                Name = name
            };
            this._thread.Start();
        }

        public void Dispose()
        {
            lock (this._lock)
            {
                this._isDone = true;
                Monitor.PulseAll(this._lock);
                this._signalHandler = (Action)null;
            }
            if (null != this._thread)
                this._thread.Join();
            if (null == this._tasks)
                return;
            this._tasks.Clear();
        }

        public void Signal()
        {
            lock (this._lock)
            {
                if (this._isSignaled)
                    return;
                this._isSignaled = true;
                Monitor.Pulse(this._lock);
            }
        }

        private void Run()
        {
            try
            {
                while (true)
                {
                    bool flag1 = true;
                    bool flag2 = false;
                    Action action;
                    Task task1;
                    lock (this._lock)
                    {
                        while (true)
                        {
                            flag1 = true;
                            if (!this._isDone && null != this._signalHandler)
                            {
                                action = this._signalHandler;
                                bool local_3 = false;
                                task1 = (Task)null;
                                if (this._tasks.Count > 0)
                                {
                                    task1 = this._tasks.Dequeue();
                                    local_3 = true;
                                }
                                if (this._isSignaled)
                                {
                                    this._isSignaled = false;
                                    flag2 = true;
                                    local_3 = true;
                                }
                                if (!local_3)
                                    Monitor.Wait(this._lock);
                                else
                                    goto label_14;
                            }
                            else
                                break;
                        }
                        break;
                    }
                    label_14:
                    if (flag2)
                    {
                        Task task2 = new Task(action);
                        task2.RunSynchronously((TaskScheduler)this);
                        AggregateException exception = task2.Exception;
                        if (null != exception)
                            Debug.WriteLine("SingleThreadSignalTaskScheduler.Run() signal handler failed: " + ExceptionExtensions.ExtendedMessage((Exception)exception));
                    }
                    if (null != task1)
                        this.TryExecuteTask(task1);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SingleThreadSignalTaskScheduler.Run() failed: " + ex.Message);
            }
        }

        protected override void QueueTask(Task task)
        {
            lock (this._lock)
            {
                this._tasks.Enqueue(task);
                Monitor.Pulse(this._lock);
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            lock (this._lock)
                return (IEnumerable<Task>)this._tasks.ToArray();
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (Thread.CurrentThread != this._thread || taskWasPreviouslyQueued)
                return false;
            return this.TryExecuteTask(task);
        }

        [Conditional("DEBUG")]
        public void ThrowIfNotOnThread()
        {
            if (Thread.CurrentThread != this._thread)
                throw new InvalidOperationException("Not running on worker thread");
        }
    }
}
