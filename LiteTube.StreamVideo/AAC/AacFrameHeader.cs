using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.AAC
{
    public sealed class AacFrameHeader : IAudioFrameHeader
    {
        private static readonly int[] SamplingFrequencyTable = new int[13]
        {
            96000,
            88200,
            64000,
            48000,
            44100,
            32000,
            24000,
            22050,
            16000,
            12000,
            11025,
            8000,
            7350
        };

        private static readonly Dictionary<int, string> ProfileNames = new Dictionary<int, string>()
        {
            { 0, "AAC Main" },
            { 1, "AAC LC (Low Complexity)" },
            { 2, "AAC SSR (Scalable Sample Rate)" },
            { 3, "AAC LTP (Long Term Prediction)" },
            { 4, "SBR (Spectral Band Replication)" },
            { 5, "AAC Scalable" },
            { 6, "TwinVQ" },
            { 7, "CELP (Code Excited Linear Prediction)" },
            { 8, "HXVC (Harmonic Vector eXcitation Coding)" },
            { 11, "TTSI (Text-To-Speech Interface)" },
            { 12, "Main Synthesis" },
            { 13, "Wavetable Synthesis" },
            { 14, "General MIDI" },
            { 15, "Algorithmic Synthesis and Audio Effects" },
            { 16, "ER (Error Resilient) AAC LC" },
            { 18, "ER AAC LTP" },
            { 19, "ER AAC Scalable" },
            { 20, "ER TwinVQ" },
            { 21, "ER BSAC (Bit-Sliced Arithmetic Coding)" },
            { 22, "ER AAC LD (Low Delay)" },
            { 23, "ER CELP" },
            { 24, "ER HVXC" },
            { 25, "ER HILN (Harmonic and Individual Lines plus Noise)" },
            { 26, "ER Parametric" },
            { 27, "SSC (SinuSoidal Coding)" },
            { 28, "PS (Parametric Stereo)" },
            { 29, "MPEG Surround" },
            { 31, "Layer-1" },
            { 32, "Layer-2" },
            { 33, "Layer-3 (MP3)" },
            { 34, "DST (Direct Stream Transfer)" },
            { 35, "ALS (Audio Lossless)" },
            { 36, "SLS (Scalable LosslesS)" },
            { 37, "SLS non-core" },
            { 38, "ER AAC ELD (Enhanced Low Delay)" },
            { 39, "SMR (Symbolic Music Representation) Simple" },
            { 40, "SMR Main" },
            { 41, "USAC (Unified Speech and Audio Coding) (no SBR)" },
            { 42, "SAOC (Spatial Audio Object Coding)" },
            { 43, "LD MPEG Surround" },
            { 44, "USAC" }
        };

        private byte[] _audioSpecificConfig;
        private int _frames;
        public int Profile { get; private set; }
        public int Layer { get; private set; }
        public int FrequencyIndex { get; private set; }
        public ushort ChannelConfig { get; private set; }
        public bool CrcFlag { get; set; }

        public ICollection<byte> AudioSpecificConfig
        {
            get
            {
                if (null == _audioSpecificConfig)
                    _audioSpecificConfig = AacDecoderSettings.Parameters.AudioSpecificConfigFactory(this).ToArray();
                return _audioSpecificConfig;
            }
            set
            {
                _audioSpecificConfig = value.ToArray();
            }
        }

        public int Bitrate { get; private set; }

        public int HeaderLength
        {
            get
            {
                return CrcFlag ? 9 : 7;
            }
        }

        public int HeaderOffset { get; private set; }

        public TimeSpan Duration
        {
            get
            {
                return FullResolutionTimeSpan.FromSeconds(_frames * 1024.0 / SamplingFrequency);
            }
        }

        public int SamplingFrequency { get; private set; }

        public int FrameLength { get; private set; }

        public string Name { get; private set; }

        public bool Parse(byte[] buffer, int index, int length, bool verbose = false)
        {
            int num1 = index;
            int num2 = index + length;
            HeaderOffset = 0;
            if (length < 7)
                return false;

            byte num3;
            do
            {
                bool flag = true;
                do
                {
                    flag = true;
                    if (index + 7 > num2)
                        return false;
                }
                while (byte.MaxValue != buffer[index++]);

                if (index + 6 <= num2)
                    num3 = buffer[index++];
                else
                    return false;
            }
            while (15 != (num3 >> 4 & 15));

            HeaderOffset = index - num1 - 2;
            if (HeaderOffset < 0)
                return false;

            bool flag1 = 0 == (num3 & 8);
            Layer = num3 >> 1 & 3;
            if (0 != Layer)
                Debug.WriteLine("AacFrameHeader.Parse() unknown layer: " + Layer);

            CrcFlag = 0 == (num3 & 1);
            byte num4 = buffer[index++];
            Profile = num4 >> 6 & 3;
            FrequencyIndex = num4 >> 2 & 15;
            SamplingFrequency = GetSamplingFrequency(FrequencyIndex);

            if (SamplingFrequency <= 0)
                return false;

            int num5 = num4 >> 1 & 1;
            byte num6 = buffer[index++];
            ChannelConfig = (ushort) ((num4 & 1) << 2 | num6 >> 6 & 3);
            int num7 = num6 >> 5 & 1;
            int num8 = num6 >> 4 & 1;
            int num9 = num6 >> 3 & 1;
            int num10 = num6 >> 2 & 1;
            byte num11 = buffer[index++];
            byte num12 = buffer[index++];
            FrameLength = (num6 & 3) << 11 | num11 << 3 | num12 >> 5 & 7;

            if (FrameLength < 1)
                return false;

            byte num13 = buffer[index++];
            int num14 = (num12 & 31) << 6 | num13 >> 2 & 63;
            _frames = 1 + (num13 & 3);

            if (_frames < 1)
                return false;

            if (CrcFlag)
            {
                if (index + 2 > num2)
                    return false;
                byte num15 = buffer[index++];
                byte num16 = buffer[index++];
            }
            if (string.IsNullOrEmpty(Name))
                Name = string.Format("{0}, {1}kHz {2} channels", GetProfileName(), (SamplingFrequency / 1000.0), ChannelConfig);

            if (verbose)
                Debug.WriteLine("Configuration AAC layer {0} profile \"{1}\" channels {2} sampling {3}kHz length {4} CRC {5}", Layer, Name, ChannelConfig, (SamplingFrequency / 1000.0), FrameLength, (CrcFlag));
            return true;
        }

        private string GetProfileName()
        {
            string str;
            if (AacFrameHeader.ProfileNames.TryGetValue(Profile, out str))
                return str;
            return "Profile" + Profile;
        }

        private int GetSamplingFrequency(int samplingIndex)
        {
            if (samplingIndex < 0 || samplingIndex >= AacFrameHeader.SamplingFrequencyTable.Length)
                return -1;
            return AacFrameHeader.SamplingFrequencyTable[samplingIndex];
        }
    }
}
