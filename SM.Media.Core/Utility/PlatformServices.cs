using System;

namespace SM.Media.Core.Utility
{
  public static class PlatformServicesExtensions
  {
    public static void GetSecureRandom(this IPlatformServices platformServices, ulong[] output)
    {
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      if (null == output)
        throw new ArgumentNullException("output");
      byte[] bytes = new byte[output.Length * 8];
      platformServices.GetSecureRandom(bytes);
      Buffer.BlockCopy((Array) bytes, 0, (Array) output, 0, bytes.Length);
    }

    public static void GetSecureRandom(this IPlatformServices platformServices, uint[] output)
    {
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      if (null == output)
        throw new ArgumentNullException("output");
      byte[] bytes = new byte[output.Length * 4];
      platformServices.GetSecureRandom(bytes);
      Buffer.BlockCopy((Array) bytes, 0, (Array) output, 0, bytes.Length);
    }
  }
}
