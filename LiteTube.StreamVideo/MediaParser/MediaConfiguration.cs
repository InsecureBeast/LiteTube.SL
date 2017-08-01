using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.MediaParser
{
  public class MediaConfiguration : IMediaConfiguration
  {
    public IReadOnlyCollection<IMediaParserMediaStream> AlternateStreams { get; set; }

    public TimeSpan? Duration { get; set; }

    public IMediaParserMediaStream Audio { get; set; }

    public IMediaParserMediaStream Video { get; set; }
  }
}
