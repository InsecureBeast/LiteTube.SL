// Decompiled with JetBrains decompiler
// Type: SM.Media.H264.H264BitstreamExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

namespace SM.Media.H264
{
  internal static class H264BitstreamExtensions
  {
    public static uint ReadUe(this H264Bitstream h264Bitstream)
    {
      int count = 0;
      while (true)
      {
        if (0 == (int) h264Bitstream.ReadBits(1))
          ++count;
        else
          break;
      }
      if (0 == count)
        return 0;
      uint num = h264Bitstream.ReadBits(count);
      return (uint) ((1 << count) - 1) + num;
    }

    public static int ReadSe(this H264Bitstream h264Bitstream)
    {
      uint num1 = H264BitstreamExtensions.ReadUe(h264Bitstream);
      if (num1 < 2U)
        return (int) num1;
      int num2 = (int) (num1 >> 1);
      if (0 == ((int) num1 & 1))
        return -num2;
      return num2;
    }

    public static int ReadSignedBits(this H264Bitstream h264Bitstream, int count)
    {
      uint num1 = h264Bitstream.ReadBits(count);
      int num2 = 32 - count;
      return (int) num1 << num2 >> num2;
    }

    public static uint ReadFfSum(this H264Bitstream h264Bitstream)
    {
      uint num1 = 0;
      uint num2;
      do
      {
        num2 = h264Bitstream.ReadBits(8);
        num1 += num2;
      }
      while ((int) num2 == (int) byte.MaxValue);
      return num1;
    }
  }
}
