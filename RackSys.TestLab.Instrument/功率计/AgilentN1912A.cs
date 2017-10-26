using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace RackSys.TestLab.Instrument
{
    //功率计参数设置结构
    public class PowerMeterSetting
    {
        private double m_Freq = 1e9;

        public double Freq
        {
            get { return m_Freq; }
            set { m_Freq = value; }
        }

        private uint m_AverageNumber = 1;

        public uint AverageNumber
        {
            get { return m_AverageNumber; }
            set { m_AverageNumber = value; }
        }

        private double m_Offset = 0;

        public double Offset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }

        private RackSys.TestLab.Instrument.PowerMeter.MeasType m_MeasureType = RackSys.TestLab.Instrument.PowerMeter.MeasType.AVER;

        public RackSys.TestLab.Instrument.PowerMeter.MeasType MeasureType
        {
            get { return m_MeasureType; }
            set { m_MeasureType = value; }
        }

        private int m_ChannelNumber = 1;

        public int ChannelNumber
        {
            get { return m_ChannelNumber; }
            set { m_ChannelNumber = value; }
        }



    }

    internal class AgilentN1912A : PowerMeter
    {
        public AgilentN1912A(string inAddress)
            : base(inAddress)
        {
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

        public override double GetPulseWidth(int ChannelNumber)
        {
            //TRAC:MEAS:TRAN10:POS:DUR? 
            string str = string.Format("TRAC{0}:MEAS:TRAN1:POS:DUR?", ChannelNumber);
            return base.GetPulseWidth(ChannelNumber);
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

        public override void Preset(PresetTypes PType)
        {
            base.Send(string.Concat("SYSTem:PRESet ", PType.ToString()));
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

        public override double ReturnPulsePeriod(int TraceNum, int PulseNum)
        {
            return base.QueryNumber(string.Format("TRACe{0}:MEASurement:PULSe{1}:PERiod?", new object[] { TraceNum, PulseNum }));
        }

        public override double ReturnRiseEdge(int TraceNum, int PulseNum)
        {
            return base.QueryNumber(string.Format("TRACe{0}:MEASurement:TRANsition{1}:POSitive:DURation?", new object[] { TraceNum, PulseNum }));
        }

        public override double ReturnFallEdge(int TraceNum, int PulseNum)
        {
            return base.QueryNumber(string.Format("TRACe{0}:MEASurement:TRANsition{1}:NEGative:DURation?", new object[] { TraceNum, PulseNum }));
        }
        public override void GetTraceX(int TunnelCode, out double XTimeStart, out double XTimeLength)
        {
            XTimeStart = this.QueryNumber(string.Format("SENSe{0}:TRACe:OFFSet:TIME?", TunnelCode));
            XTimeLength = this.QueryNumber(string.Format("SENSe{0}:TRACe:TIME?", TunnelCode));
        }
        public override void SetTraceState(int ChannelNumber, bool TraceState)
        {
            string strTraceState = TraceState ? "1" : "0";
            base.Send(string.Format("TRACe{0}:STATe {1}", new object[] { ChannelNumber, strTraceState }));
        }

        public override void SetInitiateContinuousState(int ChannelNumber, bool State)
        {
            string strState = State ? "1" : "0";
            base.Send(string.Format("INITiate{0}:CONTinuous {1}", new object[] { ChannelNumber, strState }));
        }

        public override void GetTraceDataByChannel(int ChannelNumber, out double[] TraceData)
        {
            byte[] result;
            result = base.ReadBlock(string.Concat("TRACe", ChannelNumber, "? LRES"));
            //AgN191x N1911A = new AgN191x("TCPIP0::192.168.1.105::inst0::INSTR");
            //N1911A.SCPI.TRACe.DATA.Query(null, "LRESolution", out data);
            try
            {
                TraceData = new double[result.Length / 4];
                for (int i = 0, j = 0; j < TraceData.Length; i += 4, j++)
                {
                    byte[] tempByte = new byte[] { result[i + 3], result[i + 2], result[i + 1], result[i] };
                    TraceData[j] = BitConverter.ToSingle(tempByte, 0);
                }
            }
            catch (Exception ex)
            {
                throw (new Exception(string.Format("非法数据解析失败：{0}", ex.ToString())));
            }
        }

        public override void GetTraceData(int ChannelNumber, TraceDataResolution Resolution, out double[] TraceData)
        {
            byte[] result;
            result = base.ReadBlock(string.Format("TRACe{0}? {1}", new object[] { ChannelNumber, Resolution }));

            try
            {
                TraceData = new double[result.Length / 4];
                for (int i = 0, j = 0; j < TraceData.Length; i += 4, j++)
                {
                    byte[] tempByte = new byte[] { result[i + 3], result[i + 2], result[i + 1], result[i] };
                    TraceData[j] = BitConverter.ToSingle(tempByte, 0);
                }
            }
            catch (Exception ex)
            {
                throw (new Exception(string.Format("非法数据解析失败：{0}", ex.ToString())));
            }
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
            base.Send(string.Format("DISPlay:WINDow{0}:FORMat {1}", new object[] { WindowCode, DisplayTypeCode }));
        }

        public override void SetAutoGateRef(int ChannelNum, int GateNum, int RefNum, double RefPercentage)
        {
            base.Send(string.Format("SENSe{0}:SWEep{1}:AUTO:REF{2} {3}", new object[] { ChannelNum, GateNum, RefNum, RefPercentage }));
        }

        public override void AutoGate(int inChannelID, int GateCode)
        {
            base.Send(string.Format("SENSe{0}:SWEep{1}:AUTO ONCE", new object[] { inChannelID, GateCode }));
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

        public override void SetVideoAverageState(int Sensor, bool AutoState)
        {
            string strAutoState = AutoState ? "1" : "0";
            base.Send(string.Format("SENSe{0}:AVERage2 {1}", new object[] { Sensor, strAutoState }));
        }

        public override void SetVideoAverageFactor(int Sensor, int AvgFactor)
        {
            base.Send(string.Format("SENSe{0}:AVERage2:COUNt {1}", new object[] { Sensor, AvgFactor }));
        }

        public override void SetVideoBW(int Sensor, PowerMeter.VideoBandWidth VBW)
        {
            base.Send(string.Format("SENSe{0}:BWIDth:VIDeo {1}", new object[] { Sensor, VBW }));
        }

        public double ReadMeasAverageFactor()
        {

            return base.QueryNumber("AVER:COUN?");
        }

        public override void SetFrequence(int GateCode, double FreqValue)
        {
            //  string[] str = new string[] { "SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()};
            base.Send(string.Concat("SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()));
            //string[] str = new string[] { "SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()};
            base.Send(string.Concat("SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()));
        }


        public double ReadFrequence(int sensorCode)
        {
            return base.QueryNumber(string.Format("SENS{0}:FREQ?", sensorCode));
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
            switch (MeasurementTypeCode)
            {
                case MeasType.AVER:
                    {
                        string cmd = string.Format("CALC{0}:FEED{1} ", TunnelCode, GateCode) + "\"POW:AVER\"";
                        base.Send(cmd);
                        break;
                    }
                case MeasType.PEAK:
                    {
                        string cmd = string.Format("CALC{0}:FEED{1} ", TunnelCode, GateCode) + "\"POW:PEAK\"";

                        base.Send(cmd);
                        break;
                    }
                default: break;
            }
        }
        public PowerMeter.MeasType ReadMeasType(int TunnelCode, int GateCode)
        {
            string type = base.Query(string.Format("CALC{0}:FEED{1}?", TunnelCode, GateCode));
            if (type.Contains("AVER")) return MeasType.AVER;
            else if (type.Contains("PEAK")) return MeasType.PEAK;
            return MeasType.AVER;

        }

        public override void SetOffset(int GateCode, double OffsetValue)
        {
            base.Send(string.Concat("SENS", GateCode.ToString(), ":CORR:GAIN2 ", OffsetValue.ToString()));
        }
        public double ReadOffset(int GateCode)
        { return base.QueryNumber(string.Format("SENS{0}:CORR:GAIN2?", GateCode)); }





        public override void SetScreenFormat(PowerMeter.ScreenFormat ScrnFmt)
        {
            base.Send(string.Concat("DISPlay:SCReen:FORMat ", ScrnFmt.ToString()));
        }

        public override void SetTraceX(int TunnelCode, double XTimeStart, double XTimeLength)
        {
            //base.SendOpc(string.Concat("SENSe", TunnelCode.ToString(), ":TRACe:OFFSet:TIME ", XTimeStart.ToString()), 5);
            //base.SendOpc(string.Concat("SENSe", TunnelCode.ToString(), ":TRACe:TIME ", XTimeLength.ToString()), 5);
            base.SendOpc(string.Concat("SENSe", TunnelCode.ToString(), ":TRACe:OFFSet:TIME ", XTimeStart.ToString()), 5);
            base.Send(string.Concat("SENSe", TunnelCode.ToString(), ":TRACe:TIME ", XTimeLength.ToString()));
        }

        public override void SetScale(int TunnelCode, double Scale)
        {
            base.Send("SENSe" + TunnelCode.ToString() + ":TRACe:Y:SCALe:PDIV " + Scale.ToString());
        }

        public override void SetTrigEdge(PowerMeter.TrigEdge EdgeCode)
        {
            base.SendOpc(string.Concat("TRIG:SEQ:SLOP ", EdgeCode.ToString()), 10);
        }

        public override void SetTrigSource(int TunnelCode, PowerMeter.TrigSourMode TrigSourCode)
        {
            base.SendOpc(string.Concat("TRIG", TunnelCode.ToString(), ":SOUR ", TrigSourCode.ToString()), 10);
        }

        public override void SetTrigAutoLvlState(bool isAutoLvl)
        {
            string AutoLvlState = isAutoLvl ? "1" : "0";
            base.Send(string.Concat("TRIGger:LEVel:AUTO ", AutoLvlState));
        }

        public override void RefSourceEnable(bool value)
        {
            this.Send(string.Concat("OUTP:ROSC:STAT ", (value ? "ON" : "OFF")));
        }

        public override void ReturnSensorInfo(int TunnelCode, out SensorInfo m_SensorInfo)
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
            else if (m_SensorInfo.m_Type == "N8481B")
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
                tmp[0] = "N1922A";
                tmp[1] = "NA";
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

        public override Image CaptureScreenImage()
        {
            byte[] ImageDataInBytes = base.ReadBlock("SYSTem:DISPlay:BMP?");
            return ImageBytesConvertor.ConvertByteToImg(ImageDataInBytes);
        }


        public override bool PowerMeterConfigure(PowerMeterSetting PMSetting)
        {
            this.SetMeasType(1, PMSetting.ChannelNumber, PMSetting.MeasureType);
            this.WaitOpc();
            this.SetOffset(PMSetting.ChannelNumber, PMSetting.Offset);
            this.WaitOpc();
            this.SetFrequence(PMSetting.ChannelNumber, PMSetting.Freq);
            this.WaitOpc();
            this.SetMeasAverageFactor(PMSetting.ChannelNumber, (int)PMSetting.AverageNumber);
            this.WaitOpc();

            return true;
        }


        public override bool ReadPowerMeterConfiguration(out PowerMeterSetting PMSettingResult)
        {
            PMSettingResult = new PowerMeterSetting();
            int ChannelNumber = 1;
            try
            {
                this.Query("SENS1:FREQ?");


            }
            catch
            {
                ChannelNumber = 2;

            }
            PMSettingResult.MeasureType = this.ReadMeasType(1, ChannelNumber);
            PMSettingResult.Offset = this.ReadOffset(ChannelNumber);
            PMSettingResult.Freq = this.ReadFrequence(ChannelNumber);
            PMSettingResult.AverageNumber = (uint)this.ReadMeasAverageFactor();

            return true;
        }
    }
}
