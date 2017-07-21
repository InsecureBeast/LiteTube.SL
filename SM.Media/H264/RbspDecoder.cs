// Decompiled with JetBrains decompiler
// Type: SM.Media.H264.RbspDecoder
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SM.Media.H264
{
  public class RbspDecoder : INalParser
  {
    private readonly List<byte> _outputBuffer = new List<byte>();

    public Action<IList<byte>> CompletionHandler { get; set; }

    public bool Parse(byte[] buffer, int offset, int length, bool hasEscape)
    {
      if (hasEscape)
      {
        buffer = this.RemoveEscapes(buffer, offset, length);
        offset = 0;
        length = buffer.Length;
      }
      int length1 = length;
      for (int index = length - 1; index > 0 && 0 == (int) buffer[offset + index]; --index)
      {
        Debug.WriteLine("RBSP with trailing zero");
        length1 = index + 1;
      }
      if (length1 <= 0)
        return false;
      if (null != this.CompletionHandler)
      {
        if (!hasEscape || length1 != length)
        {
          byte[] numArray = new byte[length1];
          Array.Copy((Array) buffer, offset, (Array) numArray, 0, length1);
          buffer = numArray;
          length = length1;
          offset = 0;
        }
        this.CompletionHandler((IList<byte>) buffer);
      }
      return true;
    }

    private byte[] RemoveEscapes(byte[] buffer, int offset, int length)
    {
      this.PrepareOutputBuffer(length);
      int num1 = 0;
      for (int index = 0; index < length; ++index)
      {
        byte num2 = buffer[offset + index];
        if (2 == num1)
        {
          if ((int) num2 < 3)
            throw new FormatException("Invalid escape sequence");
          if (3 == (int) num2)
          {
            if (index + 1 < length && (int) buffer[offset + index + 1] > 3)
              throw new FormatException("Invalid escape sequence");
            num1 = 0;
            continue;
          }
        }
        if (0 == (int) num2)
          ++num1;
        else
          num1 = 0;
        this._outputBuffer.Add(num2);
      }
      return this._outputBuffer.ToArray();
    }

    private void PrepareOutputBuffer(int length)
    {
      this._outputBuffer.Clear();
      if (this._outputBuffer.Capacity >= length)
        return;
      int num = this._outputBuffer.Capacity;
      if (num < 8)
        num = 8;
      else if (length > 536870911)
        num = length;
      while (num < length)
        num *= 2;
      this._outputBuffer.Capacity = num;
    }
  }
}
