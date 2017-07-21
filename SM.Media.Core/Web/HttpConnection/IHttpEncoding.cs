using System.Text;

namespace SM.Media.Core.Web.HttpConnection
{
  public interface IHttpEncoding
  {
    Encoding HeaderDecoding { get; }

    Encoding HeaderEncoding { get; }
  }
}
