namespace SM.Media.Core.Utility
{
  public class ApplicationInformation : IApplicationInformation
  {
    private readonly string _title;
    private readonly string _version;

    public string Title
    {
      get
      {
        return this._title;
      }
    }

    public string Version
    {
      get
      {
        return this._version;
      }
    }

    public ApplicationInformation(string title, string version)
    {
      this._title = title ?? "Unknown";
      this._version = version ?? "0.0";
    }
  }
}
