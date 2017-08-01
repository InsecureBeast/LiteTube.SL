using System.Collections.Generic;

namespace LiteTube.StreamVideo.Web
{
  public interface IWebReaderManagerParameters
  {
    IEnumerable<KeyValuePair<string, string>> DefaultHeaders { get; }
  }
}
