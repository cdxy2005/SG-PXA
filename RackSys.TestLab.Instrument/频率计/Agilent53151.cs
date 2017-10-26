/* =============================================
 * Copyright @ 2013 北京中创锐科技术有限公司 
 * 名    称：Agilent53151
 * 功    能：Agilent53151
 * 作    者：Chen xf Administrator
 * 添加时间：2014-10-21 14:33:09
 * =============================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    public class Agilent53151 : FrequencyCounter
    {
        public Agilent53151(string inAddress)
            : base(inAddress)
        {
        }

        public override double MeasureFrequency(int Channel)
        {
            double num = 0;
            base.SendOpc(":STAT:PRES", 1000);
            if (Channel != 1)
            {
                base.SendOpc(":CONF:FREQ DEFAULT,DEFAULT,(@2)", 1000);
            }
            else
            {
                base.SendOpc(":CONF:FREQ DEFAULT,DEFAULT,(@1)", 1000);
                base.SendOpc(":INPut:FILTer:STATe ON", 1000);
            }
            base.SendOpc(":INIT:CONT ON", 1000);
            Thread.Sleep(1500);
            try
            {
                num = base.QueryNumber(":FETCH?");
            }
            catch
            {
            }
            return num;
        }

        public override void SetInputImpedance(int Channel, double impedance)
        {
        }

        public override void SetGateTime(double gateInSeconds)
        {
        }

        public override void Reset()
        {
            base.Send("*RST");
        }
    }
}