// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.UriExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.IO;

namespace SM.Media.Web
{
  public static class UriExtensions
  {
    private static readonly char[] Slashes = new char[2]
    {
      '/',
      '\\'
    };

    public static bool HasExtension(this Uri url, string extension)
    {
      if (!url.IsAbsoluteUri)
        return false;
      return url.LocalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
    }

    public static string GetExtension(this Uri url)
    {
      if (!url.IsAbsoluteUri)
        return (string) null;
      string localPath = url.LocalPath;
      int startIndex = localPath.LastIndexOf('.');
      if (startIndex <= 0 || startIndex + 1 == localPath.Length || localPath.LastIndexOfAny(UriExtensions.Slashes) >= startIndex)
        return (string) null;
      return localPath.Substring(startIndex);
    }

    public static string GetExtension(string path)
    {
      if (string.IsNullOrEmpty(path))
        return (string) null;
      return Path.GetExtension(path);
    }
  }
}
