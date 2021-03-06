﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.TagSupport.ExtStreamInfTagInstance
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using SM.Media.M3U8.AttributeSupport;
using System.Collections.Generic;

namespace SM.Media.M3U8.TagSupport
{
  public sealed class ExtStreamInfTagInstance : AttributesTagInstance
  {
    internal ExtStreamInfTagInstance(M3U8Tag tag, IEnumerable<M3U8AttributeInstance> attributes)
      : base(tag, attributes)
    {
    }

    public static ExtStreamInfTagInstance Create(M3U8Tag tag, string value)
    {
      return new ExtStreamInfTagInstance(tag, AttributesTagInstance.ParseAttributes(value, ExtStreamInfSupport.Attributes));
    }
  }
}
