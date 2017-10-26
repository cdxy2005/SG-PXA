using System;
using System.Xml.Serialization;
using RackSys.TestLab.Hardware;
using System.Drawing;
using RackSys.TestLab.Instrument;
using System.Threading;
using RackSys.TestLab;
//using RackSys.TestLab.Calibration;

namespace RAC_Test.Test
{

    public class NoiseFigureTest 
    {
        public  string TestProcedureName
        {
            get
            {
                return "噪声系数";
            }
        }

        ///// <summary>
        ///// 根据测试类型获取激励端口类型
        ///// </summary>
        //public override StimulateSignalPortType[] GetStimulateSignalPortType()
        //{
        //    StimulateSignalPortType[] tmp = new StimulateSignalPortType[1];
        //    tmp[0] = StimulateSignalPortType.NoiseSourceRFOut;
        //    return tmp;
        //}

        ///// <summary>
        ///// 根据测试类型获取接收端端口类型
        ///// </summary>
        //public override SensorInputPort[] GetSensorInputPortType()
        //{
        //    SensorInputPort[] tmp = new SensorInputPort[1];
        //    tmp[0] = SensorInputPort.PXAInput;
        //    return tmp;
        //}

  

        public struct NoiseFigureTestStructParameter
        {

            /// <summary>
            /// 起始频率，in Hz
            /// </summary>
            public double m_StartFerq;

            /// <summary>
            /// 截止频率，in Hz
            /// </summary>
            public double m_StopFerq;

            /// <summary>
            /// 噪声系数分析带宽，in Hz
            /// </summary>
            public double m_RBW;

            /// <summary>
            /// 平均次数
            /// </summary>
            public uint m_AverageNum;

            /// <summary>
            /// 扫描点数
            /// </summary>
            public int m_SweepPoints;
            /// <summary>
            /// 要读取的频点列表
            /// </summary>
            public double[] m_FreqList;
            /// <summary>
            /// BeforeDUT补偿列表
            /// </summary>
            public string m_LossCompTabelBeforeDUT;
            /// <summary>
            /// AfterDUT补偿列表
            /// </summary>
            public string m_LossCompTabelAfterDUT;
            /// <summary>
            /// 是否使用状态文件
            /// </summary>
            public bool m_RecallStateFileEnable;
            /// <summary>
            /// 状态文件
            /// </summary>
            public string m_RecallStateFileName;
            /// <summary>
            /// 测试轨迹最大值
            /// </summary>
            public bool m_IsReadMaxNoiseFigure;
            ///输入路径损耗
            //public RouteLossCalResultForOnePath m_InputRouteLossCalResultForOnePath;
            ////输出路径损耗
            //public RouteLossCalResultForOnePath m_OutputRouteLossCalResultForOnePath;
            public double Yscale;//频谱仪截图纵轴标尺刻度
            public bool m_NeedContinuousScanning;//是否进行调试模式，如果为真，测试结束之后，仪表激励不关闭，且扫描模式改成连续扫描
        }

        public struct NoiseFigureTestStructResult
        {
            public double[] m_FreqList;
            public double[] m_NoiseFigure;
            public double[] m_Gain;
            public double m_NoiseFigureMax;
            /// <summary>
            /// 最大频点
            /// </summary>
            public double m_FreqForNoiseFigureMax;
            public Image m_ScreenImage;
        }


        /// <summary>
        /// 执行测试过程
        /// </summary>
        /// <param name="inSys">SystemHardware</param>
        /// <param name="m_NoiseFigureParameter">参数</param>
        /// <param name="m_NoiseFigureTestResult">结果</param>
        /// <returns>0：正常返回，，，，-1：强制中断</returns>
        public static int ExecuteTest(
            SystemHardware inSys, 
            NoiseFigureTestStructParameter m_NoiseFigureParameter,          
            out NoiseFigureTestStructResult m_NoiseFigureTestResult
            )
        {
            try
            {
#if Simulate
                m_NoiseFigureTestResult = new NoiseFigureTestStructResult();
                m_NoiseFigureTestResult.m_FreqList = m_NoiseFigureParameter.m_FreqList;
                m_NoiseFigureTestResult.m_Gain = new double[m_NoiseFigureParameter.m_FreqList.Length];
                m_NoiseFigureTestResult.m_NoiseFigure = new double[m_NoiseFigureParameter.m_FreqList.Length];
                for (int i = 0; i < m_NoiseFigureTestResult.m_NoiseFigure.Length; i++)
                {
                    m_NoiseFigureTestResult.m_NoiseFigure[i] = 3 + 0.1 * i;
                }
                //噪声系数最大值的仿真处理
                m_NoiseFigureTestResult.m_NoiseFigureMax =  3 + 0.1 * (m_NoiseFigureTestResult.m_NoiseFigure.Length - 1);
                //最大值频点位置
                m_NoiseFigureTestResult.m_FreqForNoiseFigureMax
                    = m_NoiseFigureTestResult.m_FreqList[m_NoiseFigureTestResult.m_FreqList.Length - 1];

                string ImagefileName = AppDomain.CurrentDomain.BaseDirectory + @"\屏幕截图仿真\噪声系数测试样图.png";

                m_NoiseFigureTestResult.m_ScreenImage = Image.FromFile(ImagefileName);

                Thread.Sleep(100);
                DUTCtrl.CaptureDUTMonitorinfo(out tmpDutMonitorInfo);

                GlobalStatusReport.Report("噪声系数测试：仿真测试结果");
                return 0;
#endif
                m_NoiseFigureTestResult = new NoiseFigureTestStructResult();
                m_NoiseFigureTestResult.m_FreqList = m_NoiseFigureParameter.m_FreqList;
                m_NoiseFigureTestResult.m_NoiseFigure = new double[m_NoiseFigureParameter.m_FreqList.Length];
                m_NoiseFigureTestResult.m_Gain = new double[m_NoiseFigureParameter.m_FreqList.Length];
                inSys.SpectrumAnalyzer.Timeout = 30 * 1000;

                //inSys.SpectrumAnalyzer.SaveStateToPath("D:/User_My_Documents/Instrument/My Documents/SA/state/CalStateTemp.state");
                //inSys.SpectrumAnalyzer.WaitOpc();
                //频谱仪复位
                inSys.SpectrumAnalyzer.AutoAlignEnabled = false;
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.Preset();
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.NoiseFigureMode();
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.ModePreset();
                inSys.SpectrumAnalyzer.WaitOpc();
                
                if (m_NoiseFigureParameter.m_RecallStateFileEnable)
                {
                    inSys.SpectrumAnalyzer.LoadStateFromPath(m_NoiseFigureParameter.m_RecallStateFileName);
                }
                else
                {
                    //无需调用原有的状态文件时，则重新调用当前状态，抵消仪表reset带来的影响。
                    inSys.SpectrumAnalyzer.LoadState("CalStateTemp");
                }
                inSys.SpectrumAnalyzer.SendSCPI(":NFIG:CAL:STAT ON");
                inSys.SpectrumAnalyzer.WaitOpc();
                //次数设置测试所使用的参数
                //频谱仪设置
                inSys.SpectrumAnalyzer.StartFrequency = m_NoiseFigureParameter.m_StartFerq;
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.StopFrequency = m_NoiseFigureParameter.m_StopFerq;
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.NF_RBW = m_NoiseFigureParameter.m_RBW;
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.Averages = m_NoiseFigureParameter.m_AverageNum;
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.SweepPoints = m_NoiseFigureParameter.m_SweepPoints;
                inSys.SpectrumAnalyzer.WaitOpc();


                //关闭连续扫描
                inSys.SpectrumAnalyzer.ContinuousSweepEnabled = false;
                inSys.SpectrumAnalyzer.WaitOpc();

                //设置显示窗口Zoom，将Gain的曲线隐藏，调试时请屏蔽代码，用于观察信号。
                //inSys.SpectrumAnalyzer.SelectDisplayWindow(1);
                //inSys.SpectrumAnalyzer.WaitOpc();
                //inSys.SpectrumAnalyzer.SetWindowZoom(true);
                //inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.SetNFWindowAutoScale(1,true);
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.SetNFWindowAutoScale(2, true);
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.SetDisPlayFormat(SpectrumAnalyzer.DisPlayModeType.GRAPh);
                inSys.SpectrumAnalyzer.WaitOpc();


                //路径补偿数据
                //inSys.SpectrumAnalyzer.NoiseFigureLossCompModeBeforeDUT(SpectrumAnalyzer.NoiseFigureLossCompMode.TABL);
                //inSys.SpectrumAnalyzer.WaitOpc();
                //inSys.SpectrumAnalyzer.NoiseFigureLossCompModeAfterDUT(SpectrumAnalyzer.NoiseFigureLossCompMode.TABL);
                //inSys.SpectrumAnalyzer.WaitOpc();
                //inSys.SpectrumAnalyzer.SetLossCompTableBeforeDUT(m_NoiseFigureParameter.m_LossCompTabelBeforeDUT);        //2014.3.11改变噪声系数补偿方式，不采用beforeDUT，afterDUT的补偿方式，改为程序自己计算。苏渊红。
                //inSys.SpectrumAnalyzer.WaitOpc();
                //inSys.SpectrumAnalyzer.SetLossCompTableAfterDUT(m_NoiseFigureParameter.m_LossCompTabelAfterDUT);
                //inSys.SpectrumAnalyzer.WaitOpc();
      
                //启动扫描一次
                inSys.SpectrumAnalyzer.Sweep();
               //inSys.SpectrumAnalyzer.WaitOpc();
       
                inSys.SpectrumAnalyzer.WaitOpc(Convert.ToInt32(m_NoiseFigureParameter.m_SweepPoints * m_NoiseFigureParameter.m_AverageNum * 1000));

                //读取噪声系数测试结果
                inSys.SpectrumAnalyzer.SetMarkerModeByIndex(SpectrumAnalyzer.MarkerModeType.Position, 1);
                inSys.SpectrumAnalyzer.WaitOpc();
                double temp_maxNF = 0;
                double temp_maxNFFreq = 0;

                

                for (int i = 0; i < m_NoiseFigureParameter.m_FreqList.Length; i++)
                {
            
                    inSys.SpectrumAnalyzer.SetMarkerPositionByIndex(m_NoiseFigureParameter.m_FreqList[i], 1);
                    inSys.SpectrumAnalyzer.WaitOpc();
                    double tempNF = inSys.SpectrumAnalyzer.GetMarkerNFValueByIndex(1);
                    inSys.SpectrumAnalyzer.WaitOpc();
                    double tempGain = inSys.SpectrumAnalyzer.GetMarkerGainValueByIndex(1);
                    inSys.SpectrumAnalyzer.WaitOpc();
                    m_NoiseFigureTestResult.m_NoiseFigure[i] = Math.Round(tempNF, 2);
                    m_NoiseFigureTestResult.m_Gain[i] = Math.Round(tempGain, 2);

                    

                    //add 2014.03.11，噪声系数计算公式--根据总的噪声系数和增益，计算被测DUT实际的噪声系数
                    #region 真实噪声系数计算
                    NoseFigureCalResult RealNFResult = new NoseFigureCalResult();
                    //todo
                    double inputRouteLossIndB = 0;
 //m_NoiseFigureParameter.m_InputRouteLossCalResultForOnePath.GetRouteLossIndBAtFreq(m_NoiseFigureParameter.m_FreqList[i]);
                    double outputRouteLossIndB =
                        0;
                    NoisFigureCalculation(inputRouteLossIndB, outputRouteLossIndB, tempNF, tempGain, out RealNFResult);
                    m_NoiseFigureTestResult.m_NoiseFigure[i] = Math.Round(RealNFResult.RealNoiseFigure, 2);
                    m_NoiseFigureTestResult.m_Gain[i] = Math.Round(RealNFResult.RealGain, 2);
                    if (m_NoiseFigureTestResult.m_NoiseFigure[i] > temp_maxNF)//追踪最大噪声系数的点
                    { temp_maxNF = m_NoiseFigureTestResult.m_NoiseFigure[i];
                    temp_maxNFFreq = m_NoiseFigureParameter.m_FreqList[i];
                    }
                    #endregion 真实噪声系数计算
                }

      
                if (m_NoiseFigureParameter.m_IsReadMaxNoiseFigure)
                {
                    m_NoiseFigureTestResult.m_NoiseFigureMax = temp_maxNF;
                    m_NoiseFigureTestResult.m_FreqForNoiseFigureMax = temp_maxNFFreq;
                }
                if (m_NoiseFigureParameter.Yscale > 0)    //  Scale大于零这进行设置，如果不是大于0，代表采用Auto
                {
                    string YscaleString = m_NoiseFigureParameter.Yscale.ToString();
                    inSys.SpectrumAnalyzer.SendSCPI("DISP:WIND:TRAC:Y:PDIV " + YscaleString);
                    inSys.SpectrumAnalyzer.WaitOpc();

                }
                m_NoiseFigureTestResult.m_ScreenImage = inSys.SpectrumAnalyzer.CaptureScreenImage();

                //重置仪表
                //inSys.SpectrumAnalyzer.Preset();
                //inSys.SpectrumAnalyzer.WaitOpc();
                //判断是否进行调试模式，如果是，则不关闭激励，并且将仪器扫描方式改成连续扫描
                if (m_NoiseFigureParameter.m_NeedContinuousScanning)
                {
                    
                    inSys.SpectrumAnalyzer.ContinuousSweepEnabled = true;
                    inSys.SpectrumAnalyzer.WaitOpc();
                }
                inSys.SpectrumAnalyzer.Timeout = 10 * 1000;
                return 0;
            }
            catch (Exception ex)
            {
                GlobalStatusReport.ReportError(ex);
                throw ex;
            }
        }

        public struct NoseFigureCalResult
        {
            public double RealNoiseFigure;
            public double RealGain;
        }
        /// <summary>
        /// 噪声系数计算公式
        /// </summary>
        /// <param name="inputLoss">输入开关矩阵的损耗，正值</param>
        /// <param name="outputLoss">输出开关矩阵的损耗，正值</param>
        /// <param name="TotalNoiseFigure">读取的总的噪声系数</param>
        /// <param name="TotalGain">读取的总的增益</param>
        public static int  NoisFigureCalculation(double inputLoss,double outputLoss,double TotalNoiseFigure,double TotalGain,out NoseFigureCalResult noiseFigureCalResult)
        {
            //目前默认两个衰减值都是取负数，如果统一改为正数，则需要修改符号。
            double tempGain = TotalGain - inputLoss-outputLoss;    //去除输入开关,输出开关矩阵矩阵衰减之后,DUT增益
            double tempNoisFigure = TotalNoiseFigure +inputLoss;   //去除输入开关矩阵之后，DUT和输出开关矩阵的总的噪声系数
            double linearTempGain = Math.Pow(10, 0.1 * tempGain);
            double linearTemNoiseFigure = Math.Pow(10, 0.1 * tempNoisFigure);
            double linearOutputMatrixNoiseFigure = Math.Pow(10, 0.1 * Math.Abs(outputLoss));      //此处将输出开关矩阵的损耗值（正的），直接作为输出开关矩阵的噪声系数进行运算
            double linearDUTNoiseFigure = linearTemNoiseFigure - ((linearOutputMatrixNoiseFigure - 1) / linearTempGain);
            noiseFigureCalResult.RealNoiseFigure=10*Math.Log10(linearDUTNoiseFigure);
            noiseFigureCalResult.RealGain = tempGain;
            return 0;
        }



 




    }
}
