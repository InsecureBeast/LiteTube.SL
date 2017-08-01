using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Segments
{
  public interface ISegmentManagerFactoryFinder : IContentServiceFactoryFinder<ISegmentManager, ISegmentManagerParameters>
  {
    void Register(ContentType contentType, ISegmentManagerFactoryInstance factory);
  }
}
