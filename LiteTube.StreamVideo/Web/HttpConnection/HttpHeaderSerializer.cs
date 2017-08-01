using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LiteTube.StreamVideo.Web.HttpConnection
{
  public class HttpHeaderSerializer : IHttpHeaderSerializer
  {
    private const string HttpEol = "\r\n";
    private readonly Encoding _headerEncoding;
    private readonly string _userAgentLine;

    public HttpHeaderSerializer(IUserAgentEncoder userAgentEncoder, IHttpEncoding httpEncoding)
    {
      if (null == userAgentEncoder)
        throw new ArgumentNullException("userAgentEncoder");
      if (null == httpEncoding)
        throw new ArgumentNullException("httpEncoding");
      string usAsciiUserAgent = userAgentEncoder.UsAsciiUserAgent;
      if (!string.IsNullOrWhiteSpace(usAsciiUserAgent))
        this._userAgentLine = "User-Agent: " + usAsciiUserAgent.Trim();
      this._headerEncoding = httpEncoding.HeaderEncoding;
    }

    public void WriteHeader(Stream stream, string method, HttpConnectionRequest request)
    {
      Uri url = request.Url;
      using (StreamWriter streamWriter = new StreamWriter(stream, this._headerEncoding, 1024, true))
      {
        streamWriter.NewLine = "\r\n";
        string requestTarget = HttpHeaderSerializer.GetRequestTarget(request);
        string host = HttpHeaderSerializer.GetHost(url);
        streamWriter.WriteLine(method.ToUpperInvariant() + " " + requestTarget + " HTTP/1.1");
        streamWriter.WriteLine("Host: " + host);
        streamWriter.WriteLine(request.KeepAlive ? "Connection: Keep-Alive" : "Connection: Close");
        if ((Uri) null != request.Referrer)
          streamWriter.WriteLine("Referer:" + (object) request.Referrer);
        long? nullable = request.RangeFrom;
        int num;
        if (!nullable.HasValue)
        {
          nullable = request.RangeTo;
          num = !nullable.HasValue ? 1 : 0;
        }
        else
          num = 0;
        if (num == 0)
          streamWriter.WriteLine("Range: bytes={0}-{1}", new object[2]
          {
            (object) request.RangeFrom,
            (object) request.RangeTo
          });
        if (null != this._userAgentLine)
          streamWriter.WriteLine(this._userAgentLine);
        if (!string.IsNullOrWhiteSpace(request.Accept))
          streamWriter.WriteLine("Accept: " + request.Accept.Trim());
        if (null != request.Headers)
        {
          foreach (KeyValuePair<string, string> keyValuePair in request.Headers)
          {
            string str1 = keyValuePair.Value;
            string str2 = !string.IsNullOrWhiteSpace(str1) ? str1.Trim() : string.Empty;
            streamWriter.WriteLine(keyValuePair.Key.Trim() + ": " + str2);
          }
        }
        streamWriter.WriteLine();
        streamWriter.Flush();
      }
    }

    private static string GetHost(Uri url)
    {
      return url.IsDefaultPort ? url.Host : url.Host + (object) ':' + (string) (object) url.Port;
    }

    private static string GetRequestTarget(HttpConnectionRequest request)
    {
      Uri url = request.Url;
      if (!((Uri) null != request.Proxy))
        return url.PathAndQuery;
      return new UriBuilder(url)
      {
        Fragment = ((string) null)
      }.Uri.ToString();
    }
  }
}
