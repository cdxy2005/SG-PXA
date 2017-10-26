using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class AgilentE8257D: SignalGenerator
    {
        public AgilentE8257D(string address)
            : base(address)
        {
            string str = ScpiInstrument.DetermineModel(address);
            base.GetErrorQueue();
        }



        /// <summary>
        /// 将模拟信号源的各设置参数实现到仪表
        /// </summary>
        /// <param name="SGsetting"></param>
        /// <returns></returns>
        public override bool SignalGeratorConfigure(SignalGeneratorSetting SGsetting)
        {
            this.RFFrequency = SGsetting.Freq;
            this.WaitOpc();
            this.AmplOffset = SGsetting.Offset;
            this.WaitOpc();
            this.RFPower = SGsetting.RFPower;
            this.WaitOpc();
            this.RFOutputEnabled = SGsetting.RFOn;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SGsetting"></param>
        /// <returns></returns>
        public override bool SignalGeratorConfigurationRead(out SignalGeneratorSetting SGsetting)
        {
            SGsetting = new SignalGeneratorSetting();
            SGsetting.Freq = this.RFFrequency;
            this.WaitOpc();
            SGsetting.Offset = this.AmplOffset;
            this.WaitOpc();
            SGsetting.RFPower = this.RFPower;
            this.WaitOpc();
            SGsetting.RFOn = this.RFOutputEnabled;

            return true;
        }


        //杨飞添加驱动 2016.01.12 信号源脉冲周期和脉冲宽度
        /// <summary>
        /// 脉冲周期
        /// </summary>
        public override double PulsePeriod
        {
            get
            {
                double PPeriod = Convert.ToDouble(base.Query(":PULM:INTernal:PERiod?"));
                return PPeriod;
            }
            set
            {
                base.Send(":PULM:INTernal:PERiod " + value.ToString());
            }
        }

        /// <summary>
        /// 脉冲宽度
        /// </summary>
        public override double PulseWidth
        {
            get
            {
                double PWidth = Convert.ToDouble(base.Query(":PULM:INTernal:WIDTh?"));
                return PWidth;
            }
            set
            {
                base.Send(":PULM:INTernal:WIDTh " + value.ToString() + "s");
            }
        }

    }


    /// <summary>
    /// 模拟信号源设置参数
    /// </summary>
    public class SignalGeneratorSetting
    {
        private double m_RFPower = 0;

        public double RFPower
        {
            get { return m_RFPower; }
            set { m_RFPower = value; }
        }

        private double m_Freq = 1e9;

        public double Freq
        {
            get { return m_Freq; }
            set { m_Freq = value; }
        }

        private double m_Offset = 0;

        public double Offset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }

        private bool m_RFOn = false;

        public bool RFOn
        {
            get { return m_RFOn; }
            set { m_RFOn = value; }
        }

    }
}
