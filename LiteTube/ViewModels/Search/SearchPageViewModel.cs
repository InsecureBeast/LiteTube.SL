using Google.Apis.YouTube.v3;
using LiteTube.Common;
using LiteTube.DataModel;
using LiteTube.Resources;
using LiteTube.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiteTube.ViewModels
{
    class SearchPageViewModel : ProgressIndicatorViewModel
    {
        private readonly SearchVideoViewModel _searchVideoViewModel;
        private readonly SearchChannelsViewModel _searchChannelsViewModel;
        private readonly SearchPlaylistsViewModel _searchPlaylistsViewModel;
        private readonly RelayCommand _searchSettingCommand;
        private string _searchString;
        private int _selectedIndex;

        private bool _isSearchSettingsVisible = false;
        private List<OrderSearchFilterItem> _orderItems;
        private List<UploadSearchFilterItem> _uploadItems;
        private List<VideoDurationSearchFilterItem> _durationItems;
        private List<VideoDefinitionSearchFilterItem> _definitionItems;
        private OrderSearchFilterItem _selectedOrder;
        private UploadSearchFilterItem _selectedUploadItem;
        private VideoDurationSearchFilterItem _selectedDurationItem;
        private VideoDefinitionSearchFilterItem _selectedDefinitionItem;

        public SearchPageViewModel(Func<IDataSource> getDataSource, IConnectionListener connectionListener)
            : base(getDataSource, connectionListener, null)
        {
            _searchVideoViewModel = new SearchVideoViewModel(getDataSource, connectionListener, ChangeProgressIndicator);
            _searchChannelsViewModel = new SearchChannelsViewModel(getDataSource, connectionListener, ChangeProgressIndicator);
            _searchPlaylistsViewModel = new SearchPlaylistsViewModel(getDataSource, connectionListener, ChangeProgressIndicator);
            _searchSettingCommand = new RelayCommand(SearchSettings);

            _orderItems = new List<OrderSearchFilterItem>()
            {
                new OrderSearchFilterItem(AppResources.Relevance, SearchResource.ListRequest.OrderEnum.Relevance),
                new OrderSearchFilterItem(AppResources.UploadDate, SearchResource.ListRequest.OrderEnum.Date),
                new OrderSearchFilterItem(AppResources.ViewCount, SearchResource.ListRequest.OrderEnum.ViewCount),
                new OrderSearchFilterItem(AppResources.Rating, SearchResource.ListRequest.OrderEnum.Rating)
            };

            SelectedOrder = _orderItems.First();

            _uploadItems = new List<UploadSearchFilterItem>()
            {
                new UploadSearchFilterItem(AppResources.AllTime, null),
                new UploadSearchFilterItem(AppResources.LastHour, DateTime.Now - TimeSpan.FromHours(1)),
                new UploadSearchFilterItem(AppResources.TToday, DateTime.Today),
                new UploadSearchFilterItem(AppResources.ThisWeek, DateTime.Today - TimeSpan.FromDays(7)),
                new UploadSearchFilterItem(AppResources.ThisMonth, DateTime.Today - TimeSpan.FromDays(30)),
                new UploadSearchFilterItem(AppResources.ThisYear, DateTime.Today - TimeSpan.FromDays(365)),
            };
            SelectedUploadItem = _uploadItems.First();

            _definitionItems = new List<VideoDefinitionSearchFilterItem>()
            {
                new VideoDefinitionSearchFilterItem(AppResources.Any, SearchResource.ListRequest.VideoDefinitionEnum.Any),
                new VideoDefinitionSearchFilterItem(AppResources.Standart, SearchResource.ListRequest.VideoDefinitionEnum.Standard),
                new VideoDefinitionSearchFilterItem(AppResources.High, SearchResource.ListRequest.VideoDefinitionEnum.High),
            };
            SelectedDefinitionItem = _definitionItems.First();

            _durationItems = new List<VideoDurationSearchFilterItem>()
            {
                new VideoDurationSearchFilterItem(AppResources.Any, SearchResource.ListRequest.VideoDurationEnum.Any),
                new VideoDurationSearchFilterItem(AppResources.Long, SearchResource.ListRequest.VideoDurationEnum.Long__),
                new VideoDurationSearchFilterItem(AppResources.Medium, SearchResource.ListRequest.VideoDurationEnum.Medium),
                new VideoDurationSearchFilterItem(AppResources.Short, SearchResource.ListRequest.VideoDurationEnum.Short__)
            };
            SelectedDurationItem = _durationItems.First();
        }

        public SearchVideoViewModel SearchVideoViewModel
        {
            get { return _searchVideoViewModel; }
        }

        public SearchChannelsViewModel SearchChannelsViewModel
        {
            get { return _searchChannelsViewModel; }
        }

        public SearchPlaylistsViewModel SearchPlaylistsViewModel
        {
            get { return _searchPlaylistsViewModel; }
        }

        public ICommand SearchSettingCommand
        {
            get { return _searchSettingCommand; }
        }

        public bool IsSearchSettingsVisible
        {
            get { return _isSearchSettingsVisible; }
            set
            {
                _isSearchSettingsVisible = value;
                NotifyOfPropertyChanged(() => IsSearchSettingsVisible);
            }
        }

        public string SearchString
        {
            get { return _searchString; }
            set { _searchString = value; }
        }

        public List<OrderSearchFilterItem> OrderItems
        {
            get { return _orderItems; }
        }

        public OrderSearchFilterItem SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                _searchVideoViewModel.SearchFilter.Order = _selectedOrder.Value;
                _searchChannelsViewModel.SearchFilter.Order = _selectedOrder.Value;
                _searchPlaylistsViewModel.SearchFilter.Order = _selectedOrder.Value;

                Search(_selectedIndex);
                NotifyOfPropertyChanged(() => SelectedOrder);
            }
        }

        public List<UploadSearchFilterItem> UploadItems
        {
            get { return _uploadItems; }
        }

        public UploadSearchFilterItem SelectedUploadItem
        {
            get { return _selectedUploadItem; }
            set
            {
                _selectedUploadItem = value;
                _searchVideoViewModel.SearchFilter.PublishedAfter = _selectedUploadItem.Value;
                _searchChannelsViewModel.SearchFilter.PublishedAfter = _selectedUploadItem.Value;
                _searchPlaylistsViewModel.SearchFilter.PublishedAfter = _selectedUploadItem.Value;

                Search(_selectedIndex);
                NotifyOfPropertyChanged(() => SelectedUploadItem);
            }
        }

        public List<VideoDurationSearchFilterItem> DurationItems
        {
            get { return _durationItems; }
        }

        public VideoDurationSearchFilterItem SelectedDurationItem
        {
            get { return _selectedDurationItem; }
            set
            {
                _selectedDurationItem = value;
                _searchVideoViewModel.SearchFilter.VideoDuration = _selectedDurationItem.Value;
                _searchChannelsViewModel.SearchFilter.VideoDuration = SearchResource.ListRequest.VideoDurationEnum.Any;
                _searchPlaylistsViewModel.SearchFilter.VideoDuration = SearchResource.ListRequest.VideoDurationEnum.Any;

                Search(_selectedIndex);
                NotifyOfPropertyChanged(() => SelectedDurationItem);
            }
        }

        public List<VideoDefinitionSearchFilterItem> DefinitionItems
        {
            get { return _definitionItems; }
        }

        public VideoDefinitionSearchFilterItem SelectedDefinitionItem
        {
            get { return _selectedDefinitionItem; }
            set
            {
                _selectedDefinitionItem = value;
                _searchVideoViewModel.SearchFilter.VideoDefinition = _selectedDefinitionItem.Value;
                _searchChannelsViewModel.SearchFilter.VideoDuration = SearchResource.ListRequest.VideoDurationEnum.Any;
                _searchPlaylistsViewModel.SearchFilter.VideoDuration = SearchResource.ListRequest.VideoDurationEnum.Any;

                Search(_selectedIndex);
                NotifyOfPropertyChanged(() => SelectedDefinitionItem);
            }
        }

        internal async Task Search(int selectedIndex)
        {
            if (string.IsNullOrEmpty(_searchString))
                return;

            if (_selectedIndex == selectedIndex)
                Clear();

            _selectedIndex = selectedIndex;
            switch (selectedIndex)
            {
                case 0:
                    if (!_searchVideoViewModel.Items.Any())
                        await _searchVideoViewModel.Search(_searchString);
                    break;
                case 1:
                    if (!_searchChannelsViewModel.Items.Any())
                        await _searchChannelsViewModel.Search(_searchString);
                    break;
                case 2:
                    if (!_searchPlaylistsViewModel.Items.Any())
                        await _searchPlaylistsViewModel.Search(_searchString);
                    break;
                default:
                    break;
            }
        }

        private void Clear()
        {
            _searchVideoViewModel.Items.Clear();
            _searchChannelsViewModel.Items.Clear();
            _searchPlaylistsViewModel.Items.Clear();
        }

        private void ChangeProgressIndicator(bool isVisible)
        {
            if (isVisible)
            {
                ShowProgressIndicator();
                return;
            }
            HideProgressIndicator();
        }

        private void SearchSettings()
        {
            IsSearchSettingsVisible = !IsSearchSettingsVisible;
        }
    }
}
