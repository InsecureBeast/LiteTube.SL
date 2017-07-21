using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;

namespace SM.Media.Core.Segments
{
  public static class SegmentReaderManagerFactoryExtensions
  {
    public static Task<ISegmentReaderManager> CreateAsync(this ISegmentReaderManagerFactory factory, ISegmentManagerParameters parameters, CancellationToken cancellationToken)
    {
      return factory.CreateAsync(parameters, (ContentType) null, cancellationToken);
    }
  }
}
