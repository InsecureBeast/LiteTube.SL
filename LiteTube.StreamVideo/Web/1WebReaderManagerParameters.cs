using System.Collections.Generic;

namespace LiteTube.StreamVideo.Web
{
  public class WebReaderManagerParameters : IWebReaderManagerParameters
  {
    public IEnumerable<KeyValuePair<string, string>> DefaultHeaders { get; set; }
  }
}
