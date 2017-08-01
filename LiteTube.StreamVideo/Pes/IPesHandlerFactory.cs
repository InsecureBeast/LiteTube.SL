using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Pes
{
    public interface IPesHandlerFactory
    {
        PesStreamHandler CreateHandler(PesStreamParameters parameters);
    }
}
