using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core
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
            this.CreateTimeout = MediaStreamFacadeParameters.DefaultStartTimeout;
        }
    }
}
