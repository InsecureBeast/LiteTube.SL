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
        private ObservableCollection<SubscriptionNodeViewModel> _channels;

        public SubscriptionChannelsViewModel(IDataSource dataSource) : base(dataSource)
        {
            _channels = new ObservableCollection<SubscriptionNodeViewModel>();
        }

        public ObservableCollection<SubscriptionNodeViewModel> CategoryItems
        {
            get { return _channels; }
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
            var model = navObject.ViewModel as SubscriptionNodeViewModel;
            if (model == null)
                return;

            NavigationHelper.Navigate("/ChannelPage.xaml", new ChannelPageViewModel(model.Id, _dataSource));
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
        }
    }
}
