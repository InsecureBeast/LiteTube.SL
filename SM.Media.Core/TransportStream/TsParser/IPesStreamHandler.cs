namespace SM.Media.Core.TransportStream.TsParser
{
  public interface IPesStreamHandler
  {
    void PacketHandler(TsPesPacket packet);
  }
}
