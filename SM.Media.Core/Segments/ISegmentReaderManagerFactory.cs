using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;

namespace SM.Media.Core.Segments
{
  public interface ISegmentReaderManagerFactory
  {
    Task<ISegmentReaderManager> CreateAsync(ISegmentManagerParameters parameters, ContentType contentType, CancellationToken cancellationToken);
  }
}
