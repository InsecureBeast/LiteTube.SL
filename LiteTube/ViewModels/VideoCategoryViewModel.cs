using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
{
    public class VideoCategoryViewModel : SectionBaseViewModel
    {
        private readonly string _categoryId;

        public VideoCategoryViewModel(string categoryId, string title, IDataSource dataSource) : base(dataSource)
        {
            _categoryId = categoryId;
            Title = title;
        }

        public override string ToString()
        {
            return Title;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _dataSource.GetCategoryVideoList(_categoryId, nextPageToken);
        }
    }
}
