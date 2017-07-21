// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.TsTransportStreamDescriptionTable
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.TransportStream.TsParser.Descriptor;
using System;

namespace SM.Media.TransportStream.TsParser
{
  public class TsTransportStreamDescriptionTable : TsProgramSpecificInformation
  {
    private const int MinimumDescriptionTableLength = 5;
    private readonly ITsDescriptorFactory _descriptorFactory;

    public TsTransportStreamDescriptionTable(ITsDescriptorFactory descriptorFactory)
      : base(TsProgramSpecificInformation.TsTableId.TS_description_section)
    {
      if (null == descriptorFactory)
        throw new ArgumentNullException("descriptorFactory");
      this._descriptorFactory = descriptorFactory;
    }

    protected override void ParseSection(TsPacket packet, int offset, int length)
    {
      if (length < 5)
        return;
      int num1 = offset;
      byte[] buffer = packet.Buffer;
      int num2 = num1 + length;
      int num3 = num1 + 2;
      byte[] numArray1 = buffer;
      int index1 = num3;
      int num4 = 1;
      int num5 = index1 + num4;
      byte num6 = numArray1[index1];
      int num7 = (int) num6 & 1;
      byte num8 = (byte) ((int) num6 >> 1 & 31);
      byte[] numArray2 = buffer;
      int index2 = num5;
      int num9 = 1;
      int num10 = index2 + num9;
      byte num11 = numArray2[index2];
      byte[] numArray3 = buffer;
      int index3 = num10;
      int num12 = 1;
      int offset1 = index3 + num12;
      byte num13 = numArray3[index3];
      TsDescriptors.Parse(this._descriptorFactory, buffer, offset1, num2 - offset1);
    }
  }
}
