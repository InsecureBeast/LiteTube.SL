using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Platform
{
    public interface IMediaStreamControl
    {
        Task<IMediaStreamConfiguration> OpenAsync(CancellationToken cancellationToken);

        Task<TimeSpan> SeekAsync(TimeSpan position, CancellationToken cancellationToken);

        Task CloseAsync(CancellationToken cancellationToken);
    }
}
