using LiteTube.Common.Tools;
using LiteTube.ViewModels;
using Microsoft.Phone.Shell;
using System.Linq;
using System.Windows.Controls;
using LiteTube.ViewModels.Playlist;

namespace LiteTube.Common.Helpers
{
    static class NavigationHelper
    {
        public static void GoHome()
        {
            App.NavigateTo("/MainPage.xaml");
        }

        public static void Navigate(string uri, object viewModel)
        {
            PhoneApplicationService.Current.State["model"] = viewModel;
            App.NavigateTo(uri);
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
            var datasource = App.ViewModel.GetDataSource;
            var connectionListener = App.ViewModel.ConnectionListener;
            NavigationHelper.Navigate("/SearchPage.xaml", new SearchPageViewModel(datasource, connectionListener));
        }

        public static bool Contains(string url)
        {
            var list = App.RootFrame.BackStack.ToList();
            return list.Any(journalEntry => journalEntry.Source.OriginalString.Contains(url));
        }

        public static void GoToVideoPage(string videoId)
        {
            //Log
            LastRequest.VideoId = videoId; 

            var datasource = App.ViewModel.GetDataSource;
            var connectionListener = App.ViewModel.ConnectionListener;
            NavigationHelper.Navigate("/VideoPage.xaml?videoId=" + videoId, new VideoPageViewModel(videoId, datasource, connectionListener));
        }

        public static void GoToPLaylistPage(string playlistId)
        {
            LastRequest.PlaylistId = playlistId;
            var datasource = App.ViewModel.GetDataSource;
            var connectionListener = App.ViewModel.ConnectionListener;
            NavigationHelper.Navigate("/PlaylistVideoPage.xaml?playlistId=" + playlistId, new PlaylistVideoPageViewModel(playlistId, datasource, connectionListener));
        }

        public static void GoToChannelPage(string channelId, string username)
        {
            LastRequest.ChannelId = channelId;
            var datasource = App.ViewModel.GetDataSource;
            var connectionListener = App.ViewModel.ConnectionListener;
            NavigationHelper.Navigate("/ChannelPage.xaml?channelId=" + channelId, new ChannelPageViewModel(channelId, username, datasource, connectionListener));
        }

        public static void GoToPLaylistMangePage(IPlaylistsChangeHandler playlistsChangeHandler)
        {
            var datasource = App.ViewModel.GetDataSource;
            var connectionListener = App.ViewModel.ConnectionListener;
            NavigationHelper.Navigate("/PlaylistsManagePage.xaml", new PlaylistsManagePageViewModel(datasource, connectionListener, playlistsChangeHandler));
        }
    }
}
