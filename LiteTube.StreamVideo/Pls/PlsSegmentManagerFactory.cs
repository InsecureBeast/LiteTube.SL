using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Segments;
using LiteTube.StreamVideo.Utility;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Pls
{
    public class PlsSegmentManagerFactory : ISegmentManagerFactoryInstance
    {
        private static readonly ICollection<ContentType> _types = new[] { ContentTypes.Pls };
        private readonly IPlsSegmentManagerPolicy _plsSegmentManagerPolicy;
        private readonly IRetryManager _retryManager;
        private readonly IWebReaderManager _webReaderManager;

        public PlsSegmentManagerFactory(IWebReaderManager webReaderManager, IPlsSegmentManagerPolicy plsSegmentManagerPolicy, IRetryManager retryManager)
        {
            if (null == webReaderManager)
                throw new ArgumentNullException(nameof(webReaderManager));
            if (null == plsSegmentManagerPolicy)
                throw new ArgumentNullException(nameof(plsSegmentManagerPolicy));
            if (null == retryManager)
                throw new ArgumentNullException(nameof(retryManager));
            _webReaderManager = webReaderManager;
            _plsSegmentManagerPolicy = plsSegmentManagerPolicy;
            _retryManager = retryManager;
        }

        public ICollection<ContentType> KnownContentTypes => _types;

        public virtual async Task<ISegmentManager> CreateAsync(ISegmentManagerParameters parameters, ContentType contentType, CancellationToken cancellationToken)
        {
            foreach (var uri in parameters.Source)
            {
                var localUrl = uri;
                var retry = _retryManager.CreateWebRetry(3, 333);
                var segmentManager = await retry.CallAsync(async () =>
                {
                    var webReader = _webReaderManager.CreateReader(localUrl, ContentTypes.Pls.Kind, null, ContentTypes.Pls);
                    ISegmentManager segmentManager2;
                    try
                    {
                        using (var webStreamResponse = await webReader.GetWebStreamAsync(localUrl, false, cancellationToken, null, new long?(), new long?()))
                        {
                            if (!webStreamResponse.IsSuccessStatusCode)
                            {
                                webReader.Dispose();
                                segmentManager2 = null;
                            }
                            else
                            {
                                using (var stream = await webStreamResponse.GetStreamAsync(cancellationToken))
                                    segmentManager2 = await ReadPlaylistAsync(webReader, webStreamResponse.ActualUrl, stream, cancellationToken);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        webReader.Dispose();
                        throw;
                    }
                    return segmentManager2;
                }, cancellationToken);

                if (null == segmentManager)
                    continue;
                
                return segmentManager;
            }
            return null;
        }

        protected virtual async Task<ISegmentManager> CreateManagerAsync(PlsParser pls, IWebReader webReader, CancellationToken cancellationToken)
        {
            var trackUrl = await _plsSegmentManagerPolicy.GetTrackAsync(pls, webReader.ContentType, cancellationToken);
            ISegmentManager segmentManager;
            if (null == trackUrl)
            {
                segmentManager = null;
            }
            else
            {
                var contentType = await webReader.DetectContentTypeAsync(trackUrl, ContentKind.AnyMedia, cancellationToken);
                if (null == contentType)
                {
                    Debug.WriteLine("PlsSegmentManagerFactory.CreateSegmentManager() unable to detect type for " + trackUrl);
                    segmentManager = null;
                }
                else
                {
                    segmentManager = new SimpleSegmentManager(webReader, new[] {trackUrl}, contentType);
                }
            }
            return segmentManager;
        }

        protected virtual async Task<ISegmentManager> ReadPlaylistAsync(IWebReader webReader, Uri url, Stream stream, CancellationToken cancellationToken)
        {
            var pls = new PlsParser(url);
            using (var streamReader = new StreamReader(stream))
            {
                var ret = await pls.ParseAsync(streamReader);
                if (!ret)
                    return null;
            }
            var segmentManager = await CreateManagerAsync(pls, webReader, cancellationToken);
            return segmentManager;
        }
    }
}
