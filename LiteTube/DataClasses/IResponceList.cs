using System;
using System.Collections.Generic;
using System.Text;

namespace LiteTube.DataClasses
{
    public interface IPageInfo
    {
        string ETag { get; }
        int? ResultsPerPage { get; }
        int? TotalResults { get; }
    }

    public interface ITokenPagination
    {
        string ETag { get; }
    }

    public interface IResponceList
    {
        string EventId { get; }
        string ETag { get; }
        string NextPageToken { get; }
        IPageInfo PageInfo { get; }
        string PrevPageToken { get; }
        ITokenPagination TokenPagination { get; }
        string VisitorId { get; }
    }

    class MResponceList : IResponceList
    {
        public static IResponceList Empty
        {
            get { return new MResponceList(); }
        }

        public string ETag
        {
            get { return string.Empty; }
        }

        public string EventId
        {
            get { return string.Empty; }
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

        public ITokenPagination TokenPagination
        {
            get { return MTokenPagination.Empty; }
        }

        public string VisitorId
        {
            get { return string.Empty; }
        }
    }
}
