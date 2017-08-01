using System;
using System.IO;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Pes
{
    public sealed class PesStream : Stream
    {
        private int _location;
        private TsPesPacket _packet;

        public TsPesPacket Packet
        {
            get
            {
                return _packet;
            }
            set
            {
                _packet = value;
                _location = 0;
            }
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => Packet.Length;

        public override long Position
        {
            get
            {
                return _location;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            TsPesPacket packet = Packet;
            count = Math.Min(count, packet.Length - _location);
            if (count < 1)
                return 0;
            Array.Copy(packet.Buffer, packet.Index + _location, buffer, offset, count);
            _location += count;
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset > Packet.Length || offset < 0L)
                        throw new ArgumentOutOfRangeException(nameof(offset));
                    _location = (int)offset;
                    break;
                case SeekOrigin.Current:
                    long num = _location + offset;
                    if (num < 0L || num > Packet.Length)
                        throw new ArgumentOutOfRangeException(nameof(offset));
                    _location = (int)num;
                    break;
                case SeekOrigin.End:
                    if (offset > Packet.Length || offset < 0L)
                        throw new ArgumentOutOfRangeException(nameof(offset));
                    _location = Packet.Length - (int)offset;
                    break;
                default:
                    throw new ArgumentException("origin");
            }
            return _location;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
