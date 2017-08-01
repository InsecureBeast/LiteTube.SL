using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Playlists;

namespace LiteTube.StreamVideo.Hls
{
    public interface IHlsPlaylistSegmentManagerPolicy
    {
        Task<ISubProgram> CreateSubProgramAsync(ICollection<Uri> source, ContentType contentType, CancellationToken cancellationToken);
    }
}
