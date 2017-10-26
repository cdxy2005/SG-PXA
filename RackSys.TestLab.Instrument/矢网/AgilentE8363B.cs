using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class E8363C : NetworkAnalyser
    {
        public int IsLink = 0;
        private string NWA_FileName;

        public override bool bIsLink()
        {
            bool result = false;
            if (this.IsLink == 1)
            {
                result = true;
            }
            return result;
        }

        public override void Close()
        {
            this.NWA_visa.Close();
        }

        public override void Dispose()
        {
        }

        public override float Get_MarkerPosition(int num)
        {
            base.SendT(":CALCulate1:MARKer" + num + ":X?", true);
            return float.Parse(this.NWA_visa.ReadStringT());
        }

        public override string Get_CurrentDir()
        {
            string result = "";
            base.Send("MMEMory:CDIRectory?", true);
            result = this.NWA_visa.ReadStringT();
            this.NWA_FileName = "\"" + result.Substring(1, result.Length - 3) + "\\TX1AMS_tmp.png\"";
            return result;
        }

        public override double Get_CWFrequency()
        {
            base.Send(":SENSe1:FREQuency:CW?", true);
            return Convert.ToDouble(this.NWA_visa.ReadStringT());
        }

 
        public override float Get_MarkerValue(int num)
        {
            base.SendT(":CALCulate1:MARKer" + num + ":Y?", true);
            return float.Parse(LD_String.subs(this.NWA_visa.ReadStringT(), 1, ','));
        }

        public override float Get_RfOutputLevel(int ch, int port)
        {
            base.Send("SOURce" + ch.ToString() + ":POWer" + port.ToString() + ":LEVel:IMMediate:AMPLitude?", true);
            return float.Parse(this.NWA_visa.ReadStringT());
        }

        public override float Get_dBperDiv(int window, int trace)
        {
            base.Send("DISPlay:WINDow" + window.ToString() + ":TRACe" + trace.ToString() + ":Y:SCALe:PDIVision?", true);
            return float.Parse(this.NWA_visa.ReadStringT());
        }

        public override string Get_TraceName()
        {
            base.SendT("CALC:PAR:CAT? ", true);
            return this.NWA_visa.ReadStringT();
        }

        public override void HCOPy()
        {
            base.Send(":HCOPy:FILE " + this.NWA_FileName, true);
            base.Send("*OPC?", true);
            this.NWA_visa.ReadStringT();
        }

        public override void LoadStateFile(string path)
        {
            base.Send("MMEMory:LOAD:CSARchive \"" + path + "\"", true);
            Thread.Sleep(0x1770);
        }

        public override void OpenString()
        {
            IniFile inifile = new IniFile(globalvariables.MainPath + @"\switch.ini");
            int index = inifile.GetInt16("config", "NWA_Item", -1);
            string strIP = inifile.GetString("config", "矢网IP" + index.ToString(), "IP无记录，请设置");
            string strPort = inifile.GetString("config", "矢网port" + index.ToString(), "PORT无记录，请设置");
            string symbol = "TCPIP0::" + strIP + "::inst0::INSTR";
            this.NWA_visa.OpenString(symbol);
        }

        public override bool Get_MarkerState(int num)
        {
            bool result = false;
            base.SendT("CALCulate1:MARKer" + num.ToString() + ":STATe?", true);
            string ans = this.NWA_visa.ReadStringT();
            if (int.Parse(ans) == 0)
            {
                return false;
            }
            if (int.Parse(ans) == 1)
            {
                result = true;
            }
            return result;
        }

        public override byte[] CaptureScreenImage(string strDIR)
        {
            this.Get_CurrentDir();
            this.HCOPy();
            return this.TRANsfer();
        }

        public override void SelectTrace(string name)
        {
            base.Send(":CALCulate1:PARameter:SELect \"" + name + "\"");
        }

        public override void SetAddress(string addr)
        {
        }

        public override void AutoScale(int window, int trace)
        {
            base.Send(string.Format("display:window{0}:trace{1}:y:scale:auto", new object[]{ window, trace}));
        }

        public override void GoToLocal()
        {
        }

        public override void Set_OutputPower(float pow)
        {
        }

        public override void Set_RfOutputState(bool state)
        {
            if (state)
            {
                base.Send("OUTPut:STATe ON", true);
            }
            else if (!state)
            {
                base.Send("OUTPut:STATe OFF", true);
            }
        }

        public override void Set_dBperDiv(int window, int trace, float div)
        {
            base.Send("display:window" + window.ToString() + ":trace" + trace.ToString() + ":y:scale:pdivision " + div.ToString(), true);
        }

        public override void SetSweepModeToContTrigger()
        {
            base.Send("SENS1:SWE:MODE CONT;", true);
            base.Send(":TRIGger:SEQuence:SOURce IMMediate", true);
        }

        public override void SetSweepModeToSingle()
        {
            base.Send("SENS1:SWE:MODE SINGle;*OPC?", true);
            this.NWA_visa.ReadStringT();
            base.Send(":TRIGger:SEQuence:SOURce MANual", true);
        }

        public override void Set_TraceMarkerState(int traceNum, int markerNum, bool state)
        {
            if (state)
            {
                base.Send("CALCulate" + traceNum.ToString() + ":MARKer" + markerNum.ToString() + ":STATe ON", true);
            }
            else if (!state)
            {
                base.Send("CALCulate" + traceNum.ToString() + ":MARKer" + markerNum.ToString() + ":STATe OFF", true);
            }
        }

        public override void Set_TraceMarkerPosition(int traceNum, int markerNum, float x)
        {
            base.Send("CALCulate" + traceNum.ToString() + ":MARKer" + markerNum.ToString() + ":X " + x.ToString(), true);
        }

        public override void TestLink()
        {
            try
            {
                this.OpenString();
                Thread.Sleep(30);
                base.Send("*IDN?", true);
                string idn = this.NWA_visa.ReadString();
                this.IsLink = 1;
            }
            catch (SystemException)
            {
                this.IsLink = 0;
            }
            new UI_LineAnimation().VectorNetworkAnalyzer_PipelineColoring(this.IsLink, globalvariables.mainwindow);
        }

        public override byte[] TRANsfer()
        {
            byte[] buf = new byte[0xffff];
            base.Send(":MMEMory:TRANsfer? " + this.NWA_FileName, true);
            return this.NWA_visa.ReadStream();
        }

        public override void TriggerSweep()
        {
            base.Send("INITiate1:IMMediate;*OPC?", true);
            this.NWA_visa.ReadStringT();
        }
    }
}
