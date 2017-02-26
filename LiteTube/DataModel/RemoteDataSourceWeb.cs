using Google.Apis.YouTube.v3;
using LiteTube.Common;
using LiteTube.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube.DataModel
{
    partial class RemoteDataSource
    {
        private async Task<IVideoList> GetMostPopularWeb(string culture, int maxResult, string pageToken)
        {
            var res = await _youTubeWeb.GetMostPopular(culture, pageToken);
            if (res == null)
                return MVideoList.Empty;

            return await GetVideoList(res);
        }
    }
}
