namespace LiteTube.StreamVideo.Metadata
{
  public static class MetadataSinkExtensions
  {
    public static void SetParameter(this IMediaStreamFacadeBase mediaStreamFacade, IMetadataSink metadataSink)
    {
      mediaStreamFacade.Builder.RegisterSingleton<IMetadataSink>(metadataSink);
    }
  }
}
