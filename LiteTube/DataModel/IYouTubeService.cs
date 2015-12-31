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
        Task Logout();
        bool IsAuthorized { get; }
#if WINDOWS_PHONE_APP
        void Login();
        Task<string> ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args, string username);
#else
        Task<string> Login(string username);
#endif
        Task RefreshToken(string username);
        string OAuthToken { get; }
    }
}
