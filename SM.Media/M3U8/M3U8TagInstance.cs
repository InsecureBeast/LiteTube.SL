// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.M3U8TagInstance
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

namespace SM.Media.M3U8
{
  public class M3U8TagInstance
  {
    public readonly M3U8Tag Tag;

    public M3U8TagInstance(M3U8Tag tag)
    {
      this.Tag = tag;
    }

    public override string ToString()
    {
      return this.Tag.ToString();
    }
  }
}
