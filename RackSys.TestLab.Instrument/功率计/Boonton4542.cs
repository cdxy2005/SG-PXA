using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    internal class Boonton4542 : PowerMeter
    {
        public Boonton4542(string inAddress)
            : base(inAddress)
        {

        }

        public override void CalPowerMeter(int ChannelNumber)
        {
            base.Send(string.Format("CALibration{0}:AUTO", ChannelNumber));
        }
        public override void WaitOpc()
        {
            //this.Query("*OPC?");波顿功率计OPC会出错
        }
        public override void WaitOpc(int inTimeOutInMs)
        {
            //this.Query("*OPC?", inTimeOutInMs);
        }
        public override void ZeroPowerMeter(int ChannelNumber)
        {
            base.Send(string.Format("CALibration{0}:ZERO", ChannelNumber));
        }
        public override void ZeroAndCalPowerMeter(int ChannleNumber)
        {
            base.Send(string.Format("CALibration{0}:ZERO", ChannleNumber));
            base.Send(string.Format("CALibration{0}:AUTO", ChannleNumber));
        }

        public override void FixedCalPowerMeter(int ChannelNuber)
        {
            base.Send(string.Format("CALibration{0}:USER:FREQcal", ChannelNuber));
        }
        public override void Preset()
        {
            base.Send("SYSTem:PRESet");
            base.Send("*CLS");
        }
        public override void SetReferenceValue(int ChannelNumber, double RefValue)
        {
            base.Send("DISPlay:TRACe" + ChannelNumber.ToString() + ":VCENTer " + RefValue.ToString());
        }

        public override void SetScale(int ChannelNumber, double Scale)
        {
            base.Send("DISPlay:TRACe" + ChannelNumber.ToString() + ":VSCALe " + Scale.ToString());
        }

        public override void AutoScale(int inChannelID)
        {
            base.Send("SYSTem:AUTOSET");
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

        /// <summary>
        /// 设置频率
        /// </summary>
        /// <param name="GateCode"></param>
        /// <param name="FreqValue">1e6 to 110.0e9 Hz 可用</param>
        public override void SetFrequence(int GateCode, double FreqValue)
        {
            base.Send(string.Format("SENSe{0}:CORRection:FREQuency {1}", GateCode, FreqValue));
        }

        public override void SetBoontonPowerMeter(int ChannelID,double PulseWidth,double Freq,double Offset)
        {
            if (PulseWidth != 0)
            {
                SetFrequence(ChannelID, Freq);
                Thread.Sleep(10);
                SetOffset(ChannelID, Offset);
                Thread.Sleep(10);
                AutoScale();
                Thread.Sleep(10);
                double delay;
                delay = PulseWidth / 5;
                base.Send(string.Format("DISPlay:PULSe:TIMEBASE {0}", delay));
                Thread.Sleep(10);
                base.Send(string.Format("TRIGger:DELay {0}", PulseWidth / 2));
                Thread.Sleep(10);
                base.Send(string.Format("MARKer1:POSItion:TIMe {0}", PulseWidth * 0.2));
                Thread.Sleep(10);
                base.Send(string.Format("MARKer2:POSItion:TIMe {0}", PulseWidth * 0.8));
                Thread.Sleep(10);
                SetChannelUnits(ChannelID, Units.W);
                Thread.Sleep(10);
                delay = GetAvgValueBeteewnMarker(ChannelID);
                base.Send(string.Format("DISPlay:TRACe{0}:VCENTer {1}", ChannelID, delay));
                Thread.Sleep(10);
                base.Send(string.Format("DISPlay:TRACe{0}:VSCALe {1}", ChannelID, (delay / 2)));
                Thread.Sleep(10);
            }
            else
            {
                SetFrequence(ChannelID, Freq);
                Thread.Sleep(10);
                SetOffset(ChannelID, Offset);
                Thread.Sleep(10);
                AutoScale();
                Thread.Sleep(10);
                SetChannelUnits(ChannelID, Units.W);
                Thread.Sleep(10);
            }
        }

        public double ReadFrequence(int sensorCode)
        {
            return base.QueryNumber(string.Format("SENS{0}:CORRection:FREQ?", sensorCode));
        }

        /// <summary>
        /// 设置全局偏置
        /// </summary>
        /// <param name="GateCode">功率计通道，双通道1,2可用</param>
        /// <param name="OffsetValue">偏置值，-300dB~+300dB可用</param>
        public override void SetOffset(int GateCode, double OffsetValue)
        {
            base.Send("SENSe" + GateCode.ToString() + ":CORRection:OFFSet " + OffsetValue.ToString());
        }

        public override void SetMeasAverageState(int SensorCode, bool AverageState)
        {
            if (!AverageState)
            { base.Send("SENSe" + SensorCode.ToString() + ":AVERage 1"); }
        }

        public override void AutoScale()
        {
            base.Send("SYSTem:AUTOSET");
        }

        /// <summary>
        /// 设置功率计单位
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <param name="units"></param>
        public override void SetChannelUnits(int ChannelNumber, Units units)
        {
            base.Send(string.Format("CALCulate{0}:UNIT {1}", ChannelNumber, units.ToString()));
        }
       
        /// <summary>
        /// 设置平均次数（脉冲模式）
        /// </summary>
        /// <param name="SensorCode"></param>
        /// <param name="AvgFactor">1~16384可用</param>
        public override void SetMeasAverageFactor(int SensorCode, int AvgFactor)
        {
            base.Send(string.Concat("SENSe", SensorCode.ToString(), ":AVERage ", AvgFactor.ToString()));
        }

        public double ReadMeasAverageFactor(int SensorCode)
        {
            return base.QueryNumber(string.Format("SENSe{0}:AVERage?", SensorCode));
        }

        /// <summary>
        /// 获取对应频点功率
        /// </summary>
        /// <param name="m_Frequency">频率，按Hz计</param>
        /// <param name="ChannelNumber">功率计通道号</param>
        /// <returns>正常返回平均功率测量值，测量不到时返回-999</returns>
        public override double MeasurePower(double m_Frequency, int ChannelNumber)
        {
            string ResultsString;
            string[] ResultsList;
            double Result = -999;

            this.SetFrequence(ChannelNumber, m_Frequency);//设置频率

            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                ResultsString = base.Query("MEASure" + ChannelNumber.ToString() + ":POWer?");
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (((ResultsList.Length < 2)) && i < 10);

            if (ResultsList.Length < 2)
            { return -999; }

            if (ResultsList[0] == "1")
            {
                if (!double.TryParse(ResultsList[1], out Result))
                { Result = -999; }
                return Result;
            }
            else
            { return -999; }
        }

        public override double MeasurePower(int ChannelNumber)
        {
            string ResultsString;
            string[] ResultsList;
            double Result = -999;

            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                ResultsString = base.Query("MEASure" + ChannelNumber.ToString() + ":POWer?");
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (ResultsList.Length < 2 && i < 10);

            if (ResultsList.Length < 2)
            { return -999; }

            if (ResultsList[0] == "1")
            {
                if (!double.TryParse(ResultsList[1], out Result))
                { Result = -999; }
                return Result;
            }
            else
            { return -999; }
        }

        /// <summary>
        /// 返回脉冲IEEE TOP功率
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetPulseTopValue(int ChannelNumber)
        {
            string ResultsInfo = string.Empty;
            ResultsInfo = base.Query("FETCh" + ChannelNumber.ToString() + ":ARRay:AMEAsure:POWer?");
            string[] ResultsList = ResultsInfo.Split(',');

            int i = 0;
            while (ResultsList.Length < 12 && i < 10)
            {
                System.Threading.Thread.Sleep(10);
                ResultsInfo = base.Query("FETCh" + ChannelNumber.ToString() + ":ARRay:AMEAsure:POWer?");
                ResultsList = ResultsInfo.Split(',');
                i++;
            }

            if (ResultsList[6] == "1")
            { return Convert.ToDouble(ResultsList[7]); }
            else
            { return -999; }
        }
 
        /// <summary>
        /// 获取脉内平均功率
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetPulseOnAvgValue(int ChannelNumber)
        {
            byte[] ResultsBytes;
            string ResultsString;
            string[] ResultsList;
            double Result = -999;

            int count = 0;
            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                base.Send("FETCh" + ChannelNumber.ToString() + ":ARRay:AMEAsure:POWer?");
                System.Threading.Thread.Sleep(10);
                base.Read(out ResultsBytes, ref count);
                ResultsString = Encoding.Default.GetString(ResultsBytes);
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (ResultsList.Length < 12 && i < 10);

            if (ResultsList.Length < 12)
            { return -999; }

            if (ResultsList[4] == "1")
            {
                if (!double.TryParse(ResultsList[5], out Result))
                { Result = -999; }
                return Result;
            }
            else
            { return -999; }
        }

        /// <summary>
        /// 获取连续波均值功率
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetCWAvgValue(int ChannelNumber)
        {
            string ResultsString;
            string[] ResultsList;
            double Result = -999;

            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                ResultsString=base.Query("FETCh" + ChannelNumber.ToString() + ":ARRay:CW:POWer?");
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (ResultsList.Length < 8 && i < 10);

            if (ResultsList.Length < 8)
            { return -999; }

            if (ResultsList[2] == "1")
            {
                if (!double.TryParse(ResultsList[3], out Result))
                { Result = -999; }
                return Result;
            }
            else
            { return -999; }
        }

        /// <summary>
        /// 获取连续波最大功率值
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetCWMaxValue(int ChannelNumber)
        {
            byte[] ResultsBytes;
            string ResultsString;
            string[] ResultsList;
            double Result = -999;

            int count = 0;
            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                base.Send("FETCH" + ChannelNumber.ToString() + ":ARRay:CW:POWer?");
                System.Threading.Thread.Sleep(30);
                base.Read(out ResultsBytes, ref count);
                ResultsString = Encoding.Default.GetString(ResultsBytes);
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (ResultsList.Length < 8 && i < 10);
            if (ResultsList.Length < 8)
            {
                return -999;
            }
            else if (ResultsList[2] == "1")
            {
                if (!double.TryParse(ResultsList[3], out Result))
                { return -999; }
                return Result;
            }
            else
            { return -999; }
        }

        /// <summary>
        /// 获取脉冲峰值功率
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetPulsePeakValue(int ChannelNumber)
        {
            byte[] ResultsBytes;
            string ResultsString;
            string[] ResultsList;
            double Result = -999;

            int count = 0;
            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                base.Send("FETCh" + ChannelNumber.ToString() + ":ARRay:AMEAsure:POWer?");
                System.Threading.Thread.Sleep(10);
                base.Read(out ResultsBytes, ref count);
                ResultsString = Encoding.Default.GetString(ResultsBytes);
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (ResultsList.Length < 12 && i < 10);

            if (ResultsList.Length < 12)
            { return -999; }

            if (ResultsList[0] == "1")
            {
                if (!double.TryParse(ResultsList[1], out Result))
                { Result = -999; }
                return Result;
            }
            else
            { return -999; }
        }

        public override double GetPulseCycleAvgValue(int ChannelNumber)
        {
            byte[] ResultsBytes;
            string ResultsString;
            string[] ResultsList;
            double Result = -999;

            int count = 0;
            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                base.Send("FETCh" + ChannelNumber.ToString() + ":ARRay:AMEAsure:POWer?");
                System.Threading.Thread.Sleep(10);
                base.Read(out ResultsBytes, ref count);
                ResultsString = Encoding.Default.GetString(ResultsBytes);
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (ResultsList.Length < 12 && i < 10);

            if (ResultsList.Length < 12)
            { return -999; }

            if (ResultsList[2] == "1")
            {
                if (!double.TryParse(ResultsList[3], out Result))
                { Result = -999; }
                return Result;
            }
            else
            { return -999; }
        }

        /// <summary>
        /// 获取脉冲上升沿时间
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetPulseRiseTime(int ChannelNumber)
        {
            byte[] ResultsBytes;
            string ResultsString;
            string[] ResultsList;
            double Result = -999;

            int count = 0;
            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                base.Send("FETCh" + ChannelNumber.ToString() + ":ARRay:AMEAsure:TIMe?");
                System.Threading.Thread.Sleep(10);
                base.Read(out ResultsBytes, ref count);
                ResultsString = Encoding.Default.GetString(ResultsBytes);
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (ResultsList.Length < 18 && i < 10);

            if (ResultsList.Length < 18)
            { return -999; }

            if (ResultsList[10] == "1")
            {
                if (!double.TryParse(ResultsList[11], out Result))
                { Result = -999; }
                return Result;
            }
            else
            { return -999; }
        }

        /// <summary>
        /// 获取脉冲下降沿时间
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetPulseFallTime(int ChannelNumber)
        {
            byte[] ResultsBytes;
            string ResultsString;
            string[] ResultsList;
            double Result = -999;

            int count = 0;
            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                base.Send("FETCh" + ChannelNumber.ToString() + ":ARRay:AMEAsure:TIMe?");
                System.Threading.Thread.Sleep(10);
                base.Read(out ResultsBytes, ref count);
                ResultsString = Encoding.Default.GetString(ResultsBytes);
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (ResultsList.Length < 18 && i < 10);

            if (ResultsList.Length < 18)
            { return -999; }

            if (ResultsList[12] == "1")
            {
                if (!double.TryParse(ResultsList[13], out Result))
                { Result = -999; }
                return Result;
            }
            else
            { return -999; }
        }

        /// <summary>
        /// 获取脉冲宽度
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetPulseWidth(int ChannelNumber)
        {
            string ResultsString;
            string[] ResultsList;
            double Result = -999;

            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                ResultsString=base.Query("FETCh" + ChannelNumber.ToString() + ":ARRay:AMEAsure:TIMe?");
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (ResultsList.Length < 18 && i < 10);

            if (ResultsList.Length < 18)
            { return -999; }

            if (ResultsList[4] == "1")
            {
                if (!double.TryParse(ResultsList[5], out Result))
                { Result = -999; }
                return Result;
            }
            else
            { return -999; }
        }

        public override void SetTraceX(int TunnelCode, double XTimeStart, double XTimeLength)
        {
            base.Send("TRIGger:POSition LEFT");
            base.Send("TRIGger:DELay " + XTimeStart.ToString());
            base.Send("DISPlay:PULSe:TIMEBASE " + (XTimeLength / 10).ToString());
        }

        public override void SetTraceXByCenter(int TunnelCode, double XTimeCenter, double XTimeSpan)
        {
            base.Send("TRIGger:POSition MIDDLE,");
            base.Send("TRIGger:DELay " + XTimeCenter.ToString());
            base.Send("DISPlay:PULSe:TIMEBASE " + (XTimeSpan / 10).ToString());
        }

        public override void SetTrigEdge(PowerMeter.TrigEdge EdgeCode)
        {
            if (EdgeCode == TrigEdge.POSitive)
            { base.Send("TRIGger:SLOPe POS"); }
            else if (EdgeCode == TrigEdge.NEGative)
            { base.Send("TRIGger:SLOPe NEG"); }
        }

        /// <summary>
        /// 显示模式
        /// </summary>
        /// <param name="Modedata">‘GRAPH’和“TEXT”两种可选</param>
        public override void DisplayMode(string Modedata)
        {
            base.Send("DISPlay:MODE "+Modedata);
        }

        /// <summary>
        /// 获取两个marker之间平均值
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetAvgValueBeteewnMarker(int ChannelNumber)
        {
            string ResultsString;
            string[] ResultsList;
            double Result = -999;
            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                ResultsString = base.Query("FETCh" + ChannelNumber.ToString() + ":ARRay:MARKer:POWer?");
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (ResultsList.Length < 14 && i < 10);

            if (ResultsList.Length < 14)
            { return -999; }

            if (ResultsList[0] == "1")
            {
                if (!double.TryParse(ResultsList[1], out Result))
                { Result = -999; }
                return Result;
            }
            else
            { return -999; }
        }

        /// <summary>
        /// 获取marker1的值
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetMarker1Value(int ChannelNumber)
        {
            string ResultsString;
            string[] ResultsList;
            double Result = -999;

            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                ResultsString=base.Query("FETCh" + ChannelNumber.ToString() + ":ARRay:MARKer:POWer?");
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (ResultsList.Length < 14 && i < 10);

            if (ResultsList.Length < 14)
            { return -999; }

            if (ResultsList[8] == "1")
            {
                if (!double.TryParse(ResultsList[9], out Result))
                { Result = -999; }
                return Result;
            }
            else
            { return -999; }
        }

        /// <summary>
        /// 获取marker2的值
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetMarker2Value(int ChannelNumber)
        {
            string ResultsString;
            string[] ResultsList;
            double Result = -999;

            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(10);
                ResultsString=base.Query("FETCh" + ChannelNumber.ToString() + ":ARRay:MARKer:POWer?");
                ResultsList = ResultsString.Split(',');
                i++;
            }
            while (ResultsList.Length < 14 && i < 10);

            if (ResultsList.Length < 14)
            { return -999; }

            if (ResultsList[10] == "1")
            {
                if (!double.TryParse(ResultsList[11], out Result))
                { Result = -999; }
                return Result;
            }
            else
            { return -999; }
        }
    }
}