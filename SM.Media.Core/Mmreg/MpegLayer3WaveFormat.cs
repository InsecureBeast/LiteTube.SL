using System.Collections.Generic;

namespace SM.Media.Core.Mmreg
{
  internal class MpegLayer3WaveFormat : WaveFormatEx
  {
    public ushort nFramesPerBlock = 1;
    public ushort wID = 1;
    private const int MpegLayer3WfxExtraBytes = 12;
    public uint fdwFlags;
    public ushort nBlockSize;
    public ushort nCodecDelay;

    public override ushort cbSize
    {
      get
      {
        return (ushort) ((uint) base.cbSize + 12U);
      }
    }

    public MpegLayer3WaveFormat()
    {
      this.wFormatTag = (ushort) 85;
    }

    public override void ToBytes(IList<byte> buffer)
    {
      base.ToBytes(buffer);
      WaveFormatExExtensions.AddLe(buffer, this.wID);
      WaveFormatExExtensions.AddLe(buffer, this.fdwFlags);
      WaveFormatExExtensions.AddLe(buffer, this.nBlockSize);
      WaveFormatExExtensions.AddLe(buffer, this.nFramesPerBlock);
      WaveFormatExExtensions.AddLe(buffer, this.nCodecDelay);
    }

    [System.Flags]
    public enum Flags : uint
    {
      PaddingIso = 0,
      PaddingOn = 1,
      PaddingOff = 2,
    }

    public enum Id : ushort
    {
      Unkown,
      Mpeg,
      ConstantFrameSize,
    }
  }
}
