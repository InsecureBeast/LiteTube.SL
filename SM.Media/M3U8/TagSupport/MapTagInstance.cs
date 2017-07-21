// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.TagSupport.MapTagInstance
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;

namespace SM.Media.M3U8.TagSupport
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
