using SM.Media.Core.TransportStream.TsParser.Utility;

namespace SM.Media.Core.TransportStream.TsParser
{
  public abstract class TsProgramSpecificInformation
  {
    private const int MinimumPsiSize = 4;
    private const int CrcSize = 4;
    private const int MaximumSectionLength = 1021;
    private readonly byte _tableId;

    protected TsProgramSpecificInformation(TsProgramSpecificInformation.TsTableId tableId)
    {
      this._tableId = (byte) tableId;
    }

    internal void Add(TsPacket packet)
    {
      if (null == packet)
        return;
      int bufferOffset = packet.BufferOffset;
      int num1 = bufferOffset;
      byte[] buffer = packet.Buffer;
      int bufferLength = packet.BufferLength;
      if (bufferLength < 4)
        return;
      byte[] numArray1 = buffer;
      int index1 = num1;
      int num2 = 1;
      int num3 = index1 + num2 + (int) numArray1[index1];
      if (num3 + 4 >= bufferOffset + bufferLength)
        return;
      int offset1 = num3;
      byte[] numArray2 = buffer;
      int index2 = num3;
      int num4 = 1;
      int index3 = index2 + num4;
      if ((int) this._tableId != (int) numArray2[index2])
        return;
      int num5 = (int) buffer[index3] << 8 | (int) buffer[index3 + 1];
      int num6 = index3 + 2;
      bool flag = 0 != (num5 & 32768);
      if (0 != (num5 & 16384))
        return;
      int num7 = num5 & 4095;
      if (num7 > 1021 || num7 + num6 - bufferOffset > bufferLength)
        return;
      int length1 = num7 + num6 - offset1;
      if (!Crc32Msb.Validate(buffer, offset1, length1))
        return;
      int offset2 = num6;
      int length2 = num7 - 4;
      this.ParseSection(packet, offset2, length2);
    }

    protected abstract void ParseSection(TsPacket packet, int offset, int length);

    public enum TsTableId : byte
    {
      program_association_section,
      conditional_access_section,
      TS_program_map_section,
      TS_description_section,
      ISO_IEC_14496_scene_description_section,
      ISO_IEC_14496_object_descriptor_section,
      Metadata_section,
      IPMP_Control_Information_section,
    }
  }
}
