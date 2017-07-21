// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnectionReader.HttpConnectionRequestFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Web.HttpConnection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SM.Media.Web.HttpConnectionReader
{
  public class HttpConnectionRequestFactory : IHttpConnectionRequestFactory
  {
    private readonly IHttpConnectionRequestFactoryParameters _parameters;

    public HttpConnectionRequestFactory(IHttpConnectionRequestFactoryParameters parameters)
    {
      if (null == parameters)
        throw new ArgumentNullException("parameters");
      this._parameters = parameters;
    }

    public virtual HttpConnectionRequest CreateRequest(Uri url, Uri referrer, ContentType contentType, long? fromBytes, long? toBytes, IEnumerable<KeyValuePair<string, string>> headers)
    {
      HttpConnectionRequest connectionRequest = new HttpConnectionRequest()
      {
        Url = url,
        Referrer = referrer,
        RangeFrom = fromBytes,
        RangeTo = toBytes,
        Proxy = this._parameters.Proxy,
        Headers = headers
      };
      if ((ContentType) null != contentType)
        connectionRequest.Accept = this.CreateAcceptHeader(contentType);
      return connectionRequest;
    }

    protected virtual string CreateAcceptHeader(ContentType contentType)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(contentType.MimeType);
      if (null != contentType.AlternateMimeTypes)
      {
        foreach (string str in (IEnumerable<string>) contentType.AlternateMimeTypes)
        {
          stringBuilder.Append(", ");
          stringBuilder.Append(str);
        }
      }
      stringBuilder.Append(", */*; q=0.1");
      return stringBuilder.ToString();
    }
  }
}
