using System;
using System.Diagnostics;
using SM.Media.Core.Audio;
using SM.Media.Core.TransportStream.TsParser;
using SM.Media.Core.TransportStream.TsParser.Utility;

namespace SM.Media.Core.MP3
{
  public sealed class Mp3Parser : AudioParserBase
  {
    private int _skip;

    public Mp3Parser(ITsPesPacketPool pesPacketPool, Action<IAudioFrameHeader> configurationHandler, Action<TsPesPacket> submitPacket)
      : base((IAudioFrameHeader) new Mp3FrameHeader(), pesPacketPool, configurationHandler, submitPacket)
    {
    }

    public override void ProcessData(byte[] buffer, int offset, int length)
    {
      Debug.Assert(length > 0);
      Debug.Assert(offset + length <= buffer.Length);
      if (this._skip == 0 && this._index == 0 && 0 == this._startIndex)
      {
        int? id3Length = Mp3Parser.GetId3Length(buffer, offset, length);
        if (id3Length.HasValue)
        {
          this._skip = id3Length.Value + 10;
          Debug.WriteLine("Mp3Parser.ProcessData() ID3 detected, length {0}", (object) this._skip);
        }
      }
      if (this._skip > 0)
      {
        if (this._skip >= length)
        {
          this._skip -= length;
          return;
        }
        offset += this._skip;
        length -= this._skip;
        this._skip = 0;
      }
      int num1 = offset + length;
      this.EnsureBufferSpace(128);
      int sourceIndex = offset;
label_32:
      while (sourceIndex < num1)
      {
        int num2 = this._index - this._startIndex;
        if (num2 < 4)
        {
          byte num3 = buffer[sourceIndex++];
          while (0 != num2)
          {
            if (1 == num2)
            {
              if (224 == (224 & (int) num3))
              {
                this._packet.Buffer[this._index++] = (byte) (int) num3;
                goto label_31;
              }
              else
              {
                this._index = this._startIndex;
                num2 = 0;
                ++this._badBytes;
              }
            }
            else if (2 == num2)
            {
              this._packet.Buffer[this._index++] = (byte) (int) num3;
              goto label_31;
            }
            else if (3 == num2)
            {
              this._packet.Buffer[this._index++] = (byte) (int) num3;
              bool verbose = !this._isConfigured && this._hasSeenValidFrames && 0 == this._badBytes;
              if (!this._frameHeader.Parse(this._packet.Buffer, this._startIndex, this._index - this._startIndex, verbose))
              {
                this.SkipInvalidFrameHeader();
                goto label_32;
              }
              else
              {
                Debug.Assert(this._frameHeader.FrameLength > 4);
                if (verbose)
                {
                  this._configurationHandler(this._frameHeader);
                  this._isConfigured = true;
                }
                this.EnsureBufferSpace(this._frameHeader.FrameLength - 4);
                goto label_31;
              }
            }
            else
              goto label_31;
          }
          if ((int) byte.MaxValue == (int) num3)
            this._packet.Buffer[this._index++] = (byte) (int) byte.MaxValue;
          else
            ++this._badBytes;
        }
        else
        {
          int val2 = this._frameHeader.FrameLength - (this._index - this._startIndex);
          int length1 = Math.Min(num1 - sourceIndex, val2);
          Debug.Assert(length1 > 0);
          Array.Copy((Array) buffer, sourceIndex, (Array) this._packet.Buffer, this._index, length1);
          this._index += length1;
          sourceIndex += length1;
          if (this._index - this._startIndex == this._frameHeader.FrameLength)
            this.SubmitFrame();
        }
label_31:;
      }
    }

    public override void FlushBuffers()
    {
      base.FlushBuffers();
      this._skip = 0;
    }

    private static int? GetId3Length(byte[] buffer, int offset, int length)
    {
      if (length < 10)
        return new int?();
      if (73 != (int) buffer[offset] || 68 != (int) buffer[offset + 1] || 51 != (int) buffer[offset + 2])
        return new int?();
      if ((int) byte.MaxValue == (int) buffer[offset + 3])
        return new int?();
      if ((int) byte.MaxValue == (int) buffer[offset + 4])
        return new int?();
      byte num1 = buffer[offset + 5];
      int num2 = 0;
      for (int index = 0; index < 4; ++index)
      {
        byte num3 = buffer[offset + 6 + index];
        if (0 != (128 & (int) num3))
          return new int?();
        num2 = num2 << 7 | (int) num3;
      }
      return new int?(num2);
    }

    private void SkipInvalidFrameHeader()
    {
      if ((int) byte.MaxValue == (int) this._packet.Buffer[this._startIndex + 1] && 224 == (224 & (int) this._packet.Buffer[this._startIndex + 2]))
      {
        this._packet.Buffer[this._startIndex + 1] = this._packet.Buffer[this._startIndex + 2];
        this._packet.Buffer[this._startIndex + 2] = this._packet.Buffer[this._startIndex + 3];
        this._index = this._startIndex + 3;
        ++this._badBytes;
      }
      else if ((int) byte.MaxValue == (int) this._packet.Buffer[this._startIndex + 2] && 224 == (224 & (int) this._packet.Buffer[this._startIndex + 3]))
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
