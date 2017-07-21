// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.UserAgent
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Utility;

namespace SM.Media.Web
{
  public class UserAgent : IUserAgent
  {
    private readonly string _name;
    private readonly string _version;

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public string Version
    {
      get
      {
        return this._version;
      }
    }

    public UserAgent(IApplicationInformation applicationInformation)
    {
      this._name = applicationInformation.Title ?? "Unknown";
      this._version = applicationInformation.Version ?? "0.0";
    }
  }
}
