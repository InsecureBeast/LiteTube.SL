// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.TextEncodings.SmEncodings
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Diagnostics;
using System.Text;

namespace SM.Media.Core.Utility.TextEncodings
{
  public class SmEncodings : ISmEncodings
  {
    internal static readonly Encoding Ascii = SmEncodings.GetAsciiEncoding();
    internal static readonly Encoding Latin1 = SmEncodings.GetLatin1Encoding();

    public Encoding Latin1Encoding
    {
      get
      {
        return SmEncodings.Latin1;
      }
    }

    public Encoding AsciiEncoding
    {
      get
      {
        return SmEncodings.Ascii;
      }
    }

    private static Encoding GetLatin1Encoding()
    {
      Encoding encoding1 = SmEncodings.GetEncoding("Windows-1252");
      if (null != encoding1)
        return encoding1;
      Encoding encoding2 = SmEncodings.GetEncoding("iso-8859-1");
      if (null != encoding2)
        return encoding2;
      return (Encoding) new Windows1252Encoding();
    }

    private static Encoding GetAsciiEncoding()
    {
      Encoding encoding = SmEncodings.GetEncoding("us-ascii");
      if (null != encoding)
        return encoding;
      return (Encoding) new AsciiEncoding();
    }

    private static Encoding GetEncoding(string name)
    {
      try
      {
        return Encoding.GetEncoding(name);
      }
      catch (Exception ex)
      {
        Debug.WriteLine("Unable to get " + name + " encoding");
      }
      return (Encoding) null;
    }
  }
}
