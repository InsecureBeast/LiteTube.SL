// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.WebRequestReader.HttpWebRequestExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web.WebRequestReader
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
      Task<WebResponse> task = Task<WebResponse>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(((WebRequest) request).BeginGetResponse), new Func<IAsyncResult, WebResponse>(((WebRequest) request).EndGetResponse), (object) null);
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
