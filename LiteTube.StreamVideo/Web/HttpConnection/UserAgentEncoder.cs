namespace LiteTube.StreamVideo.Web.HttpConnection
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
