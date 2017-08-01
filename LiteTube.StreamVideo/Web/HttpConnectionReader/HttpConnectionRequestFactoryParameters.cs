namespace LiteTube.StreamVideo.Web.HttpConnectionReader
{
  public static class HttpCOnnectionRequestFactoryParametersExtensions
  {
    public static void SetParameter(this IMediaStreamFacadeBase mediaStreamFacade, IHttpConnectionRequestFactoryParameters parameters)
    {
      mediaStreamFacade.Builder.RegisterSingleton<IHttpConnectionRequestFactoryParameters>(parameters);
    }
  }
}
