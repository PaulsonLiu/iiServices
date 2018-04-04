using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iiFramework.Util
{
    public static class ProcessHelper
    {
        public static void KillProcess(string processName)
        {
            try
            {
                Process[] pro = Process.GetProcesses();//获取已开启的所有进程
                //遍历所有查找到的进程
                for (int i = 0; i < pro.Length; i++)
                {

                    //判断此进程是否是要查找的进程
                    if (pro[i].ProcessName.ToString() == processName)
                    {
                        pro[i].Kill();//结束进程
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static void StopProcess(string processName)
        {
            try
            {
                System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName(processName);
                foreach (System.Diagnostics.Process p in ps)
                {
                    p.Kill();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
