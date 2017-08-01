using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.Configuration;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.Mmreg;

namespace LiteTube.StreamVideo.MP3
{
  public sealed class Mp3Configurator : ConfiguratorBase, IAudioConfigurator, IAudioConfigurationSource, IConfigurationSource, IFrameParser
  {
    private readonly Mp3FrameHeader _frameHeader = new Mp3FrameHeader();

    public AudioFormat Format
    {
      get
      {
        return AudioFormat.Mp3;
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

    public Mp3Configurator(IMediaStreamMetadata mediaStreamMetadata, string streamDescription = null)
      : base(ContentTypes.Mp3, mediaStreamMetadata)
    {
      this.StreamDescription = streamDescription;
    }

    public void Configure(IAudioFrameHeader frameHeader)
    {
      Mp3FrameHeader mp3FrameHeader = (Mp3FrameHeader) frameHeader;
      MpegLayer3WaveFormat layer3WaveFormat1 = new MpegLayer3WaveFormat();
      layer3WaveFormat1.nChannels = (ushort) mp3FrameHeader.Channels;
      layer3WaveFormat1.nSamplesPerSec = (uint) frameHeader.SamplingFrequency;
      layer3WaveFormat1.nAvgBytesPerSec = (uint) mp3FrameHeader.Bitrate / 8U;
      layer3WaveFormat1.nBlockSize = (ushort) frameHeader.FrameLength;
      MpegLayer3WaveFormat layer3WaveFormat2 = layer3WaveFormat1;
      this.CodecPrivateData = WaveFormatExExtensions.ToCodecPrivateData((WaveFormatEx) layer3WaveFormat2);
      this.Channels = (int) layer3WaveFormat2.nChannels;
      this.SamplingFrequency = frameHeader.SamplingFrequency;
      this.Bitrate = new int?(mp3FrameHeader.Bitrate);
      this.Name = frameHeader.Name;
      this.SetConfigured();
    }

    public bool Parse(byte[] buffer, int index, int length)
    {
      if (length < 10 || !this._frameHeader.Parse(buffer, index, length, true))
        return false;
      this.Configure((IAudioFrameHeader) this._frameHeader);
      return true;
    }
  }
}
