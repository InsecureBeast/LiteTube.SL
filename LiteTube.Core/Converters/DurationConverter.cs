using System;
using System.Xml;

namespace LiteTube.Core.Converters
{
    class DurationConverter
    {
        public static TimeSpan Convert (string duration)
        {
            try
            {
                var timespan = XmlConvert.ToTimeSpan(duration);
                return timespan;
            }
            catch (Exception)
            {
                return TimeSpan.FromSeconds(0);
            }
        }
    }
}
