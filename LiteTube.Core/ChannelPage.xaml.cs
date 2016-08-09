using System;
using System.ComponentModel;
using System.Windows.Navigation;
using LiteTube.Core.Common.Helpers;
using LiteTube.Core.Common.Tools;
using LiteTube.Core.Resources;
using LiteTube.Core.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LiteTube.Core
{
    public partial class ChannelPage : PhoneApplicationPage
    {
        private readonly ApplicationBarIconButton _subscribeButton;
        private readonly ApplicationBarIconButton _unsubscribeButton;

        public ChannelPage()
        {
            InitializeComponent();

            _subscribeButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Subscribe.png", AppResources.Subscribe, Subscribe_Click);
            _unsubscribeButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Unsubscribe.png", AppResources.Unsubscribe, Unsubscribe_Click);
        }

        private ChannelPageViewModel ViewModel
        {
            get { return DataContext as ChannelPageViewModel; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(this);

            if (ViewModel == null)
                return;

            ClearAppBar();
            AddAppBarButtons();
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ClearAppBar();
            base.OnNavigatedFrom(e);
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ViewModel == null)
                return;

            if (e.PropertyName == "Description")
            {
                if (string.IsNullOrEmpty(ViewModel.Description))
                    return;

                HyperlinkHighlighter.HighlightUrls(ViewModel.Description, DescriptionRcTbx);
                return;
            }

            if (e.PropertyName != "IsSubscribed")
                return;

            ClearAppBar();
            AddAppBarButtons();
        }

        private void AddAppBarButtons()
        {
            if (!ViewModel.NavigationPanelViewModel.IsAuthorized)
                return;

            if (ViewModel.IsSubscribed)
            {
                ApplicationBar.Buttons.Add(_unsubscribeButton);
                return;
            }

            ApplicationBar.Buttons.Add(_subscribeButton);
        }

        private void ClearAppBar()
        {
            ApplicationBar.Buttons.Remove(_subscribeButton);
            ApplicationBar.Buttons.Remove(_unsubscribeButton);
        }

        private void Home_Click(object sender, EventArgs e)
        {
            NavigationHelper.GoHome();
        }

        private void Subscribe_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as ChannelPageViewModel;
            if (viewModel == null)
                return;

            viewModel.SubscribeCommand.Execute(null);
        }

        private void Unsubscribe_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as ChannelPageViewModel;
            if (viewModel == null)
                return;

            viewModel.UnsubscribeCommand.Execute(null);
        }
    }
}