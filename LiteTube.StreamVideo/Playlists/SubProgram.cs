using System;
using System.Collections.Generic;
using LiteTube.StreamVideo.Hls;

namespace LiteTube.StreamVideo.Playlists
{
  public abstract class SubProgram : ISubProgram
  {
    private readonly IProgram _program;

    public HlsProgramManager.MediaGroup AudioGroup { get; set; }

    public IProgram Program
    {
      get
      {
        return this._program;
      }
    }

    public int? Height { get; set; }

    public int? Width { get; set; }

    public long Bandwidth { get; set; }

    public abstract IProgramStream Audio { get; }

    public abstract IProgramStream Video { get; }

    public abstract ICollection<IProgramStream> AlternateAudio { get; }

    public abstract ICollection<IProgramStream> AlternateVideo { get; }

    public TimeSpan? Duration
    {
      get
      {
        return new TimeSpan?();
      }
    }

    protected SubProgram(IProgram program)
    {
      if (null == program)
        throw new ArgumentNullException("program");
      this._program = program;
    }

    public override string ToString()
    {
      return string.Format("{0:F3} Mbit/s from {1}", new object[2]
      {
        (object) ((double) this.Bandwidth * 1E-06),
        (object) this._program.PlaylistUrl
      });
    }
  }
}
