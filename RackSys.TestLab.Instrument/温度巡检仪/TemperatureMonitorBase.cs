//**********************************************
//*类名：TemperatureMonitorBase
//*作者: Gavin
//*创建时间: 2014/8/4 10:18:10
//*功能: 温度巡检仪基类
//***********************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="channelList">需要读的通道列表，值固定位应为01~16，对应16路温度数据</param>
    /// <param name="valueList">返回的16个通道温度值</param>
    /// 
    public struct TemperatureResult
    {
        public uint channel;
        public string value;
    }

    abstract public class TemperatureMonitorBase : ScpiInstrument
    {
        public TemperatureMonitorBase(string inAddress)
        : base(inAddress)
        {
        }

        protected override void DetermineIdentity()
        {

            switch (TemperatureMonitorConfig.CurTemperatureMonitorConfig.CurTemperatureMonitorType)
            {
                case TemperatureMonitorType.XSL_D_16RS2P0V0_XIAN504:
                    new XSL_D_16RS2P0V0(this.m_address);
                    break;
                case TemperatureMonitorType.KSL_XIAN504:
                    new KSL_XIAN504(this.m_address);
                    break;
            }
            throw new Exception("不被支持的巡检仪！");

        }

        protected override void DetermineOptions()
        {
            base.m_options = "";
        }

        public static TemperatureMonitorBase Connect(string inAddress)
        {

            switch (TemperatureMonitorConfig.CurTemperatureMonitorConfig.CurTemperatureMonitorType)
            {
                case TemperatureMonitorType.XSL_D_16RS2P0V0_XIAN504:
                    return new XSL_D_16RS2P0V0(inAddress);
                case TemperatureMonitorType.KSL_XIAN504:
                    return new KSL_XIAN504(inAddress);
            }
            throw new Exception("不被支持的巡检仪！");

        }

        abstract public void MonitorTemperatureValue(out TemperatureResult[] valueResult);

        public virtual void MonitorTemperatureValue(int InstAddress, int TunnelStart, int TunnelStop, out TemperatureResult[] valueResult)
        { valueResult = new TemperatureResult[0]; }
    }
}
