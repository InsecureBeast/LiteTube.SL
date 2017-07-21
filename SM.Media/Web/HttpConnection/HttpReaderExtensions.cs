// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnection.HttpReaderExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web.HttpConnection
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
