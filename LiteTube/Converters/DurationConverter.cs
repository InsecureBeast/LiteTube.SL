using System;
using System.Xml;

namespace LiteTube.Common
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
