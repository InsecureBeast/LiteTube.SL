using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.Program
{
    public interface IProgram
    {
        Uri PlaylistUrl { get; }
        ICollection<ISubProgram> SubPrograms { get; }
    }
}
