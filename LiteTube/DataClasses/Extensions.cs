namespace LiteTube.DataClasses
{
    static class ThumbnailsExtensions
    {
        public static string GetThumbnailUrl(this IThumbnailDetails details)
        {
            if (details.Default != null && !string.IsNullOrEmpty(details.Default.Url))
                return details.Default.Url;
            if (details.Medium != null && !string.IsNullOrEmpty(details.Medium.Url))
                return details.Medium.Url;
            if (details.Maxres != null && !string.IsNullOrEmpty(details.Maxres.Url))
                return details.Maxres.Url;
            if (details.High != null && !string.IsNullOrEmpty(details.High.Url))
                return details.High.Url;
            if (details.Standard != null && !string.IsNullOrEmpty(details.Standard.Url))
                return details.Standard.Url;
            return null;
        }
    }
}
