﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiFramework.Util
{
    public class IpInfoResponse
    {
        public int code { get; set; }
        public IpInfo data { get; set; }
    }

    public class IpInfo
    {
        public string ip { get; set; }
        public string country { get; set; }
        public string area { get; set; }
        public string region { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string isp { get; set; }
        public string country_id { get; set; }
        public string area_id { get; set; }
        public string region_id { get; set; }
        public string city_id { get; set; }
        public string county_id { get; set; }
        public string isp_id { get; set; }
    }
}
