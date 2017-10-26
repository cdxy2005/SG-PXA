using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class MutiChannelDataAcquisition: ScpiInstrument
    {
        public MutiChannelDataAcquisition(string inAddress)
            : base(inAddress)
        {
        }

        public static MutiChannelDataAcquisition Connect(string inAddress)
        {
            return new MutiChannelDataAcquisition(inAddress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelList">需要读的通道列表，取值应为01~20，对应20路遥测通道</param>
        /// <param name="valueList">返回的电压值，长度等于ChannelList</param>
        /// 
        public struct VoltageResult
        {
           public uint  channel;
           public double  value;
        }

        /// <summary>
        /// 监控20路数据采集器的电压信息 
        /// </summary>
        /// <param name="channelList">监控通道列表</param>
        /// <param name="length">列表长度</param>
        /// <param name="valueResult">电压信息</param>
        public void MonitorVotageValue(uint[] channelList, int length, out VoltageResult [] valueResult)
        {
#if NO34972
            valueResult = new VoltageResult[length];
            
            return ;
#endif

            if (length != channelList.Length)
            {
                throw new Exception("多路数据采集的长度有错误！");
                
            }

            if (channelList.Length == 0)
            {
                valueResult = new VoltageResult[0];
                return;
            }
            
            valueResult = new VoltageResult[length];
           
            string Scanstring = "";//扫描的channel字符串
            string VoltageString = "";//直接从仪器读回的电压结果字符串
            string[] tempvoltageStr = new string[length];//中间存放的电压字符串数组
            
            //根据channellist，拼凑scan的字符串列表
            for (int i = 0; i < channelList.Length; i++)
            {
                if (channelList[i] < 10)
                {
                    Scanstring = Scanstring + "10" + channelList[i] + ",";
                }
                else 
                {
                    Scanstring = Scanstring + "1" + channelList[i] + ",";
                
                }

            }
            this.Send("CONF:VOLT:DC (@" + Scanstring + ")");

            this.WaitOpc();

            this.Send("ROUT:SCAN (@" + Scanstring + ")");
            this.WaitOpc();
            VoltageString = this.Query("READ?");
            this.WaitOpc();
            tempvoltageStr = VoltageString.Split(',');

            for (int index = 0; index < length; index++)
            {
                
                valueResult[index].channel = channelList[index];
                if (Math.Abs(Convert.ToDouble(tempvoltageStr[index])) < 0.01)//读回的值太小，认为是没有真实的激励，将值置为NaN
                {
                    valueResult[index].value = new double();
                }
                else
                {
                    valueResult[index].value = Convert.ToDouble(tempvoltageStr[index]);
                }


            }




        }

        public void MonitorTestVotageValue(uint channelList, out double valueResult)
        {

            this.Send("CONF:VOLT:DC (@" + channelList.ToString() + ")");

            this.WaitOpc();

            this.Send("ROUT:SCAN (@" + channelList.ToString() + ")");
            this.WaitOpc();
            valueResult =Convert.ToDouble( this.Query("READ?"));
           
        }
    }
}
