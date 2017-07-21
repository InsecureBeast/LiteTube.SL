using System;
using SM.Media.Core.Content;

namespace SM.Media.Core.Metadata
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
