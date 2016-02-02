﻿using Google.Apis.YouTube.v3;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace LiteTube.DataModel
{
    interface IYouTubeService
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
