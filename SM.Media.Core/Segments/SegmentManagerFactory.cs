using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Web;

namespace SM.Media.Core.Segments
{
  public class SegmentManagerFactory : ContentServiceFactory<ISegmentManager, ISegmentManagerParameters>, ISegmentManagerFactory, IContentServiceFactory<ISegmentManager, ISegmentManagerParameters>
  {
    private readonly IWebReaderManager _webReaderManager;

    public SegmentManagerFactory(ISegmentManagerFactoryFinder factoryFinder, IWebReaderManager webReaderManager)
      : base((IContentServiceFactoryFinder<ISegmentManager, ISegmentManagerParameters>) factoryFinder)
    {
      this._webReaderManager = webReaderManager;
    }

    public async Task<ISegmentManager> CreateAsync(ISegmentManagerParameters parameters, CancellationToken cancellationToken)
    {
      ISegmentManager segmentManager;
      foreach (Uri url in (IEnumerable<Uri>) parameters.Source)
      {
        ContentType contentType = await this._webReaderManager.DetectContentTypeAsync(url, ContentKind.Unknown, cancellationToken, (IWebReader) null).ConfigureAwait(false);
        if (!((ContentType) null == contentType))
        {
          segmentManager = await this.CreateAsync(parameters, contentType, cancellationToken).ConfigureAwait(false);
          goto label_10;
        }
      }
      segmentManager = (ISegmentManager) null;
label_10:
      return segmentManager;
    }
  }
}
