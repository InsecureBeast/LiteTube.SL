using System;
using System.Collections.Generic;
using LiteTube.StreamVideo.Hls;

namespace LiteTube.StreamVideo.Program
{
    public abstract class SubProgram : ISubProgram
    {
        protected SubProgram(IProgram program)
        {
            if (null == program)
                throw new ArgumentNullException(nameof(program));
            Program = program;
        }

        public HlsProgramManager.MediaGroup AudioGroup { get; set; }
        public IProgram Program { get; }
        public int? Height { get; set; }
        public int? Width { get; set; }
        public long Bandwidth { get; set; }
        public abstract IProgramStream Audio { get; }
        public abstract IProgramStream Video { get; }
        public abstract ICollection<IProgramStream> AlternateAudio { get; }
        public abstract ICollection<IProgramStream> AlternateVideo { get; }
        public TimeSpan? Duration => new TimeSpan?();

        public override string ToString()
        {
            return $"{Bandwidth*1E-06:F3} Mbit/s from {Program.PlaylistUrl}";
        }
    }
}
