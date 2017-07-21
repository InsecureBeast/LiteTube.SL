﻿using System;
using SM.Media.Core.Content;

namespace SM.Media.Core.Metadata
{
  public interface IStreamMetadata
  {
    Uri Url { get; }

    ContentType ContentType { get; }

    int? Bitrate { get; }

    TimeSpan? Duration { get; }

    string Name { get; }

    string Description { get; }

    string Genre { get; }

    Uri Website { get; }
  }
}
