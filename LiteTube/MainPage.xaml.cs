using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Windows.Threading;
using LiteTube.Common;
using LiteTube.Common.Helpers;

namespace LiteTube
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
        }

        // Load data for the ViewModel Items
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                await App.ViewModel.LoadData();

                var userId = SettingsHelper.GetRefreshToken();
                if (string.IsNullOrEmpty(userId))
                    return;

                await App.ViewModel.DataSource.LoginSilently(string.Empty);
            }
        }

        private void Find_Click(object sender, EventArgs e)
        {
            NavigationHelper.GoToFindPage();
        }
    }
}