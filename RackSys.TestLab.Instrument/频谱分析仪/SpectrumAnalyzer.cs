using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Windows;

namespace RackSys.TestLab.Instrument
{
    public abstract class SpectrumAnalyzer : ScpiInstrument
    {
        private static SpectrumAnalyzer.ValidateSupportDelegate m_validateSupportDelegate;

      //  public abstract void aaa();

        /// <summary>
        /// 将平均类型改成功率平均类型
        /// </summary>
        public abstract void SetPowerAvgType();

        //public virtual void SetTraceMode(int TraceNumber, SpectrumAnalyzer.TraceMode ModeOfTrace) { };
        /// <summary>
        /// 输入衰减
        /// </summary>
        public abstract double Attenuation
        {
            get;
            set;
        }

        /// <summary>
        /// 噪声系数模式下，读取噪声系数的所有值，返回double数组
        /// </summary>
        /// <param name="noisefigure"></param>
        public virtual void ReadNFTraceData(out double[] noisefigure) { noisefigure = new double[0]; }

        public virtual void Set_RefLvlOffsetByWindow(int WindowNumber, double OffsetValue) { }
        /// <summary>
        /// 噪声系数模式下，读取增益的所有值，返回double数组
        /// 
        /// </summary>
        /// <param name="gain"></param>
        public virtual void ReadGainTraceData(out double[] gain) { gain = new double[0]; }
        public virtual void Set_CorrectionState(int DataCollectionNum, bool State) { }
        public abstract  double[] TraceXaxis
        { get; }
        /// <summary>
        /// 输入起始截止频率，回读trace的横坐标和纵坐标的值
        /// </summary>
        /// <param name="StartFreq"></param>
        /// <param name="StopFreq"></param>
        /// <param name="XAxis"></param>
        /// <param name="YAxis"></param>
        //public abstract void ReadTraceXAndYValue(double StartFreq, double StopFreq, out double[] XAxis, out double[] YAxis);
        /// <summary>
        /// 打开AutoAlign
        /// </summary>
        public abstract bool AutoAlignEnabled
        {
            get;
            set;
        }
        /// <summary>
        /// 频谱仪参数设置
        /// </summary>
        /// <param name="SASetting"></param>
        /// <returns></returns>
        public abstract bool SpectrumAnalyzerConfigure(SpecTrumAnalyzerSetting SASetting);
        /// <summary>
        /// 频谱仪参数读取
        /// </summary>
        /// <param name="isNFMode"></param>
        /// <param name="SASettingResult"></param>
        /// <returns></returns>
        public abstract bool SpectrumAnalyzerConfigurationRead(bool isNFMode,  SpecTrumAnalyzerSetting SASettingResult);
        public abstract void AllMarkerOFF();
        public abstract  string CorrectionTable1
        {
            get;
            set;
        }
        public abstract  double AMP_Offset
        {
            get;
            set;
        }
        public abstract double Inter_Attenuation
        {
            get;

            set;

        }



        /// <summary>
        /// 执行平均测量
        /// </summary>
        public abstract uint Averages
        {
            get;
            set;
        }
        public virtual NoiseFigureENRMode NFENRMode
        {
            get;
            set;
        }

        /// <summary>
        /// 设置和获取中心频率
        /// </summary>
        public abstract double CenterFrequency
        {
            get;
            set;
        }

        /// <summary>
        /// 连续扫频？
        /// </summary>
        public abstract bool ContinuousSweepEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// 每格的dB数值
        /// </summary>
        public abstract double dBperDiv
        {
            get;
            set;
        }
       
        
        
        /// <summary>
        /// 是否启用频谱仪显示
        /// </summary>
        public abstract bool DisplayEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// 最高频率
        /// </summary>
        public abstract double FrequencyMax
        {
            get;
        }

        /// <summary>
        /// 输入过载
        /// </summary>
        public abstract bool InputOverload
        {
            get;
        }

        #region Marker功能部分
        /// <summary>
        /// Marker功能类型
        /// </summary>
        public abstract SpectrumAnalyzer.MarkerFunctionType Marker1Function
        {
            get;
            set;
        }
        public abstract MarkerFunctionType GetMakerFunctionByIndex(int MarkerIndex);
        public abstract void SetMakerFunctionByIndex(MarkerFunctionType inMarkerType, int MarkerIndex);
        public abstract void SetMarkerModeByIndex(MarkerModeType inMarkerMode, int MarkerIndex);
        public abstract MarkerModeType GetMarkerModeByIndex(int MarkerIndex);
        public abstract void SetMarkerPositionByIndex(double inMarkerPosition, int MarkerIndex);
        public abstract double GetMarkerPositionByIndex(int MarkerIndex);

        public abstract double GetMarkerFCountByIndex(int MarkerIndex);//使用count功能读取频率

        public abstract double GetMarkerValueByIndex(int MarkerIndex);
        public abstract double GetMarkerFuntionBPOwerValueByIndex(int MarkerIndex);
        public abstract double GetMarkerFuntionNoiseValueByIndex(int MarkerIndex);

        public abstract void SetDisPlayFormat(DisPlayModeType value);
        public abstract double GetMarkerNFValueByIndex(int MarkerIndex);
        public abstract double GetMarkerGainValueByIndex(int MarkerIndex);
        public abstract void MarkerToCenterFreq(int markerIndex);
        

        /// <summary>
        /// Marker模式类型
        /// </summary>
        public abstract SpectrumAnalyzer.MarkerModeType Marker1Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Marker位置
        /// 暂时仅仅支持了Marker1的操作，其他Marker的操作尚未支持
        /// </summary>
        public abstract double Marker1Position
        {
            get;
            set;
        }

        public abstract double MarkerPositionPoints
        {
            get;
            set;
        }

        public abstract double MarkerSpanPoints
        {
            get;
            set;
        }

        public abstract double Marker1Value
        {
            get;
        }
        
        #endregion
        public abstract bool NeedsAlignment
        {
            get;
        }

        public abstract double RBW
        {
            get;
            set;
        }

        public abstract double ReferenceLevel
        {
            get;
            set;
        }
   
        public abstract bool RFExternalReference
        {
            get;
        }

        public abstract double Span
        {
            get;
            set;
        }

        public abstract int SweepPoints
        {
            get;
            set;
        }

        public abstract double SweepTime
        {
            get;
            set;
        }

        public abstract double[] TraceDataByIndex(int TraceIndex);


        public abstract double VBW
        {
            get;
            set;
        }

        #region 三阶交调部分
        public abstract void TOIMode();

        public abstract double TOI_ReferenceLevel
        {
            get;
            set;
        }
        public abstract double TOI_RBW
        {
            get;
            set;
        }
        public abstract double TOI_Span
        {
            get;
            set;
        }
        public abstract double TOI_VBW
        {
            get;
            set;
        }
        public abstract double TOI_dBperDiv
        {
            get;
            set;
        }

        public abstract TOITestResult TOI_GetTestResult();
        
        #endregion


        static SpectrumAnalyzer()
        {
            SpectrumAnalyzer.m_validateSupportDelegate = null;
        }

        protected SpectrumAnalyzer(string address)
            : base(address)
        {
        }

        public static SpectrumAnalyzer Connect(string currentAddress, SpectrumAnalyzer.ValidateSupportDelegate supportDelegate, bool interactive)
        {
            SpectrumAnalyzer spectrumAnalyzer = null;
            string str = (currentAddress != null ? currentAddress : "GPIB0::18::INSTR");
            SpectrumAnalyzer.m_validateSupportDelegate = supportDelegate;
            if (interactive)
            {
                throw new Exception("不支持交互模式");
            }
            try
            {
                if (SpectrumAnalyzer.DetermineSupport(str) == null)
                {
                    spectrumAnalyzer = SpectrumAnalyzer.CreateDetectedSpectrumAnalyzer(str);
                }
            }
            catch
            {
               // MessageBox.Show("设备连接异常！");
            }
            SpectrumAnalyzer.m_validateSupportDelegate = null;
            if (spectrumAnalyzer != null)
            {
                spectrumAnalyzer.Connected = true;
            }
            return spectrumAnalyzer;
        }

        public static SpectrumAnalyzer CreateDetectedSpectrumAnalyzer(string address)
        {
            SpectrumAnalyzer scpiSpectrumAnalyzer;
            try
            {
                string str = ScpiInstrument.DetermineModel(address);
                if (  SpectrumAnalyzer.IsMXA(str) || SpectrumAnalyzer.IsEXA(str) || SpectrumAnalyzer.IsPXA(str))
                {
                    scpiSpectrumAnalyzer = new ScpiSpectrumAnalyzer(address);
                }
                else if (SpectrumAnalyzer.IsFSW(str))
                {
                    scpiSpectrumAnalyzer = new ScpiSpectrumAnalyzerFSW(address);
                }
                else if (SpectrumAnalyzer.IsESA(str) || SpectrumAnalyzer.IsPSA(str) || SpectrumAnalyzer.IsE4446A(str))
                {
                    scpiSpectrumAnalyzer = new ScpiSpectrumAnalyzerE4446A(address);
                }
                else if (SpectrumAnalyzer.IsAV4003(str))
                {
                    scpiSpectrumAnalyzer = new ScpiSpectrumAnalyzerAV4033(address); 
                }
                else
                {
                    throw new Exception(string.Concat(str, " 不支持对应型号的频谱仪"));
                }
                
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("连接频谱仪错误: ", exception.Message));
            }
            return scpiSpectrumAnalyzer;
        }

        public abstract void DeleteState(string filename);

        protected override void DetermineOptions()
        {
            if (!SpectrumAnalyzer.IsESA(this.Model))
            {
                base.DetermineOptions();
                return;
            }
            this.m_options = "";
        }

        private static string DetermineSupport(string address)
        {
            if (SpectrumAnalyzer.m_validateSupportDelegate == null)
            {
                return null;
            }
            SpectrumAnalyzer spectrumAnalyzer = null;
            try
            {
                spectrumAnalyzer = SpectrumAnalyzer.CreateDetectedSpectrumAnalyzer(address);
            }
            catch
            {
                throw;
            }
            if (spectrumAnalyzer == null)
            {
                return "无法识别对应的频谱仪";
            }
            return SpectrumAnalyzer.m_validateSupportDelegate(spectrumAnalyzer);
        }

        public virtual bool HasOption(string option)
        {
            return this.Options.ToUpper().IndexOf(option.ToUpper()) != -1;
        }

        public static bool IsESA(string model)
        {
            if (model == "E4401B" || model == "E4402B" || model == "E4403B" || model == "E4404B" || model == "E4405B" || model == "E4407B" || model == "E4408B")
            {
                return true;
            }
            return model == "E4411B";
        }

        public virtual bool IsESA()
        {
            return SpectrumAnalyzer.IsESA(this.Model);
        }

        public static bool IsEXA(string model)
        {
            if (model == "N9010A" || model == "N9010B")
            {
                return true;
            }
            return model=="N9010A";
        }

        public virtual bool IsEXA()
        {
            return SpectrumAnalyzer.IsEXA(this.Model);
        }

        public static bool IsAV4003(string model)
        {
            return model == "AV4003";
        }
        /// <summary>
        /// 41所频谱仪（杨飞添加）
        /// </summary>
        /// <returns></returns>
        public virtual bool IsAV4003()
        {
            if (Model.Contains("4003"))
            {
                return true;
            }
            else
                return false;
        }
        //去现场验证返回的型号为多少.

        public bool IsXA()
        {
            return this.IsPXA() || this.IsMXA() || this.IsEXA();
        }
        public virtual double StartFrequency
        {
            get;
            set;
        }

        public virtual double StopFrequency
        {
            get;
            set;
        }


        public static bool IsMXA(string model)
        {
            return model == "N9020A";
        }

        public virtual bool IsMXA()
        {
            return SpectrumAnalyzer.IsMXA(this.Model);
        }
        
        public static bool IsPSA(string model)
        {
            if (model == "N8201A" || model == "E4440A" || model == "E4443A" || model == "E4445A"||model.StartsWith("E444"))
            {
                return true;
            }
            return model == "E4448A";
        }
        public virtual void CreatFile(string Filename)
        {
        }

        public virtual bool IsPSA()
        {
            return SpectrumAnalyzer.IsPSA(this.Model);
        }

        public static bool IsPXA(string model)
        {
            return model == "N9030A";
        }

        public static bool IsFSW(string model)
        {
            return model == "FSW-43"||model =="FSW-26";
        }


        public virtual string strCalENRTable
        {
            get;
            set;
        }
        public static bool IsE4446A(string model)
        {
            return model.Contains("4446");
        }
        public virtual bool IsPXA()
        {
            return SpectrumAnalyzer.IsPXA(this.Model);
        }

        public abstract void LoadState(string filename);

        #region 峰值搜索部分

        /// <summary>
        /// 找到最高峰值
        /// </summary>
        /// <param name="MarkerIndex"></param>
        public abstract void MarkerPeakSearch(int MarkerIndex);

        /// <summary>
        /// 找到下一峰值
        /// </summary>
        /// <param name="MarkerIndex"></param>
        public abstract void MarkerNextPeakSearch(int MarkerIndex);


        /// <summary>
        /// 找到第n高的峰值
        /// </summary>
        /// <param name="num"></param>
        /// <param name="YPeak"></param>
        /// <returns></returns>
        public abstract double PeakSearchByOrder(int num, int YPeak);

        //
        #endregion

        /// <summary>
        /// 是否在屏幕上打开Marker表
        /// </summary>
        public abstract bool MarkerTableState
        {
            get;
            set;
        }

        /// <summary>
        /// 根据精度测量噪低
        /// </summary>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        public double MeasureNoiseFloor(double accuracy)
        {
            accuracy = Math.Max(accuracy, 0.01);
            accuracy = Math.Min(accuracy, 10);
            double num = Math.Pow(10, 0.1 * accuracy);
            double num1 = 1 / (num - 1);
            double num2 = num1 * num1;
            int num3 = Math.Max(65, (int)(num2 / 10));
            double num4 = 100;
            double num5 = 1 / (6.28318530717959 * num4);
            double sweepTime = this.SweepTime;
            double num6 = 3 * num5 * (double)num3;
            this.RBW = 1000;
            this.VBW = num4;
            this.SweepPoints = num3;
            this.SweepTime = num6;
            this.Marker1Function = SpectrumAnalyzer.MarkerFunctionType.BandIntervalPower;
            if (SpectrumAnalyzer.IsPSA(this.Model) || SpectrumAnalyzer.IsESA(this.Model))
            {
                this.Marker1Mode = SpectrumAnalyzer.MarkerModeType.Span;
            }
            else
            {
                this.Sweep();
            }
            this.MarkerPositionPoints = (double)((num3 - 1) / 2);
            this.MarkerSpanPoints = (double)(num3 - 4);
            this.Sweep();
            this.SweepTime = sweepTime;
            return this.Marker1Value - 30;
        }

        public double MeasureTraceStdev()
        {
            double vBW = 1 / (6.28318530717959 * this.VBW);
            int num = 1000;
            double num1 = 3 * vBW * (double)num;
            if (num1 > 10)
            {
                num = (int)Math.Round(10 / (3 * vBW));
                if (num < 9)
                {
                    num = 9;
                }
                num1 = 3 * vBW * (double)num;
            }
            double sweepTime = this.SweepTime;
            this.SweepTime = num1;
            this.Sweep();
            double[] traceData = this.TraceDataByIndex(1);
            double length = 0;
            int num2 = 0;
            while (num2 < (int)traceData.Length)
            {
                int num3 = num2;
                num2 = num3 + 1;
                length = length + traceData[num3];
            }
            length = length / (double)((int)traceData.Length);
            double length1 = 0;
            for (int i = 0; i < (int)traceData.Length; i++)
            {
                double num4 = traceData[i] - length;
                length1 = length1 + num4 * num4;
            }
            length1 = length1 / (double)((int)traceData.Length);
            return Math.Sqrt(length1);
        }

        public abstract void Preset();
        //郝佳添加代码2013.11.25
        public abstract void ModePreset();
        public abstract void MarkerToRefLev(int MarkerNum);
        public abstract void BandIntervalPowerMarkerSpan(uint MarkerNumber, double Span);
        public abstract void NoiseFigureLossCompModeBeforeDUT(SpectrumAnalyzer.NoiseFigureLossCompMode Mode);
        public abstract void ClearLossCompTableBeforeDUT();
        public abstract void SaveLossCompTableBeforeDUT(string FileAddressAndName);
        public abstract void SetLossCompTableBeforeDUT(string FreqAndLossList);
        public abstract void NoiseFigureLossCompModeAfterDUT(SpectrumAnalyzer.NoiseFigureLossCompMode Mode);
        public abstract void ClearLossCompTableAfterDUT();
        public abstract void SetLossCompTableAfterDUT(string FreqAndLossList);
        public abstract void SelectDisplayWindow(uint WindowNumber);
        public abstract void SetNFWindowAutoScale(uint WindowNumber, bool value);
        public abstract void SetWindowZoom(bool value);
        /// <summary>
        /// 根据路径，获取所有文件
        /// </summary>
        /// <param name="DefaultDir"></param>
        /// <param name="calsetResult"></param>
        public abstract void GetAllsCalsetNames(string DefaultDir, out List<string> calsetResult);

        #region 噪声系数
        public abstract double NF_RBW
        {
            get;
            set;
        }

        #endregion







        /// <summary>
        /// ENR模式（噪声系数模式）
        /// </summary>
        public enum NoiseFigureENRMode
        {
            /// <summary>
            /// 列表模式
            /// </summary>
            TABLe = 0,
            /// <summary>
            /// 散粒模式
            /// </summary>
            SPOT
        }




        public double RequiredNoiseFloor(double signalLevel, double additiveNoiseError)
        {
            double num = Math.Pow(10, 0.1 * (signalLevel - 30));
            double num1 = Math.Pow(10, 0.1 * Math.Abs(additiveNoiseError));
            double num2 = num * (num1 - 1);
            return 10 * Math.Log10(num2) + 30;
        }

        public double RequiredRbw(double referenceNoiseFloor, double desiredNoiseFloor)
        {
            return Math.Pow(10, 0.1 * (desiredNoiseFloor - referenceNoiseFloor));
        }

        /// <summary>
        /// 该函数通过不断调节参考电平，期望测得准确的数值
        /// 改进部分：可以用0Span的方式加快测量速度
        /// </summary>
        /// <param name="inFreq"></param>
        /// <returns></returns>
        public virtual double MeaureAmplitudeByAdjustRefLevel(double inFreq)
        {
            this.CenterFrequency = inFreq;
            //this.Span = 0; 尚未验证
            this.Sweep();
            this.Marker1Position = inFreq;
            while (true)
            {
                this.Sweep();
                double Amplitude = this.Marker1Value;
                double RefLevelNow = this.ReferenceLevel;
                if (Amplitude < (RefLevelNow - 2.0))
                {
                    RefLevelNow = Amplitude + 1.5f;
                    //RefLevelNow--;
                    this.ReferenceLevel = RefLevelNow;
                }
                else if (Amplitude > (RefLevelNow - 1.0))
                {
                    RefLevelNow = Amplitude + 1.5f;
                    //RefLevelNow++;
                    this.ReferenceLevel = RefLevelNow;
                }
                else
                {
                    return Amplitude;
                }
            }
        }

        public int RequiredTraceLength(double desiredStdev)
        {
            double num = this.MeasureTraceStdev();
            double num1 = Math.Pow(num / desiredStdev, 2);
            num1 = Math.Max(4, num1);
            if (num1 > 2147483647)
            {
                throw new ArgumentOutOfRangeException("Cannot achieve desired level of standard deviation");
            }
            return (int)Math.Round(num1);
        }

        public double RequiredZeroSpanSweepTime(double minSweepTime)
        {
            double vBW = 1 / (6.28318530717959 * this.VBW);
            double sweepPoints = (double)this.SweepPoints * 3 * vBW;
            return Math.Min(1000, Math.Max(minSweepTime, sweepPoints));
        }

        public abstract void SAMode();
        public abstract void NoiseFigureMode();//郝佳添加驱动 2013.12.2
        public abstract void NoiseFigureCalNow(string ENRTable);//郝佳添加驱动 2013.12.2  
        public abstract void NoiseFigureMixerModeSetup(double fixLOFreq, bool isHighConverter);
        public abstract void NFCALStatON();
        public abstract string SaveStateToPath(string filename); //苏渊红2014.3.3添加
        public abstract void LoadStateFromPath(string filename); //苏渊红2014.3.4添加
        /// <summary>
        /// 使用频谱仪指定的状态文件
        /// </summary>
        /// <returns></returns>
        public virtual bool SpectrumAnalyzerUseSelectedState(bool isNFMode, string stateFilepPathAndName, SpecTrumAnalyzerSetting SASetting)
        {
            SASetting = new SpecTrumAnalyzerSetting();
            return false;
        }
        /// <summary>
        /// 使用频谱仪当前的状态文件
        /// </summary>
        /// <returns></returns>
        public virtual bool SpectrumAnalyzerUseCurrentState(bool isNFMode, out SpecTrumAnalyzerSetting SASetting)
        {
            SASetting = new SpecTrumAnalyzerSetting();
            return false;
        }
        //public abstract string ReadENRMeasTable()        //苏渊红2014.3.5添加读取ENRTable的值底层函数。
        //{
        //    return "error";
        //}
       

        public abstract void SaveState(string filename);

        /// <summary>
        /// 采集当前屏幕，并传回计算机
        /// </summary>
        /// <param name="strDIR"></param>
        /// <returns></returns>
        public abstract Image CaptureScreenImage();

        public virtual void SetDetectorMode(int TraceNumber, DetectorMode ModeOfDetector)
        { }
        public abstract void Sweep(int timeMS=5000);


        public enum MarkerFunctionType
        {
            
            BandIntervalDensity,
            BandIntervalPower,
            Noise,
            None
        }

       
        public enum MarkerModeType
        {
            Position,
            Delta,
            Band,
            Span,
            Off
        }

        public enum NoiseFigureLossCompMode
        {
            OFF,
            FIX,
            TABL
        }
        public enum DetectorMode
        {
            NORM,//Normal
            AVER,//Average / RMS
            POS,//Positive peak
            SAMP,//Sample
            NEG,//Negative peak
            QPE,//Quasi Peak
            EAV,//EMI Average
            RAV,//RMS Average
        }
        public enum DisPlayModeType
        {
            GRAPh,
            TABLe,
            METer
        }
        public delegate string ValidateSupportDelegate(SpectrumAnalyzer sa);


        //杨飞添加驱动 2016.01.07
        #region  迹线状态和触发模式

        /// <summary>
        /// 迹线状态
        /// </summary>
        public enum TraceState
        {
            //[Description("Maxhold")]
            Maxhold,
            Minhold,
            Write,
            Average,
            View,
            Blank
        }
        //        public enum TraceMode
        //{
        //    WRITe,// Clear Write
        //    AVERage,//Trace Average
        //    MAXHold,// Maximum Hold
        //    MINHold // Minimum Hold
        //}
        public virtual void SetBoontonPowerMeter(int TraceIndex)
        { }

        /// <summary>
        /// 设置迹线状态
        /// </summary>
        /// <param name="inState">迹线状态</param>
        /// <param name="TraceIndex">迹线标号</param>
        public abstract void SetTraceStateByIndex(TraceState inState, int TraceIndex);

        /// <summary>
        /// 获取迹线状态
        /// </summary>
        /// <param name="TraceIndex">迹线标号</param>
        /// <returns></returns>
        public abstract TraceState GetTraceStateByIndex(int TraceIndex);

        /// <summary>
        /// 获取Marker状态
        /// </summary>
        /// <param name="MarkerIndex">Marker序号</param>
        /// <returns>Marker开返回true，关返回false</returns>
        public virtual bool GetMarkerStateByIndex(int MarkerIndex)
        { return true;}

        /// <summary>
        /// 设置Marker开关
        /// </summary>
        /// <param name="MarkerIndex">Marker序号</param>
        /// <param name="value">ON或者OFF</param>
        public virtual void SetMarkerStateByIndex(int MarkerIndex,bool value)
        { }

        /// <summary>
        /// 获取Marker所在迹线序号
        /// </summary>
        /// <param name="MarkerIndex">Marker序号</param>
        /// <returns>迹线序号</returns>
        public abstract int GetTraceNumberByIndex(int MarkerIndex);

        /// <summary>
        /// 设置Marker所在迹线
        /// </summary>
        /// <param name="MarkerIndex">Marker</param>
        /// <param name="TraceIndex">迹线序号</param>
        public virtual void SetMarkerToTraceIndex(int MarkerIndex, int TraceIndex)
        { }


        /// <summary>
        /// 触发状态
        /// </summary>
        public enum TriggerMode
        {
            Immediate,
            Video,
            Ext1,
            Ext2,
            IFPower,
            RFPower,
            BBPower,
            Frame,
            Line,
            RFBurst,
            Extern,
            Psen
        }

        /// <summary>
        /// 频谱仪自校准方式
        /// </summary>
        public enum AlignmentsNow
        {
            All,
            AllbutRF,
            RF,
            ExternalMixer
        }

        public abstract TriggerMode TriggerModeState
        {
            get;
            set;
        }

        public virtual double TriggerDelay
        {
            get
            {
                return this.QueryNumber("TRIG:DEL?");
            }
            set
            {
                if (value == 0)
                {
                    this.Send("TRIG:DEL:STAT OFF");
                }
                else
                {
                    this.Send("TRIG:DEL:STAT ON");
                    this.Send(string.Format("TRIG:DEL {0}",value));
                }
            }
        }

        /// <summary>
        /// 查询迹线是否启用
        /// </summary>
        /// <param name="TraceIndex">迹线标号</param>
        /// <returns>启用返回true，未启用返回false</returns>
        public virtual bool EnableTraceState(int TraceIndex)
        {
            if (base.Query(":DISPlay:TRACe" + TraceIndex.ToString() + "?").Contains("1")||base.Query(":DISPlay:TRACe" + TraceIndex.ToString() + "?").ToUpper().StartsWith("ON"))
            { return true; }
            else { return false; }
        }

        /// <summary>
        /// 频谱仪自校准
        /// </summary>
        /// <param name="AlignMeasure">频谱仪自校准方式</param>
        public virtual void DoAlignNow(AlignmentsNow AlignMeasure)
        { }

        /// <summary>
        /// 获取频谱仪自校准方式
        /// </summary>
        public virtual AlignmentsNow GetAlignNow
        {
            get
            {
                return AlignmentsNow.All;
            }
            //set;
        }

        /// <summary>
        /// 是否启用预放
        /// </summary>
        public virtual bool PreAmplifier
        {
            get;
            set;
        }

        public virtual bool EnabledDisplayLine
        {
            get;
            set;
        }

        public virtual void SetDisplayLine(string value) { }

        public virtual void DisplayLineValue(double value)
        { }

        //public virtual void MarkerEnabelForE4446()
        //{ }

        #endregion


    }



    public class TOITestResult
    {
        private double m_LowerBaseFreq;

        public double LowerBaseFreq
        {
            get { return m_LowerBaseFreq; }
            set { m_LowerBaseFreq = value; }
        }
        private double m_LowerBasePower;

        public double LowerBasePower
        {
            get { return m_LowerBasePower; }
            set { m_LowerBasePower = value; }
        }
        private double m_UpperBaseFreq;

        public double UpperBaseFreq
        {
            get { return m_UpperBaseFreq; }
            set { m_UpperBaseFreq = value; }
        }
        private double m_UpperBasePower;

        public double UpperBasePower
        {
            get { return m_UpperBasePower; }
            set { m_UpperBasePower = value; }
        }

        private double m_Lower_InterModulate_Freq;

        public double Lower_InterModulate_Freq
        {
            get { return m_Lower_InterModulate_Freq; }
            set { m_Lower_InterModulate_Freq = value; }
        }
        private double m_Lower_InterModulate_Power;

        public double Lower_InterModulate_Power
        {
            get { return m_Lower_InterModulate_Power; }
            set { m_Lower_InterModulate_Power = value; }
        }

        private double m_Upper_InterModulate_Freq;

        public double Upper_InterModulate_Freq
        {
            get { return m_Upper_InterModulate_Freq; }
            set { m_Upper_InterModulate_Freq = value; }
        }
        private double m_Upper_IntreModulate_Power;

        public double Upper_IntreModulate_Power
        {
            get { return m_Upper_IntreModulate_Power; }
            set { m_Upper_IntreModulate_Power = value; }
        }
        
        


    }
        

   /// <summary>
    /// 频谱仪参数结构类型
   /// </summary>
    public class SpecTrumAnalyzerSetting
    {
        private bool m_NFStateEnable = false;

        /// <summary>
        /// true为NF模式，false为扫频模式
        /// </summary>
        public bool NFStateEnable
        {
            get { return m_NFStateEnable; }
            set { m_NFStateEnable = value; }
        }
        private bool m_SingleSweep = false;

        /// <summary>
        /// true为单次扫描，false为连续扫描
        /// </summary>
        public bool SingleSweep
        {
            get { return m_SingleSweep; }
            set { m_SingleSweep = value; }
        }



        private double m_FreqStart = 1e9;

        /// <summary>
        /// 起始频率，以Hz为单位
        /// </summary>
        public double FreqStart
        {
            get { return m_FreqStart; }
            set { m_FreqStart = value; }
        }
        private double m_FreqStop = 2e9;
        /// <summary>
        /// 终止频率，以hz为单位
        /// </summary>
        public double FreqStop
        {
            get { return m_FreqStop; }
            set { m_FreqStop = value; }
        }
        private double m_RBW = 1e6;
        /// <summary>
        ///RBW设置，扫频模式为RBW，NF模式为分析贷款的值
        /// </summary>
        public double RBW
        {
            get { return m_RBW; }
            set { m_RBW = value; }
        }
        private double m_ReferenceLevel = 0;

        /// <summary>
        /// 参考电平，扫频模式有效
        /// </summary>
        public double ReferenceLevel
        {
            get { return m_ReferenceLevel; }
            set { m_ReferenceLevel = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        private double m_InterAttenuation = 10;

        public double InterAttenuation
        {
            get { return m_InterAttenuation; }
            set { m_InterAttenuation = value; }
        }
        /// <summary>
        /// 0为auto，大于0为自定义
        /// </summary>
        private double m_Yscale;

        public double Yscale
        {
            get { return m_Yscale; }
            set { m_Yscale = value; }
        }
        private double m_Offset = 0;
        /// <summary>
        /// Y轴偏置
        /// </summary>
        public double Offset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }
        private uint m_AverageNumber = 1;
        /// <summary>
        /// 读数平均次数
        /// </summary>
        public uint AverageNumber
        {
            get { return m_AverageNumber; }
            set { m_AverageNumber = value; }
        }


        private string m_NFStateFilePathAndName = string.Empty;
        /// <summary>
        /// 噪声系数状态文件路径及名称
        /// </summary>
        public string NFStateFilePathAndName
        {
            get { return m_NFStateFilePathAndName; }
            set { m_NFStateFilePathAndName = value; }
        }
   
        private uint m_SweepPoints = 11;
        public uint SweepPoints
        {
            get { return m_SweepPoints; }
            set { m_SweepPoints = value; }
        }
        



    }
    
}
