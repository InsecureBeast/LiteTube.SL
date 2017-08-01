using System;
using System.Diagnostics;
using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.TransportStream.TsParser.Utility;

namespace LiteTube.StreamVideo.MP3
{
    public sealed class Mp3Parser : AudioParserBase
    {
        private int _skip;

        public Mp3Parser(ITsPesPacketPool pesPacketPool, Action<IAudioFrameHeader> configurationHandler, Action<TsPesPacket> submitPacket)
          : base(new Mp3FrameHeader(), pesPacketPool, configurationHandler, submitPacket)
        {
        }

        public override void ProcessData(byte[] buffer, int offset, int length)
        {
            Debug.Assert(length > 0);
            Debug.Assert(offset + length <= buffer.Length);
            if (_skip == 0 && _index == 0 && 0 == _startIndex)
            {
                int? id3Length = GetId3Length(buffer, offset, length);
                if (id3Length.HasValue)
                {
                    _skip = id3Length.Value + 10;
                    Debug.WriteLine("Mp3Parser.ProcessData() ID3 detected, length {0}", _skip);
                }
            }
            if (_skip > 0)
            {
                if (_skip >= length)
                {
                    _skip -= length;
                    return;
                }
                offset += _skip;
                length -= _skip;
                _skip = 0;
            }
            int num1 = offset + length;
            EnsureBufferSpace(128);
            int sourceIndex = offset;
            label_32:
            while (sourceIndex < num1)
            {
                int num2 = _index - _startIndex;
                if (num2 < 4)
                {
                    byte num3 = buffer[sourceIndex++];
                    while (0 != num2)
                    {
                        if (1 == num2)
                        {
                            if (224 == (224 & (int)num3))
                            {
                                _packet.Buffer[_index++] = (byte)(int)num3;
                                goto label_31;
                            }
                            else
                            {
                                _index = _startIndex;
                                num2 = 0;
                                ++_badBytes;
                            }
                        }
                        else if (2 == num2)
                        {
                            _packet.Buffer[_index++] = (byte)(int)num3;
                            goto label_31;
                        }
                        else if (3 == num2)
                        {
                            _packet.Buffer[_index++] = (byte)(int)num3;
                            bool verbose = !_isConfigured && _hasSeenValidFrames && 0 == _badBytes;
                            if (!_frameHeader.Parse(_packet.Buffer, _startIndex, _index - _startIndex, verbose))
                            {
                                SkipInvalidFrameHeader();
                                goto label_32;
                            }
                            else
                            {
                                Debug.Assert(_frameHeader.FrameLength > 4);
                                if (verbose)
                                {
                                    _configurationHandler(_frameHeader);
                                    _isConfigured = true;
                                }
                                EnsureBufferSpace(_frameHeader.FrameLength - 4);
                                goto label_31;
                            }
                        }
                        else
                            goto label_31;
                    }
                    if ((int)byte.MaxValue == (int)num3)
                        _packet.Buffer[_index++] = (byte)(int)byte.MaxValue;
                    else
                        ++_badBytes;
                }
                else
                {
                    int val2 = _frameHeader.FrameLength - (_index - _startIndex);
                    int length1 = Math.Min(num1 - sourceIndex, val2);
                    Debug.Assert(length1 > 0);
                    Array.Copy((Array)buffer, sourceIndex, (Array)_packet.Buffer, _index, length1);
                    _index += length1;
                    sourceIndex += length1;
                    if (_index - _startIndex == _frameHeader.FrameLength)
                        SubmitFrame();
                }
                label_31:;
            }
        }

        public override void FlushBuffers()
        {
            base.FlushBuffers();
            _skip = 0;
        }

        private static int? GetId3Length(byte[] buffer, int offset, int length)
        {
            if (length < 10)
                return new int?();
            if (73 != (int)buffer[offset] || 68 != (int)buffer[offset + 1] || 51 != (int)buffer[offset + 2])
                return new int?();
            if ((int)byte.MaxValue == (int)buffer[offset + 3])
                return new int?();
            if ((int)byte.MaxValue == (int)buffer[offset + 4])
                return new int?();
            byte num1 = buffer[offset + 5];
            int num2 = 0;
            for (int index = 0; index < 4; ++index)
            {
                byte num3 = buffer[offset + 6 + index];
                if (0 != (128 & (int)num3))
                    return new int?();
                num2 = num2 << 7 | (int)num3;
            }
            return new int?(num2);
        }

        private void SkipInvalidFrameHeader()
        {
            if ((int)byte.MaxValue == (int)_packet.Buffer[_startIndex + 1] && 224 == (224 & (int)_packet.Buffer[_startIndex + 2]))
            {
                _packet.Buffer[_startIndex + 1] = _packet.Buffer[_startIndex + 2];
                _packet.Buffer[_startIndex + 2] = _packet.Buffer[_startIndex + 3];
                _index = _startIndex + 3;
                ++_badBytes;
            }
            else if ((int)byte.MaxValue == (int)_packet.Buffer[_startIndex + 2] && 224 == (224 & (int)_packet.Buffer[_startIndex + 3]))
            {
                _packet.Buffer[_startIndex + 1] = _packet.Buffer[_startIndex + 3];
                _index = _startIndex + 2;
                _badBytes += 2;
            }
            else if ((int)byte.MaxValue == (int)_packet.Buffer[_startIndex + 3])
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
