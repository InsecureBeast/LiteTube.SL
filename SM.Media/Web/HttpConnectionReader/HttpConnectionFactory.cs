// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnectionReader.HttpConnectionFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Web.HttpConnection;
using System;

namespace SM.Media.Web.HttpConnectionReader
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
