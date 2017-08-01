using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.M3U8;
using LiteTube.StreamVideo.Segments;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Hls
{
    public class HlsSegmentsFactory : IHlsSegmentsFactory
    {
        private readonly IHlsStreamSegmentsFactory _streamSegmentsFactory;

        public HlsSegmentsFactory(IHlsStreamSegmentsFactory streamSegmentsFactory)
        {
            if (null == streamSegmentsFactory)
                throw new ArgumentNullException(nameof(streamSegmentsFactory));
            _streamSegmentsFactory = streamSegmentsFactory;
        }

        public Task<ICollection<ISegment>> CreateSegmentsAsync(M3U8Parser parser, IWebReader webReader, CancellationToken cancellationToken)
        {
            return _streamSegmentsFactory.Create(parser, webReader).CreateSegmentsAsync(cancellationToken);
        }
    }
}
