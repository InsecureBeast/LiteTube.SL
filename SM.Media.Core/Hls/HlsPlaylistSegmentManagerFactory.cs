using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Playlists;
using SM.Media.Core.Segments;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Hls
{
    public class HlsPlaylistSegmentManagerFactory : ISegmentManagerFactoryInstance, IContentServiceFactoryInstance<ISegmentManager, ISegmentManagerParameters>
    {
        private static readonly ICollection<ContentType> _types = new[]
        {
            ContentTypes.M3U8,
            ContentTypes.M3U
        };

        private readonly IHlsPlaylistSegmentManagerPolicy _hlsPlaylistSegmentManagerPolicy;
        private readonly IPlatformServices _platformServices;

        public ICollection<ContentType> KnownContentTypes => _types;

        public HlsPlaylistSegmentManagerFactory(IHlsPlaylistSegmentManagerPolicy hlsPlaylistSegmentManagerPolicy, IPlatformServices platformServices)
        {
            if (null == hlsPlaylistSegmentManagerPolicy)
                throw new ArgumentNullException(nameof(hlsPlaylistSegmentManagerPolicy));
            if (null == platformServices)
                throw new ArgumentNullException(nameof(platformServices));

            _hlsPlaylistSegmentManagerPolicy = hlsPlaylistSegmentManagerPolicy;
            _platformServices = platformServices;
        }

        public async Task<ISegmentManager> CreateAsync(ISegmentManagerParameters parameters, ContentType contentType, CancellationToken cancellationToken)
        {
            var subProgram = await _hlsPlaylistSegmentManagerPolicy.CreateSubProgramAsync(parameters.Source, contentType, cancellationToken).ConfigureAwait(false);
            var segmentManager = new HlsPlaylistSegmentManager(subProgram.Video, _platformServices, cancellationToken);
            return segmentManager;
        }
    }
}
