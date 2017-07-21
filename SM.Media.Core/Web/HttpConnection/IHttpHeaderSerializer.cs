using System.IO;

namespace SM.Media.Core.Web.HttpConnection
{
  public interface IHttpHeaderSerializer
  {
    void WriteHeader(Stream stream, string method, HttpConnectionRequest request);
  }
}
