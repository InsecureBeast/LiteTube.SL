using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.Segments;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Playlists
{
  public interface IProgramStream
  {
    string StreamType { get; }

    string Language { get; }

    ICollection<Uri> Urls { get; }

    IWebReader WebReader { get; }

    bool IsDynamicPlaylist { get; }

    IStreamMetadata StreamMetadata { get; }

    ICollection<ISegment> Segments { get; }

    Task RefreshPlaylistAsync(CancellationToken cancellationToken);

    Task<ContentType> GetContentTypeAsync(CancellationToken cancellationToken);
  }
}
