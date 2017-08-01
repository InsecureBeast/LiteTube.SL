using System;

namespace LiteTube.StreamVideo.Web.HttpConnectionReader
{
  public class HttpConnectionRequestFactoryParameters : IHttpConnectionRequestFactoryParameters
  {
    public Uri Proxy { get; set; }
  }
}
