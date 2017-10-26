using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    internal class AgilentE3649A : DCPowerBase
    {
        public AgilentE3649A(string inAddress)
            : base(inAddress)
        {
        }
        /// <summary>
        /// 决定仪器的选件
        /// </summary>
        protected override void DetermineOptions()
        {
            base.m_options = "";
        }

        public override void Set_OutputStateByChannelID(int ChannelID, bool inOnOffState, out bool IsOK)
        {
            double voltageNouse = 0;
            string OutputStateStr = inOnOffState ? "ON" : "OFF";
            try
            {
                this.Send("OUTPut " + OutputStateStr);
                this.WaitOpc();
                this.Send("curr:prot:stat off");
                this.WaitOpc();
                this.Send("curr:prot:stat on");
                this.WaitOpc();
                //this.Query("*OPC?", 2000);
                IsOK = true;
            }
            catch
            {
                IsOK = false;
            }
            voltageNouse = this.QueryNumber("MEASure:VOLTage?");
        }

        public override bool Get_OutputStateByChannelID(int ChannelID)
        {
            bool OutputOnOffState = false;
            try
            {
                if (this.QueryNumber("OUTPut?") == 1.0)
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

        public override void Get_CurrentByChannelID(int inChannelId, out double outCurrentNow, out bool IsOK)
        {
            try
            {
                outCurrentNow = this.QueryNumber("MEASure:CURRent?");
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
            try
            {
                
                    this.Send("CURRent:PROTection " + inCurrentLimit);
                    this.Send("CURRent:PROTection:STATe ON");

                    IsOK = true;
                
            }
            catch
            {
                IsOK = false;
            }
            Thread.Sleep(500);
        }

        public override void Get_OutputVoltageByChannelID(int ChannelID, out double OutputVoltage, out bool IsOK)
        {
            try
            {
                OutputVoltage = this.QueryNumber("MEASure:VOLTage?");
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
               //2014.3.17苏渊红添加判断，是否超过25V的判断
               if (inOutputVoltage >= 25)
               {
                   this.Send(":VOLTage:RANGe P50V");
                   this.Query("*OPC?", 2000);
               }
               else 
               {
                   this.Send(":VOLTage:RANGe P25V");
                   this.Query("*OPC?", 2000);
               }
               this.Send(":VOLTage " + inOutputVoltage.ToString());
               
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
