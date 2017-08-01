using System.Collections.Generic;

namespace LiteTube.StreamVideo.Mmreg
{
  public class HeAacWaveInfo : WaveFormatEx
  {
    public ushort wAudioProfileLevelIndication = 254;
    public ushort wPayloadType = 1;
    public uint dwReserved2;
    public ushort wReserved1;
    public ushort wStructType;
    public ICollection<byte> pbAudioSpecificConfig;

    public override ushort cbSize
    {
      get
      {
        int num = (int) base.cbSize + 12;
        if (null != this.pbAudioSpecificConfig)
          num += this.pbAudioSpecificConfig.Count;
        return (ushort) num;
      }
    }

    public HeAacWaveInfo()
    {
      this.wFormatTag = (ushort) 5648;
      this.wBitsPerSample = (ushort) 16;
      this.nBlockAlign = (ushort) 1;
    }

    public override void ToBytes(IList<byte> buffer)
    {
      base.ToBytes(buffer);
      WaveFormatExExtensions.AddLe(buffer, this.wPayloadType);
      WaveFormatExExtensions.AddLe(buffer, this.wAudioProfileLevelIndication);
      WaveFormatExExtensions.AddLe(buffer, this.wStructType);
      WaveFormatExExtensions.AddLe(buffer, this.wReserved1);
      WaveFormatExExtensions.AddLe(buffer, this.dwReserved2);
      if (this.pbAudioSpecificConfig == null || this.pbAudioSpecificConfig.Count <= 0)
        return;
      foreach (byte num in (IEnumerable<byte>) this.pbAudioSpecificConfig)
        buffer.Add(num);
    }

    public enum PayloadType : ushort
    {
      Raw,
      ADTS,
      ADIF,
      LOAS,
    }
  }
}
