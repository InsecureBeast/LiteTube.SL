using System.Windows.Media;
using LiteTube.StreamVideo.Builder;
using LiteTube.StreamVideo.MediaManager;

namespace LiteTube.StreamVideo
{
    public class MediaStreamFacade : MediaStreamFacadeBase<MediaStreamSource>, IMediaStreamFacade
    {
        public MediaStreamFacade(VideoQuality quality, IBuilder<IMediaManager> builder = null) 
            : base(builder ?? new TsMediaManagerBuilder(MediaStreamFacadeSettings.Parameters.UseHttpConnection, MediaStreamFacadeSettings.Parameters.UseSingleStreamMediaManager, quality))
        {
        }
    }
}
