﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Playlists;
using SM.Media.Core.Segments;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Hls
{
  public class HlsPlaylistSegmentManagerFactory : ISegmentManagerFactoryInstance, IContentServiceFactoryInstance<ISegmentManager, ISegmentManagerParameters>
  {
    private static readonly ICollection<ContentType> Types = (ICollection<ContentType>) new ContentType[2]
    {
      ContentTypes.M3U8,
      ContentTypes.M3U
    };
    private readonly IHlsPlaylistSegmentManagerPolicy _hlsPlaylistSegmentManagerPolicy;
    private readonly IPlatformServices _platformServices;

    public ICollection<ContentType> KnownContentTypes
    {
      get
      {
        return HlsPlaylistSegmentManagerFactory.Types;
      }
    }

    public HlsPlaylistSegmentManagerFactory(IHlsPlaylistSegmentManagerPolicy hlsPlaylistSegmentManagerPolicy, IPlatformServices platformServices)
    {
      if (null == hlsPlaylistSegmentManagerPolicy)
        throw new ArgumentNullException("hlsPlaylistSegmentManagerPolicy");
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      this._hlsPlaylistSegmentManagerPolicy = hlsPlaylistSegmentManagerPolicy;
      this._platformServices = platformServices;
    }

    public async Task<ISegmentManager> CreateAsync(ISegmentManagerParameters parameters, ContentType contentType, CancellationToken cancellationToken)
    {
      ISubProgram subProgram = await this._hlsPlaylistSegmentManagerPolicy.CreateSubProgramAsync(parameters.Source, contentType, cancellationToken).ConfigureAwait(false);
      HlsPlaylistSegmentManager segmentManager = new HlsPlaylistSegmentManager(subProgram.Video, this._platformServices, cancellationToken);
      return (ISegmentManager) segmentManager;
    }
  }
}