namespace LiteTube.StreamVideo.Utility
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
