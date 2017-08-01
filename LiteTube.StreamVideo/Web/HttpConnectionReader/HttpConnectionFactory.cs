using System;
using LiteTube.StreamVideo.Web.HttpConnection;

namespace LiteTube.StreamVideo.Web.HttpConnectionReader
{
  public class HttpConnectionFactory : IHttpConnectionFactory
  {
    private readonly Func<IHttpConnection> _httpConnectionFactory;

    public HttpConnectionFactory(Func<IHttpConnection> httpConnectionFactory)
    {
      if (null == httpConnectionFactory)
        throw new ArgumentNullException("httpConnectionFactory");
      this._httpConnectionFactory = httpConnectionFactory;
    }

    public IHttpConnection CreateHttpConnection()
    {
      return this._httpConnectionFactory();
    }
  }
}
