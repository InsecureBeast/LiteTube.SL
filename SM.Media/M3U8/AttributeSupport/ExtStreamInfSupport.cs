// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.AttributeSupport.ExtStreamInfSupport
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Media.M3U8.AttributeSupport
{
  public static class ExtStreamInfSupport
  {
    public static readonly M3U8ValueAttribute<long> AttrBandwidth = new M3U8ValueAttribute<long>("BANDWIDTH", true, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<long>>(M3U8AttributeSupport.DecimalIntegerParser));
    public static readonly M3U8ValueAttribute<long> AttrProgramId = new M3U8ValueAttribute<long>("PROGRAM-ID", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<long>>(M3U8AttributeSupport.DecimalIntegerParser));
    public static readonly M3U8ValueAttribute<IEnumerable<string>> AttrCodecs = new M3U8ValueAttribute<IEnumerable<string>>("CODECS", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<IEnumerable<string>>>(M3U8AttributeSupport.QuotedCsvParser));
    public static readonly M3U8Attribute AttrResolution = new M3U8Attribute("RESOLUTION", false, new Func<M3U8Attribute, string, M3U8AttributeInstance>(ResolutionAttributeInstance.Create));
    public static readonly M3U8ValueAttribute<string> AttrAudio = new M3U8ValueAttribute<string>("AUDIO", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    public static readonly M3U8ValueAttribute<string> AttrVideo = new M3U8ValueAttribute<string>("VIDEO", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    public static readonly M3U8ValueAttribute<string> AttrSubtitles = new M3U8ValueAttribute<string>("SUBTITLES", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    internal static readonly IDictionary<string, M3U8Attribute> Attributes = (IDictionary<string, M3U8Attribute>) Enumerable.ToDictionary<M3U8Attribute, string>((IEnumerable<M3U8Attribute>) new M3U8Attribute[7]
    {
      (M3U8Attribute) ExtStreamInfSupport.AttrBandwidth,
      (M3U8Attribute) ExtStreamInfSupport.AttrProgramId,
      (M3U8Attribute) ExtStreamInfSupport.AttrCodecs,
      ExtStreamInfSupport.AttrResolution,
      (M3U8Attribute) ExtStreamInfSupport.AttrAudio,
      (M3U8Attribute) ExtStreamInfSupport.AttrVideo,
      (M3U8Attribute) ExtStreamInfSupport.AttrSubtitles
    }, (Func<M3U8Attribute, string>) (a => a.Name));
  }
}
