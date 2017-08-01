using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo
{
    public static class MediaStreamFacadeSettings
    {
        private static readonly ResettableParameters<MediaStreamFacadeParameters> MediaStreamFacadeParameters = new ResettableParameters<MediaStreamFacadeParameters>();

        public static MediaStreamFacadeParameters Parameters
        {
            get
            {
                return MediaStreamFacadeSettings.MediaStreamFacadeParameters.Parameters;
            }
            set
            {
                MediaStreamFacadeSettings.MediaStreamFacadeParameters.Parameters = value;
            }
        }
    }
}
