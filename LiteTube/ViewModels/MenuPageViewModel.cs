using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Linq;
using System.Collections.Specialized;
using LiteTube.Common.Helpers;
using LiteTube.DataModel;
using MyToolkit.Command;
using LiteTube.Common;
using System.Threading.Tasks;
using LiteTube.ViewModels.Playlist;
#if SILVERLIGHT
using Microsoft.Phone.Shell;
using Windows.Phone.UI.Input;
#endif

namespace LiteTube.ViewModels
{
    class MenuPageViewModel : PropertyChangedBase, IListener<ConnectionEventArgs>, IPlaylistsSevice
    {
        private readonly Func<IDataSource> _getDataSource;
        private readonly IConnectionListener _connectionListener;
        private readonly RecommendedSectionViewModel _recommendedSectionViewModel;
        private readonly NavigationPanelViewModel _navigatioPanelViewModel;
        private readonly SubscriptionChannelsViewModel _subscriptions;
        private readonly HistoryPageViewModel _history;
        private readonly LikedViewModel _likedViewModel;
        private readonly UploadedPageViewModel _uploadedPageViewModel;
        private readonly MyPlaylistListViewModel _myPlaylistListViewModel;
        private readonly ObservableCollection<GuideCategoryNodeViewModel> _categories;

        private readonly RelayCommand<NavigationObject> _categoryCommand;

        private int _selectedIndex;
        private bool _isConnected = true;

#if SILVERLIGHT
        private ProgressIndicator _progressIndicator;
        //private PreventNavigationHelper _navigationHelper;
#endif
        private bool _isRequestSend = false;

        public MenuPageViewModel(int index, Func<IDataSource> getGetDataSource, IConnectionListener connectionListener)
        {
            _getDataSource = getGetDataSource;
            _connectionListener = connectionListener;
            _connectionListener.Subscribe(this);
            _navigatioPanelViewModel = new NavigationPanelViewModel(_getDataSource, connectionListener);
            _categories = new ObservableCollection<GuideCategoryNodeViewModel>();

            if (_getDataSource().IsAuthorized)
            {
                _recommendedSectionViewModel = new RecommendedSectionViewModel(_getDataSource, connectionListener, this);
                _subscriptions = new SubscriptionChannelsViewModel(_getDataSource, connectionListener);
                _history = new HistoryPageViewModel(_getDataSource, connectionListener);
                _likedViewModel = new LikedViewModel(_getDataSource, connectionListener);
                _uploadedPageViewModel = new UploadedPageViewModel(_getDataSource, connectionListener);
                _myPlaylistListViewModel = new MyPlaylistListViewModel(_getDataSource, connectionListener);
            }
            
            _categoryCommand = new RelayCommand<NavigationObject>(CategoryLoad);

            SelectedIndex = index;

            _isConnected = connectionListener.CheckNetworkAvailability();
            //App.ViewModel.IndicatorHolder.Subscribe(() =>
            //{
            //    ProgressIndicator = App.ViewModel.IndicatorHolder.ProgressIndicator;
            //});
        }

        public ObservableCollection<GuideCategoryNodeViewModel> Categories
        {
            get { return _categories; }
        }

        public NavigationPanelViewModel NavigationPanelViewModel
        {
            get { return _navigatioPanelViewModel; }
        }

        public RecommendedSectionViewModel RecommendedSectionViewModel
        {
            get { return _recommendedSectionViewModel; }
        }

        public SubscriptionChannelsViewModel SubscriptionChannelsViewModel
        {
            get { return _subscriptions; }
        }

        public HistoryPageViewModel HistoryPageViewModel
        {
            get { return _history; }
        }

        public LikedViewModel LikedViewModel
        {
            get { return _likedViewModel; }
        }

        public UploadedPageViewModel UploadedPageViewModel
        {
            get { return _uploadedPageViewModel; }
        }

        public MyPlaylistListViewModel MyPlaylistListViewModel
        {
            get { return _myPlaylistListViewModel; }
        }

        public ICommand CategoryCommand
        {
            get { return _categoryCommand; }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnSelectedIndexChanged(_selectedIndex);
                NotifyOfPropertyChanged(() => SelectedIndex);
            }
        }

        public bool IsAuthorized
        {
            get { return _getDataSource().IsAuthorized; }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            protected set
            {
                _isConnected = value;
                NotifyOfPropertyChanged(() => IsConnected);
            }
        }
        
        public bool IsLoading
        {
            get { return false; }
        }

        public bool IsEmpty
        {
            get { return false; }
        }

#if SILVERLIGHT
        public ProgressIndicator ProgressIndicator
        {
            get { return _progressIndicator; }
            set
            {
                if (value == _progressIndicator)
                    return;

                _progressIndicator = value;
                NotifyOfPropertyChanged(() => ProgressIndicator);
            }
        }
#endif

        private void OnSelectedIndexChanged(int index)
        {
            if (IsAuthorized)
            {
                SetAuthorizedSelection(index);
                return;
            }

            SetSelection(index);
        }

        private async void SetAuthorizedSelection(int index)
        {
            switch (index)
            {
                case 0:
                    Debug.WriteLine("Video categories");
                    await LoadCategories();
                    break;

                case 1:
                    Debug.WriteLine("recommended");
                    if (RecommendedSectionViewModel.Items.Count > 0)
                        return;

                    await RecommendedSectionViewModel.FirstLoad();
                    break;

                case 2:
                    Debug.WriteLine("subscriptions");
                    await SubscriptionChannelsViewModel.FirstLoad();
                    break;

                case 3:
                    Debug.WriteLine("playlists");
                    await MyPlaylistListViewModel.FirstLoad();
                    break;

                case 4:
                    Debug.WriteLine("liked");
                    await LikedViewModel.FirstLoad();
                    break;

                case 5:
                    Debug.WriteLine("uploaded");
                    await UploadedPageViewModel.FirstLoad();
                    break;

                case 6:
                    Debug.WriteLine("history");
                    await HistoryPageViewModel.FirstLoad();
                    break;
            }
        }

        private async void SetSelection(int index)
        {
            switch (index)
            {
                case 0:
                    Debug.WriteLine("Video categories");
                    await LoadCategories();
                    break;
            }
        }

        private async Task LoadCategories()
        {
            if (_isRequestSend)
                return;

            if (_categories.Count > 0)
                return;

            _isRequestSend = true;
            var sections = await _getDataSource().GetGuideCategories();
            if (sections == null)
                return;
                
            _categories.Clear();
            foreach (var section in sections)
            {
                _categories.Add(new GuideCategoryNodeViewModel(section, _getDataSource()));
            }
            _isRequestSend = false;
        }

        private void CategoryLoad(NavigationObject navObject)
        {
            if (navObject == null)
                return;

            var item = (GuideCategoryNodeViewModel)navObject.ViewModel;
            var id = item.CategoryId;
            var title = item.Title;
            var view = string.Format("/ChannelListPage.xaml?categoriId={0}", id);
            var viewModel = new ChannelListPageViewModel(id, title, _getDataSource, _connectionListener);
#if SILVERLIGHT
            NavigationHelper.Navigate(view, viewModel);
#endif
        }

        public void Notify(ConnectionEventArgs e)
        {
            LayoutHelper.InvokeFromUiThread(async() =>
            {
                IsConnected = e.IsConnected;

                if (e.IsConnected)
                {
                    await LoadCategories();
                    return;
                }

                if (Categories.Count > 0)
                    IsConnected = true;
            });
        }

        public PlaylistsContainerViewModel PlaylistListViewModel
        {
            get { return App.ViewModel.PlaylistListViewModel; }
        }

        public void ShowContainer(bool show, string videoId)
        {
            PlaylistListViewModel.IsContainerShown = true;
            LayoutHelper.InvokeFromUiThread(async () =>
            {
                PlaylistListViewModel.SetVideoId(videoId);
                await PlaylistListViewModel.FirstLoad();
            });
        }
    }
}
