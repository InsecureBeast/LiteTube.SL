using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Buffering;

namespace LiteTube.StreamVideo.Utility
{
  public sealed class QueueWorker<TWorkItem> : IQueueThrottling, IDisposable where TWorkItem : class
  {
    private readonly CancellationTokenSource _abortTokenSource = new CancellationTokenSource();
    private readonly TaskCompletionSource<bool> _closeTaskCompletionSource = new TaskCompletionSource<bool>();
    private readonly LinkedList<TWorkItem> _processBuffers = new LinkedList<TWorkItem>();
    private readonly object _processLock = new object();
    private readonly Action<TWorkItem> _callback;
    private readonly Action<TWorkItem> _cleanup;
    private readonly SignalTask _workerTask;
    private Exception _exception;
    private bool _isClosed;
    private int _isDisposed;
    private bool _isEnabled;
    private bool _isPaused;

    public Exception Exception
    {
      get
      {
        return this._exception;
      }
    }

    public bool IsEnabled
    {
      get
      {
        return !this._isClosed && this._isEnabled;
      }
      set
      {
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._processLock, ref lockTaken);
          if (this._isClosed || value == this._isEnabled)
            return;
          this._isEnabled = value;
          if (this._isEnabled && !this._isPaused && this._processBuffers.Count > 0)
            this.UnlockedWakeWorker();
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
        if (value)
          return;
        this.Clear(false);
      }
    }

    private bool IsPaused
    {
      get
      {
        return this._isPaused;
      }
      set
      {
        this.ThrowIfDisposed();
        if (this._isClosed || this._isPaused == value)
          return;
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._processLock, ref lockTaken);
          this._isPaused = value;
          if (this._isEnabled && !this._isPaused && this._processBuffers.Count > 0)
            this.UnlockedWakeWorker();
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
      }
    }

    public QueueWorker(Action<TWorkItem> callback, Action<TWorkItem> cleanup)
    {
      this._callback = callback;
      this._cleanup = cleanup;
      this._workerTask = SignalTask.Create(new Action(this.Worker));
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      this._abortTokenSource.Cancel();
      this.Clear(true);
      this._workerTask.Dispose();
      this._abortTokenSource.Dispose();
    }

    public void Pause()
    {
      this.IsPaused = true;
    }

    public void Resume()
    {
      this.IsPaused = false;
    }

    public void Enqueue(TWorkItem value)
    {
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._processLock, ref lockTaken);
        if (this._isClosed)
        {
          if (null != this._exception)
            throw new AggregateException(new Exception[1]
            {
              this._exception
            });
          throw new InvalidOperationException("The worker is closed");
        }
        this.ThrowIfDisposed();
        this._processBuffers.AddLast(value);
        if (this._isPaused)
          return;
        this.UnlockedWakeWorker();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
    }

    private void UnlockedWakeWorker()
    {
      this._workerTask.Fire();
    }

    private void ThrowIfDisposed()
    {
      if (0 != this._isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    private void Clear(bool closeQueue)
    {
      TWorkItem[] array = (TWorkItem[]) null;
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._processLock, ref lockTaken);
        if (this._processBuffers.Count > 0)
        {
          array = new TWorkItem[this._processBuffers.Count];
          this._processBuffers.CopyTo(array, 0);
          this._processBuffers.Clear();
        }
        if (closeQueue)
        {
          if (!this._isClosed)
            this._isClosed = true;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (null == array)
        return;
      foreach (TWorkItem workItem in array)
      {
        if (null != (object) workItem)
          this._cleanup(workItem);
      }
    }

    public Task FlushAsync()
    {
      this.IsPaused = false;
      return this._workerTask.WaitAsync();
    }

    public Task ClearAsync()
    {
      this.Clear(false);
      return this._workerTask.WaitAsync();
    }

    public Task CloseAsync()
    {
      this.Clear(true);
      return this._workerTask.WaitAsync();
    }

    private void Worker()
    {
      bool condition = false;
      try
      {
        while (true)
        {
          this.ThrowIfDisposed();
          TWorkItem workItem = default (TWorkItem);
          object obj = null;
          try
          {
            bool lockTaken = false;
            try
            {
              Monitor.Enter(obj = this._processLock, ref lockTaken);
              if (!this._isEnabled || this._isClosed || this._isPaused || this._closeTaskCompletionSource.Task.IsCompleted)
              {
                condition = true;
                break;
              }
              if (0 == this._processBuffers.Count)
              {
                condition = true;
                break;
              }
              LinkedListNode<TWorkItem> first = this._processBuffers.First;
              this._processBuffers.RemoveFirst();
              workItem = first.Value;
            }
            finally
            {
              if (lockTaken)
                Monitor.Exit(obj);
            }
            this._callback(workItem);
          }
          catch (OperationCanceledException ex)
          {
            condition = true;
            break;
          }
          catch (Exception ex)
          {
            Debug.WriteLine("Callback failed: " + ex.Message);
            bool lockTaken = false;
            try
            {
              Monitor.Enter(obj = this._processLock, ref lockTaken);
              this._exception = ex;
            }
            finally
            {
              if (lockTaken)
                Monitor.Exit(obj);
            }
            this.Clear(true);
            condition = true;
            break;
          }
          finally
          {
            if (null != (object) workItem)
              this._cleanup(workItem);
          }
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("WorkAsync failed: " + ex.Message);
      }
      finally
      {
        Debug.Assert(condition, "QueueWorker.WorkerAsync() exited unexpectedly");
      }
    }
  }
}
