using SM.Media.Core.Web.HttpConnection;

namespace SM.Media.Core.Web.HttpConnectionReader
{
  public interface IHttpConnectionFactory
  {
    IHttpConnection CreateHttpConnection();
  }
}
