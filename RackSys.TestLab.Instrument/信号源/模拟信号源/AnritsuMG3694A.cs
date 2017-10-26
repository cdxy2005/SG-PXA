using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    public class AnritsuMG3694A : SignalGenerator
    {
        public AnritsuMG3694A(string address)
            : base(address)
        {
            //string str = ScpiInstrument.DetermineModel(address);
            //base.GetErrorQueue();
        }
        protected override void DetermineOptions()
        {
            this.m_options = this.Query("*OPT?");
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

}
