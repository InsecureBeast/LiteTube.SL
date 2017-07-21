using System.Collections.Generic;

namespace SM.Media.Core.Web
{
  public interface IWebReaderManagerParameters
  {
    IEnumerable<KeyValuePair<string, string>> DefaultHeaders { get; }
  }
}
