using System;
using System.Diagnostics;
using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.TransportStream.TsParser.Utility;

namespace LiteTube.StreamVideo.AAC
{
    public sealed class AacParser : AudioParserBase
    {
        public AacParser(ITsPesPacketPool pesPacketPool, Action<IAudioFrameHeader> configurationHandler, Action<TsPesPacket> submitPacket)
            : base(new AacFrameHeader(), pesPacketPool, configurationHandler, submitPacket)
        {
        }

        public override void ProcessData(byte[] buffer, int offset, int length)
        {
            Debug.Assert(length > 0);
            Debug.Assert(offset + length <= buffer.Length);
            int endOffset = offset + length;
            EnsureBufferSpace(128);
            int index = offset;
            while (index < endOffset)
            {
                int storedLength = _index - _startIndex;
                if (storedLength <= 9)
                    ProcessHeader(storedLength, buffer[index++]);
                else
                    index += ProcessBody(buffer, index, storedLength, endOffset);
            }
        }

        private int ProcessBody(byte[] buffer, int index, int storedLength, int endOffset)
        {
            int val2 = _frameHeader.FrameLength - storedLength;
            Debug.Assert(val2 >= 0);
            int val1 = endOffset - index;
            Debug.Assert(val1 >= 0);
            int length = Math.Min(val1, val2);
            Debug.Assert(length >= 0);

            if (length > 0)
                Array.Copy(buffer, index, _packet.Buffer, _index, length);

            _index += length;
            storedLength += length;
            Debug.Assert(storedLength >= 0 && storedLength == _index - _startIndex);
            Debug.Assert(storedLength <= _frameHeader.FrameLength);

            if (storedLength == _frameHeader.FrameLength)
            {
                if (AacDecoderSettings.Parameters.UseRawAac)
                    _startIndex += _frameHeader.HeaderLength;
                SubmitFrame();
            }

            return length;
        }

        private void ProcessHeader(int storedLength, byte data)
        {
            while (0 != storedLength)
            {
                if (1 == storedLength)
                {
                    if (240 == (240 & data))
                    {
                        _packet.Buffer[_index++] = data;
                        return;
                    }

                    _index = _startIndex;
                    storedLength = 0;
                    ++_badBytes;
                }
                else
                {
                    if (storedLength < 9)
                    {
                        _packet.Buffer[_index++] = data;
                        return;
                    }

                    _packet.Buffer[_index++] = data;
                    bool verbose = !_isConfigured && _hasSeenValidFrames && 0 == _badBytes;
                    if (!_frameHeader.Parse(_packet.Buffer, _startIndex, _index - _startIndex, verbose))
                    {
                        SkipInvalidFrameHeader();
                        return;
                    }

                    Debug.Assert(_frameHeader.FrameLength > 7);
                    if (verbose)
                    {
                        _configurationHandler(_frameHeader);
                        _isConfigured = true;
                    }
                    
                    EnsureBufferSpace(_frameHeader.FrameLength);
                    return;
                }
            }

            if (byte.MaxValue == data)
                _packet.Buffer[_index++] = byte.MaxValue;
            else
                ++_badBytes;
        }

        private void SkipInvalidFrameHeader()
        {
            if (byte.MaxValue == _packet.Buffer[_startIndex + 1] && 240 == (240 & _packet.Buffer[_startIndex + 2]))
            {
                _packet.Buffer[_startIndex + 1] = _packet.Buffer[_startIndex + 2];
                _packet.Buffer[_startIndex + 2] = _packet.Buffer[_startIndex + 3];
                _index = _startIndex + 3;
                ++_badBytes;
            }
            else if (byte.MaxValue == _packet.Buffer[_startIndex + 2] && 240 == (240 & _packet.Buffer[_startIndex + 3]))
            {
                _packet.Buffer[_startIndex + 1] = _packet.Buffer[_startIndex + 3];
                _index = _startIndex + 2;
                _badBytes += 2;
            }
            else if (byte.MaxValue == _packet.Buffer[_startIndex + 3])
            {
                _index = _startIndex + 1;
                _badBytes += 3;
            }
            else
            {
                _index = _startIndex;
                _badBytes += 4;
            }
        }
    }
}
