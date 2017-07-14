using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace LiteTube.DataClasses
{
    class WebVideo
    {
        public WebVideo(string xml)
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                XmlResolver = null
            };

            var serializer = new XmlSerializer(typeof (Feed));
            using (var stReader = new StringReader(xml))
            using (var reader = XmlReader.Create(stReader, settings))// new XmlReader(xml, settings))
            {
                try
                {
                    Feed = (Feed)serializer.Deserialize(reader);
                }
                catch (Exception)
                {
                    Feed = new Feed();
                }
            }
        }

        public Feed Feed { get; }

        /*<?xml version="1.0" encoding="utf-16"?>
         * <feed xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
         *       xmlns:xsd="http://www.w3.org/2001/XMLSchema" />*/
    }

    [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
    public class Feed
    {
        public Feed()
        {
            Entries = new List<Entry>();    
        }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("channelId", Namespace = "http://www.youtube.com/xml/schemas/2015")]
        public string ChannelId { get; set; }

        [XmlElement("title")]
        public string ChannelTitle { get; set; }

        [XmlElement("entry")]
        public List<Entry> Entries { get; set; }
    }

    public class Entry
    {
        public Entry()
        {
            Media = new Media();    
        }

        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("videoId", Namespace = "http://www.youtube.com/xml/schemas/2015")]
        public string VideoId { get; set; }

        [XmlElement("channelId", Namespace = "http://www.youtube.com/xml/schemas/2015")]
        public string ChannelId { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("published")]
        public string Published { get; set; }

        [XmlElement("updated")]
        public string Updated { get; set; }

        [XmlElement("group", Namespace = "http://search.yahoo.com/mrss/")]
        public Media Media { get; set; }
    }

    public class Media
    {
        public Media()
        {
            Community = new Community();    
        }

        //[XmlElement("thumbnail", Namespace = "http://search.yahoo.com/mrss/")]
        //public Thumbnail Thumbnail { get; set; }

        [XmlElement("community", Namespace = "http://search.yahoo.com/mrss/")]
        public Community Community { get; set; }
    }

    //public class Thumbnail
    //{
    //    [XmlAttribute("url", Namespace = "http://search.yahoo.com/mrss/")]
    //    public string url { get; set; }
    //}

    public class Community
    {
        public Community()
        {
            Statistics = new Statistics();    
        }

        [XmlElement("statistics", Namespace = "http://search.yahoo.com/mrss/")]
        public Statistics Statistics { get; set; }
    }

    public class Statistics
    {
        [XmlAttribute("views")]
        public string Views { get; set; }
    }

    /*
     <feed xmlns:yt="http://www.youtube.com/xml/schemas/2015" 
           xmlns:media="http://search.yahoo.com/mrss/"   
           xmlns="http://www.w3.org/2005/Atom">
        <link rel="self" href="http://www.youtube.com/feeds/videos.xml?channel_id=UCt7sv-NKh44rHAEb-qCCxvA"/>
        <id>yt:channel:UCt7sv-NKh44rHAEb-qCCxvA</id>
        <yt:channelId>UCt7sv-NKh44rHAEb-qCCxvA</yt:channelId>
        <title>Wylsacom</title>
        <link rel="alternate" href="https://www.youtube.com/channel/UCt7sv-NKh44rHAEb-qCCxvA"/>
        <author>
            <name>Wylsacom</name>
            <uri>https://www.youtube.com/channel/UCt7sv-NKh44rHAEb-qCCxvA</uri>
        </author>
        <published>2011-07-31T17:05:38+00:00</published>
        
        <entry>
            <id>yt:video:cZ04KCXNwko</id>
            <yt:videoId>cZ04KCXNwko</yt:videoId>
            <yt:channelId>UCt7sv-NKh44rHAEb-qCCxvA</yt:channelId>
            <title>Мини-сигвей Ninebot Mini Pro за 35000р. против карта</title>
            <link rel="alternate" href="https://www.youtube.com/watch?v=cZ04KCXNwko"/>
            <author>
                <name>Wylsacom</name>
                <uri>https://www.youtube.com/channel/UCt7sv-NKh44rHAEb-qCCxvA</uri>
            </author>
            <published>2017-06-27T17:02:31+00:00</published>
            <updated>2017-06-28T14:49:41+00:00</updated>
            <media:group>
                <media:title>Мини-сигвей Ninebot Mini Pro за 35000р. против карта</media:title>
                <media:content url="https://www.youtube.com/v/cZ04KCXNwko?version=3" type="application/x-shockwave-flash" width="640" height="390"/>
                <media:thumbnail url="https://i4.ytimg.com/vi/cZ04KCXNwko/hqdefault.jpg" width="480" height="360"/>
                <media:description>Как получить скидку в 1 000 рублей - указать в заказе комментарий &quot;Wylsacom&quot; или назвать этот код по телефону.
                    http://bit.ly/Ninebot_Mini_Pro
                    Илья - http://goo.gl/hyGYwu
                    Twitter: http://twitter.com/wylsacom
                    Instagram: http://instagram.com/wylsacom
                </media:description>
                <media:community>
                    <media:starRating count="13697" average="4.29" min="1" max="5"/>
                    <media:statistics views="258672"/>
                </media:community>
            </media:group>
        </entry>
        
    */


    public class EntryComparer : IComparer<Entry>, IComparer
    {
        public int Compare(Entry entry1, Entry entry2)
        {
            return DoCompare(entry1, entry2);
        }

        public int Compare(object x, object y)
        {
            var entry1 = x as Entry;
            var entry2 = y as Entry;
            return DoCompare(entry1, entry2);
        }

        private int DoCompare(Entry entry1, Entry entry2)
        {
            if (entry1 == null && entry2 == null)
                return 0;
            if (entry1 == null)
                return 1;
            if (entry2 == null)
                return -1;

            DateTime date1;
            DateTime.TryParse(entry1.Published, out date1);

            DateTime date2;
            DateTime.TryParse(entry2.Published, out date2);

            if (date1 == DateTime.MinValue && date2 == DateTime.MinValue)
                return 0;
            if (date1 == DateTime.MinValue)
                return 1;
            if (date2 == DateTime.MinValue)
                return -1;

            return date2.CompareTo(date1);
        }
    }
}
