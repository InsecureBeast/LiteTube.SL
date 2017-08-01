using System.Collections.Generic;

namespace LiteTube.StreamVideo.H264
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
