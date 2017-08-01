using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.M3U8;
using LiteTube.StreamVideo.Segments;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Hls
{
    public interface IHlsSegmentsFactory
    {
        Task<ICollection<ISegment>> CreateSegmentsAsync(M3U8Parser parser, IWebReader webReader, CancellationToken cancellationToken);
    }
}
