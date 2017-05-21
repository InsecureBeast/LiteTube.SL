using System;
using System.Windows.Input;
using LiteTube.Common;
using LiteTube.DataModel;
using MyToolkit.Command;
using LiteTube.Common.Helpers;
using System.Threading;

namespace LiteTube.ViewModels
{
    public class NavigationPanelViewModel : PropertyChangedBase, IListener<UpdateContextEventArgs>
    {
        private readonly Func<IDataSource> _getDataSource;
        private readonly IConnectionListener _connectionListener;
        private readonly IPurchase _purchase;
        private readonly Common.RelayCommand _loginCommand;
        private readonly Common.RelayCommand _logoutCommand;
        private readonly Common.RelayCommand _homeCommand;
        private readonly Common.RelayCommand _settingsCommand;
        private readonly Common.RelayCommand _searchCommand;
        private readonly Common.RelayCommand _donateCommand;
        private readonly RelayCommand<string> _channelCommand;
        private bool _isMenuSelected = false;
        private bool _isSettingsSelected = false;
        private bool _isSubscribtionSelected = false;
        private string _profileImage;
        private string _profileDisplayName;
        private bool _isProfileChecked;
        private string _profileRegistered;
        private string _profileSecondDisplayName;
        private string _profileChannelId;

        public NavigationPanelViewModel(Func<IDataSource> getDataSource, IConnectionListener connectionListener, IPurchase purchase)
        {
            _getDataSource = getDataSource;
            _connectionListener = connectionListener;
            _purchase = purchase;
            _loginCommand = new Common.RelayCommand(Login);
            _logoutCommand = new Common.RelayCommand(Logout);
            _homeCommand = new Common.RelayCommand(Home);
            _settingsCommand = new Common.RelayCommand(Settings, CanSettings);
            _searchCommand = new Common.RelayCommand(Search);
            _channelCommand = new RelayCommand<string>(LoadChannel);
            _donateCommand = new Common.RelayCommand(ViewDonate);
        }

        public ICommand LoginCommand
        {
            get { return _loginCommand; }
        }

        public ICommand LogoutCommand
        {
            get { return _logoutCommand; }
        }

        public ICommand HomeCommand
        {
            get { return _homeCommand; }
        }

        public ICommand SettingsCommand
        {
            get { return _settingsCommand; }
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        public ICommand ChannelCommand
        {
            get { return _channelCommand; }
        }

        public ICommand DonateCommand
        {
            get { return _donateCommand; }
        }

        public bool IsMenuSelected
        {
            get { return _isMenuSelected; }
            set
            {
                _isMenuSelected = value;
                NotifyOfPropertyChanged(() => IsMenuSelected);
            }
        }

        public bool IsSettingsSelected
        {
            get { return _isSettingsSelected; }
            set
            {
                _isSettingsSelected = value;
                NotifyOfPropertyChanged(() => IsSettingsSelected);
                _settingsCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsSubscribtionSelected
        {
            get { return _isSubscribtionSelected; }
            set
            {
                _isSubscribtionSelected = value;
                NotifyOfPropertyChanged(() => IsSubscribtionSelected);
            }
        }

        public bool IsAuthorized
        {
            get { return _getDataSource().IsAuthorized; }
        }

        public string ProfileImage
        {
            get { return _profileImage; }
            set
            {
                _profileImage = value;
                NotifyOfPropertyChanged(() => ProfileImage);
            }
        }

        public string ProfileDisplayName
        {
            get { return _profileDisplayName; }
            set
            {
                _profileDisplayName = value;
                NotifyOfPropertyChanged(() => ProfileDisplayName);
            }
        }

        public string ProfileSecondDisplayName
        {
            get { return _profileSecondDisplayName; }
            set
            {
                _profileSecondDisplayName = value;
                NotifyOfPropertyChanged(() => ProfileSecondDisplayName);
            }
        }

        public string ProfileRegistered
        {
            get { return _profileRegistered; }
            set
            {
                _profileRegistered = value;
                NotifyOfPropertyChanged(() => ProfileRegistered);
            }
        }

        public bool IsProfileChecked
        {
            get { return _isProfileChecked; }
            set
            {
                _isProfileChecked = value;
                NotifyOfPropertyChanged(() => IsProfileChecked);
            }
        }

        public string ProfileChannelId
        {
            get { return _profileChannelId; }
            set
            {
                NotifyOfPropertyChanged(() => ProfileChannelId);
                _profileChannelId = value;
            }
        }

        public string Memory
        {
            get
            {
                var mem = Windows.System.MemoryManager.AppMemoryUsage / 1024 / 1024;
                return mem.ToString();
            }
        }

        public string TotalMemory
        {
            get
            {
                var mem = Windows.System.MemoryManager.AppMemoryUsageLimit / 1024 / 1024;
                return mem.ToString();
            }
        }

        private void Login()
        {
            _getDataSource().Login();
            Home();
        }

        private void Logout()
        {
            _getDataSource().Logout();
            Home();
        }

        private void Home()
        {
#if SILVERLIGHT
            NavigationHelper.GoHome();
#endif
        }

        private void Settings()
        {
#if SILVERLIGHT
            NavigationHelper.Navigate("/SettingsPage.xaml", new SettingsViewModel(_getDataSource, _connectionListener, _purchase));
#endif
        }

        private bool CanSettings()
        {
            return !_isSettingsSelected;
        }

        private void Search()
        {
#if SILVERLIGHT
            NavigationHelper.Navigate("/SearchPage.xaml", new SearchPageViewModel(_getDataSource, _connectionListener));
#endif
        }

        private void LoadChannel(string channelId)
        {
#if SILVERLIGHT
            NavigationHelper.GoToChannelPage(channelId, null);
#endif
        }

        public void Notify(UpdateContextEventArgs e)
        {
            NotifyOfPropertyChanged(() => IsAuthorized);
        }

        private void ViewDonate()
        {
            NavigationHelper.Navigate("/Donate.xaml", new DonateViewModel(_purchase));
        }

        private void Callback(object state)
        {
            LayoutHelper.InvokeFromUiThread(() => NotifyOfPropertyChanged(() => Memory));
        }
    }
}
