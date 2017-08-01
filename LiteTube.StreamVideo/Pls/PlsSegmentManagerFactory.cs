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
using WebResponse = LiteTube.StreamVideo.Web.WebResponse;

namespace LiteTube.StreamVideo.Pls
{
  public class PlsSegmentManagerFactory : ISegmentManagerFactoryInstance, IContentServiceFactoryInstance<ISegmentManager, ISegmentManagerParameters>
  {
    private static readonly ICollection<ContentType> Types = (ICollection<ContentType>) new ContentType[1]
    {
      ContentTypes.Pls
    };
    private readonly IPlsSegmentManagerPolicy _plsSegmentManagerPolicy;
    private readonly IRetryManager _retryManager;
    private readonly IWebReaderManager _webReaderManager;

    public ICollection<ContentType> KnownContentTypes
    {
      get
      {
        return PlsSegmentManagerFactory.Types;
      }
    }

    public PlsSegmentManagerFactory(IWebReaderManager webReaderManager, IPlsSegmentManagerPolicy plsSegmentManagerPolicy, IRetryManager retryManager)
    {
      if (null == webReaderManager)
        throw new ArgumentNullException("webReaderManager");
      if (null == plsSegmentManagerPolicy)
        throw new ArgumentNullException("plsSegmentManagerPolicy");
      if (null == retryManager)
        throw new ArgumentNullException("retryManager");
      this._webReaderManager = webReaderManager;
      this._plsSegmentManagerPolicy = plsSegmentManagerPolicy;
      this._retryManager = retryManager;
    }

    public virtual async Task<ISegmentManager> CreateAsync(ISegmentManagerParameters parameters, ContentType contentType, CancellationToken cancellationToken)
    {
      ISegmentManager segmentManager1;
      foreach (Uri uri in (IEnumerable<Uri>) parameters.Source)
      {
        Uri localUrl = uri;
        IRetry retry = RetryManagerExtensions.CreateWebRetry(this._retryManager, 3, 333);
        ISegmentManager segmentManager = await retry.CallAsync<ISegmentManager>((Func<Task<ISegmentManager>>) (async () =>
        {
          IWebReader webReader = this._webReaderManager.CreateReader(localUrl, ContentTypes.Pls.Kind, (IWebReader) null, ContentTypes.Pls);
          ISegmentManager segmentManager2;
          try
          {
            using (IWebStreamResponse webStreamResponse = await webReader.GetWebStreamAsync(localUrl, false, cancellationToken, (Uri) null, new long?(), new long?(), (WebResponse) null).ConfigureAwait(false))
            {
              if (!webStreamResponse.IsSuccessStatusCode)
              {
                webReader.Dispose();
                segmentManager2 = (ISegmentManager) null;
              }
              else
              {
                using (Stream stream = await webStreamResponse.GetStreamAsync(cancellationToken).ConfigureAwait(false))
                  segmentManager2 = await this.ReadPlaylistAsync(webReader, webStreamResponse.ActualUrl, stream, cancellationToken).ConfigureAwait(false);
              }
            }
          }
          catch (Exception ex)
          {
            webReader.Dispose();
            throw;
          }
          return segmentManager2;
        }), cancellationToken);
        if (null != segmentManager)
        {
          segmentManager1 = segmentManager;
          goto label_10;
        }
      }
      segmentManager1 = (ISegmentManager) null;
label_10:
      return segmentManager1;
    }

    protected virtual async Task<ISegmentManager> CreateManagerAsync(PlsParser pls, IWebReader webReader, CancellationToken cancellationToken)
    {
      Uri trackUrl = await this._plsSegmentManagerPolicy.GetTrackAsync(pls, webReader.ContentType, cancellationToken);
      ISegmentManager segmentManager;
      if ((Uri) null == trackUrl)
      {
        segmentManager = (ISegmentManager) null;
      }
      else
      {
        ContentType contentType = await WebReaderExtensions.DetectContentTypeAsync(webReader, trackUrl, ContentKind.AnyMedia, cancellationToken).ConfigureAwait(false);
        if ((ContentType) null == contentType)
        {
          Debug.WriteLine("PlsSegmentManagerFactory.CreateSegmentManager() unable to detect type for " + (object) trackUrl);
          segmentManager = (ISegmentManager) null;
        }
        else
          segmentManager = (ISegmentManager) new SimpleSegmentManager(webReader, (IEnumerable<Uri>) new Uri[1]
          {
            trackUrl
          }, contentType);
      }
      return segmentManager;
    }

    [Conditional("DEBUG")]
    private void DumpIcy(IEnumerable<KeyValuePair<string, IEnumerable<string>>> httpResponseHeaders)
    {
      foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in Enumerable.Where<KeyValuePair<string, IEnumerable<string>>>(httpResponseHeaders, (Func<KeyValuePair<string, IEnumerable<string>>, bool>) (kv => kv.Key.StartsWith("icy-", StringComparison.OrdinalIgnoreCase))))
      {
        Debug.WriteLine("Icecast {0}: ", (object) keyValuePair.Key);
        foreach (string str in keyValuePair.Value)
          Debug.WriteLine("       " + str);
      }
    }

    protected virtual async Task<ISegmentManager> ReadPlaylistAsync(IWebReader webReader, Uri url, Stream stream, CancellationToken cancellationToken)
    {
      PlsParser pls = new PlsParser(url);
      ISegmentManager segmentManager;
      using (StreamReader streamReader = new StreamReader(stream))
      {
        bool ret = await pls.ParseAsync((TextReader) streamReader).ConfigureAwait(false);
        if (!ret)
        {
          segmentManager = (ISegmentManager) null;
          goto label_8;
        }
      }
      segmentManager = await this.CreateManagerAsync(pls, webReader, cancellationToken).ConfigureAwait(false);
label_8:
      return segmentManager;
    }
  }
}
