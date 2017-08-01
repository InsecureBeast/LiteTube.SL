using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.M3U8;
using LiteTube.StreamVideo.M3U8.AttributeSupport;
using LiteTube.StreamVideo.Playlists;
using LiteTube.StreamVideo.Utility;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Hls
{
    public class HlsProgramManager : IProgramManager
    {
        private static readonly IDictionary<long, Program> _noPrograms = new Dictionary<long, Program>();
        private readonly IHlsProgramStreamFactory _programStreamFactory;
        private readonly IRetryManager _retryManager;
        private readonly IWebReaderManager _webReaderManager;
        private IWebReader _playlistWebReader;

        public ICollection<Uri> Playlists { get; set; }

        public HlsProgramManager(IHlsProgramStreamFactory programStreamFactory, IWebReaderManager webReaderManager, IRetryManager retryManager)
        {
            if (null == programStreamFactory)
                throw new ArgumentNullException(nameof(programStreamFactory));
            if (null == webReaderManager)
                throw new ArgumentNullException(nameof(webReaderManager));
            if (null == retryManager)
                throw new ArgumentNullException(nameof(retryManager));
            _programStreamFactory = programStreamFactory;
            _webReaderManager = webReaderManager;
            _retryManager = retryManager;
        }

        public async Task<IDictionary<long, Program>> LoadAsync(CancellationToken cancellationToken)
        {
            var playlists = Playlists;
            foreach (var uri in playlists)
            {
                try
                {
                    var parser = new M3U8Parser();
                    _playlistWebReader?.Dispose();
                    _playlistWebReader = _webReaderManager.CreateReader(uri, ContentKind.Playlist);
                    var actualPlaylist = await parser.ParseAsync(_playlistWebReader, _retryManager, uri, cancellationToken).ConfigureAwait(false);
                    var dictionary = await LoadAsync(_playlistWebReader, parser, cancellationToken);
                    return dictionary;
                }
                catch (WebException ex)
                {
                    Debug.WriteLine("HlsProgramManager.LoadAsync: " + ex.Message);
                }
            }
            return _noPrograms;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private async Task<IDictionary<long, Program>> LoadAsync(IWebReader webReader, M3U8Parser parser, CancellationToken cancellationToken)
        {
            var audioStreams = new Dictionary<string, MediaGroup>();
            foreach (var m3U8TagInstance in parser.GlobalTags)
            {
                if (M3U8Tags.ExtXMedia != m3U8TagInstance.Tag)
                    continue;

                try
                {
                    if (null != m3U8TagInstance.Attribute(ExtMediaSupport.AttrType, "AUDIO"))
                        AddMedia(parser.BaseUrl, m3U8TagInstance, audioStreams);
                }
                catch (NullReferenceException)
                {
                    ;
                }
            }

            var programs = new Dictionary<long, Program>();
            var hasSegments = false;
            foreach (var m3U8Uri in parser.Playlist)
            {
                if (m3U8Uri.Tags == null || m3U8Uri.Tags.Length < 1)
                {
                    hasSegments = true;
                }
                else
                {
                    var streamInfTagInstance = M3U8Tags.ExtXStreamInf.Find(m3U8Uri.Tags);
                    var programId = long.MinValue;
                    MediaGroup mediaGroup = null;
                    if (null != streamInfTagInstance)
                    {
                        var attributeValueInstance1 = streamInfTagInstance.Attribute(ExtStreamInfSupport.AttrProgramId);
                        if (null != attributeValueInstance1)
                            programId = attributeValueInstance1.Value;

                        var key = streamInfTagInstance.AttributeObject(ExtStreamInfSupport.AttrAudio);
                        if (null != key)
                            audioStreams.TryGetValue(key, out mediaGroup);

                        var uri = parser.ResolveUrl(m3U8Uri.Uri);
                        var attributeValueInstance2 = streamInfTagInstance.Attribute(ExtStreamInfSupport.AttrBandwidth);
                        var attributeInstance = streamInfTagInstance.AttributeInstance<ResolutionAttributeInstance>(ExtStreamInfSupport.AttrResolution);
                        var baseUrl = parser.BaseUrl;
                        var program = GetProgram(programs, programId, baseUrl);
                        var hlsProgramStream = _programStreamFactory.Create(new [] { uri }, webReader);
                        var playlistSubProgram1 = new PlaylistSubProgram(program, hlsProgramStream);
                        playlistSubProgram1.Bandwidth = attributeValueInstance2?.Value ?? 0L;
                        playlistSubProgram1.Playlist = uri;
                        playlistSubProgram1.AudioGroup = mediaGroup;
                        var playlistSubProgram2 = playlistSubProgram1;
                        if (null != attributeInstance)
                        {
                            playlistSubProgram2.Width = attributeInstance.X;
                            playlistSubProgram2.Height = attributeInstance.Y;
                        }
                        program.SubPrograms.Add(playlistSubProgram2);
                    }
                    else
                        hasSegments = true;
                }
            }
            if (hasSegments)
            {
                var program = GetProgram(programs, long.MinValue, parser.BaseUrl);
                var hlsProgramStream = _programStreamFactory.Create(new [] { webReader.RequestUri }, webReader);
                await hlsProgramStream.SetParserAsync(parser, cancellationToken).ConfigureAwait(false);
                var subProgram = new PlaylistSubProgram(program, hlsProgramStream);
                program.SubPrograms.Add(subProgram);
            }
            return programs;
        }

        private static Program GetProgram(IDictionary<long, Program> programs, long programId, Uri programUrl)
        {
            Program program;
            if (!programs.TryGetValue(programId, out program))
            {
                program = new Program()
                {
                    PlaylistUrl = programUrl,
                    ProgramId = programId
                };
                programs[programId] = program;
            }
            return program;
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        private static void AddMedia(Uri playlist, M3U8TagInstance gt, IDictionary<string, MediaGroup> audioStreams)
        {
            var key = gt.Attribute(ExtMediaSupport.AttrGroupId).Value;
            var uriString = gt.AttributeObject(ExtMediaSupport.AttrUri);
            Uri uri = null;
            if (null != uriString)
                uri = new Uri(playlist, new Uri(uriString, UriKind.RelativeOrAbsolute));

            var str = gt.AttributeObject(ExtMediaSupport.AttrLanguage);
            var playlistSubStream1 = new PlaylistSubStream
            {
                Type = gt.AttributeObject(ExtMediaSupport.AttrType),
                Name = key,
                Playlist = uri,
                IsAutoselect = IsYesNo(gt, ExtMediaSupport.AttrAutoselect),
                Language = str?.Trim().ToLower()
            };

            var playlistSubStream2 = playlistSubStream1;
            MediaGroup mediaGroup;
            if (!audioStreams.TryGetValue(key, out mediaGroup))
            {
                mediaGroup = new MediaGroup()
                {
                    Default = playlistSubStream2
                };
                audioStreams[key] = mediaGroup;
            }
            if (IsYesNo(gt, ExtMediaSupport.AttrDefault))
                mediaGroup.Default = playlistSubStream2;
            var index = gt.Attribute(ExtMediaSupport.AttrName).Value;
            mediaGroup.Streams[index] = playlistSubStream2;
        }

        private static bool IsYesNo(M3U8TagInstance tag, M3U8ValueAttribute<string> attribute, bool defaultValue = false)
        {
            var attributeValueInstance = tag.Attribute(attribute);
            if (attributeValueInstance == null || string.IsNullOrWhiteSpace(attributeValueInstance.Value))
                return defaultValue;
            return 0 == string.CompareOrdinal("YES", attributeValueInstance.Value.ToUpperInvariant());
        }

        public class MediaGroup
        {
            public readonly IDictionary<string, SubStream> Streams = new Dictionary<string, SubStream>();

            public SubStream Default { get; set; }
        }
    }
}
