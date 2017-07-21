using System;
using System.Collections.Generic;

namespace SM.Media.Core.Playlists
{
  public interface IProgram
  {
    Uri PlaylistUrl { get; }

    ICollection<ISubProgram> SubPrograms { get; }
  }
}
