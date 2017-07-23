using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SM.Media.Core.Builder;
using SM.Media.Core.MediaManager;

namespace SM.Media.Core
{
    public static class DefaultMediaStreamFacadeParameters
    {
        public static Func<IMediaStreamFacadeBase> Factory = () => new MediaStreamFacade(Quality, null);
        private static VideoQuality Quality = VideoQuality.Quality360P;

        public static IMediaStreamFacade Create(this MediaStreamFacadeParameters parameters, VideoQuality quality)
        {
            Quality = quality;
            return (IMediaStreamFacade)(parameters.Factory ?? Factory)();
        }
    }
}
