using System;

namespace LiteTube.StreamVideo.Utility
{
  public static class FullResolutionTimeSpan
  {
    public static TimeSpan FromSeconds(double seconds)
    {
      return new TimeSpan((long) Math.Round(10000000.0 * seconds));
    }
  }
}
