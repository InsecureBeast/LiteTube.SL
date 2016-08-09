﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.Core.Common;
using LiteTube.Core.Common.Helpers;
using LiteTube.Core.DataClasses;
using LiteTube.Core.DataModel;
using LiteTube.Core.ViewModels.Nodes;

namespace LiteTube.Core.ViewModels
{
    class SubscriptionChannelsViewModel : SectionBaseViewModel
    {
        public SubscriptionChannelsViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
        }

        public override void Notify(ConnectionEventArgs e)
        {
            base.Notify(e);
            LayoutHelper.InvokeFromUiThread(async () =>
            {
                IsConnected = e.IsConnected;

                if (e.IsConnected)
                {
                    await FirstLoad();
                    return;
                }

                if (Items.Count > 0)
                    IsConnected = true;
            });
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getGeDataSource().GetSubscribtions(nextPageToken);
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

#if SILVERLIGHT
            NavigationHelper.GoToChannelPage(model.Id, null);
#endif
        }

        internal void AddItems(IEnumerable<ISubscription> items)
        {
            var itemsList = Items.ToList();
            foreach (var item in items)
            {
                if (itemsList.Exists(i => i.Id == item.ChannelId))
                    continue;
                Items.Add(new SubscriptionNodeViewModel(item));
            }
        }
    }
}
