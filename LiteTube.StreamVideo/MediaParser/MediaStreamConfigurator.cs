using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.MediaParser
{
  public static class MediaStreamSourceExtensions
  {
    public static Task PlayAsync(this IMediaStreamConfigurator mediaStreamConfigurator, IEnumerable<IMediaParserMediaStream> mediaParserMediaStreams, TimeSpan? duration, CancellationToken cancellationToken)
    {
      return mediaStreamConfigurator.PlayAsync(MediaParserMediaStreamExtensions.CreateMediaConfiguration(mediaParserMediaStreams, duration), cancellationToken);
    }
  }
}
