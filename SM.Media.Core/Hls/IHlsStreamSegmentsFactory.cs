using SM.Media.Core.M3U8;
using SM.Media.Core.Web;

namespace SM.Media.Core.Hls
{
    public interface IHlsStreamSegmentsFactory
    {
        IHlsStreamSegments Create(M3U8Parser parser, IWebReader webReader);
    }
}
