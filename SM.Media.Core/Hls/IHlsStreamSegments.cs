using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Segments;

namespace SM.Media.Core.Hls
{
  public interface IHlsStreamSegments
  {
    Task<ICollection<ISegment>> CreateSegmentsAsync(CancellationToken cancellationToken);
  }
}
