using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RackSys.TestLab.Instrument
{
    public abstract class PowerMeter : ScpiInstrument
    {
        public PowerMeter(string address)
            : base(address)
        {
        }

        public virtual void AutoGate(int inChannelID, int GateCode)
        { }
                //杨飞添加驱动 2016.01.08
        /// <summary>
        /// 固定校准
        /// </summary>
        /// <param name="ChannelNuber"></param>
        public virtual void FixedCalPowerMeter(int ChannelNuber) { }
        /// <summary>
        /// 获取Marker1点处的功率值
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public virtual double GetMarker1Value(int ChannelNumber)
        { return 0; }
        ///// <summary>
        ///// 获取脉冲下降沿时间
        ///// </summary>
        ///// <param name="ChannelNumber"></param>
        ///// <returns></returns>
        //public virtual double GetPulseFallTime(int ChannelNumber)
        //{ return 0; }
        ///// <summary>
        ///// 获取脉冲上升沿时间
        ///// </summary>
        ///// <param name="ChannelNumber"></param>
        ///// <returns></returns>
        //public virtual double GetPulseRiseTime(int ChannelNumber)
        //{ return 0; }
        /// <summary>
        /// 获取脉冲IEEE TOP功率
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        //public virtual double GetPulseTopValue(int ChannelNumber) { return 0; }
        public virtual void SetTraceXByCenter(int TunnelCode, double XTimeCenter, double XTimeSpan)
        { }
        /// <summary>
        /// 设置测脉冲波时候的波顿功率计
        /// </summary>
        /// <param name="ChannelID">通道ID</param>
        /// <param name="PulseWidth">脉宽</param>
        /// <param name="Freq">频率</param>
        /// <param name="Offset">偏置</param>
        public virtual void SetBoontonPowerMeter(int ChannelID, double PulseWidth, double Freq, double Offset)
        { }
        /// <summary>
        /// 获取脉冲宽度
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public virtual double GetPulseWidth(int ChannelNumber)
        { return 0; }
        /// <summary>
        /// 获取脉内平均功率
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public virtual double GetPulseOnAvgValue(int ChannelNumber) { return 0; }
        /// <summary>
        /// 获取脉冲峰值功率
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        //public virtual double GetPulsePeakValue(int ChannelNumber) { return 0; }
        /// <summary>
        /// 获取Marker2点处的功率值
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        /// 
        public virtual double GetMarker2Value(int ChannelNumber)
        { return 0; }
        /// <summary>
        /// 获取脉冲波周期平均功率
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public virtual double GetPulseCycleAvgValue(int ChannelNumber) { return 0; }

        public virtual void AutoScale(int inChannelID)
        {
        }

        public virtual void CalPowerMeter(int ChannleNumber)
        {
        }

        public virtual void ZeroPowerMeter(int ChannleNumber)
        { }

        public virtual void ZeroAndCalPowerMeter(int ChannleNumber)
        { }
        public virtual void RefSourceEnable(bool value)
        { }

        private static PowerMeter.ValidateSupportDelegate m_validateSupportDelegate;

        public static PowerMeter Connect(string currentAddress, PowerMeter.ValidateSupportDelegate supportDelegate, bool interactive)
        {
            PowerMeter APowerMeter = null;
            string str = (currentAddress != null ? currentAddress : "GPIB0::18::INSTR");
            PowerMeter.m_validateSupportDelegate = supportDelegate;
            if (interactive)
            {
                throw new Exception("不支持交互模式");
            }
            try
            {
                if (PowerMeter.DetermineSupport(str) == null)
                {
                    APowerMeter = PowerMeter.CreateDetectedPowerMeter(str);
                }
            }
            catch
            {
                //throw;
            }
            PowerMeter.m_validateSupportDelegate = null;
            if (APowerMeter != null)
            {
                APowerMeter.Connected = true;
            }
            return APowerMeter;
        }

        private static string DetermineSupport(string address)
        {
            if (PowerMeter.m_validateSupportDelegate == null)
            {
                return null;
            }
            PowerMeter APowerMeter = null;
            try
            {
                APowerMeter = PowerMeter.CreateDetectedPowerMeter(address);
            }
            catch
            {
                throw;
            }
            if (APowerMeter == null)
            {
                return "无法识别对应的功率计";
            }
            return PowerMeter.m_validateSupportDelegate(APowerMeter);
        }

        public static PowerMeter CreateDetectedPowerMeter(string address)
        {
            PowerMeter APowerMeter;
            try
            {
                string Model = ScpiInstrument.DetermineModel(address);

                if (
                    (Model.IndexOf("E441") < 0) &&
                    (Model.IndexOf("437B") < 0) &&
                    (Model.IndexOf("N191") < 0) &&
                    (Model.IndexOf("ML2488B") < 0)
                    )
                {
                    throw new Exception(string.Concat(Model, " 无法识别对应的功率计,IP地址：" + address));
                }

                if (Model.IndexOf("E441") >= 0)
                {
                    APowerMeter = new AgilentEmp(address);
                }
                else if (Model.IndexOf("437B") >= 0)
                {
                    APowerMeter = new AgilentEmp(address);
                }
                else if (Model.IndexOf("N191") >= 0)
                {
                    APowerMeter = new AgilentN1912A(address);
                }
                else if (Model.IndexOf("ML2488B") >= 0)
                {
                    APowerMeter = new AnritsuML2496(address);
                }
                else
                {
                    APowerMeter = null;
                }

            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("连接功率计错误: ", exception.Message));
            }
            return APowerMeter;
        }



        public virtual void SetTraceState(int ChannelNumber, bool TraceState)
        { }

        public virtual void SetInitiateContinuousState(int ChannelNumber, bool State)
        { }

        public virtual void GetTraceDataByChannel(int ChannelNumber, out double[] TraceData)
        { TraceData = new double[0]; }

        public enum TraceDataResolution
        {
            HRESolution,
            MRESolution,
            LRESolution
        }

        public virtual void GetTraceData(int ChannelNumber, TraceDataResolution Resolution, out double[] TraceData)
        { TraceData = new double[0]; }

        public virtual void MReceiverSystemConfigure()
        {
        }

        public virtual void OffsetSwitch(int GateCode, bool IsOffsetOn)
        {
        }

        public virtual void Reset()
        {
            base.Send("*RST");
        }

        public enum PresetTypes
        {
            DEFault,
            GSM900,
            EDGE,
            NADC,
            BLUetooth,
            CDMAone,
            WCDMA,
            CDMA2000,
            IDEN,
            MCPa,
            RADar,
            WL802DOT11A,
            WL802DOT11B,
            XEVDO,
            XEVDV,
            TDSCdma,
            DVB,
            HIPERLAN2,
            WIMAX,
            HSDPA,
            DME,
            DMEPRT,
            LTE
        }

        /// <summary>
        /// 预置
        /// </summary>
        public virtual void Preset(PresetTypes PType)
        { }

        public virtual double ReturnMeasValue(int ChannelNumber)
        {
            return 0;
        }


        public virtual double ReturnPulsePeriod(int TraceNum, int PulseNum)
        { return 0; }

        public virtual double ReturnRiseEdge(int TraceNum, int PulseNum)
        { return 0; }

        public virtual double ReturnFallEdge(int TraceNum, int PulseNum)
        { return 0; }

        public virtual void SetAutoGateRef(int ChannelNum, int GateNum, int RefNum, double RefPercentage)
        { }

        public virtual void SetAcquisitionMode(int TunnelCode, bool IsContinuousTrigOn)
        {
        }

        public virtual void SetDispType(int WindowCode, PowerMeter.DispType DisplayTypeCode)
        {
        }

        public virtual void SetMeasAverageState(int SensorCode, bool AverageState)
        {
        }

        public virtual void SetMeasAutoAverageMode(int SensorCode, bool AverageAutoState)
        {
        }

        public virtual void SetMeasAverageFactor(int SensorCode, int AvgFactor)
        {
        }

        public virtual void SetVideoAverageState(int Sensor, bool AutoState)
        { }

        public virtual void SetVideoAverageFactor(int Sensor, int AvgFactor)
        { }

        public enum VideoBandWidth
        {
            HIGH,
            MEDium,
            LOW,
            OFF,
        }

        public virtual void SetVideoBW(int Sensor, VideoBandWidth VBW)
        { }

        public virtual void SetFrequence(int GateCode, double FreqValue)
        {
        }

        public virtual void SetGate(int TunnelCode, int GateCode, double GateStartTime, double GateTimeLength)
        {
        }

        public virtual void SetMeasType(int TunnelCode, int GateCode, PowerMeter.MeasType MeasurementTypeCode)
        {
        }

        public virtual void SetOffset(int GateCode, double OffsetValue)
        {
        }

        public virtual void SetScreenFormat(PowerMeter.ScreenFormat ScrnFmt)
        {
        }

        public virtual void SetTraceX(int TunnelCode, double XTimeStart, double XTimeLength)
        {
        }

        public virtual void SetScale(int TunnelCode, double Scale)
        { }

        public virtual void SetTrigEdge(PowerMeter.TrigEdge EdgeCode)
        {
        }

        public virtual void SetTrigSource(int TunnelCode, PowerMeter.TrigSourMode TrigSourCode)
        { }

        public virtual void SetTrigAutoLvlState(bool isAutoLvl)
        { }

        public virtual void ReturnSensorInfo(int TunnelCode, out SensorInfo m_SensorInfo)
        { m_SensorInfo = new SensorInfo(); }

        public virtual Image CaptureScreenImage()
        { return null; }
        public virtual void GetTraceX(int TunnelCode, out double XTimeStart, out double XTimeLength)
        {
            XTimeStart = double.NaN;
            XTimeLength = double.NaN;
        }
        public virtual double GetCWAvgValue(int ChannelNumber)
        {
            return 0;
        }
        public virtual double MeasurePower(double m_Frequency, int ChannelNumber)
        {
            return 0;
        }
        public virtual void SetChannelUnits(int ChannelNumber, Units units)
        {
        }
        public virtual double MeasurePower(int ChannelNumber)
        {
            return 0;
        }
        public struct SensorInfo
        {
            public string m_Type;//功率传感器型号
            public string m_SN;//功率传感器序列号
            public string m_PowerRangeMax;//功率传感器功率范围最大值
            public string m_PowerRangeMin;//功率传感器功率范围最小值
            public string m_FreqRangeMax;//功率传感器频率范围最大值
            public string m_FreqRangeMin;//功率传感器频率范围最小值
        }

        /// <summary>
        /// 获取探头类型标示；请重写该方法时，手动标识探头标示。
        /// </summary>
        public virtual string[] SensorType
        {
            get
            {
                string[] tmp = new string[2];
                return tmp;
            }
        }

        public virtual int ChnsCount
        {
            get
            {
                return 0;
            }
        }

        public enum DispType
        {
            DIGital,
            ANALog,
            SNUMeric,
            DNUMeric,
            TRACe,
            CTRAce,
            CTABle
        }

        public enum MeasType
        {
            PEAK,
            PTAV,
            AVER,
            MIN
        }
        public enum Units 
        {
            W,
            dBm
        }

        /// <summary>
        /// 显示格式
        /// </summary>
        public enum ScreenFormat
        {
            /// <summary>
            /// 双窗口显示
            /// </summary>
            WINDowed,
            /// <summary>
            /// 单窗口扩展显示
            /// </summary>
            EXPanded,
            /// <summary>
            /// 全屏显示
            /// </summary>
            FSCReen
        }

        public enum TrigEdge
        {
            POSitive,
            NEGative
        }

        public enum TrigSourMode
        {
            BUS,
            EXTernal,
            HOLD,
            IMMediate,
            INTernal1,
            INTernal2
        }

        public delegate string ValidateSupportDelegate(PowerMeter inPowerMeter);

        /// <summary>
        /// 功率计参数设置写入
        /// </summary>
        /// <param name="PMSetting"></param>
        /// <returns></returns>
        public virtual bool PowerMeterConfigure(PowerMeterSetting PMSetting)
        {
            PMSetting = new PowerMeterSetting();
            return false;
        }

        /// <summary>
        /// 功率计参数设置读取
        /// </summary>
        /// <param name="PMSetting"></param>
        /// <param name="PMSettingResult"></param>
        /// <returns></returns>
        public virtual bool ReadPowerMeterConfiguration(out PowerMeterSetting PMSettingResult)
        {
            PMSettingResult = new PowerMeterSetting();
            return false;


        }


    }

}
