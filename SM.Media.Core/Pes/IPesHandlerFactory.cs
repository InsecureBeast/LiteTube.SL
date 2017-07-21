using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.Pes
{
  public interface IPesHandlerFactory
  {
    PesStreamHandler CreateHandler(PesStreamParameters parameters);
  }
}
