// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.Descriptor.TsDescriptors
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SM.Media.TransportStream.TsParser.Descriptor
{
  public static class TsDescriptors
  {
    public static IEnumerable<TsDescriptor> Parse(this ITsDescriptorFactory factory, byte[] buffer, int offset, int length)
    {
      while (length > 0)
      {
        if (length < 2)
        {
          Debug.WriteLine("Unused buffer " + (object) length);
          break;
        }
        byte code = buffer[offset];
        byte descriptorLength = buffer[offset + 1];
        offset += 2;
        length -= 2;
        if (length < (int) descriptorLength)
        {
          Debug.WriteLine(" " + (object) descriptorLength + " exceeds buffer (" + (string) (object) length + " remaining)");
          break;
        }
        TsDescriptor descriptor = factory.Create(code, buffer, offset, (int) descriptorLength);
        if (null != descriptor)
          yield return descriptor;
        length -= (int) descriptorLength;
        offset += (int) descriptorLength;
      }
    }

    public static void WriteDescriptors(TextWriter writer, byte[] buffer, int offset, int length)
    {
      while (length > 0)
      {
        if (length < 2)
        {
          writer.WriteLine("Unused buffer " + (object) length);
          break;
        }
        byte code = buffer[offset];
        byte num = buffer[offset + 1];
        offset += 2;
        length -= 2;
        TsDescriptorType descriptorType = TsDescriptorTypes.GetDescriptorType(code);
        if (null == descriptorType)
          writer.Write((string) (object) code + (object) ":Unknown");
        else
          writer.Write((object) descriptorType);
        if (length < (int) num)
        {
          writer.WriteLine(" " + (object) num + " exceeds buffer (" + (string) (object) length + " remaining)");
          break;
        }
        length -= (int) num;
        offset += (int) num;
      }
    }

    [Conditional("DEBUG")]
    public static void DebugWrite(byte[] buffer, int offset, int length)
    {
      using (StringWriter stringWriter = new StringWriter())
      {
        TsDescriptors.WriteDescriptors((TextWriter) stringWriter, buffer, offset, length);
        Debug.WriteLine(stringWriter.ToString());
      }
    }

    public static string GetDefaultLanguage(this IEnumerable<TsDescriptor> descriptors)
    {
      if (null == descriptors)
        return (string) null;
      TsIso639LanguageDescriptor languageDescriptor = Enumerable.FirstOrDefault<TsIso639LanguageDescriptor>(Enumerable.OfType<TsIso639LanguageDescriptor>((IEnumerable) descriptors));
      if (null == languageDescriptor || languageDescriptor.Languages.Length < 1)
        return (string) null;
      return languageDescriptor.Languages[0].Iso639_2;
    }
  }
}
