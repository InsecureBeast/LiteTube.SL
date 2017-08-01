using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.Configuration;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Metadata;

namespace LiteTube.StreamVideo.Ac3
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
