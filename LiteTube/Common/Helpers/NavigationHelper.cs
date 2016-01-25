using LiteTube.ViewModels;
using Microsoft.Phone.Shell;
using System.Linq;
using System.Windows.Controls;

namespace LiteTube.Common.Helpers
{
    static class NavigationHelper
    {
        public static void GoHome()
        {
            var list = App.RootFrame.BackStack.ToList();
            for (int i = 0; i < list.Count - 1; i++)
            {
                App.RootFrame.RemoveBackEntry();
            }

            if (App.RootFrame.CanGoBack)
                App.RootFrame.GoBack();
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
            var datasource = App.ViewModel.DataSource;
            NavigationHelper.Navigate("/SearchPage.xaml", new SearchPageViewModel(datasource));
        }

        public static bool Contains(string url)
        {
            var list = App.RootFrame.BackStack.ToList();
            return list.Any(journalEntry => journalEntry.Source.OriginalString.Contains(url));
        }
    }
}
