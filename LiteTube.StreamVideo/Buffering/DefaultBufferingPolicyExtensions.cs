using System;

namespace LiteTube.StreamVideo.Buffering
{
  public static class DefaultBufferingPolicyExtensions
  {
    private const int BytesMaximumLowerLimit = 524288;
    private const int BytesMinimumLowerLimit = 1024;

    public static DefaultBufferingPolicy SetBandwidth(this DefaultBufferingPolicy policy, double bitsPerSecond)
    {
      if (null == policy)
        throw new ArgumentNullException("policy");
      if (bitsPerSecond < 100.0 || bitsPerSecond > 524288000.0)
        throw new ArgumentOutOfRangeException("bitsPerSecond");
      double num1 = bitsPerSecond * 0.125;
      int num2 = (int) Math.Round(policy.DurationStartingDone.TotalSeconds * num1);
      TimeSpan timeSpan = policy.DurationBufferingDone;
      int num3 = (int) Math.Round(timeSpan.TotalSeconds * num1);
      double num4 = 2.0;
      timeSpan = policy.DurationBufferingMax;
      double totalSeconds = timeSpan.TotalSeconds;
      int num5 = (int) Math.Round(num4 * totalSeconds * num1);
      if (num2 < 1024)
        num2 = 1024;
      if (num3 < 1024)
        num3 = 1024;
      if (num5 < 524288)
        num5 = 524288;
      if (num3 > num5)
        num3 = num5;
      if (num2 > num5)
        num2 = num5;
      policy.BytesMinimumStarting = num2;
      policy.BytesMinimum = num3;
      policy.BytesMaximum = num5;
      return policy;
    }
  }
}
