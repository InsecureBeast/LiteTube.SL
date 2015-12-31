using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace LiteTube.Common
{
    class DurationConverter
    {
        public static TimeSpan Convert (string duration)
        {
            var timespan = XmlConvert.ToTimeSpan(duration);
            return timespan;
        }
    }
}
