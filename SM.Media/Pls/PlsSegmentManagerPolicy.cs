// Decompiled with JetBrains decompiler
// Type: SM.Media.Pls.PlsSegmentManagerPolicy
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using SM.Media.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Pls
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
