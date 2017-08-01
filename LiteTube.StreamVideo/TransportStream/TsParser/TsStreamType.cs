using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LiteTube.StreamVideo.TransportStream.TsParser
{
  public class TsStreamType : IEquatable<TsStreamType>
  {
    public static readonly byte H262StreamType = 2;
    public static readonly byte Mp3Iso11172 = 3;
    public static readonly byte Mp3Iso13818 = 4;
    public static readonly byte AacStreamType = 15;
    public static readonly byte H264StreamType = 27;
    public static readonly byte Ac3StreamType = 129;
    private static readonly Dictionary<byte, TsStreamType> Types = Enumerable.ToDictionary<TsStreamType, byte>((IEnumerable<TsStreamType>) new TsStreamType[34]
    {
      new TsStreamType((byte) 0, TsStreamType.StreamContents.Reserved, "ITU-T|ISO/IEC Reserved"),
      new TsStreamType((byte) 1, TsStreamType.StreamContents.Video, "ISO/IEC 11172-2 Video"),
      new TsStreamType(TsStreamType.H262StreamType, TsStreamType.StreamContents.Video, "ITU-T Rec. H.262 | ISO/IEC 13818-2 Video or ISO/IEC 11172-2 constrained parameter video stream")
      {
        FileExtension = ".h262"
      },
      new TsStreamType(TsStreamType.Mp3Iso11172, TsStreamType.StreamContents.Audio, "ISO/IEC 11172-3 Audio")
      {
        FileExtension = ".mp3"
      },
      new TsStreamType(TsStreamType.Mp3Iso13818, TsStreamType.StreamContents.Audio, "ISO/IEC 13818-3 Audio")
      {
        FileExtension = ".mp3"
      },
      new TsStreamType((byte) 5, TsStreamType.StreamContents.Other, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 private_sections"),
      new TsStreamType((byte) 6, TsStreamType.StreamContents.Other, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 PES packets containing private data"),
      new TsStreamType((byte) 7, TsStreamType.StreamContents.Other, "ISO/IEC 13522 MHEG"),
      new TsStreamType((byte) 8, TsStreamType.StreamContents.Other, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Annex A DSM-CC"),
      new TsStreamType((byte) 9, TsStreamType.StreamContents.Other, "ITU-T Rec. H.222.1"),
      new TsStreamType((byte) 10, TsStreamType.StreamContents.Other, "ISO/IEC 13818-6 type A"),
      new TsStreamType((byte) 11, TsStreamType.StreamContents.Other, "ISO/IEC 13818-6 type B"),
      new TsStreamType((byte) 12, TsStreamType.StreamContents.Other, "ISO/IEC 13818-6 type C"),
      new TsStreamType((byte) 13, TsStreamType.StreamContents.Other, "ISO/IEC 13818-6 type D"),
      new TsStreamType((byte) 14, TsStreamType.StreamContents.Other, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 auxiliary"),
      new TsStreamType(TsStreamType.AacStreamType, TsStreamType.StreamContents.Audio, "ISO/IEC 13818-7 Audio with ADTS transport syntax")
      {
        FileExtension = ".aac"
      },
      new TsStreamType((byte) 16, TsStreamType.StreamContents.Other, "ISO/IEC 14496-2 Visual"),
      new TsStreamType((byte) 17, TsStreamType.StreamContents.Audio, "ISO/IEC 14496-3 Audio with the LATM transport syntax as defined in ISO/IEC 14496-3"),
      new TsStreamType((byte) 18, TsStreamType.StreamContents.Other, "ISO/IEC 14496-1 SL-packetized stream or FlexMux stream carried in PES packets"),
      new TsStreamType((byte) 19, TsStreamType.StreamContents.Other, "ISO/IEC 14496-1 SL-packetized stream or FlexMux stream carried in ISO/IEC 14496_sections"),
      new TsStreamType((byte) 20, TsStreamType.StreamContents.Other, "ISO/IEC 13818-6 Synchronized Download Protocol"),
      new TsStreamType((byte) 21, TsStreamType.StreamContents.Other, "Metadata carried in PES packets"),
      new TsStreamType((byte) 22, TsStreamType.StreamContents.Other, "Metadata carried in metadata_sections"),
      new TsStreamType((byte) 23, TsStreamType.StreamContents.Other, "Metadata carried in ISO/IEC 13818-6 Data Carousel"),
      new TsStreamType((byte) 24, TsStreamType.StreamContents.Other, "Metadata carried in ISO/IEC 13818-6 Object Carousel"),
      new TsStreamType((byte) 25, TsStreamType.StreamContents.Other, "Metadata carried in ISO/IEC 13818-6 Synchronized Download Protocol"),
      new TsStreamType((byte) 26, TsStreamType.StreamContents.Other, "IPMP stream (defined in ISO/IEC 13818-11, MPEG-2 IPMP)"),
      new TsStreamType(TsStreamType.H264StreamType, TsStreamType.StreamContents.Video, "AVC video stream conforming to one or more profiles defined in Annex A of ITU-T Rec. H.264 | ISO/IEC 14496-10 or AVC video sub-bitstream as defined in 2.1.78")
      {
        FileExtension = ".h264"
      },
      new TsStreamType((byte) 28, TsStreamType.StreamContents.Audio, "ISO/IEC 14496-3 Audio, without using any additional transport syntax, such as DST, ALS and SLS"),
      new TsStreamType((byte) 29, TsStreamType.StreamContents.Other, "ISO/IEC 14496-17 Text"),
      new TsStreamType((byte) 30, TsStreamType.StreamContents.Video, "Auxiliary video stream as defined in ISO/IEC 23002-3"),
      new TsStreamType((byte) 31, TsStreamType.StreamContents.Video, "SVC video sub-bitstream of an AVC video stream conforming to one or more profiles defined in Annex G of ITU-T Rec. H.264 | ISO/IEC 14496-10"),
      new TsStreamType((byte) 127, TsStreamType.StreamContents.Other, "IPMP stream"),
      new TsStreamType(TsStreamType.Ac3StreamType, TsStreamType.StreamContents.Audio, "Dolby AC-3")
      {
        FileExtension = ".ac3"
      }
    }, (Func<TsStreamType, byte>) (v => v.StreamType));
    private static readonly Dictionary<byte, TsStreamType> UnknownTypes = new Dictionary<byte, TsStreamType>();

    public byte StreamType { get; private set; }

    public TsStreamType.StreamContents Contents { get; private set; }

    public string Description { get; private set; }

    public string FileExtension { get; private set; }

    public TsStreamType(byte streamType, TsStreamType.StreamContents contents, string description)
    {
      this.StreamType = streamType;
      this.Contents = contents;
      this.Description = description;
    }

    public bool Equals(TsStreamType other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      return (int) this.StreamType == (int) other.StreamType;
    }

    public static TsStreamType FindStreamType(byte streamType)
    {
      TsStreamType tsStreamType;
      if (TsStreamType.Types.TryGetValue(streamType, out tsStreamType))
        return tsStreamType;
      bool lockTaken = false;
      Dictionary<byte, TsStreamType> dictionary = null;
      try
      {
        Monitor.Enter((object) (dictionary = TsStreamType.UnknownTypes), ref lockTaken);
        if (!TsStreamType.UnknownTypes.TryGetValue(streamType, out tsStreamType))
        {
          TsStreamType.StreamContents contents = TsStreamType.StreamContents.Unknown;
          if ((int) streamType >= 128)
            contents = TsStreamType.StreamContents.Private;
          else if ((int) streamType >= 32 && (int) streamType <= 126)
            contents = TsStreamType.StreamContents.Reserved;
          tsStreamType = new TsStreamType(streamType, contents, string.Format("Stream type {0:X2} ({1})", new object[2]
          {
            (object) streamType,
            (object) contents
          }));
          TsStreamType.UnknownTypes[streamType] = tsStreamType;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
      return tsStreamType;
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as TsStreamType);
    }

    public override int GetHashCode()
    {
      return (int) this.StreamType;
    }

    public override string ToString()
    {
      return string.Format("0x{0:x2}/{1}/{2}", (object) this.StreamType, (object) this.Contents, (object) this.Description);
    }

    public enum StreamContents
    {
      Unknown,
      Audio,
      Video,
      Other,
      Private,
      Reserved,
    }
  }
}
