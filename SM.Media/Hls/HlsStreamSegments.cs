// Decompiled with JetBrains decompiler
// Type: SM.Media.Hls.HlsStreamSegments
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using SM.Media.M3U8;
using SM.Media.M3U8.AttributeSupport;
using SM.Media.M3U8.TagSupport;
using SM.Media.Playlists;
using SM.Media.Segments;
using SM.Media.Utility;
using SM.Media.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Hls
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
        throw new ArgumentNullException("parser");
      if (null == webReader)
        throw new ArgumentNullException("webReader");
      if (null == retryManager)
        throw new ArgumentNullException("retryManager");
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      this._parser = parser;
      this._webReader = webReader;
      this._retryManager = retryManager;
      this._platformServices = platformServices;
      this._mediaSequence = M3U8Tags.ExtXMediaSequence.GetValue<long>(parser.GlobalTags);
    }

    public Task<ICollection<ISegment>> CreateSegmentsAsync(CancellationToken cancellationToken)
    {
      this._playlist = Enumerable.ToArray<ISegment>(Enumerable.Select<M3U8Parser.M3U8Uri, ISegment>(this._parser.Playlist, (Func<M3U8Parser.M3U8Uri, ISegment>) (url => this.CreateStreamSegment(url, cancellationToken))));
      return TaskEx.FromResult<ICollection<ISegment>>((ICollection<ISegment>) this._playlist);
    }

    private ISegment CreateStreamSegment(M3U8Parser.M3U8Uri uri, CancellationToken cancellationToken)
    {
      SubStreamSegment segment = new SubStreamSegment(M3U8ParserExtensions.ResolveUrl(this._parser, uri.Uri), this._parser.BaseUrl);
      if (this._mediaSequence.HasValue)
      {
        SubStreamSegment subStreamSegment = segment;
        long? nullable1 = this._mediaSequence;
        long num = (long) this._segmentIndex;
        long? nullable2 = nullable1.HasValue ? new long?(nullable1.GetValueOrDefault() + num) : new long?();
        subStreamSegment.MediaSequence = nullable2;
      }
      ++this._segmentIndex;
      M3U8TagInstance[] m3U8TagInstanceArray = uri.Tags;
      if (m3U8TagInstanceArray == null || 0 == m3U8TagInstanceArray.Length)
        return (ISegment) segment;
      ExtinfTagInstance extinfTagInstance = M3U8Tags.ExtXInf.Find((IEnumerable<M3U8TagInstance>) m3U8TagInstanceArray);
      if (null != extinfTagInstance)
        segment.Duration = new TimeSpan?(TimeSpan.FromSeconds((double) extinfTagInstance.Duration));
      ByterangeTagInstance byteRange = M3U8Tags.ExtXByteRange.Find((IEnumerable<M3U8TagInstance>) m3U8TagInstanceArray);
      if (null != byteRange)
        this.HandleByteRange(segment, byteRange);
      IEnumerable<ExtKeyTagInstance> all = M3U8Tags.ExtXKey.FindAll((IEnumerable<M3U8TagInstance>) m3U8TagInstanceArray);
      if (null != all)
        this.HandleKey(segment, all, cancellationToken);
      return (ISegment) segment;
    }

    private void HandleByteRange(SubStreamSegment segment, ByterangeTagInstance byteRange)
    {
      if (byteRange.Offset.HasValue)
      {
        segment.Offset = byteRange.Offset.Value;
        this._byteRangeOffset = byteRange.Offset.Value;
      }
      else
        segment.Offset = this._byteRangeOffset;
      segment.Length = byteRange.Length;
      this._byteRangeOffset += byteRange.Length;
    }

    private void HandleKey(SubStreamSegment segment, IEnumerable<ExtKeyTagInstance> extKeys, CancellationToken cancellationToken)
    {
      ExtKeyTagInstance[] extKeyTagInstanceArray = Enumerable.ToArray<ExtKeyTagInstance>(extKeys);
      if (extKeyTagInstanceArray.Length < 1)
        return;
      string url = (string) null;
      byte[] iv = (byte[]) null;
      foreach (ExtKeyTagInstance extKeyTagInstance in extKeyTagInstanceArray)
      {
        string b = M3U8TagInstanceExtensions.AttributeObject<string>((M3U8TagInstance) extKeyTagInstance, ExtKeySupport.AttrMethod);
        if (string.Equals("NONE", b, StringComparison.OrdinalIgnoreCase))
        {
          url = (string) null;
        }
        else
        {
          if (!string.Equals("AES-128", b, StringComparison.OrdinalIgnoreCase))
          {
            if (string.Equals("SAMPLE-AES", b, StringComparison.OrdinalIgnoreCase))
              throw new NotImplementedException("Method SAMPLE-AES decryption is not implemented");
            throw new NotSupportedException("Unknown decryption method type: " + b);
          }
          string str = M3U8TagInstanceExtensions.AttributeObject<string>((M3U8TagInstance) extKeyTagInstance, ExtKeySupport.AttrUri);
          if (null != str)
            url = str;
          byte[] numArray = M3U8TagInstanceExtensions.AttributeObject<byte[]>((M3U8TagInstance) extKeyTagInstance, ExtKeySupport.AttrIv);
          if (null != numArray)
            iv = numArray;
        }
      }
      if (null == url)
        return;
      if (null == iv)
      {
        iv = new byte[16];
        long num = segment.MediaSequence ?? (long) (this._segmentIndex - 1);
        iv[15] = (byte) ((ulong) num & (ulong) byte.MaxValue);
        iv[14] = (byte) ((ulong) (num >> 8) & (ulong) byte.MaxValue);
        iv[13] = (byte) ((ulong) (num >> 16) & (ulong) byte.MaxValue);
        iv[12] = (byte) ((ulong) (num >> 24) & (ulong) byte.MaxValue);
      }
      Func<Stream, CancellationToken, Task<Stream>> filter = segment.AsyncStreamFilter;
      Uri uri = M3U8ParserExtensions.ResolveUrl(this._parser, url);
      segment.AsyncStreamFilter = (Func<Stream, CancellationToken, Task<Stream>>) (async (stream, ct) =>
      {
        if (null != filter)
          stream = await filter(stream, ct).ConfigureAwait(false);
        byte[] key;
        if (!this._keyCache.TryGetValue(uri, out key))
        {
          key = await this.LoadKeyAsync(uri, cancellationToken).ConfigureAwait(false);
          if (16 != key.Length)
            throw new FormatException("AES-128 key length mismatch: " + (object) key.Length);
          this._keyCache[uri] = key;
        }
        Debug.WriteLine("Segment AES-128: key {0} iv {1}", (object) BitConverter.ToString(key), (object) BitConverter.ToString(iv));
        return this._platformServices.Aes128DecryptionFilter(stream, key, iv);
      });
    }

    private Task<byte[]> LoadKeyAsync(Uri uri, CancellationToken cancellationToken)
    {
      Debug.WriteLine("HlsStreamSegments.LoadKeyAsync() " + (object) uri);
      return RetryManagerExtensions.CreateWebRetry(this._retryManager, 4, 100).CallAsync<byte[]>((Func<Task<byte[]>>) (() => this._webReader.GetByteArrayAsync(uri, cancellationToken, (WebResponse) null)), cancellationToken);
    }
  }
}
