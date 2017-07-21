// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.Descriptor.TsIso639LanguageDescriptorFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Utility.TextEncodings;
using System;
using System.Text;

namespace SM.Media.TransportStream.TsParser.Descriptor
{
  public class TsIso639LanguageDescriptorFactory : TsDescriptorFactoryInstanceBase
  {
    private const int BlockLength = 4;
    private readonly Encoding _latin1;

    public TsIso639LanguageDescriptorFactory(ISmEncodings smEncodings)
      : base(TsIso639LanguageDescriptor.DescriptorType)
    {
      if (null == smEncodings)
        throw new ArgumentNullException("smEncodings");
      this._latin1 = smEncodings.Latin1Encoding;
    }

    public override TsDescriptor Create(byte[] buffer, int offset, int length)
    {
      if (length < 4)
        return (TsDescriptor) null;
      int index = offset;
      TsIso639LanguageDescriptor.Language[] languages = new TsIso639LanguageDescriptor.Language[length / 4];
      int num = 0;
      while (length >= 4)
      {
        string @string = this._latin1.GetString(buffer, index, 3);
        byte audioType = buffer[3];
        languages[num++] = new TsIso639LanguageDescriptor.Language(@string, audioType);
        index += 4;
        length -= 4;
      }
      return (TsDescriptor) new TsIso639LanguageDescriptor(languages);
    }
  }
}
