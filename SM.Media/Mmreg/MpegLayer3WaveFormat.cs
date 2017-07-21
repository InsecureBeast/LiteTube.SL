// Decompiled with JetBrains decompiler
// Type: SM.Media.Mmreg.MpegLayer3WaveFormat
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;

namespace SM.Media.Mmreg
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
