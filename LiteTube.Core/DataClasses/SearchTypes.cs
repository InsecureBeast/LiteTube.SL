namespace LiteTube.Core.DataClasses
{
    public enum SearchType
    {
        Video = 0,
        Channel = 1,
        Playlist = 2
    }

    static class SearchTypeExt
    {
        public static string ToTypeString(this SearchType type)
        {
            switch (type)
            {
                case SearchType.Video:
                    return "video";
                case SearchType.Channel:
                    return "channel";
                case SearchType.Playlist:
                    return "playlist";
                default:
                    return "video";
            }
        }
    }
}
