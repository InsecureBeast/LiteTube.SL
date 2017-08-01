using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.Program
{
    public class Program : IProgram
    {
        public long ProgramId { get; set; }
        public Uri PlaylistUrl { get; set; }
        public ICollection<ISubProgram> SubPrograms { get; } = new List<ISubProgram>();

        public override string ToString()
        {
            return $"{ ProgramId} { SubPrograms.Count} SubPrograms { PlaylistUrl}";
        }
    }
}
