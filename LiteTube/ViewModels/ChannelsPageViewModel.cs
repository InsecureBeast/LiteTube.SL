using System;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiteTube.ViewModels.Nodes;
using LiteTube.Common;
using Microsoft.Phone.Shell;

namespace LiteTube.ViewModels
{
    class ChannelListPageViewModel : SectionBaseViewModel
    {
        private readonly string _categoryId;
        private readonly ObservableCollection<ChannelNodeViewModel> _channels;

        public ChannelListPageViewModel(string categoryId, string title, Func<IDataSource> getGetGeDataSource, IConnectionListener connectionListener)
            : base(getGetGeDataSource, connectionListener)
        {
            _uniqueId = categoryId;
            _categoryId = categoryId;
            Title = title;
            _channels = new ObservableCollection<ChannelNodeViewModel>();
            LayoutHelper.InvokeFromUIThread(async() => await FirstLoad());
        }

        public override string ToString()
        {
            return Title;
        }

        public ObservableCollection<ChannelNodeViewModel> CategoryItems
        {
            get { return _channels; }
        }

        public override void Notify(ConnectionEventArgs e)
        {
            base.Notify(e);
            if (e.IsConnected)
            {
                LayoutHelper.InvokeFromUIThread(async () =>
                {
                    await FirstLoad();
                });
            }
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getGeDataSource().GetChannels(_categoryId, nextPageToken);
        }

        internal override void LoadItems(IResponceList videoList)
        {
            var channelList = videoList as IChannelList;
            if (channelList == null)
                return;

            AddItems(channelList.Items);
        }

        internal void AddItems(IEnumerable<IChannel> items)
        {
            var itemsList = _channels.ToList();
            foreach (var item in items)
            {
                if (itemsList.Exists(i => i.Id == item.Id))
                    continue;
                _channels.Add(new ChannelNodeViewModel(item));
            }

            IsLoading = false;
            if (!_channels.Any())
                IsEmpty = true;

            HideProgressIndicator();
        }

        internal override void NavigateTo(NavigationObject navObject)
        {
            var id = ((ChannelNodeViewModel)navObject.ViewModel).Channel.Id;
            PhoneApplicationService.Current.State["model"] = new ChannelPageViewModel(id, _getGeDataSource, _connectionListener);
            App.NavigateTo("/ChannelPage.xaml");
        }
    }
}
