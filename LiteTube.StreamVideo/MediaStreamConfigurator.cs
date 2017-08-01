using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using LiteTube.StreamVideo.Configuration;
using LiteTube.StreamVideo.MediaManager;
using LiteTube.StreamVideo.MediaParser;
using LiteTube.StreamVideo.Platform;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo
{
    public sealed class MediaStreamConfigurator : IMediaStreamConfigurator, IDisposable, IMediaStreamControl
    {
        private static readonly TimeSpan OpenTimeout = TimeSpan.FromSeconds(20.0);
        private readonly object _streamConfigurationLock = new object();
        private readonly Func<TsMediaStreamSource> _mediaStreamSourceFactory;
        private MediaStreamDescription _audioStreamDescription;
        private IStreamSource _audioStreamSource;
        private IMediaManager _mediaManager;
        private TsMediaStreamSource _mediaStreamSource;
        private TaskCompletionSource<IMediaStreamConfiguration> _openCompletionSource;
        private TaskCompletionSource<object> _playingCompletionSource;
        private MediaStreamDescription _videoStreamDescription;
        private IStreamSource _videoStreamSource;

        public MediaStreamDescription AudioStreamDescription
        {
            get
            {
                return this._audioStreamDescription;
            }
            set
            {
                this._audioStreamDescription = value;
            }
        }

        public MediaStreamDescription VideoStreamDescription
        {
            get
            {
                return this._videoStreamDescription;
            }
            set
            {
                this._videoStreamDescription = value;
            }
        }

        private IStreamSource AudioStreamSource
        {
            get
            {
                return this._audioStreamSource;
            }
            set
            {
                this._audioStreamSource = value;
            }
        }

        private IStreamSource VideoStreamSource
        {
            get
            {
                return this._videoStreamSource;
            }
            set
            {
                this._videoStreamSource = value;
            }
        }

        public TimeSpan? SeekTarget
        {
            get
            {
                return this._mediaStreamSource.SeekTarget;
            }
            set
            {
                this._mediaStreamSource.SeekTarget = value;
            }
        }

        public IMediaManager MediaManager
        {
            get
            {
                return this._mediaManager;
            }
            set
            {
                if (object.ReferenceEquals((object)this._mediaManager, (object)value))
                    return;
                if (null != value)
                {
                    this._playingCompletionSource = new TaskCompletionSource<object>();
                    this.ResetOpenCompletionSource();
                }
                this._mediaManager = value;
                if (null != value)
                    return;
                this.CleanupMediaStreamSource();
            }
        }

        public MediaStreamConfigurator(Func<TsMediaStreamSource> mediaStreamSourceFactory)
        {
            this._mediaStreamSourceFactory = mediaStreamSourceFactory;
        }

        public void Dispose()
        {
            this.AudioStreamSource = (IStreamSource)null;
            this.AudioStreamDescription = (MediaStreamDescription)null;
            this.VideoStreamSource = (IStreamSource)null;
            this.VideoStreamDescription = (MediaStreamDescription)null;
            this.CleanupMediaStreamSource();
        }

        public void Initialize()
        {
        }

        public void CheckForSamples()
        {
            TsMediaStreamSource mediaStreamSource = this._mediaStreamSource;
            if (null == mediaStreamSource)
                return;
            mediaStreamSource.CheckForSamples();
        }

        public void ValidateEvent(MediaStreamFsm.MediaEvent mediaEvent)
        {
            TsMediaStreamSource mediaStreamSource = this._mediaStreamSource;
            if (null == mediaStreamSource)
                return;
            mediaStreamSource.ValidateEvent(mediaEvent);
        }

        async Task<IMediaStreamConfiguration> IMediaStreamControl.OpenAsync(CancellationToken cancellationToken)
        {
            if (null == this._mediaStreamSource)
                throw new InvalidOperationException("MediaStreamSource has not been created");
            IMediaManager mediaManager = this.MediaManager;
            if (null == mediaManager)
                throw new InvalidOperationException("MediaManager has not been initialized");
            TaskCompletionSource<IMediaStreamConfiguration> openCompletionSource = this._openCompletionSource;
            Action cancellationAction = (Action)(() =>
            {
                Task task = mediaManager.CloseMediaAsync();
                TaskCollector.Default.Add(task, "MediaSteamConfigurator.OpenAsync mediaManager.CloseMediaAsync");
                openCompletionSource.TrySetCanceled();
            });
            using (cancellationToken.Register(cancellationAction))
            {
                Task timeoutTask = TaskEx.Delay(MediaStreamConfigurator.OpenTimeout, cancellationToken);
                Task task = await TaskEx.WhenAny((Task)this._openCompletionSource.Task, timeoutTask).ConfigureAwait(false);
            }
            if (!this._openCompletionSource.Task.IsCompleted)
                cancellationAction();
            return await this._openCompletionSource.Task.ConfigureAwait(false);
        }

        Task<TimeSpan> IMediaStreamControl.SeekAsync(TimeSpan position, CancellationToken cancellationToken)
        {
            IMediaManager mediaManager = this.MediaManager;
            if (null == mediaManager)
                throw new InvalidOperationException("MediaManager has not been initialized");
            return mediaManager.SeekMediaAsync(position);
        }

        Task IMediaStreamControl.CloseAsync(CancellationToken cancellationToken)
        {
            if (null == this.MediaManager)
            {
                Debug.WriteLine("MediaStreamConfigurator.CloseMediaHandler() null media manager");
                return TplTaskExtensions.CompletedTask;
            }
            this._playingCompletionSource.TrySetResult((object)null);
            return TplTaskExtensions.CompletedTask;
        }

        public Task PlayAsync(IMediaConfiguration configuration, CancellationToken cancellationToken)
        {
            if (null != configuration.Audio)
                this.ConfigureAudioStream(configuration.Audio);
            if (null != configuration.Video)
                this.ConfigureVideoStream(configuration.Video);
            lock (this._streamConfigurationLock)
                this.CompleteConfigure(configuration.Duration);
            return (Task)this._playingCompletionSource.Task;
        }

        public Task<TMediaStreamSource> CreateMediaStreamSourceAsync<TMediaStreamSource>(CancellationToken cancellationToken) where TMediaStreamSource : class
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (null != this._mediaStreamSource)
                throw new InvalidOperationException("MediaStreamSource already exists");
            TsMediaStreamSource mediaStreamSource = this._mediaStreamSourceFactory();
            TMediaStreamSource result = mediaStreamSource as TMediaStreamSource;
            if (null == (object)result)
            {
                mediaStreamSource.Dispose();
                this._openCompletionSource.TrySetCanceled();
                this._playingCompletionSource.TrySetResult((object)null);
                throw new InvalidCastException(string.Format("Cannot convert {0} to {1}", (object)mediaStreamSource.GetType().FullName, (object)typeof(TMediaStreamSource).FullName));
            }
            this._mediaStreamSource = mediaStreamSource;
            return TaskEx.FromResult<TMediaStreamSource>(result);
        }

        public async Task CloseAsync()
        {
            TsMediaStreamSource mediaStreamSource = this._mediaStreamSource;
            TaskCompletionSource<object> pcs = this._playingCompletionSource;
            if (null == mediaStreamSource)
            {
                if (null != this._openCompletionSource)
                    this._openCompletionSource.TrySetCanceled();
                if (null != pcs)
                    pcs.TrySetResult((object)null);
            }
            else
            {
                await mediaStreamSource.CloseAsync().ConfigureAwait(false);
                if (null != pcs)
                {
                    object obj = await pcs.Task.ConfigureAwait(false);
                }
            }
        }

        public void ReportError(string message)
        {
            TsMediaStreamSource mediaStreamSource = this._mediaStreamSource;
            if (null == mediaStreamSource)
                Debug.WriteLine("MediaStreamConfigurator.ReportError() null _mediaStreamSource");
            else
                mediaStreamSource.ReportError(message);
        }

        private void ResetOpenCompletionSource()
        {
            if (this._openCompletionSource != null && !this._openCompletionSource.Task.IsCompleted)
                this._openCompletionSource.TrySetCanceled();
            this._openCompletionSource = new TaskCompletionSource<IMediaStreamConfiguration>();
        }

        private void CleanupMediaStreamSource()
        {
            TsMediaStreamSource mediaStreamSource = this._mediaStreamSource;
            if (null == mediaStreamSource)
                return;
            this._mediaStreamSource = (TsMediaStreamSource)null;
            DisposeExtensions.DisposeSafe((IDisposable)mediaStreamSource);
        }

        private void CompleteConfigure(TimeSpan? duration)
        {
            try
            {
                List<MediaStreamDescription> list = new List<MediaStreamDescription>();
                if (this._videoStreamSource != null && null != this._videoStreamDescription)
                    list.Add(this._videoStreamDescription);
                if (this._audioStreamSource != null && null != this._audioStreamSource)
                    list.Add(this._audioStreamDescription);
                Dictionary<MediaSourceAttributesKeys, string> dictionary = new Dictionary<MediaSourceAttributesKeys, string>();
                dictionary[MediaSourceAttributesKeys.Duration] = !duration.HasValue ? string.Empty : duration.Value.Ticks.ToString((IFormatProvider)CultureInfo.InvariantCulture);
                bool hasValue = duration.HasValue;
                dictionary[MediaSourceAttributesKeys.CanSeek] = hasValue.ToString();
                this._openCompletionSource.TrySetResult((IMediaStreamConfiguration)new MediaStreamConfiguration()
                {
                    VideoStreamSource = this._videoStreamSource,
                    AudioStreamSource = this._audioStreamSource,
                    Descriptions = (ICollection<MediaStreamDescription>)list,
                    Attributes = (IDictionary<MediaSourceAttributesKeys, string>)dictionary,
                    Duration = duration
                });
            }
            catch (OperationCanceledException ex)
            {
                this._openCompletionSource.TrySetCanceled();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MediaStreamConfigurator.CompleteConfigure() failed: " + ex.Message);
                this._openCompletionSource.TrySetException(ex);
            }
        }

        private void ConfigureVideoStream(IMediaParserMediaStream video)
        {
            IVideoConfigurationSource configurationSource = (IVideoConfigurationSource)video.ConfigurationSource;
            Dictionary<MediaStreamAttributeKeys, string> dictionary = new Dictionary<MediaStreamAttributeKeys, string>();
            dictionary[MediaStreamAttributeKeys.VideoFourCC] = configurationSource.VideoFourCc;
            string codecPrivateData = configurationSource.CodecPrivateData;
            Debug.WriteLine("MediaStreamConfigurator.ConfigureVideoStream(): CodecPrivateData: " + codecPrivateData);
            if (!string.IsNullOrWhiteSpace(codecPrivateData))
                dictionary[MediaStreamAttributeKeys.CodecPrivateData] = codecPrivateData;
            dictionary[MediaStreamAttributeKeys.Height] = configurationSource.Height.ToString();
            dictionary[MediaStreamAttributeKeys.Width] = configurationSource.Width.ToString();
            MediaStreamDescription streamDescription = new MediaStreamDescription(MediaStreamType.Video, (IDictionary<MediaStreamAttributeKeys, string>)dictionary);
            lock (this._streamConfigurationLock)
            {
                this._videoStreamSource = video.StreamSource;
                this._videoStreamDescription = streamDescription;
            }
        }

        private void ConfigureAudioStream(IMediaParserMediaStream audio)
        {
            IAudioConfigurationSource configurationSource = (IAudioConfigurationSource)audio.ConfigurationSource;
            Dictionary<MediaStreamAttributeKeys, string> dictionary = new Dictionary<MediaStreamAttributeKeys, string>();
            string codecPrivateData = configurationSource.CodecPrivateData;
            Debug.WriteLine("TsMediaStreamSource.ConfigureAudioStream(): CodecPrivateData: " + codecPrivateData);
            if (!string.IsNullOrWhiteSpace(codecPrivateData))
                dictionary[MediaStreamAttributeKeys.CodecPrivateData] = codecPrivateData;
            MediaStreamDescription streamDescription = new MediaStreamDescription(MediaStreamType.Audio, (IDictionary<MediaStreamAttributeKeys, string>)dictionary);
            lock (this._streamConfigurationLock)
            {
                this._audioStreamSource = audio.StreamSource;
                this._audioStreamDescription = streamDescription;
            }
        }
    }
}
