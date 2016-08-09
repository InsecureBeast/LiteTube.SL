using System.Linq;
using System.Windows.Controls;
using LiteTube.Core.Common.Tools;
using LiteTube.Core.ViewModels;
using LiteTube.Core.ViewModels.Playlist;
using LiteTube.Core.ViewModels.Search;
using Microsoft.Phone.Shell;

namespace LiteTube.Core.Common.Helpers
{
    public static class NavigationHelper
    {
        public static void GoHome()
        {
            LiteTubeApp.NavigateTo("/MainPage.xaml");
        }

        public static void Navigate(string uri, object viewModel)
        {
            PhoneApplicationService.Current.State["model"] = viewModel;
            LiteTubeApp.NavigateTo(uri);
        }

        public static void OnNavigatedTo(Page page)
        {
            if (page.DataContext != null)
                return;

            object model;
            if (PhoneApplicationService.Current.State.TryGetValue("model", out model))
            {
                page.DataContext = model;
                PhoneApplicationService.Current.State["model"] = null;
            }
        }

        public static void GoToFindPage()
        {
            var datasource = LiteTubeApp.ViewModel.GetDataSource;
            var connectionListener = LiteTubeApp.ViewModel.ConnectionListener;
            Navigate("/SearchPage.xaml", new SearchPageViewModel(datasource, connectionListener));
        }

        public static bool Contains(string url)
        {
            var list = LiteTubeApp.RootFrame.BackStack.ToList();
            return list.Any(journalEntry => journalEntry.Source.OriginalString.Contains(url));
        }

        public static void GoToVideoPage(string videoId)
        {
            //Log
            LastRequest.VideoId = videoId;

            var datasource = LiteTubeApp.ViewModel.GetDataSource;
            var connectionListener = LiteTubeApp.ViewModel.ConnectionListener;
            Navigate("/VideoPage.xaml?videoId=" + videoId, new VideoPageViewModel(videoId, datasource, connectionListener));
        }

        public static void GoToPLaylistPage(string playlistId)
        {
            LastRequest.PlaylistId = playlistId;
            var datasource = LiteTubeApp.ViewModel.GetDataSource;
            var connectionListener = LiteTubeApp.ViewModel.ConnectionListener;
            Navigate("/PlaylistVideoPage.xaml?playlistId=" + playlistId, new PlaylistVideoPageViewModel(playlistId, datasource, connectionListener));
        }

        public static void GoToChannelPage(string channelId, string username)
        {
            LastRequest.ChannelId = channelId;
            var datasource = LiteTubeApp.ViewModel.GetDataSource;
            var connectionListener = LiteTubeApp.ViewModel.ConnectionListener;
            Navigate("/ChannelPage.xaml?channelId=" + channelId, new ChannelPageViewModel(channelId, username, datasource, connectionListener));
        }

        public static void GoToMenuPage(int index)
        {
            var datasource = LiteTubeApp.ViewModel.GetDataSource;
            var connectionListener = LiteTubeApp.ViewModel.ConnectionListener;
            Navigate("/MenuPage.xaml?item=" + index, new MenuPageViewModel(index, datasource, connectionListener));
        }
    }
}
