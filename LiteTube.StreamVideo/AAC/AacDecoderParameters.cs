using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.AAC
{
    public class AacDecoderParameters
    {
        private bool _useParser = true;
        private Func<AacFrameHeader, ICollection<byte>> _audioSpecificConfigFactory;

        public bool UseParser
        {
            get { return _useParser || this.UseRawAac; }
            set { _useParser = value; }
        }

        public bool UseRawAac { get; set; }

        public WaveFormatEx ConfigurationFormat { get; set; }

        public Func<AacFrameHeader, ICollection<byte>> AudioSpecificConfigFactory
        {
            get
            {
                if (null == _audioSpecificConfigFactory)
                    return new Func<AacFrameHeader, ICollection<byte>>(AacAudioSpecificConfig.DefaultAudioSpecificConfigFactory);

                return _audioSpecificConfigFactory;
            }
            set
            {
                _audioSpecificConfigFactory = value;
            }
        }

        public Func<AacFrameHeader, string> CodecPrivateDataFactory { get; set; }

        public AacDecoderParameters()
        {
            ConfigurationFormat = AacDecoderParameters.WaveFormatEx.HeAac;
        }

        public enum WaveFormatEx
        {
            RawAac,
            HeAac,
        }
    }
}
