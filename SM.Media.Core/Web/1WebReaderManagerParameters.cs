using System.Collections.Generic;

namespace SM.Media.Core.Web
{
  public class WebReaderManagerParameters : IWebReaderManagerParameters
  {
    public IEnumerable<KeyValuePair<string, string>> DefaultHeaders { get; set; }
  }
}
