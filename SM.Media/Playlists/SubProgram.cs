// Decompiled with JetBrains decompiler
// Type: SM.Media.Playlists.SubProgram
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Hls;
using System;
using System.Collections.Generic;

namespace SM.Media.Playlists
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
