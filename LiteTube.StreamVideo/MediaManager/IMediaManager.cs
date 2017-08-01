using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.MediaParser;

namespace LiteTube.StreamVideo.MediaManager
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
