using System;
using System.Collections.Generic;
using System.Text;

namespace LiteTube.DataClasses
{
    public interface IPageInfo
    {
        int? ResultsPerPage { get; }
        int? TotalResults { get; }
    }

    public interface IResponceList
    {
        string NextPageToken { get; }
        IPageInfo PageInfo { get; }
        string PrevPageToken { get; }
        string VisitorId { get; }
    }

    class MResponceList : IResponceList
    {
        public static IResponceList Empty
        {
            get { return new MResponceList(); }
        }

        public string NextPageToken
        {
            get { return string.Empty; }
        }

        public IPageInfo PageInfo
        {
            get { return MPageInfo.Empty; }
        }

        public string PrevPageToken
        {
            get { return string.Empty; }
        }

        public string VisitorId
        {
            get { return string.Empty; }
        }
    }
}
