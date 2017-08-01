using LiteTube.StreamVideo.Web.HttpConnection;

namespace LiteTube.StreamVideo.Web.HttpConnectionReader
{
  public interface IHttpConnectionFactory
  {
    IHttpConnection CreateHttpConnection();
  }
}
