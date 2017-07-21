// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.RandomGenerators.NormalDistributionExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;

namespace SM.Media.Core.Utility.RandomGenerators
{
  public static class NormalDistributionExtensions
  {
    public static IEnumerable<float> AsEnumerable(this NormalDistribution normalDistribution)
    {
      while (true)
        yield return normalDistribution.Next();
    }
  }
}
