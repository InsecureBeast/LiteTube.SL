using System;

namespace SM.Media.Core.Web.HttpConnectionReader
{
  public class HttpConnectionRequestFactoryParameters : IHttpConnectionRequestFactoryParameters
  {
    public Uri Proxy { get; set; }
  }
}
