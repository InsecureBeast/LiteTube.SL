using SM.Media.Core.M3U8;
using SM.Media.Core.Utility;
using SM.Media.Core.Web;

namespace SM.Media.Core.Hls
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
