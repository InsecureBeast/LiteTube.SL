using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.MediaParser;

namespace SM.Media.Core.MediaManager
{
    public interface IMediaManager : IDisposable
    {
        TimeSpan? SeekTarget { get; set; }

        MediaManagerState State { get; }

        ContentType ContentType { get; set; }

        Task PlayingTask { get; }

        event EventHandler<MediaManagerStateEventArgs> OnStateChange;

        Task<IMediaStreamConfigurator> OpenMediaAsync(ICollection<Uri> source, CancellationToken cancellationToken);

        Task StopMediaAsync(CancellationToken cancellationToken);

        Task CloseMediaAsync();

        Task<TimeSpan> SeekMediaAsync(TimeSpan position);
    }
}
