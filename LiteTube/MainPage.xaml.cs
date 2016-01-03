using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.ApplicationModel.Activation;
using Windows.Security.Authentication.Web;
using LiteTube.ViewModels;
using System.Windows.Threading;

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

            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Interval = new TimeSpan(0, 0, 0);
            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        // Load data for the ViewModel Items
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                await App.ViewModel.LoadData();
            }
        }

        private async void timer_Tick(object sender, object e)
        {
            _timer.Stop();
            await App.ViewModel.DataSource.LoginSilently(string.Empty);
        }
    }
}