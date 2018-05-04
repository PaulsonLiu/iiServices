using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Net;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Reflection;
using iiService.Models;
using Newtonsoft.Json;

namespace iiFramework.Util
{
    public class WebApiHelper : HttpClient
    {
        #region Ctor
        static WebApiHelper()
        {

        }
        private WebApiHelper()
        {
            IsInitialize = false;
            HeaderValue = "application/json";
        }
        #endregion

        #region Property
        private static WebApiHelper instance = null;

        public string BasicUri { get; set; }
        public string HeaderValue { get;private set; }

        public bool IsInitialize { get;private set; }
        public bool IsConnected { get; set; }
        public static string AppKey { get; set; } = AppConfigurationHelper.Config.AppKey;
        public static string tokenApiUrl { get; set; } = AppConfigurationHelper.Config.BUSysMgmtServer + "/api/Token/GetToken";
        #endregion

        #region Methods

        #region GetInstance
        public static WebApiHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new WebApiHelper();
                instance.MaxResponseContentBufferSize = 256000;
                instance.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            }
            return instance;
        }

        public static void CreateNewInstance()
        {
            instance = new WebApiHelper();
        }

        #endregion

        #region Initialize
        public void Initialize(string uriAddress, string headerValue)
        {
            //只能启动一次请求
            if (!instance.IsInitialize)
            {
                instance.BaseAddress = new Uri(uriAddress);
                instance.HeaderValue = headerValue;
                instance.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(headerValue));
                IsInitialize = true;
            }
        }

        public void Initialize(string IP, int Port)
        {
            //只能启动一次请求
            if (!instance.IsInitialize)
            {
                if (IP == null || Port == 0)
                {
                    throw new ArgumentNullException("IP and Port can not be null.");
                }

                BasicUri = "http://" + IP + ":" + Port.ToString();

                instance.BaseAddress = new Uri(BasicUri);
                instance.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(this.HeaderValue));
                IsInitialize = true;
            }
        }
        public void Initialize(string url)
        {
            //只能启动一次请求
            if (!instance.IsInitialize)
            {
                BasicUri = url;

                instance.BaseAddress = new Uri(BasicUri);
                instance.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(this.HeaderValue));
                IsInitialize = true;
            }
        }
        #endregion

        #region GetItems
        public async Task<List<T>> GetItems<T>(string ApiControlerUri)
        {
            List<T> result = new List<T>();
            try
            {
                if (!IsInitialize) return null;
                var response = await instance.GetAsync(ApiControlerUri);
                response.EnsureSuccessStatusCode(); // Throw on error code（有错误码时报出异常）.

                var items = await response.Content.ReadAsAsync<IEnumerable<T>>();
                if (items != null)
                {
                    result.CopyFrom(items);
                }
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                // This exception indicates a problem deserializing the request body.
                // 这个异常指明了一个解序列化请求体的问题。
                throw new Newtonsoft.Json.JsonException("Get items Error:" + jEx.Message);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Get items Error:" + ex.Message);
            }
            //finally
            //{
            //}
            return result;
        }
        #endregion

        #region Get
        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webApi"></param>
        /// <param name="queryStr"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        private static HttpResponseMsg Get<T>(string ApiUrl, string query, string queryStr, string appKey, bool sign = true)
        {
            string strResult = GetJson(ApiUrl, query, queryStr, appKey, sign);
            HttpResponseMsg resMsg = JsonConvert.DeserializeObject<HttpResponseMsg>(strResult);
            var resultT = JsonConvert.DeserializeObject<T>(resMsg.Data.ToString());
            resMsg.Data = resultT;

            return resMsg;
        } 
        #endregion

        #region GetItemsByParameters
        public async Task<List<T>> GetItemsByParameters<T>(string ApiControlerUri , string[] Parameters , string[] Values)
        {
            string apiString = ApiControlerUri + "/?";
            if (Parameters.Length != Values.Length) return null;

            int i = 0;
            foreach (var para in Parameters)
            {
                apiString = apiString + para + "=" + Values[i] + "&";
                i++;
            }
            apiString = apiString.Substring(0, apiString.Length - 1);//remove the last "&"

            return await GetItems<T>(apiString);
        }
        #endregion

        #region Get
        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webApi"></param>
        /// <param name="queryStr"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        //private static HttpResponseMsg Get<T>(string ApiUrl, string query, string queryStr, string appKey, bool sign = true)
        //{
        //    string strResult = GetJson(ApiUrl, query, queryStr, appKey, sign);
        //    HttpResponseMsg resMsg = JsonConvert.DeserializeObject<HttpResponseMsg>(strResult);
        //    var resultT = JsonConvert.DeserializeObject<T>(resMsg.Data.ToString());
        //    resMsg.Data = resultT;

        //    return resMsg;
        //}
        
        public static string GetJson(string ApiUrl, Dictionary<string, string> parames)
        {
            Tuple<string, string> parameters = WebApiHelper.GetQueryString(parames);

            return WebApiHelper.GetJson(ApiUrl, parameters.Item1, parameters.Item2,AppKey);
        }

        private static string GetJson(string ApiUrl, string query, string queryStr, string appKey, bool sign = true)
        {
            string token = System.Web.HttpContext.Current?.Request.Headers["token"];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiUrl + "?" + queryStr);
            string timeStamp = GetTimeStamp();
            string nonce = GetRandom();
            //加入头信息
            request.Headers.Add("appKey", appKey.ToString()); //当前请求用户appKey
            request.Headers.Add("timestamp", timeStamp); //发起请求时的时间戳（单位：毫秒）
            request.Headers.Add("nonce", nonce); //发起请求时的时间戳（单位：毫秒） 
            if (sign)
            {
                request.Headers.Add("signature", GetSignature(timeStamp, nonce, appKey, query)); //当前请求内容的数字签名
            }
            if (string.IsNullOrEmpty(token) && !string.IsNullOrWhiteSpace(UserContext.Current.token))
            {
                token = UserContext.Current.token;
            }
            request.Headers.Add("token", token);

            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 90000;
            request.Headers.Set("Pragma", "no-cache");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamReceive = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamReceive, Encoding.UTF8);
            string strResult = streamReader.ReadToEnd();

            streamReader.Close();
            streamReceive.Close();
            request.Abort();
            response.Close();

            return strResult;
        }
        #endregion

        #region Post
        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static string Post(string url, string data, Dictionary<string, string> parames, string mytoken ="")
        {
            string token = System.Web.HttpContext.Current?.Request.Headers["token"];
            Tuple<string, string> parameters = WebApiHelper.GetQueryString(parames);

            byte[] bytes = data == null ? new byte[0] : Encoding.UTF8.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?" + parameters.Item2);

            string timeStamp = GetTimeStamp();
            string nonce = GetRandom();
            //加入头信息
            request.Headers.Add("appKey", AppKey.ToString()); //当前请求用户appKey
            request.Headers.Add("timestamp", timeStamp); //发起请求时的时间戳（单位：毫秒）
            request.Headers.Add("nonce", nonce); //发起请求时的时间戳（单位：毫秒）
            request.Headers.Add("signature", GetSignature(timeStamp, nonce, AppKey, data)); //当前请求内容的数字签名

            if (string.IsNullOrEmpty(token) && !string.IsNullOrWhiteSpace(UserContext.Current.token))
            {
                token = UserContext.Current.token;
            }
            if (string.IsNullOrEmpty(token) && !string.IsNullOrWhiteSpace(mytoken))
            {
                token = mytoken;
            }
            request.Headers.Add("token", token);

            //写数据
            request.Method = "POST";
            request.ContentLength = bytes.Length;
            request.ContentType = "application/json";
            Stream reqstream = request.GetRequestStream();
            reqstream.Write(bytes, 0, bytes.Length);

            //读数据
            request.Timeout = 300000;
            request.Headers.Set("Pragma", "no-cache");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamReceive = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamReceive, Encoding.UTF8);
            string strResult = streamReader.ReadToEnd();

            //关闭流
            reqstream.Close();
            streamReader.Close();
            streamReceive.Close();
            request.Abort();
            response.Close();

            return strResult;
        }
        #endregion

        #region PostItem
        /// <summary>
        /// 增加一个条目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ApiControlerUri"></param>
        /// <param name="item"></param>
        public async Task<bool> PostItem<T>(string ApiControlerUri, T item)
        {
            try
            {
                if (IsInitialize)
                {
                    ApiControlerUri = ApiControlerUri + "/Post";
                    var response = await instance.PostAsJsonAsync<T>(ApiControlerUri, item);
                    response.EnsureSuccessStatusCode(); // Throw on error code（有错误码时抛出）. 
                    return true;
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Post item Error:" + ex.Message);
            }
            catch (System.FormatException)
            {
                throw new FormatException("Post Item Format error.");
            }
            //finally
            //{
                
            //}
        }
        #endregion

        #region PostItemRange
        /// <summary>
        /// 增加一个条目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ApiControlerUri"></param>
        /// <param name="items"></param>
        public async Task<bool> PostItemRange<T>(string ApiControlerUri, List<T> items)
        {
            try
            {
                if (IsInitialize)
                {
                    ApiControlerUri = ApiControlerUri + "/PostRange";
                    var response = await instance.PostAsJsonAsync(ApiControlerUri, items);
                    response.EnsureSuccessStatusCode(); // Throw on error code（有错误码时抛出）. 
                    return true;
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Post item Error:" + ex.Message);
            }
            catch (System.FormatException)
            {
                throw new FormatException("Post Item Format error.");
            }
            //finally
            //{
            //}
        }
        #endregion

        #region PostItemRangeNone
        /// <summary>
        /// 增加一个条目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ApiControlerUri"></param>
        /// <param name="items"></param>
        public async Task<bool> PostItemRangeNone<T>(string ApiControlerUri, List<T> items)
        {
            try
            {
                if (IsInitialize)
                {
                    //ApiControlerUri = ApiControlerUri + "/PostRange";
                    var response = await instance.PostAsJsonAsync(ApiControlerUri, items);
                    response.EnsureSuccessStatusCode(); // Throw on error code（有错误码时抛出）. 
                    return true;
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Post item Error:" + ex.Message);
            }
            catch (System.FormatException)
            {
                throw new FormatException("Post Item Format error.");
            }
            //finally
            //{
            //}
        }
        #endregion

        #region PutItem
        /// <summary>
        /// 修改一个条目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ApiControlerUri"></param>
        /// <param name="item"></param>
        public async Task<bool> PutItem<T>(string ApiControlerUri, T item)
        {
            try
            {
                if (IsInitialize)
                {
                    ApiControlerUri = ApiControlerUri + "/Put";
                    var response = await instance.PutAsJsonAsync<T>(ApiControlerUri, item);
                    response.EnsureSuccessStatusCode(); // Throw on error code（有错误码时抛出）. 
                    return true;
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Put item Error:" + ex.Message);
            }
            catch (System.FormatException)
            {
                throw new FormatException("Put Item Format error.");
            }
            //finally
            //{
            //}
        }
        #endregion

        #region DeleteItem
        /// <summary>
        /// 根据输入的值删除一个条目
        /// </summary>
        /// <param name="ApiControlerUri"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> DeleteItem(string ApiControlerUri)
        {
            try
            {
                if (IsInitialize)
                {
                    var response = await instance.DeleteAsync(ApiControlerUri);
                    response.EnsureSuccessStatusCode(); // Throw on error code（有错误码时抛出）. 
                    return true;
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Delete item Error:" + ex.Message);
            }
            catch (System.FormatException)
            {
                throw new FormatException("Delete Item Format error.");
            }
            //finally
            //{
            //}
        }
        #endregion

        #region DeleteItem by key and value
        /// <summary>
        /// 根据输入的值删除一个条目
        /// </summary>
        /// <param name="ApiControlerUri"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> DeleteItem(string ApiControlerUri, string key, string value)
        {
            try
            {
                if (IsInitialize)
                {
                    ApiControlerUri = ApiControlerUri + "/Delete";
                    string requestUri = ApiControlerUri + string.Format("/?{0}={1}",key, value);
                    var response = await instance.DeleteAsync(requestUri);
                    response.EnsureSuccessStatusCode(); // Throw on error code（有错误码时抛出）. 
                    return true;
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Delete item Error:" + ex.Message);
            }
            catch (System.FormatException)
            {
                throw new FormatException("Delete Item Format error.");
            }
            //finally
            //{
            //}
        }
        #endregion

        #region DeleteItems
        /// <summary>
        /// 根据Uri删除条目
        /// </summary>
        /// <param name="ApiControlerUri"></param>
        /// <returns></returns>
        public async Task<bool> DeleteItems(string ApiControlerUri)
        {
            try
            {
                if (IsInitialize)
                {
                    ApiControlerUri = ApiControlerUri + "/Delete";
                    var response = await instance.DeleteAsync(ApiControlerUri);
                    response.EnsureSuccessStatusCode(); // Throw on error code（有错误码时抛出）. 
                    return true;
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Delete item Error:" + ex.Message);
            }
            catch (System.FormatException)
            {
                throw new FormatException("Delete Item Format error.");
            }
            //finally
            //{
            //}
        }
        #endregion

        #region UploadFiles
        //未测试
        public IEnumerable<HDFile> UploadFiles(string ControlName , params string[] FullFileNames)
        {
            Uri server = new Uri("api/" + ControlName);
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();

            foreach (string fullfilename in FullFileNames)
            {
                string filename = Path.GetFileName(fullfilename);
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(fullfilename);
                //这里会向服务器上传一个png图片和一个txt文件
                StreamContent streamConent = new StreamContent(new FileStream(fullfilename, FileMode.Open, FileAccess.Read, FileShare.Read));

                multipartFormDataContent.Add(streamConent, filenameWithoutExtension, filename);
            }

            HttpResponseMessage responseMessage = instance.PostAsync(server, multipartFormDataContent).Result;

            if (responseMessage.IsSuccessStatusCode)
            {
                IList<HDFile> hdFiles = null;

                string content = responseMessage.Content.ReadAsStringAsync().Result;
                hdFiles = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<HDFile>>(content);

                if (hdFiles.Count > 0)
                    return hdFiles;
                else
                    return null;
            }
            return null;
        }
        #endregion

        #region DownLoadFile
        /// <summary>
        /// 下载库文件,ServerFileName Like:Devices\a.dll, SaveFileName Like:Devices\a.dll
        /// </summary>
        /// <param name="ServerFileName"></param>
        /// <param name="SaveFileName"></param>
        /// <returns></returns>
        public bool DownLoadFile(string ServerFileName, string SaveFileName,string ControlName, string ActionName)
        {
            try
            {
                string requestUri = WebApiHelper.GetRequestUri(
                    ControlName,
                    ActionName,
                    new Hashtable(){{ "file", "Download/" + ServerFileName }}
                    );
                //string requestUri = String.Format("api/{0}/GetFile?file={1}", Controller.Download.ToString(), ("Download/" + ServerFileName));

                //Uri server = new Uri(String.Format("{0}?file={1}", ControlUri.Download,("Download/" + ServerFileName)));

                string p = Path.GetDirectoryName(SaveFileName);

                if (p == "")
                {
                    throw new Exception("Server File Name must have a directory,like \'Download\\Devices\a.dll\'");
                }

                if (!Directory.Exists(p))
                    Directory.CreateDirectory(p);

                HttpResponseMessage responseMessage = instance.GetAsync(requestUri).Result;

                if (responseMessage.IsSuccessStatusCode)
                {
                    using (FileStream fs = File.Create(SaveFileName))
                    {
                        Stream streamFromService = responseMessage.Content.ReadAsStreamAsync().Result;
                        streamFromService.CopyTo(fs);
                        return true;
                    }
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region GetRequestUri
        public static string GetRequestUri(string controller,string action, Hashtable parameters)
        {
            string uri = string.Format("api/{0}/{1}", controller, action);
            if (parameters != null)
            {
                uri = uri + "?";
                foreach(DictionaryEntry p in parameters)
                {
                    if (p.Value.GetType().IsArray)
                    {
                        IEnumerable items = p.Value as IEnumerable;
                        foreach (object item in items)
                        {
                            uri = uri + p.Key + "=" + item.ToString() + "&";
                        }
                    }
                    else
                    {
                        uri = uri + p.Key + "=" + p.Value + "&";
                    }
                }
                uri = uri.Substring(0, uri.Length - 1);
            }
            return uri;
        }
        #endregion

        #region GetQueryString
        /// <summary>
        /// 拼接get参数
        /// </summary>
        /// <param name="parames"></param>
        /// <returns></returns>
        public static Tuple<string, string> GetQueryString(Dictionary<string, string> parames)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parames);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");  //签名字符串
            StringBuilder queryStr = new StringBuilder(""); //url参数
            if (parames == null || parames.Count == 0)
                return new Tuple<string, string>("", "");

            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = System.Web.HttpUtility.UrlEncode(dem.Current.Value);
                if (!string.IsNullOrEmpty(key))
                {
                    query.Append(key).Append(value);
                    queryStr.Append("&").Append(key).Append("=").Append(value);
                }
            }

            return new Tuple<string, string>(query.ToString(), queryStr.ToString().Substring(1, queryStr.Length - 1));
        }

        #endregion

        #region GetSignToken

        #endregion
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public static HttpResponseMsg GetSignToken(string appKey)
        {
            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("appKey", appKey.ToString());
            Tuple<string, string> parameters = GetQueryString(parames);
            var result = WebApiHelper.Get<Token>(tokenApiUrl, parameters.Item1, parameters.Item2, appKey, false);

            return result;
        }

        #region GetSignature
        /// <summary>
        /// 计算签名
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="nonce"></param>
        /// <param name="appKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GetSignature(string timestamp, string nonce, string appkey, string data)
        {
            Token token = null;
            var resultMsg = GetSignToken(appkey);
            if (resultMsg != null)
            {
                if (resultMsg.StatusCode == (int)StatusCode.Successed)
                {
                    token = resultMsg.Data as Token; //.Result;
                }
                else
                {
                    throw new Exception(resultMsg.Data.ToString());
                }
            }
            else
            {
                throw new Exception("token为null，员工编号为：" + AppKey);
            }

            var hash = System.Security.Cryptography.MD5.Create();
            //拼接签名数据
            var signStr = timestamp + nonce + AppKey + token.SignToken.ToString() + data;
            //将字符串中字符按升序排序
            var sortStr = string.Concat(signStr.OrderBy(c => c));
            var bytes = Encoding.UTF8.GetBytes(sortStr);
            //使用MD5加密
            var md5Val = hash.ComputeHash(bytes);
            //把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            foreach (var c in md5Val)
            {
                result.Append(c.ToString("X2"));
            }
            return result.ToString().ToUpper();
        }
        #endregion

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        private static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }


        /// <summary>  
        /// 获取随机数
        /// </summary>  
        /// <returns></returns>  
        private static string GetRandom()
        {
            Random rd = new Random(DateTime.Now.Millisecond);
            int i = rd.Next(0, int.MaxValue);
            return i.ToString();
        }

        #endregion

    }

    #region HDFile
    public class HDFile
    {
        public HDFile(string name, string url, string size)
        {
            Name = name;
            Url = url;
            Size = size;
        }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Size { get; set; }
    }
    #endregion

}
