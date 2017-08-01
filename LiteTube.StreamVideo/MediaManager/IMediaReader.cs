using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.MediaParser;

namespace LiteTube.StreamVideo.MediaManager
{
  internal interface IMediaReader : IDisposable
  {
    bool IsConfigured { get; }

    bool IsEnabled { get; set; }

    ICollection<IMediaParserMediaStream> MediaStreams { get; }

    Task<long> ReadAsync(CancellationToken cancellationToken);

    Task CloseAsync();

    Task StopAsync();

    bool IsBuffered(TimeSpan position);
  }
}
