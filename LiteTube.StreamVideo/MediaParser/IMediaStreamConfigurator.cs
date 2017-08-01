using System;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.MediaManager;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.MediaParser
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
