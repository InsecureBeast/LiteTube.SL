namespace SM.Media.Core.Metadata
{
  public interface IShoutcastSegmentMetadata : ISegmentMetadata
  {
    bool SupportsIcyMetadata { get; }

    int? IcyMetaInt { get; }
  }
}
