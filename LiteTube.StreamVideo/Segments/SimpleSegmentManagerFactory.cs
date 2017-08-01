using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Utility;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Segments
{
  public class SimpleSegmentManagerFactory : ISegmentManagerFactoryInstance, IContentServiceFactoryInstance<ISegmentManager, ISegmentManagerParameters>
  {
    private static readonly ICollection<ContentType> Types = (ICollection<ContentType>) new ContentType[4]
    {
      ContentTypes.Aac,
      ContentTypes.Ac3,
      ContentTypes.Mp3,
      ContentTypes.TransportStream
    };
    private readonly IWebReaderManager _webReaderManager;

    public ICollection<ContentType> KnownContentTypes
    {
      get
      {
        return SimpleSegmentManagerFactory.Types;
      }
    }

    public SimpleSegmentManagerFactory(IWebReaderManager webReaderManager)
    {
      if (null == webReaderManager)
        throw new ArgumentNullException("webReaderManager");
      this._webReaderManager = webReaderManager;
    }

    public Task<ISegmentManager> CreateAsync(ISegmentManagerParameters parameters, ContentType contentType, CancellationToken cancellationToken)
    {
      return TaskEx.FromResult<ISegmentManager>((ISegmentManager) new SimpleSegmentManager(parameters.WebReader ?? WebReaderManagerExtensions.CreateRootReader(this._webReaderManager, ContentKind.AnyMedia, contentType), (IEnumerable<Uri>) parameters.Source, contentType));
    }
  }
}
