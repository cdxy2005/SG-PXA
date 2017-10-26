/* =============================================
 * Copyright @ 2013 北京中创锐科技术有限公司 
 * 名    称：NoiseFigureAnalyzer
 * 功    能：噪声系数分析仪基类
 * 作    者：Chen xf Administrator
 * 添加时间：2014/3/18 9:58:38
 * =============================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RackSys.TestLab.Instrument
{
    public abstract class NoiseFigureAnalyzer: ScpiInstrument
    {

        public NoiseFigureAnalyzer(string in_InstAddr)
            : base(in_InstAddr)
        {
        }

        /// <summary>
        /// 显示的格式，分为图形、列表和表盘
        /// </summary>
        public enum FormatOfDisplay
        {
            GRAPh,
            TABLe,
            METer
        }

        /// <summary>
        /// DUT的类型，分为放大器、下变频器件和上变频器件
        /// </summary>
        public enum TypeOfDUT
        {
            AMPLifier,
            DOWNconv,
            UPConv
        }


        public enum TypeOfNoiseSource
        {
            NORMal,
            SNS
        }

        public enum ModeOfFrequency
        {
            SWEep,
            FIXed,
            LIST
        }

        public enum ModeOfAverage
        {
            POINt,
            SWEep
        }

        public enum ModeOfLossCompensation
        {
            FIXed,
            TABLe
        }

        public enum TypeOfScreenImage
        {
            NORMal,
            REVerse
        }

        public enum StateOfWindowZoom
        {
            OFF,
            UPPer,
            LOWer
        }

        public enum TypeOfResults
        {
            NFIGure,
            GAIN,
            YFACtor,
            TEFFective,
            PHOT,
            PCOLd
        }

        public enum TypeOfTrace
        {
            NFIGure,
            GAIN,
            YFACtor,
            TEFFective,
            PHOT,
            PCOLd
        }

        public abstract Image CaptureScreenImage();

        public static NoiseFigureAnalyzer Connect(string currentAddress, NoiseFigureAnalyzer.ValidateSupportDelegate supportDelegate, bool interactive)
        {
            NoiseFigureAnalyzer ANoiseFigureAnalyzer = null;
            string str = (currentAddress != null ? currentAddress : "GPIB0::18::INSTR");
            NoiseFigureAnalyzer.m_validateSupportDelegate = supportDelegate;
            if (interactive)
            {
                throw new Exception("不支持交互模式");
            }
            try
            {
                if (NoiseFigureAnalyzer.DetermineSupport(str) == null)
                {
                    ANoiseFigureAnalyzer = NoiseFigureAnalyzer.CreateDetectedNoiseFigureSupply(str);
                }
            }
            catch
            {
                //throw;
            }
            NoiseFigureAnalyzer.m_validateSupportDelegate = null;
            if (ANoiseFigureAnalyzer != null)
            {
                ANoiseFigureAnalyzer.Connected = true;
            }
            return ANoiseFigureAnalyzer;
        }

        /// <summary>
        /// delegate声明
        /// </summary>
        /// <param name="inDCPower"></param>
        /// <returns></returns>
        public delegate string ValidateSupportDelegate(NoiseFigureAnalyzer inNoiseFigure);

        /// <summary>
        /// delegate保存位置
        /// </summary>
        private static NoiseFigureAnalyzer.ValidateSupportDelegate m_validateSupportDelegate;

        /// <summary>
        /// 判断是否可以支持对应型号的直流电源
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static string DetermineSupport(string address)
        {
            if (NoiseFigureAnalyzer.m_validateSupportDelegate == null)
            {
                return null;
            }
            NoiseFigureAnalyzer ANoiseFigureAnalyzer = null;
            try
            {
                ANoiseFigureAnalyzer = NoiseFigureAnalyzer.CreateDetectedNoiseFigureSupply(address);
            }
            catch
            {
                throw;
            }
            if (ANoiseFigureAnalyzer == null)
            {
                return "不是一个可以识别的噪声系数分析仪";
            }
            return NoiseFigureAnalyzer.m_validateSupportDelegate(ANoiseFigureAnalyzer);
        }

        /// <summary>
        /// 创建对应的噪声分析仪控制对象。
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static NoiseFigureAnalyzer CreateDetectedNoiseFigureSupply(string address)
        {
            NoiseFigureAnalyzer NoiseFigureAnalyzerSupply = null;
            try
            {
                string ModelNo = ScpiInstrument.DetermineModel(address);
                if (!NoiseFigureAnalyzer.IsSupportedModel(ModelNo))
                {
                    throw new Exception(string.Concat(ModelNo, " 不是一个可以支持的噪声系统分析仪"));
                }
                
      
                if (ModelNo.IndexOf("N8975A") >= 0  )
                {
                    NoiseFigureAnalyzerSupply = new AgilentN8975A(address);
                }
                else if (ModelNo.IndexOf("AV3985") >=0)
                {
                    NoiseFigureAnalyzerSupply = new AV3985A(address);
                }
                else
                {
                    throw new Exception(string.Concat(ModelNo, " 不是一个可以支持的噪声系统分析仪"));
 
                }
          
                
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("连接噪声分析仪错误: ", exception.Message));
            }
            return NoiseFigureAnalyzerSupply;
        }

        public static bool IsSupportedModel(string model)
        {
            if (model.IndexOf("N8975A") >=0 || model.IndexOf("AV3985") >= 0 )
            {
                return true;
            }
          
            else
            {
                return false;   
            }
 
        }

        public virtual void Reset()
        { }

        public virtual void Preset()
        { }

        /// <summary>
        /// 显示格式
        /// </summary>
        public virtual FormatOfDisplay DisplayFormat
        {
            get;
            set;
        }

        public virtual bool FullScreenState
        {
            get;
            set;
        }

        public virtual StateOfWindowZoom WinZoomState
        {
            get;
            set;
        }

        public virtual void SetYScalePerDiv(TypeOfResults ResultsType, double ScalePerDiv)
        { }

        public virtual void SetGraphUpperLimit(TypeOfTrace TraceType, double UpperLimit)
        { }

        public virtual void SetGraphLowerLimit(TypeOfTrace TraceType, double LowerLimit)
        { }

        public virtual void SetReferenceLevelValue(TypeOfResults ResultType, double RefLvlValue)
        { }

        public virtual TypeOfDUT DUTType
        {
            get;
            set;
        }

        public virtual TypeOfNoiseSource NoiseSource
        {
            get;
            set;
        }

        public virtual ModeOfFrequency FreqMode
        {
            get;
            set;
        }

        public virtual ModeOfAverage AverageMode
        {
            get;
            set;
        }

        public virtual bool LossCompState_AfterDUT
        {
            get;
            set;
        }

        public virtual bool LossCompState_BeforeDUT
        {
            get;
            set;
        }

        public virtual ModeOfLossCompensation AfterDUTLossCompMode
        {
            get;
            set;
        }

        public virtual ModeOfLossCompensation BeforeDUTLossCompMode
        {
            get;
            set;
        }

        public virtual void Set_AfterDUTLossCompData(double[] LossCompData)
        { }

        public virtual void Set_BeforeDUTLossCompData(double[] LossCompData)
        { }

        public virtual double StartFreq
        {
            get;
            set;
        }

        public virtual double StopFreq
        {
            get;
            set;
        }

        public virtual int SweepPoint
        {
            get;
            set;
        }

        public virtual void Set_FreqList(double[] FreqList)
        { }

        public virtual void Get_FreqList(out double[] FreqList)
        { FreqList = null; }

        public virtual bool AverageState
        {
            get;
            set;
        }

        public virtual uint AverageNumber
        {
            get;
            set;
        }

        public virtual bool AutoLoadENRState
        {
            get;
            set;
        }

        public virtual bool CommonENRTableState
        {
            get;
            set;
        }

        public virtual string CalENRTable
        {
            get;
            set;
        }

        public virtual string MeasENRTable
        {
            get;
            set;
        }

        public virtual string BeforeDUTLossCompTable
        {
            get;
            set;
        }

        public virtual string AfterDUTLossCompTable
        {
            get;
            set;
        }




        public virtual void LoadCalENRTableFromSNS()
        { }

        public virtual void LoadMeasENRTableFromSNS()
        { }

        public virtual void LoadCommonENRTableFromSNS()
        { }

        public virtual bool CorrectedDataDisplayState
        {
            get;
            set;
        }

        public virtual bool ContinuousMeasState
        {
            get;
            set;
        }

        public virtual void InitiateCalibration()
        { }

        public virtual void DeleteFile(string FilePathAndName)
        { }

        public virtual void StoreStateFile(string FilePathAndName)
        { }

        public virtual void LoadStateFile(string FilePathAndName)
        { }

        public virtual void StoreBeforeDUTLossCompData(string FilePathAndName)
        { }

        public virtual void StoreAfterDUTLossCompData(string FilePathAndName)
        { }

        public virtual void LoadBeforeDUTLossCompData(string FilePathAndName)
        { }

        public virtual void LoadAfterDUTLossCompData(string FilePathAndName)
        { }

        /// <summary>
        /// 存储屏幕截图
        /// </summary>
        /// <param name="ScrImgType">屏幕截图类型：正常为黑色背景，反色为白色背景</param>
        /// <param name="FilePathAndName">截图文件存储路径和文件名，只能为.gif格式和.wmf格式</param>
        public virtual void SaveScreenImage(TypeOfScreenImage ScrImgType,string FilePathAndName)
        { }

        public virtual void Measure()
        { }

        public virtual void Get_SweptCorrectedNF(out double[] NF_List)
        { NF_List = null; }

        public virtual void BW(double IFBW)
        { }

        public virtual double BeforeDUTTemperature
        {
            get;
            set;
        }

        public virtual double AfterDUTTemperature
        {
            get;
            set;
        }

    }
}
