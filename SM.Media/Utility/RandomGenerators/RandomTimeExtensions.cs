// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.RandomGenerators.RandomTimeExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Utility.RandomGenerators
{
  public static class RandomTimeExtensions
  {
    public static TimeSpan RandomTimeSpan(this IRandomGenerator<ulong> generator, TimeSpan minimum, TimeSpan maximum)
    {
      long lessThan = maximum.Ticks - minimum.Ticks + 1L;
      long num = RandomGeneratorExtensions.Next(generator, lessThan);
      return minimum + TimeSpan.FromTicks(num);
    }

    public static Task RandomDelay(this IRandomGenerator<ulong> generator, TimeSpan minimum, TimeSpan maximum, CancellationToken cancellationToken)
    {
      return TaskEx.Delay(RandomTimeExtensions.RandomTimeSpan(generator, minimum, maximum), cancellationToken);
    }
  }
}
