using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Playlists;

namespace LiteTube.StreamVideo.Hls
{
    public class HlsPlaylistSegmentManagerPolicy : IHlsPlaylistSegmentManagerPolicy
    {
        public static Func<ICollection<ISubProgram>, ISubProgram> SelectSubProgram = programs =>  programs.FirstOrDefault();
        private readonly Func<HlsProgramManager> _programManagerFactory;
        private readonly VideoQuality _quality;

        public HlsPlaylistSegmentManagerPolicy(Func<HlsProgramManager> programManagerFactory, VideoQuality quality)
        {
            if (null == programManagerFactory)
                throw new ArgumentNullException(nameof(programManagerFactory));

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
                throw new NotSupportedException($"Content type {contentType?.ToString() ?? "<unknown>"} not supported by this program manager");

            var hlsProgramManager = _programManagerFactory();
            hlsProgramManager.Playlists = source;
            return hlsProgramManager;
        }

        protected virtual async Task<ISubProgram> LoadSubProgram(IProgramManager programManager, ContentType contentType, CancellationToken cancellationToken)
        {
            ISubProgram subProgram;
            try
            {
                var programs = await programManager.LoadAsync(cancellationToken).ConfigureAwait(false);
                var program = programs.Values.FirstOrDefault();
                if (null == program)
                {
                    Debug.WriteLine("PlaylistSegmentManagerFactory.SetMediaSource(): program not found");
                    throw new FileNotFoundException("Unable to load program");
                }
                subProgram = TryFindBestQuality(program.SubPrograms, _quality);
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

        //private bool FirstVideo(ISubProgram subProgram, VideoQuality quality)
        //{
        //    switch (quality)
        //    {
        //        case VideoQuality.Quality144P:
        //            if (subProgram.Height == 144)
        //                return true;
        //            break;
        //        case VideoQuality.Quality240P:
        //            if (subProgram.Height == 240)
        //                return true;
        //            break;
        //        case VideoQuality.Quality360P:
        //            if (subProgram.Height == 360)
        //                return true;
        //            break;
        //        case VideoQuality.Quality480P:
        //            if (subProgram.Height == 480)
        //                return true;
        //            break;
        //        case VideoQuality.Quality720P:
        //            if (subProgram.Height == 720)
        //                return true;
        //            break;
        //        default:
        //            break;
        //    }
        //    return false;
        //}

        private static ISubProgram TryFindBestQuality(IEnumerable<ISubProgram> programs, VideoQuality maxVideoQuality)
        {
            var selected = programs.Where(p => p.Height <= GetResolution(maxVideoQuality));
            var ordered = selected.OrderByDescending(u => u.Height).ToList();
            return ordered.FirstOrDefault();
        }

        private static int GetResolution(VideoQuality quality)
        {
            switch (quality)
            {
                case VideoQuality.Unknown:
                    return -1;
                case VideoQuality.Quality144P:
                    return 144;
                case VideoQuality.Quality240P:
                    return 240;
                case VideoQuality.Quality270P:
                    return 270;
                case VideoQuality.Quality360P:
                    return 360;
                case VideoQuality.Quality480P:
                    return 480;
                case VideoQuality.Quality720P:
                    return 720;
                case VideoQuality.Quality1080P:
                    return 1080;
                default:
                    return -1;
            }
        }
    }
}
