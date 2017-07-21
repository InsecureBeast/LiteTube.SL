using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Platform
{
    public interface IMediaStreamControl
    {
        Task<IMediaStreamConfiguration> OpenAsync(CancellationToken cancellationToken);

        Task<TimeSpan> SeekAsync(TimeSpan position, CancellationToken cancellationToken);

        Task CloseAsync(CancellationToken cancellationToken);
    }
}
