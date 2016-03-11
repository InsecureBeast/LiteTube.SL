using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Linq;
using System.Collections.Specialized;
using Microsoft.Phone.Shell;
using Windows.Phone.UI.Input;
using LiteTube.Common.Helpers;
using LiteTube.DataModel;
using MyToolkit.Command;
using LiteTube.Common;

namespace LiteTube.ViewModels
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
        private readonly ObservableCollection<GuideCategoryNodeViewModel> _categories;

        private readonly Common.RelayCommand _selectCommand;
        private readonly Common.RelayCommand _deleteCommand;
        private readonly RelayCommand<NavigationObject> _categoryCommand;

        private int _selectedIndex;
        private bool _isConnected = true;
        private ProgressIndicator _progressIndicator;
        //private PreventNavigationHelper _navigationHelper;

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
                _likedViewModel = new LikedViewModel(_getDataSource, connectionListener);
                _favoritesViewModel.SelectedItems.CollectionChanged += SelectedItemsCollectionChanged;
            }
            
            _selectCommand = new Common.RelayCommand(SelectItems);
            _deleteCommand = new Common.RelayCommand(DeleteItems, CanDelete);
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
                    Debug.WriteLine("recommended");
                    if (RecommendedSectionViewModel.Items.Count > 0)
                        return;

                    await RecommendedSectionViewModel.FirstLoad();
                    break;

                case 1:
                    Debug.WriteLine("subscriptions");
                    await SubscriptionChannelsViewModel.FirstLoad();
                    break;

                case 2:
                    Debug.WriteLine("favorites");
                    await FavoritesViewModel.FirstLoad();
                    break;

                case 3:
                    Debug.WriteLine("liked");
                    await LikedViewModel.FirstLoad();
                    break;

                case 4:
                    Debug.WriteLine("history");
                    await HistoryPageViewModel.FirstLoad();
                    break;

                case 5:
                    Debug.WriteLine("Video categories");
                    LoadCategories();
                    break;
            }

            NotifyOfPropertyChanged(() => IsFavoritesSelectedVisible);
        }

        private void SetSelection(int index)
        {
            switch (index)
            {
                case 0:
                    Debug.WriteLine("Video categories");
                    LoadCategories();
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

        private void LoadCategories()
        {
            if (_categories.Count > 0)
                return;

            LayoutHelper.InvokeFromUiThread(async () =>
            {
                var sections = await _getDataSource().GetGuideCategories();
                if (sections == null)
                    return;
                
                _categories.Clear();
                foreach (var section in sections)
                {
                    _categories.Add(new GuideCategoryNodeViewModel(section));
                }
            });
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
            HardwareButtons.BackPressed += OnBackPressed;
        }

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
            NavigationHelper.Navigate(view, viewModel);
        }

        public void Notify(ConnectionEventArgs e)
        {
            LayoutHelper.InvokeFromUiThread(() =>
            {
                IsConnected = e.IsConnected;

                if (e.IsConnected)
                {
                    LoadCategories();
                    return;
                }

                if (Categories.Count > 0)
                    IsConnected = true;
            });
        }
    }
}
