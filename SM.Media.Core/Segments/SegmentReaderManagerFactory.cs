using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Metadata;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Segments
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
