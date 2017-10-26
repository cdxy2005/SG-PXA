using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RackSys.TestLab.Instrument
{
    /// <summary>
    /// 2013-11-21：目前看：矢网部分的接口还缺很多；如去嵌入，校准等部分，需要补充；
    /// </summary>
    public abstract class NetworkAnalyzer : ScpiInstrument
    {
        protected NetworkAnalyzer(string inAddress): base(inAddress)
        {
        }
       public virtual bool OutPutState
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public virtual void AutoScale()
        {
        }

        public virtual byte[] GetS2pFileData()
        {
            byte[] aa = new byte[]{};
            return aa;
        }


        public virtual byte[] GetFileData(string fileName)
        {
            byte[] aa = new byte[] { };
            return aa;
        }
        public virtual void Average_ON(int count)
        {
        }

        public virtual void CentFreq(double mhz)
        {
        }
        public virtual bool EcalAutoOrient
        {
            set;
            get;

        }
        public virtual void Div_dB(double range)
        {
        }

        //郝佳添加代码2014.2.27
        public virtual void CreatFile(string Filename)
        {
        }
        //苏渊红2014.3.1添加
        public virtual string GetFileNameinPath(string FilePath)
        {
            return "error";

        }
       

        /// <summary>
        /// 迹线最大化 张学超
        /// </summary>
        /// <param name="m_TraceMax"></param>
        public virtual void TraceMax(bool traceMax)
        { }
        /// <summary>
        /// 清除所有迹线 张学超2015年9月18日09:55:18
        /// </summary>
        public virtual void CLearAllMeasure()
        {

        }
        public virtual void Average_OFF()
        { }

        public virtual double Div_dB()
        {
            return 0;
        }

        public virtual void ECal_S11()
        {
        }

        public virtual void ECALS11(string MeasmentName, bool IsECalKit, int CalKitNum, int docNum, double rfPow, double startFreq, double stopFreq, int SweepPoint, double ifbw)
        {
        }

        public virtual void ECALS11(string MeasmentName, bool IsECalKit, int CalKitNum, string CalKitType, int docNum, double rfPow, double startFreq, double stopFreq, int SweepPoint, double ifbw)
        {
        }

        public virtual void ECALS21(string MeasmentName, bool IsECalKit, int CalKitNum, int docNum, double rfPow, double startFreq, double stopFreq, int SweepPoint, double ifbw)
        {
        }

        public virtual string GetTrace()
        {
            return string.Empty;
        }

        public virtual void IFBW(double khz)
        {
        }

        public virtual void IniTestSta(NetworkAnalyzer.MeasMode mode, NetworkAnalyzer.Format format)
        {

        }
        private static NetworkAnalyzer.ValidateSupportDelegate m_validateSupportDelegate;

        public static NetworkAnalyzer Connect(string currentAddress, NetworkAnalyzer.ValidateSupportDelegate supportDelegate, bool interactive)
        {
            NetworkAnalyzer ANetworkAnalyzer = null;
            string str = (currentAddress != null ? currentAddress : "GPIB0::18::INSTR");
            NetworkAnalyzer.m_validateSupportDelegate = supportDelegate;
            if (interactive)
            {
                throw new Exception("不支持交互模式");
            }
            try
            {
                if (NetworkAnalyzer.DetermineSupport(str) == null)
                {
                    ANetworkAnalyzer = NetworkAnalyzer.CreateDetectedNetworkAnalyzer(str);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            NetworkAnalyzer.m_validateSupportDelegate = null;
            if (ANetworkAnalyzer != null)
            {
                ANetworkAnalyzer.Connected = true;
            }
            return ANetworkAnalyzer;
        }

        private static string DetermineSupport(string address)
        {
            if (NetworkAnalyzer.m_validateSupportDelegate == null)
            {
                return null;
            }
            NetworkAnalyzer spectrumAnalyzer = null;
            try
            {
                spectrumAnalyzer = NetworkAnalyzer.CreateDetectedNetworkAnalyzer(address);
            }
            catch
            {
                throw;
            }
            if (spectrumAnalyzer == null)
            {
                return "无法识别对应的矢网";
            }
            return NetworkAnalyzer.m_validateSupportDelegate(spectrumAnalyzer);
        }

        public static NetworkAnalyzer CreateDetectedNetworkAnalyzer(string address)
        {
            NetworkAnalyzer APna;
            try
            {
                string str = ScpiInstrument.DetermineModel(address);
                if (!NetworkAnalyzer.IsPNA(str))
                {
                    throw new Exception(string.Concat(str, " ，不支持对应型号的网络分析仪"));
                }
                APna = new AgilentPNA(address);
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("连接矢网错误: ", exception.Message));
            }
            return APna;
        }

        private static bool IsPNA(string str)
        {
            return str.IndexOf("N52") >= 0 ;
        }

        public virtual void Interpolate(bool on)
        {
        }

        public virtual void Key(int key)
        {
        }

        public virtual bool MinPowQuery(double Power)
        {
            return false;
        }

        public virtual void Mode_S11()
        {
        }

        public virtual void Mode_S21()
        {
        }

        public virtual void OutBandRestrain(double rfPow, double startFreq, double stopFreq, double ifbw, bool smooth_OnOff, int smooth, int sweepPoint, bool Avg_OnOff, int avgCount, ref string trace, ref double val)
        {
        }

        public virtual void PowOutput(double dbm)
        {
        }
        /// <summary>
        /// 关闭矢网错误报告
        /// </summary>
                public virtual bool ErrorMessageDisplay
        {
                get
            {
                return true;
            }
            set
            {

            }
    }
        public virtual bool RecallFromFile(int num)
        {
            return false;
        }

        public virtual void RecallReg(int num)
        {
        }

        public virtual double Rel_dB()
        {
            return 0;
        }

        public virtual void Reset()
        {
        }

        public virtual double ReturnMarkX()
        {
            return 0;
        }

        public virtual double ReturnMarkY(double MarkX_MHz)
        {
            return 0;
        }

        public virtual double ReturnPeakMax()
        {
            return 0;
        }

        public virtual double ReturnPeakMin()
        {
            return 0;
        }

        public virtual void RippleInBand(double rfPow, double startFreq, double stopFreq, double ifbw, bool smooth_OnOff, int smooth, int sweepPoint, bool Avg_OnOff, int avgCount, ref string trace, ref double val)
        {
        }

        public virtual void Rl_unit(NetworkAnalyzer.REL_UNIT aa)
        {
        }

        public virtual void SaveReg(int num, string name)
        {
        }

        public virtual void SaveToFile(int num)
        {
        }

        public virtual void Smooth_OFF()
        {
        }

        public virtual void Smooth_ON(int count)
        {
        }

        public virtual void SpanFreq(double mhz)
        {
        }

        public virtual void StarFreq(double mhz)
        {
        }

        public virtual void StdStyle_N50()
        {
        }

        public virtual void StopFreq(double mhz)
        {
        }

        public virtual void TestPathLoss(double rfPow, double startFreq, double stopFreq, double ifbw, bool smooth_OnOff, int smooth, int sweepPoint, bool Avg_OnOff, int avgCount, string[] Marks, ref string trace, ref string[] PathLoss)
        {
        }

        public virtual void TracePointNum(int num)
        {
        }

        public virtual void TransTimeDelay(double rfPow, double startFreq, double stopFreq, double ifbw, bool smooth_OnOff, int smooth, int sweepPoint, bool Avg_OnOff, int avgCount, ref string trace, ref double val)
        {
        }

        public virtual void UnitFormat(NetworkAnalyzer.Format format)
        {
        }

        public virtual void VSWR(double rfPow, double startFreq, double stopFreq, double ifbw, bool smooth_OnOff, int smooth, int sweepPoint, bool Avg_OnOff, int avgCount, ref string trace, ref double val)
        {
        }

        public virtual void WaitOPC()
        {
        }

        public virtual void Write_String(string command)
        {
        }

        public virtual void CreatFloder(string FloderPathAndName)
        { }

        public abstract Image CaptureScreenImage(string strDIR);

        public virtual event NetworkAnalyzer.SendNAStatusDelegate SendNAStatus;

        public enum CalKit
        {
            CALK24MM,
            CALK292MM,
            CALK292S,
            CALK32F,
            CALK35MC,
            CALK35MD,
            CALK35ME,
            CALK716,
            CALK7MM,
            CALKN50,
            CALKN75,
            CALKTRLK,
            CALKUSED
        }

        public enum Format
        {
            MLINear,
            MLOGarithmic,
            PHASe,
            IMAGinary,
            REAL,
            POLar,
            SMITh,
            SWR,
            GDELay
        }

        public enum MeasMode
        {
            S21,
            S11,
            S12,
            S22
        }

        public enum REL_UNIT
        {
            LOGM,
            DELA,
            SWR
        }

        public delegate bool SendNAStatusDelegate(int id, string info);

        //public enum SweepType
        //{
        //    LINFREQ,
        //    LOGFREQ,
        //    LISFREQ,
        //    POWS,
        //    CWFREQ
        //}

        public enum TestResult
        {
            CannotSupport,
            Fail,
            Pass
        }

        public enum TriggerType
        {
            HOLD,
            SINGLE,
            NUMG,
            CONT
        }


        /// <summary>
        /// 矢网设置参数结构
        /// </summary>
        public class PNAsetting
        {
            private bool m_ISconverterMode = false;

            public bool ISconverterMode
            {
                get { return m_ISconverterMode; }
                set { m_ISconverterMode = value; }
            }

            public SweepType Type;

            private double m_StartFreq = 1e9;

            public double StartFreq
            {
                get { return m_StartFreq; }
                set { m_StartFreq = value; }
            }


            private double m_StopFreq = 2e9;

            public double StopFreq
            {
                get { return m_StopFreq; }
                set { m_StopFreq = value; }
            }

            private double m_TestPower = -10;

            public double TestPower
            {
                get { return m_TestPower; }
                set { m_TestPower = value; }
            }

            private uint m_SweepPoints = 201;

            public uint SweepPoints
            {
                get { return m_SweepPoints; }
                set { m_SweepPoints = value; }
            }

            private double m_IBW = 10e3;

            public double IBW
            {
                get { return m_IBW; }
                set { m_IBW = value; }
            }

            private uint m_AverageNumber = 1;

            public uint AverageNumber
            {
                get { return m_AverageNumber; }
                set { m_AverageNumber = value; }
            }

            private double m_Yscale = 0;

            public double Yscale
            {
                get { return m_Yscale; }
                set { m_Yscale = value; }
            }

            private double m_StartPower = -30;

            public double StartPower
            {
                get { return m_StartPower; }
                set { m_StartPower = value; }
            }

            private double m_StopPower = -10;

            public double StopPower
            {
                get { return m_StopPower; }
                set { m_StopPower = value; }
            }

            private double m_CWFreq = 1e9;

            public double CWFreq
            {
                get { return m_CWFreq; }
                set { m_CWFreq = value; }
            }

            private string m_PNAStateFilePathAndName = string.Empty;

            public string PNAStateFilePathAndName
            {
                get { return m_PNAStateFilePathAndName; }
                set { m_PNAStateFilePathAndName = value; }
            }

            private double m_FixedLOFreq = 0;
            /// <summary>
            /// 混频器模式，固定本振与FixedOutputFreq互斥
            /// </summary>
            public double FixedLOFreq
            {
                get { return m_FixedLOFreq; }
                set { m_FixedLOFreq = value; }
            }

            private double m_FixedOutputFreq = 0;

            public double FixedOutputFreq
            {
                get { return m_FixedOutputFreq; }
                set { m_FixedOutputFreq = value; }
            }

            private bool m_IsFixedLOMode = true;
            /// <summary>
            /// 固定本振模式和固定输出模式，true表示为固定本振模式
            /// </summary>
            public bool IsFixedLOMode
            {
                get { return m_IsFixedLOMode; }
                set { m_IsFixedLOMode = value; }
            }


            private bool m_IsHighConverter = false;
            /// <summary>
            /// 表示变频方向，true为上变频
            /// </summary>
            public bool IsHighConverter
            {
                get { return m_IsHighConverter; }
                set { m_IsHighConverter = value; }
            }

        }


        ///// <summary>
        ///// 非变频模式的矢网设置参数结构
        ///// </summary>
        //public class PNAsettingNormalMode
        //{
        //    public SweepType Type;

        //    private double m_StartFreq = 1e9;

        //    public double StartFreq
        //    {
        //        get { return m_StartFreq; }
        //        set { m_StartFreq = value; }
        //    }


        //    private double m_StopFreq = 2e9;

        //    public double StopFreq
        //    {
        //        get { return m_StopFreq; }
        //        set { m_StopFreq = value; }
        //    }

        //    private double m_TestPower = -10;

        //    public double TestPower
        //    {
        //        get { return m_TestPower; }
        //        set { m_TestPower = value; }
        //    }

        //    private uint m_SweepPoints = 201;

        //    public uint SweepPoints
        //    {
        //        get { return m_SweepPoints; }
        //        set { m_SweepPoints = value; }
        //    }

        //    private double m_IBW = 10e3;

        //    public double IBW
        //    {
        //        get { return m_IBW; }
        //        set { m_IBW = value; }
        //    }

        //    private uint m_AverageNumber = 1;

        //    public uint AverageNumber
        //    {
        //        get { return m_AverageNumber; }
        //        set { m_AverageNumber = value; }
        //    }

        //    private double m_Yscale = 0;

        //    public double Yscale
        //    {
        //        get { return m_Yscale; }
        //        set { m_Yscale = value; }
        //    }

        //    private double m_StartPower = -30;

        //    public double StartPower
        //    {
        //        get { return m_StartPower; }
        //        set { m_StartPower = value; }
        //    }

        //    private double m_StopPower = -10;

        //    public double StopPower
        //    {
        //        get { return m_StopPower; }
        //        set { m_StopPower = value; }
        //    }

        //    private double m_CWFreq = 1e9;

        //    public double CWFreq
        //    {
        //        get { return m_CWFreq; }
        //        set { m_CWFreq = value; }
        //    }

        //    private string m_PNAStateFilePathAndName = string.Empty;

        //    public string PNAStateFilePathAndName
        //    {
        //        get { return m_PNAStateFilePathAndName; }
        //        set { m_PNAStateFilePathAndName = value; }
        //    }


        //}
        public enum SweepType { FreqSweep, PowerSweep };
        public delegate string ValidateSupportDelegate(NetworkAnalyzer sa);
        public abstract void GetAllsCalsetNames(string DefaultDir, out List<string> calsetResult);
        public abstract bool UseCurrentState(AgilentPNA835x.Application PNA,  out PNAsetting PNAsetting);
        public abstract bool ReadPNAConfiguration(AgilentPNA835x.Application PNA,   PNAsetting PNAsetting);
        public abstract bool PNAConfiguration(AgilentPNA835x.Application PNA, PNAsetting PNAsetting);
        public abstract bool PNACalSetApply(AgilentPNA835x.Application PNA, string FilePathAndName,  out PNAsetting PNAsetting);

    
    }
}





