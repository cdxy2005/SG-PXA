using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public delegate void EvnetOverDCCurrent(object sender,bool isOver);
    public abstract class DCPowerBase : ScpiInstrument
    {
        public static event EvnetOverDCCurrent MessageEventOverDCCurrent;

        public static void RepportOver(bool isOver)
        {
            if (MessageEventOverDCCurrent != null)
            {
                MessageEventOverDCCurrent(null,isOver);
 
            }
 
        }
        public DCPowerBase(string in_InstAddr): base(in_InstAddr)
        {

        }

        public abstract void Get_CurrentByChannelID(int inChannelId, out double outCurrentNow, out bool IsOK);

        public abstract void Set_CurrentLimitByChannelID(int ChannelID, double inCurrentLimit, out bool IsOK);

        public abstract void Set_OutputStateByChannelID(int ChannelID, bool inOnOffState, out bool IsOK);
        /// <summary>
        /// 获取最大输出电流
        /// </summary>
        /// <param name="ChannelID"></param>
        /// <returns></returns>
        public virtual double Get_MaxCurrent(int ChannelID) { return 0; }
        /// <summary>
        /// 获取最大输出电压
        /// </summary>
        /// <param name="ChannelID"></param>
        /// <returns></returns>
        public virtual double Get_MaxVoltage(int ChannelID){return 0;}
        public abstract bool Get_OutputStateByChannelID(int ChannelID);

        public abstract void Get_OutputVoltageByChannelID(int ChannelID, out double OutputVoltage, out bool IsOK);

        public abstract void Set_OutputVoltageByChannelID(int ChannelID, double inOutputVoltage, out bool IsOK);
        public abstract void ClearOCPInfo();
        public virtual void ClearOCPInfochan(int chan) { }
        public abstract void Get_IsOCPTripped(int ChannelID, out bool isOCP, out bool IsOK);
    

        /// <summary>
        /// delegate声明
        /// </summary>
        /// <param name="inDCPower"></param>
        /// <returns></returns>
        public delegate string ValidateSupportDelegate(DCPowerBase inDCPower);

        /// <summary>
        /// delegate保存位置
        /// </summary>
        private static DCPowerBase.ValidateSupportDelegate m_validateSupportDelegate;

        /// <summary>
        /// 判断是否可以支持对应型号的直流电源
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static string DetermineSupport(string address)
        {
            if (DCPowerBase.m_validateSupportDelegate == null)
            {
                return null;
            }
            DCPowerBase ADCPower = null;
            try
            {
                ADCPower = DCPowerBase.CreateDetectedDCPowerSupply(address);
            }
            catch
            {
                throw;
            }
            if (ADCPower == null)
            {
                return "不是一个可以识别的直流电源";
            }
            return DCPowerBase.m_validateSupportDelegate(ADCPower);
        }

        /// <summary>
        /// 连接某直流电源，并返回对应的控制对象；
        /// </summary>
        /// <param name="currentAddress"></param>
        /// <param name="supportDelegate"></param>
        /// <param name="interactive"></param>
        /// <returns></returns>
        public static DCPowerBase Connect(string currentAddress, DCPowerBase.ValidateSupportDelegate supportDelegate, bool interactive)
        {
            DCPowerBase ADCPower = null;
            string str = (currentAddress != null ? currentAddress : "GPIB0::18::INSTR");
            DCPowerBase.m_validateSupportDelegate = supportDelegate;
            if (interactive)
            {
                throw new Exception("不支持交互模式");
            }
            try
            {
                if (DCPowerBase.DetermineSupport(str) == null)
                {
                    ADCPower = DCPowerBase.CreateDetectedDCPowerSupply(str);
                }
            }
            catch
            {
                throw;
            }
            DCPowerBase.m_validateSupportDelegate = null;
            if (ADCPower != null)
            {
                ADCPower.Connected = true;
            }
            return ADCPower;
        }

        /// <summary>
        /// 判断对应的型号是否可以支持
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static bool IsSupportedModel(string model)
        {
            if (model.IndexOf("N6705") >= 0)
            {
                return true;
            }
            else if (model.IndexOf("E3634A") >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 创建对应的直流电源控制对象。
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static DCPowerBase CreateDetectedDCPowerSupply(string address)
        {
            DCPowerBase DCPowerSupply = null;
            try
            {
                string ModelNo = ScpiInstrument.DetermineModel(address);
                if (ModelNo.IndexOf("N6705B") >= 0)
                {
                    DCPowerSupply = new AgilentN6705B(address);
                }
                else if (ModelNo.IndexOf("E3634A") >= 0)
                {
                    DCPowerSupply = new AgilentE3634A(address);
                }
                else if (ModelNo.IndexOf("6675A") >= 0)
                {
                    DCPowerSupply = new Agilent6675A(address);
                }
                else if (ModelNo.IndexOf("3649A") >= 0)
                {
                    DCPowerSupply = new AgilentE3649A(address);
                }
                if (ModelNo.IndexOf("N6702A") >= 0)
                {
                    DCPowerSupply = new AgilentN6700B(address);
                }
                if (ModelNo.IndexOf("N6701A") >= 0)
                {
                    DCPowerSupply = new AgilentN6700B(address);
                }
                else
                {
                    throw new Exception(string.Concat(ModelNo, " 不是一个可以支持的直流电源"));
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("连接直流电源错误: ", exception.Message));
            }
            return DCPowerSupply;
        }

        public virtual void Reset()
        {
            base.Send("*RST");
            this.Query("*IDN?", 2000);
        }
        public virtual void Get_OutputChannelByChannelID(int ChannelID, out bool IsConnectModel, out string PowerModel) { IsConnectModel = true; PowerModel = ""; }
     }
}
