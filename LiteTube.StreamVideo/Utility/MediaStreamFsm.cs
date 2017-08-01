using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace LiteTube.StreamVideo.Utility
{
  public struct MediaStreamFsm
  {
    private static readonly Dictionary<MediaStreamFsm.MediaState, Dictionary<MediaStreamFsm.MediaEvent, MediaStreamFsm.MediaState>> ValidTransitions = new Dictionary<MediaStreamFsm.MediaState, Dictionary<MediaStreamFsm.MediaEvent, MediaStreamFsm.MediaState>>()
    {
      {
        MediaStreamFsm.MediaState.Idle,
        new Dictionary<MediaStreamFsm.MediaEvent, MediaStreamFsm.MediaState>()
        {
          {
            MediaStreamFsm.MediaEvent.MediaStreamSourceAssigned,
            MediaStreamFsm.MediaState.Assigned
          },
          {
            MediaStreamFsm.MediaEvent.OpenMediaAsyncCalled,
            MediaStreamFsm.MediaState.Opening
          },
          {
            MediaStreamFsm.MediaEvent.DisposeCalled,
            MediaStreamFsm.MediaState.Idle
          }
        }
      },
      {
        MediaStreamFsm.MediaState.Assigned,
        new Dictionary<MediaStreamFsm.MediaEvent, MediaStreamFsm.MediaState>()
        {
          {
            MediaStreamFsm.MediaEvent.OpenMediaAsyncCalled,
            MediaStreamFsm.MediaState.Opening
          }
        }
      },
      {
        MediaStreamFsm.MediaState.Opening,
        new Dictionary<MediaStreamFsm.MediaEvent, MediaStreamFsm.MediaState>()
        {
          {
            MediaStreamFsm.MediaEvent.CallingReportOpenMediaCompleted,
            MediaStreamFsm.MediaState.AwaitSeek
          },
          {
            MediaStreamFsm.MediaEvent.CallingReportOpenMediaCompletedLive,
            MediaStreamFsm.MediaState.Playing
          },
          {
            MediaStreamFsm.MediaEvent.CloseMediaCalled,
            MediaStreamFsm.MediaState.Closing
          }
        }
      },
      {
        MediaStreamFsm.MediaState.AwaitSeek,
        new Dictionary<MediaStreamFsm.MediaEvent, MediaStreamFsm.MediaState>()
        {
          {
            MediaStreamFsm.MediaEvent.SeekAsyncCalled,
            MediaStreamFsm.MediaState.Seeking
          }
        }
      },
      {
        MediaStreamFsm.MediaState.Seeking,
        new Dictionary<MediaStreamFsm.MediaEvent, MediaStreamFsm.MediaState>()
        {
          {
            MediaStreamFsm.MediaEvent.CallingReportSeekCompleted,
            MediaStreamFsm.MediaState.Playing
          },
          {
            MediaStreamFsm.MediaEvent.GetSampleAsyncCalled,
            MediaStreamFsm.MediaState.Seeking
          },
          {
            MediaStreamFsm.MediaEvent.CallingReportSampleCompleted,
            MediaStreamFsm.MediaState.Seeking
          },
          {
            MediaStreamFsm.MediaEvent.CloseMediaCalled,
            MediaStreamFsm.MediaState.Closing
          }
        }
      },
      {
        MediaStreamFsm.MediaState.Playing,
        new Dictionary<MediaStreamFsm.MediaEvent, MediaStreamFsm.MediaState>()
        {
          {
            MediaStreamFsm.MediaEvent.GetSampleAsyncCalled,
            MediaStreamFsm.MediaState.Playing
          },
          {
            MediaStreamFsm.MediaEvent.CloseMediaCalled,
            MediaStreamFsm.MediaState.Draining
          },
          {
            MediaStreamFsm.MediaEvent.CallingReportSampleCompleted,
            MediaStreamFsm.MediaState.Playing
          },
          {
            MediaStreamFsm.MediaEvent.SeekAsyncCalled,
            MediaStreamFsm.MediaState.Seeking
          }
        }
      },
      {
        MediaStreamFsm.MediaState.Draining,
        new Dictionary<MediaStreamFsm.MediaEvent, MediaStreamFsm.MediaState>()
        {
          {
            MediaStreamFsm.MediaEvent.StreamsClosed,
            MediaStreamFsm.MediaState.Closing
          },
          {
            MediaStreamFsm.MediaEvent.GetSampleAsyncCalled,
            MediaStreamFsm.MediaState.Draining
          },
          {
            MediaStreamFsm.MediaEvent.CallingReportSampleCompleted,
            MediaStreamFsm.MediaState.Draining
          },
          {
            MediaStreamFsm.MediaEvent.SeekAsyncCalled,
            MediaStreamFsm.MediaState.Draining
          },
          {
            MediaStreamFsm.MediaEvent.CallingReportSeekCompleted,
            MediaStreamFsm.MediaState.Draining
          }
        }
      },
      {
        MediaStreamFsm.MediaState.Closing,
        new Dictionary<MediaStreamFsm.MediaEvent, MediaStreamFsm.MediaState>()
        {
          {
            MediaStreamFsm.MediaEvent.DisposeCalled,
            MediaStreamFsm.MediaState.Idle
          }
        }
      }
    };
    private static bool NoisyLogging = false;
    private volatile int _mediaState;

    private MediaStreamFsm.MediaState? Find(MediaStreamFsm.MediaState source, MediaStreamFsm.MediaEvent mediaEvent)
    {
      Dictionary<MediaStreamFsm.MediaEvent, MediaStreamFsm.MediaState> dictionary;
      if (!MediaStreamFsm.ValidTransitions.TryGetValue(source, out dictionary))
        return new MediaStreamFsm.MediaState?();
      MediaStreamFsm.MediaState mediaState;
      if (!dictionary.TryGetValue(mediaEvent, out mediaState))
        return new MediaStreamFsm.MediaState?();
      return new MediaStreamFsm.MediaState?(mediaState);
    }

    public void ValidateEvent(MediaStreamFsm.MediaEvent mediaEvent)
    {
      MediaStreamFsm.MediaState source = (MediaStreamFsm.MediaState) this._mediaState;
      MediaStreamFsm.MediaState? nullable;
      MediaStreamFsm.MediaState mediaState;
      while (true)
      {
        nullable = this.Find(source, mediaEvent);
        if (nullable.HasValue)
        {
          mediaState = (MediaStreamFsm.MediaState) Interlocked.CompareExchange(ref this._mediaState, (int) nullable.Value, (int) source);
          if (mediaState != source)
            source = mediaState;
          else
            goto label_3;
        }
        else
          break;
      }
      Debug.WriteLine(string.Format("ValidateEvent Invalid state transition: state {0} event {1}", new object[2]
      {
        (object) source,
        (object) mediaEvent
      }));
      return;
label_3:
      if (!MediaStreamFsm.NoisyLogging && mediaState == nullable.Value)
        return;
      Debug.WriteLine("Media {0}: {1} -> {2} at {3}", (object) mediaEvent, (object) mediaState, (object) nullable, (object) DateTimeOffset.Now);
    }

    public void Reset()
    {
      this._mediaState = 1;
    }

    public override string ToString()
    {
      return string.Format("[Media {0}]", (object) (MediaStreamFsm.MediaState) this._mediaState);
    }

    public enum MediaEvent
    {
      MediaStreamSourceAssigned,
      OpenMediaAsyncCalled,
      CallingReportOpenMediaCompleted,
      CallingReportOpenMediaCompletedLive,
      SeekAsyncCalled,
      CallingReportSeekCompleted,
      CallingReportSampleCompleted,
      GetSampleAsyncCalled,
      CloseMediaCalled,
      MediaStreamSourceCleared,
      DisposeCalled,
      StreamsClosed,
    }

    public enum MediaState
    {
      Invalid,
      Idle,
      Assigned,
      Opening,
      AwaitSeek,
      Seeking,
      AwaitPlaying,
      Playing,
      Closing,
      Disposing,
      Draining,
    }
  }
}
