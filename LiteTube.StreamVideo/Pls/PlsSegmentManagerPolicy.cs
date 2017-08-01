using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.Pls
{
  public class PlsSegmentManagerPolicy : IPlsSegmentManagerPolicy
  {
    public static Func<ICollection<PlsTrack>, PlsTrack> SelectTrack = (Func<ICollection<PlsTrack>, PlsTrack>) (tracks => Enumerable.FirstOrDefault<PlsTrack>((IEnumerable<PlsTrack>) tracks));

    public Task<Uri> GetTrackAsync(PlsParser pls, ContentType contentType, CancellationToken cancellationToken)
    {
      ICollection<PlsTrack> tracks = pls.Tracks;
      PlsTrack plsTrack = PlsSegmentManagerPolicy.SelectTrack(tracks);
      if (null == plsTrack)
        return TaskEx.FromResult<Uri>((Uri) null);
      if (tracks.Count > 1)
        Debug.WriteLine("PlsSegmentManagerPolicy.GetTrackAsync() multiple tracks are not supported");
      if (null == plsTrack.File)
        Debug.WriteLine("PlsSegmentManagerPolicy.GetTrackAsync() track does not have a file");
      Uri result;
      if (Uri.TryCreate(pls.BaseUrl, plsTrack.File, out result))
        return TaskEx.FromResult<Uri>(result);
      Debug.WriteLine("PlsSegmentManagerPolicy.GetTrackAsync() invalid track file: " + plsTrack.File);
      return TaskEx.FromResult<Uri>((Uri) null);
    }
  }
}
