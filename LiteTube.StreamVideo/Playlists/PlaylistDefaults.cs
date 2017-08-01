using System;
using System.Collections.Generic;
using System.Linq;
using LiteTube.StreamVideo.M3U8;
using LiteTube.StreamVideo.M3U8.TagSupport;

namespace LiteTube.StreamVideo.Playlists
{
  public static class PlaylistDefaults
  {
    public static bool IsDynamicPlayist(M3U8Parser parser)
    {
      if (null != M3U8TagInstanceExtensions.Tag(parser.GlobalTags, M3U8Tags.ExtXEndList))
        return false;
      return Enumerable.All<M3U8Parser.M3U8Uri>(parser.Playlist, (Func<M3U8Parser.M3U8Uri, bool>) (p =>
      {
        ExtinfTagInstance extinfTagInstance = M3U8Tags.ExtXInf.Find((IEnumerable<M3U8TagInstance>) p.Tags);
        return extinfTagInstance != null && extinfTagInstance.Duration >= new Decimal(0);
      }));
    }
  }
}
