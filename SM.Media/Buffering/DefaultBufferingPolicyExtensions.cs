// Decompiled with JetBrains decompiler
// Type: SM.Media.Buffering.DefaultBufferingPolicyExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Buffering
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
