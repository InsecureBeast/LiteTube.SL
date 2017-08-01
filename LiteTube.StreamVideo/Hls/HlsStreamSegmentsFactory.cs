using LiteTube.StreamVideo.M3U8;
using LiteTube.StreamVideo.Utility;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Hls
{
    public class HlsStreamSegmentsFactory : IHlsStreamSegmentsFactory
    {
        private readonly IPlatformServices _platformServices;
        private readonly IRetryManager _retryManager;

        public HlsStreamSegmentsFactory(IRetryManager retryManager, IPlatformServices platformServices)
        {
            _retryManager = retryManager;
            _platformServices = platformServices;
        }

        public IHlsStreamSegments Create(M3U8Parser parser, IWebReader webReader)
        {
            return new HlsStreamSegments(parser, webReader, _retryManager, _platformServices);
        }
    }
}
