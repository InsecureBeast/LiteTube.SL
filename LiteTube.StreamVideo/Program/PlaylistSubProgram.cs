using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.Program
{
    public class PlaylistSubProgram : SubProgram
    {
        public PlaylistSubProgram(IProgram program, IProgramStream video) : base(program)
        {
            Video = video;
        }

        public Uri Playlist { get; set; }

        public override IProgramStream Audio
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IProgramStream Video { get; }

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

        public override string ToString()
        {
            return $"{Playlist?.ToString() ?? "<none>"} {base.ToString()}";
        }
    }
}
