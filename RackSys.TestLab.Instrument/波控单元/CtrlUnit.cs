using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{



    public abstract class CtrlUnit : ScpiInstrument
    {
        public CtrlUnit(string address)
            : base(address)
        { }

        /// <summary>
        /// delegate声明
        /// </summary>
        /// <param name="inDCPower"></param>
        /// <returns></returns>
        public delegate string ValidateSupportDelegate(CtrlUnit inCtrlUnit);

        /// <summary>
        /// delegate保存位置
        /// </summary>
        public static CtrlUnit Connect(string currentAddress, CtrlUnit.ValidateSupportDelegate supportDelegate, bool interactive)
        {
            CtrlUnit tempUnit = null;
            try
            {
                //CtrlUnit tempUnit = null;
                string str = (currentAddress != null ? currentAddress : "GPIB0::18::INSTR");
                if (CtrlUnit.DetermineSupport(str))
                {
                    tempUnit = CtrlUnit.CreateDetectedDCPowerSupply(str);
                }
                tempUnit.Connected = true;
                return tempUnit;
            }
            catch (Exception exception)
            {
                return null;
                //throw new Exception(exception.ToString());
            }
            //if (tempUnit != null)
            //{
            //tempUnit.Connected = true;
            //}
            
        }
        //public delegate string ValidateSupportDelegate(CtrlUnit mo);
        //public static CtrlUnit.ValidateSupportDelegate m_ValidateSupportDelegate;
        //public static CtrlUnit Connect(string currentAddress, CtrlUnit.ValidateSupportDelegate supportDelegate, bool interactive)
        //{
        //    CtrlUnit AModulationUnit = null;
        //    string str = (currentAddress != null ? currentAddress : "GPIB0::19::INSTR");
        //    CtrlUnit.m_ValidateSupportDelegate = supportDelegate;
        //    if (interactive)
        //    {
        //        throw new Exception("不支持交互模式");
        //    }
        //    try
        //    {
        //        string str1 = CtrlUnit.DetermineSupport(str);
        //        if (str1 != null)
        //        {
        //            throw new Exception(str1);
        //        }
        //       AModulationUnit = new CtrlUnitTR29(currentAddress);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return AModulationUnit;

        //}
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


        /// <summary>
        /// 判断是否可以支持对应型号的频率测量模块
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static bool DetermineSupport(string address)
        {
            CtrlUnit tempUnit = null;
            try
            {
                tempUnit = CtrlUnit.CreateDetectedDCPowerSupply(address);
            }
            catch
            {
                throw;
            }

            return tempUnit != null;
        }

        /// <summary>
        /// 创建对应的频率测量对象。
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static CtrlUnit CreateDetectedDCPowerSupply(string address)
        {
            CtrlUnit tempUnit = null;
            try
            {
                //Racksystem,RAC1110,RACXXXXXXXXXX,1.0
                string ModelNo = ScpiInstrument.DetermineModel(address);
                if (ModelNo.IndexOf("RAC1110") >= 0 )
                {
                    tempUnit = new CtrlUnitTR29(address);
                }
                else if (ModelNo.IndexOf("RAC-4001B") >= 0)
                {
                      tempUnit = new CtrlUnitTR29(address);
                }
                
                else
                {
                    throw new Exception(string.Concat(ModelNo, " 不是一个可以支持的波控"));
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("连接波控模块错误: ", exception.Message));
            }
            return tempUnit;
        }
        /// <summary>
        /// 参数设置请求
        /// </summary>
        /// <param name="Toplevel">顶电平 0.000~3.300V</param>
        /// <param name="Bottomlevel">底电平 0.000~3.300V</param>
        /// <param name="Pulsewidth">脉宽 小于等于500us</param>
        public virtual string SetAndACK(double Toplevel, double Bottomlevel, double Pulsewidth)
        {
          
            return "";
        }

        /// <summary>
        /// 设备状态查询
        /// </summary>
        /// <param name="Toplevel">顶电平电压</param>
        /// <param name="Bottomlevel">底电平电压</param>
        /// <param name="Pulsewidth">脉宽</param>
        /// <param name="ControlState">控制状态 0表示本地控制，1表示远程控制</param>
        public virtual void EquipmentState()
        {
      
        }

        /// <summary>
        /// 系统命令 设置电平类型
        /// </summary>
        /// <param name="LevelType">TTL；LvTTL；LVDS</param>
        /// <returns></returns>

        public virtual string SetLevelType(string LevelType)
        {
            return "ERROR";
        }

        public virtual void TR_reset()
        { }


        /// <summary>
        /// 串口命令 设置时钟个数
        /// </summary>
        /// <param name="CLKNo">时钟个数范围：1~40 </param>
        /// <returns></returns>
        public virtual string SetCLKNo(int CLKNo)
        {
            return "ERROR";
        }

        /// <summary>
        /// 串口命令 设置时钟频率
        /// </summary>
        /// <param name="freq">频率范围 1~30 000 000 </param>
        /// <returns></returns>
        public virtual string SetFreq(double Freq)
        {
            return "ERROR";
        }

        /// <summary>
        /// 串口设置 设置上升沿有效或者下降沿有效
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>

        public virtual string SetRisingEdgeEnable(bool enable)
        {
            return "ERROR";

        }
        /// <summary>
        /// 设置串口DATA
        /// </summary>
        /// <param name="LineNo">2~10</param>
        /// <param name="Data">10位16进制数</param>
        /// <returns></returns>
        public virtual string SetSeriesData(int LineNo, string Data)
        {
            return "ERROR";
        }
        /// <summary>
        /// 设置串口通道开启和关闭
        /// </summary>
        /// <param name="Port">3位16进制数</param>
        /// <returns></returns>
        public virtual string SetSeriesPortEnable(string Port)
        {
            return "ERROR";
        }
        /// <summary>
        /// 串口输出触发
        /// </summary>
        /// <returns></returns>
        public virtual string TriggerSeriesOutput()
        {
            return "ERROR";
        }

        /// <summary>
        /// 串口输出控制
        /// </summary>
        /// <param name="LineNo"></param>
        /// <param name="level"></param>
        /// <returns></returns>

        public virtual string ParallelOutput(int LineNo, bool level)
        {
            return "ERROR";
        }
        /// <summary>
        /// 选择内部斩波或者外部斩波
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        public virtual string IsInternChopping(bool enable)
        {
            return "ERROR";
        }
        /// <summary>
        /// 设置外部斩波是否反向
        /// </summary>
        /// <param name="str"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public virtual string SetExternalChoppingOpposition(string str, bool enable)
        {
            return "ERROR";
        }
        /// <summary>
        /// 设置内部斩波置高或者置低
        /// </summary>
        /// <param name="str"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public virtual string SetInternalChoppingHigh(string str, bool enable)
        {
            return "ERROR";
        }


        public virtual string SetSynchronousProt(int port1, int port2)
        {
            return "ERROR";
        }
        public virtual string GetTemp()
        {
            return "ERROR";
        }   


    }
}
