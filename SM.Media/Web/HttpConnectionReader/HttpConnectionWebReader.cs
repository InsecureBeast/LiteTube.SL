// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnectionReader.HttpConnectionWebReader
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Utility;
using SM.Media.Web;
using SM.Media.Web.HttpConnection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web.HttpConnectionReader
{
  public sealed class HttpConnectionWebReader : IWebReader, IDisposable
  {
    private readonly Uri _baseAddress;
    private readonly IContentTypeDetector _contentTypeDetector;
    private readonly Uri _referrer;
    private readonly HttpConnectionWebReaderManager _webReaderManager;

    public Uri BaseAddress
    {
      get
      {
        return this._baseAddress;
      }
    }

    public Uri RequestUri { get; private set; }

    public ContentType ContentType { get; private set; }

    public IWebReaderManager Manager
    {
      get
      {
        return (IWebReaderManager) this._webReaderManager;
      }
    }

    public HttpConnectionWebReader(HttpConnectionWebReaderManager webReaderManager, Uri baseAddress, Uri referrer, ContentType contentType, IContentTypeDetector contentTypeDetector)
    {
      if (null == webReaderManager)
        throw new ArgumentNullException("webReaderManager");
      if (contentTypeDetector == null)
        throw new ArgumentNullException("contentTypeDetector");
      this._webReaderManager = webReaderManager;
      this._baseAddress = baseAddress;
      this._referrer = referrer;
      this.ContentType = contentType;
      this._contentTypeDetector = contentTypeDetector;
    }

    public void Dispose()
    {
    }

    public async Task<IWebStreamResponse> GetWebStreamAsync(Uri url, bool waitForContent, CancellationToken cancellationToken, Uri referrer = null, long? from = null, long? to = null, WebResponse webResponse = null)
    {
      HttpConnectionRequest request = this._webReaderManager.CreateRequest(url, referrer, (IWebReader) this, this.ContentType, (string) null, waitForContent, from, to);
      IHttpConnectionResponse response = await this._webReaderManager.GetAsync(request, cancellationToken).ConfigureAwait(false);
      this.Update(url, response, webResponse);
      return (IWebStreamResponse) new HttpConnectionWebStreamResponse(response);
    }

    public async Task<byte[]> GetByteArrayAsync(Uri url, CancellationToken cancellationToken, WebResponse webResponse = null)
    {
      if ((Uri) null != this._baseAddress && !url.IsAbsoluteUri)
        url = new Uri(this._baseAddress, url);
      byte[] numArray;
      using (IHttpConnectionResponse response = await this._webReaderManager.SendAsync(url, (IWebReader) this, cancellationToken, (string) null, (ContentType) null, true, (Uri) null, new long?(), new long?()).ConfigureAwait(false))
      {
        response.EnsureSuccessStatusCode();
        this.Update(url, response, webResponse);
        using (MemoryStream memoryStream = new MemoryStream())
        {
          await response.ContentReadStream.CopyToAsync((Stream) memoryStream, 4096, cancellationToken).ConfigureAwait(false);
          numArray = memoryStream.ToArray();
        }
      }
      return numArray;
    }

    public async Task<IHttpConnectionResponse> SendAsync(HttpConnectionRequest request, bool allowBuffering, CancellationToken cancellationToken, WebResponse webResponse = null)
    {
      Uri url = request.Url;
      IHttpConnectionResponse response = await this._webReaderManager.GetAsync(request, cancellationToken);
      this.Update(url, response, webResponse);
      return response;
    }

    public HttpConnectionRequest CreateWebRequest(Uri url, Uri referrer = null)
    {
      return this._webReaderManager.CreateRequest(url, referrer ?? this._referrer, (IWebReader) this, this.ContentType, (string) null, false, new long?(), new long?());
    }

    private void Update(Uri url, IHttpConnectionResponse response, WebResponse webResponse)
    {
      if (null != webResponse)
      {
        webResponse.RequestUri = response.ResponseUri;
        WebResponse webResponse1 = webResponse;
        long? nullable1 = response.Status.ContentLength;
        long? nullable2;
        if ((nullable1.GetValueOrDefault() < 0L ? 0 : (nullable1.HasValue ? 1 : 0)) == 0)
        {
          nullable1 = new long?();
          nullable2 = nullable1;
        }
        else
          nullable2 = response.Status.ContentLength;
        webResponse1.ContentLength = nullable2;
        webResponse.Headers = this.GetHeaders(response.Headers);
        webResponse.ContentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(response.ResponseUri, Enumerable.FirstOrDefault<string>(response.Headers["Content-Type"]), (string) null));
      }
      if (url != this.BaseAddress)
        return;
      this.RequestUri = response.ResponseUri;
      if (!((ContentType) null == this.ContentType))
        return;
      this.ContentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(this.RequestUri, Enumerable.FirstOrDefault<string>(response.Headers["Content-Type"]), (string) null));
    }

    private IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetHeaders(ILookup<string, string> headers)
    {
      return Enumerable.Select<IGrouping<string, string>, KeyValuePair<string, IEnumerable<string>>>((IEnumerable<IGrouping<string, string>>) headers, (Func<IGrouping<string, string>, KeyValuePair<string, IEnumerable<string>>>) (h => new KeyValuePair<string, IEnumerable<string>>(h.Key, (IEnumerable<string>) h)));
    }

    public override string ToString()
    {
      string str = (ContentType) null == this.ContentType ? "<unknown>" : this.ContentType.ToString();
      if ((Uri) null != this.RequestUri && this.RequestUri != this.BaseAddress)
        return string.Format("HttpConnectionReader {0} [{1}] ({2})", (object) this.BaseAddress, (object) this.RequestUri, (object) str);
      return string.Format("HttpConnectionReader {0} ({1})", new object[2]
      {
        (object) this.BaseAddress,
        (object) str
      });
    }
  }
}
