// Decompiled with JetBrains decompiler
// Type: SM.Media.H264.NalUnitParser
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.H264
{
  public class NalUnitParser
  {
    private static readonly byte[] ZeroBuffer = new byte[4];
    private readonly Func<byte, NalUnitParser.ParserStateHandler> _resolveHandler;
    private NalUnitParser.ParserStateHandler _currentParser;
    private bool _expectingNalUnitType;
    private bool _hasEscape;
    private int _lastCompletedOffset;
    private int _nalOffset;
    private int _zeroCount;

    public NalUnitParser(Func<byte, NalUnitParser.ParserStateHandler> resolveHandler)
    {
      this._resolveHandler = resolveHandler;
    }

    private void CompleteNalUnit(byte[] buffer, int offset, int length)
    {
      this._lastCompletedOffset = offset + length;
      if (null == this._currentParser)
        return;
      int num = this._currentParser(buffer, offset, length, this._hasEscape) ? 1 : 0;
    }

    public void Reset()
    {
      this._zeroCount = 0;
      this._expectingNalUnitType = false;
      this._nalOffset = -1;
      this._hasEscape = false;
    }

    public int Parse(byte[] buffer, int offset, int length, bool isLast = true)
    {
      this._lastCompletedOffset = 0;
      if (0 == length)
      {
        if (null != this._currentParser)
        {
          if (this._zeroCount > 0 && !this._expectingNalUnitType)
            this.CompleteNalUnit(NalUnitParser.ZeroBuffer, 0, Math.Min(this._zeroCount, 3));
          int num = this._currentParser((byte[]) null, 0, 0, false) ? 1 : 0;
          this._currentParser = (NalUnitParser.ParserStateHandler) null;
        }
        this._zeroCount = 0;
        return 0;
      }
      for (int index = 0; index < length; ++index)
      {
        byte num = buffer[index + offset];
        if (0 == (int) num)
        {
          if (++this._zeroCount >= 3)
          {
            if (this._nalOffset >= 0)
            {
              int length1 = index + 1 - this._zeroCount - this._nalOffset;
              if (length1 > 0)
                this.CompleteNalUnit(buffer, offset + this._nalOffset, length1);
            }
            this._nalOffset = -1;
          }
          this._expectingNalUnitType = false;
        }
        else
        {
          int val1 = this._zeroCount;
          this._zeroCount = 0;
          if (this._expectingNalUnitType)
          {
            this._expectingNalUnitType = false;
            this._currentParser = this._resolveHandler(num);
            this._nalOffset = index;
            this._hasEscape = false;
          }
          else if (val1 >= 2)
          {
            if ((int) num == 1)
            {
              if (this._nalOffset >= 0)
              {
                int length1 = index - this._nalOffset - Math.Min(val1, 3);
                if (length1 > 0)
                  this.CompleteNalUnit(buffer, offset + this._nalOffset, length1);
              }
              this._expectingNalUnitType = true;
              this._nalOffset = -1;
            }
            else if ((int) num == 3)
              this._hasEscape = true;
          }
        }
      }
      if (isLast && !this._expectingNalUnitType && this._nalOffset >= 0)
      {
        int length1 = length - this._nalOffset;
        if (length1 > 0)
          this.CompleteNalUnit(buffer, offset + this._nalOffset, length1);
      }
      int num1 = this._lastCompletedOffset - offset;
      if (isLast)
        this.Reset();
      if (num1 > 0)
        return num1;
      return 0;
    }

    public delegate bool ParserStateHandler(byte[] buffer, int offset, int length, bool hasEscape);
  }
}
