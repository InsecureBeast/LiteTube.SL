using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LiteTube.ViewModels;
using LiteTube.Common.Helpers;

namespace LiteTube
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(this);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as SettingsViewModel;
            if (viewModel == null)
                return;

            viewModel.Save();
            NavigationService.GoBack();
        }
    }
}