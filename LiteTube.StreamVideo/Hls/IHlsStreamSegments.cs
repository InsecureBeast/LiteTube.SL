using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Segments;

namespace LiteTube.StreamVideo.Hls
{
    public interface IHlsStreamSegments
    {
        Task<ICollection<ISegment>> CreateSegmentsAsync(CancellationToken cancellationToken);
    }
}
