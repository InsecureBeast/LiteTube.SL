using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.Playlists
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
