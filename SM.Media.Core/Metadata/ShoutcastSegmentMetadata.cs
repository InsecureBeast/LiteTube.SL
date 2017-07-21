namespace SM.Media.Core.Metadata
{
  public class ShoutcastSegmentMetadata : SegmentMetadata, IShoutcastSegmentMetadata, ISegmentMetadata
  {
    public bool SupportsIcyMetadata { get; set; }

    public int? IcyMetaInt { get; set; }
  }
}
