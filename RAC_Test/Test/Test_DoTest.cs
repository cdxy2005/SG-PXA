
using RackSys.TestLab.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RackSys.TestLab.Instrument;
using System.Threading;
using RackSys.TestLab;
using System.Windows;

namespace RAC_Test.Test
{
    public class Test_DoTest
    {
        public static bool stopRun = false;
        private Test_Result m_CurResult;
        public DUT CurCtrlDut = SystemHardware.Hardware<DUT>.GetElmentByName("RackSys.TestLab.Instrument.DUT1");
       
        /// <summary>
        /// 当前测试结果
        /// </summary>
        public Test_Result CurResult
        {
            get { return m_CurResult; }
            set { m_CurResult = value; }
        }

        public void AutoRun(SystemHardware inSys, Test_Paramter testParam, out TestExecutionResult outTestExecuteResultFlag)
        {
            try
            {
                outTestExecuteResultFlag = TestExecutionResult.Init;
                if (CurCtrlDut == null)
                {
                
                    MessageBox.Show("被测件没有连接！");
                    outTestExecuteResultFlag = TestExecutionResult.Error;
                    return;
                }
                else
                {
                    //需要获取测试项内容
                    //初始化结果
                    this.m_CurResult = new Test_Result(testParam);
                    m_CurResult.TestParam = testParam;
                    m_CurResult.TestResultList = new List<TestResultByFreq>();
                    
                    //inSys.SpectrumAnalyzer.Preset();
                    int freqN = (int)Math.Floor(1 + (testParam.SGAndPxaParamter.StopFreq - testParam.SGAndPxaParamter.StartFreq) / testParam.SGAndPxaParamter.FreqSpac);
                    TestStructParameters inParam = new TestStructParameters();
                    inParam = Get_testParam(testParam);
                    List<TestResultByFreq> resultList = new List<TestResultByFreq>();
                    List<double> freqList = new List<double>();

                    CurCtrlDut.SETChannel(testParam.ChannelNumber);
                    //todo

                    //
                    for (int i = 0; i < freqN; i++)
                    {
                        freqList.Add(testParam.SGAndPxaParamter.StartFreq + i * testParam.SGAndPxaParamter.FreqSpac);
                    }

                    if (testParam.isTxWork)//是否是发射
                    {
                        CurCtrlDut.CtrlTWork();
                        Thread.Sleep((int)(testParam.sleepTime * 1000));
                    }
                    else
                    {
                        CurCtrlDut.CtrlRWork();
                        Thread.Sleep((int)(testParam.sleepTime * 1000));
                    }

                    if (testParam.IsPowerTest || testParam.IsClutterTest)
                    {
                        for (int i = 0; i < freqN; i++)
                        {
                            if (stopRun)
                            {
                                if (MessageBoxResult.Yes == MessageBox.Show("测试强行停止，是否停止？", "提 示", MessageBoxButton.YesNo))
                                {
                                    outTestExecuteResultFlag = TestExecutionResult.Error;
                                    return;
                                }
                            }                            
                            if (i == 0)
                            {
                                inParam.m_FirstTest = true;
                                inParam.m_LastTest = false;
                            }
                            else
                            {
                                inParam.m_FirstTest = false;
                            }
                            if (i == freqN - 1)
                            {
                                inParam.m_LastTest = true;
                            }
                            if (testParam.isTxWork)//是否是发射
                            {
                                inParam.m_SGFreq = testParam.DUTOutFreq;
                                inParam.m_SAFreq = freqList[i];
                              
                                Thread.Sleep((int)(testParam.sleepTime * 1000));
                            }
                            else
                            {
                                inParam.m_SAFreq = testParam.DUTOutFreq;
                                inParam.m_SGFreq = freqList[i];
                            }
                            //设置本振频率
                            CurCtrlDut.CtrlFreq(freqList[i]);
                            Thread.Sleep((int)(testParam.sleepTime * 1000));

                            TestStructResults rest = new TestStructResults();
                            TestResultByFreq oneResult = new TestResultByFreq();
                            inParam.m_SGOffsetValue = this.GetOFFset(inParam.m_SGFreq, testParam.InputFileoffst);
                           double outputloss = this.GetOFFset(inParam.m_SAFreq, testParam.OutputFileoffst);
                             double inputloss= this.GetOFFset(inParam.m_SAFreq, testParam.InputFileoffst);
                            inParam.m_SAGlobalOffset = outputloss - inputloss;


                            this.ExecuteTestSpurious(inSys, inParam, out rest);
                            oneResult.TestFreq = freqList[i];

                            oneResult.MaxClutter = rest.m_NOSourcePower;
                            oneResult.OutPower = rest.m_CarrierPower;
                            oneResult.ClutterRejection = rest.m_SpuriousSuppression;
                            oneResult.Gain = rest.m_CarrierPower - inParam.m_InputPower;
                            resultList.Add(oneResult);
                        }
                    }
                 
                    CurResult.TestResultList = resultList;
                    outTestExecuteResultFlag = TestExecutionResult.Complete;
                }
                outTestExecuteResultFlag = TestExecutionResult.Complete;
            }
            catch (Exception ex)
            {
                outTestExecuteResultFlag = TestExecutionResult.Error;
                throw new Exception(ex.ToString());
            }
            finally
            {
                GC.Collect();

            }
        }


        private TestStructParameters Get_testParam(Test_Paramter testParam)
        {
            TestStructParameters newtestParam = new TestStructParameters();
            newtestParam.m_AcquireTraceData = false;
            newtestParam.m_AutoRBW = false;
            newtestParam.m_AutoScale = true;
            newtestParam.m_AutoSweepTime = true;
            newtestParam.m_AutoVBW = false;
            newtestParam.m_AverageFactor = 1;
            newtestParam.m_CaptureScrSht = false;
            newtestParam.m_ContinueMeasAfterTest = true;
            newtestParam.m_EditFreq = true;
            newtestParam.m_EditPower = false;
            newtestParam.m_EditSAFDOTable = false;
            newtestParam.m_EditSAGOffset = true;
            newtestParam.m_EditScale = true;
            newtestParam.m_EditSGOffset = true;
            newtestParam.m_IgnoreBWidth = testParam.SGAndPxaParamter.IngroeFreq;
            newtestParam.m_InputPower = testParam.SGAndPxaParamter.InPower;
            newtestParam.m_RBW = testParam.SGAndPxaParamter.RBW;
            newtestParam.m_SAFDOTable = testParam.OutputFileoffst;
            newtestParam.m_Scale = 10;

            newtestParam.m_StartFreq = testParam.SGAndPxaParamter.AnaStartFreq;
            newtestParam.m_StopFreq = testParam.SGAndPxaParamter.AnaStopFreq;
            newtestParam.m_SweepPoints = 401;
            newtestParam.m_SweepTime = 0.1;
            //newtestParam.m_ChannelNumber = 1;
            newtestParam.m_UseExtAmplifier = false;
            newtestParam.m_UseExtAttenuator = false;
            newtestParam.m_VBW = testParam.SGAndPxaParamter.VBW;

            return newtestParam;
        }
        private NoiseFigureTest.NoiseFigureTestStructParameter Get_NoiseParam(Test_Paramter testParam)
        {
            NoiseFigureTest.NoiseFigureTestStructParameter Parameter = new NoiseFigureTest.NoiseFigureTestStructParameter();
            Parameter.m_AverageNum = 1;
            //Parameter.m_FreqList=
            Parameter.m_IsReadMaxNoiseFigure = false;
            Parameter.m_NeedContinuousScanning = true;
            Parameter.m_RBW = testParam.SGAndPxaParamter.RBW;
            Parameter.m_RecallStateFileEnable = true;
            Parameter.m_RecallStateFileName = NoiseFigureCal.noiseResFile;
            Parameter.m_StartFerq = testParam.SGAndPxaParamter. StartFreq;
            Parameter.m_StopFerq = testParam.SGAndPxaParamter.StopFreq;
            Parameter.m_SweepPoints = (int)((testParam.SGAndPxaParamter.StopFreq - testParam.SGAndPxaParamter.StartFreq) / testParam.SGAndPxaParamter.FreqSpac) + 1;
            Parameter.Yscale = 1;
            return Parameter;
        }

        private double GetOFFset(double freq, List<offsetFreq> Inputoffst)//todo 需要测试
        {
            if (Inputoffst.Count==0)
            {
                return 0;
            }
            double offset = 0;
            if (Inputoffst[Inputoffst.Count - 1].FreqInMHz <= freq)
            {
                return Inputoffst[Inputoffst.Count - 1].ReceiverOffsetFreqInMHz;
            }
            else if (Inputoffst[0].FreqInMHz >= freq)
            {
                return Inputoffst[0].ReceiverOffsetFreqInMHz;
            }
            else
            {
                for (int i = 0; i < Inputoffst.Count - 1; i++)
                {
                    if (freq >= Inputoffst[i].FreqInMHz && freq < Inputoffst[i + 1].FreqInMHz)
                    {
                        double aa = Inputoffst[i].ReceiverOffsetFreqInMHz + (freq - Inputoffst[i].FreqInMHz) * (Inputoffst[i + 1].ReceiverOffsetFreqInMHz - Inputoffst[i].ReceiverOffsetFreqInMHz) / (Inputoffst[i + 1].FreqInMHz - Inputoffst[i].FreqInMHz);
                        return aa;
                    }
                }
            }

            return offset;
        }


        /// <summary>
        /// 核心测试函数 输入参数结构
        /// </summary>
        public struct TestStructParameters
        {
            /// <summary>
            /// 是否无源测试
            /// </summary>
            public bool m_NoSourceTest;
            /// <summary>
            /// 是否为第一次测试
            /// </summary>
            public bool m_FirstTest;
            /// <summary>
            /// 是否为最后一次测试
            /// </summary>
            public bool m_LastTest;
            /// <summary>
            /// 测试完成后是否连续测量。只能在最后一个通道的最后的一个频点测完后起效。
            /// </summary>
            public bool m_ContinueMeasAfterTest;
            /// <summary>
            /// 是否截屏
            /// </summary>
            public bool m_CaptureScrSht;
            /// <summary>
            /// 是否获取（并输出）迹线数据（用于画图或分析等）
            /// </summary>
            public bool m_AcquireTraceData;

            /// <summary>
            /// 是否使用外部驱放
            /// </summary>
            public bool m_UseExtAmplifier;
            /// <summary>
            /// 是否使用外部衰减器
            /// </summary>
            public bool m_UseExtAttenuator;

            ///// <summary>
            ///// 激励模式，Pulse或CW
            ///// </summary>
            //public ExcitationMode m_StimulationMode;
            /// <summary>
            /// 脉冲宽度，按s计
            /// </summary>
            public double m_PulseWidth;
            /// <summary>
            /// 脉冲周期，按s计
            /// </summary>
            public double m_PulsePeriod;

            /// <summary>
            /// 输入功率补偿值，按dB计
            /// </summary>
            public double m_SGOffsetValue;
            /// <summary>
            /// 频谱仪补偿表，FDO型。Key为频率，按Hz计；Value为补偿值，按dB计。
            /// </summary>
            public List<offsetFreq> m_SAFDOTable;
            /// <summary>
            /// 频谱仪全局偏置
            /// </summary>
            public double m_SAGlobalOffset;
            /// <summary>
            /// 信号源频率，按Hz计
            /// </summary>
            public double m_SGFreq;
            /// <summary>
            /// 频谱仪测试频率，按Hz计
            /// </summary>
            public double m_SAFreq;
            /// <summary>
            /// 输入功率，按dBm计
            /// </summary>
            public double m_InputPower;
            /// <summary>
            /// 忽略频率列表（忽略列表频点上的杂波）
            /// </summary>
            public double[] m_IgnoreFreqList;
            /// <summary>
            /// 起始频率
            /// </summary>
            public double m_StartFreq;
            /// <summary>
            /// 终止频率
            /// </summary>
            public double m_StopFreq;
            ///// <summary>
            ///// 频率范围
            ///// </summary>
            //public double m_SpanFreq;
            /// <summary>
            /// 载波附近忽略的带宽
            /// </summary>
            public double m_IgnoreBWidth;
            /// <summary>
            /// 是否使用自动RBW
            /// </summary>
            public bool m_AutoRBW;
            /// <summary>
            /// 分析带宽
            /// </summary>
            public double m_RBW;
            /// <summary>
            /// 是否使用自动VBW
            /// </summary>
            public bool m_AutoVBW;
            /// <summary>
            /// 视频带宽
            /// </summary>
            public double m_VBW;
            /// <summary>
            /// 是否使用自动扫描时间
            /// </summary>
            public bool m_AutoSweepTime;
            /// <summary>
            /// 扫描时间
            /// </summary>
            public double m_SweepTime;
            ///// <summary>
            ///// 扫描时间
            ///// </summary>
            //public int m_ChannelNumber;
            /// <summary>
            /// 扫描点数，按自然数计
            /// </summary>
            public int m_SweepPoints;
            /// <summary>
            /// 平均次数，正整数
            /// </summary>
            public int m_AverageFactor;
            /// <summary>
            /// 是否自动确立垂直标尺
            /// </summary>
            public bool m_AutoScale;
            /// <summary>
            /// 手动输入的垂直标尺分度，在AutoScale无效时有效
            /// </summary>
            public double m_Scale;
            //
            public double sleepTime;
            /// <summary>
            /// 打开激励
            /// </summary>
            public bool m_TurnOnStimulation;
            /// <summary>
            /// 关闭激励
            /// </summary>
            public bool m_TurnOffStimulation;

            /// <summary>
            /// 是否编辑信号源偏置
            /// </summary>
            public bool m_EditSGOffset;
            /// <summary>
            /// 是否编辑频谱仪的FDO表
            /// </summary>
            public bool m_EditSAFDOTable;
            /// <summary>
            /// 是否编辑频谱仪全局偏置
            /// </summary>
            public bool m_EditSAGOffset;

            /// <summary>
            /// 是否更新频率设置
            /// </summary>
            public bool m_EditFreq;
            /// <summary>
            /// 是否更新功率设置
            /// </summary>
            public bool m_EditPower;
            /// <summary>
            /// 是否调节标尺
            /// </summary>
            public bool m_EditScale;
        }

        /// <summary>
        /// 核心测试函数 输出参数结构
        /// </summary>
        public struct TestStructResults
        {
            /// <summary>
            /// 杂波抑制度
            /// </summary>
            public double m_SpuriousSuppression;
            /// <summary>
            /// 载波功率
            /// </summary>
            public double m_CarrierPower;
            /// <summary>
            /// 无源功率
            /// </summary>
            public double m_NOSourcePower;
            /// <summary>
            /// 杂波功率
            /// </summary>
            public double m_SpuriousPower;
            /// <summary>
            /// 杂波频率
            /// </summary>
            public double m_SpuriousFreq;

            /// <summary>
            /// 截屏
            /// </summary>
            public System.Drawing.Image m_ScrSht;
            /// <summary>
            /// 二维迹线，key为第一维度值，value为第二维度值
            /// </summary>
            public KeyValuePair<double, double>[] m_TraceData;

        }

        private static SignalGenerator m_SG;
        public int ExecuteTestSpurious(SystemHardware inSys, TestStructParameters m_Parameters, out TestStructResults m_Results)
        {
#if Simulate
            m_Results = new TestStructResults();
            m_Results.m_MonitorInfo = new DUTMonitorInfo();
            #region 获取遥测数据
            DUTCtrl.CaptureDUTMonitorinfo(out m_Results.m_MonitorInfo);
            #endregion
            m_Results.m_SpuriousSuppression = 30;//杂波抑制度
            m_Results.m_CarrierPower = m_Parameters.m_InputPower;
            m_Results.m_SpuriousPower = m_Parameters.m_InputPower + 30;
            m_Results.m_SpuriousFreq = m_Parameters.m_Freq + 100 * 1e6;
            m_Results.m_TraceData = new KeyValuePair<double, double>[201];
            for (int i = 0; i < 201; i++)
            {
                m_Results.m_TraceData[i] = new KeyValuePair<double, double>(10 * 1e6 + 200 * 1e6 * i, 10 * Math.Tan(Math.PI * i / 100));

            }
            return 0;
 
#endif
            try
            {
                double m_StartFreq = m_Parameters.m_StartFreq;
                double m_StopFreq = m_Parameters.m_StopFreq;
                double Temp_LeftSideSpuriousFreq = -999;
                double Temp_LeftSideSpuriousPower = -999;
                double Temp_RightSideSpuriousFreq = -999;
                double Temp_RightSideSpuriousPower = -999;
                double Temp_RefLevle = -999;//参考功率，调节时功率测量时使用
                double Temp_MaxPower = -999;//频谱中出现的最大功率
                double Temp_Scale = 10;//频谱仪当前垂直尺度
                double Temp_MarkerValue;//当前游标值
                string Temp_SAFDOList = string.Empty;

                m_Results = new TestStructResults();

                #region 设备复位
                m_SG = SystemHardware.Hardware<SignalGenerator>.GetElmentByName("RackSys.TestLab.Instrument.SignalGenerator1");
                if (m_Parameters.m_FirstTest)
                {
                    //信号源复位
                    //inSys.AnalogSignalGenerator.Preset();
                    //inSys.CurrentInstruments
                    m_SG.Preset();
                    //频谱仪复位
                    inSys.SpectrumAnalyzer.AutoAlignEnabled = false;
                    inSys.SpectrumAnalyzer.WaitOpc();
                    inSys.SpectrumAnalyzer.ModePreset();
                    inSys.SpectrumAnalyzer.WaitOpc();
                }
                #endregion 设备复位

                #region 设备基础设置

                //信号源设置
                if (m_Parameters.m_FirstTest)
                {
                    m_SG.RFFrequency = m_Parameters.m_SGFreq;
                    m_SG.AmplOffset = m_Parameters.m_SGOffsetValue;
                    m_SG.RFPower = m_Parameters.m_InputPower;

                }
                inSys.SpectrumAnalyzer.StartFrequency = m_Parameters.m_StartFreq;
                inSys.SpectrumAnalyzer.StopFrequency = m_Parameters.m_StopFreq;
                //频谱仪设置
                if (m_Parameters.m_FirstTest)
                {
                    //RBW设置
                    if (!m_Parameters.m_AutoRBW)
                    { inSys.SpectrumAnalyzer.RBW = m_Parameters.m_RBW; }

                    //VBW设置
                    if (!m_Parameters.m_AutoVBW)
                    { inSys.SpectrumAnalyzer.VBW = m_Parameters.m_VBW; }

                    //Scale设置
                    if (m_Parameters.m_AutoScale)
                    { inSys.SpectrumAnalyzer.dBperDiv = 10; }
                    else
                    { inSys.SpectrumAnalyzer.dBperDiv = m_Parameters.m_Scale; }
                    Temp_Scale = inSys.SpectrumAnalyzer.dBperDiv;//记录实际垂直标尺

                    //补偿表设置
                    for (int i = 0; i < 6; i++)
                    { inSys.SpectrumAnalyzer.Set_CorrectionState(i + 1, false); }
                    inSys.SpectrumAnalyzer.Set_CorrectionState(1, true);

                    //inSys.SpectrumAnalyzer.SetTraceMode(1, SpectrumAnalyzer.TriggerMode.);
                    inSys.SpectrumAnalyzer.SetDetectorMode(1, SpectrumAnalyzer.DetectorMode.POS);
                    inSys.SpectrumAnalyzer.SetMarkerModeByIndex(SpectrumAnalyzer.MarkerModeType.Position, 1);
                    //inSys.SpectrumAnalyzer.PeakExcursionStateOfNxtPkCriteria = false;

                    //扫描时间设置
                    if (m_Parameters.m_AutoSweepTime)
                    { }
                    else
                    { inSys.SpectrumAnalyzer.SweepTime = m_Parameters.m_SweepTime; }

                    inSys.SpectrumAnalyzer.ContinuousSweepEnabled = true;
                }

                #endregion 设备基础设置

                #region 设置变更
                //偏置与补偿变更设置
                if (m_Parameters.m_EditSGOffset)
                {
                    m_SG.AmplOffset = -m_Parameters.m_SGOffsetValue;
                    m_SG.RFPower = m_Parameters.m_InputPower;
                    //m_DUTProtection.Sg_RFPower(inSys.AnalogSignalGenerator, m_Parameters.m_InputPower);
                }
                //频率变更设置
                m_SG.RFFrequency = m_Parameters.m_SGFreq;
                double ampoffset = m_Parameters.m_SAGlobalOffset;
                inSys.SpectrumAnalyzer.AMP_Offset = -ampoffset;

                #endregion
                    //inSys.SpectrumAnalyzer.MarkerToCenterFreq(1);
                //inSys.SpectrumAnalyzer.ContinuousSweepEnabled = false;
                inSys.SpectrumAnalyzer.Sweep(100000);
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.MarkerPeakSearch(1);
                m_Results.m_NOSourcePower = inSys.SpectrumAnalyzer.Marker1Value;
                m_SG.RFOutputEnabled = true;
                inSys.SpectrumAnalyzer.WaitOpc();
                Thread.Sleep(50);
                //inSys.SpectrumAnalyzer.MarkerPeakSearch(1);
                #region 测试载波参数
                //inSys.SpectrumAnalyzer.CenterFrequency = m_Parameters.m_Freq;
                //inSys.SpectrumAnalyzer.Span = m_Parameters.m_IgnoreBWidth;
                //inSys.SpectrumAnalyzer.SetMarkerPositionByIndex(m_Parameters., 1);

                #region 关闭平均以快速调整RefLvl
                inSys.SpectrumAnalyzer.Averages = 1;
                inSys.SpectrumAnalyzer.WaitOpc();
               
                inSys.SpectrumAnalyzer.Sweep(100000);
                inSys.SpectrumAnalyzer.WaitOpc();

                for (int i = 0; i < 20; i++)
                {
                    inSys.SpectrumAnalyzer.MarkerPeakSearch(1);
                    Temp_RefLevle = inSys.SpectrumAnalyzer.Marker1Value;
                    if (Temp_RefLevle < -170)
                    { Temp_RefLevle = -170; }
                    inSys.SpectrumAnalyzer.ReferenceLevel = Temp_RefLevle;

                    inSys.SpectrumAnalyzer.Sweep(100000);
                    inSys.SpectrumAnalyzer.WaitOpc();
                    Temp_MarkerValue = inSys.SpectrumAnalyzer.Marker1Value;
                    //检查markertoRef功能，是都已经调整到位。
                    if ((Temp_RefLevle - Temp_MarkerValue) > (Temp_Scale * -0.25) && (Temp_RefLevle - Temp_MarkerValue) < (Temp_Scale * 0.5))
                    { break; }
                }
                Temp_RefLevle = inSys.SpectrumAnalyzer.ReferenceLevel;
                inSys.SpectrumAnalyzer.ReferenceLevel = Temp_RefLevle + Temp_Scale;
                #endregion

                //平均设置
                if (m_Parameters.m_AverageFactor > 0)
                { inSys.SpectrumAnalyzer.Averages = (uint)m_Parameters.m_AverageFactor; }
                inSys.SpectrumAnalyzer.ContinuousSweepEnabled = false;
                //扫描测量
                inSys.SpectrumAnalyzer.Sweep(100000);
                inSys.SpectrumAnalyzer.WaitOpc();
                Thread.Sleep(50);
                inSys.SpectrumAnalyzer.MarkerPeakSearch(1);
                m_Results.m_CarrierPower = inSys.SpectrumAnalyzer.Marker1Value;
                #endregion 测试载波参数
                //通过计算获取杂波信号
                //double[] Ytracedate=inSys.SpectrumAnalyzer.TraceDataByIndex(1);
                //double[] Xtracedata= inSys.SpectrumAnalyzer.TraceXaxis;
                //List<double> SpuriousList = new List<double>();

                //for (int i = 0; i < Xtracedata.Count(); i++)
                //{
                //    if (Xtracedata[i]< m_Parameters.m_SAFreq - m_Parameters.m_IgnoreBWidth / 2||
                //        Xtracedata[i] > m_Parameters.m_SAFreq + m_Parameters.m_IgnoreBWidth / 2)
                //    {
                //        SpuriousList.Add(Ytracedate[i]);
                //    }
                //}
                //m_Results.m_SpuriousPower = SpuriousList.Max();
                //#region 测试左侧参数

                if (m_StartFreq < (m_Parameters.m_SAFreq - m_Parameters.m_IgnoreBWidth / 2))
                {
                    inSys.SpectrumAnalyzer.StartFrequency = m_StartFreq;
                    inSys.SpectrumAnalyzer.StopFrequency = m_Parameters.m_SAFreq - m_Parameters.m_IgnoreBWidth / 2;
                    if (m_Parameters.m_SweepPoints > 1001)
                    { inSys.SpectrumAnalyzer.SweepPoints = 1001; }
                    else
                    { inSys.SpectrumAnalyzer.SweepPoints = m_Parameters.m_SweepPoints; }

                    inSys.SpectrumAnalyzer.Sweep(100000 * (int)m_Parameters.m_AverageFactor);
                    inSys.SpectrumAnalyzer.WaitOpc();

                    inSys.SpectrumAnalyzer.MarkerPeakSearch(1);
                    Temp_LeftSideSpuriousPower = inSys.SpectrumAnalyzer.Marker1Value;
                    Temp_LeftSideSpuriousFreq = inSys.SpectrumAnalyzer.Marker1Position;
                }
#endregion 测试左侧参数

                #region 测试右侧参数
                if (m_StopFreq > (m_Parameters.m_SAFreq + m_Parameters.m_IgnoreBWidth / 2))
                {
                    inSys.SpectrumAnalyzer.StartFrequency = m_Parameters.m_SAFreq + m_Parameters.m_IgnoreBWidth / 2;
                    inSys.SpectrumAnalyzer.StopFrequency = m_StopFreq;
                    if (m_Parameters.m_SweepPoints > 1001)
                    { inSys.SpectrumAnalyzer.SweepPoints = 1001; }
                    else
                    { inSys.SpectrumAnalyzer.SweepPoints = m_Parameters.m_SweepPoints; }

                    inSys.SpectrumAnalyzer.Sweep(100000 * (int)m_Parameters.m_AverageFactor);
                    inSys.SpectrumAnalyzer.WaitOpc();

                    inSys.SpectrumAnalyzer.MarkerPeakSearch(1);
                    Temp_RightSideSpuriousPower = inSys.SpectrumAnalyzer.Marker1Value;
                    Temp_RightSideSpuriousFreq = inSys.SpectrumAnalyzer.Marker1Position;
                }
                #endregion 测试右侧参数

                m_SG.RFOutputEnabled = false;

                //#region 完成测试
                //if (m_Parameters.m_LastTest)
                //{
                //    if (m_Parameters.m_ContinueMeasAfterTest)//继续测量
                //    {
                //        inSys.SpectrumAnalyzer.ContinuousSweepEnabled = true;
                //        m_SG.RFOutputEnabled = true;
                //    }
                //    else//关闭信号源输出
                //    {
                //        m_SG.RFOutputEnabled = false;
                //    }
                //}
                //#endregion

                #region 计算抑制度
                if (Temp_LeftSideSpuriousPower >= Temp_RightSideSpuriousPower)
                {
                    m_Results.m_SpuriousPower = Temp_LeftSideSpuriousPower;
                    m_Results.m_SpuriousFreq = Temp_LeftSideSpuriousFreq;
                }
                else
                {
                    m_Results.m_SpuriousPower = Temp_RightSideSpuriousPower;
                    m_Results.m_SpuriousFreq = Temp_RightSideSpuriousFreq;
                }
                m_Results.m_SpuriousSuppression = m_Results.m_CarrierPower - m_Results.m_SpuriousPower;
                #endregion 计算抑制度

                return 0;
            }
            catch (Exception ex)
            {
                GlobalStatusReport.ReportError("[信号源 + 频谱仪] 杂波特性 核心测试异常：" + ex.StackTrace, ex);
                throw ex;
            }
        }


        
    }
}
