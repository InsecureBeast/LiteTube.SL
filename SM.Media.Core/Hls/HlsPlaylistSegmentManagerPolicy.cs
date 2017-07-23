using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Playlists;

namespace SM.Media.Core.Hls
{
    public class HlsPlaylistSegmentManagerPolicy : IHlsPlaylistSegmentManagerPolicy
    {
        //TODO choose program by quality
        public static Func<ICollection<ISubProgram>, ISubProgram> SelectSubProgram = programs =>  programs.FirstOrDefault();
        private readonly Func<HlsProgramManager> _programManagerFactory;
        private readonly VideoQuality _quality;

        public HlsPlaylistSegmentManagerPolicy(Func<HlsProgramManager> programManagerFactory, VideoQuality quality)
        {
            if (null == programManagerFactory)
                throw new ArgumentNullException("programManagerFactory");

            _programManagerFactory = programManagerFactory;
            _quality = quality;
        }

        public Task<ISubProgram> CreateSubProgramAsync(ICollection<Uri> source, ContentType contentType, CancellationToken cancellationToken)
        {
            return LoadSubProgram(CreateProgramManager(source, contentType), contentType, cancellationToken);
        }

        protected virtual IProgramManager CreateProgramManager(ICollection<Uri> source, ContentType contentType)
        {
            if (ContentTypes.M3U != contentType && ContentTypes.M3U8 != contentType)
                throw new NotSupportedException(string.Format("Content type {0} not supported by this program manager", null == contentType ? "<unknown>" : contentType.ToString()));

            HlsProgramManager hlsProgramManager = _programManagerFactory();
            hlsProgramManager.Playlists = source;
            return hlsProgramManager;
        }

        protected virtual async Task<ISubProgram> LoadSubProgram(IProgramManager programManager, ContentType contentType, CancellationToken cancellationToken)
        {
            ISubProgram subProgram;
            try
            {
                IDictionary<long, Program> programs = await programManager.LoadAsync(cancellationToken).ConfigureAwait(false);
                Program program = programs.Values.FirstOrDefault();
                if (null == program)
                {
                    Debug.WriteLine("PlaylistSegmentManagerFactory.SetMediaSource(): program not found");
                    throw new FileNotFoundException("Unable to load program");
                }
                subProgram = program.SubPrograms.FirstOrDefault(x => FirstVideo(x, _quality));
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

        private bool FirstVideo(ISubProgram subProgram, VideoQuality quality)
        {
            switch (quality)
            {
                case VideoQuality.Quality144P:
                    if (subProgram.Height == 144)
                        return true;
                    break;
                case VideoQuality.Quality240P:
                    if (subProgram.Height == 240)
                        return true;
                    break;
                case VideoQuality.Quality360P:
                    if (subProgram.Height == 360)
                        return true;
                    break;
                case VideoQuality.Quality480P:
                    if (subProgram.Height == 480)
                        return true;
                    break;
                case VideoQuality.Quality720P:
                    if (subProgram.Height == 720)
                        return true;
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}
