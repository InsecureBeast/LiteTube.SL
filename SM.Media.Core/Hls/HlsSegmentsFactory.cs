using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.M3U8;
using SM.Media.Core.Segments;
using SM.Media.Core.Web;

namespace SM.Media.Core.Hls
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
