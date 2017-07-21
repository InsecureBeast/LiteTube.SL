namespace SM.Media.Core.TransportStream.TsParser
{
  public interface IProgramStream
  {
    uint Pid { get; }

    TsStreamType StreamType { get; }

    bool BlockStream { get; set; }

    string Language { get; }
  }
}
