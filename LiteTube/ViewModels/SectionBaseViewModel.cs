﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Windows.Input;
using LiteTube.ViewModels.Nodes;
using Microsoft.Phone.Shell;
using MyToolkit.Command;
using LiteTube.Common;
using LiteTube.Common.Helpers;

namespace LiteTube.ViewModels
{
    /// <summary>
    /// Модель для отображения определенной секции. 
    /// Например видео канала или самого популярного
    /// </summary>
    public class SectionBaseViewModel : PropertyChangedBase, IListener<ConnectionEventArgs>
    {
        protected readonly NavigationPanelViewModel _navigatioPanelViewModel;
        protected string _uniqueId;
        protected string _title;
        protected readonly IDataSource _dataSource;
        protected readonly ConnectionListener _connectionListener;
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
        private bool _isConnected = true;

        public SectionBaseViewModel(IDataSource dataSource, ConnectionListener connectionListener)
        {
            if (dataSource == null) 
                throw new ArgumentNullException("dataSource");
            if (connectionListener == null) 
                throw new ArgumentNullException("connectionListener");
            
            _dataSource = dataSource;
            _connectionListener = connectionListener;
            _connectionListener.Subscribe(this);

            _hasItems = true;
            _navigatioPanelViewModel = new NavigationPanelViewModel(_dataSource, connectionListener);
            Items = new ObservableCollection<NodeViewModelBase>();
            _loadMoreCommand = new Common.RelayCommand(LoadMore);
            _itemClickCommand = new RelayCommand<NavigationObject>(NavigateTo);
            _selectCommand = new Common.RelayCommand(SelectItems);
            _deleteCommand = new Common.RelayCommand(DeleteItems);

            //SelectionMode = ListViewSelectionMode.None;
            IsItemClickEnabled = true;
            _selectedItems = new ObservableCollection<NodeViewModelBase>();

            _isConnected = ConnectionListener.CheckNetworkAvailability();
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
            get
            {
                return _isConnected && _isLoading;
            }
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

        public bool IsEmpty
        {
            get
            {
                return _isConnected && _isEmpty;
            }
            protected set
            {
                _isEmpty = value;
                NotifyOfPropertyChanged(() => IsEmpty);
            }
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

        internal async Task FirstLoad()
        {
            if (Items.Count > 0)
                return;

            IsLoading = true;

            var responseList = await GetItems(string.Empty);
            if (responseList == null || responseList.PageInfo == null)
            {
                IsLoading = false;
                if (!Items.Any())
                    IsEmpty = true;
                return;
            }

            LoadItems(responseList);
            _pageToken = responseList.NextPageToken;
            _hasItems = !string.IsNullOrEmpty(_pageToken);
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
            if (navObject == null)
                return;
            var id = navObject.ViewModel.VideoId;
            var view = string.Format("/VideoPage.xaml?videoId={0}", id);
            NavigationHelper.Navigate(view, new VideoPageViewModel(id, _dataSource, _connectionListener));
        }

        protected void ShowProgressIndicator()
        {
            //var statusBar = StatusBar.GetForCurrentView();
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var arstring = resourceLoader.GetString("LoadingString");
            //statusBar.ProgressIndicator.Text = arstring;
            SystemTray.ProgressIndicator.IsVisible = true;
        }

        protected void HideProgressIndicator()
        {
            SystemTray.ProgressIndicator.IsVisible = false;
        }

        protected virtual void DeleteItems()
        {
            SetNonSelected();
        }

        protected virtual void SelectItems()
        {
            SetSelected();
        }

        public virtual void Notify(ConnectionEventArgs e)
        {
            LayoutHelper.InvokeFromUIThread(() =>
            {
                IsConnected = e.IsConnected;

                if (e.IsConnected)
                    return;

                IsEmpty = false;
                IsLoading = false;

                if (Items.Count > 0)
                    IsConnected = true;
            });
        }
    }
}
