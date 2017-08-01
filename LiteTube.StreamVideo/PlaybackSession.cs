using System.Windows.Media;

namespace LiteTube.StreamVideo
{
    public sealed class PlaybackSession : PlaybackSessionBase<MediaStreamSource>
    {
        public PlaybackSession(IMediaStreamFacadeBase<MediaStreamSource> mediaStreamFacade)
          : base(mediaStreamFacade)
        {
        }
    }
}
