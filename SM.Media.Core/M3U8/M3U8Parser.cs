using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Media.Core.M3U8
{
  public class M3U8Parser
  {
    private readonly List<M3U8TagInstance> _globalTags = new List<M3U8TagInstance>();
    private readonly List<M3U8Parser.M3U8Uri> _playlist = new List<M3U8Parser.M3U8Uri>();
    private readonly List<M3U8TagInstance> _sharedTags = new List<M3U8TagInstance>();
    private readonly List<M3U8TagInstance> _tags = new List<M3U8TagInstance>();
    private Uri _baseUrl;
    private bool _hasMarker;
    private int _lineNumber;

    public IEnumerable<M3U8TagInstance> GlobalTags
    {
      get
      {
        return (IEnumerable<M3U8TagInstance>) this._globalTags;
      }
    }

    public IEnumerable<M3U8Parser.M3U8Uri> Playlist
    {
      get
      {
        return (IEnumerable<M3U8Parser.M3U8Uri>) this._playlist;
      }
    }

    public Uri BaseUrl
    {
      get
      {
        return this._baseUrl;
      }
    }

    public bool HasMarker
    {
      get
      {
        return this._hasMarker;
      }
    }

    public Uri ResolveUrl(Uri url)
    {
      if (url.IsAbsoluteUri)
        return url;
      return new Uri(this._baseUrl, url);
    }

    public void Parse(Uri baseUri, IEnumerable<string> lines)
    {
      this._baseUrl = baseUri;
      this._playlist.Clear();
      this._lineNumber = 0;
      this._hasMarker = false;
      foreach (string line in lines)
      {
        ++this._lineNumber;
        if (!string.IsNullOrWhiteSpace(line))
        {
          if (line.StartsWith("#EXT"))
            this.HandleExt(line);
          else if (!line.StartsWith("#"))
          {
            M3U8Parser.M3U8Uri m3U8Uri = new M3U8Parser.M3U8Uri()
            {
              Uri = line
            };
            if (this._tags.Count > 0 || this._sharedTags.Count > 0)
            {
              m3U8Uri.Tags = Enumerable.ToArray<M3U8TagInstance>(Enumerable.Union<M3U8TagInstance>((IEnumerable<M3U8TagInstance>) this._tags, (IEnumerable<M3U8TagInstance>) this._sharedTags));
              this._tags.Clear();
            }
            this._playlist.Add(m3U8Uri);
          }
        }
      }
    }

    private void HandleExt(string line)
    {
      int length = line.IndexOf(':');
      string tagName = line;
      string str = (string) null;
      if (length > 3)
      {
        tagName = line.Substring(0, length);
        if (length + 1 < line.Length)
          str = line.Substring(length + 1);
      }
      M3U8TagInstance tagInstance = M3U8Tags.Default.Create(tagName, str);
      if (null == tagInstance)
        return;
      switch (tagInstance.Tag.Scope)
      {
        case M3U8TagScope.Global:
          if (1 == this._lineNumber && tagInstance.Tag == M3U8Tags.ExtM3UMarker)
            this._hasMarker = true;
          this._globalTags.Add(tagInstance);
          break;
        case M3U8TagScope.Shared:
          this.ResolveShared(tagInstance);
          break;
        case M3U8TagScope.Segment:
          this._tags.Add(tagInstance);
          break;
      }
    }

    private void ResolveShared(M3U8TagInstance tagInstance)
    {
      this._sharedTags.Add(tagInstance);
    }

    public class M3U8Uri
    {
      public M3U8TagInstance[] Tags;
      public string Uri;

      public override string ToString()
      {
        return this.Uri;
      }
    }
  }
}
