using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.M3U8;
using SM.Media.Core.Metadata;
using SM.Media.Core.Segments;
using SM.Media.Core.TransportStream.TsParser;
using SM.Media.Core.Utility;
using SM.Media.Core.Web;
using WebResponse = SM.Media.Core.Web.WebResponse;

namespace SM.Media.Core.Hls
{
    public class HlsProgramStream : IHlsProgramStream, IProgramStream
    {
        private static readonly ISegment[] _noPlaylist = new ISegment[0];
        private bool _isDynamicPlaylist = true;
        private ICollection<ISegment> _segments = _noPlaylist;
        private readonly IHlsSegmentsFactory _segmentsFactory;
        private readonly IWebMetadataFactory _webMetadataFactory;
        private readonly IWebReader _webReader;
        private Uri _actualUrl;
        private ContentType _contentType;
        private IWebCache _subPlaylistCache;

        public IWebReader WebReader => _webReader;

        public uint Pid { get; }
        TsStreamType IProgramStream.StreamType { get; }
        public bool BlockStream { get; set; }
        public string StreamType { get; internal set; }
        public string Language { get; internal set; }
        public ICollection<Uri> Urls { get; set; }
        public bool IsDynamicPlaylist => _isDynamicPlaylist;
        public IStreamMetadata StreamMetadata { get; set; }
        public ICollection<ISegment> Segments => _segments;

        public HlsProgramStream(IWebReader webReader, ICollection<Uri> urls, IHlsSegmentsFactory segmentsFactory, IWebMetadataFactory webMetadataFactory, IPlatformServices platformServices, IRetryManager retryManager)
        {
            if (null == segmentsFactory)
                throw new ArgumentNullException(nameof(segmentsFactory));
            if (null == webMetadataFactory)
                throw new ArgumentNullException(nameof(webMetadataFactory));
            if (null == webReader)
                throw new ArgumentNullException(nameof(webReader));
            if (null == platformServices)
                throw new ArgumentNullException(nameof(platformServices));
            if (null == retryManager)
                throw new ArgumentNullException(nameof(retryManager));
            _webReader = webReader;
            _segmentsFactory = segmentsFactory;
            _webMetadataFactory = webMetadataFactory;
            Urls = urls;
        }

        public async Task RefreshPlaylistAsync(CancellationToken cancellationToken)
        {
            if (_isDynamicPlaylist || _segments == null || _segments.Count <= 0)
            {
                var parser = await FetchPlaylistAsync(cancellationToken).ConfigureAwait(false);
                await UpdateAsync(parser, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<ContentType> GetContentTypeAsync(CancellationToken cancellationToken)
        {
            ContentType contentType;
            if (null == _segments)
            {
                contentType = null;
            }
            else
            {
                ISegment segment0 = _segments.FirstOrDefault();
                if (segment0?.Url == null)
                {
                    contentType = null;
                }
                else
                {
                    _contentType = await _subPlaylistCache.WebReader.DetectContentTypeAsync(segment0.Url, ContentKind.AnyMedia, cancellationToken).ConfigureAwait(false);
                    contentType = _contentType;
                }
            }
            return contentType;
        }

        public Task SetParserAsync(M3U8Parser parser, CancellationToken cancellationToken)
        {
            UpdateSubPlaylistCache(parser.BaseUrl);
            return UpdateAsync(parser, cancellationToken);
        }

        private async Task UpdateAsync(M3U8Parser parser, CancellationToken cancellationToken)
        {
            _segments = await _segmentsFactory.CreateSegmentsAsync(parser, _subPlaylistCache.WebReader, cancellationToken).ConfigureAwait(false);
            _isDynamicPlaylist = HlsPlaylistSettings.Parameters.IsDynamicPlaylist(parser);
            _actualUrl = parser.BaseUrl;
        }

        private void UpdateSubPlaylistCache(Uri playlist)
        {
            if (_subPlaylistCache != null && !(_subPlaylistCache.WebReader.BaseAddress != playlist))
                return;
            _subPlaylistCache?.WebReader.Dispose();
            _subPlaylistCache = _webReader.CreateWebCache(playlist, ContentKind.Playlist);
        }

        private async Task<M3U8Parser> FetchPlaylistAsync(CancellationToken cancellationToken)
        {
            var urls = Urls;
            if (urls == null || urls.Count < 1)
                return null;

            foreach (Uri playlist in urls)
            {
                UpdateSubPlaylistCache(playlist);
                cancellationToken.ThrowIfCancellationRequested();
                WebResponse webResponse = null;
                if (null == StreamMetadata)
                    webResponse = new WebResponse();

                var parsedPlaylist = await _subPlaylistCache.ReadAsync((actualUri, bytes) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (bytes.Length < 1)
                        return null;
                    var parser = new M3U8Parser();
                    using (var memoryStream = new MemoryStream(bytes))
                    {
                        parser.Parse(actualUri, memoryStream);
                    }

                    return parser;
                }, cancellationToken, webResponse).ConfigureAwait(false);

                if (null != parsedPlaylist)
                {
                    if (null != webResponse)
                        StreamMetadata = _webMetadataFactory.CreateStreamMetadata(webResponse);

                    return parsedPlaylist;
                }
            }

            return null;
        }

        public override string ToString()
        {
            return $"dynamic { _isDynamicPlaylist} segments { _segments?.Count ?? 0} url { _actualUrl}";
        }
    }
}
