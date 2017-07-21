using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.MediaManager
{
    public enum MediaManagerState
    {
        Idle,
        Opening,
        OpenMedia,
        Seeking,
        Playing,
        Closed,
        Error,
        Closing,
    }
}
