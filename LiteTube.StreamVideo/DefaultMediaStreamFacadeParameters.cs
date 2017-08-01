using System;

namespace LiteTube.StreamVideo
{
    public static class DefaultMediaStreamFacadeParameters
    {
        public static Func<IMediaStreamFacadeBase> Factory = () => new MediaStreamFacade(_quality);
        private static VideoQuality _quality = VideoQuality.Quality360P;

        public static IMediaStreamFacade Create(this MediaStreamFacadeParameters parameters, VideoQuality quality)
        {
            _quality = quality;
            return (IMediaStreamFacade)(parameters.Factory ?? Factory)();
        }
    }
}
