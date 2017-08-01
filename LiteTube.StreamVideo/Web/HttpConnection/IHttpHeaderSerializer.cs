using System.IO;

namespace LiteTube.StreamVideo.Web.HttpConnection
{
  public interface IHttpHeaderSerializer
  {
    void WriteHeader(Stream stream, string method, HttpConnectionRequest request);
  }
}
