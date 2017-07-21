// Decompiled with JetBrains decompiler
// Type: SM.Media.AAC.AacDecoderParameters
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections.Generic;

namespace SM.Media.AAC
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
