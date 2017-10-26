using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    internal class AgilentN5747A : DCPowerBase
    {
        public AgilentN5747A( string inAddress): base(inAddress)
        {
        }

        public override void Get_CurrentByChannelID(int inChannelId, out double outCurrentNow, out bool IsOK)
        {
            outCurrentNow = 0.0;
            try
            {
                outCurrentNow = base.QueryNumber("MEAS:CURR?");
                IsOK = true;
            }
            catch
            {
                IsOK = false;
            }
        }

        public override void Set_CurrentLimitByChannelID(int ChannelID, double inCurrentLimit, out bool IsOK)
        {
            try
            {
                base.SendNumber("CURR ", inCurrentLimit);
                base.Query("CURR:PROT:STAT ON; *OPC?", 1000);
                IsOK = true;
                
            }
            catch
            {
                IsOK = false;
            }
            Thread.Sleep(500);
        }

        public override void Set_OutputStateByChannelID(int ChannelID, bool inOnOffState, out bool IsOK)
        {
            IsOK = true;
            if (!inOnOffState)
            {
                try
                {
                    base.Send("OUTPut OFF");
                }
                catch
                {
                    IsOK = false;
                }
            }
            else
            {
                try
                {
                    base.Send("OUTPut ON");
                }
                catch
                {
                    IsOK = false;
                }
            }
        }

        public override bool Get_OutputStateByChannelID(int ChannelID)
        {
            bool OutputOnOffState = false;
            try
            {
                if (base.Query("OUTPut?") == "1")
                {
                    OutputOnOffState = true;
                }
                else
                {
                    OutputOnOffState = false;
                }
            }
            catch
            {
            }
            return OutputOnOffState;
         }

        public void PowerSupply()
        {
            base.Send("*RST");
            Thread.Sleep(1000);
        }


        public override void Get_OutputVoltageByChannelID(int ChannelID, out double OutputVoltage, out bool IsOK)
        {
            IsOK = true;
            try
            {
                OutputVoltage = base.QueryNumber("MEAS:VOLT?");
            }
            catch
            {
                IsOK = false;
                OutputVoltage = 0.0;
            }
        }

        public override void Set_OutputVoltageByChannelID(int ChannelID, double inOutputVoltage, out bool IsOK)
        {
            try
            {
                base.Send("VOLT:PROT:LEV 60");
                base.Send("VOLT:LIM:LOW 0");
                base.SendNumber("VOLT ", inOutputVoltage);
                IsOK = true;
            }
            catch
            {
                IsOK = false;
            }
            Thread.Sleep(500);
        }
    }
}
