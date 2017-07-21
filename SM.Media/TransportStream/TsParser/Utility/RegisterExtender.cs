// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.Utility.RegisterExtender
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Diagnostics;

namespace SM.Media.TransportStream.TsParser.Utility
{
  internal sealed class RegisterExtender
  {
    private readonly int _width;
    private ulong _value;

    public RegisterExtender(ulong initialValue, int actualWidth)
    {
      if (actualWidth < 2 || actualWidth > 63)
        throw new ArgumentOutOfRangeException("actualWidth", "actualWidth must be between 2 and 63, inclusive.");
      this._value = initialValue;
      this._width = actualWidth;
    }

    public ulong Extend(ulong value)
    {
      long num1 = 1L << this._width - 1;
      ulong num2 = 1UL << this._width;
      ulong num3 = (ulong) ~((long) num2 - 1L);
      Debug.Assert(0L == ((long) value & (long) num3));
      value |= this._value & num3;
      if (Math.Abs((long) value - (long) this._value) > num1)
      {
        ulong num4 = value <= this._value ? value + num2 : value - num2;
        if (Math.Abs((long) num4 - (long) this._value) <= Math.Abs((long) value - (long) this._value))
          value = num4;
        Debug.Assert(Math.Abs((long) value - (long) this._value) <= num1);
      }
      return this._value = value;
    }
  }
}
