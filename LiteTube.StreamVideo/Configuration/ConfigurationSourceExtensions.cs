using LiteTube.StreamVideo.Metadata;

namespace LiteTube.StreamVideo.Configuration
{
    public static class ConfigurationSourceExtensions
    {
        public static string GetLanguage(this IConfigurationSource configurationSource)
        {
            IMediaStreamMetadata mediaStreamMetadata = configurationSource.MediaStreamMetadata;
            if (null == mediaStreamMetadata || string.IsNullOrWhiteSpace(mediaStreamMetadata.Language))
                return (string)null;
            return mediaStreamMetadata.Language;
        }
    }
}
