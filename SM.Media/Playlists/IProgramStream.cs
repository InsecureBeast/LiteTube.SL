// Decompiled with JetBrains decompiler
// Type: SM.Media.Playlists.IProgramStream
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Metadata;
using SM.Media.Segments;
using SM.Media.Web;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Playlists
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
