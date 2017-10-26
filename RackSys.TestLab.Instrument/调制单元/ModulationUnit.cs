using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public abstract class ModulationUnit : ScpiInstrument
    {
        public ModulationUnit(string address)
            : base(address)
        { }
        public delegate string ValidateSupportDelegate(ModulationUnit mo);
        public static ModulationUnit.ValidateSupportDelegate m_ValidateSupportDelegate;
        public static ModulationUnit Connect(string currentAddress, ModulationUnit.ValidateSupportDelegate supportDelegate, bool interactive)
        {
            ModulationUnit AModulationUnit = null;
            string str = (currentAddress != null ? currentAddress : "GPIB0::19::INSTR");
            ModulationUnit.m_ValidateSupportDelegate = supportDelegate;
            if (interactive)
            {
                throw new Exception("不支持交互模式");
            }
            try
            {
                string str1 = ModulationUnit.DetermineSupport(str);
                if (str1 != null)
                {
                    throw new Exception(str1);
                }
                AModulationUnit = new ModulationUnit504(currentAddress);
            }
            catch
            {
                throw;
            }
            return AModulationUnit;

        }
        private static string GetCmdBody(string cmd)
        {
            byte[] temp = Encoding.ASCII.GetBytes(cmd);
            int h = temp[0];
            for (int i = 1; i < temp.Length; i++)
            {
                h ^= temp[i];
            }
            string hh = Convert.ToString(h, 16);
            string Cmdstr = string.Format("${0}*{1}", cmd, hh);
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
        /// 参数设置请求
        /// </summary>
        /// <param name="Toplevel">顶电平 0.000~3.300V</param>
        /// <param name="Bottomlevel">底电平 0.000~3.300V</param>
        /// <param name="Pulsewidth">脉宽 小于等于500us</param>
        public virtual string SetAndACK(double Toplevel, double Bottomlevel, double Pulsewidth)
        {
            string strd = this.Query(GetCmdBody(string.Format("SET,{0},{1},{2}", Toplevel, Bottomlevel, Pulsewidth)));
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
        public virtual void EquipmentState(out double Toplevel, out double Bottomlevel, out double Pulsewidth, out int ControlState)
        {
            Toplevel = new double();
            Bottomlevel = new double();
            Pulsewidth = new double();
            ControlState = new int();
            string strd = this.Query(GetCmdBody("GET"));
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
                    ControlState = int.Parse(str2[4]);
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























    }
}
