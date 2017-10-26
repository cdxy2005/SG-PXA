/* =============================================
 * Copyright @ 2013 北京中创锐科技术有限公司 
 * 名    称：Agilent53130
 * 功    能：Agilent53130
 * 作    者：Chen xf Administrator
 * 添加时间：2014-10-21 14:34:07
 * =============================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    /// <summary>
    /// 参照51151的代码编写（2013-12-10 白新跃）
    /// </summary>
    public class Agilent53130 : FrequencyCounter
    {
        public Agilent53130(string inAddress)
            : base(inAddress)
        {
        }

        public override double MeasureFrequency(int Channel)
        {
            double num = 0;

            try
            {
                // 这种测量方法好像会导致门时间无法设置
                int tmpTimeOut = base.Timeout;
                base.Timeout = 15000;
                //num = base.QueryNumber(string.Format(":MEASURE:FREQ? 1,1E-11,(@{0})", Channel == 1 ? 1 : 2) );
                num = base.QueryNumber("READ:FREQ? ");
                base.Timeout = tmpTimeOut;
                Thread.Sleep(100);
                base.Send(":INIT:CONT ON");
            }
            catch (Exception ex)
            {
                this.IO.Clear();
            }

            return num;
        }

        public override void SetInputImpedance(int Channel, double impedance)
        {
            if (Channel == 1)
            {
                if (impedance == 50)
                    base.Send(":INPut1:IMPedance 50");
                else
                    base.Send(":INPut1:IMPedance 1E6");
            }
            else
            {
                if (impedance == 50)
                    base.Send(":INPut2:IMPedance 50");
                else
                    base.Send(":INPut2:IMPedance 1E6");
            }
        }

        public override void SetGateTime(double gateInSeconds)
        {
            if (gateInSeconds < 1E-3 || gateInSeconds > 1000.0)
            {
                throw new Exception("设置的门时间超出正常范围。");
            }

            base.Send(":FREQ:ARM:STAR:SOUR IMM");
            base.Send(":FREQ:ARM:STOP:SOUR TIM");
            //base.Send(string.Format("FREQ:ARM:STOP:TIM {0}", gateInSeconds));
            base.Send(string.Format(":FREQ:ARM:STOP:TIM {0}", gateInSeconds));

        }


        public class FrequencyCounterSetting
        {
            private uint m_channelNumber = 1;
            /// <summary>
            /// channelNumber取值固定为1，2,3
            /// </summary>
            public uint ChannelNumber
            {
                get { return m_channelNumber; }
                set { m_channelNumber = value; }
            }

            
            private double m_Impedance = 50;
            /// <summary>
            /// 取值为50或1e6
            /// </summary>
            public double Impedance
            {
                get { return m_Impedance; }
                set { m_Impedance = value; }
            }


            private double m_GateTime = 1e-2;
            /// <summary>
            /// 取值固定位1e-2，1e-1，1，10
            //
            /// 
            /// </summary>
            public double GateTime
            {
                get { return m_GateTime; }
                set { m_GateTime = value; }
            }

        }


        public bool FrequencyCounterConfigre(FrequencyCounterSetting counterSetting)
        {
            this.SetInputImpedance((int)counterSetting.ChannelNumber, counterSetting.Impedance);
            this.WaitOpc();
            this.SetGateTime(counterSetting.GateTime);

            return true;
        }
    }
}