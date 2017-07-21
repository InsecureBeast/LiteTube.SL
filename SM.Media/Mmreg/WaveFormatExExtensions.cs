// Decompiled with JetBrains decompiler
// Type: SM.Media.Mmreg.WaveFormatExExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;
using System.Text;

namespace SM.Media.Mmreg
{
  public static class WaveFormatExExtensions
  {
    public static void AddLe(this IList<byte> buffer, ushort value)
    {
      buffer.Add((byte) ((uint) value & (uint) byte.MaxValue));
      buffer.Add((byte) ((uint) value >> 8));
    }

    public static void AddLe(this IList<byte> buffer, uint value)
    {
      WaveFormatExExtensions.AddLe(buffer, (ushort) value);
      WaveFormatExExtensions.AddLe(buffer, (ushort) (value >> 16));
    }

    public static string ToCodecPrivateData(this WaveFormatEx waveFormatEx)
    {
      List<byte> list = new List<byte>(18 + (int) waveFormatEx.cbSize);
      waveFormatEx.ToBytes((IList<byte>) list);
      StringBuilder stringBuilder = new StringBuilder(list.Count * 2);
      foreach (byte num in list)
        stringBuilder.Append(num.ToString("X2"));
      return stringBuilder.ToString();
    }
  }
}
