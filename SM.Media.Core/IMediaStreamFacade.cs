using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using SM.Media.Core.Builder;
using SM.Media.Core.Content;
using SM.Media.Core.MediaManager;

namespace SM.Media.Core
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
