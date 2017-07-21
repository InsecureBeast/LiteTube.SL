using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Web.HttpConnection
{
  public static class HttpReaderExtensions
  {
    public static async Task<Tuple<string, string>> ReadHeaderAsync(this HttpReader httpReader, CancellationToken cancellationToken)
    {
      string header;
      int colon;
      while (true)
      {
        header = await httpReader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(header))
        {
          colon = header.IndexOf(':');
          if (colon < 1)
            Debug.WriteLine("Bad header: " + header);
          else
            goto label_5;
        }
        else
          break;
      }
      Tuple<string, string> tuple = (Tuple<string, string>) null;
      goto label_7;
label_5:
      string name = header.Substring(0, colon).Trim();
      string value = colon + 1 < header.Length ? header.Substring(colon + 1).Trim() : string.Empty;
      tuple = Tuple.Create<string, string>(name, value);
label_7:
      return tuple;
    }

    public static async Task<string> ReadNonBlankLineAsync(this IHttpReader httpReader, CancellationToken cancellationToken)
    {
      string line;
      do
      {
        line = await httpReader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
        if (null == line)
          goto label_2;
      }
      while (string.IsNullOrWhiteSpace(line));
      goto label_4;
label_2:
      string str = (string) null;
      goto label_6;
label_4:
      str = line;
label_6:
      return str;
    }
  }
}
