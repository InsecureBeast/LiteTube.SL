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
}
