using System;

namespace LiteTube.StreamVideo
{
    public class MediaStreamFacadeParameters
    {
        public static TimeSpan DefaultStartTimeout = TimeSpan.FromSeconds(10.0);

        public Func<IMediaStreamFacadeBase> Factory { get; set; }

        public bool UseHttpConnection { get; set; }

        public bool UseSingleStreamMediaManager { get; set; }

        public TimeSpan CreateTimeout { get; set; }

        public MediaStreamFacadeParameters()
        {
            CreateTimeout = DefaultStartTimeout;
        }
    }
}
