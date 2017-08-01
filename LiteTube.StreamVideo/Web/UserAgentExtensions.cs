namespace LiteTube.StreamVideo.Web
{
  public static class UserAgentExtensions
  {
    public static void SetParameter(this IMediaStreamFacadeBase mediaStreamFacade, IUserAgent userAgent)
    {
      mediaStreamFacade.Builder.RegisterSingleton<IUserAgent>(userAgent);
    }
  }
}
