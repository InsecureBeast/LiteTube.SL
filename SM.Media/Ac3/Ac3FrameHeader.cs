// Decompiled with JetBrains decompiler
// Type: SM.Media.Ac3.Ac3FrameHeader
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SM.Media.Ac3
{
  internal sealed class Ac3FrameHeader : IAudioFrameHeader
  {
    internal static readonly TimeSpan FrameDuration = TimeSpan.FromMilliseconds(32.0);
    private static readonly int[] SamplingFrequencyTable = new int[3]
    {
      48000,
      44100,
      32000
    };
    private static readonly Dictionary<byte, Ac3FrameHeader.FrameCode> FrameCodes = Enumerable.ToDictionary<Ac3FrameHeader.FrameCode, byte>((IEnumerable<Ac3FrameHeader.FrameCode>) new Ac3FrameHeader.FrameCode[38]
    {
      new Ac3FrameHeader.FrameCode((byte) 0, (short) 32, (short) 96, (short) 69, (short) 64),
      new Ac3FrameHeader.FrameCode((byte) 1, (short) 32, (short) 96, (short) 70, (short) 64),
      new Ac3FrameHeader.FrameCode((byte) 2, (short) 40, (short) 120, (short) 87, (short) 80),
      new Ac3FrameHeader.FrameCode((byte) 3, (short) 40, (short) 120, (short) 88, (short) 80),
      new Ac3FrameHeader.FrameCode((byte) 4, (short) 48, (short) 144, (short) 104, (short) 96),
      new Ac3FrameHeader.FrameCode((byte) 5, (short) 48, (short) 144, (short) 105, (short) 96),
      new Ac3FrameHeader.FrameCode((byte) 6, (short) 56, (short) 168, (short) 121, (short) 112),
      new Ac3FrameHeader.FrameCode((byte) 7, (short) 56, (short) 168, (short) 122, (short) 112),
      new Ac3FrameHeader.FrameCode((byte) 8, (short) 64, (short) 192, (short) 139, (short) 128),
      new Ac3FrameHeader.FrameCode((byte) 9, (short) 64, (short) 192, (short) 140, (short) 128),
      new Ac3FrameHeader.FrameCode((byte) 10, (short) 80, (short) 240, (short) 174, (short) 160),
      new Ac3FrameHeader.FrameCode((byte) 11, (short) 80, (short) 240, (short) 175, (short) 160),
      new Ac3FrameHeader.FrameCode((byte) 12, (short) 96, (short) 288, (short) 208, (short) 192),
      new Ac3FrameHeader.FrameCode((byte) 13, (short) 96, (short) 288, (short) 209, (short) 192),
      new Ac3FrameHeader.FrameCode((byte) 14, (short) 112, (short) 336, (short) 243, (short) 224),
      new Ac3FrameHeader.FrameCode((byte) 15, (short) 112, (short) 336, (short) 244, (short) 224),
      new Ac3FrameHeader.FrameCode((byte) 16, (short) 128, (short) 384, (short) 278, (short) 256),
      new Ac3FrameHeader.FrameCode((byte) 17, (short) 128, (short) 384, (short) 279, (short) 256),
      new Ac3FrameHeader.FrameCode((byte) 18, (short) 160, (short) 480, (short) 348, (short) 320),
      new Ac3FrameHeader.FrameCode((byte) 19, (short) 160, (short) 480, (short) 349, (short) 320),
      new Ac3FrameHeader.FrameCode((byte) 20, (short) 192, (short) 576, (short) 417, (short) 384),
      new Ac3FrameHeader.FrameCode((byte) 21, (short) 192, (short) 576, (short) 418, (short) 384),
      new Ac3FrameHeader.FrameCode((byte) 22, (short) 224, (short) 672, (short) 487, (short) 448),
      new Ac3FrameHeader.FrameCode((byte) 23, (short) 224, (short) 672, (short) 488, (short) 448),
      new Ac3FrameHeader.FrameCode((byte) 24, (short) 256, (short) 768, (short) 557, (short) 512),
      new Ac3FrameHeader.FrameCode((byte) 25, (short) 256, (short) 768, (short) 558, (short) 512),
      new Ac3FrameHeader.FrameCode((byte) 26, (short) 320, (short) 960, (short) 696, (short) 640),
      new Ac3FrameHeader.FrameCode((byte) 27, (short) 320, (short) 960, (short) 697, (short) 640),
      new Ac3FrameHeader.FrameCode((byte) 28, (short) 384, (short) 1152, (short) 835, (short) 768),
      new Ac3FrameHeader.FrameCode((byte) 29, (short) 384, (short) 1152, (short) 836, (short) 768),
      new Ac3FrameHeader.FrameCode((byte) 30, (short) 448, (short) 1344, (short) 975, (short) 896),
      new Ac3FrameHeader.FrameCode((byte) 31, (short) 448, (short) 1344, (short) 976, (short) 896),
      new Ac3FrameHeader.FrameCode((byte) 32, (short) 512, (short) 1536, (short) 1114, (short) 1024),
      new Ac3FrameHeader.FrameCode((byte) 33, (short) 512, (short) 1536, (short) 1115, (short) 1024),
      new Ac3FrameHeader.FrameCode((byte) 34, (short) 576, (short) 1728, (short) 1253, (short) 1152),
      new Ac3FrameHeader.FrameCode((byte) 35, (short) 576, (short) 1728, (short) 1254, (short) 1152),
      new Ac3FrameHeader.FrameCode((byte) 36, (short) 640, (short) 1920, (short) 1393, (short) 1280),
      new Ac3FrameHeader.FrameCode((byte) 37, (short) 640, (short) 1920, (short) 1394, (short) 1280)
    }, (Func<Ac3FrameHeader.FrameCode, byte>) (v => v.Code));

    public int Bitrate { get; private set; }

    public int SamplingFrequency { get; private set; }

    public int FrameLength { get; private set; }

    public int HeaderLength
    {
      get
      {
        return 5;
      }
    }

    public int HeaderOffset { get; private set; }

    public string Name { get; private set; }

    public TimeSpan Duration
    {
      get
      {
        return Ac3FrameHeader.FrameDuration;
      }
    }

    public bool Parse(byte[] buffer, int index, int length, bool verbose = false)
    {
      this.HeaderOffset = 0;
      int num1 = index;
      int num2 = index + length;
      if (length < 5)
        return false;
      do
      {
        bool flag = true;
        do
        {
          flag = true;
          if (index + 5 > num2)
            goto label_2;
        }
        while (11 != (int) buffer[index++]);
        if (index + 4 > num2)
          goto label_6;
      }
      while (119 != (int) buffer[index++]);
      goto label_9;
label_2:
      return false;
label_6:
      return false;
label_9:
      this.HeaderOffset = index - num1 - 2;
      if (this.HeaderOffset < 0)
        return false;
      int num3 = (int) buffer[index++] << 8 | (int) buffer[index++];
      byte num4 = buffer[index++];
      int num5 = (int) num4 >> 6 & 3;
      this.SamplingFrequency = this.GetSamplingFrequency(num5);
      if (this.SamplingFrequency <= 0)
        return false;
      Ac3FrameHeader.FrameCode frameCode = this.GetFrameCode((byte) ((uint) num4 & 63U));
      if (null == frameCode)
        return false;
      this.Bitrate = 1000 * (int) frameCode.Bitrate;
      this.FrameLength = frameCode.GetFrameSize(num5);
      if (string.IsNullOrEmpty(this.Name))
        this.Name = string.Format("AC-3 {0}kHz", (object) ((double) this.SamplingFrequency / 1000.0));
      if (verbose)
        Debug.WriteLine("Configuration AC-3 sampling {0}kHz bitrate {1}kHz", (object) ((double) this.SamplingFrequency / 1000.0), (object) ((double) this.Bitrate / 1000.0));
      return true;
    }

    private int GetSamplingFrequency(int samplingIndex)
    {
      if (samplingIndex < 0 || samplingIndex >= Ac3FrameHeader.SamplingFrequencyTable.Length)
        return -1;
      return Ac3FrameHeader.SamplingFrequencyTable[samplingIndex];
    }

    private Ac3FrameHeader.FrameCode GetFrameCode(byte frmsizcod)
    {
      Ac3FrameHeader.FrameCode frameCode;
      if (!Ac3FrameHeader.FrameCodes.TryGetValue(frmsizcod, out frameCode))
        return (Ac3FrameHeader.FrameCode) null;
      return frameCode;
    }

    private class FrameCode
    {
      private readonly short[] _frame;

      public byte Code { get; private set; }

      public short Bitrate { get; private set; }

      public FrameCode(byte code, short bitrate, short f32k, short f44k, short f48k)
      {
        this.Code = code;
        this.Bitrate = bitrate;
        this._frame = new short[3]
        {
          f48k,
          f44k,
          f32k
        };
      }

      public int GetFrameSize(int fscod)
      {
        if (fscod < 0 || fscod >= this._frame.Length)
          throw new ArgumentOutOfRangeException("fscod");
        return 2 * (int) this._frame[fscod];
      }
    }
  }
}
