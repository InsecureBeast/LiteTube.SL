// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.ByteConversion
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

namespace SM.Media.Utility
{
  public static class ByteConversion
  {
    private const double ToMiB = 9.5367431640625E-07;

    public static double BytesToMiB(this long value)
    {
      return (double) value * 9.5367431640625E-07;
    }

    public static double BytesToMiB(this ulong value)
    {
      return (double) value * 9.5367431640625E-07;
    }

    public static double? BytesToMiB(this long? value)
    {
      long? nullable = value;
      return nullable.HasValue ? new double?((double) nullable.GetValueOrDefault() * 9.5367431640625E-07) : new double?();
    }

    public static double? BytesToMiB(this ulong? value)
    {
      ulong? nullable = value;
      return nullable.HasValue ? new double?((double) nullable.GetValueOrDefault() * 9.5367431640625E-07) : new double?();
    }
  }
}
