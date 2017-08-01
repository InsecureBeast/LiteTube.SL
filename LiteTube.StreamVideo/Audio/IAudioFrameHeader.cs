using System;

namespace LiteTube.StreamVideo.Audio
{
  public interface IAudioFrameHeader
  {
    int SamplingFrequency { get; }

    int FrameLength { get; }

    int HeaderLength { get; }

    int HeaderOffset { get; }

    string Name { get; }

    TimeSpan Duration { get; }

    bool Parse(byte[] buffer, int index, int length, bool verbose = false);
  }
}
