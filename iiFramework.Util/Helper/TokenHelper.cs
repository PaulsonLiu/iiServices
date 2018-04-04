using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Author: Paulson Liu
/// Description: Token Helper
/// Create Time: 2018/1/18
/// Change Time: 2018/1/18
/// </summary>
namespace iiFramework.Util
{
    public static class TokenHelper
    {
        public static SimpleTokenClass DecodeSimpleToken(string token)
        {
            return Decode<SimpleTokenClass>(token);
        }

        public static string GetToken(object value)
        {
            string jsonValue = JsonConvert.SerializeObject(value);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonValue));
        }

        public static T Decode<T>(string token)
        {
            var decodebyte = Convert.FromBase64String(token);
            string decodeStr = Encoding.UTF8.GetString(decodebyte);
            return JsonConvert.DeserializeObject<T>(decodeStr);
        }

    }

    public class SimpleTokenClass
    {
        public string apikey { get; set; }
        public int timestamp { get; set; }
    }

    public class Token
    {
        /// <summary>
        /// App密钥
        /// </summary>
        public string appKey { get; set; }
        /// <summary>
        /// 用户名对应签名Token
        /// </summary>
        public Guid SignToken { get; set; }
        /// <summary>
        /// Token过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
    }

}