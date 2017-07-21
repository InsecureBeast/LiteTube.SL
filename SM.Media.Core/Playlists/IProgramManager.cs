using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Playlists
{
  public interface IProgramManager : IDisposable
  {
    ICollection<Uri> Playlists { get; }

    Task<IDictionary<long, Program>> LoadAsync(CancellationToken cancellationToken);
  }
}
