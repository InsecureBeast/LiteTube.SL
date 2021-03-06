﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteTube.StreamVideo.M3U8.AttributeSupport
{
  internal static class ExtIFrameStreamInfSupport
  {
    public static readonly M3U8ValueAttribute<string> AttrUri = new M3U8ValueAttribute<string>("URI", true, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    public static readonly M3U8ValueAttribute<long> AttrBandwidth = new M3U8ValueAttribute<long>("BANDWIDTH", true, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<long>>(M3U8AttributeSupport.DecimalIntegerParser));
    public static readonly M3U8ValueAttribute<long> AttrProgramId = new M3U8ValueAttribute<long>("PROGRAM-ID", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<long>>(M3U8AttributeSupport.DecimalIntegerParser));
    public static readonly M3U8ValueAttribute<IEnumerable<string>> AttrCodecs = new M3U8ValueAttribute<IEnumerable<string>>("CODECS", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<IEnumerable<string>>>(M3U8AttributeSupport.QuotedCsvParser));
    public static readonly M3U8Attribute AttrResolution = new M3U8Attribute("RESOLUTION", false, new Func<M3U8Attribute, string, M3U8AttributeInstance>(ResolutionAttributeInstance.Create));
    public static readonly M3U8ValueAttribute<string> AttrVideo = new M3U8ValueAttribute<string>("VIDEO", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    internal static readonly IDictionary<string, M3U8Attribute> Attributes = (IDictionary<string, M3U8Attribute>) Enumerable.ToDictionary<M3U8Attribute, string>((IEnumerable<M3U8Attribute>) new M3U8Attribute[6]
    {
      (M3U8Attribute) ExtIFrameStreamInfSupport.AttrUri,
      (M3U8Attribute) ExtIFrameStreamInfSupport.AttrBandwidth,
      (M3U8Attribute) ExtIFrameStreamInfSupport.AttrProgramId,
      (M3U8Attribute) ExtIFrameStreamInfSupport.AttrCodecs,
      ExtIFrameStreamInfSupport.AttrResolution,
      (M3U8Attribute) ExtIFrameStreamInfSupport.AttrVideo
    }, (Func<M3U8Attribute, string>) (a => a.Name));
  }
}
