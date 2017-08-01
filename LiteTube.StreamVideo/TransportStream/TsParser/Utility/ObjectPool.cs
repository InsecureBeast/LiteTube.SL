using System;
using System.Collections.Generic;
using System.Threading;

namespace LiteTube.StreamVideo.TransportStream.TsParser.Utility
{
  internal sealed class ObjectPool<T> : IDisposable where T : new()
  {
    private readonly Stack<T> _pool = new Stack<T>();

    public T Allocate()
    {
      bool lockTaken = false;
      Stack<T> stack = null;
      try
      {
        Monitor.Enter((object) (stack = this._pool), ref lockTaken);
        if (this._pool.Count > 0)
          return this._pool.Pop();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) stack);
      }
      return new T();
    }

    public void Free(T poolObject)
    {
      bool lockTaken = false;
      Stack<T> stack = null;
      try
      {
        Monitor.Enter((object) (stack = this._pool), ref lockTaken);
        this._pool.Push(poolObject);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) stack);
      }
    }

    public void Dispose()
    {
      this.Clear();
    }

    public void Clear()
    {
      bool lockTaken = false;
      Stack<T> stack = null;
      try
      {
        Monitor.Enter((object) (stack = this._pool), ref lockTaken);
        this._pool.Clear();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) stack);
      }
    }
  }
}
