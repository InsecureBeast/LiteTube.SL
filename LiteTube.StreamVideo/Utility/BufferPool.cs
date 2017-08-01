using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using LiteTube.StreamVideo.TransportStream.TsParser.Utility;

namespace LiteTube.StreamVideo.Utility
{
  public sealed class BufferPool : IBufferPool, IDisposable
  {
    private readonly BufferPool.BufferSubPool[] _pools;
    private int _isDisposed;
    private int _requestedAllocationBytes;
    private int _actualAllocationBytes;
    private int _actualFreeBytes;
    private int _allocationCount;
    private int _freeCount;
    private int _nonPoolAllocationCount;

    public BufferPool(IBufferPoolParameters bufferPoolParameters)
    {
      if (null == bufferPoolParameters)
        throw new ArgumentNullException("bufferPoolParameters");
      this._pools = new BufferPool.BufferSubPool[bufferPoolParameters.Pools];
      int baseSize = bufferPoolParameters.BaseSize;
      for (int index = 0; index < this._pools.Length; ++index)
      {
        this._pools[index] = new BufferPool.BufferSubPool(baseSize);
        baseSize <<= 2;
      }
    }

    private BufferPool.BufferSubPool FindPool(int size)
    {
      int index1 = 0;
      int num = this._pools.Length;
      while (index1 < num)
      {
        int index2 = (num + index1) / 2;
        BufferPool.BufferSubPool bufferSubPool = this._pools[index2];
        if (bufferSubPool.Size == size)
          return bufferSubPool;
        if (bufferSubPool.Size < size)
          index1 = index2 + 1;
        else
          num = index2;
      }
      if (num >= this._pools.Length)
        return (BufferPool.BufferSubPool) null;
      return this._pools[index1];
    }

    public BufferInstance Allocate(int minSize)
    {
      Interlocked.Increment(ref this._allocationCount);
      Interlocked.Add(ref this._requestedAllocationBytes, minSize);
      BufferPool.BufferSubPool pool = this.FindPool(minSize);
      PoolBufferInstance poolBufferInstance;
      if (null != pool)
      {
        poolBufferInstance = pool.Allocate(minSize);
      }
      else
      {
        poolBufferInstance = new PoolBufferInstance(minSize);
        Interlocked.Increment(ref this._nonPoolAllocationCount);
      }
      Interlocked.Add(ref this._actualAllocationBytes, poolBufferInstance.Buffer.Length);
      poolBufferInstance.Reference();
      return poolBufferInstance;
    }

    public void Free(BufferInstance bufferInstance)
    {
      if (!bufferInstance.Dereference())
        return;
      for (int index = 0; index < bufferInstance.Buffer.Length; ++index)
        bufferInstance.Buffer[index] = byte.MaxValue;
      Interlocked.Increment(ref this._freeCount);
      Interlocked.Add(ref this._actualFreeBytes, bufferInstance.Buffer.Length);
      BufferPool.BufferSubPool pool = this.FindPool(bufferInstance.Buffer.Length);
      if (null == pool)
        return;
      if (pool.Size != bufferInstance.Buffer.Length)
        throw new ArgumentException("Invalid buffer size", "bufferInstance");
      pool.Free((PoolBufferInstance) bufferInstance);
    }

    public void Clear()
    {
      Debug.Assert((this._allocationCount != this._freeCount ? 0 : (this._actualAllocationBytes == this._actualFreeBytes ? 1 : 0)) != 0, string.Format("BufferPool.Dispose(): _allocationCount {0} == _freeCount {1} && _actualAllocationBytes {2} == _actualFreeBytes {3}", (object) this._allocationCount, (object) this._freeCount, (object) this._actualAllocationBytes, (object) this._actualFreeBytes));
      foreach (BufferPool.BufferSubPool bufferSubPool in this._pools)
        bufferSubPool.Clear();
      Debug.WriteLine("Pool clear: alloc {0} free {1} req bytes {2}, alloc bytes {3} free bytes {4}", (object) this._allocationCount, (object) this._freeCount, (object) this._requestedAllocationBytes, (object) this._actualAllocationBytes, (object) this._actualFreeBytes);
      Interlocked.Exchange(ref this._allocationCount, 0);
      Interlocked.Exchange(ref this._freeCount, 0);
      Interlocked.Exchange(ref this._requestedAllocationBytes, 0);
      Interlocked.Exchange(ref this._actualAllocationBytes, 0);
      Interlocked.Exchange(ref this._actualFreeBytes, 0);
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      this.Clear();
      if (null == this._pools)
        return;
      foreach (BufferPool.BufferSubPool bufferSubPool in this._pools)
        bufferSubPool.Dispose();
    }

    private sealed class BufferSubPool : IDisposable
    {
      private readonly Stack<PoolBufferInstance> _pool = new Stack<PoolBufferInstance>();
      private readonly List<PoolBufferInstance> _allocationTracker = new List<PoolBufferInstance>();
      public readonly int Size;
      private int _allocationActualSize;
      private int _allocationCount;
      private int _freeCount;
      private int _newAllocationCount;

      public BufferSubPool(int size)
      {
        this.Size = size;
      }

      public PoolBufferInstance Allocate(int actualSize)
      {
        Interlocked.Increment(ref this._allocationCount);
        Interlocked.Add(ref this._allocationActualSize, actualSize);
        Debug.Assert(actualSize <= this.Size);
        bool lockTaken1 = false;
        Stack<PoolBufferInstance> stack = null;
        try
        {
          Monitor.Enter((object) (stack = this._pool), ref lockTaken1);
          if (this._pool.Count > 0)
            return this._pool.Pop();
        }
        finally
        {
          if (lockTaken1)
            Monitor.Exit((object) stack);
        }
        PoolBufferInstance poolBufferInstance = new PoolBufferInstance(this.Size);
        Interlocked.Increment(ref this._newAllocationCount);
        bool lockTaken2 = false;
        List<PoolBufferInstance> list = null;
        try
        {
          Monitor.Enter((object) (list = this._allocationTracker), ref lockTaken2);
          this._allocationTracker.Add(poolBufferInstance);
        }
        finally
        {
          if (lockTaken2)
            Monitor.Exit((object) list);
        }
        return poolBufferInstance;
      }

      public void Free(PoolBufferInstance buffer)
      {
        Interlocked.Increment(ref this._freeCount);
        if (this.Size != buffer.Buffer.Length)
          throw new ArgumentException("Invalid buffer size", "buffer");
        bool lockTaken = false;
        Stack<PoolBufferInstance> stack = null;
        try
        {
          Monitor.Enter((object) (stack = this._pool), ref lockTaken);
          this._pool.Push(buffer);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit((object) stack);
        }
      }

      public void Clear()
      {
        bool lockTaken = false;
        Stack<PoolBufferInstance> stack = null;
        try
        {
          Monitor.Enter((object) (stack = this._pool), ref lockTaken);
          Debug.Assert((this._allocationCount != this._freeCount ? 0 : (this._newAllocationCount == this._pool.Count ? 1 : 0)) != 0, string.Format("BufferSubPool.Dispose(): _allocationCount {0} == _freeCount {1} && _newAllocationCount {2} == _pool.Count {3}", (object) this._allocationCount, (object) this._freeCount, (object) this._newAllocationCount, (object) this._pool.Count));
          if (this._pool.Count != this._allocationTracker.Count)
            Debug.WriteLine("SubPool {0}: Pool size {1} != Tracker {2}", (object) this.Size, (object) this._pool.Count, (object) this._allocationTracker.Count);
          this._pool.Clear();
          Debug.WriteLine("SubPool {0}: new {1} alloc {2} free {3} allocSize {4}", (object) this.Size, (object) this._newAllocationCount, (object) this._allocationCount, (object) this._freeCount, (object) this._allocationActualSize);
          this._allocationTracker.Clear();
          this._newAllocationCount = 0;
          this._freeCount = 0;
          this._allocationActualSize = 0;
          this._allocationCount = 0;
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

      public override string ToString()
      {
        return string.Format("Pool {0}k ({1} free of {2})", (object) ((double) this.Size * 0.0009765625), (object) this._freeCount, (object) this._newAllocationCount);
      }
    }
  }
}
