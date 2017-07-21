using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Metadata;
using SM.Media.Core.Segments;
using SM.Media.Core.Web;

namespace SM.Media.Core.Playlists
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
