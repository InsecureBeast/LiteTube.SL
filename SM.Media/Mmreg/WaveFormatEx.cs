// Decompiled with JetBrains decompiler
// Type: SM.Media.Mmreg.WaveFormatEx
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;

namespace SM.Media.Mmreg
{
  public class WaveFormatEx
  {
    public uint nAvgBytesPerSec;
    public ushort nBlockAlign;
    public ushort nChannels;
    public uint nSamplesPerSec;
    public ushort wBitsPerSample;
    public ushort wFormatTag;

    public virtual ushort cbSize
    {
      get
      {
        return 0;
      }
    }

    public virtual void ToBytes(IList<byte> buffer)
    {
      WaveFormatExExtensions.AddLe(buffer, this.wFormatTag);
      WaveFormatExExtensions.AddLe(buffer, this.nChannels);
      WaveFormatExExtensions.AddLe(buffer, this.nSamplesPerSec);
      WaveFormatExExtensions.AddLe(buffer, this.nAvgBytesPerSec);
      WaveFormatExExtensions.AddLe(buffer, this.nBlockAlign);
      WaveFormatExExtensions.AddLe(buffer, this.wBitsPerSample);
      WaveFormatExExtensions.AddLe(buffer, this.cbSize);
    }

    public enum WaveFormatTag : ushort
    {
      Mpeg = 80,
      MpegLayer3 = 85,
      RawAac1 = 255,
      FraunhoferIisMpeg2Aac = 384,
      AdtsAac = 5632,
      RawAac = 5633,
      HeAac = 5648,
      Mpeg4Aac = 41222,
    }
  }
}
