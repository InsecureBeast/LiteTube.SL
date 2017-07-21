using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SM.Media.Core
{
    public sealed class PlaybackSession : PlaybackSessionBase<MediaStreamSource>
    {
        public PlaybackSession(IMediaStreamFacadeBase<MediaStreamSource> mediaStreamFacade)
          : base(mediaStreamFacade)
        {
        }
    }
}
