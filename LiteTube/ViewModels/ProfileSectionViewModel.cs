using LiteTube.DataModel;
using MyToolkit.Command;
using System.Windows.Input;
using System;
using System.Windows;
using Microsoft.Phone.Shell;

namespace LiteTube.ViewModels
{
    public class ProfileSectionViewModel : PropertyChangedBase
    {
        private readonly RelayCommand<FrameworkElement> _subscriptionsCommand;
        private readonly RelayCommand<FrameworkElement> _historyCommand;
        private readonly RelayCommand<FrameworkElement> _recommendedCommand;
        private readonly RelayCommand<FrameworkElement> _videoCategoryCommand;
        private readonly RelayCommand<FrameworkElement> _favoritesCommand;
        private readonly IDataSource _datasource;

        public ProfileSectionViewModel(IDataSource datasource)
        {
            _datasource = datasource;
            _subscriptionsCommand = new RelayCommand<FrameworkElement>(Subscriptions);
            _historyCommand = new RelayCommand<FrameworkElement>(GetHistory);
            _recommendedCommand = new RelayCommand<FrameworkElement>(Recommended);
            _videoCategoryCommand = new RelayCommand<FrameworkElement>(VideoCategories);
            _favoritesCommand = new RelayCommand<FrameworkElement>(Favorites);

            datasource.ContextUpdated += OnContextUpdated;
        }

        public ICommand SubsribtionsCommand
        {
            get { return _subscriptionsCommand; }
        }

        public ICommand HistoryCommand
        {
            get { return _historyCommand; }
        }

        public ICommand RecommendedCommand
        {
            get { return _recommendedCommand; }
        }

        public ICommand VideoCategoryCommand
        {
            get { return _videoCategoryCommand; }
        }

        public ICommand FavoritesCommand
        {
            get { return _favoritesCommand; }
        }

        public bool IsAuthorized
        {
            get { return _datasource.IsAuthorized; }
        }

        private void Recommended(FrameworkElement control)
        {
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(0, _datasource);
            App.NavigateTo("/MenuPage.xaml?item=0");
        }

        private void Subscriptions(FrameworkElement control)
        {
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(1, _datasource);
            App.NavigateTo("/MenuPage.xaml?item=1");
        }

        private void GetHistory(FrameworkElement control)
        {
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(3, _datasource);
            App.NavigateTo("/MenuPage.xaml?item=3");
        }

        private void VideoCategories(FrameworkElement control)
        {
            var index = IsAuthorized ? 4 : 0;
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(index, _datasource);
            App.NavigateTo(string.Format("/MenuPage.xaml?item={0}", index));
        }

        private void Favorites(FrameworkElement control)
        {
            var index = 2;
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(index, _datasource);
            App.NavigateTo(string.Format("/MenuPage.xaml?item={0}", index));
            ////var index = IsAuthorized ? 2 : 0; //TODO favorites saved on device!!
        }

        private void OnContextUpdated(object sender, EventArgs e)
        {
            NotifyOfPropertyChanged(() => IsAuthorized);
        }
    }
}
