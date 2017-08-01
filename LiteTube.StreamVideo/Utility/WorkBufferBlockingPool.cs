using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Metadata;

namespace LiteTube.StreamVideo.Utility
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
