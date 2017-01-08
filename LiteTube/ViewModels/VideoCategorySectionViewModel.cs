using System;
using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;
using LiteTube.Common.Helpers;

namespace LiteTube.ViewModels
{
    public class VideoCategorySectionViewModel : SectionBaseViewModel
    {
        private readonly string _categoryId;

        public VideoCategorySectionViewModel(string categoryId, string title, Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener, null)
        {
            _categoryId = categoryId;
            Title = title;
            ShowAdv = SettingsHelper.IsAdvVisible;
            LayoutHelper.InvokeFromUiThread(async() => await FirstLoad());
        }

        public override string ToString()
        {
            return Title;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getDataSource().GetCategoryVideoList(_categoryId, nextPageToken);
        }

        public override void Notify(ConnectionEventArgs e)
        {
            base.Notify(e);
            if (e.IsConnected)
            {
                LayoutHelper.InvokeFromUiThread(async () =>
                {
                    await FirstLoad();
                });
            }
        }
    }
}
