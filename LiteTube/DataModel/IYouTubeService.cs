using Google.Apis.YouTube.v3;
using System.Threading.Tasks;

namespace LiteTube.DataModel
{
    public interface IYouTubeService
    {
        string ApiKey { get; }
        YouTubeService GetService();
        YouTubeService GetAuthorizedService();
        Task Login();
        Task RefreshToken(string username);
        void Logout();
        bool IsAuthorized { get; }
        string OAuthToken { get; }
    }
}
