using System;
using System.Windows.Input;
using LiteTube.Common;
using LiteTube.DataModel;
using System.Windows.Controls;
using MyToolkit.Command;
using Microsoft.Phone.Shell;

namespace LiteTube.ViewModels
{
    public class NavigationPanelViewModel : PropertyChangedBase
    {
        private readonly IDataSource _datasource;
        private readonly RelayCommand<Page> _loginCommand;
        private readonly RelayCommand<Page> _logoutCommand;
        private readonly RelayCommand<Page> _homeCommand;
        private readonly RelayCommand<Page> _settingsCommand;
        private readonly Common.RelayCommand _subscriptionsCommand;
        private readonly RelayCommand<Page> _historyCommand;
        private readonly RelayCommand<object> _searchCommand;
        private readonly RelayCommand<Page> _recommendedCommand;
        private bool _isHomeSelected = false;
        private bool _isMenuSelected = false;
        private bool _isSettingsSelected = false;
        private bool _isSubscribtionSelected = false;
        private bool _isHistorySelected = false;
        private string _profileImage;
        private string _profileDisplayName;
        private bool _isProfileChecked;
        private string _profileRegistered;
        private string _profileSecondDisplayName;

        public NavigationPanelViewModel(IDataSource datasource)
        {
            _datasource = datasource;
            _loginCommand = new RelayCommand<Page>(Login);
            _logoutCommand = new RelayCommand<Page>(Logout);
            _homeCommand = new RelayCommand<Page>(Home);
            _settingsCommand = new RelayCommand<Page>(Settings, CanSettings);
            _subscriptionsCommand = new Common.RelayCommand(Subscriptions);
            _historyCommand = new RelayCommand<Page>(GetHistory);
            _searchCommand = new RelayCommand<object>(Search);
            _recommendedCommand = new RelayCommand<Page>(Recommended);

            datasource.ContextUpdated += OnContextUpdated;
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

        public ICommand SubsribtionsCommand
        {
            get { return _subscriptionsCommand; }
        }

        public ICommand HistoryCommand
        {
            get { return _historyCommand; }
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        public ICommand RecommendedCommand
        {
            get { return _recommendedCommand; }
        }

        public IDataSource DataSource
        {
            get { return _datasource; }
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

        public bool IsHomeSelected
        {
            get { return _isHomeSelected; }
            set
            {
                _isHomeSelected = value;
                NotifyOfPropertyChanged(() => IsHomeSelected);
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

        public bool IsHistorySelected
        {
            get { return _isHistorySelected; }
            set
            {
                _isHistorySelected = value;
                NotifyOfPropertyChanged(() => IsHistorySelected);
            }
        }

        public bool IsAuthorized
        {
            get { return _datasource.IsAuthorized; }
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
                if (_isProfileChecked)
                {
                    LoadProfileInfo();
                }
            }
        }

        private void Login(Page page)
        {
            _datasource.Login();
            Home(page);
        }

        private void Logout(Page page)
        {
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                await _datasource.Logout();
                Home(page);
            });
        }

        private void Home(Page page)
        {
            if (page == null)
                return;

            //_page.Frame.Navigate(typeof(HubPage), new HubPageViewModel(_datasource));
        }

        private void Settings(Page page)
        {
            if (page == null)
                return;

            //_page.Frame.Navigate(typeof(SettingsPage), new SettingsViewModel(_datasource));
        }

        private bool CanSettings(Page page)
        {
            return !_isSettingsSelected;
        }

        private void Subscriptions()
        {
            //_page.Frame.Navigate(typeof(HubPage), new SubscribtionsPageViewModel(_datasource));
        }

        private void GetHistory(Page page)
        {
            if (page == null)
                return;

            //_page.Frame.Navigate(typeof(SectionPage), new HistoryPageViewModel(_datasource));
        }

        private void Search(object page)
        {
            PhoneApplicationService.Current.State["searchModel"] = new SearchPageViewModel(_datasource);
            App.RootFrame.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void OnContextUpdated(object sender, EventArgs e)
        {
            NotifyOfPropertyChanged(() => IsAuthorized);
        }

        private void Recommended(Page page)
        {
            if (page == null)
                return;

            //_page.Frame.Navigate(typeof(PivotPage), new PivotPageViewModel(0, _datasource));
        }

        private void LoadProfileInfo()
        {
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                ProfileImage = null;
                ProfileDisplayName = string.Empty;
                ProfileSecondDisplayName = string.Empty;

                var profile = await _datasource.GetProfile();
                ProfileImage = profile.Image;
                var names = profile.DisplayName.Split(' ');
                if (names.Length >= 1)
                    ProfileDisplayName = names[0];
                if (names.Length >= 2)
                    ProfileSecondDisplayName = names[1];
                if (profile.Registered != null)
                    ProfileRegistered = profile.Registered.Value.ToString("d");
            });
        }
    }
}
