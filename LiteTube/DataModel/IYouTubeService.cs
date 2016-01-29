using Google.Apis.YouTube.v3;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace LiteTube.DataModel
{
    interface IYouTubeService
    {
        string ApiKey { get; }
        YouTubeService GetService();
        YouTubeService GetAuthorizedService();
        void Logout();
        bool IsAuthorized { get; }
        void Login();
        Task<string> ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args, string username);
        Task RefreshToken(string username);
        string OAuthToken { get; }
    }
}
