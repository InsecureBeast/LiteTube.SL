using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.M3U8;
using LiteTube.StreamVideo.Playlists;

namespace LiteTube.StreamVideo.Hls
{
    public interface IHlsProgramStream : IProgramStream
    {
        Task SetParserAsync(M3U8Parser parser, CancellationToken cancellationToken);
    }
}
