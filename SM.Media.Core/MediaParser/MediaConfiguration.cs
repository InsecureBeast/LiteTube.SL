using System;
using System.Collections.Generic;

namespace SM.Media.Core.MediaParser
{
  public class MediaConfiguration : IMediaConfiguration
  {
    public IReadOnlyCollection<IMediaParserMediaStream> AlternateStreams { get; set; }

    public TimeSpan? Duration { get; set; }

    public IMediaParserMediaStream Audio { get; set; }

    public IMediaParserMediaStream Video { get; set; }
  }
}
