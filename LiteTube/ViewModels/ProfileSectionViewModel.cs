using LiteTube.DataModel;
using MyToolkit.Command;
using System.Windows.Input;
using System;
using System.Windows;
using Microsoft.Phone.Shell;
using LiteTube.Common;

namespace LiteTube.ViewModels
{
    public class ProfileSectionViewModel : PropertyChangedBase, IListener<UpdateContextEventArgs>
    {
        private readonly RelayCommand<FrameworkElement> _subscriptionsCommand;
        private readonly RelayCommand<FrameworkElement> _historyCommand;
        private readonly RelayCommand<FrameworkElement> _recommendedCommand;
        private readonly RelayCommand<FrameworkElement> _videoCategoryCommand;
        private readonly RelayCommand<FrameworkElement> _favoritesCommand;
        private readonly Func<IDataSource> _getDatasource;
        private readonly IConnectionListener _connectionListener;

        public ProfileSectionViewModel(Func<IDataSource> getDatasource, IConnectionListener connectionListener)
        {
            _getDatasource = getDatasource;
            _connectionListener = connectionListener;
            _subscriptionsCommand = new RelayCommand<FrameworkElement>(Subscriptions);
            _historyCommand = new RelayCommand<FrameworkElement>(GetHistory);
            _recommendedCommand = new RelayCommand<FrameworkElement>(Recommended);
            _videoCategoryCommand = new RelayCommand<FrameworkElement>(VideoCategories);
            _favoritesCommand = new RelayCommand<FrameworkElement>(Favorites);

            _getDatasource().Subscribe(this);
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
            get { return _getDatasource().IsAuthorized; }
        }

        private void Recommended(FrameworkElement control)
        {
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(0, _getDatasource, _connectionListener);
            App.NavigateTo("/MenuPage.xaml?item=0");
        }

        private void Subscriptions(FrameworkElement control)
        {
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(1, _getDatasource, _connectionListener);
            App.NavigateTo("/MenuPage.xaml?item=1");
        }

        private void GetHistory(FrameworkElement control)
        {
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(3, _getDatasource, _connectionListener);
            App.NavigateTo("/MenuPage.xaml?item=3");
        }

        private void VideoCategories(FrameworkElement control)
        {
            var index = IsAuthorized ? 4 : 0;
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(index, _getDatasource, _connectionListener);
            App.NavigateTo(string.Format("/MenuPage.xaml?item={0}", index));
        }

        private void Favorites(FrameworkElement control)
        {
            var index = 2;
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(index, _getDatasource, _connectionListener);
            App.NavigateTo(string.Format("/MenuPage.xaml?item={0}", index));
            ////var index = IsAuthorized ? 2 : 0; //TODO favorites saved on device!!
        }

        public void Notify(UpdateContextEventArgs e)
        {
            NotifyOfPropertyChanged(() => IsAuthorized);
        }
    }
}
