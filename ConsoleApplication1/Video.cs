﻿using System;
using System.IO;
using System.Text;

namespace LiteTube.LibVideo
{
    public abstract class Video
    {
        internal Video()
        {
        }

        public abstract string Title { get; }
        public virtual VideoFormat Format => VideoFormat.Unknown;
        // public virtual AudioFormat AudioFormat => AudioFormat.Unknown;


        public virtual string FileExtension
        {
            get
            {
                switch (Format)
                {
                    case VideoFormat.Flash: return ".flv";
                    case VideoFormat.Mobile: return ".3gp";
                    case VideoFormat.Mp4: return ".mp4";
                    case VideoFormat.WebM: return ".webm";
                    case VideoFormat.Unknown: return string.Empty;
                    default:
                        throw new NotImplementedException($"Format {Format} is unrecognized! Please file an issue at libvideo on GitHub.");
                }
            }
        }

        public string FullName
        {
            get
            {
                var builder = new StringBuilder(Title).Append(FileExtension);
                foreach (char bad in Path.GetInvalidFileNameChars())
                {
                    builder.Replace(bad, '_');
                }
                return builder.ToString();
            }
        }
    }
}
