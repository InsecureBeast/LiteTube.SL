using LiteTube.StreamVideo.M3U8;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Hls
{
    public interface IHlsStreamSegmentsFactory
    {
        IHlsStreamSegments Create(M3U8Parser parser, IWebReader webReader);
    }
}
