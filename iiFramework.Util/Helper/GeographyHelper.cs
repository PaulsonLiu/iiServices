using iiService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace iiFramework.Util
{

    public static class GeographyHelper
    {
        /// <summary>
        /// 通过ip获取地理位置信息(freegeoip)
        /// </summary>
        /// <param name="ipStr"></param>
        /// <returns></returns>
        public static Response GetGeographyByIP(string ipStr)
        {
            Response data = new Response();
            string strURL = "http://www.freegeoip.net/xml/" + ipStr;   //网址URL

            try
            {
                XmlSerializer graXML = new XmlSerializer(typeof(Response));
                using (XmlReader reader = XmlReader.Create(strURL))
                {
                    data = (Response)graXML.Deserialize(reader);
                }
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 通过ip获取地理位置信息(taobao),无经纬度信息
        /// </summary>
        /// <param name="ipStr"></param>
        /// <returns></returns>
        public static IpInfo GetIpInfo(string ipStr)
        {
            string requestUrl = "http://ip.taobao.com/service/getIpInfo.php?ip=" + ipStr;   //网址URL
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            try
            {
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(requestUrl));
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                Stream st = httpResponse.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                string responseStr = reader.ReadToEnd();

                var response = JsonConvert.DeserializeObject<IpInfoResponse>(responseStr);
                return response.data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据经纬度获取地理信息
        /// </summary>
        /// <param name="latlng">纬度,经度</param>
        public static LatlngInfo[] GetLatlngInfo(string latlng)
        {
            //中文：http://maps.google.cn/maps/api/geocode/json?latlng=23.1354500000,113.2945500000&language=CN
            //英文：http://maps.google.cn/maps/api/geocode/json?latlng=23.1354500000,113.2945500000&language=EN

            LatlngInfo[] info = null;
            string requestUrl = $"http://maps.google.cn/maps/api/geocode/json?latlng={latlng}&language=CN";   //网址URL,连续访问多次会被限制
            //string responseStr = $"https://maps.google.cn/maps/api/geocode/json?latlng={latlng}&language=CN&key=AIzaSyAHaNkUelBXQwpErVupeg31psKAPwPU-Ck";   //网址URL，key为开发者apikey
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            try
            {
                int tryTime = 3;//三次重发获取
                while (tryTime > 0)
                {
                    httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(requestUrl));
                    httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    Stream st = httpResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                    string responseStr = reader.ReadToEnd();
                    var response = JsonConvert.DeserializeObject<LatlngInfoResponse>(responseStr);

                    if (response?.status != "OK")
                    {
                        tryTime--;
                        Thread.Sleep(500);
                    }
                    else
                    {
                        info = response.results;
                        break;
                    }
                }

                return info;
            }
            catch (Exception)
            {
                return info;
            }
        }

    }
}