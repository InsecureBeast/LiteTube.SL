// Decompiled with JetBrains decompiler
// Type: SM.Media.Playlists.Program
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections.Generic;

namespace SM.Media.Playlists
{
  public class Program : IProgram
  {
    private readonly ICollection<ISubProgram> _subPrograms = (ICollection<ISubProgram>) new List<ISubProgram>();

    public long ProgramId { get; set; }

    public Uri PlaylistUrl { get; set; }

    public ICollection<ISubProgram> SubPrograms
    {
      get
      {
        return this._subPrograms;
      }
    }

    public override string ToString()
    {
      return string.Format("{0} {1} SubPrograms {2}", (object) this.ProgramId, (object) this._subPrograms.Count, (object) this.PlaylistUrl);
    }
  }
}
