// Decompiled with JetBrains decompiler
// Type: SM.Media.Hls.HlsPlaylistSegmentManagerFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Playlists;
using SM.Media.Segments;
using SM.Media.Utility;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Hls
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
