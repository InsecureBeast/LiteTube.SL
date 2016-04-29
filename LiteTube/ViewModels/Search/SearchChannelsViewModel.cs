using LiteTube.Common;
using LiteTube.Common.Helpers;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.ViewModels.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteTube.ViewModels.Search
{
    class SearchChannelsViewModel : SearchBaseViewModel
    {
        public SearchChannelsViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener, Action<bool> changeProgressIndicator)
            : base(SearchType.Channel, geDataSource, connectionListener, changeProgressIndicator)
        {
        }

        internal override void LoadItems(IResponceList responceList)
        {
            var list = responceList as IChannelList;
            if (list == null)
                return;

            AddItems(list.Items);
        }

        internal override void NavigateTo(NavigationObject navObject)
        {
            if (navObject == null)
                return;

            if (navObject.ViewModel == null)
                return;

            var viewModel = navObject.ViewModel as ChannelNodeViewModel;
            if (viewModel == null)
                return;

            var channel = viewModel.Channel;
            if (channel == null)
                return;

#if SILVERLIGHT
            NavigationHelper.Navigate("/ChannelPage.xaml", new ChannelPageViewModel(channel.Id, _getGeDataSource, _connectionListener));
#endif
        }

        private void AddItems(IEnumerable<IChannel> items)
        {
            var itemsList = Items.ToList();
            foreach (var item in items)
            {
                if (itemsList.Exists(i => i.Id == item.Id))
                    continue;
                Items.Add(new ChannelNodeViewModel(item));
            }
        }
    }
}
