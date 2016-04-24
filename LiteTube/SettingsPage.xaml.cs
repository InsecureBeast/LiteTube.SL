using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using LiteTube.ViewModels;
using LiteTube.Common.Helpers;
using Microsoft.Phone.Shell;
using LiteTube.Common.Tools;
using LiteTube.Resources;

namespace LiteTube
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
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

        private void Cancel_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as SettingsViewModel;
            if (viewModel == null)
                return;

            viewModel.Cancel();
            NavigationService.GoBack();
        }

        private void BuildLocalizedApplicationBar()
        {
            var appBar = new ApplicationBar();
            var saveButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Save.png", AppResources.Save, Save_Click);
            appBar.Buttons.Add(saveButton);
            var cancelButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Cancel.png", AppResources.Cancel, Cancel_Click);
            appBar.Buttons.Add(cancelButton);

            ApplicationBar = appBar;
        }
    }
}