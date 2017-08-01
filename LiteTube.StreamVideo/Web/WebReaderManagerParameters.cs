namespace LiteTube.StreamVideo.Web
{
  public static class WebReaderManagerParametersExtensions
  {
    public static void SetParameter(this IMediaStreamFacadeBase mediaStreamFacade, IWebReaderManagerParameters parameters)
    {
      mediaStreamFacade.Builder.RegisterSingleton<IWebReaderManagerParameters>(parameters);
    }
  }
}
