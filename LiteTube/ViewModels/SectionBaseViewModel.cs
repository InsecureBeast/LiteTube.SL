using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Windows.Input;
using LiteTube.ViewModels.Nodes;
using MyToolkit.Command;
using LiteTube.Common;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LiteTube.ViewModels
{
    /// <summary>
    /// Модель для отображения определенной секции. 
    /// Например видео канала или самого популярного
    /// </summary>
    public class SectionBaseViewModel : PropertyChangedBase //IHubSection, IDataSourceContext
    {
        protected readonly NavigationPanelViewModel _navigatioPanelViewModel;
        protected string _uniqueId;
        protected string _title;
        protected readonly IDataSource _dataSource;
        protected Frame _frame;
        protected bool _hasItems;
        private readonly Common.RelayCommand _loadMoreCommand;
        private readonly RelayCommand<NavigationObject> _itemClickCommand;
        private readonly Common.RelayCommand _selectCommand;
        private readonly Common.RelayCommand _deleteCommand;
        private bool _isLoading = true;
        private bool _isEmpty = false;
        private bool _inCall = false;
        private bool _isItemClickEnabled;
        protected string _pageToken = string.Empty;
        //private ListViewSelectionMode _selectionMode;
        private readonly ObservableCollection<NodeViewModelBase> _selectedItems;

        public SectionBaseViewModel(IDataSource dataSource)
        {
            _dataSource = dataSource;
            _hasItems = true;
            _navigatioPanelViewModel = new NavigationPanelViewModel(_dataSource);
            Items = new ObservableCollection<NodeViewModelBase>();
            _loadMoreCommand = new Common.RelayCommand(LoadMore);
            _itemClickCommand = new RelayCommand<NavigationObject>(NavigateTo);
            _selectCommand = new Common.RelayCommand(SelectItems);
            _deleteCommand = new Common.RelayCommand(DeleteItems);

            //SelectionMode = ListViewSelectionMode.None;
            IsItemClickEnabled = true;
            _selectedItems = new ObservableCollection<NodeViewModelBase>();
        }

        public NavigationPanelViewModel NavigationPanelViewModel
        {
            get { return _navigatioPanelViewModel; }
        }

        public string UniqueId 
        {
            get { return _uniqueId; }
        }

        public string Title 
        {
            get { return _title; }
            protected set
            {
                _title = value;
                NotifyOfPropertyChanged(() => Title);
            }
        }

        public string Subtitle { get; private set; }
        public string Description { get; set; }
        public string ImagePath { get; private set; }
        public ObservableCollection<NodeViewModelBase> Items { get; private set; }

        public IDataSource DataSource
        {
            get { return _dataSource; }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            protected set
            {
                _isLoading = value;
                NotifyOfPropertyChanged(() => IsLoading);
            }
        }

        //public ListViewSelectionMode SelectionMode
        //{
        //    get { return _selectionMode; }
        //    protected set
        //    {
        //        _selectionMode = value;
        //        NotifyOfPropertyChanged(() => SelectionMode);
        //    }
        //}

        public ObservableCollection<NodeViewModelBase> SelectedItems
        {
            get { return _selectedItems; }
        }

        public bool IsItemClickEnabled
        {
            get { return _isItemClickEnabled; }
            protected set
            {
                _isItemClickEnabled = value;
                NotifyOfPropertyChanged(() => IsItemClickEnabled);
            }
        }

        internal async Task FirstLoad()
        {
            //LayoutHelper.InvokeFromUIThread(async () =>
            //{
                var responseList = await GetItems(string.Empty);
                if (responseList == null)
                    return;
                LoadItems(responseList);
                _pageToken = responseList.NextPageToken;
                _hasItems = !string.IsNullOrEmpty(_pageToken);
            //});
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }
            protected set
            {
                _isEmpty = value;
                NotifyOfPropertyChanged(() => IsEmpty);
            }
        }

        public ICommand LoadMoreCommand
        {
            get { return _loadMoreCommand; }
        }

        public ICommand ItemClickCommand
        {
            get { return _itemClickCommand; }
        }

        public ICommand SelectCommand
        {
            get { return _selectCommand; }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public void SetNavigationFrame(Frame frame)
        {
            _frame = frame;
        }

        internal virtual void LoadItems(IResponceList videoList)
        {
            var list = videoList as IVideoList;
            if (list == null)
                return;

            AddItems(list.Items);
        }

        internal virtual Task<IResponceList> GetItems(string nextPageToken)
        {
            return null; 
        }

        internal void SetNonSelected()
        {
            //SelectionMode = ListViewSelectionMode.None;
            IsItemClickEnabled = true;
        }

        internal void SetSelected()
        {
            //SelectionMode = ListViewSelectionMode.Multiple;
            IsItemClickEnabled = false;
        }

        internal void AddItems(IEnumerable<IVideoItem> items)
        {
            var itemsList = Items.ToList();
            foreach (var item in items)
            {
                if (itemsList.Exists(i => i.Id == item.Details.Video.Id))
                    continue;
                Items.Add(new VideoItemViewModel(item));
            }

            IsLoading = false;
            if (!Items.Any())
                IsEmpty = true;
        }

        private async void LoadMore()
        {
            if (!_hasItems)
                return;

            if (_inCall)
                return;

            _inCall = true;
            ShowProgressIndicator();
            var result = await GetItems(_pageToken);
            if (result == null)
            {
                _inCall = false;
                _hasItems = false;
                HideProgressIndicator();
                return;
            }

            LoadItems(result);
            _pageToken = result.NextPageToken;
            _hasItems = !string.IsNullOrEmpty(_pageToken);
            _inCall = false;
            HideProgressIndicator();
        }

        internal virtual void NavigateTo(NavigationObject navObject)
        {
            if (navObject.ViewModel is VideoCategoryNodeViewModel)
            {
                var viewModel = (VideoCategoryNodeViewModel)navObject.ViewModel;
                var categoryId = viewModel.CategoryId;
                var title = viewModel.Title;
                PhoneApplicationService.Current.State["model"] = new VideoCategorySectionViewModel(categoryId, title, _dataSource);
                App.NavigateTo("/SectionPage.xaml");
                return;
            }

            var id = navObject.ViewModel.VideoId;
            PhoneApplicationService.Current.State["model"] = new VideoPageViewModel(id, _dataSource);
            App.NavigateTo(string.Format("/VideoPage.xaml?videoId={0}", id));
        }

        protected void ShowProgressIndicator()
        {
            //var statusBar = StatusBar.GetForCurrentView();
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var arstring = resourceLoader.GetString("LoadingString");
            //statusBar.ProgressIndicator.Text = arstring;
            //SystemTray.ProgressIndicator.IsVisible = true;
        }

        protected void HideProgressIndicator()
        {
            //SystemTray.ProgressIndicator.IsVisible = false;
        }

        protected virtual void DeleteItems()
        {
            SetNonSelected();
        }

        protected virtual void SelectItems()
        {
            SetSelected();
        }
    }
}
