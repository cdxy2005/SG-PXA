using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    internal class AgilentN6705B : DCPowerBase
    {
        public AgilentN6705B(string inAddress)
            : base(inAddress)
        {
        }

        public override void Get_CurrentByChannelID(int inChannelId, out double outCurrentNow, out bool IsOK)
        {
            this.AssetChannelID(inChannelId);

            try
            {
                outCurrentNow = this.QueryNumber(string.Format("MEAS:CURR? (@{0})", new object[] { inChannelId }));
                IsOK = true;
            }
            catch
            {
                IsOK = false;
                outCurrentNow = 0;
            }
        }

        public override void Set_CurrentLimitByChannelID(int ChannelID, double inCurrentLimit, out bool IsOK)
        {
            AssetChannelID(ChannelID);
            try
            {
                //base.Send(string.Format("CURRent:LIMit:COUPle ON,(@{0})", new object[]{ ChannelID }));
                this.Send(string.Format("CURRent {0},(@{1})", new object[] { inCurrentLimit, ChannelID}));
                
                IsOK = true;
            }
            catch
            {
                IsOK = false;
            }
            Thread.Sleep(500);
        }

        private void AssetChannelID(int ChannelID)
        {
            if ((ChannelID <= 0) || (ChannelID > 4))
            {
                throw new Exception("电源通道编号超界。");
            }
        }

        public override void Set_OutputStateByChannelID(int ChannelID, bool inOnOffState, out bool IsOK)
        {
            string OutputStateStr = inOnOffState ? "ON" : "OFF";
            try
            {
                this.Send(string.Format("OUTPut {0},(@{1})", new object[]{OutputStateStr, ChannelID}));
                this.Query("*OPC?", 2000);
                IsOK = true;
            }
            catch
            {
                IsOK = false;
            }
        }

        public override bool Get_OutputStateByChannelID(int ChannelID)
        {
            bool OutputOnOffState = false;
            try
            {
                if (this.QueryNumber(string.Format("OUTPut? (@{0})", new object[] { ChannelID })) == 1.0)
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
                OutputOnOffState = false;
            }

            return OutputOnOffState;
        }

        public override void Get_OutputVoltageByChannelID(int ChannelID, out double OutputVoltage, out bool IsOK)
        {
            try
            {
                OutputVoltage = this.QueryNumber(string.Format("MEAS:VOLT? (@{0})", ChannelID));
                IsOK = true;
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
                this.Send(string.Format("VOLT {0},(@{1})", new object[]{inOutputVoltage, ChannelID}));
                IsOK = true;
            }
            catch
            {
                IsOK = false;
            }
            Thread.Sleep(500);
        }
        public override void ClearOCPInfo()
        {
            try
            {
                int tmpTimeOut = base.Timeout;
                base.Timeout = 50000;
                base.Send("CURR:PROT:CLE");
                base.Timeout = tmpTimeOut;
                //isOCP = state == 1;

                //IsOK = true;
            }
            catch
            {
                //IsOK = false;
            }
        }
        public override void Get_IsOCPTripped(int ChannelID, out bool isOCP, out bool IsOK)
        {
            isOCP = false;

            //AssertChannelID(ChannelID);
            try
            {
                int tmpTimeOut = base.Timeout;
                base.Timeout = 2000;
                double state = 0.0;
                try
                {
                    state = base.QueryNumber("CURRent:PROTection:TRIPped?");
                }
                finally
                {
                    base.Timeout = tmpTimeOut;
                }
                isOCP = state == 1;

                IsOK = true;
            }
            catch
            {
                IsOK = false;
            }
        }
    
    }

}
