using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Utility
{
  public sealed class SignalTask : IDisposable
  {
    private readonly object _lock = new object();
    private Func<Task> _handler;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private bool _isDisposed;
    private bool _isPending;
    private Task _task;
    private TaskCompletionSource<bool> _taskCompletionSource;
    private int _callCounter;

    public bool IsActive
    {
      get
      {
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._lock, ref lockTaken);
          return this._isPending || null != this._task;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
      }
    }

    public SignalTask(Func<Task> handler)
    {
      if (handler == null)
        throw new ArgumentNullException("handler");
      this._handler = handler;
      this._cancellationTokenSource = new CancellationTokenSource();
    }

    public SignalTask(Func<Task> handler, CancellationToken token)
    {
      if (handler == null)
        throw new ArgumentNullException("handler");
      this._handler = handler;
      this._cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
    }

    public static SignalTask Create(Action handler)
    {
      return new SignalTask((Func<Task>) (() =>
      {
        handler();
        return TplTaskExtensions.CompletedTask;
      }));
    }

    public static SignalTask Create(Action handler, CancellationToken token)
    {
      return new SignalTask((Func<Task>) (() =>
      {
        handler();
        return TplTaskExtensions.CompletedTask;
      }), token);
    }

    public void Dispose()
    {
      bool lockTaken = false;
      object obj = null;
      Task task;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        if (this._isDisposed)
          return;
        this._isDisposed = true;
        task = this._task;
        this._handler = (Func<Task>) null;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (!this._cancellationTokenSource.IsCancellationRequested)
        this._cancellationTokenSource.Cancel();
      if (null == task)
        return;
      try
      {
        task.Wait();
      }
      catch (OperationCanceledException ex)
      {
      }
      catch (AggregateException ex)
      {
        if (Enumerable.Any<Exception>((IEnumerable<Exception>) ex.Flatten().InnerExceptions, (Func<Exception, bool>) (e => !(e is OperationCanceledException))))
          Debug.WriteLine("SignalTask.Dispose(): " + ExceptionExtensions.ExtendedMessage((Exception) ex));
      }
      catch (Exception ex)
      {
        Debug.WriteLine("SignalTask.Dispose(): " + ExceptionExtensions.ExtendedMessage(ex));
      }
      this._cancellationTokenSource.Dispose();
    }

    public void Fire()
    {
      bool lockTaken1 = false;
      object obj = null;
      Task<Task> task1;
      Task task2;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken1);
        if (this._isDisposed)
          throw new ObjectDisposedException(this.GetType().FullName);
        if (this._isPending || this._cancellationTokenSource.IsCancellationRequested)
          return;
        this._isPending = true;
        if (null != this._task)
          return;
        task1 = new Task<Task>(new Func<Task>(this.CallHandlerAsync), this._cancellationTokenSource.Token, TaskCreationOptions.DenyChildAttach);
        this._task = task2 = TaskExtensions.Unwrap(task1);
        this._taskCompletionSource = (TaskCompletionSource<bool>) null;
      }
      finally
      {
        if (lockTaken1)
          Monitor.Exit(obj);
      }
      try
      {
        if (TaskStatus.Created == task1.Status)
          task1.Start(TaskScheduler.Default);
      }
      catch (Exception ex)
      {
        Debug.WriteLine("SignalTask.Fire() task start failed: " + ex.Message);
        bool lockTaken2 = false;
        try
        {
          Monitor.Enter(obj = this._lock, ref lockTaken2);
          if (object.ReferenceEquals((object) task2, (object) this._task))
          {
            this._task = (Task) null;
            this._taskCompletionSource = (TaskCompletionSource<bool>) null;
            this._isPending = false;
          }
        }
        finally
        {
          if (lockTaken2)
            Monitor.Exit(obj);
        }
        throw;
      }
      if (this._isDisposed)
        throw new ObjectDisposedException(this.GetType().FullName);
    }

    private async Task CallHandlerAsync()
    {
      try
      {
        while (true)
        {
          bool lockTaken = false;
          Func<Task> handler;
          object obj = null;
          try
          {
            Monitor.Enter(obj = this._lock, ref lockTaken);
            if (!this._isPending || this._isDisposed || this._cancellationTokenSource.IsCancellationRequested || null == this._handler)
            {
              this._task = (Task) null;
              break;
            }
            this._isPending = false;
            handler = this._handler;
          }
          finally
          {
            if (lockTaken)
              Monitor.Exit(obj);
          }
          int count = Interlocked.Increment(ref this._callCounter);
          Debug.Assert(1 == count, "SignalTask.CallHandlerAsync(): concurrent call detected");
          try
          {
            await handler().ConfigureAwait(false);
          }
          catch (Exception ex)
          {
            Debug.WriteLine("SignalTask.CallHandlerAsync() handler failed: " + ex.Message);
          }
          finally
          {
            count = Interlocked.Decrement(ref this._callCounter);
            Debug.Assert(0 == count, "SignalTask.CallHandlerAsync(): concurrent call detected after return");
          }
        }
      }
      catch (Exception ex)
      {
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._lock, ref lockTaken);
          this._task = (Task) null;
          this._isPending = false;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
        Debug.WriteLine("SignalTask.CallHandlerAsync() failed: " + ex.Message);
      }
    }

    public Task WaitAsync()
    {
      bool flag = false;
      bool lockTaken = false;
      object obj = null;
      Task task;
      TaskCompletionSource<bool> taskCompletionSource;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        task = this._task;
        if (task == null || task.IsCompleted)
          return TplTaskExtensions.CompletedTask;
        if (null == this._taskCompletionSource)
        {
          this._taskCompletionSource = new TaskCompletionSource<bool>();
          flag = true;
        }
        taskCompletionSource = this._taskCompletionSource;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (flag)
        task.ContinueWith((Action<Task>) (t =>
        {
          if (taskCompletionSource.TrySetResult(true))
            return;
          Debug.WriteLine("SignalTask.WaitAsync() TrySetResult failed, status: " + (object) taskCompletionSource.Task.Status);
        }));
      return (Task) taskCompletionSource.Task;
    }
  }
}
