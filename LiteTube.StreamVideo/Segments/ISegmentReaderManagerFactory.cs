using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Segments
{
  public interface ISegmentReaderManagerFactory
  {
    Task<ISegmentReaderManager> CreateAsync(ISegmentManagerParameters parameters, ContentType contentType, CancellationToken cancellationToken);
  }
}
