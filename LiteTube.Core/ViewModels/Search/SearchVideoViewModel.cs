using System;
using LiteTube.Core.DataClasses;
using LiteTube.Core.DataModel;

namespace LiteTube.Core.ViewModels.Search
{
    public class SearchVideoViewModel : SearchBaseViewModel
    {
        public SearchVideoViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener, Action<bool> changeProgressIndicator)
            : base(SearchType.Video, geDataSource, connectionListener, changeProgressIndicator)
        {
        }
    }
}
