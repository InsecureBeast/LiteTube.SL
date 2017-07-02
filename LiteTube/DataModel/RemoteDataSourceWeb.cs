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
    public partial class RemoteDataSource
    {
        private async Task<IVideoList> GetMostPopularWeb(string culture, int maxResult, string pageToken)
        {
            var res = await _youTubeWeb.GetMostPopular(culture, pageToken);
            if (res == null)
                return MVideoList.Empty;

            return await GetVideoList(res);
        }

        private async Task<IVideoList> GetActivityWeb(string culture, int maxResult, string pageToken)
        {
            var res = await _youTubeWeb.GetActivity(_youTubeServiceControl.OAuthToken, pageToken);
            if (res == null)
                return null;

            var videoIds = new StringBuilder();
            foreach (var id in res.Ids)
            {
                videoIds.AppendLine(id);
                videoIds.AppendLine(",");
            }

            var videos = await GetVideo(videoIds.ToString());
            videos.NextPageToken = res.NextPageToken;
            return new MVideoList(videos);
        }

        private async Task<IVideoList> GetChannelVideosWeb(string channelId, string nextPageToken)
        {
            var res = await _youTubeWeb.GetChannelVideos(channelId, _youTubeServiceControl.OAuthToken, nextPageToken);
            if (res == null)
                return MVideoList.Empty;

            var videoIds = new StringBuilder();
            foreach (var id in res.Ids)
            {
                videoIds.AppendLine(id);
                videoIds.AppendLine(",");
            }

            var videos = await GetVideo(videoIds.ToString());
            videos.NextPageToken = res.NextPageToken;
            return new MVideoList(videos);
        }

        public async Task<IVideoList> GetSubscriptionsVideoWeb(IEnumerable<string> subscriptions,  int maxResult, string pageToken)
        {
            var res = await _youTubeWeb.GetSubscriptionsVideo(subscriptions, _youTubeServiceControl.OAuthToken, pageToken);
            if (res == null)
                return MVideoList.Empty;

            return await GetVideoList(res);
        }
    }
}
