namespace LiteTube.DataClasses
{
    /// <summary>
    /// YouTube kind resources
    /// </summary>
    public enum DataKind
    {
        None = 0,
        GuideCategory = 1,
        PlayListItem = 2
    }

    class DataClasses
    {
        public static DataKind GetDataKind(string youTubeKind)
        {
            if (youTubeKind.Equals("youtube#guideCategory"))
                return DataKind.GuideCategory;

            if (youTubeKind.Equals("youtube#playlistItem"))
                return DataKind.GuideCategory;

            return DataKind.None;
        }
    }
}
