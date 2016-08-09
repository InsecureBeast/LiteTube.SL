﻿using System;
using System.Threading.Tasks;
using LiteTube.Core.Common.Helpers;
using LiteTube.Core.DataClasses;
using LiteTube.Core.DataModel;

namespace LiteTube.Core.ViewModels
{
    public class MostPopularViewModel : SectionBaseViewModel
    {
        private readonly IVideoList _videoList;

        public MostPopularViewModel(IVideoList videoList, Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
            _videoList = videoList;
            _uniqueId = videoList.GetHashCode().ToString();
            ShowAdv = SettingsHelper.IsAdvVisible;
        }

        public MostPopularViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
            ShowAdv = SettingsHelper.IsAdvVisible;
        }

        public override string ToString()
        {
            return Title;
        }

        public override void Notify(ConnectionEventArgs e)
        {
            base.Notify(e);
            LayoutHelper.InvokeFromUiThread(async () =>
            {
                if (!e.IsConnected)
                    return;

                Items.Clear();
                await FirstLoad();    
            });
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getGeDataSource().GetMostPopular(nextPageToken);
        }
    }
}
