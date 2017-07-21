using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Playlists;

namespace SM.Media.Core.Hls
{
  public interface IHlsPlaylistSegmentManagerPolicy
  {
    Task<ISubProgram> CreateSubProgramAsync(ICollection<Uri> source, ContentType contentType, CancellationToken cancellationToken);
  }
}
