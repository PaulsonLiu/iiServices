using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiService.Models
{

    /// <summary>
    /// 经纬度信息（参照google地图）
    /// </summary>
    public class LatlngInfo
    {
        public AddressComponent[] address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
    }


    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }

    public class Geometry
    {
        public object bounds { get; set; }
        public object location { get; set; }
        public string location_type { get; set; }
        public object viewport { get; set; }
        public string place_id { get; set; }
        public string[] types { get; set; }
    }

    public class LatlngInfoResponse
    {
        public LatlngInfo[] results { get; set; }
        public string status { get; set; }
    }

}
