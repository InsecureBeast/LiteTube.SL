// Decompiled with JetBrains decompiler
// Type: SM.Media.H264.IH264Reader
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;

namespace SM.Media.H264
{
  internal interface IH264Reader
  {
    int? FrameRateDenominator { get; }

    int? FrameRateNumerator { get; }

    string Name { get; }

    int? Width { get; }

    int? Height { get; }

    void ReadSliceHeader(H264Bitstream r, bool IdrPicFlag);

    void ReadSei(H264Bitstream r, ICollection<byte> buffer);

    bool ReaderCheckConfigure(H264Configurator h264Configurator);

    void ReadSps(H264Bitstream r);

    void ReadPps(H264Bitstream r);

    void TryReparseTimingSei(H264Configurator h264Configurator);
  }
}
