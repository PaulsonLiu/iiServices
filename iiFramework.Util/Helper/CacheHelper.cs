using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace iiFramework.Util
{
    public static class CacheHelper
    {
        public static event EventHandler<CacheArgs> OnRemoveItem;
        public static void RaiseRemoveItem(CacheArgs args)
        {
            OnRemoveItem?.Invoke(null, args);
        }
        public static bool Insert(string key,object value, int expiredSecond = 600)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null) return false;
            //HttpRuntime.Cache.Insert(key, value, null, DateTime.Now.AddSeconds(expiredSecond), TimeSpan.Zero);//将值存入缓存中
            HttpRuntime.Cache.Insert(
                     key,
                     value,
                     null,
                     DateTime.Now.AddSeconds(expiredSecond),
                     Cache.NoSlidingExpiration,
                     CacheItemPriority.Default,
                     new CacheItemRemovedCallback(ReportRemovedCallback));
            return true;
        }

        public static object Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            return HttpRuntime.Cache.Get(key);
        }

        public static T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return default(T);
            return (T)HttpRuntime.Cache.Get(key);
        }

        public static T Remove<T>(string key)
        {
            return (T)HttpRuntime.Cache.Remove(key);
        }

        public static object Remove(string key)
        {
            return HttpRuntime.Cache.Remove(key);
        }

        /// <summary>
        /// 失效通知
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="removedReason"></param>
        public static void ReportRemovedCallback(String key, object value,  CacheItemRemovedReason removedReason)
        {
            RaiseRemoveItem(new CacheArgs() { key = key, value = value ,reson = removedReason});
        }
    }
}
