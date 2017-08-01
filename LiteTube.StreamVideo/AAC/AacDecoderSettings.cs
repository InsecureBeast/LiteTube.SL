using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.AAC
{
    public static class AacDecoderSettings
    {
        private static readonly ResettableParameters<AacDecoderParameters> AacDecoderParameters = new ResettableParameters<AacDecoderParameters>();

        public static AacDecoderParameters Parameters
        {
            get { return AacDecoderParameters.Parameters; }
            set { AacDecoderParameters.Parameters = value; }
        }
    }
}
