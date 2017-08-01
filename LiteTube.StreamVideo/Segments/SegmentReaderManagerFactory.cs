using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.Segments
{
    public class SegmentReaderManagerFactory : ISegmentReaderManagerFactory
    {
        private readonly IPlatformServices _platformServices;
        private readonly IRetryManager _retryManager;
        private readonly ISegmentManagerFactory _segmentManagerFactory;
        private readonly IWebMetadataFactory _webMetadataFactory;

        public SegmentReaderManagerFactory(ISegmentManagerFactory segmentManagerFactory, IWebMetadataFactory webMetadataFactory, IRetryManager retryManager, IPlatformServices platformServices)
        {
            if (null == segmentManagerFactory)
                throw new ArgumentNullException(nameof(segmentManagerFactory));
            if (null == webMetadataFactory)
                throw new ArgumentNullException(nameof(webMetadataFactory));
            if (null == retryManager)
                throw new ArgumentNullException(nameof(retryManager));
            if (null == platformServices)
                throw new ArgumentNullException(nameof(platformServices));

            _segmentManagerFactory = segmentManagerFactory;
            _webMetadataFactory = webMetadataFactory;
            _retryManager = retryManager;
            _platformServices = platformServices;
        }

        public async Task<ISegmentReaderManager> CreateAsync(ISegmentManagerParameters parameters, ContentType contentType, CancellationToken cancellationToken)
        {
            ISegmentManager playlist;
            if (null == contentType)
                playlist = await _segmentManagerFactory.CreateAsync(parameters, cancellationToken).ConfigureAwait(false);
            else
                playlist = await _segmentManagerFactory.CreateAsync(parameters, contentType, cancellationToken).ConfigureAwait(false);

            if (null == playlist)
                throw new FileNotFoundException("Unable to create playlist");

            var segmentManagers = new[] { playlist };
            return new SegmentReaderManager(segmentManagers, _webMetadataFactory, _retryManager, _platformServices);
        }
    }
}
