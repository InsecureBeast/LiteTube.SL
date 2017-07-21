using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Utility
{
  public sealed class AsyncLock : IDisposable
  {
    private readonly object _lock = new object();
    private Queue<TaskCompletionSource<IDisposable>> _pending = new Queue<TaskCompletionSource<IDisposable>>();
    private bool _isLocked;

    public void Dispose()
    {
      bool lockTaken = false;
      object obj = null;
      TaskCompletionSource<IDisposable>[] completionSourceArray;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        if (null == this._pending)
          return;
        this.CheckInvariant();
        this._isLocked = true;
        if (0 == this._pending.Count)
        {
          this._pending = (Queue<TaskCompletionSource<IDisposable>>) null;
          return;
        }
        completionSourceArray = this._pending.ToArray();
        this._pending.Clear();
        this._pending = (Queue<TaskCompletionSource<IDisposable>>) null;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      foreach (TaskCompletionSource<IDisposable> completionSource in completionSourceArray)
        completionSource.TrySetCanceled();
    }

    [Conditional("DEBUG")]
    private void CheckInvariant()
    {
      Debug.Assert(this._isLocked || this._pending != null && 0 == this._pending.Count, "Either we are locked or we have an empty queue");
    }

    public IDisposable TryLock()
    {
      this.ThrowIfDisposed();
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        this.CheckInvariant();
        if (this._isLocked)
          return (IDisposable) null;
        this._isLocked = true;
        return (IDisposable) new AsyncLock.Releaser(this);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
    }

    public Task<IDisposable> LockAsync(CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      cancellationToken.ThrowIfCancellationRequested();
      bool lockTaken = false;
      object obj = null;
      TaskCompletionSource<IDisposable> tcs;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        this.CheckInvariant();
        if (!this._isLocked)
        {
          this._isLocked = true;
          return TaskEx.FromResult<IDisposable>((IDisposable) new AsyncLock.Releaser(this));
        }
        tcs = new TaskCompletionSource<IDisposable>();
        this._pending.Enqueue(tcs);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (!cancellationToken.CanBeCanceled)
        return tcs.Task;
      return this.CancellableTaskAsync(tcs, cancellationToken);
    }

    private async Task<IDisposable> CancellableTaskAsync(TaskCompletionSource<IDisposable> tcs, CancellationToken cancellationToken)
    {
      IDisposable disposable;
      using (cancellationToken.Register((Action) (() =>
      {
        bool lockTaken = false;
        object obj = null;
        bool flag;
        try
        {
          Monitor.Enter(obj = this._lock, ref lockTaken);
          this.CheckInvariant();
          flag = QueueExtensions.Remove<TaskCompletionSource<IDisposable>>(this._pending, tcs);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
        if (!flag)
          return;
        Task task = TaskEx.Run((Action) (() => tcs.TrySetCanceled()));
        TaskCollector.Default.Add(task, "AsyncLock Propagate cancel");
      })))
        disposable = await tcs.Task.ConfigureAwait(false);
      return disposable;
    }

    private void Release()
    {
      TaskCompletionSource<IDisposable> completionSource;
      do
      {
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._lock, ref lockTaken);
          this.CheckInvariant();
          Debug.Assert(this._isLocked, "AsyncLock.Release() was unlocked");
          if (null == this._pending)
            break;
          if (0 == this._pending.Count)
          {
            this._isLocked = false;
            break;
          }
          completionSource = this._pending.Dequeue();
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
      }
      while (!completionSource.TrySetResult((IDisposable) new AsyncLock.Releaser(this)));
    }

    private void ThrowIfDisposed()
    {
      if (null == this._pending)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    private sealed class Releaser : IDisposable
    {
      private AsyncLock _asyncLock;

      public Releaser(AsyncLock asynclock)
      {
        this._asyncLock = asynclock;
      }

      public void Dispose()
      {
        AsyncLock asyncLock = Interlocked.Exchange<AsyncLock>(ref this._asyncLock, (AsyncLock) null);
        if (null == asyncLock)
          return;
        asyncLock.Release();
      }
    }
  }
}
