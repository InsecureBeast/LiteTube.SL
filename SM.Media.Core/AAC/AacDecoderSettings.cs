using SM.Media.Core.Utility;

namespace SM.Media.Core.AAC
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
