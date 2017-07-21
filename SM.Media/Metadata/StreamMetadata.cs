// Decompiled with JetBrains decompiler
// Type: SM.Media.Metadata.StreamMetadata
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using System;

namespace SM.Media.Metadata
{
  public class StreamMetadata : IStreamMetadata
  {
    public Uri Url { get; set; }

    public ContentType ContentType { get; set; }

    public int? Bitrate { get; set; }

    public TimeSpan? Duration { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Genre { get; set; }

    public Uri Website { get; set; }

    public override string ToString()
    {
      return "Stream " + (string.IsNullOrWhiteSpace(this.Name) ? "{null}" : (string) (object) '"' + (object) this.Name + (string) (object) '"') + " <" + ((Uri) null == this.Url ? "null" : this.Url.ToString()) + "> " + ((ContentType) null == this.ContentType ? "<unknown>" : this.ContentType.Name);
    }
  }
}
