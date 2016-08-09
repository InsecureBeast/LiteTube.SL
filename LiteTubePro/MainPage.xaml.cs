using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LiteTube.Core;
using LiteTube.Core.Common.Helpers;
using LiteTube.Core.Resources;
using LiteTube.Core.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LiteTubePro
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = LiteTubeApp.ViewModel;

            var currentApplicationBar = new ApplicationBar();
            currentApplicationBar.Mode = ApplicationBarMode.Minimized;
            currentApplicationBar.Buttons.Add(ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Refresh.png", AppResources.Refresh, Refresh_Click));
            ApplicationBar = currentApplicationBar;
        }

        // Load data for the ViewModel Items
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            if (viewModel == null)
                return;

            if (viewModel.IsDataLoaded)
                return;

            if (SettingsHelper.IsContainsAuthorizationData())
                //загрузка произойдет когда прийдет нотификация об изменении состояния контекста
                await viewModel.GetDataSource().LoginSilently(string.Empty);
            else
                await viewModel.LoadData();
        }

        private async void Refresh_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            if (viewModel == null)
                return;

            await viewModel.ReloadData();
        }
    }
    /*
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
     */
}