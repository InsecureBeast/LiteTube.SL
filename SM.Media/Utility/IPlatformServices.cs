// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.PlatformServicesExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Utility
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
