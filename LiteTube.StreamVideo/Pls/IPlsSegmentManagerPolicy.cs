using System;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Pls
{
  public interface IPlsSegmentManagerPolicy
  {
    Task<Uri> GetTrackAsync(PlsParser pls, ContentType contentType, CancellationToken cancellationToken);
  }
}
