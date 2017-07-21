// Decompiled with JetBrains decompiler
// Type: SM.Media.Segments.SimpleSegmentManagerFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using SM.Media.Content;
using SM.Media.Web;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Segments
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
