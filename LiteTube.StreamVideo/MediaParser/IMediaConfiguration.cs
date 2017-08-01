using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.MediaParser
{
  public interface IMediaConfiguration
  {
    TimeSpan? Duration { get; }

    IMediaParserMediaStream Audio { get; }

    IMediaParserMediaStream Video { get; }

    IReadOnlyCollection<IMediaParserMediaStream> AlternateStreams { get; }
  }
}
