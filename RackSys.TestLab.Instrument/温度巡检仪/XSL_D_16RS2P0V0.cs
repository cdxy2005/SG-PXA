using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class XSL_D_16RS2P0V0 : TemperatureMonitorBase
    {
        public XSL_D_16RS2P0V0(string inAddress)
            : base(inAddress)
        {
        }

        public override string Identity
        {
            get
            {
                return "XSL/D_16RS2P0V0 温度巡检仪";
            }
        }

        protected override void DetermineIdentity()
        {

            TemperatureResult[] valueResult = new TemperatureResult[16];

            string tempstring = this.QueryWithoutLineFeed("#010116\r\n");

        }

        protected override void DetermineOptions()
        {
            base.m_options = "";
        }

        /// <summary>
        /// 获取1~16通道的温度
        /// </summary>
        /// <param name="valueResult"></param>
        public override void MonitorTemperatureValue(out TemperatureResult[] valueResult)
        {
            valueResult = new TemperatureResult[17];
            string[] ChannelvalueBeforeHandle = new string[17];
            int repeat = 0;
            do
            {
                if (10 < repeat++)
                {
                    throw new Exception("温度巡检仪失去连接！");
                }
                string tempstring = this.QueryWithoutLineFeed("#010116\r\n");
                ChannelvalueBeforeHandle = tempstring.Split('=');
            }
            while (ChannelvalueBeforeHandle.Length != 17);

            for (uint i = 0; i < 16; i++)
            {
                valueResult[i].channel = i + 1;

                valueResult[i].value = (ChannelvalueBeforeHandle[i + 1].Substring(0, 6));

            }


        }

        /// <summary>
        /// 获取指定通道范围的温度
        /// </summary>
        /// <param name="InstAddress">仪表地址，00~99</param>
        /// <param name="TunnelStart">起始通道号，01~80</param>
        /// <param name="TunnelStop">终止通道号，01~80。如果测量单通道，则和起始通道保持一致。</param>
        /// <param name="valueResult">通道温度结果列表</param>
        public override void MonitorTemperatureValue(int InstAddress, int TunnelStart, int TunnelStop, out TemperatureResult[] valueResult)
        {
            if (InstAddress < 0 || InstAddress > 99)
            { throw new Exception("地址超限"); }
            if (TunnelStart < 1 || TunnelStart > 80)
            { throw new Exception("起始通道号超限"); }
            if (TunnelStop < 1 || TunnelStop > 80)
            { throw new Exception("终止通道号超限"); }

            string AddressCode = InstAddress.ToString("##");

            string TunnelCode = "";
            int TunnelCount = 0;
            if (TunnelStart == TunnelStop)
            {
                TunnelCode = TunnelStart.ToString("##");
            }
            else if (TunnelStart > TunnelStop)
            {
                TunnelCode = TunnelStop.ToString("##") + TunnelStart.ToString("##");
            }
            else
            {
                TunnelCode = TunnelStart.ToString("##") + TunnelStop.ToString("##");
            }
            TunnelCount = Math.Abs(TunnelStart - TunnelStop) + 1;

            string Cmd = "#" + AddressCode + TunnelCode + "\r\n";
            string tempResult = "";
            string[] tempResultList = new string[0];
            int repeat = 0;
            do
            {
                if (repeat > 10)
                { throw new Exception("温度巡检仪连接异常"); }
                tempResult = this.QueryWithoutLineFeed(Cmd);
                tempResultList = tempResult.Split('=');
                repeat++;
            }
            while (tempResultList.Length != TunnelCount + 1);

            valueResult = new TemperatureResult[tempResultList.Length];
            for (int i = 0; i < tempResultList.Length; i++)
            {
                valueResult[i].channel = (uint)(TunnelStart + i);
                valueResult[i].value = tempResultList[i + 1].Substring(0, 6);
            }
        }
    }
}
