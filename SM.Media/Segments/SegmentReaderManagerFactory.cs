// Decompiled with JetBrains decompiler
// Type: SM.Media.Segments.SegmentReaderManagerFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Metadata;
using SM.Media.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Segments
{
  public class SegmentReaderManagerFactory : ISegmentReaderManagerFactory
  {
    private readonly IPlatformServices _platformServices;
    private readonly IRetryManager _retryManager;
    private readonly ISegmentManagerFactory _segmentManagerFactory;
    private readonly IWebMetadataFactory _webMetadataFactory;

    public SegmentReaderManagerFactory(ISegmentManagerFactory segmentManagerFactory, IWebMetadataFactory webMetadataFactory, IRetryManager retryManager, IPlatformServices platformServices)
    {
      if (null == segmentManagerFactory)
        throw new ArgumentNullException("segmentManagerFactory");
      if (null == webMetadataFactory)
        throw new ArgumentNullException("webMetadataFactory");
      if (null == retryManager)
        throw new ArgumentNullException("retryManager");
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      this._segmentManagerFactory = segmentManagerFactory;
      this._webMetadataFactory = webMetadataFactory;
      this._retryManager = retryManager;
      this._platformServices = platformServices;
    }

    public async Task<ISegmentReaderManager> CreateAsync(ISegmentManagerParameters parameters, ContentType contentType, CancellationToken cancellationToken)
    {
      ISegmentManager playlist;
      if ((ContentType) null == contentType)
        playlist = await this._segmentManagerFactory.CreateAsync(parameters, cancellationToken).ConfigureAwait(false);
      else
        playlist = await this._segmentManagerFactory.CreateAsync(parameters, contentType, cancellationToken).ConfigureAwait(false);
      if (null == playlist)
        throw new FileNotFoundException("Unable to create playlist");
      return (ISegmentReaderManager) new SegmentReaderManager((IEnumerable<ISegmentManager>) new ISegmentManager[1]
      {
        playlist
      }, this._webMetadataFactory, this._retryManager, this._platformServices);
    }
  }
}
