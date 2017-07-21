// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.Descriptor.TsIso639LanguageDescriptor
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.TransportStream.TsParser.Descriptor
{
  public class TsIso639LanguageDescriptor : TsDescriptor
  {
    public static readonly TsDescriptorType DescriptorType = new TsDescriptorType((byte) 10, "ISO 639 language");
    private readonly TsIso639LanguageDescriptor.Language[] _languages;

    public TsIso639LanguageDescriptor.Language[] Languages
    {
      get
      {
        return this._languages;
      }
    }

    public TsIso639LanguageDescriptor(TsIso639LanguageDescriptor.Language[] languages)
      : base(TsIso639LanguageDescriptor.DescriptorType)
    {
      if (null == languages)
        throw new ArgumentNullException("languages");
      this._languages = languages;
    }

    private enum AudioType
    {
      Undefined,
      Clean_effects,
      Hearing_impaired,
      Visual_impaired_commentary,
    }

    public class Language
    {
      private readonly byte _audioType;
      private readonly string _iso639;

      public string Iso639_2
      {
        get
        {
          return this._iso639;
        }
      }

      public byte AudioType
      {
        get
        {
          return this._audioType;
        }
      }

      public Language(string iso639, byte audioType)
      {
        if (iso639 == null)
          throw new ArgumentNullException("iso639");
        this._iso639 = iso639;
        this._audioType = audioType;
      }
    }
  }
}
