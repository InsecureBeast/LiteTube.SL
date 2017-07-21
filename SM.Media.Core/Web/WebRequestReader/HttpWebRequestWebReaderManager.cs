using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Web.WebRequestReader
{
  public class HttpWebRequestWebReaderManager : IWebReaderManager, IDisposable
  {
    private readonly IContentTypeDetector _contentTypeDetector;
    private readonly IHttpWebRequests _httpWebRequests;
    private readonly IRetryManager _retryManager;
    private readonly IWebReaderManagerParameters _webReaderManagerParameters;
    private int _disposed;

    public HttpWebRequestWebReaderManager(IHttpWebRequests httpWebRequests, IWebReaderManagerParameters webReaderManagerParameters, IContentTypeDetector contentTypeDetector, IRetryManager retryManager)
    {
      if (null == httpWebRequests)
        throw new ArgumentNullException("httpWebRequests");
      if (null == webReaderManagerParameters)
        throw new ArgumentNullException("webReaderManagerParameters");
      if (null == contentTypeDetector)
        throw new ArgumentNullException("contentTypeDetector");
      if (null == retryManager)
        throw new ArgumentNullException("retryManager");
      this._httpWebRequests = httpWebRequests;
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

    public virtual IWebReader CreateReader(Uri url, ContentKind contentKind, IWebReader parent, ContentType contentType)
    {
      return (IWebReader) this.CreateHttpWebRequestWebReader(url, parent, contentType);
    }

    public virtual IWebCache CreateWebCache(Uri url, ContentKind contentKind, IWebReader parent = null, ContentType contentType = null)
    {
      return (IWebCache) new HttpWebRequestWebCache(this.CreateHttpWebRequestWebReader(url, parent, contentType), this._httpWebRequests, this._retryManager);
    }

    public virtual async Task<ContentType> DetectContentTypeAsync(Uri url, ContentKind contentKind, CancellationToken cancellationToken, IWebReader parent = null)
    {
      ContentType contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(url, (string) null, (string) null));
      ContentType contentType1;
      if ((ContentType) null != contentType)
      {
        Debug.WriteLine("HttpWebRequestWebReaderManager.DetectContentTypeAsync() url ext \"{0}\" type {1}", (object) url, (object) contentType);
        contentType1 = contentType;
      }
      else
      {
        try
        {
          using (HttpWebResponse httpWebResponse = await this.SendAsync(url, parent, cancellationToken, "HEAD", (ContentType) null, false, (Uri) null, new long?(), new long?()).ConfigureAwait(false))
          {
            contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(httpWebResponse.ResponseUri, httpWebResponse.Headers[HttpRequestHeader.ContentType], (string) null));
            if ((ContentType) null != contentType)
            {
              Debug.WriteLine("HttpWebRequestWebReaderManager.DetectContentTypeAsync() url HEAD \"{0}\" type {1}", (object) url, (object) contentType);
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
          using (HttpWebResponse httpWebResponse = await this.SendAsync(url, parent, cancellationToken, (string) null, (ContentType) null, false, (Uri) null, new long?(0L), new long?(0L)).ConfigureAwait(false))
          {
            contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(httpWebResponse.ResponseUri, httpWebResponse.Headers[HttpRequestHeader.ContentType], (string) null));
            if ((ContentType) null != contentType)
            {
              Debug.WriteLine("HttpWebRequestWebReaderManager.DetectContentTypeAsync() url range GET \"{0}\" type {1}", (object) url, (object) contentType);
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
          using (HttpWebResponse httpWebResponse = await this.SendAsync(url, parent, cancellationToken, (string) null, (ContentType) null, false, (Uri) null, new long?(), new long?()).ConfigureAwait(false))
          {
            contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(httpWebResponse.ResponseUri, httpWebResponse.Headers[HttpRequestHeader.ContentType], (string) null));
            if ((ContentType) null != contentType)
            {
              Debug.WriteLine("HttpWebRequestWebReaderManager.DetectContentTypeAsync() url GET \"{0}\" type {1}", (object) url, (object) contentType);
              contentType1 = contentType;
              goto label_29;
            }
          }
        }
        catch (WebException ex)
        {
        }
        Debug.WriteLine("HttpWebRequestWebReaderManager.DetectContentTypeAsync() url header \"{0}\" unknown type", (object) url);
        contentType1 = (ContentType) null;
      }
label_29:
      return contentType1;
    }

    internal async Task<HttpWebResponse> SendAsync(Uri url, IWebReader parent, CancellationToken cancellationToken, string method = null, ContentType contentType = null, bool allowBuffering = true, Uri referrer = null, long? fromBytes = null, long? toBytes = null)
    {
      HttpWebRequest request = this.CreateRequest(url, referrer, parent, contentType, method, allowBuffering, fromBytes, toBytes);
      return await HttpWebRequestExtensions.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    protected virtual HttpWebRequestWebReader CreateHttpWebRequestWebReader(Uri url, IWebReader parent = null, ContentType contentType = null)
    {
      if ((ContentType) null == contentType && (Uri) null != url)
        contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>) this._contentTypeDetector.GetContentType(url, (string) null, (string) null));
      return new HttpWebRequestWebReader(this, url, parent == null ? (Uri) null : parent.BaseAddress, contentType, this._contentTypeDetector);
    }

    internal virtual HttpWebRequest CreateRequest(Uri url, Uri referrer, IWebReader parent, ContentType contentType, string method = null, bool allowBuffering = false, long? fromBytes = null, long? toBytes = null)
    {
      referrer = referrer ?? HttpWebRequestWebReaderManager.GetReferrer(parent);
      if ((Uri) null == url && null != parent)
        url = parent.RequestUri ?? parent.BaseAddress;
      if ((Uri) null != referrer && ((Uri) null == url || !url.IsAbsoluteUri))
        url = new Uri(referrer, url);
      HttpWebRequest webRequest = this._httpWebRequests.CreateWebRequest(url, referrer, method, contentType, allowBuffering, fromBytes, toBytes);
      if (null != this._webReaderManagerParameters.DefaultHeaders)
      {
        foreach (KeyValuePair<string, string> keyValuePair in this._webReaderManagerParameters.DefaultHeaders)
        {
          try
          {
            webRequest.Headers[keyValuePair.Key] = keyValuePair.Value;
          }
          catch (ArgumentException ex)
          {
            Debug.WriteLine("HttpWebRequestWebReaderManager.CreateRequest({0}) header {1}={2} failed: {3}", (object) url, (object) keyValuePair.Key, (object) keyValuePair.Value, (object) ExceptionExtensions.ExtendedMessage((Exception) ex));
          }
        }
      }
      return webRequest;
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
