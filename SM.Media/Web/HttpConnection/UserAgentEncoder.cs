// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnection.UserAgentEncoder
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Web;

namespace SM.Media.Web.HttpConnection
{
  public class UserAgentEncoder : IUserAgentEncoder
  {
    private readonly string _userAgent;

    public string UsAsciiUserAgent
    {
      get
      {
        return this._userAgent;
      }
    }

    public UserAgentEncoder(IUserAgent userAgent)
    {
      this._userAgent = Rfc2047Encoding.Rfc2047Encode(userAgent.Name.Trim().Replace(' ', '_')) + "/" + Rfc2047Encoding.Rfc2047Encode(userAgent.Version.Trim().Replace(' ', '_'));
    }
  }
}
