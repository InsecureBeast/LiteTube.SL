using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Segments
{
  public interface ISegmentManagerFactory : IContentServiceFactory<ISegmentManager, ISegmentManagerParameters>
  {
    Task<ISegmentManager> CreateAsync(ISegmentManagerParameters parameters, CancellationToken cancellationToken);
  }
}
