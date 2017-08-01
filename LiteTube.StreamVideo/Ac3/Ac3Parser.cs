using System;
using System.Diagnostics;
using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.TransportStream.TsParser.Utility;

namespace LiteTube.StreamVideo.Ac3
{
  public sealed class Ac3Parser : AudioParserBase
  {
    public Ac3Parser(ITsPesPacketPool pesPacketPool, Action<IAudioFrameHeader> configurationHandler, Action<TsPesPacket> submitPacket)
      : base((IAudioFrameHeader) new Ac3FrameHeader(), pesPacketPool, configurationHandler, submitPacket)
    {
    }

    public override void ProcessData(byte[] buffer, int offset, int length)
    {
      Debug.Assert(length > 0);
      Debug.Assert(offset + length <= buffer.Length);
      int num1 = offset + length;
      this.EnsureBufferSpace(128);
      int sourceIndex = offset;
label_23:
      while (sourceIndex < num1)
      {
        int num2 = this._index - this._startIndex;
        if (num2 <= 4)
        {
          byte num3 = buffer[sourceIndex++];
          while (0 != num2)
          {
            if (1 == num2)
            {
              if (119 == (int) num3)
              {
                this._packet.Buffer[this._index++] = (byte) (int) num3;
                goto label_22;
              }
              else
              {
                this._index = this._startIndex;
                num2 = 0;
                ++this._badBytes;
              }
            }
            else if (num2 < 4)
            {
              this._packet.Buffer[this._index++] = (byte) (int) num3;
              goto label_22;
            }
            else
            {
              this._packet.Buffer[this._index++] = (byte) (int) num3;
              bool verbose = !this._isConfigured && this._hasSeenValidFrames && 0 == this._badBytes;
              if (!this._frameHeader.Parse(this._packet.Buffer, this._startIndex, this._index - this._startIndex, verbose))
              {
                this.SkipInvalidFrameHeader();
                goto label_23;
              }
              else
              {
                Debug.Assert(this._frameHeader.FrameLength > 7);
                if (verbose)
                {
                  this._configurationHandler(this._frameHeader);
                  this._isConfigured = true;
                }
                this.EnsureBufferSpace(this._frameHeader.FrameLength);
                goto label_22;
              }
            }
          }
          if (11 == (int) num3)
            this._packet.Buffer[this._index++] = (byte) 11;
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
label_22:;
      }
    }

    private void SkipInvalidFrameHeader()
    {
      for (int sourceIndex = this._startIndex + 1; sourceIndex < this._index; ++sourceIndex)
      {
        if ((int) byte.MaxValue == (int) this._packet.Buffer[sourceIndex])
        {
          Array.Copy((Array) this._packet.Buffer, sourceIndex, (Array) this._packet.Buffer, this._startIndex, this._index - sourceIndex);
          this._index = sourceIndex;
          this._badBytes += sourceIndex - this._startIndex;
          return;
        }
      }
      this._badBytes += this._index - this._startIndex;
      this._index = this._startIndex;
    }
  }
}
