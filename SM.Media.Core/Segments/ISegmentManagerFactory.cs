using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;

namespace SM.Media.Core.Segments
{
  public interface ISegmentManagerFactory : IContentServiceFactory<ISegmentManager, ISegmentManagerParameters>
  {
    Task<ISegmentManager> CreateAsync(ISegmentManagerParameters parameters, CancellationToken cancellationToken);
  }
}
