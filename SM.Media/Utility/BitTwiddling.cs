// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.BitTwiddling
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

namespace SM.Media.Utility
{
  public static class BitTwiddling
  {
    public static uint NextPowerOf2(uint v)
    {
      v = BitTwiddling.PowerOf2Mask(v);
      ++v;
      return v;
    }

    public static ulong NextPowerOf2(ulong v)
    {
      v = BitTwiddling.PowerOf2Mask(v);
      ++v;
      return v;
    }

    public static uint PowerOf2Mask(uint v)
    {
      if (0 == (int) v)
        return 0;
      --v;
      v |= v >> 1;
      v |= v >> 2;
      v |= v >> 4;
      v |= v >> 8;
      v |= v >> 16;
      return v;
    }

    public static ulong PowerOf2Mask(ulong v)
    {
      if (0L == (long) v)
        return 0;
      --v;
      v |= v >> 1;
      v |= v >> 2;
      v |= v >> 4;
      v |= v >> 8;
      v |= v >> 16;
      v |= v >> 32;
      return v;
    }
  }
}
