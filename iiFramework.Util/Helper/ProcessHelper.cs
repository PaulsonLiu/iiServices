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
        public static void StartProcess(string exeFilePath, string arguments, EventHandler callback)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = exeFilePath;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = false;
                //p.StartInfo.Verb = "runas";

                p.EnableRaisingEvents = true;
                p.Exited += new EventHandler(callback);

                p.Start();

                //p.BeginOutputReadLine();
                //p.BeginErrorReadLine();

                //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                //string str = string.Format(@"""{0}"" {1} {2}", ExeFilePath, cmdStr, "&exit");

                //p.StandardInput.WriteLine(str);
                //p.StandardInput.AutoFlush = true;
                //p.StandardOutput.ReadToEnd();//获取返回值 
                //p.WaitForExit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CheckProcessExists(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process p in processes)
            {
                if (p.ProcessName.ToString() == processName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 直接终止线程，不释放占用资源。若遇到access denied异常，可使用cmdHelper调用wmic终止
        /// </summary>
        /// <param name="processName"></param>
        public static void KillProcess(string processName)
        {
            //得到所有打开的进程   
            try
            {
                Process[] ps = Process.GetProcessesByName(processName);
                foreach (Process p in ps)
                {
                    p.Kill();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 关闭线程，非直接终止，调用线程Dispose，释放占用资源
        /// </summary>
        /// <param name="processName"></param>
        public static void DisposeProcess(string processName)
        {
            try
            {
                System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName(processName);
                foreach (System.Diagnostics.Process p in ps)
                {
                    p.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
