// Decompiled with JetBrains decompiler
// Type: SM.Media.Playlists.ISubProgram
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections.Generic;

namespace SM.Media.Playlists
{
  public interface ISubProgram
  {
    IProgram Program { get; }

    int? Height { get; }

    int? Width { get; }

    TimeSpan? Duration { get; }

    long Bandwidth { get; }

    IProgramStream Audio { get; }

    IProgramStream Video { get; }

    ICollection<IProgramStream> AlternateAudio { get; }

    ICollection<IProgramStream> AlternateVideo { get; }
  }
}
