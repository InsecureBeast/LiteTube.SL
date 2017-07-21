using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Web.WebRequestReader
{
  public static class HttpWebRequestExtensions
  {
    private static readonly byte[] NoData = new byte[0];

    public static async Task<byte[]> ReadAsByteArrayAsync(this HttpWebResponse response, CancellationToken cancellationToken)
    {
      long contentLength = response.ContentLength;
      byte[] numArray;
      if (0L == contentLength)
      {
        numArray = HttpWebRequestExtensions.NoData;
      }
      else
      {
        if (contentLength > 2097152L)
          throw new WebException("Too much data for GetByteArrayAsync: " + (object) contentLength);
        using (MemoryStream memoryStream = contentLength > 0L ? new MemoryStream((int) contentLength) : new MemoryStream())
        {
          using (Stream responseStream = response.GetResponseStream())
            await responseStream.CopyToAsync((Stream) memoryStream, 4096, cancellationToken).ConfigureAwait(false);
          numArray = memoryStream.ToArray();
        }
      }
      return numArray;
    }

    public static async Task<HttpWebResponse> SendAsync(this HttpWebRequest request, CancellationToken cancellationToken)
    {
      var task = Task<System.Net.WebResponse>.Factory.FromAsync(((WebRequest) request).BeginGetResponse, ((WebRequest) request).EndGetResponse, null);
      HttpWebResponse httpWebResponse;
      using (cancellationToken.Register((Action<object>) (r => ((WebRequest) r).Abort()), (object) request, false))
      {
        try
        {
          httpWebResponse = (HttpWebResponse) await task.ConfigureAwait(false);
        }
        catch (WebException ex)
        {
          if (cancellationToken.IsCancellationRequested && ex.Status == WebExceptionStatus.RequestCanceled)
            throw new OperationCanceledException(ex.Message, (Exception) ex, cancellationToken);
          throw;
        }
      }
      return httpWebResponse;
    }
  }
}
