// Decompiled with JetBrains decompiler
// Type: SM.Media.Mmreg.RawAacWaveInfo
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;

namespace SM.Media.Mmreg
{
  public class RawAacWaveInfo : WaveFormatEx
  {
    public ICollection<byte> pbAudioSpecificConfig;

    public override ushort cbSize
    {
      get
      {
        ushort cbSize = base.cbSize;
        if (null != this.pbAudioSpecificConfig)
          cbSize += (ushort) this.pbAudioSpecificConfig.Count;
        return cbSize;
      }
    }

    public RawAacWaveInfo()
    {
      this.wFormatTag = (ushort) byte.MaxValue;
      this.nBlockAlign = (ushort) 4;
      this.wBitsPerSample = (ushort) 16;
    }

    public override void ToBytes(IList<byte> buffer)
    {
      base.ToBytes(buffer);
      if (this.pbAudioSpecificConfig == null || this.pbAudioSpecificConfig.Count <= 0)
        return;
      foreach (byte num in (IEnumerable<byte>) this.pbAudioSpecificConfig)
        buffer.Add(num);
    }
  }
}
