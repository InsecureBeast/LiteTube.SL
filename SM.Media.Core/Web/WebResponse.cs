using System;
using System.Collections.Generic;
using SM.Media.Core.Content;

namespace SM.Media.Core.Web
{
  public class WebResponse
  {
    public Uri RequestUri { get; set; }

    public long? ContentLength { get; set; }

    public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; }

    public ContentType ContentType { get; set; }
  }
}
