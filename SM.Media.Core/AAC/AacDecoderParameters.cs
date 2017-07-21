using System;
using System.Collections.Generic;

namespace SM.Media.Core.AAC
{
  public class AacDecoderParameters
  {
    private bool _useParser = true;
    private Func<AacFrameHeader, ICollection<byte>> _audioSpecificConfigFactory;

    public bool UseParser
    {
      get
      {
        return this._useParser || this.UseRawAac;
      }
      set
      {
        this._useParser = value;
      }
    }

    public bool UseRawAac { get; set; }

    public AacDecoderParameters.WaveFormatEx ConfigurationFormat { get; set; }

    public Func<AacFrameHeader, ICollection<byte>> AudioSpecificConfigFactory
    {
      get
      {
        if (null == this._audioSpecificConfigFactory)
          return new Func<AacFrameHeader, ICollection<byte>>(AacAudioSpecificConfig.DefaultAudioSpecificConfigFactory);
        return this._audioSpecificConfigFactory;
      }
      set
      {
        this._audioSpecificConfigFactory = value;
      }
    }

    public Func<AacFrameHeader, string> CodecPrivateDataFactory { get; set; }

    public AacDecoderParameters()
    {
      this.ConfigurationFormat = AacDecoderParameters.WaveFormatEx.HeAac;
    }

    public enum WaveFormatEx
    {
      RawAac,
      HeAac,
    }
  }
}
