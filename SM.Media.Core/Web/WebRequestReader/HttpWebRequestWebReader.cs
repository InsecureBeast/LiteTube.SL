using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Web.WebRequestReader
{
  public sealed class HttpWebRequestWebReader : IWebReader, IDisposable
  {
    private readonly Uri _baseAddress;
    private readonly IContentTypeDetector _contentTypeDetector;
    private readonly Uri _referrer;
    private readonly HttpWebRequestWebReaderManager _webReaderManager;

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

    public HttpWebRequestWebReader(HttpWebRequestWebReaderManager webReaderManager, Uri baseAddress, Uri referrer, ContentType contentType, IContentTypeDetector contentTypeDetector)
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
      HttpWebRequest request = this._webReaderManager.CreateRequest(url, referrer, (IWebReader) this, this.ContentType, (string) null, waitForContent, from, to);
      HttpWebResponse response = await HttpWebRequestExtensions.SendAsync(request, cancellationToken).ConfigureAwait(false);
      this.Update(url, response, webResponse);
      return (IWebStreamResponse) new HttpWebRequestWebStreamResponse(request, response);
    }

    public async Task<byte[]> GetByteArrayAsync(Uri url, CancellationToken cancellationToken, WebResponse webResponse = null)
    {
      if ((Uri) null != this._baseAddress && !url.IsAbsoluteUri)
        url = new Uri(this._baseAddress, url);
      byte[] numArray;
      using (HttpWebResponse response = await this._webReaderManager.SendAsync(url, (IWebReader) this, cancellationToken, (string) null, (ContentType) null, true, (Uri) null, new long?(), new long?()).ConfigureAwait(false))
      {
        this.Update(url, response, webResponse);
        numArray = await HttpWebRequestExtensions.ReadAsByteArrayAsync(response, cancellationToken).ConfigureAwait(false);
      }
      return numArray;
    }

    public async Task<HttpWebResponse> SendAsync(HttpWebRequest request, bool allowBuffering, CancellationToken cancellationToken, WebResponse webResponse = null)
    {
      Uri url = request.RequestUri;
      HttpWebResponse response = await this._webReaderManager.SendAsync(this._baseAddress, (IWebReader) this, cancellationToken, (string) null, (ContentType) null, allowBuffering, (Uri) null, new long?(), new long?()).ConfigureAwait(false);
      this.Update(url, response, webResponse);
      return response;
    }

    public HttpWebRequest CreateWebRequest(Uri url)
    {
      return this._webReaderManager.CreateRequest(url, (Uri) null, (IWebReader) this, this.ContentType, (string) null, false, new long?(), new long?());
    }

    private void Update(Uri url, HttpWebResponse response, WebResponse webResponse)
    {
      if (null != webResponse)
      {
        webResponse.RequestUri = response.ResponseUri;
        webResponse.ContentLength = response.ContentLength >= 0L ? new long?(response.ContentLength) : new long?();
        webResponse.Headers = this.GetHeaders(response.Headers);
        webResponse.ContentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(response.ResponseUri, response.Headers[HttpRequestHeader.ContentType], (string) null));
      }
      if (url != this.BaseAddress)
        return;
      this.RequestUri = response.ResponseUri;
      if (!((ContentType) null == this.ContentType))
        return;
      this.ContentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(this.RequestUri, response.Headers[HttpRequestHeader.ContentType], (string) null));
    }

    private IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetHeaders(WebHeaderCollection myWebHeaderCollection)
    {
      foreach (string key in myWebHeaderCollection.AllKeys)
      {
        string joinedValues = myWebHeaderCollection[key];
        if (null != joinedValues)
        {
          string[] values = joinedValues.Split(',');
          yield return new KeyValuePair<string, IEnumerable<string>>(key, (IEnumerable<string>) values);
        }
      }
    }

    public override string ToString()
    {
      string str = (ContentType) null == this.ContentType ? "<unknown>" : this.ContentType.ToString();
      if ((Uri) null != this.RequestUri && this.RequestUri != this.BaseAddress)
        return string.Format("HttpWebReader {0} [{1}] ({2})", (object) this.BaseAddress, (object) this.RequestUri, (object) str);
      return string.Format("HttpWebReader {0} ({1})", new object[2]
      {
        (object) this.BaseAddress,
        (object) str
      });
    }
  }
}
