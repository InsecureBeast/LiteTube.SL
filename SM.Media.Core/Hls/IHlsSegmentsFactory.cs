using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.M3U8;
using SM.Media.Core.Segments;
using SM.Media.Core.Web;

namespace SM.Media.Core.Hls
{
    public interface IHlsSegmentsFactory
    {
        Task<ICollection<ISegment>> CreateSegmentsAsync(M3U8Parser parser, IWebReader webReader, CancellationToken cancellationToken);
    }
}
