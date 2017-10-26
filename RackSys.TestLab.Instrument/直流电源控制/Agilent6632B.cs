using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    internal class Agilent6632B : DCPowerBase
    {
        public Agilent6632B(string inAddress): base(inAddress)
        {
        }

        public override void Get_CurrentByChannelID(int inChannelId, out double outCurrentNow, out bool IsOK)
        {
            try
            {
                outCurrentNow = base.QueryNumber("MEAS:CURR?");
                IsOK = true;
            }
            catch
            {
                IsOK = false;
                outCurrentNow = 0.0;
            }
            
        }

        /// <summary>
        /// 是否正确有待验证
        /// </summary>
        /// <param name="ChannelID"></param>
        /// <param name="inCurrentLimit"></param>
        /// <param name="IsOK"></param>
        public override void Set_CurrentLimitByChannelID(int ChannelID, double inCurrentLimit, out bool IsOK)
        {
            try
            {
                base.SendNumber("CURR ", inCurrentLimit);
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
            if (base.Query("OUTPut?") != "1")
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public override void Get_OutputVoltageByChannelID(int ChannelID, out double OutputVoltage, out bool IsOK)
        {
            OutputVoltage = 0.0;
            IsOK = true;
            try
            {

                OutputVoltage = base.QueryNumber("MEAS:VOLT?");
            }
            catch
            {
                IsOK = false;
            }
        }

        public override void Set_OutputVoltageByChannelID(int ChannelID, double inOutputVoltage, out bool IsOK)
        {
            try
            {
                base.Send(string.Concat("VOLT ", inOutputVoltage.ToString()));
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
