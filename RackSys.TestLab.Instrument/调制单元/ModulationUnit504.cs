using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace RackSys.TestLab.Instrument
{
    public class ModulationUnit504 : ModulationUnit
    {
        public ModulationUnit504(string address)
            : base(address)
        { }
        protected override void DetermineIdentity()
        {
            double Toplevel;
            double Bottomlevel;
            double Pulsewidth;
            int ControlState;
            EquipmentState(out Toplevel, out Bottomlevel, out Pulsewidth, out ControlState);
            this.Connected = true;

        }
        protected override void DetermineOptions()
        {

        }
        public delegate string ValidateSupportDelegate(ModulationUnit mo);
        public static ModulationUnit.ValidateSupportDelegate m_ValidateSupportDelegate;
        private static string GetCmdBody(string cmd)
        {
            byte[] temp = Encoding.ASCII.GetBytes(cmd);
            int h = temp[0];
            for (int i = 1; i < temp.Length; i++)
            {
                h ^= temp[i];
            }
            string hh = Convert.ToString(h, 16);
            string Cmdstr = string.Format("${0}*{1}\r\n", cmd, hh.ToUpper());
            return Cmdstr;

        }
        private static string DetermineSupport(string address)
        {
            if (ModulationUnit.m_ValidateSupportDelegate == null)
            {
                return null;
            }
            ModulationUnit AModulationUnit = null;
            if (AModulationUnit == null)
            {
                return "无法识别对应的调制单元";
            }
            return ModulationUnit.m_ValidateSupportDelegate(AModulationUnit);
        }
        /// <summary>
        /// 去掉回车符
        /// </summary>
        /// <param name="inToRemoveStr"></param>
        /// <returns></returns>
        private string RemoveTrailingLineFeed(string inToRemoveStr)
        {
            char[] trimChars = new char[] { '\n', '\r' };
            return inToRemoveStr.Trim(trimChars);
        }
        /// <summary>
        /// 查询，并返回字符串
        /// </summary>
        /// <param name="inQueryCmd"></param>
        /// <returns></returns>
        public override string QueryWithoutLineFeed(string inQueryCmd)
        {
            this.m_CommMutex.WaitOne();
            try
            {
                StreamWriter file = new StreamWriter(new FileStream("LOG.txt", FileMode.Append));
                file.WriteLine("发送：" + this.RemoveTrailingLineFeed(inQueryCmd));
                this.SendWithoutLineFeed(inQueryCmd);
                byte[] x = new byte[1024];
                Thread.Sleep(500);
                this.Read(out x);
                string st = this.RemoveTrailingLineFeed(Encoding.ASCII.GetString(x));
                file.WriteLine("收到：" + st + "\r\n");
                file.Flush();
                file.Close();
                return st;
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }

        }
        /// <summary>
        /// 参数设置请求
        /// </summary>
        /// <param name="Toplevel">顶电平 0.000~3.300V</param>
        /// <param name="Bottomlevel">底电平 0.000~3.300V</param>
        /// <param name="Pulsewidth">脉宽 小于等于500us</param>
        public override string SetAndACK(double Toplevel, double Bottomlevel, double Pulsewidth)
        {
            string cmd = GetCmdBody(string.Format("SET,{0},{1},{2}", Toplevel, Bottomlevel, Pulsewidth));
            string strd = this.QueryWithoutLineFeed(cmd);
            //"$ACK,{0},{1} *hh \r\n
            string[] str1 = strd.Split(' ');
            if (str1[0] != null)
            {
                string[] str2 = str1[0].Split(',');
                if (str2.Length == 3)
                {
                    if (str2[2] == "0")
                    { return "成功"; }
                    else if (str2[2] == "1")
                    { return "参数错误"; }
                    else if (str2[2] == "2")
                    { return "校验错误"; }
                }
                else
                { return "读取错误"; }
            }
            return "读取错误";
        }


        /// <summary>
        /// 设备状态查询
        /// </summary>
        /// <param name="Toplevel">顶电平电压</param>
        /// <param name="Bottomlevel">底电平电压</param>
        /// <param name="Pulsewidth">脉宽</param>
        /// <param name="ControlState">控制状态 0表示本地控制，1表示远程控制</param>
        public override void EquipmentState(out double Toplevel, out double Bottomlevel, out double Pulsewidth, out int ControlState)
        {
            Toplevel = new double();
            Bottomlevel = new double();
            Pulsewidth = new double();
            ControlState = new int();
            try
            {
                string strd = this.QueryWithoutLineFeed(GetCmdBody("GET"));
                //$RPT,<1>,<2>,<3>,<4> *hh<CR><LF>
                string[] str1 = strd.Split(' ');
                if (str1[0] != null)
                {
                    string[] str2 = str1[0].Split(',');
                    if (str2.Length == 5)
                    {
                        Toplevel = double.Parse(str2[1]);
                        Bottomlevel = double.Parse(str2[2]);
                        Pulsewidth = double.Parse(str2[3]);
                        ControlState = int.Parse(str2[4].Split('*')[0]);
                    }
                    else
                    {
                        throw new Exception("返回值有误");
                    }
                }
                else
                {
                    throw new Exception("返回值有误");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }























    }
}
