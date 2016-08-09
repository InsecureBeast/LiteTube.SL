using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteTube.Core.Common;
using LiteTube.Core.Common.Helpers;
using LiteTube.Core.Common.Notifier;
using LiteTube.Core.DataModel;
using LiteTube.Core.ViewModels.Nodes;
using LiteTube.Core.ViewModels.Playlist;
using Microsoft.Phone.Shell;
using MyToolkit.Command;
using RelayCommand = LiteTube.Core.Common.Commands.RelayCommand;
#if SILVERLIGHT

#endif

namespace LiteTube.Core.ViewModels
{
    class MenuPageViewModel : PropertyChangedBase, IListener<ConnectionEventArgs>
    {
        private readonly Func<IDataSource> _getDataSource;
        private readonly IConnectionListener _connectionListener;
        private readonly RecommendedSectionViewModel _recommendedSectionViewModel;
        private readonly NavigationPanelViewModel _navigatioPanelViewModel;
        private readonly SubscriptionChannelsViewModel _subscriptions;
        private readonly HistoryPageViewModel _history;
        private readonly FavoritesViewModel _favoritesViewModel;
        private readonly LikedViewModel _likedViewModel;
        private readonly UploadedPageViewModel _uploadedPageViewModel;
        private readonly MyPlaylistListViewModel _myPlaylistListViewModel;
        private readonly ObservableCollection<GuideCategoryNodeViewModel> _categories;

        private readonly RelayCommand _selectCommand;
        private readonly RelayCommand _deleteCommand;
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
                _recommendedSectionViewModel = new RecommendedSectionViewModel(_getDataSource, connectionListener);
                _subscriptions = new SubscriptionChannelsViewModel(_getDataSource, connectionListener);
                _history = new HistoryPageViewModel(_getDataSource, connectionListener);
                _favoritesViewModel = new FavoritesViewModel(_getDataSource, connectionListener);
                _favoritesViewModel.SelectedItems.CollectionChanged += SelectedItemsCollectionChanged;
                _likedViewModel = new LikedViewModel(_getDataSource, connectionListener);
                _uploadedPageViewModel = new UploadedPageViewModel(_getDataSource, connectionListener);
                _myPlaylistListViewModel = new MyPlaylistListViewModel(_getDataSource, connectionListener);
            }
            
            _selectCommand = new RelayCommand(SelectItems);
            _deleteCommand = new RelayCommand(DeleteItems, CanDelete);
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

        public FavoritesViewModel FavoritesViewModel
        {
            get { return _favoritesViewModel; }
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

        public ICommand SelectCommand
        {
            get { return _selectCommand; }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
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

        public bool IsFavoritesSelectedVisible
        {
            get { return _favoritesViewModel.IsItemClickEnabled && _selectedIndex == 2; }
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
                _favoritesViewModel.SetNonSelected();
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
                    Debug.WriteLine("favorites");
                    await FavoritesViewModel.FirstLoad();
                    break;

                case 5:
                    Debug.WriteLine("liked");
                    await LikedViewModel.FirstLoad();
                    break;

                case 6:
                    Debug.WriteLine("uploaded");
                    await UploadedPageViewModel.FirstLoad();
                    break;

                case 7:
                    Debug.WriteLine("history");
                    await HistoryPageViewModel.FirstLoad();
                    break;
            }

            NotifyOfPropertyChanged(() => IsFavoritesSelectedVisible);
        }

        private async void SetSelection(int index)
        {
            switch (index)
            {
                case 0:
                    Debug.WriteLine("Video categories");
                    await LoadCategories();
                    break;
                //case 1:
                //    Debug.WriteLine("subscriptions");
                //    break;

                //case 2:
                //    Debug.WriteLine("history");
                //    break;
                //case 3:
                //    break;
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
                _categories.Add(new GuideCategoryNodeViewModel(section));
            }
            _isRequestSend = false;
        }

        private async void DeleteItems()
        {
            _favoritesViewModel.SetNonSelected();
            NotifyOfPropertyChanged(() => IsFavoritesSelectedVisible);
            //_navigationHelper.IsCanGoBack = true;
            var items = _favoritesViewModel.SelectedItems;
            foreach (var item in items)
            {
                await _getDataSource().RemoveFromFavorites(item.Id);
                _favoritesViewModel.Items.Remove(item);
            }
        }

        private bool CanDelete()
        {
            return _favoritesViewModel.SelectedItems.Any();
        }

        private void SelectItems()
        {
            _favoritesViewModel.SetSelected();
            NotifyOfPropertyChanged(() => IsFavoritesSelectedVisible);
            //_navigationHelper.IsCanGoBack = false;
            //HardwareButtons.BackPressed += OnBackPressed;
        }
        
        /*
        private void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            if (!_favoritesViewModel.IsItemClickEnabled)
            {
                _favoritesViewModel.SetNonSelected();
                NotifyOfPropertyChanged(() => IsFavoritesSelectedVisible);
                e.Handled = true;
                HardwareButtons.BackPressed -= OnBackPressed;
                //_navigationHelper.IsCanGoBack = true;
            }
        }
        */

        private void SelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _deleteCommand.RaiseCanExecuteChanged();
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
    }
}
