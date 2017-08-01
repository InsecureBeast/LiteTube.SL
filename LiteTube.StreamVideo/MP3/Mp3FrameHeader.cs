using System;
using System.Diagnostics;
using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.MP3
{
  internal sealed class Mp3FrameHeader : IAudioFrameHeader
  {
    private static readonly short[] V1L1 = new short[16]
    {
      (short) 0,
      (short) 32,
      (short) 64,
      (short) 96,
      (short) 128,
      (short) 160,
      (short) 192,
      (short) 224,
      (short) 256,
      (short) 288,
      (short) 320,
      (short) 352,
      (short) 384,
      (short) 416,
      (short) 448,
      (short) -1
    };
    private static readonly short[] V1L2 = new short[16]
    {
      (short) 0,
      (short) 32,
      (short) 48,
      (short) 56,
      (short) 64,
      (short) 80,
      (short) 96,
      (short) 112,
      (short) 128,
      (short) 160,
      (short) 192,
      (short) 224,
      (short) 256,
      (short) 320,
      (short) 384,
      (short) -1
    };
    private static readonly short[] V1L3 = new short[16]
    {
      (short) 0,
      (short) 32,
      (short) 40,
      (short) 48,
      (short) 56,
      (short) 64,
      (short) 80,
      (short) 96,
      (short) 112,
      (short) 128,
      (short) 160,
      (short) 192,
      (short) 224,
      (short) 256,
      (short) 320,
      (short) -1
    };
    private static readonly short[] V2L1 = new short[16]
    {
      (short) 0,
      (short) 32,
      (short) 48,
      (short) 56,
      (short) 64,
      (short) 80,
      (short) 96,
      (short) 112,
      (short) 128,
      (short) 144,
      (short) 160,
      (short) 176,
      (short) 192,
      (short) 224,
      (short) 256,
      (short) -1
    };
    private static readonly short[] V2L23 = new short[16]
    {
      (short) 0,
      (short) 8,
      (short) 16,
      (short) 24,
      (short) 32,
      (short) 40,
      (short) 48,
      (short) 56,
      (short) 64,
      (short) 80,
      (short) 96,
      (short) 112,
      (short) 128,
      (short) 144,
      (short) 160,
      (short) -1
    };
    private static readonly short[] SamplesV1 = new short[4]
    {
      (short) -1,
      (short) 384,
      (short) 1152,
      (short) 1152
    };
    private static readonly short[] SamplesV2 = new short[4]
    {
      (short) -1,
      (short) 384,
      (short) 1152,
      (short) 576
    };
    private static readonly int[] Rates = new int[3]
    {
      11025,
      12000,
      8000
    };
    private static readonly int[] VersionRateMultiplier = new int[4]
    {
      0,
      4,
      2,
      1
    };
    private static readonly string[] VersionName = new string[4]
    {
      "MPEG Version 2.5",
      "Reserved01",
      "MPEG Version 2 (ISO/IEC 13818-3)",
      "MPEG Version 1 (ISO/IEC 11172-3)"
    };
    private static readonly string[] LayerName = new string[4]
    {
      "Reserved00",
      "Layer III",
      "Layer II",
      "Layer I"
    };

    public int ChannelMode { get; private set; }

    public int Bitrate { get; private set; }

    public int Channels { get; private set; }

    public int? MarkerIndex { get; private set; }

    public int? EndIndex
    {
      get
      {
        if (!this.MarkerIndex.HasValue)
          return new int?();
        return new int?(this.MarkerIndex.Value + this.FrameLength);
      }
    }

    public int FrameLength { get; private set; }

    public int HeaderLength { get; private set; }

    public int HeaderOffset { get; private set; }

    public int SamplingFrequency { get; private set; }

    public TimeSpan Duration { get; private set; }

    public string Name { get; private set; }

    public bool Parse(byte[] buffer, int index, int length, bool verbose = false)
    {
      this.MarkerIndex = new int?();
      this.HeaderOffset = 0;
      if (length < 4)
        return false;
      int num1 = index;
      int num2 = index + length;
      int num3;
      byte num4;
      do
      {
        bool flag = true;
        do
        {
          flag = true;
          if (index + 4 <= num2)
            num3 = index;
          else
            goto label_3;
        }
        while ((int) byte.MaxValue != (int) buffer[index++]);
        if (index + 3 <= num2)
          num4 = buffer[index++];
        else
          goto label_7;
      }
      while (7 != ((int) num4 >> 5 & 7));
      goto label_10;
label_3:
      return false;
label_7:
      return false;
label_10:
      this.HeaderOffset = index - num1 - 2;
      if (this.HeaderOffset < 0)
        return false;
      this.MarkerIndex = new int?(num3);
      int index1 = (int) num4 >> 3 & 3;
      if (1 == index1)
        return false;
      int version = 1;
      if (0 == (index1 & 1))
        version = (index1 & 2) == 0 ? 3 : 2;
      int index2 = (int) num4 >> 1 & 3;
      if (index2 < 1)
        return false;
      int layer = 4 - index2;
      this.HeaderLength = 0 == ((int) num4 & 1) ? 6 : 4;
      byte num5 = buffer[index++];
      int bitrateIndex = (int) num5 >> 4 & 15;
      int bitrate = Mp3FrameHeader.GetBitrate(version, layer, bitrateIndex);
      if (bitrate < 1)
        return false;
      int sampleIndex = (int) num5 >> 2 & 3;
      int sampleRate = Mp3FrameHeader.GetSampleRate(version, sampleIndex);
      if (sampleRate <= 0)
        return false;
      bool paddingFlag = 0 != ((int) num5 >> 1 & 1);
      bool flag1 = 0 != ((int) num5 & 1);
      byte num6 = buffer[index++];
      int num7 = (int) num6 >> 6 & 3;
      int num8 = (int) num6 >> 4 & 3;
      int num9 = (int) num6 >> 3 & 1;
      int num10 = (int) num6 >> 2 & 1;
      int num11 = (int) num6 & 3;
      if (this.ChannelMode != num7)
      {
        this.ChannelMode = num7;
        this.Name = (string) null;
      }
      int num12 = num7 == 3 ? 1 : 2;
      if (this.Channels != num12)
      {
        this.Channels = num12;
        this.Name = (string) null;
      }
      if (this.Bitrate != bitrate)
      {
        this.Bitrate = bitrate;
        this.Name = (string) null;
      }
      if (this.SamplingFrequency != sampleRate)
      {
        this.SamplingFrequency = sampleRate;
        this.Name = (string) null;
      }
      this.FrameLength = Mp3FrameHeader.GetFrameSize(version, layer, bitrate, sampleRate, paddingFlag);
      this.Duration = this.GetDuration(version, layer, sampleRate);
      if (string.IsNullOrEmpty(this.Name))
        this.Name = string.Format("MP3 {0}, {1} sample {2}kHz bitrate {3}kHz {4} channels", (object) Mp3FrameHeader.VersionName[index1], (object) Mp3FrameHeader.LayerName[index2], (object) ((double) sampleRate / 1000.0), (object) ((double) bitrate / 1000.0), (object) this.Channels);
      if (verbose)
        Debug.WriteLine("Configuration MP3 Frame: {0}, {1} sample {2}kHz bitrate {3}kHz channel mode {4}", (object) Mp3FrameHeader.VersionName[index1], (object) Mp3FrameHeader.LayerName[index2], (object) ((double) sampleRate / 1000.0), (object) ((double) bitrate / 1000.0), (object) num7);
      return true;
    }

    private static int GetBitrate(int version, int layer, int bitrateIndex)
    {
      short[] numArray = (short[]) null;
      if (version > 1)
      {
        numArray = layer == 1 ? Mp3FrameHeader.V2L1 : Mp3FrameHeader.V2L23;
      }
      else
      {
        switch (layer)
        {
          case 1:
            numArray = Mp3FrameHeader.V1L1;
            break;
          case 2:
            numArray = Mp3FrameHeader.V1L2;
            break;
          case 3:
            numArray = Mp3FrameHeader.V1L3;
            break;
        }
      }
      if (null == numArray)
        return -1;
      return (int) numArray[bitrateIndex] * 1000;
    }

    private static int GetSampleRate(int version, int sampleIndex)
    {
      if (sampleIndex >= Mp3FrameHeader.Rates.Length)
        return -1;
      return Mp3FrameHeader.VersionRateMultiplier[version] * Mp3FrameHeader.Rates[sampleIndex];
    }

    private static int GetFrameSize(int version, int layer, int bitrate, int sampleRate, bool paddingFlag)
    {
      int num1;
      switch (layer)
      {
        case 1:
          num1 = 12;
          break;
        case 3:
          num1 = 1 == version ? 144 : 72;
          break;
        default:
          num1 = 144;
          break;
      }
      int num2 = 1 == layer ? 4 : 1;
      int num3 = paddingFlag ? num2 : 0;
      return (num1 * bitrate / sampleRate + num3) * num2;
    }

    private TimeSpan GetDuration(int version, int layer, int sampleRate)
    {
      return FullResolutionTimeSpan.FromSeconds((1 == version ? (double) Mp3FrameHeader.SamplesV1[layer] : (double) Mp3FrameHeader.SamplesV2[layer]) / (double) sampleRate);
    }
  }
}
