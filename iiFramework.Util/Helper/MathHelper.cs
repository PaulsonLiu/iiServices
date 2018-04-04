using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiFramework.Util
{
    public static class MathHelper
    {
        /// <summary>  
        /// 获取随机数
        /// </summary>  
        /// <returns></returns>  
        public static string GetRandom()
        {
            Random rd = new Random(DateTime.Now.Millisecond);
            int i = rd.Next(0, int.MaxValue);
            return i.ToString();
        }
    }
}
