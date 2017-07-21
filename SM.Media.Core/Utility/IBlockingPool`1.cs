using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Utility
{
  public interface IBlockingPool<TItem> : IDisposable where TItem : new()
  {
    Task<TItem> AllocateAsync(CancellationToken cancellationToken);

    void Free(TItem item);
  }
}
