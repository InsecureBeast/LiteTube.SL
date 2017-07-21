// Decompiled with JetBrains decompiler
// Type: SM.Media.Audio.IAudioFrameHeader
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Audio
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
