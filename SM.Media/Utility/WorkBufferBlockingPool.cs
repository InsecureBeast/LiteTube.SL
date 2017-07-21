// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.WorkBufferBlockingPool
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Metadata;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Utility
{
  public sealed class WorkBufferBlockingPool : IBlockingPool<WorkBuffer>, IDisposable
  {
    private BlockingPool<WorkBuffer> _pool;

    public WorkBufferBlockingPool(int poolSize)
    {
      this._pool = new BlockingPool<WorkBuffer>(poolSize);
    }

    public void Dispose()
    {
      BlockingPool<WorkBuffer> blockingPool = this._pool;
      this._pool = (BlockingPool<WorkBuffer>) null;
      blockingPool.Dispose();
    }

    public async Task<WorkBuffer> AllocateAsync(CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      WorkBuffer item = await this._pool.AllocateAsync(cancellationToken).ConfigureAwait(false);
      Debug.Assert(null == item.Metadata, "Pending metadata");
      return item;
    }

    public void Free(WorkBuffer item)
    {
      this.ThrowIfDisposed();
      item.Metadata = (ISegmentMetadata) null;
      this._pool.Free(item);
    }

    private void ThrowIfDisposed()
    {
      if (null == this._pool)
        throw new ObjectDisposedException(this.GetType().Name);
    }
  }
}
