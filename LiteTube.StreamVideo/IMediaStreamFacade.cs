using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using LiteTube.StreamVideo.Builder;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.MediaManager;

namespace LiteTube.StreamVideo
{
    public interface IMediaStreamFacade : IMediaStreamFacadeBase<MediaStreamSource>, IMediaStreamFacadeBase, IDisposable
    {
    }

    public interface IMediaStreamFacadeBase<TMediaStreamSource> : IMediaStreamFacadeBase, IDisposable where TMediaStreamSource : class
    {
        Task<TMediaStreamSource> CreateMediaStreamSourceAsync(Uri source, CancellationToken cancellationToken);
    }

    public interface IMediaStreamFacadeBase : IDisposable
    {
        ContentType ContentType { get; set; }

        TimeSpan? SeekTarget { get; set; }

        MediaManagerState State { get; }

        IBuilder<IMediaManager> Builder { get; }

        bool IsDisposed { get; }

        Task PlayingTask { get; }

        event EventHandler<MediaManagerStateEventArgs> StateChange;

        Task StopAsync(CancellationToken cancellationToken);

        Task CloseAsync();
    }
}
