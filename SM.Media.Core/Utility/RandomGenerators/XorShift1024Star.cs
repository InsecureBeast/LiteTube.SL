// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.RandomGenerators.XorShift1024Star
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Core.Utility.RandomGenerators
{
  public class XorShift1024Star : IRandomGenerator<ulong>, IRandomGenerator<uint>, IRandomGenerator
  {
    private readonly ulong[] _s = new ulong[16];
    private const float FloatScale = 2.328306E-10f;
    private const double DoubleScale = 5.42101086242752E-20;
    private readonly IPlatformServices _platformServices;
    private int _p;
    private uint? _stored;

    public XorShift1024Star(IPlatformServices platformServices)
    {
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      this._platformServices = platformServices;
      this.Reseed();
    }

    uint IRandomGenerator<uint>.Next()
    {
      if (this._stored.HasValue)
      {
        uint num = this._stored.Value;
        this._stored = new uint?();
        return num;
      }
      ulong num1 = this.Next();
      this._stored = new uint?((uint) (num1 >> 32));
      return (uint) num1;
    }

    public void Reseed()
    {
      this._p = 0;
      PlatformServicesExtensions.GetSecureRandom(this._platformServices, this._s);
    }

    public ulong Next()
    {
      ulong num1 = this._s[this._p];
      ulong num2 = this._s[this._p = this._p + 1 & 15];
      ulong num3 = num2 ^ num2 << 31;
      ulong num4 = num3 ^ num3 >> 11;
      return (this._s[this._p] = num1 ^ num1 >> 30 ^ num4) * 1181783497276652981UL;
    }

    public void GetBytes(byte[] buffer, int offset, int count)
    {
label_5:
      ulong num = this.Next();
      for (int index = 0; index < 8; ++index)
      {
        if (count <= 0)
          return;
        buffer[offset++] = (byte) num;
        --count;
        num >>= 8;
      }
      goto label_5;
    }

    public float NextFloat()
    {
      return (float) ((IRandomGenerator<uint>) this).Next() * 2.328306E-10f;
    }

    public double NextDouble()
    {
      return (double) this.Next() * 5.42101086242752E-20;
    }
  }
}
