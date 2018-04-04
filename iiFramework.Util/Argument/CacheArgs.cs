using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;

namespace iiFramework.Util
{
    public class CacheArgs
    {
        public string key { get; set; }
        public object value { get; set; }
        public CacheItemRemovedReason reson { get; set; }
    }
}
