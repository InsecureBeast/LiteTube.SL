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

                if (!SettingsHelper.IsContainsAuthorizationData())
                    return;

                await App.ViewModel.GetGeDataSource().LoginSilently(string.Empty);
            }
        }
    }
}