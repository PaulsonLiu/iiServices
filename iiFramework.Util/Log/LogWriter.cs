using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiFramework.Util
{
    public class LogWriter
    {
        /// <summary>
        /// 保存信息到日志文件，保存格式例如：
        /// 2016-10-23 10:20:10    #日志信息
        /// </summary>
        /// <param name="filename">文件前缀</param>
        /// <param name="msg"></param>
        /// <param name="lodDirectory">文件夹</param>
        /// <param name="timeFormat">时间格式</param>
        /// <param name="extension">文件扩展名</param>
        public static void RecordLog(string filename , string msg , string lodDirectory = "log",string timeFormat = "yyyy-MM-dd",string extension = "log")
        {
            string filepath = lodDirectory + "\\" + filename + "_" + DateTime.Now.ToString(timeFormat) + ".log";
            if (!Directory.Exists(lodDirectory))
            {
                Directory.CreateDirectory(lodDirectory);
            }
            msg = DateTime.Now.ToString() + "      #" + msg + "\r\n";
            File.AppendAllText(filename,msg);
        }

        /// <summary>
        /// 记录日志信息，默认保存位置当前文件夹下的log文件夹，文件名：log_yyyy-MM-dd.log
        /// </summary>
        /// <param name="msg"></param>
        public static void RecordLog(string msg)
        {
            RecordLog("log", msg);
        }

    }
}
