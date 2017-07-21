// Decompiled with JetBrains decompiler
// Type: SM.Media.AAC.AacParser
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Audio;
using SM.Media.TransportStream.TsParser;
using SM.Media.TransportStream.TsParser.Utility;
using System;
using System.Diagnostics;

namespace SM.Media.AAC
{
  public sealed class AacParser : AudioParserBase
  {
    public AacParser(ITsPesPacketPool pesPacketPool, Action<IAudioFrameHeader> configurationHandler, Action<TsPesPacket> submitPacket)
      : base((IAudioFrameHeader) new AacFrameHeader(), pesPacketPool, configurationHandler, submitPacket)
    {
    }

    public override void ProcessData(byte[] buffer, int offset, int length)
    {
      Debug.Assert(length > 0);
      Debug.Assert(offset + length <= buffer.Length);
      int endOffset = offset + length;
      this.EnsureBufferSpace(128);
      int index = offset;
      while (index < endOffset)
      {
        int storedLength = this._index - this._startIndex;
        if (storedLength <= 9)
          this.ProcessHeader(storedLength, buffer[index++]);
        else
          index += this.ProcessBody(buffer, index, storedLength, endOffset);
      }
    }

    private int ProcessBody(byte[] buffer, int index, int storedLength, int endOffset)
    {
      int val2 = this._frameHeader.FrameLength - storedLength;
      Debug.Assert(val2 >= 0);
      int val1 = endOffset - index;
      Debug.Assert(val1 >= 0);
      int length = Math.Min(val1, val2);
      Debug.Assert(length >= 0);
      if (length > 0)
        Array.Copy((Array) buffer, index, (Array) this._packet.Buffer, this._index, length);
      this._index += length;
      storedLength += length;
      Debug.Assert(storedLength >= 0 && storedLength == this._index - this._startIndex);
      Debug.Assert(storedLength <= this._frameHeader.FrameLength);
      if (storedLength == this._frameHeader.FrameLength)
      {
        if (AacDecoderSettings.Parameters.UseRawAac)
          this._startIndex += this._frameHeader.HeaderLength;
        this.SubmitFrame();
      }
      return length;
    }

    private void ProcessHeader(int storedLength, byte data)
    {
      while (0 != storedLength)
      {
        if (1 == storedLength)
        {
          if (240 == (240 & (int) data))
          {
            this._packet.Buffer[this._index++] = (byte) (int) data;
            return;
          }
          this._index = this._startIndex;
          storedLength = 0;
          ++this._badBytes;
        }
        else
        {
          if (storedLength < 9)
          {
            this._packet.Buffer[this._index++] = (byte) (int) data;
            return;
          }
          this._packet.Buffer[this._index++] = (byte) (int) data;
          bool verbose = !this._isConfigured && this._hasSeenValidFrames && 0 == this._badBytes;
          if (!this._frameHeader.Parse(this._packet.Buffer, this._startIndex, this._index - this._startIndex, verbose))
          {
            this.SkipInvalidFrameHeader();
            return;
          }
          Debug.Assert(this._frameHeader.FrameLength > 7);
          if (verbose)
          {
            this._configurationHandler(this._frameHeader);
            this._isConfigured = true;
          }
          this.EnsureBufferSpace(this._frameHeader.FrameLength);
          return;
        }
      }
      if ((int) byte.MaxValue == (int) data)
        this._packet.Buffer[this._index++] = (byte) (int) byte.MaxValue;
      else
        ++this._badBytes;
    }

    private void SkipInvalidFrameHeader()
    {
      if ((int) byte.MaxValue == (int) this._packet.Buffer[this._startIndex + 1] && 240 == (240 & (int) this._packet.Buffer[this._startIndex + 2]))
      {
        this._packet.Buffer[this._startIndex + 1] = this._packet.Buffer[this._startIndex + 2];
        this._packet.Buffer[this._startIndex + 2] = this._packet.Buffer[this._startIndex + 3];
        this._index = this._startIndex + 3;
        ++this._badBytes;
      }
      else if ((int) byte.MaxValue == (int) this._packet.Buffer[this._startIndex + 2] && 240 == (240 & (int) this._packet.Buffer[this._startIndex + 3]))
      {
        this._packet.Buffer[this._startIndex + 1] = this._packet.Buffer[this._startIndex + 3];
        this._index = this._startIndex + 2;
        this._badBytes += 2;
      }
      else if ((int) byte.MaxValue == (int) this._packet.Buffer[this._startIndex + 3])
      {
        this._index = this._startIndex + 1;
        this._badBytes += 3;
      }
      else
      {
        this._index = this._startIndex;
        this._badBytes += 4;
      }
    }
  }
}
