using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.Web
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
