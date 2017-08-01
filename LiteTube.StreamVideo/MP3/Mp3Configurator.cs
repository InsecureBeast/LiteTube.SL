using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.Configuration;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.Mmreg;

namespace LiteTube.StreamVideo.MP3
{
    public sealed class Mp3Configurator : ConfiguratorBase, IAudioConfigurator, IFrameParser
    {
        private readonly Mp3FrameHeader _frameHeader = new Mp3FrameHeader();

        public Mp3Configurator(IMediaStreamMetadata mediaStreamMetadata, string streamDescription = null) : base(ContentTypes.Mp3, mediaStreamMetadata)
        {
            StreamDescription = streamDescription;
        }

        public AudioFormat Format => AudioFormat.Mp3;

        public int SamplingFrequency { get; private set; }

        public int Channels { get; private set; }

        public int FrameLength => _frameHeader.FrameLength;

        public void Configure(IAudioFrameHeader frameHeader)
        {
            var mp3FrameHeader = (Mp3FrameHeader)frameHeader;
            var layer3WaveFormat1 = new MpegLayer3WaveFormat
            {
                nChannels = (ushort) mp3FrameHeader.Channels,
                nSamplesPerSec = (uint) frameHeader.SamplingFrequency,
                nAvgBytesPerSec = (uint) mp3FrameHeader.Bitrate/8U,
                nBlockSize = (ushort) frameHeader.FrameLength
            };

            var layer3WaveFormat2 = layer3WaveFormat1;
            CodecPrivateData = layer3WaveFormat2.ToCodecPrivateData();
            Channels = layer3WaveFormat2.nChannels;
            SamplingFrequency = frameHeader.SamplingFrequency;
            Bitrate = mp3FrameHeader.Bitrate;
            Name = frameHeader.Name;
            SetConfigured();
        }

        public bool Parse(byte[] buffer, int index, int length)
        {
            if (length < 10 || !_frameHeader.Parse(buffer, index, length, true))
                return false;

            Configure(_frameHeader);
            return true;
        }
    }
}
