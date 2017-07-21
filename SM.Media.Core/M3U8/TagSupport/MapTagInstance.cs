namespace SM.Media.Core.M3U8.TagSupport
{
  internal sealed class MapTagInstance : M3U8TagInstance
  {
    private MapTagInstance(M3U8Tag tag)
      : base(tag)
    {
    }

    internal static M3U8TagInstance Create(M3U8Tag tag, string value)
    {
      return (M3U8TagInstance) new MapTagInstance(tag);
    }
  }
}
