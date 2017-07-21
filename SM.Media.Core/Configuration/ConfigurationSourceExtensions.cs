using SM.Media.Core.Metadata;

namespace SM.Media.Core.Configuration
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
