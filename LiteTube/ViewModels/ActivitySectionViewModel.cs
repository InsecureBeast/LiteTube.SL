using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System;
using System.Threading.Tasks;
using LiteTube.Common.Helpers;

namespace LiteTube.ViewModels
{
    public class ActivitySectionViewModel : SectionBaseViewModel
    {
        public ActivitySectionViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
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
            return await _getGeDataSource().GetActivity(nextPageToken);
        }
    }
}

