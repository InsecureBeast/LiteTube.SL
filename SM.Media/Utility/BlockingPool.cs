// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.BlockingPool`1
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Utility
{
  public sealed class BlockingPool<TItem> : IBlockingPool<TItem>, IDisposable where TItem : class, new()
  {
    private readonly Queue<TItem> _pool = new Queue<TItem>();
    private readonly Queue<CancellationTaskCompletionSource<TItem>> _waiters = new Queue<CancellationTaskCompletionSource<TItem>>();
    private readonly List<TItem> _allocationTracker = new List<TItem>();
    private readonly int _poolSize;
    private int _allocationCount;

    public BlockingPool(int poolSize)
    {
      this._poolSize = poolSize;
    }

    public Task<TItem> AllocateAsync(CancellationToken cancellationToken)
    {
      bool lockTaken = false;
      Queue<TItem> queue;
      try
      {
        Monitor.Enter((object) (queue = this._pool), ref lockTaken);
        if (this._pool.Count > 0)
        {
          TItem obj = this._pool.Dequeue();
          Debug.Assert(!EqualityComparer<TItem>.Default.Equals(default (TItem), obj));
          return TaskEx.FromResult<TItem>(obj);
        }
        if (this._allocationCount >= this._poolSize)
        {
          CancellationTaskCompletionSource<TItem> completionSource = new CancellationTaskCompletionSource<TItem>((Action<CancellationTaskCompletionSource<TItem>>) (wh => QueueExtensions.Remove<CancellationTaskCompletionSource<TItem>>(this._waiters, wh)), cancellationToken);
          this._waiters.Enqueue(completionSource);
          return completionSource.Task;
        }
        ++this._allocationCount;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) queue);
      }
      TItem instance = Activator.CreateInstance<TItem>();
      this._allocationTracker.Add(instance);
      return TaskEx.FromResult<TItem>(instance);
    }

    public void Free(TItem item)
    {
      if (EqualityComparer<TItem>.Default.Equals(default (TItem), item))
        throw new ArgumentNullException("item");
      bool lockTaken = false;
      Queue<TItem> queue;
      try
      {
        Monitor.Enter((object) (queue = this._pool), ref lockTaken);
        Debug.Assert(this._allocationTracker.Contains(item), "Unknown item has been freed");
        Debug.Assert(!this._pool.Contains(item), "Item is already in pool");
        while (this._waiters.Count > 0)
        {
          Debug.Assert(0 == this._pool.Count, "The pool should be empty when there are waiters");
          if (this._waiters.Dequeue().TrySetResult(item))
            return;
        }
        this._pool.Enqueue(item);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) queue);
      }
    }

    public void Dispose()
    {
      this.Clear();
    }

    private void Clear()
    {
      bool lockTaken = false;
      Queue<TItem> queue;
      CancellationTaskCompletionSource<TItem>[] completionSourceArray;
      try
      {
        Monitor.Enter((object) (queue = this._pool), ref lockTaken);
        completionSourceArray = this._waiters.ToArray();
        this._waiters.Clear();
        this._pool.Clear();
        this._allocationCount = 0;
        this._allocationTracker.Clear();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) queue);
      }
      foreach (CancellationTaskCompletionSource<TItem> completionSource in completionSourceArray)
        completionSource.Dispose();
    }
  }
}
