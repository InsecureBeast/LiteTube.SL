// Decompiled with JetBrains decompiler
// Type: SM.Media.Segments.SegmentManagerFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Web;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Segments
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
