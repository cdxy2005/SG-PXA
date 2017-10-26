//**********************************************
//*类名：KSL_XIAN504
//*作者: Gavin
//*创建时间: 2014/8/4 10:47:21
//*功能: 温度巡检仪
//***********************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class KSL_XIAN504 : TemperatureMonitorBase
    {
        public KSL_XIAN504(string inAddress)
        : base(inAddress)
        {
        }


        public override string Identity
        {
            get
            {
                return "KSL 温度巡检仪";
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
        /// 读取温度信息
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
    }
}
