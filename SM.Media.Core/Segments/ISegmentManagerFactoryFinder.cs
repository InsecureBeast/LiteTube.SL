using SM.Media.Core.Content;

namespace SM.Media.Core.Segments
{
  public interface ISegmentManagerFactoryFinder : IContentServiceFactoryFinder<ISegmentManager, ISegmentManagerParameters>
  {
    void Register(ContentType contentType, ISegmentManagerFactoryInstance factory);
  }
}
