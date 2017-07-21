// Decompiled with JetBrains decompiler
// Type: SM.Media.Playlists.PlaylistSubProgram
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections.Generic;

namespace SM.Media.Playlists
{
  public class PlaylistSubProgram : SubProgram
  {
    private readonly IProgramStream _video;

    public Uri Playlist { get; set; }

    public override IProgramStream Audio
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public override IProgramStream Video
    {
      get
      {
        return this._video;
      }
    }

    public override ICollection<IProgramStream> AlternateAudio
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public override ICollection<IProgramStream> AlternateVideo
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public PlaylistSubProgram(IProgram program, IProgramStream video)
      : base(program)
    {
      this._video = video;
    }

    public override string ToString()
    {
      return string.Format("{0} {1}", new object[2]
      {
        (Uri) null == this.Playlist ? (object) "<none>" : (object) this.Playlist.ToString(),
        (object) base.ToString()
      });
    }
  }
}
