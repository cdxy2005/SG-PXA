using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    internal class AgilentN1914 : PowerMeter
    {
        public AgilentN1914(string inAddress)
            : base(inAddress)
        {
        }

        public override void AutoGate(int inChannelNum, int GateCode)
        {
            string[] str = new string[] { "SENS", inChannelNum.ToString(), ":SWE", GateCode.ToString(), ":AUTO ONCE" };
            base.SendOpc(string.Concat(str), 5);
        }

        public override void AutoScale(int inChannelID)
        {
            base.SendOpc(string.Concat("SENSe", inChannelID.ToString(), ":TRACe:AUToscale"), 5);
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




        public override double MeasurePower(double m_Frequency,int ChannelNumber)
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

        public override void MReceiverSystemConfigure()
        {
            base.SendOpc("SYST:COMM:LAN:ADDR '192.168.100.2'", 100);
            base.SendOpc("SYST:COMM:LAN:SMAS '255.255.255.0'", 100);
            base.SendOpc(":CALibration:ALL", 1000);
            Thread.Sleep(5000);
        }

        public override void OffsetSwitch(int GateCode, bool IsOffsetOn)
        {
            if (!IsOffsetOn)
            {
                base.Send(string.Concat("CALC", GateCode.ToString(), ":GAIN:STAT OFF"));
            }
            else
            {
                base.Send(string.Concat("CALC", GateCode.ToString(), ":GAIN:STAT ON"));
            }
        }

        public override void Reset()
        {
            base.Send("SYST:PRES DEF");
        }

        public override double ReturnMeasValue(int ChannelNumber)
        {
            double num;
            double num1 = 0;
            try
            {
                num = Convert.ToDouble(base.Query("FETCH" + ChannelNumber.ToString() + ":POW?"));
            }
            catch
            {
                double num2 = -999;
                num1 = num2;
                num = num2;
            }
            return num;
        }

        public override void SetAcquisitionMode(int TunnelCode, bool IsContinuousTrigOn)
        {
            if (!IsContinuousTrigOn)
            {
                base.SendOpc(string.Concat("INIT", TunnelCode.ToString(), ":CONT OFF"), 10);
            }
            else
            {
                base.SendOpc(string.Concat("INIT", TunnelCode.ToString(), ":CONT ON"), 10);
            }
        }

        public override void SetDispType(int WindowCode, PowerMeter.DispType DisplayTypeCode)
        {
            base.Send(string.Concat("DISP:WIND", WindowCode.ToString(), ":FORM ", DisplayTypeCode.ToString()));
        }

        public override void SetMeasAverageState(int SensorCode, bool AverageState)
        {
            if (AverageState)
            { base.Send(string.Concat("SENSe", SensorCode.ToString(), ":AVERage ON")); }
            else
            { base.Send(string.Concat("SENSe", SensorCode, ":AVERage OFF")); }
        }

        public override void SetMeasAutoAverageMode(int SensorCode, bool AverageAutoState)
        {
            if (AverageAutoState)
            { base.Send(string.Concat("SENSe", SensorCode.ToString(), ":AVERage:COUNt:AUTO ON")); }
            else
            { base.Send(string.Concat("SENSe", SensorCode.ToString(), ":AVERage:COUNt:AUTO OFF")); }
        }

        public override void SetMeasAverageFactor(int SensorCode, int AvgFactor)
        {
            base.Send(string.Concat("SENSe", SensorCode.ToString(), ":AVERage:COUNt ", AvgFactor.ToString()));
        }

        public override void SetFrequence(int GateCode, double FreqValue)
        {
          //  string[] str = new string[] { "SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()};
            base.Send(string.Concat("SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()));
            //string[] str = new string[] { "SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()};
            base.Send(string.Concat("SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()));
        }

        public override void SetGate(int TunnelCode, int GateCode, double GateStartTime, double GateTimeLength)
        {
            string[] str = new string[] { "SENS", TunnelCode.ToString(), ":SWE", GateCode.ToString(), ":OFFS:TIME ", GateStartTime.ToString() };
            base.SendOpc(string.Concat(str), 10);
            str = new string[] { "SENS", TunnelCode.ToString(), ":SWE", GateCode.ToString(), ":TIME ", GateTimeLength.ToString() };
            base.SendOpc(string.Concat(str), 10);
        }

        public override void SetMeasType(int TunnelCode, int GateCode, PowerMeter.MeasType MeasurementTypeCode)
        {
            object[] str = new object[] { "CALC1:FEED", TunnelCode.ToString(), " \"POW:", MeasurementTypeCode, " ON SWEEP", GateCode.ToString(), "\"" };
            base.SendOpc(string.Concat(str), 10);
        }

        public override void SetOffset(int GateCode, double OffsetValue)
        {
            base.Send(string.Concat("SENS", GateCode.ToString(), ":CORR:GAIN2 ", OffsetValue.ToString()));
        }

        public override void SetScreenFormat(PowerMeter.ScreenFormat ScrnFmt)
        {
            base.Send(string.Concat("DISP:SCReen:FORM ", ScrnFmt.ToString()));
        }

        public override void SetTraceX(int TunnelCode, double XTimeStart, double XTimeLength)
        {
            base.SendOpc(string.Concat("SENSe", TunnelCode.ToString(), ":TRACe:OFFSet:TIME ", XTimeStart.ToString()), 5);
            base.SendOpc(string.Concat("SENSe", TunnelCode.ToString(), ":TRACe:TIME ", XTimeLength.ToString()), 5);
        }
        public override void GetTraceX(int TunnelCode,out double XTimeStart,out double XTimeLength)
    {
       XTimeStart= this.QueryNumber(string.Format("SENSe{0}:TRACe:OFFSet:TIME?", TunnelCode));
       XTimeLength=this.QueryNumber(string.Format("SENSe{0}:TRACe:TIME ", TunnelCode));
        }
        public override void SetTrigEdge(PowerMeter.TrigEdge EdgeCode)
        {
            base.SendOpc(string.Concat("TRIG:SEQ:SLOP ", EdgeCode.ToString()), 10);
        }

        public override void SetTrigSource(int TunnelCode, PowerMeter.TrigSourMode TrigSourCode)
        {
            base.SendOpc(string.Concat("TRIG", TunnelCode.ToString(), ":SOUR ", TrigSourCode.ToString()), 10);
        }

        public override void RefSourceEnable(bool value)
        {
            this.Send(string.Concat("OUTP:ROSC:STAT ", (value ? "ON" : "OFF")));
        }

        public override void ReturnSensorInfo(int TunnelCode,out SensorInfo m_SensorInfo)
        {
            m_SensorInfo = new SensorInfo();
            m_SensorInfo.m_Type = "QueryFailed";
            m_SensorInfo.m_SN = "QueryFailed";
            m_SensorInfo.m_PowerRangeMax = "QueryFailed";
            m_SensorInfo.m_PowerRangeMin = "QueryFailed";
            m_SensorInfo.m_FreqRangeMax = "QueryFailed";
            m_SensorInfo.m_FreqRangeMin = "QueryFailed";

            try
            { m_SensorInfo.m_Type = this.Query("SERVice:SENSor" + TunnelCode.ToString() + ":TYPE?"); }
            catch
            { m_SensorInfo.m_Type = "QueryFailed"; }
            try
            { m_SensorInfo.m_SN = this.Query("SERVice:SENSor" + TunnelCode.ToString() + ":SNUMber?"); }
            catch
            { m_SensorInfo.m_SN = "QueryFailed"; }
            if (m_SensorInfo.m_Type == "E4413A")
            {
                m_SensorInfo.m_PowerRangeMax = "20";
                m_SensorInfo.m_PowerRangeMin = "-70";
                m_SensorInfo.m_FreqRangeMax = "26.5e9";
                m_SensorInfo.m_FreqRangeMin = "50e6";
            }
            else if(m_SensorInfo.m_Type=="N8481B")
            {
                m_SensorInfo.m_PowerRangeMax = "44";
                m_SensorInfo.m_PowerRangeMin = "-5";
                m_SensorInfo.m_FreqRangeMax = "18e9";
                m_SensorInfo.m_FreqRangeMin = "10e6";
            }
        }

        public override string[] SensorType
        {
            get
            {
                string[] tmp = new string[2];
                tmp[0] = "E4413A";
                tmp[1] = "N8481B";
                return tmp;
            }
        }

        public override int ChnsCount
        {
            get
            {
                return 2;
            }
        }

    }
}
