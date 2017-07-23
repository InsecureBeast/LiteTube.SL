using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.M3U8;
using SM.Media.Core.M3U8.AttributeSupport;
using SM.Media.Core.M3U8.TagSupport;
using SM.Media.Core.Playlists;
using SM.Media.Core.Utility;
using SM.Media.Core.Web;

namespace SM.Media.Core.Hls
{
  public class HlsProgramManager : IProgramManager, IDisposable
  {
    private static readonly IDictionary<long, Program> NoPrograms = (IDictionary<long, Program>) new Dictionary<long, Program>();
    private readonly IHlsProgramStreamFactory _programStreamFactory;
    private readonly IRetryManager _retryManager;
    private readonly IWebReaderManager _webReaderManager;
    private IWebReader _playlistWebReader;

    public ICollection<Uri> Playlists { get; set; }

    public HlsProgramManager(IHlsProgramStreamFactory programStreamFactory, IWebReaderManager webReaderManager, IRetryManager retryManager)
    {
      if (null == programStreamFactory)
        throw new ArgumentNullException("programStreamFactory");
      if (null == webReaderManager)
        throw new ArgumentNullException("webReaderManager");
      if (null == retryManager)
        throw new ArgumentNullException("retryManager");
      this._programStreamFactory = programStreamFactory;
      this._webReaderManager = webReaderManager;
      this._retryManager = retryManager;
    }

    public async Task<IDictionary<long, Program>> LoadAsync(CancellationToken cancellationToken)
    {
      ICollection<Uri> playlists = this.Playlists;
      IDictionary<long, Program> dictionary;
      foreach (Uri uri in (IEnumerable<Uri>) playlists)
      {
        try
        {
          M3U8Parser parser = new M3U8Parser();
          if (null != this._playlistWebReader)
            this._playlistWebReader.Dispose();
          this._playlistWebReader = this._webReaderManager.CreateReader(uri, ContentKind.Playlist, (IWebReader) null, (ContentType) null);
          Uri actualPlaylist = await M3U8ParserExtensions.ParseAsync(parser, this._playlistWebReader, this._retryManager, uri, cancellationToken).ConfigureAwait(false);
          dictionary = await this.LoadAsync(this._playlistWebReader, parser, cancellationToken);
          goto label_14;
        }
        catch (WebException ex)
        {
          Debug.WriteLine("HlsProgramManager.LoadAsync: " + ex.Message);
        }
      }
      dictionary = HlsProgramManager.NoPrograms;
label_14:
      return dictionary;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private async Task<IDictionary<long, Program>> LoadAsync(IWebReader webReader, M3U8Parser parser, CancellationToken cancellationToken)
    {
      Dictionary<string, HlsProgramManager.MediaGroup> audioStreams = new Dictionary<string, HlsProgramManager.MediaGroup>();
      foreach (M3U8TagInstance m3U8TagInstance in parser.GlobalTags)
      {
        if (M3U8Tags.ExtXMedia == m3U8TagInstance.Tag)
        {
          try
          {
            if (null != M3U8TagInstanceExtensions.Attribute<string>(m3U8TagInstance, ExtMediaSupport.AttrType, "AUDIO"))
              HlsProgramManager.AddMedia(parser.BaseUrl, m3U8TagInstance, audioStreams);
          }
          catch (NullReferenceException ex)
          {
          }
        }
      }
      Dictionary<long, Program> programs = new Dictionary<long, Program>();
      bool hasSegments = false;
      foreach (M3U8Parser.M3U8Uri m3U8Uri in parser.Playlist)
      {
        if (m3U8Uri.Tags == null || m3U8Uri.Tags.Length < 1)
        {
          hasSegments = true;
        }
        else
        {
          ExtStreamInfTagInstance streamInfTagInstance = M3U8Tags.ExtXStreamInf.Find((IEnumerable<M3U8TagInstance>) m3U8Uri.Tags);
          long programId = long.MinValue;
          HlsProgramManager.MediaGroup mediaGroup = (HlsProgramManager.MediaGroup) null;
          if (null != streamInfTagInstance)
          {
            M3U8AttributeValueInstance<long> attributeValueInstance1 = M3U8TagInstanceExtensions.Attribute<long>((M3U8TagInstance) streamInfTagInstance, ExtStreamInfSupport.AttrProgramId);
            if (null != attributeValueInstance1)
              programId = attributeValueInstance1.Value;
            string key = M3U8TagInstanceExtensions.AttributeObject<string>((M3U8TagInstance) streamInfTagInstance, ExtStreamInfSupport.AttrAudio);
            if (null != key)
              audioStreams.TryGetValue(key, out mediaGroup);
            Uri uri = M3U8ParserExtensions.ResolveUrl(parser, m3U8Uri.Uri);
            M3U8AttributeValueInstance<long> attributeValueInstance2 = M3U8TagInstanceExtensions.Attribute<long>((M3U8TagInstance) streamInfTagInstance, ExtStreamInfSupport.AttrBandwidth);
            ResolutionAttributeInstance attributeInstance = M3U8TagInstanceExtensions.AttributeInstance<ResolutionAttributeInstance>((M3U8TagInstance) streamInfTagInstance, ExtStreamInfSupport.AttrResolution);
            Uri baseUrl = parser.BaseUrl;
            Program program = HlsProgramManager.GetProgram((IDictionary<long, Program>) programs, programId, baseUrl);
            IHlsProgramStream hlsProgramStream = _programStreamFactory.Create((ICollection<Uri>) new Uri[1]
            {
              uri
            }, webReader);
            PlaylistSubProgram playlistSubProgram1 = new PlaylistSubProgram(program, (IProgramStream) hlsProgramStream);
            playlistSubProgram1.Bandwidth = attributeValueInstance2 == null ? 0L : attributeValueInstance2.Value;
            playlistSubProgram1.Playlist = uri;
            playlistSubProgram1.AudioGroup = mediaGroup;
            PlaylistSubProgram playlistSubProgram2 = playlistSubProgram1;
            if (null != attributeInstance)
            {
              playlistSubProgram2.Width = new int?(attributeInstance.X);
              playlistSubProgram2.Height = new int?(attributeInstance.Y);
            }
            program.SubPrograms.Add((ISubProgram) playlistSubProgram2);
          }
          else
            hasSegments = true;
        }
      }
      if (hasSegments)
      {
        Program program = HlsProgramManager.GetProgram((IDictionary<long, Program>) programs, long.MinValue, parser.BaseUrl);
        IHlsProgramStream hlsProgramStream = this._programStreamFactory.Create((ICollection<Uri>) new Uri[1]
        {
          webReader.RequestUri
        }, webReader);
        await hlsProgramStream.SetParserAsync(parser, cancellationToken).ConfigureAwait(false);
        PlaylistSubProgram subProgram = new PlaylistSubProgram((IProgram) program, (IProgramStream) hlsProgramStream);
        program.SubPrograms.Add((ISubProgram) subProgram);
      }
      return (IDictionary<long, Program>) programs;
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
      if (disposing)
        ;
    }

    private static void AddMedia(Uri playlist, M3U8TagInstance gt, Dictionary<string, HlsProgramManager.MediaGroup> audioStreams)
    {
      string key = M3U8TagInstanceExtensions.Attribute<string>(gt, ExtMediaSupport.AttrGroupId).Value;
      string uriString = M3U8TagInstanceExtensions.AttributeObject<string>(gt, ExtMediaSupport.AttrUri);
      Uri uri = (Uri) null;
      if (null != uriString)
        uri = new Uri(playlist, new Uri(uriString, UriKind.RelativeOrAbsolute));
      string str = M3U8TagInstanceExtensions.AttributeObject<string>(gt, ExtMediaSupport.AttrLanguage);
      PlaylistSubStream playlistSubStream1 = new PlaylistSubStream();
      playlistSubStream1.Type = M3U8TagInstanceExtensions.AttributeObject<string>(gt, ExtMediaSupport.AttrType);
      playlistSubStream1.Name = key;
      playlistSubStream1.Playlist = uri;
      playlistSubStream1.IsAutoselect = HlsProgramManager.IsYesNo(gt, ExtMediaSupport.AttrAutoselect, false);
      playlistSubStream1.Language = str == null ? (string) null : str.Trim().ToLower();
      PlaylistSubStream playlistSubStream2 = playlistSubStream1;
      HlsProgramManager.MediaGroup mediaGroup;
      if (!audioStreams.TryGetValue(key, out mediaGroup))
      {
        mediaGroup = new HlsProgramManager.MediaGroup()
        {
          Default = (SubStream) playlistSubStream2
        };
        audioStreams[key] = mediaGroup;
      }
      if (HlsProgramManager.IsYesNo(gt, ExtMediaSupport.AttrDefault, false))
        mediaGroup.Default = (SubStream) playlistSubStream2;
      string index = M3U8TagInstanceExtensions.Attribute<string>(gt, ExtMediaSupport.AttrName).Value;
      mediaGroup.Streams[index] = (SubStream) playlistSubStream2;
    }

    private static bool IsYesNo(M3U8TagInstance tag, M3U8ValueAttribute<string> attribute, bool defaultValue = false)
    {
      M3U8AttributeValueInstance<string> attributeValueInstance = M3U8TagInstanceExtensions.Attribute<string>(tag, attribute);
      if (attributeValueInstance == null || string.IsNullOrWhiteSpace(attributeValueInstance.Value))
        return defaultValue;
      return 0 == string.CompareOrdinal("YES", attributeValueInstance.Value.ToUpperInvariant());
    }

    public class MediaGroup
    {
      public readonly IDictionary<string, SubStream> Streams = (IDictionary<string, SubStream>) new Dictionary<string, SubStream>();

      public SubStream Default { get; set; }
    }
  }
}
