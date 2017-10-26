using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class AV1464BSignalGenerator : SignalGenerator
    {
        public AV1464BSignalGenerator(string address)
            : base(address)
        {
            string str = ScpiInstrument.DetermineModel(address);
            base.GetErrorQueue();
        }

        protected override void DetermineOptions()
        {
            this.m_options = "";
        }


        /// <summary>
        /// 调制开关
        /// </summary>
        public override bool ModOutputEnabled
        {
            get
            {
                return false;
            }
            set
            {
            }
        }



        public override void Preset()
        {
        }
        public override double RFFrequencyMax 
        {
            get 
            {
                return 40e9;
            }
        }

        //杨飞添加驱动 2016.01.12 信号源脉冲周期和脉冲宽度
        /// <summary>
        /// 脉冲周期（50ns~21.000000000s）
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
        /// 脉冲宽度(20ns~21.000000000s)
        /// </summary>
        public override double PulseWidth
        {
            get
            {
                double PWidth = Convert.ToDouble(base.Query(":PULM:INTernal:PWIDth?"));
                return PWidth;
            }
            set
            {
                base.Send(":PULM:INTernal:PWIDth " + value.ToString() + "s");
            }
        }


    }
}
