using System;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.MediaManager;
using SM.Media.Core.Utility;

namespace SM.Media.Core.MediaParser
{
  public interface IMediaStreamConfigurator : IDisposable
  {
    TimeSpan? SeekTarget { get; set; }

    IMediaManager MediaManager { get; set; }

    void Initialize();

    Task<TMediaStreamSource> CreateMediaStreamSourceAsync<TMediaStreamSource>(CancellationToken cancellationToken) where TMediaStreamSource : class;

    Task PlayAsync(IMediaConfiguration configuration, CancellationToken cancellationToken);

    Task CloseAsync();

    void ReportError(string message);

    void CheckForSamples();

    void ValidateEvent(MediaStreamFsm.MediaEvent mediaEvent);
  }
}
