using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LiteTube.Common.Tools;
using LiteTube.Resources;
using Microsoft.Phone.Controls;
using System.Windows.Threading;
using LiteTube.Common;
using LiteTube.Common.Helpers;
using Microsoft.Phone.Shell;

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

            var currentApplicationBar = new ApplicationBar();
            currentApplicationBar.Mode = ApplicationBarMode.Minimized;
            currentApplicationBar.Buttons.Add(ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Refresh.png", AppResources.Refresh, Refresh_Click));
            ApplicationBar = currentApplicationBar;
        }

        // Load data for the ViewModel Items
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //var appObject = Application.Current as App;
            //if (appObject == null)
            //    return;

            //if (appObject.WabContinuationArgs != null)
            //{
            //    await App.ViewModel.Login(appObject.WabContinuationArgs);
            //    appObject.WabContinuationArgs = null;
            //}


            if (App.ViewModel.IsDataLoaded) 
                return;
            
            if (SettingsHelper.IsContainsAuthorizationData())
                //загрузка произойдет когда прийдет нотификация об изменении состояния контекста
                await App.ViewModel.GetGeDataSource().LoginSilently(string.Empty);
            else
                await App.ViewModel.LoadData();    
        }

        private async void Refresh_Click(object sender, EventArgs e)
        {
            await App.ViewModel.ReloadData();
        }
    }
}