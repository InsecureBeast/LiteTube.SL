namespace LiteTube.StreamVideo.TransportStream.TsParser
{
  public interface IProgramStream
  {
    uint Pid { get; }

    TsStreamType StreamType { get; }

    bool BlockStream { get; set; }

    string Language { get; }
  }
}
