using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    internal class AgilentEmp : PowerMeter
    {
        public AgilentEmp(string inAddress) : base(inAddress)
        {
            base.Send("SYST:PRES DEF");
            Thread.Sleep(1000);
        }



        public override void CalPowerMeter(int ChannelNumber)
        {
            // throw new NotImplementedException();原有驱动存在问题，注释掉。
            base.Send("CAL" + ChannelNumber.ToString() + ":AUTO ONCE");

        }
        public override void ZeroPowerMeter(int ChannelNumber)
        {
            //throw new NotImplementedException();原有驱动存在问题，注释掉。
            base.Send("CAL" + ChannelNumber.ToString() + ":ZERO:AUTO ONCE");
        }

        public override void ZeroAndCalPowerMeter(int ChannelNumber)
        {
            base.Send("CAL" + ChannelNumber.ToString() + ":ALL");
        }

        public override double MeasurePower(double m_Frequency, int ChannelNumber)
        {
            double num = 0;
            if (base.Query("INIT:CONT?").IndexOf("1") < 0)
            {
                base.Send("INIT:CONT ON");
            }
            base.Send(string.Concat("SENS:FREQ ", m_Frequency.ToString()));
            try
            {
                num = base.QueryNumber("FETCH" + ChannelNumber.ToString() + ":POW?");
            }
            catch
            {
            }
            return num;
        }

        public override double GetCWAvgValue(int ChannelNumber)
        {
            double num;
            return num = base.QueryNumber("FETCH" + ChannelNumber.ToString() + ":POW?");
        }

        public override void SetOffset(int GateCode, double OffsetValue)
        {
            base.Send(string.Format("CALC{0}:GAIN {1}", GateCode, OffsetValue));
        }

        public override void SetFrequence(int GateCode, double FreqValue)
        {
            base.Send(string.Concat("SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()));
        }

        public override double MeasurePower(int ChannelNumber)
        {
            double num = 0;
            if (base.Query("INIT:CONT?").IndexOf("1") < 0)
            {
                base.Send("INIT:CONT ON");
            }
            try
            {
                num = base.QueryNumber("FETCH" + ChannelNumber.ToString() + ":POW?");
            }
            catch
            {
            }
            return num;
        }

        /// <summary>
        /// 设置功率计单位
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <param name="units"></param>
        public override void SetChannelUnits(int ChannelNumber, Units units)
        {
            base.Send(string.Format("UNIT{0}:POW {1}", ChannelNumber, units));
        }
    }
}
