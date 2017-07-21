// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.RandomGenerators.SystemRandomGenerator
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Core.Utility.RandomGenerators
{
  public class SystemRandomGenerator : IRandomGenerator
  {
    private readonly IPlatformServices _platformServices;
    private Random _random;

    public SystemRandomGenerator(IPlatformServices platformServices)
    {
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      this._platformServices = platformServices;
      this.Reseed();
    }

    public void GetBytes(byte[] buffer, int offset, int count)
    {
      if (null == buffer)
        throw new ArgumentNullException("buffer");
      if (offset < 0 || offset >= buffer.Length)
        throw new ArgumentOutOfRangeException("offset");
      if (count < 1 || count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count");
      if (offset == 0 && buffer.Length == offset)
      {
        this._random.NextBytes(buffer);
      }
      else
      {
        byte[] buffer1 = new byte[count];
        this._random.NextBytes(buffer1);
        Array.Copy((Array) buffer1, 0, (Array) buffer, offset, count);
      }
    }

    public float NextFloat()
    {
      return (float) this._random.NextDouble();
    }

    public double NextDouble()
    {
      return this._random.NextDouble();
    }

    public void Reseed()
    {
      byte[] bytes = new byte[4];
      this._platformServices.GetSecureRandom(bytes);
      this._random = new Random(BitConverter.ToInt32(bytes, 0));
    }
  }
}
