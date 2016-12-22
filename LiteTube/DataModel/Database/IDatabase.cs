using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube.DataModel.Database
{
    interface IDatabase
    {
        void AddHistoryVideo(string videoId);
        Task<IEnumerable<IHistoryVideo>> GetHistoryVideos(/*params?*/);
    }

    interface IHistoryVideo
    {
        int Id { get; }
        string VideoId { get; }
        DateTime DateView { get; }
    }
}
