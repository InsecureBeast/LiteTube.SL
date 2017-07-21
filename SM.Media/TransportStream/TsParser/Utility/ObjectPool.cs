// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.Utility.ObjectPool`1
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace SM.Media.TransportStream.TsParser.Utility
{
  internal sealed class ObjectPool<T> : IDisposable where T : new()
  {
    private readonly Stack<T> _pool = new Stack<T>();

    public T Allocate()
    {
      bool lockTaken = false;
      Stack<T> stack;
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
      Stack<T> stack;
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
      Stack<T> stack;
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
