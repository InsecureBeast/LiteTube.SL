using System.Collections.Generic;

namespace SM.Media.Core.Mmreg
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
