using System;
using SM.Media.Core.TransportStream.TsParser.Descriptor;

namespace SM.Media.Core.TransportStream.TsParser
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
