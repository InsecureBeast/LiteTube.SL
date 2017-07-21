// Decompiled with JetBrains decompiler
// Type: SM.Media.Hls.HlsPlaylistSegmentManagerPolicy
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Playlists;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Hls
{
  public class HlsPlaylistSegmentManagerPolicy : IHlsPlaylistSegmentManagerPolicy
  {
    public static Func<ICollection<ISubProgram>, ISubProgram> SelectSubProgram = (Func<ICollection<ISubProgram>, ISubProgram>) (programs => Enumerable.FirstOrDefault<ISubProgram>((IEnumerable<ISubProgram>) programs));
    private readonly Func<HlsProgramManager> _programManagerFactory;

    public HlsPlaylistSegmentManagerPolicy(Func<HlsProgramManager> programManagerFactory)
    {
      if (null == programManagerFactory)
        throw new ArgumentNullException("programManagerFactory");
      this._programManagerFactory = programManagerFactory;
    }

    public Task<ISubProgram> CreateSubProgramAsync(ICollection<Uri> source, ContentType contentType, CancellationToken cancellationToken)
    {
      return this.LoadSubProgram(this.CreateProgramManager(source, contentType), contentType, cancellationToken);
    }

    protected virtual IProgramManager CreateProgramManager(ICollection<Uri> source, ContentType contentType)
    {
      if (ContentTypes.M3U != contentType && ContentTypes.M3U8 != contentType)
        throw new NotSupportedException(string.Format("Content type {0} not supported by this program manager", (ContentType) null == contentType ? (object) "<unknown>" : (object) contentType.ToString()));
      HlsProgramManager hlsProgramManager = this._programManagerFactory();
      hlsProgramManager.Playlists = source;
      return (IProgramManager) hlsProgramManager;
    }

    protected virtual async Task<ISubProgram> LoadSubProgram(IProgramManager programManager, ContentType contentType, CancellationToken cancellationToken)
    {
      ISubProgram subProgram;
      try
      {
        IDictionary<long, Program> programs = await programManager.LoadAsync(cancellationToken).ConfigureAwait(false);
        Program program = Enumerable.FirstOrDefault<Program>((IEnumerable<Program>) programs.Values);
        if (null == program)
        {
          Debug.WriteLine("PlaylistSegmentManagerFactory.SetMediaSource(): program not found");
          throw new FileNotFoundException("Unable to load program");
        }
        subProgram = HlsPlaylistSegmentManagerPolicy.SelectSubProgram(program.SubPrograms);
        if (null == subProgram)
        {
          Debug.WriteLine("PlaylistSegmentManagerFactory.SetMediaSource(): no sub programs found");
          throw new FileNotFoundException("Unable to load program stream");
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("PlaylistSegmentManagerFactory.SetMediaSource(): unable to load playlist: " + ex.Message);
        throw;
      }
      return subProgram;
    }
  }
}
