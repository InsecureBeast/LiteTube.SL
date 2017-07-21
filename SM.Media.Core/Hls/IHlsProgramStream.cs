using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.M3U8;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.Hls
{
  public interface IHlsProgramStream : IProgramStream
  {
    Task SetParserAsync(M3U8Parser parser, CancellationToken cancellationToken);
  }
}
