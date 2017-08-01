namespace LiteTube.StreamVideo.TransportStream.TsParser
{
  public interface IPesStreamHandler
  {
    void PacketHandler(TsPesPacket packet);
  }
}
