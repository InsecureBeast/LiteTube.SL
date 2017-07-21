// Decompiled with JetBrains decompiler
// Type: SM.Media.Playlists.PlaylistDefaults
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using SM.Media.M3U8.TagSupport;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Media.Playlists
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
