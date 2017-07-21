using System;
using System.Collections.Generic;
using System.Text;
using SM.Media.Core.Content;
using SM.Media.Core.Web.HttpConnection;

namespace SM.Media.Core.Web.HttpConnectionReader
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
