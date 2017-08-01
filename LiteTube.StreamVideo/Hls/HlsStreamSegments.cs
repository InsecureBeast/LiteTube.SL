using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.M3U8;
using LiteTube.StreamVideo.M3U8.AttributeSupport;
using LiteTube.StreamVideo.M3U8.TagSupport;
using LiteTube.StreamVideo.Playlists;
using LiteTube.StreamVideo.Segments;
using LiteTube.StreamVideo.Utility;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Hls
{
    public class HlsStreamSegments : IHlsStreamSegments
    {
        private readonly Dictionary<Uri, byte[]> _keyCache = new Dictionary<Uri, byte[]>();
        private const string MethodAes = "AES-128";
        private const string MethodNone = "NONE";
        private const string MethodSampleAes = "SAMPLE-AES";
        private readonly M3U8Parser _parser;
        private readonly IPlatformServices _platformServices;
        private readonly IRetryManager _retryManager;
        private readonly IWebReader _webReader;
        private long _byteRangeOffset;
        private long? _mediaSequence;
        private ISegment[] _playlist;
        private int _segmentIndex;

        public HlsStreamSegments(M3U8Parser parser, IWebReader webReader, IRetryManager retryManager, IPlatformServices platformServices)
        {
            if (null == parser)
                throw new ArgumentNullException(nameof(parser));
            if (null == webReader)
                throw new ArgumentNullException(nameof(webReader));
            if (null == retryManager)
                throw new ArgumentNullException(nameof(retryManager));
            if (null == platformServices)
                throw new ArgumentNullException(nameof(platformServices));
            _parser = parser;
            _webReader = webReader;
            _retryManager = retryManager;
            _platformServices = platformServices;
            _mediaSequence = M3U8Tags.ExtXMediaSequence.GetValue<long>(parser.GlobalTags);
        }

        public Task<ICollection<ISegment>> CreateSegmentsAsync(CancellationToken cancellationToken)
        {
            _playlist = _parser.Playlist.Select(url => CreateStreamSegment(url, cancellationToken)).ToArray();
            return TaskEx.FromResult((ICollection<ISegment>)_playlist);
        }

        private ISegment CreateStreamSegment(M3U8Parser.M3U8Uri uri, CancellationToken cancellationToken)
        {
            SubStreamSegment segment = new SubStreamSegment(_parser.ResolveUrl(uri.Uri), _parser.BaseUrl);
            if (_mediaSequence.HasValue)
            {
                SubStreamSegment subStreamSegment = segment;
                long? nullable1 = _mediaSequence;
                long num = _segmentIndex;
                long? nullable2 = nullable1.GetValueOrDefault() + num;
                subStreamSegment.MediaSequence = nullable2;
            }
            ++_segmentIndex;
            M3U8TagInstance[] m3U8TagInstanceArray = uri.Tags;
            if (m3U8TagInstanceArray == null || 0 == m3U8TagInstanceArray.Length)
                return segment;
            ExtinfTagInstance extinfTagInstance = M3U8Tags.ExtXInf.Find(m3U8TagInstanceArray);
            if (null != extinfTagInstance)
                segment.Duration = TimeSpan.FromSeconds((double)extinfTagInstance.Duration);
            ByterangeTagInstance byteRange = M3U8Tags.ExtXByteRange.Find(m3U8TagInstanceArray);
            if (null != byteRange)
                HandleByteRange(segment, byteRange);
            IEnumerable<ExtKeyTagInstance> all = M3U8Tags.ExtXKey.FindAll(m3U8TagInstanceArray);
            if (null != all)
                HandleKey(segment, all, cancellationToken);
            return segment;
        }

        private void HandleByteRange(SubStreamSegment segment, ByterangeTagInstance byteRange)
        {
            if (byteRange.Offset.HasValue)
            {
                segment.Offset = byteRange.Offset.Value;
                _byteRangeOffset = byteRange.Offset.Value;
            }
            else
                segment.Offset = _byteRangeOffset;
            segment.Length = byteRange.Length;
            _byteRangeOffset += byteRange.Length;
        }

        private void HandleKey(SubStreamSegment segment, IEnumerable<ExtKeyTagInstance> extKeys, CancellationToken cancellationToken)
        {
            ExtKeyTagInstance[] extKeyTagInstanceArray = extKeys.ToArray();
            if (extKeyTagInstanceArray.Length < 1)
                return;
            string url = null;
            byte[] iv = null;
            foreach (ExtKeyTagInstance extKeyTagInstance in extKeyTagInstanceArray)
            {
                var b = extKeyTagInstance.AttributeObject(ExtKeySupport.AttrMethod);
                if (string.Equals("NONE", b, StringComparison.OrdinalIgnoreCase))
                {
                    url = null;
                }
                else
                {
                    if (!string.Equals("AES-128", b, StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.Equals("SAMPLE-AES", b, StringComparison.OrdinalIgnoreCase))
                            throw new NotImplementedException("Method SAMPLE-AES decryption is not implemented");
                        throw new NotSupportedException("Unknown decryption method type: " + b);
                    }
                    string str = extKeyTagInstance.AttributeObject(ExtKeySupport.AttrUri);
                    if (null != str)
                        url = str;
                    byte[] numArray = extKeyTagInstance.AttributeObject(ExtKeySupport.AttrIv);
                    if (null != numArray)
                        iv = numArray;
                }
            }
            if (null == url)
                return;
            if (null == iv)
            {
                iv = new byte[16];
                long num = segment.MediaSequence ?? _segmentIndex - 1;
                iv[15] = (byte)((ulong)num & byte.MaxValue);
                iv[14] = (byte)((ulong)(num >> 8) & byte.MaxValue);
                iv[13] = (byte)((ulong)(num >> 16) & byte.MaxValue);
                iv[12] = (byte)((ulong)(num >> 24) & byte.MaxValue);
            }
            Func<Stream, CancellationToken, Task<Stream>> filter = segment.AsyncStreamFilter;
            Uri uri = _parser.ResolveUrl(url);
            segment.AsyncStreamFilter = async (stream, ct) =>
           {
               if (null != filter)
                   stream = await filter(stream, ct).ConfigureAwait(false);
               byte[] key;
               if (!_keyCache.TryGetValue(uri, out key))
               {
                   key = await LoadKeyAsync(uri, cancellationToken).ConfigureAwait(false);
                   if (16 != key.Length)
                       throw new FormatException("AES-128 key length mismatch: " + key.Length);
                   _keyCache[uri] = key;
               }
               Debug.WriteLine("Segment AES-128: key {0} iv {1}", BitConverter.ToString(key), BitConverter.ToString(iv));
               return _platformServices.Aes128DecryptionFilter(stream, key, iv);
           };
        }

        private Task<byte[]> LoadKeyAsync(Uri uri, CancellationToken cancellationToken)
        {
            Debug.WriteLine("HlsStreamSegments.LoadKeyAsync() " + uri);
            return _retryManager.CreateWebRetry(4, 100).CallAsync(() => _webReader.GetByteArrayAsync(uri, cancellationToken), cancellationToken);
        }
    }
}
