using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Utility;
using LiteTube.StreamVideo.Web.HttpConnection;

namespace LiteTube.StreamVideo.Web.HttpConnectionReader
{
  public class HttpConnectionWebReaderManager : IWebReaderManager, IDisposable
  {
    private readonly IContentTypeDetector _contentTypeDetector;
    private readonly IHttpConnectionFactory _httpConnectionFactory;
    private readonly IHttpConnectionRequestFactory _httpConnectionRequestFactory;
    private readonly IRetryManager _retryManager;
    private readonly IWebReaderManagerParameters _webReaderManagerParameters;
    private int _disposed;

    public HttpConnectionWebReaderManager(IHttpConnectionFactory httpConnectionFactory, IHttpConnectionRequestFactory httpConnectionRequestFactory, IWebReaderManagerParameters webReaderManagerParameters, IContentTypeDetector contentTypeDetector, IRetryManager retryManager)
    {
      if (null == httpConnectionFactory)
        throw new ArgumentNullException("httpConnectionFactory");
      if (null == httpConnectionRequestFactory)
        throw new ArgumentNullException("httpConnectionRequestFactory");
      if (null == webReaderManagerParameters)
        throw new ArgumentNullException("webReaderManagerParameters");
      if (null == contentTypeDetector)
        throw new ArgumentNullException("contentTypeDetector");
      if (null == retryManager)
        throw new ArgumentNullException("retryManager");
      this._httpConnectionFactory = httpConnectionFactory;
      this._httpConnectionRequestFactory = httpConnectionRequestFactory;
      this._webReaderManagerParameters = webReaderManagerParameters;
      this._contentTypeDetector = contentTypeDetector;
      this._retryManager = retryManager;
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._disposed, 1))
        return;
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public virtual IWebReader CreateReader(Uri url, ContentKind contentKind, IWebReader parent = null, ContentType contentType = null)
    {
      return (IWebReader) this.CreateHttpConnectionWebReader(url, parent, contentType);
    }

    public virtual IWebCache CreateWebCache(Uri url, ContentKind contentKind, IWebReader parent = null, ContentType contentType = null)
    {
      return (IWebCache) new HttpConnectionWebCache(this.CreateHttpConnectionWebReader(url, parent, contentType), this._retryManager);
    }

    public virtual async Task<ContentType> DetectContentTypeAsync(Uri url, ContentKind contentKind, CancellationToken cancellationToken, IWebReader parent = null)
    {
      ContentType contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(url, (string) null, (string) null));
      ContentType contentType1;
      if ((ContentType) null != contentType)
      {
        Debug.WriteLine("HttpConnectionWebReaderManager.DetectContentTypeAsync() url ext \"{0}\" type {1}", (object) url, (object) contentType);
        contentType1 = contentType;
      }
      else
      {
        try
        {
          using (IHttpConnectionResponse connectionResponse = await this.SendAsync(url, parent, cancellationToken, "HEAD", (ContentType) null, false, (Uri) null, new long?(), new long?()).ConfigureAwait(false))
          {
            contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(connectionResponse.ResponseUri, Enumerable.FirstOrDefault<string>(connectionResponse.Headers["Content-Type"]), (string) null));
            if ((ContentType) null != contentType)
            {
              Debug.WriteLine("HttpConnectionWebReaderManager.DetectContentTypeAsync() url HEAD \"{0}\" type {1}", (object) url, (object) contentType);
              contentType1 = contentType;
              goto label_29;
            }
          }
        }
        catch (WebException ex)
        {
        }
        try
        {
          using (IHttpConnectionResponse connectionResponse = await this.SendAsync(url, parent, cancellationToken, (string) null, (ContentType) null, false, (Uri) null, new long?(0L), new long?(0L)).ConfigureAwait(false))
          {
            contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(connectionResponse.ResponseUri, Enumerable.FirstOrDefault<string>(connectionResponse.Headers["Content-Type"]), (string) null));
            if ((ContentType) null != contentType)
            {
              Debug.WriteLine("HttpConnectionWebReaderManager.DetectContentTypeAsync() url range GET \"{0}\" type {1}", (object) url, (object) contentType);
              contentType1 = contentType;
              goto label_29;
            }
          }
        }
        catch (WebException ex)
        {
        }
        try
        {
          using (IHttpConnectionResponse connectionResponse = await this.SendAsync(url, parent, cancellationToken, (string) null, (ContentType) null, false, (Uri) null, new long?(), new long?()).ConfigureAwait(false))
          {
            contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(connectionResponse.ResponseUri, Enumerable.FirstOrDefault<string>(connectionResponse.Headers["Content-Type"]), (string) null));
            if ((ContentType) null != contentType)
            {
              Debug.WriteLine("HttpConnectionWebReaderManager.DetectContentTypeAsync() url GET \"{0}\" type {1}", (object) url, (object) contentType);
              contentType1 = contentType;
              goto label_29;
            }
          }
        }
        catch (WebException ex)
        {
        }
        Debug.WriteLine("HttpConnectionWebReaderManager.DetectContentTypeAsync() url header \"{0}\" unknown type", (object) url);
        contentType1 = (ContentType) null;
      }
label_29:
      return contentType1;
    }

    internal Task<IHttpConnectionResponse> SendAsync(Uri url, IWebReader parent, CancellationToken cancellationToken, string method = null, ContentType contentType = null, bool allowBuffering = true, Uri referrer = null, long? fromBytes = null, long? toBytes = null)
    {
      return this.GetAsync(this.CreateRequest(url, referrer, parent, contentType, method, allowBuffering, fromBytes, toBytes), cancellationToken);
    }

    protected virtual HttpConnectionWebReader CreateHttpConnectionWebReader(Uri url, IWebReader parent = null, ContentType contentType = null)
    {
      if ((ContentType) null == contentType && (Uri) null != url)
        contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(url, (string) null, (string) null));
      return new HttpConnectionWebReader(this, url, parent == null ? (Uri) null : parent.BaseAddress, contentType, this._contentTypeDetector);
    }

    internal virtual async Task<IHttpConnectionResponse> GetAsync(HttpConnectionRequest request, CancellationToken cancellationToken)
    {
      IHttpConnection connection = this._httpConnectionFactory.CreateHttpConnection();
      Uri requestUrl = request.Url;
      Uri url = requestUrl;
      int retry = 0;
      IHttpConnectionResponse response;
      string location;
      do
      {
        await connection.ConnectAsync(request.Proxy ?? url, cancellationToken).ConfigureAwait(false);
        request.Url = url;
        response = await connection.GetAsync(request, true, cancellationToken).ConfigureAwait(false);
        request.Url = requestUrl;
        IHttpStatus status = response.Status;
        if (HttpStatusCode.Moved == status.StatusCode || HttpStatusCode.Found == status.StatusCode)
        {
          if (++retry < 8)
          {
            connection.Close();
            location = Enumerable.FirstOrDefault<string>(response.Headers["Location"]);
          }
          else
            goto label_5;
        }
        else
          goto label_3;
      }
      while (Uri.TryCreate(request.Url, location, out url));
      goto label_7;
label_3:
      IHttpConnectionResponse connectionResponse = response;
      goto label_9;
label_5:
      connectionResponse = response;
      goto label_9;
label_7:
      connectionResponse = response;
label_9:
      return connectionResponse;
    }

    internal virtual HttpConnectionRequest CreateRequest(Uri url, Uri referrer, IWebReader parent, ContentType contentType, string method = null, bool allowBuffering = false, long? fromBytes = null, long? toBytes = null)
    {
      referrer = referrer ?? HttpConnectionWebReaderManager.GetReferrer(parent);
      if ((Uri) null == url && null != parent)
        url = parent.RequestUri ?? parent.BaseAddress;
      if ((Uri) null != referrer && ((Uri) null == url || !url.IsAbsoluteUri))
        url = new Uri(referrer, url);
      return this._httpConnectionRequestFactory.CreateRequest(url, referrer, contentType, fromBytes, toBytes, this._webReaderManagerParameters.DefaultHeaders);
    }

    protected static Uri GetReferrer(IWebReader parent)
    {
      return parent == null ? (Uri) null : parent.RequestUri ?? parent.BaseAddress;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
        ;
    }
  }
}
