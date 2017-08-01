using System;
using System.Collections.Generic;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Web
{
  public class WebResponse
  {
    public Uri RequestUri { get; set; }

    public long? ContentLength { get; set; }

    public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; }

    public ContentType ContentType { get; set; }
  }
}
