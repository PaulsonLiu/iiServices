using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace iiFramework.Util
{
    /// <summary>
    /// 位置返回信息
    /// </summary>
    [Serializable()]
    public class Response
    {
        [XmlElement]
        public string IP { get; set; }
        [XmlElement]
        public string CountryCode { get; set; }
        [XmlElement]
        public string CountryName { get; set; }
        [XmlElement]
        public string RegionCode { get; set; }
        [XmlElement]
        public string RegionName { get; set; }
        [XmlElement]
        public string City { get; set; }
        [XmlElement]
        public string ZipCode { get; set; }
        [XmlElement]
        public string TimeZone { get; set; }
        [XmlElement]
        public string Latitude { get; set; }
        [XmlElement]
        public string Longitude { get; set; }
        [XmlElement]
        public string MetroCode { get; set; }

    }
}
