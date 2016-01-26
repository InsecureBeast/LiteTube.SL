using LiteTube.DataModel;
using LiteTube.Controls;
using System.Diagnostics;
using System.Collections.ObjectModel;
using LiteTube.Common;
using System.Windows.Input;
using System;
using System.Linq;
using System.Collections.Specialized;
using Microsoft.Phone.Shell;
using MyToolkit.Command;
using LiteTube.ViewModels.Nodes;
using Windows.Phone.UI.Input;
using LiteTube.Common.Helpers;

namespace LiteTube.ViewModels
{
    class MenuPageViewModel : PropertyChangedBase, IListener<ConnectionEventArgs>
    {
        private readonly IDataSource _dataSource;
        private readonly ConnectionListener _connectionListener;
        private readonly RecommendedSectionViewModel _recommendedSectionViewModel;
        private readonly NavigationPanelViewModel _navigatioPanelViewModel;
        private readonly SubscriptionChannelsViewModel _subscriptions;
        private readonly HistoryPageViewModel _history;
        private readonly FavoritesViewModel _favoritesViewModel;
        private readonly ObservableCollection<GuideCategoryNodeViewModel> _categories;

        private readonly Common.RelayCommand _selectCommand;
        private readonly Common.RelayCommand _deleteCommand;
        private readonly RelayCommand<NavigationObject> _categoryCommand;

        private int _selectedIndex;
        private bool _isConnected;
        //private PreventNavigationHelper _navigationHelper;

        public MenuPageViewModel(int index, IDataSource dataSource, ConnectionListener connectionListener)
        {
            _dataSource = dataSource;
            _connectionListener = connectionListener;
            _connectionListener.Subscribe(this);
            _navigatioPanelViewModel = new NavigationPanelViewModel(_dataSource, connectionListener);
            _recommendedSectionViewModel = new RecommendedSectionViewModel(dataSource, connectionListener);
            _subscriptions = new SubscriptionChannelsViewModel(dataSource, connectionListener);
            _history = new HistoryPageViewModel(dataSource, connectionListener);
            _favoritesViewModel = new FavoritesViewModel(dataSource, connectionListener);
            _favoritesViewModel.SelectedItems.CollectionChanged += SelectedItemsCollectionChanged;
            _categories = new ObservableCollection<GuideCategoryNodeViewModel>();

            _selectCommand = new Common.RelayCommand(SelectItems);
            _deleteCommand = new Common.RelayCommand(DeleteItems, CanDelete);
            _categoryCommand = new RelayCommand<NavigationObject>(CategoryLoad);

            SelectedIndex = index;

            _isConnected = ConnectionListener.CheckNetworkAvailability();
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
            get { return _dataSource.IsAuthorized; }
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

        public IDataSource DataSource
        {
            get { return _dataSource; }
        }

        public bool IsFavoritesSelectedVisible
        {
            get { return _favoritesViewModel.IsItemClickEnabled && _selectedIndex == 2; }
        }

        private void OnSelectedIndexChanged(int index)
        {
            _favoritesViewModel.SetNonSelected();

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
                    Debug.WriteLine("history");
                    await HistoryPageViewModel.FirstLoad();
                    break;

                case 4:
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

            LayoutHelper.InvokeFromUIThread(async () =>
            {
                var sections = await _dataSource.GetGuideCategories();
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
                await _dataSource.RemoveFromFavorites(item.Id);
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
            var viewModel = new ChannelListPageViewModel(id, title, _dataSource, _connectionListener);
            NavigationHelper.Navigate(view, viewModel);
        }

        public void Notify(ConnectionEventArgs e)
        {
            LayoutHelper.InvokeFromUIThread(async () =>
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
