using System;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;

namespace SM.Media.Core.Pls
{
  public interface IPlsSegmentManagerPolicy
  {
    Task<Uri> GetTrackAsync(PlsParser pls, ContentType contentType, CancellationToken cancellationToken);
  }
}
