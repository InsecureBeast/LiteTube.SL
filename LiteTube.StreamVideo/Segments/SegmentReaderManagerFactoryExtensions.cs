using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Segments
{
  public static class SegmentReaderManagerFactoryExtensions
  {
    public static Task<ISegmentReaderManager> CreateAsync(this ISegmentReaderManagerFactory factory, ISegmentManagerParameters parameters, CancellationToken cancellationToken)
    {
      return factory.CreateAsync(parameters, (ContentType) null, cancellationToken);
    }
  }
}
