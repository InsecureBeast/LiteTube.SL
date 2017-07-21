// Decompiled with JetBrains decompiler
// Type: SM.Media.AAC.AacFrameHeader
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Audio;
using SM.Media.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SM.Media.AAC
{
  public sealed class AacFrameHeader : IAudioFrameHeader
  {
    private static readonly int[] SamplingFrequencyTable = new int[13]
    {
      96000,
      88200,
      64000,
      48000,
      44100,
      32000,
      24000,
      22050,
      16000,
      12000,
      11025,
      8000,
      7350
    };
    private static readonly Dictionary<int, string> ProfileNames = new Dictionary<int, string>()
    {
      {
        0,
        "AAC Main"
      },
      {
        1,
        "AAC LC (Low Complexity)"
      },
      {
        2,
        "AAC SSR (Scalable Sample Rate)"
      },
      {
        3,
        "AAC LTP (Long Term Prediction)"
      },
      {
        4,
        "SBR (Spectral Band Replication)"
      },
      {
        5,
        "AAC Scalable"
      },
      {
        6,
        "TwinVQ"
      },
      {
        7,
        "CELP (Code Excited Linear Prediction)"
      },
      {
        8,
        "HXVC (Harmonic Vector eXcitation Coding)"
      },
      {
        11,
        "TTSI (Text-To-Speech Interface)"
      },
      {
        12,
        "Main Synthesis"
      },
      {
        13,
        "Wavetable Synthesis"
      },
      {
        14,
        "General MIDI"
      },
      {
        15,
        "Algorithmic Synthesis and Audio Effects"
      },
      {
        16,
        "ER (Error Resilient) AAC LC"
      },
      {
        18,
        "ER AAC LTP"
      },
      {
        19,
        "ER AAC Scalable"
      },
      {
        20,
        "ER TwinVQ"
      },
      {
        21,
        "ER BSAC (Bit-Sliced Arithmetic Coding)"
      },
      {
        22,
        "ER AAC LD (Low Delay)"
      },
      {
        23,
        "ER CELP"
      },
      {
        24,
        "ER HVXC"
      },
      {
        25,
        "ER HILN (Harmonic and Individual Lines plus Noise)"
      },
      {
        26,
        "ER Parametric"
      },
      {
        27,
        "SSC (SinuSoidal Coding)"
      },
      {
        28,
        "PS (Parametric Stereo)"
      },
      {
        29,
        "MPEG Surround"
      },
      {
        31,
        "Layer-1"
      },
      {
        32,
        "Layer-2"
      },
      {
        33,
        "Layer-3 (MP3)"
      },
      {
        34,
        "DST (Direct Stream Transfer)"
      },
      {
        35,
        "ALS (Audio Lossless)"
      },
      {
        36,
        "SLS (Scalable LosslesS)"
      },
      {
        37,
        "SLS non-core"
      },
      {
        38,
        "ER AAC ELD (Enhanced Low Delay)"
      },
      {
        39,
        "SMR (Symbolic Music Representation) Simple"
      },
      {
        40,
        "SMR Main"
      },
      {
        41,
        "USAC (Unified Speech and Audio Coding) (no SBR)"
      },
      {
        42,
        "SAOC (Spatial Audio Object Coding)"
      },
      {
        43,
        "LD MPEG Surround"
      },
      {
        44,
        "USAC"
      }
    };
    private byte[] _audioSpecificConfig;
    private int _frames;

    public int Profile { get; private set; }

    public int Layer { get; private set; }

    public int FrequencyIndex { get; private set; }

    public ushort ChannelConfig { get; private set; }

    public bool CrcFlag { get; set; }

    public ICollection<byte> AudioSpecificConfig
    {
      get
      {
        if (null == this._audioSpecificConfig)
          this._audioSpecificConfig = Enumerable.ToArray<byte>((IEnumerable<byte>) AacDecoderSettings.Parameters.AudioSpecificConfigFactory(this));
        return (ICollection<byte>) this._audioSpecificConfig;
      }
      set
      {
        this._audioSpecificConfig = Enumerable.ToArray<byte>((IEnumerable<byte>) value);
      }
    }

    public int Bitrate { get; private set; }

    public int HeaderLength
    {
      get
      {
        return this.CrcFlag ? 9 : 7;
      }
    }

    public int HeaderOffset { get; private set; }

    public TimeSpan Duration
    {
      get
      {
        return FullResolutionTimeSpan.FromSeconds((double) this._frames * 1024.0 / (double) this.SamplingFrequency);
      }
    }

    public int SamplingFrequency { get; private set; }

    public int FrameLength { get; private set; }

    public string Name { get; private set; }

    public bool Parse(byte[] buffer, int index, int length, bool verbose = false)
    {
      int num1 = index;
      int num2 = index + length;
      this.HeaderOffset = 0;
      if (length < 7)
        return false;
      byte num3;
      do
      {
        bool flag = true;
        do
        {
          flag = true;
          if (index + 7 > num2)
            goto label_2;
        }
        while ((int) byte.MaxValue != (int) buffer[index++]);
        if (index + 6 <= num2)
          num3 = buffer[index++];
        else
          goto label_6;
      }
      while (15 != ((int) num3 >> 4 & 15));
      goto label_9;
label_2:
      return false;
label_6:
      return false;
label_9:
      this.HeaderOffset = index - num1 - 2;
      if (this.HeaderOffset < 0)
        return false;
      bool flag1 = 0 == ((int) num3 & 8);
      this.Layer = (int) num3 >> 1 & 3;
      if (0 != this.Layer)
        Debug.WriteLine("AacFrameHeader.Parse() unknown layer: " + (object) this.Layer);
      this.CrcFlag = 0 == ((int) num3 & 1);
      byte num4 = buffer[index++];
      this.Profile = (int) num4 >> 6 & 3;
      this.FrequencyIndex = (int) num4 >> 2 & 15;
      this.SamplingFrequency = this.GetSamplingFrequency(this.FrequencyIndex);
      if (this.SamplingFrequency <= 0)
        return false;
      int num5 = (int) num4 >> 1 & 1;
      byte num6 = buffer[index++];
      this.ChannelConfig = (ushort) (((int) num4 & 1) << 2 | (int) num6 >> 6 & 3);
      int num7 = (int) num6 >> 5 & 1;
      int num8 = (int) num6 >> 4 & 1;
      int num9 = (int) num6 >> 3 & 1;
      int num10 = (int) num6 >> 2 & 1;
      byte num11 = buffer[index++];
      byte num12 = buffer[index++];
      this.FrameLength = ((int) num6 & 3) << 11 | (int) num11 << 3 | (int) num12 >> 5 & 7;
      if (this.FrameLength < 1)
        return false;
      byte num13 = buffer[index++];
      int num14 = ((int) num12 & 31) << 6 | (int) num13 >> 2 & 63;
      this._frames = 1 + ((int) num13 & 3);
      if (this._frames < 1)
        return false;
      if (this.CrcFlag)
      {
        if (index + 2 > num2)
          return false;
        byte num15 = buffer[index++];
        byte num16 = buffer[index++];
      }
      if (string.IsNullOrEmpty(this.Name))
        this.Name = string.Format("{0}, {1}kHz {2} channels", (object) this.GetProfileName(), (object) ((double) this.SamplingFrequency / 1000.0), (object) this.ChannelConfig);
      if (verbose)
        Debug.WriteLine("Configuration AAC layer {0} profile \"{1}\" channels {2} sampling {3}kHz length {4} CRC {5}", (object) this.Layer, (object) this.Name, (object) this.ChannelConfig, (object) ((double) this.SamplingFrequency / 1000.0), (object) this.FrameLength, (object) (bool) (this.CrcFlag ? 1 : 0));
      return true;
    }

    private string GetProfileName()
    {
      string str;
      if (AacFrameHeader.ProfileNames.TryGetValue(this.Profile, out str))
        return str;
      return "Profile" + (object) this.Profile;
    }

    private int GetSamplingFrequency(int samplingIndex)
    {
      if (samplingIndex < 0 || samplingIndex >= AacFrameHeader.SamplingFrequencyTable.Length)
        return -1;
      return AacFrameHeader.SamplingFrequencyTable[samplingIndex];
    }
  }
}
