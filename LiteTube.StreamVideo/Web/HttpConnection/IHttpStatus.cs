using System.Net;

namespace LiteTube.StreamVideo.Web.HttpConnection
{
  public interface IHttpStatus
  {
    bool ChunkedEncoding { get; }

    long? ContentLength { get; }

    HttpStatusCode StatusCode { get; }

    int VersionMajor { get; }

    int VersionMinor { get; }

    string ResponsePhrase { get; }

    string Version { get; }

    bool IsHttp { get; }

    bool IsSuccessStatusCode { get; }
  }
}
