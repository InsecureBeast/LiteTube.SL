using SM.Media.Core.Utility;

namespace SM.Media.Core.Hls
{
  public static class HlsPlaylistSettings
  {
    private static readonly ResettableParameters<HlsPlaylistParameters> PlaylistParameters = new ResettableParameters<HlsPlaylistParameters>();

    public static HlsPlaylistParameters Parameters
    {
      get
      {
        return HlsPlaylistSettings.PlaylistParameters.Parameters;
      }
      set
      {
        HlsPlaylistSettings.PlaylistParameters.Parameters = value;
      }
    }
  }
}
