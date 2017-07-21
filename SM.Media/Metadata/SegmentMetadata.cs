// Decompiled with JetBrains decompiler
// Type: SM.Media.Metadata.SegmentMetadata
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using System;

namespace SM.Media.Metadata
{
  public class SegmentMetadata : ISegmentMetadata
  {
    public Uri Url { get; set; }

    public ContentType ContentType { get; set; }

    public long? Length { get; set; }

    public override string ToString()
    {
      return "Segment <" + ((Uri) null == this.Url ? "null" : this.Url.ToString()) + "> " + ((ContentType) null == this.ContentType ? "<unknown>" : this.ContentType.Name);
    }
  }
}
