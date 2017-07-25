using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Web;

namespace SM.Media.Core.Segments
{
    public class SegmentManagerFactory : ContentServiceFactory<ISegmentManager, ISegmentManagerParameters>, ISegmentManagerFactory
    {
        private readonly IWebReaderManager _webReaderManager;

        public SegmentManagerFactory(ISegmentManagerFactoryFinder factoryFinder, IWebReaderManager webReaderManager)
          : base(factoryFinder)
        {
            _webReaderManager = webReaderManager;
        }

        public async Task<ISegmentManager> CreateAsync(ISegmentManagerParameters parameters, CancellationToken cancellationToken)
        {
            foreach (var url in parameters.Source)
            {
                var contentType = await _webReaderManager.DetectContentTypeAsync(url, ContentKind.Unknown, cancellationToken).ConfigureAwait(false);
                if (null == contentType)
                    continue;
                var segmentManager = await CreateAsync(parameters, contentType, cancellationToken).ConfigureAwait(false);
                return segmentManager;
            }

            return null;
        }
    }
}
