using System;

namespace SM.Media.Core.Utility.RandomGenerators
{
  public class Well512 : IRandomGenerator<uint>, IRandomGenerator
  {
    private readonly uint[] _state = new uint[16];
    private const float FloatScale = 2.328306E-10f;
    private const double DoubleScale = 5.42101086242752E-20;
    private readonly IPlatformServices _platformServices;
    private uint _index;

    public Well512(IPlatformServices platformServices)
    {
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      this._platformServices = platformServices;
      this.Reseed();
    }

    public void Reseed()
    {
      this._index = 0U;
      PlatformServicesExtensions.GetSecureRandom(this._platformServices, this._state);
    }

    public uint Next()
    {
      uint num1 = this._state[(int) this._index];
      uint num2 = this._state[(int) ((int) this._index + 13 & 15)];
      uint num3 = (uint) ((int) num1 ^ (int) num2 ^ (int) num1 << 16 ^ (int) num2 << 15);
      uint num4 = this._state[(int) ((int) this._index + 9 & 15)];
      uint num5 = num4 ^ num4 >> 11;
      uint num6 = this._state[(int) this._index] = num3 ^ num5;
      uint num7 = num6 ^ (uint) ((int) num6 << 5 & -633066204);
      this._index = (uint) ((int) this._index + 15 & 15);
      uint num8 = this._state[(int) this._index];
      this._state[(int) this._index] = (uint) ((int) num8 ^ (int) num3 ^ (int) num7 ^ (int) num8 << 2 ^ (int) num3 << 18 ^ (int) num5 << 28);
      return this._state[(int) this._index];
    }

    public void GetBytes(byte[] buffer, int offset, int count)
    {
label_5:
      uint num = this.Next();
      for (int index = 0; index < 4; ++index)
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
      return (float) this.Next() * 2.328306E-10f;
    }

    public double NextDouble()
    {
      return (double) ((ulong) this.Next() << 32 | (ulong) this.Next()) * 5.42101086242752E-20;
    }
  }
}
