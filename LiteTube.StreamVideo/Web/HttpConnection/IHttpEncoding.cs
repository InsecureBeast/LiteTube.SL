using System.Text;

namespace LiteTube.StreamVideo.Web.HttpConnection
{
  public interface IHttpEncoding
  {
    Encoding HeaderDecoding { get; }

    Encoding HeaderEncoding { get; }
  }
}
