// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.WebResponse
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using System;
using System.Collections.Generic;

namespace SM.Media.Web
{
  public class WebResponse
  {
    public Uri RequestUri { get; set; }

    public long? ContentLength { get; set; }

    public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; }

    public ContentType ContentType { get; set; }
  }
}
