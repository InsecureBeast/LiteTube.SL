using LiteTube.ViewModels;
using Microsoft.Phone.Shell;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;

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

            var model = PhoneApplicationService.Current.State["model"];
            page.DataContext = model;
            PhoneApplicationService.Current.State["model"] = null;
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
    }
}
