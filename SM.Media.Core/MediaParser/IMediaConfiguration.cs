using System;
using System.Collections.Generic;

namespace SM.Media.Core.MediaParser
{
  public interface IMediaConfiguration
  {
    TimeSpan? Duration { get; }

    IMediaParserMediaStream Audio { get; }

    IMediaParserMediaStream Video { get; }

    IReadOnlyCollection<IMediaParserMediaStream> AlternateStreams { get; }
  }
}
