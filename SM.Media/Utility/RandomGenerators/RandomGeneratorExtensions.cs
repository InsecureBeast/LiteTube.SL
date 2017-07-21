// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.RandomGenerators.RandomGeneratorExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Utility;
using System;

namespace SM.Media.Utility.RandomGenerators
{
  public static class RandomGeneratorExtensions
  {
    public static void GetBytes(this IRandomGenerator randomGenerator, byte[] buffer)
    {
      randomGenerator.GetBytes(buffer, 0, buffer.Length);
    }

    public static int Next(this IRandomGenerator<uint> randomGenerator, int lessThan)
    {
      if (lessThan <= 0)
        return 0;
      uint num1 = BitTwiddling.PowerOf2Mask((uint) lessThan);
      int num2;
      do
      {
        num2 = (int) randomGenerator.Next() & (int) num1;
      }
      while (num2 >= lessThan);
      return num2;
    }

    public static long Next(this IRandomGenerator<ulong> randomGenerator, long lessThan)
    {
      if (lessThan <= 0L)
        return 0;
      ulong num1 = BitTwiddling.PowerOf2Mask((ulong) lessThan);
      long num2;
      do
      {
        num2 = (long) randomGenerator.Next() & (long) num1;
      }
      while (num2 >= lessThan);
      return num2;
    }

    public static int NextInt(this IRandomGenerator randomGenerator)
    {
      IRandomGenerator<uint> randomGenerator1 = randomGenerator as IRandomGenerator<uint>;
      if (null != randomGenerator1)
        return (int) randomGenerator1.Next();
      IRandomGenerator<ulong> randomGenerator2 = randomGenerator as IRandomGenerator<ulong>;
      if (null != randomGenerator2)
        return (int) randomGenerator2.Next();
      return (int) (-randomGenerator.NextDouble() * (double) int.MinValue);
    }

    public static double NextExponential(this IRandomGenerator randomGenerator, double lambda)
    {
      return -Math.Log(randomGenerator.NextDouble()) / lambda;
    }
  }
}
