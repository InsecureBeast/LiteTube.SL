using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SM.Media.Core.M3U8.AttributeSupport;
using SM.Media.Core.M3U8.TagSupport;

namespace SM.Media.Core.M3U8
{
  public class M3U8Tags
  {
    public static readonly M3U8Tag ExtM3UMarker = new M3U8Tag("#EXTM3U", M3U8TagScope.Global, new Func<M3U8Tag, string, M3U8TagInstance>(M3U8AttributeSupport.CreateInstance));
    public static readonly M3U8ExtInfTag ExtXInf = new M3U8ExtInfTag("#EXTINF", M3U8TagScope.Segment);
    public static readonly M3U8ByterangeTag ExtXByteRange = new M3U8ByterangeTag("#EXT-X-BYTERANGE", M3U8TagScope.Segment);
    public static readonly M3U8ValueTag ExtXTargetDuration = new M3U8ValueTag("#EXT-X-TARGETDURATION", M3U8TagScope.Global, new Func<M3U8Tag, string, ValueTagInstance>(ValueTagInstance.CreateLong));
    public static readonly M3U8ValueTag ExtXMediaSequence = new M3U8ValueTag("#EXT-X-MEDIA-SEQUENCE", M3U8TagScope.Global, new Func<M3U8Tag, string, ValueTagInstance>(ValueTagInstance.CreateLong));
    public static readonly M3U8ExtKeyTag ExtXKey = new M3U8ExtKeyTag("#EXT-X-KEY", M3U8TagScope.Shared);
    public static readonly M3U8Tag ExtXProgramDateTime = (M3U8Tag) new M3U8DateTimeTag("#EXT-X-PROGRAM-DATE-TIME", M3U8TagScope.Segment);
    public static readonly M3U8Tag ExtXAllowCache = new M3U8Tag("#EXT-X-ALLOW-CACHE", M3U8TagScope.Global, new Func<M3U8Tag, string, M3U8TagInstance>(M3U8AttributeSupport.CreateInstance));
    public static readonly M3U8ValueTag ExtXPlaylistType = new M3U8ValueTag("#EXT-X-PLAYLIST-TYPE", M3U8TagScope.Global, (Func<M3U8Tag, string, ValueTagInstance>) ((tag, value) => ValueTagInstance.Create(tag, value, (Func<string, object>) (v => (object) v))));
    public static readonly M3U8Tag ExtXEndList = new M3U8Tag("#EXT-X-ENDLIST", M3U8TagScope.Global, new Func<M3U8Tag, string, M3U8TagInstance>(M3U8AttributeSupport.CreateInstance));
    public static readonly M3U8Tag ExtXMedia = (M3U8Tag) new M3U8AttributeTag("#EXT-X-MEDIA", M3U8TagScope.Global, ExtMediaSupport.Attributes, (Func<M3U8Tag, string, M3U8TagInstance>) ((tag, value) => AttributesTagInstance.Create(tag, value, ExtMediaSupport.Attributes)));
    public static readonly M3U8ExtStreamInfTag ExtXStreamInf = new M3U8ExtStreamInfTag("#EXT-X-STREAM-INF", M3U8TagScope.Segment);
    public static readonly M3U8Tag ExtXDiscontinuity = new M3U8Tag("#EXT-X-DISCONTINUITY", M3U8TagScope.Segment, new Func<M3U8Tag, string, M3U8TagInstance>(M3U8AttributeSupport.CreateInstance));
    public static readonly M3U8Tag ExtXIFramesOnly = new M3U8Tag("#EXT-X-I-FRAMES-ONLY", M3U8TagScope.Global, new Func<M3U8Tag, string, M3U8TagInstance>(M3U8AttributeSupport.CreateInstance));
    public static readonly M3U8Tag ExtXMap = new M3U8Tag("#EXT-X-MAP", M3U8TagScope.Shared, new Func<M3U8Tag, string, M3U8TagInstance>(MapTagInstance.Create));
    public static readonly M3U8Tag ExtXIFrameStreamInf = (M3U8Tag) new M3U8AttributeTag("#EXT-X-I-FRAME-STREAM-INF", M3U8TagScope.Global, ExtIFrameStreamInfSupport.Attributes, (Func<M3U8Tag, string, M3U8TagInstance>) ((tag, value) => AttributesTagInstance.Create(tag, value, ExtIFrameStreamInfSupport.Attributes)));
    public static readonly M3U8ValueTag ExtXVersion = new M3U8ValueTag("#EXT-X-VERSION", M3U8TagScope.Global, new Func<M3U8Tag, string, ValueTagInstance>(ValueTagInstance.CreateLong));
    public static readonly M3U8Tags Default = new M3U8Tags();
    private volatile Dictionary<string, M3U8Tag> _tags = Enumerable.ToDictionary<M3U8Tag, string>((IEnumerable<M3U8Tag>) new M3U8Tag[17]
    {
      M3U8Tags.ExtM3UMarker,
      (M3U8Tag) M3U8Tags.ExtXInf,
      (M3U8Tag) M3U8Tags.ExtXByteRange,
      (M3U8Tag) M3U8Tags.ExtXTargetDuration,
      (M3U8Tag) M3U8Tags.ExtXMediaSequence,
      (M3U8Tag) M3U8Tags.ExtXKey,
      M3U8Tags.ExtXProgramDateTime,
      M3U8Tags.ExtXAllowCache,
      (M3U8Tag) M3U8Tags.ExtXPlaylistType,
      M3U8Tags.ExtXEndList,
      M3U8Tags.ExtXMedia,
      (M3U8Tag) M3U8Tags.ExtXStreamInf,
      M3U8Tags.ExtXDiscontinuity,
      M3U8Tags.ExtXIFramesOnly,
      M3U8Tags.ExtXMap,
      M3U8Tags.ExtXIFrameStreamInf,
      (M3U8Tag) M3U8Tags.ExtXVersion
    }, (Func<M3U8Tag, string>) (t => t.Name));

    public void RegisterTag(IEnumerable<M3U8Tag> tags)
    {
      Dictionary<string, M3U8Tag> comparand;
      Dictionary<string, M3U8Tag> dictionary;
      do
      {
        comparand = this._tags;
        dictionary = new Dictionary<string, M3U8Tag>((IDictionary<string, M3U8Tag>) comparand);
        foreach (M3U8Tag m3U8Tag in tags)
          dictionary[m3U8Tag.Name] = m3U8Tag;
      }
      while (!object.ReferenceEquals((object) Interlocked.CompareExchange<Dictionary<string, M3U8Tag>>(ref this._tags, dictionary, comparand), (object) comparand));
    }

    public M3U8TagInstance Create(string tagName, string value)
    {
      M3U8Tag m3U8Tag;
      if (!this._tags.TryGetValue(tagName, out m3U8Tag))
        return (M3U8TagInstance) null;
      return m3U8Tag.CreateInstance(m3U8Tag, value);
    }
  }
}
