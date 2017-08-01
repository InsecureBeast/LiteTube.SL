using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Segments
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
