using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Program
{
    public interface IProgramManager : IDisposable
    {
        ICollection<Uri> Playlists { get; }
        Task<IDictionary<long, Program>> LoadAsync(CancellationToken cancellationToken);
    }
}
