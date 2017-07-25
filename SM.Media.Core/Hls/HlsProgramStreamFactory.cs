using System;
using System.Collections.Generic;
using SM.Media.Core.Metadata;
using SM.Media.Core.Utility;
using SM.Media.Core.Web;

namespace SM.Media.Core.Hls
{
    public class HlsProgramStreamFactory : IHlsProgramStreamFactory
    {
        private readonly IPlatformServices _platformServices;
        private readonly IRetryManager _retryManager;
        private readonly IHlsSegmentsFactory _segmentsFactory;
        private readonly IWebMetadataFactory _webMetadataFactory;

        public HlsProgramStreamFactory(IHlsSegmentsFactory segmentsFactory, IWebMetadataFactory webMetadataFactory, IPlatformServices platformServices, IRetryManager retryManager)
        {
            if (null == segmentsFactory)
                throw new ArgumentNullException(nameof(segmentsFactory));
            if (null == webMetadataFactory)
                throw new ArgumentNullException(nameof(webMetadataFactory));
            if (null == platformServices)
                throw new ArgumentNullException(nameof(platformServices));
            if (null == retryManager)
                throw new ArgumentNullException(nameof(retryManager));
            _segmentsFactory = segmentsFactory;
            _webMetadataFactory = webMetadataFactory;
            _platformServices = platformServices;
            _retryManager = retryManager;
        }

        public IHlsProgramStream Create(ICollection<Uri> urls, IWebReader webReader)
        {
            return new HlsProgramStream(webReader, urls, _segmentsFactory, _webMetadataFactory, _platformServices, _retryManager);
        }
    }
}
