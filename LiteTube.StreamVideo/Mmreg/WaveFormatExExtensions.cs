using System.Collections.Generic;
using System.Text;

namespace LiteTube.StreamVideo.Mmreg
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
