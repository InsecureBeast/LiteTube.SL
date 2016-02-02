using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using LiteTube.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task Login()
        {
            await Authorize();
        }

        private async Task<string> Authorize()
        {
            _credential = await GetUserCredential();
            _youTubeServiceAuth = GetYTService(_credential);
            SettingsHelper.SaveUserRefreshToken(_credential.Token.RefreshToken);
            SettingsHelper.SaveUserAccessToken(_credential.Token.AccessToken);
            return _credential.UserId;
        }

        public bool IsAuthorized
        {
            get { return _credential != null; }
        }

        private async Task<UserCredential> GetUserCredential()
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = CLIENT_ID,
                ClientSecret = SECRET
            };

            var scope = new List<string>() { YouTubeService.Scope.YoutubeForceSsl, YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeUpload };
            return await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, scope, "user", CancellationToken.None);
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
