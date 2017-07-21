using System;
using System.IO;

namespace SM.Media.Core.Web
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
