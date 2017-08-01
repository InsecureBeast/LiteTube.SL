using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.Playlists
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
