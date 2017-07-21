using System;
using System.Collections.Generic;

namespace SM.Media.Core.H264
{
  internal class H264Bitstream : IDisposable
  {
    private int _bitsLeft;
    private IEnumerator<byte> _bytes;
    private byte _currentByte;
    private byte _nextByte;

    public bool HasData
    {
      get
      {
        if (this._bitsLeft > 0)
          return true;
        return this.GetMoreBits();
      }
    }

    public H264Bitstream(IEnumerable<byte> buffer)
    {
      this._bytes = buffer.GetEnumerator();
      if (!this._bytes.MoveNext())
      {
        this._bytes.Dispose();
        this._bytes = (IEnumerator<byte>) null;
      }
      else
      {
        this._nextByte = this._bytes.Current;
        this._bitsLeft = 0;
      }
    }

    public void Dispose()
    {
      using (this._bytes)
        ;
    }

    private bool GetMoreBits()
    {
      if (this._bitsLeft > 0)
        return true;
      if (null == this._bytes)
        return false;
      this._currentByte = this._nextByte;
      if (!this._bytes.MoveNext())
      {
        this._bytes.Dispose();
        this._bytes = (IEnumerator<byte>) null;
        this._bitsLeft = 0;
        int num = 0;
        while (0 == (1 & (int) this._currentByte))
        {
          ++num;
          this._currentByte >>= 1;
        }
        if (num > 6)
          return false;
        this._currentByte >>= 1;
        this._bitsLeft = 7 - num;
        return true;
      }
      this._bitsLeft = 8;
      this._nextByte = this._bytes.Current;
      return true;
    }

    public uint ReadBits(int count)
    {
      uint num1 = 0;
      while (count > 0)
      {
        if (this._bitsLeft < 1 && !this.GetMoreBits())
          throw new FormatException("Read past the end of the RBSP stream");
        byte num2 = this._currentByte;
        if (8 == this._bitsLeft && count >= 8)
        {
          num1 = num1 << 8 | (uint) num2;
          count -= 8;
          this._bitsLeft = 0;
        }
        else
        {
          int num3 = Math.Min(this._bitsLeft, count);
          uint num4 = num1 << num3;
          uint num5 = (uint) ((1 << num3) - 1);
          byte num6 = (byte) ((uint) num2 >> this._bitsLeft - num3);
          num1 = num4 | (uint) num6 & num5;
          this._bitsLeft -= num3;
          count -= num3;
        }
      }
      return num1;
    }
  }
}
