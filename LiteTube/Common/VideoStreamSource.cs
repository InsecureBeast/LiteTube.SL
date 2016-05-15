using Microsoft.PlayerFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LiteTube.Common
{
    /*
    public class VideoMediaStreamSource : MediaStreamSource
    {
        //private Stream _videoStream;
        private Stream _audioStream;
        private WaveFormatEx _waveFormat;
        private byte[] _audioSourceBytes;
        private long _currentAudioTimeStamp;

        private MediaStreamDescription _audioDesc;

        private Stream _frameStream;

        private int _frameWidth;
        private int _frameHeight;

        private int _framePixelSize;
        private int _frameBufferSize;
        public const int BytesPerPixel = 4;   // 32 bit including alpha


        private byte[][] _frames = new byte[2][];

        private int _currentReadyFrame;
        private int _currentBufferFrame;


        private int _frameTime;
        private long _currentVideoTimeStamp;

        private MediaStreamDescription _videoDesc;
        private Dictionary<MediaSampleAttributeKeys, string> _emptySampleDict = new Dictionary<MediaSampleAttributeKeys, string>();


        //public byte[] CurrentFrameBytes
        //{
        //    get { return _frame; }
        //}


        public void WritePixel(int position, Color color)
        {
            //BitConverter.GetBytes(color).CopyTo(_frame, position * BytesPerPixel);

            int offset = position * BytesPerPixel;

            _frames[_currentBufferFrame][offset++] = color.B;
            _frames[_currentBufferFrame][offset++] = color.G;
            _frames[_currentBufferFrame][offset++] = color.R;
            _frames[_currentBufferFrame][offset++] = color.A;

            //if (position < 10)
            //    System.Diagnostics.Debug.WriteLine("Pixel at {3} is {0} {1} {2}", color.R, color.B, color.G, position);
        }



        public VideoMediaStreamSource(Stream audioStream, int frameWidth, int frameHeight)
        {
            _audioStream = audioStream;

            _frameWidth = frameWidth;
            _frameHeight = frameHeight;

            _framePixelSize = frameWidth * frameHeight;
            _frameBufferSize = _framePixelSize * BytesPerPixel;

            // PAL is 50 frames per second
            _frameTime = (int)TimeSpan.FromSeconds((double)1 / 50).Ticks;

            _frames[0] = new byte[_frameBufferSize];
            _frames[1] = new byte[_frameBufferSize];

            _currentBufferFrame = 0;
            _currentReadyFrame = 1;
        }


        public void Flip()
        {
            int f = _currentBufferFrame;
            _currentBufferFrame = _currentReadyFrame;
            _currentReadyFrame = f;
        }

        private void PrepareVideo()
        {
            _frameStream = new MemoryStream();

            // Stream Description 
            Dictionary<MediaStreamAttributeKeys, string> streamAttributes =
                new Dictionary<MediaStreamAttributeKeys, string>();

            streamAttributes[MediaStreamAttributeKeys.VideoFourCC] = "MP43";
            streamAttributes[MediaStreamAttributeKeys.Height] = _frameHeight.ToString();
            streamAttributes[MediaStreamAttributeKeys.Width] = _frameWidth.ToString();

            MediaStreamDescription msd = new MediaStreamDescription(MediaStreamType.Video, streamAttributes);
            _videoDesc = msd;
        }


        private void PrepareAudio()
        {
            short BitsPerSample = 16;
            int SampleRate = 8000;          // change this to something higher if we output sound from here
            short ChannelCount = 1;
            int ByteRate = SampleRate * ChannelCount * (BitsPerSample / 8);


            _waveFormat = new WaveFormatEx();
            _waveFormat.BitsPerSample = BitsPerSample;
            _waveFormat.AverageBytesPerSecond = (int)ByteRate;
            _waveFormat.Channels = ChannelCount;
            _waveFormat.BlockAlign = (short)(ChannelCount * (BitsPerSample / 8));
            //_waveFormat.ext = null; // ??
            _waveFormat.FormatTag = 0x0050;
            _waveFormat.SamplesPerSec = SampleRate;
            _waveFormat.Size = 0; // must be zero

            //_waveFormat.ValidateWaveFormat();


            _audioStream = new System.IO.MemoryStream();
            _audioSourceBytes = new byte[ByteRate];


            // TEMP just load the audio buffer with silence
            for (int i = 1; i < SampleRate; i++)
            {
                _audioSourceBytes[i] = 0;
            }

            // Stream Description 
            Dictionary<MediaStreamAttributeKeys, string> streamAttributes = new Dictionary<MediaStreamAttributeKeys, string>();
            streamAttributes[MediaStreamAttributeKeys.CodecPrivateData] = _waveFormat.ToHexString(); // wfx
            MediaStreamDescription msd = new MediaStreamDescription(MediaStreamType.Audio, streamAttributes);
            _audioDesc = msd;

        }

        protected override void OpenMediaAsync()
        {
            // Init
            Dictionary<MediaSourceAttributesKeys, string> sourceAttributes = new Dictionary<MediaSourceAttributesKeys, string>();
            List<MediaStreamDescription> availableStreams = new List<MediaStreamDescription>();

            PrepareVideo();

            availableStreams.Add(_videoDesc);

            // a zero timespan is an infinite video
            sourceAttributes[MediaSourceAttributesKeys.Duration] = TimeSpan.FromSeconds(0).Ticks.ToString(CultureInfo.InvariantCulture);
            sourceAttributes[MediaSourceAttributesKeys.CanSeek] = false.ToString();

            // tell Silverlight that we've prepared and opened our video
            ReportOpenMediaCompleted(sourceAttributes, availableStreams);
        }


        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            if (mediaStreamType == MediaStreamType.Audio)
            {
                GetAudioSample();
            }
            else if (mediaStreamType == MediaStreamType.Video)
            {
                GetVideoSample();
            }
        }


        // TEMP!
        private void GetAudioSample()
        {
            int bufferSize = _audioSourceBytes.Length;

            // spit out one second
            _audioStream.Write(_audioSourceBytes, 0, bufferSize);

            // Send out the next sample
            MediaStreamSample msSamp = new MediaStreamSample(
                _audioDesc,
                _audioStream,
                0,
                bufferSize,
                _currentAudioTimeStamp,
                _emptySampleDict);

            //_currentAudioTimeStamp += _waveFormat.AudioDurationFromBufferSize((uint)bufferSize);
            _currentAudioTimeStamp += _frameTime;

            ReportGetSampleCompleted(msSamp);
        }

        //private static int offset = 0;
        private void GetVideoSample()
        {
            // seems like creating a new stream is only way to avoid out of memory and
            // actually figure out the correct offset. that can't be right.
            _frameStream = new MemoryStream();
            _frameStream.Write(_frames[_currentReadyFrame], 0, _frameBufferSize);

            // Send out the next sample
            MediaStreamSample msSamp = new MediaStreamSample(
                _videoDesc,
                _frameStream,
                0,
                _frameBufferSize,
                _currentVideoTimeStamp,
                _emptySampleDict);

            _currentVideoTimeStamp += _frameTime;

            ReportGetSampleCompleted(msSamp);
        }

        protected override void CloseMedia()
        {
            _currentAudioTimeStamp = 0;
            _currentVideoTimeStamp = 0;
        }

        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }

        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }

        protected override void SeekAsync(long seekToTime)
        {
            _currentVideoTimeStamp = seekToTime;

            ReportSeekCompleted(seekToTime);
        }
    }
}
/**/
    class VideoStreamSource : MediaStreamSource, IDisposable
    {
        private MediaStreamDescription _videoStreamDescription;
        private MediaStreamDescription _audioStreamDescription;
        private int _audioSampleIndex = 0;
        private int _videoSampleIndex = 0;
        private Dictionary<MediaSampleAttributeKeys, string> _emptyDict = new Dictionary<MediaSampleAttributeKeys, string>();
        private Stream _videoStream;
        private Stream _audioStream;
        private const int _samplesPerSec = 25;
        //For example, for video I have 90 kHz and for audio 48 kHz and 25 frames per second - my frame increments will be:
        private int _videoFrameTime = (int)TimeSpan.FromSeconds(0.9 / _samplesPerSec).Ticks;
        private int _audioFrameTime = (int)TimeSpan.FromSeconds(0.48 / _samplesPerSec).Ticks;
        private int _currentAudioTimeStamp = 0;
        private int _currentVideoTimeStamp = 0;
        private int _frameWidth;
        private int _frameHeight;

        public VideoStreamSource(Stream videoStream, Stream audioStream)
        {
            _videoStream = videoStream;
            _audioStream = audioStream;
            _frameHeight = 480;
            _frameWidth = 640;
        }

        protected override void CloseMedia()
        {
        }

        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
        }

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            MediaStreamSample mediaStreamSample = null;

            if (mediaStreamType == MediaStreamType.Audio)
                mediaStreamSample = GetAudioSample();
            else if (mediaStreamType == MediaStreamType.Video)
                mediaStreamSample = GetVideoSample();

            ReportGetSampleCompleted(mediaStreamSample);
        }

        protected override void OpenMediaAsync()
        {
            //Audio
            WaveFormatEx wfx = new WaveFormatEx();
            wfx.FormatTag = 0x00FF;
            wfx.Channels = 2;
            wfx.BlockAlign = 8;
            wfx.BitsPerSample = 16;
            wfx.SamplesPerSec = _samplesPerSec;
            wfx.AverageBytesPerSecond = wfx.SamplesPerSec * wfx.Channels * wfx.BitsPerSample / wfx.BlockAlign;
            wfx.Size = 0;
            string codecPrivateData = wfx.ToHexString();

            Dictionary<MediaStreamAttributeKeys, string> audioStreamAttributes = new Dictionary<MediaStreamAttributeKeys, string>();
            //audioStreamAttributes[MediaStreamAttributeKeys.CodecPrivateData] = codecPrivateData;
            _audioStreamDescription = new MediaStreamDescription(MediaStreamType.Audio, audioStreamAttributes);

            //Video
            Dictionary<MediaStreamAttributeKeys, string> videoStreamAttributes = new Dictionary<MediaStreamAttributeKeys, string>();
            videoStreamAttributes[MediaStreamAttributeKeys.VideoFourCC] = "MP43";
            videoStreamAttributes[MediaStreamAttributeKeys.Height] = _frameHeight.ToString();
            videoStreamAttributes[MediaStreamAttributeKeys.Width] = _frameWidth.ToString();
            _videoStreamDescription = new MediaStreamDescription(MediaStreamType.Video, videoStreamAttributes);

            //Media
            Dictionary<MediaSourceAttributesKeys, string> mediaSourceAttributes = new Dictionary<MediaSourceAttributesKeys, string>();
            mediaSourceAttributes[MediaSourceAttributesKeys.Duration] = TimeSpan.FromSeconds(0).Ticks.ToString(CultureInfo.InvariantCulture);
            mediaSourceAttributes[MediaSourceAttributesKeys.CanSeek] = true.ToString();

            List<MediaStreamDescription> mediaStreamDescriptions = new List<MediaStreamDescription>();
            //mediaStreamDescriptions.Add(_audioStreamDescription);
            mediaStreamDescriptions.Add(_videoStreamDescription);

            ReportOpenMediaCompleted(mediaSourceAttributes, mediaStreamDescriptions);
        }

        protected override void SeekAsync(long seekToTime)
        {
            ReportSeekCompleted(seekToTime);
        }

        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
        }

        private MediaStreamSample GetAudioSample()
        {
            // Getting sample from buffer
        
            var mediaStreamSample = new MediaStreamSample
                (
                    _audioStreamDescription,
                    _audioStream,
                    0,
                    _audioStream.Length,
                    _currentAudioTimeStamp,
                    _emptyDict
                );

                _currentAudioTimeStamp += _audioFrameTime;
                return mediaStreamSample;
            }

            private MediaStreamSample GetVideoSample()
            {
                var mediaStreamSample = new MediaStreamSample
                (
                    _videoStreamDescription,
                    _videoStream,
                    0,
                    _videoStream.Length,
                    _currentVideoTimeStamp,
                    _emptyDict
                );

                _currentVideoTimeStamp += _videoFrameTime;

                return mediaStreamSample;
            }

            public void Dispose()
            {
            }
        }


    class A : PluginBase
    {
        protected override bool OnActivate()
        {
            return true;
        }

        protected override void OnDeactivate()
        {
        }
    }
}