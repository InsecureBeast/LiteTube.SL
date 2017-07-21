using System.Net;

namespace SM.Media.Core.Web.HttpConnection
{
  public sealed class HttpStatus : IHttpStatus
  {
    public bool ChunkedEncoding { get; set; }

    public long? ContentLength { get; set; }

    public HttpStatusCode StatusCode { get; set; }

    public int VersionMajor { get; set; }

    public int VersionMinor { get; set; }

    public string ResponsePhrase { get; set; }

    public string Version { get; set; }

    public bool IsHttp { get; set; }

    public bool IsSuccessStatusCode
    {
      get
      {
        int num = (int) this.StatusCode;
        return num >= 200 && num <= 299;
      }
    }
  }
}
