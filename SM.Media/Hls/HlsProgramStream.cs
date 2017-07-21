// Decompiled with JetBrains decompiler
// Type: SM.Media.Hls.HlsProgramStream
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.M3U8;
using SM.Media.Metadata;
using SM.Media.Playlists;
using SM.Media.Segments;
using SM.Media.Utility;
using SM.Media.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Hls
{
  public class HlsProgramStream : IHlsProgramStream, IProgramStream
  {
    private static readonly ISegment[] NoPlaylist = new ISegment[0];
    private bool _isDynamicPlaylist = true;
    private ICollection<ISegment> _segments = (ICollection<ISegment>) HlsProgramStream.NoPlaylist;
    private readonly IHlsSegmentsFactory _segmentsFactory;
    private readonly IWebMetadataFactory _webMetadataFactory;
    private readonly IWebReader _webReader;
    private Uri _actualUrl;
    private ContentType _contentType;
    private IWebCache _subPlaylistCache;

    public IWebReader WebReader
    {
      get
      {
        return this._webReader;
      }
    }

    public string StreamType { get; internal set; }

    public string Language { get; internal set; }

    public ICollection<Uri> Urls { get; set; }

    public bool IsDynamicPlaylist
    {
      get
      {
        return this._isDynamicPlaylist;
      }
    }

    public IStreamMetadata StreamMetadata { get; set; }

    public ICollection<ISegment> Segments
    {
      get
      {
        return this._segments;
      }
    }

    public HlsProgramStream(IWebReader webReader, ICollection<Uri> urls, IHlsSegmentsFactory segmentsFactory, IWebMetadataFactory webMetadataFactory, IPlatformServices platformServices, IRetryManager retryManager)
    {
      if (null == segmentsFactory)
        throw new ArgumentNullException("segmentsFactory");
      if (null == webMetadataFactory)
        throw new ArgumentNullException("webMetadataFactory");
      if (null == webReader)
        throw new ArgumentNullException("webReader");
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      if (null == retryManager)
        throw new ArgumentNullException("retryManager");
      this._webReader = webReader;
      this._segmentsFactory = segmentsFactory;
      this._webMetadataFactory = webMetadataFactory;
      this.Urls = urls;
    }

    public async Task RefreshPlaylistAsync(CancellationToken cancellationToken)
    {
      if (this._isDynamicPlaylist || this._segments == null || this._segments.Count <= 0)
      {
        M3U8Parser parser = await this.FetchPlaylistAsync(cancellationToken).ConfigureAwait(false);
        await this.UpdateAsync(parser, cancellationToken).ConfigureAwait(false);
      }
    }

    public async Task<ContentType> GetContentTypeAsync(CancellationToken cancellationToken)
    {
      ContentType contentType;
      if (null == this._segments)
      {
        contentType = (ContentType) null;
      }
      else
      {
        ISegment segment0 = Enumerable.FirstOrDefault<ISegment>((IEnumerable<ISegment>) this._segments);
        if (segment0 == null || (Uri) null == segment0.Url)
        {
          contentType = (ContentType) null;
        }
        else
        {
          ((HlsProgramStream) this)._contentType = await WebReaderExtensions.DetectContentTypeAsync(this._subPlaylistCache.WebReader, segment0.Url, ContentKind.AnyMedia, cancellationToken).ConfigureAwait(false);
          contentType = this._contentType;
        }
      }
      return contentType;
    }

    public Task SetParserAsync(M3U8Parser parser, CancellationToken cancellationToken)
    {
      this.UpdateSubPlaylistCache(parser.BaseUrl);
      return this.UpdateAsync(parser, cancellationToken);
    }

    private async Task UpdateAsync(M3U8Parser parser, CancellationToken cancellationToken)
    {
      ((HlsProgramStream) this)._segments = await this._segmentsFactory.CreateSegmentsAsync(parser, this._subPlaylistCache.WebReader, cancellationToken).ConfigureAwait(false);
      this._isDynamicPlaylist = HlsPlaylistSettings.Parameters.IsDynamicPlaylist(parser);
      this._actualUrl = parser.BaseUrl;
    }

    private void UpdateSubPlaylistCache(Uri playlist)
    {
      if (this._subPlaylistCache != null && !(this._subPlaylistCache.WebReader.BaseAddress != playlist))
        return;
      if (null != this._subPlaylistCache)
        this._subPlaylistCache.WebReader.Dispose();
      this._subPlaylistCache = WebReaderExtensions.CreateWebCache(this._webReader, playlist, ContentKind.Playlist, (ContentType) null);
    }

    private async Task<M3U8Parser> FetchPlaylistAsync(CancellationToken cancellationToken)
    {
      ICollection<Uri> urls = this.Urls;
      M3U8Parser m3U8Parser;
      if (urls == null || urls.Count < 1)
      {
        m3U8Parser = (M3U8Parser) null;
      }
      else
      {
        foreach (Uri playlist in (IEnumerable<Uri>) urls)
        {
          this.UpdateSubPlaylistCache(playlist);
          cancellationToken.ThrowIfCancellationRequested();
          WebResponse webResponse = (WebResponse) null;
          if (null == this.StreamMetadata)
            webResponse = new WebResponse();
          M3U8Parser parsedPlaylist = await this._subPlaylistCache.ReadAsync<M3U8Parser>((Func<Uri, byte[], M3U8Parser>) ((actualUri, bytes) =>
          {
            cancellationToken.ThrowIfCancellationRequested();
            if (bytes.Length < 1)
              return (M3U8Parser) null;
            M3U8Parser parser = new M3U8Parser();
            using (MemoryStream memoryStream = new MemoryStream(bytes))
              M3U8ParserExtensions.Parse(parser, actualUri, (Stream) memoryStream, (Encoding) null);
            return parser;
          }), cancellationToken, webResponse).ConfigureAwait(false);
          if (null != parsedPlaylist)
          {
            if (null != webResponse)
              this.StreamMetadata = this._webMetadataFactory.CreateStreamMetadata(webResponse, (ContentType) null);
            m3U8Parser = parsedPlaylist;
            goto label_16;
          }
        }
        m3U8Parser = (M3U8Parser) null;
      }
label_16:
      return m3U8Parser;
    }

    public override string ToString()
    {
      return string.Format("dynamic {0} segments {1} url {2}", (object) (bool) (this._isDynamicPlaylist ? 1 : 0), (object) (this._segments == null ? 0 : this._segments.Count), (object) this._actualUrl);
    }
  }
}
