using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.Playlists
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
