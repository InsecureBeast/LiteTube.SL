using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.Playlists
{
  public interface IProgram
  {
    Uri PlaylistUrl { get; }

    ICollection<ISubProgram> SubPrograms { get; }
  }
}
