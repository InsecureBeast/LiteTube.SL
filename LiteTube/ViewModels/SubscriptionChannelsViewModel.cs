using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.ViewModels.Nodes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.Common;
using LiteTube.Common.Helpers;

namespace LiteTube.ViewModels
{
    class SubscriptionChannelsViewModel : SectionBaseViewModel
    {
        private readonly ObservableCollection<SubscriptionNodeViewModel> _channels;

        public SubscriptionChannelsViewModel(IDataSource dataSource, IConnectionListener connectionListener)
            : base(dataSource, connectionListener)
        {
            _channels = new ObservableCollection<SubscriptionNodeViewModel>();
        }

        public ObservableCollection<SubscriptionNodeViewModel> CategoryItems
        {
            get { return _channels; }
        }

        public override void Notify(ConnectionEventArgs e)
        {
            base.Notify(e);
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                IsConnected = e.IsConnected;

                if (e.IsConnected)
                {
                    await FirstLoad();
                    return;
                }

                if (CategoryItems.Count > 0)
                    IsConnected = true;
            });
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _dataSource.GetSubscribtions(nextPageToken);
        }

        internal override void LoadItems(IResponceList videoList)
        {
            var channelList = videoList as ISubscriptionList;
            if (channelList == null)
                return;

            AddItems(channelList.Items);
        }

        internal override void NavigateTo(NavigationObject navObject)
        {
            if (navObject == null)
                return;

            var model = navObject.ViewModel as SubscriptionNodeViewModel;
            if (model == null)
                return;

            NavigationHelper.Navigate("/ChannelPage.xaml", new ChannelPageViewModel(model.Id, _dataSource, _connectionListener));
        }

        internal void AddItems(IEnumerable<ISubscription> items)
        {
            var itemsList = _channels.ToList();
            foreach (var item in items)
            {
                if (itemsList.Exists(i => i.Id == item.ChannelId))
                    continue;
                _channels.Add(new SubscriptionNodeViewModel(item));
            }

            IsLoading = false;
            if (!_channels.Any())
                IsEmpty = true;

            HideProgressIndicator();
        }
    }
}
