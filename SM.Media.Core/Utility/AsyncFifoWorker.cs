using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Utility
{
  public sealed class AsyncFifoWorker : IDisposable
  {
    private readonly object _lock = new object();
    private readonly Queue<AsyncFifoWorker.WorkHandle> _workQueue = new Queue<AsyncFifoWorker.WorkHandle>();
    private readonly SignalTask _signalTask;
    private bool _isClosed;
    private AsyncFifoWorker.WorkHandle _work;

    public AsyncFifoWorker(CancellationToken cancellationToken)
    {
      this._signalTask = new SignalTask(new Func<Task>(this.Worker), cancellationToken);
    }

    public AsyncFifoWorker()
    {
      this._signalTask = new SignalTask(new Func<Task>(this.Worker));
    }

    public void Dispose()
    {
      if (!this.Close())
        return;
      using (this._signalTask)
        ;
      bool lockTaken = false;
      object obj = null;
      AsyncFifoWorker.WorkHandle[] workHandleArray;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        workHandleArray = this._workQueue.ToArray();
        this._workQueue.Clear();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      foreach (AsyncFifoWorker.WorkHandle workHandle in workHandleArray)
        workHandle.Dispose();
    }

    private async Task Worker()
    {
      while (true)
      {
        bool lockTaken = false;
        AsyncFifoWorker.WorkHandle work;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._lock, ref lockTaken);
          if (this._workQueue.Count >= 1)
            work = this._workQueue.Dequeue();
          else
            break;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
        this._work = work;
        try
        {
          work.TryDeregister();
          await work.RunAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
          Debug.WriteLine("AsyncFifoWorker.Worker() failed: " + ExceptionExtensions.ExtendedMessage(ex));
        }
        this._work = (AsyncFifoWorker.WorkHandle) null;
        work.Dispose();
      }
    }

    private void RemoveWork(object workObject)
    {
      AsyncFifoWorker.WorkHandle workHandle = (AsyncFifoWorker.WorkHandle) workObject;
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        if (!QueueExtensions.Remove<AsyncFifoWorker.WorkHandle>(this._workQueue, workHandle))
          return;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      workHandle.Dispose();
    }

    public void Post(Func<Task> workFunc, string description, CancellationToken cancellationToken)
    {
      this.PostWork(workFunc, false, description, cancellationToken);
    }

    public Task PostAsync(Func<Task> workFunc, string description, CancellationToken cancellationToken)
    {
      return this.PostWork(workFunc, true, description, cancellationToken).Task;
    }

    private AsyncFifoWorker.WorkHandle PostWork(Func<Task> workFunc, bool createTcs, string description, CancellationToken cancellationToken)
    {
      if (workFunc == null)
        throw new ArgumentNullException("workFunc");
      cancellationToken.ThrowIfCancellationRequested();
      bool lockTaken = false;
      object obj = null;
      AsyncFifoWorker.WorkHandle workHandle;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        if (this._isClosed)
          throw new InvalidOperationException("AsyncFifoWorker is closed");
        workHandle = new AsyncFifoWorker.WorkHandle(workFunc, createTcs ? new TaskCompletionSource<object>() : (TaskCompletionSource<object>) null);
        workHandle.Description = description;
        this._workQueue.Enqueue(workHandle);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (!workHandle.Register(new Action<AsyncFifoWorker.WorkHandle>(this.RemoveWork), cancellationToken))
      {
        Debug.WriteLine("AsyncFifoWorker.PostWork() work already done");
        return workHandle;
      }
      try
      {
        this._signalTask.Fire();
      }
      catch (ObjectDisposedException ex)
      {
        this.RemoveWork((object) workHandle);
        if (this._workQueue.Count > 0)
          Debug.WriteLine("AsyncFifoWorker.Post() object disposed but there are still {0} pending", (object) this._workQueue.Count);
        throw;
      }
      return workHandle;
    }

    public Task CloseAsync()
    {
      this.Close();
      return this._signalTask.WaitAsync();
    }

    private bool Close()
    {
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        if (this._isClosed)
          return false;
        this._isClosed = true;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      return true;
    }

    private sealed class WorkHandle : IDisposable
    {
      private readonly TaskCompletionSource<object> _taskCompletionSource;
      private readonly Func<Task> _work;
      private CancellationTokenRegistration _cancellationTokenRegistration;
      private int _state;

      public string Description { get; set; }

      public Task Task
      {
        get
        {
          return this._taskCompletionSource == null ? (Task) null : (Task) this._taskCompletionSource.Task;
        }
      }

      public WorkHandle(Func<Task> work, TaskCompletionSource<object> taskCompletionSource)
      {
        if (null == work)
          throw new ArgumentNullException("work");
        this._work = work;
        this._taskCompletionSource = taskCompletionSource;
      }

      public void Dispose()
      {
        if (AsyncFifoWorker.WorkHandle.State.Disposed == this.SetState(AsyncFifoWorker.WorkHandle.State.Disposed))
          return;
        this.TryDeregister();
        if (null == this._taskCompletionSource)
          return;
        this._taskCompletionSource.TrySetCanceled();
      }

      public bool Register(Action<AsyncFifoWorker.WorkHandle> removeWork, CancellationToken cancellationToken)
      {
        if (AsyncFifoWorker.WorkHandle.State.Free != this.SetState(AsyncFifoWorker.WorkHandle.State.Registered))
          return false;
        this._cancellationTokenRegistration = cancellationToken.Register((Action<object>) (w => removeWork((AsyncFifoWorker.WorkHandle) w)), (object) this);
        return true;
      }

      public bool TryDeregister()
      {
        if (AsyncFifoWorker.WorkHandle.State.Registered != this.SetState(AsyncFifoWorker.WorkHandle.State.Deregistered))
          return false;
        DisposeExtensions.DisposeSafe((IDisposable) this._cancellationTokenRegistration);
        return true;
      }

      private AsyncFifoWorker.WorkHandle.State SetState(AsyncFifoWorker.WorkHandle.State state)
      {
        return (AsyncFifoWorker.WorkHandle.State) Interlocked.Exchange(ref this._state, (int) state);
      }

      public async Task RunAsync()
      {
        TaskCompletionSource<object> tcs = this._taskCompletionSource;
        try
        {
          await this._work().ConfigureAwait(false);
          if (null != tcs)
            tcs.TrySetResult((object) string.Empty);
        }
        catch (OperationCanceledException ex)
        {
          if (null != tcs)
            tcs.TrySetCanceled();
        }
        catch (Exception ex)
        {
          if (tcs == null || !tcs.TrySetException(ex))
            Debug.WriteLine("AsyncFifoWorker.WorkHandle.RunAsync() work should not throw exceptions: " + ExceptionExtensions.ExtendedMessage(ex));
          else
            goto label_9;
        }
label_9:;
      }

      private enum State
      {
        Free,
        Registered,
        Deregistered,
        Disposed,
      }
    }
  }
}
