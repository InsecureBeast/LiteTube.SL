using System;

namespace SM.Media.Core.Metadata
{
  public interface ITrackMetadata
  {
    TimeSpan? TimeStamp { get; }

    string Title { get; }

    string Album { get; }

    string Artist { get; }

    int? Year { get; }

    string Genre { get; }

    Uri Website { get; }
  }
}
