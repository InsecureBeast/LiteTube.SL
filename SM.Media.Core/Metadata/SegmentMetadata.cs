using System;
using SM.Media.Core.Content;

namespace SM.Media.Core.Metadata
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
