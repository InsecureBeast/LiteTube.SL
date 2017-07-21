using System;
using System.Text;
using SM.Media.Core.Utility.TextEncodings;

namespace SM.Media.Core.TransportStream.TsParser.Descriptor
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
