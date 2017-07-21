// Decompiled with JetBrains decompiler
// Type: SM.Media.AAC.AacConfigurator
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Audio;
using SM.Media.Configuration;
using SM.Media.Content;
using SM.Media.Metadata;
using SM.Media.Mmreg;
using System;

namespace SM.Media.AAC
{
  public sealed class AacConfigurator : ConfiguratorBase, IFrameParser, IAudioConfigurator, IAudioConfigurationSource, IConfigurationSource
  {
    private readonly AacFrameHeader _frameHeader = new AacFrameHeader();

    public AudioFormat Format
    {
      get
      {
        return AacDecoderSettings.Parameters.UseRawAac ? AudioFormat.AacRaw : AudioFormat.AacAdts;
      }
    }

    public int SamplingFrequency { get; private set; }

    public int Channels { get; private set; }

    public int FrameLength
    {
      get
      {
        return this._frameHeader.FrameLength;
      }
    }

    public AacConfigurator(IMediaStreamMetadata mediaStreamMetadata, string streamDescription = null)
      : base(ContentTypes.Aac, mediaStreamMetadata)
    {
      this.StreamDescription = streamDescription;
    }

    public void Configure(IAudioFrameHeader frameHeader)
    {
      AacFrameHeader aacFrameHeader = (AacFrameHeader) frameHeader;
      this.CodecPrivateData = AacConfigurator.BuildCodecPrivateData(aacFrameHeader);
      this.Name = frameHeader.Name;
      this.Channels = (int) aacFrameHeader.ChannelConfig;
      this.SamplingFrequency = frameHeader.SamplingFrequency;
      this.SetConfigured();
    }

    public bool Parse(byte[] buffer, int index, int length)
    {
      if (!this._frameHeader.Parse(buffer, index, length, true))
        return false;
      this.Configure((IAudioFrameHeader) this._frameHeader);
      return true;
    }

    private static string BuildCodecPrivateData(AacFrameHeader aacFrameHeader)
    {
      Func<AacFrameHeader, string> privateDataFactory = AacDecoderSettings.Parameters.CodecPrivateDataFactory;
      if (null != privateDataFactory)
        return privateDataFactory(aacFrameHeader);
      AacDecoderParameters.WaveFormatEx configurationFormat = AacDecoderSettings.Parameters.ConfigurationFormat;
      SM.Media.Mmreg.WaveFormatEx waveFormatEx;
      switch (configurationFormat)
      {
        case AacDecoderParameters.WaveFormatEx.RawAac:
          if (!AacDecoderSettings.Parameters.UseRawAac)
            throw new NotSupportedException("AacDecoderSettings.Parameters.UseRawAac must be enabled when using AacDecoderParameters.WaveFormatEx.RawAac");
          RawAacWaveInfo rawAacWaveInfo1 = new RawAacWaveInfo();
          rawAacWaveInfo1.nChannels = aacFrameHeader.ChannelConfig;
          rawAacWaveInfo1.nSamplesPerSec = (uint) aacFrameHeader.SamplingFrequency;
          RawAacWaveInfo rawAacWaveInfo2 = rawAacWaveInfo1;
          TimeSpan duration = aacFrameHeader.Duration;
          double num1;
          if (duration.TotalSeconds > 0.0)
          {
            double num2 = (double) aacFrameHeader.FrameLength;
            duration = aacFrameHeader.Duration;
            double totalSeconds = duration.TotalSeconds;
            num1 = num2 / totalSeconds;
          }
          else
            num1 = 0.0;
          int num3 = (int) (uint) num1;
          rawAacWaveInfo2.nAvgBytesPerSec = (uint) num3;
          rawAacWaveInfo1.pbAudioSpecificConfig = aacFrameHeader.AudioSpecificConfig;
          waveFormatEx = (SM.Media.Mmreg.WaveFormatEx) rawAacWaveInfo1;
          break;
        case AacDecoderParameters.WaveFormatEx.HeAac:
          HeAacWaveInfo heAacWaveInfo = new HeAacWaveInfo();
          heAacWaveInfo.wPayloadType = AacDecoderSettings.Parameters.UseRawAac ? (ushort) 0 : (ushort) 1;
          heAacWaveInfo.nChannels = aacFrameHeader.ChannelConfig;
          heAacWaveInfo.nSamplesPerSec = (uint) aacFrameHeader.SamplingFrequency;
          heAacWaveInfo.pbAudioSpecificConfig = aacFrameHeader.AudioSpecificConfig;
          waveFormatEx = (SM.Media.Mmreg.WaveFormatEx) heAacWaveInfo;
          break;
        default:
          throw new NotSupportedException("Unknown WaveFormatEx type: " + (object) configurationFormat);
      }
      return WaveFormatExExtensions.ToCodecPrivateData(waveFormatEx);
    }
  }
}
