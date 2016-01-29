using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using LiteTube.Common;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Security.Authentication.Web;

namespace LiteTube.DataModel
{
    class YouTubeServiceControl : IYouTubeService
    {
        private const string SECRET = "OWnSxF8rkgle-KhsKNCI8yNF";
        private const string API_KEY = "AIzaSyDWezZRg-dUvNumjTow51ShtIgLA642whM";
        private const string APP_NAME = "LiteTube";
        private const string CLIENT_ID = "936038716924-oj9advoucgt9flrh07du3ovqsp6tlur1.apps.googleusercontent.com";
        private const string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";
        private YouTubeService _youTubeService;
        private YouTubeService _youTubeServiceAuth;
        private UserCredential _credential;

        public YouTubeServiceControl()
        {
            _youTubeService = GetYTService();
        }

        public string ApiKey
        {
            get { return API_KEY; }
        }

        public string OAuthToken
        {
            get
            {
                if (_credential == null)
                    return string.Empty;

                return _credential.Token.AccessToken;
            }
        }

        public YouTubeService GetService()
        {
            return _youTubeService;
        }

        public YouTubeService GetAuthorizedService()
        {
            if (_youTubeServiceAuth == null || _credential == null)
                throw new Exception("Authorization requered");
            
            return _youTubeServiceAuth;
        }

        public void Logout()
        {
            if (_credential == null) 
                return;
            
            //await _credential.RevokeTokenAsync(new CancellationToken());
            _credential = null;
            SettingsHelper.SaveUserRefreshToken(string.Empty);
            SettingsHelper.SaveUserAccessToken(string.Empty);
            _youTubeService = GetYTService();
        }

        public void Login()
        {
            try
            {
                var googleURL = "https://accounts.google.com/o/oauth2/auth?client_id=" + Uri.EscapeDataString(CLIENT_ID) + "&redirect_uri=" + Uri.EscapeDataString(REDIRECT_URI) + "&response_type=code&scope=" + Uri.EscapeDataString("https://www.googleapis.com/auth/youtube");
                var startUri = new Uri(googleURL);
                //var endUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
                // When using the desktop flow, the success code is displayed in the html title of this end uri
                var endUri = new Uri("https://accounts.google.com/o/oauth2/approval?");
                WebAuthenticationBroker.AuthenticateAndContinue(startUri, endUri, null, WebAuthenticationOptions.UseTitle);
            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                //
                Debug.WriteLine(Error.ToString());
            }
        }

        public async Task<string> ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args, string username)
        {
            //await PasswordVaultDataStore.Default.StoreAsync<SerializableWebAuthResult>
            //(
            //    SerializableWebAuthResult.Name,
            //    new SerializableWebAuthResult(args.WebAuthenticationResult)
            //);

		    var userId = await Authorize(username);

            //await PasswordVaultDataStore.Default.DeleteAsync<SerializableWebAuthResult>(SerializableWebAuthResult.Name);
            return userId;
        }

        private async Task<string> Authorize(string username)
        {
            _credential = await GetUserCredential(username);
            _youTubeServiceAuth = GetYTService(_credential);
            SettingsHelper.SaveUserRefreshToken(_credential.Token.RefreshToken);
            SettingsHelper.SaveUserAccessToken(_credential.Token.AccessToken);
            return _credential.UserId;
        }

        public bool IsAuthorized
        {
            get { return _credential != null; }
        }

        private async Task<UserCredential> GetUserCredential(string username)
        {
            if (string.IsNullOrEmpty(username))
                username = "user";

           var clientSecrets = new ClientSecrets
            {
                ClientId = CLIENT_ID,
                ClientSecret = SECRET
            };

            var scope = new List<string>() { YouTubeService.Scope.YoutubeForceSsl };
            return await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, scope, username, CancellationToken.None);
        }

        private YouTubeService GetYTService(UserCredential credential = null)
        {
            return new YouTubeService(new BaseClientService.Initializer()
            {
                ApplicationName = APP_NAME,
                ApiKey = API_KEY,
                HttpClientInitializer = credential
            });
        }

        public async Task RefreshToken(string username)
        {
            if (string.IsNullOrEmpty(username))
                username = "user";

            var token = new TokenResponse
            {
                AccessToken = SettingsHelper.GetAccessToken(),
                RefreshToken = SettingsHelper.GetRefreshToken()
            };
            var clientSecrets = new ClientSecrets
            {
                ClientId = CLIENT_ID,
                ClientSecret = SECRET
            };

            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { YouTubeService.Scope.YoutubeForceSsl, YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeUpload }
            };

            var flow = new GoogleAuthorizationCodeFlow(initializer);
            _credential = new UserCredential(flow, username, token);
            Thread.Sleep(500);
            var res = await _credential.RefreshTokenAsync(CancellationToken.None);
            _youTubeServiceAuth = GetYTService(_credential);
            SettingsHelper.SaveUserRefreshToken(_credential.Token.RefreshToken);
            SettingsHelper.SaveUserAccessToken(_credential.Token.AccessToken);
        }
    }
}
