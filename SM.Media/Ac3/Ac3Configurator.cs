// Decompiled with JetBrains decompiler
// Type: SM.Media.Ac3.Ac3Configurator
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Audio;
using SM.Media.Configuration;
using SM.Media.Content;
using SM.Media.Metadata;

namespace SM.Media.Ac3
{
  public sealed class Ac3Configurator : ConfiguratorBase, IAudioConfigurator, IAudioConfigurationSource, IConfigurationSource, IFrameParser
  {
    private readonly Ac3FrameHeader _frameHeader = new Ac3FrameHeader();

    public AudioFormat Format
    {
      get
      {
        return AudioFormat.Ac3;
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

    public Ac3Configurator(IMediaStreamMetadata mediaStreamMetadata, string streamDescription = null)
      : base(ContentTypes.Ac3, mediaStreamMetadata)
    {
      this.StreamDescription = streamDescription;
    }

    public void Configure(IAudioFrameHeader frameHeader)
    {
      Ac3FrameHeader ac3FrameHeader = (Ac3FrameHeader) frameHeader;
      this.Name = frameHeader.Name;
      this.Bitrate = new int?(ac3FrameHeader.Bitrate);
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
  }
}
