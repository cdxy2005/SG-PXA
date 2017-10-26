using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using RackSys.TestLab.Hardware;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Threading;
using RackSys.TestLab.Instrument;
using System.IO;
using System.Threading;

namespace RAC_Test.Test
{

    public struct CalStructParameter
    {
        public List< double> M_SGFreqList;
        public List<double> M_SAFreqList;
        //public int m_SweepPoints;
        public string m_SaveFileName;
        public bool isInput; 
        public double m_RBW;
        public double m_VBW;
        public double m_FreqSpan;
    }

    public struct CalResult
    {
    
        public List<offsetFreq> RouteLoss;
       // public List<offsetFreq> OutputLoss;
    
    }
    class InputAndOutputCal
    {
        public static string ActiveUpOUTPUTLossFile = @"D:\CalFiles\ActiveUpOUTPUTLossFile.txt";
        public static string ActiveUpINPUTLossFile = @"D:\CalFiles\ActiveUpINPUTLossFile.txt";
        //public static string ActiveDownOUTPUTLossFile = @"C:\CalFiles\ActiveDownOUTPUTLossFile.txt";
        //public static string ActiveDownINPUTLossFile = @"C:\CalFiles\ActiveDownINPUTLossFile.txt";
        //public static string PassiveUpOUTPUTLossFile = @"C:\CalFiles\PassiveUpOUTPUTLossFile.txt";
        //public static string PassiveDownOUTPUTLossFile = @"C:\CalFiles\PassiveDownOUTPUTLossFile.txt";
        // public static string INPUTLossFile = @"C:\CalFiles\INPUTLossFile.txt";
        public static bool runExecute(SystemHardware inSys, Test_Paramter para,string fileName, bool isInput)
        {
            CalResult Result = new CalResult();
            bool res = false;
            //转化校准参数
            CalStructParameter Parameter = new CalStructParameter();
            double  m_SGStartFerq = para.SGAndPxaParamter.StartFreq;
            double  m_SGStopFerq = para.SGAndPxaParamter.StopFreq;
            double m_SGFreqSpac = para.SGAndPxaParamter.FreqSpac;
            double m_IFFreq = para.DUTOutFreq;
            //if (fileName.IndexOf("Active") >= 0)
            //{
            //     m_SGStartFerq = para.DUTAndPxaParamter.StartFreq;
            //     m_SGStopFerq = para.DUTAndPxaParamter.StopFreq;
            //     m_SGFreqSpac = para.DUTAndPxaParamter.FreqSpac;
            //}

            int m_SweepPoints = (int)Math.Floor(1 + (m_SGStopFerq - m_SGStartFerq) / m_SGFreqSpac);//扫描点数
           
            List<double> SGFreqList = new List<double>();
            SGFreqList.Add(m_IFFreq);
            for (int i = 0; i < m_SweepPoints; i++)
            {
                double freq = m_SGStartFerq + i * para.SGAndPxaParamter.FreqSpac;
                if (freq <= para.SGAndPxaParamter.StopFreq)
                {
                    SGFreqList.Add(freq);
                }
                else
                {
                    SGFreqList.Add(para.SGAndPxaParamter.StopFreq);
                }
            }
            Parameter.M_SGFreqList = SGFreqList;
            Parameter.M_SAFreqList = SGFreqList;
            //double m_SAStartFerq = para.SGAndPxaParamter.StartFreq;//todo 获取频谱仪前端校准频率
            //double m_SAStopFerq = para.SGAndPxaParamter.StopFreq;//todo 获取频谱仪前端校准频率
            //int m_TestPoints = (int)Math.Floor(1 + (m_SAStopFerq - m_SAStartFerq) / para.SGAndPxaParamter.FreqSpac);
            //List<double> testFreqList = new List<double>();
            //testFreqList.Add(m_IFFreq);
            //for (int i = 0; i < m_SweepPoints; i++)
            //{
            //    double freq = m_SGStartFerq + i * para.SGAndPxaParamter.FreqSpac;
            //    if (freq <= para.SGAndPxaParamter.StopFreq)
            //    {
            //        testFreqList.Add(freq);
            //    }
            //    else
            //    {
            //        testFreqList.Add(para.SGAndPxaParamter.StopFreq);
            //    }
            //}
            //Parameter.M_SAFreqList = testFreqList;

            Parameter.m_RBW = para.SGAndPxaParamter.RBW;
            Parameter.m_RBW = para.SGAndPxaParamter.VBW;
            Parameter.m_FreqSpan = para.SGAndPxaParamter.FreqSpac;
            //noiseFigureCalStructParameter.m_StateSaveFileName = NoiseFigureCalKeyAndResultList.path+System.IO.Path.GetFileNameWithoutExtension(para.SpectrumStateFileName) + ".state";
            Parameter.m_SaveFileName = fileName;
            Parameter.isInput = isInput;
            //Parameter.m_OutputSaveFileName = OUTPUTLossFile;
            /* 执行校准*/
            ExecuteTest(inSys, Parameter,out Result);
            //DUTOutFreq. m_InPutoffst.
            SaveDataToFile(Result.RouteLoss,fileName);
            if (fileName == InputAndOutputCal.ActiveUpINPUTLossFile)
            {
                Test_Paramter.CurPWDCPowerInfo.InputFileoffst = Result.RouteLoss;
            }
            else
            {
                Test_Paramter.CurPWDCPowerInfo.OutputFileoffst = Result.RouteLoss;
            }

            return res;
        }


        private static SignalGenerator m_SG;
        public static int ExecuteTest(SystemHardware inSys, CalStructParameter m_Parameter,out CalResult result)
        {
            try
            {
                if (m_Parameter.isInput)
                {
                    MessageBox.Show("请将信号源输出线与频谱仪连接！");
                }
                else
                {
                    MessageBox.Show("将信号源与输入DUT连接线，DUT输出RF线连接到频谱仪！");
                }
                CalResult resultnew = new CalResult();
                //信号源初始化
                m_SG = SystemHardware.Hardware<SignalGenerator>.GetElmentByName("RackSys.TestLab.Instrument.SignalGenerator1");
                m_SG.Preset();
                m_SG.RFOutputEnabled = false;
                //频谱仪复位
                inSys.SpectrumAnalyzer.AutoAlignEnabled = false;
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.Preset();
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.ModePreset();
                inSys.SpectrumAnalyzer.Set_RefLvlOffsetByWindow(1, 0);
                inSys.SpectrumAnalyzer.AMP_Offset= 0;
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.Span = 1*1e7;//固定为10Mhz
                inSys.SpectrumAnalyzer.CenterFrequency = m_Parameter.M_SAFreqList[0];
                m_SG.RFPower = 0;
                //inSys.SpectrumAnalyzer.SweepPoints = m_NFParameter.m_SweepPoints;
                //inSys.SpectrumAnalyzer.  = m_Parameter.m_RBW;
                //inSys.SpectrumAnalyzer.WaitOpc();
                //inSys.SpectrumAnalyzer.VBW = m_Parameter.m_VBW;
                //关闭连续扫描
                inSys.SpectrumAnalyzer.ContinuousSweepEnabled = false;
                m_SG.ModOutputEnabled = false;
                m_SG.RFOutputEnabled = true;
                List<offsetFreq> INputLosss = new List<offsetFreq>();
                for (int i = 0; i < m_Parameter.M_SGFreqList.Count; i++)
                {
                     offsetFreq offset = new offsetFreq();
                     offset.FreqInMHz = m_Parameter.M_SGFreqList[i]*1e-6;

                    m_SG.RFFrequency = m_Parameter.M_SGFreqList[i];
                    Thread.Sleep(100);
                    inSys.SpectrumAnalyzer.CenterFrequency = m_Parameter.M_SGFreqList[i];
                    inSys.SpectrumAnalyzer.Sweep();
                    inSys.SpectrumAnalyzer.WaitOpc(20000);
                    inSys.SpectrumAnalyzer.MarkerToCenterFreq(1);
                    inSys.SpectrumAnalyzer.MarkerPeakSearch(1);
                    inSys.SpectrumAnalyzer.Marker1Mode= SpectrumAnalyzer.MarkerModeType.Position;
                    offset.ReceiverOffsetFreqInMHz = inSys.SpectrumAnalyzer.Marker1Value;
                    INputLosss.Add(offset);
                }
                resultnew.RouteLoss = INputLosss;

                result= resultnew;
                m_SG.RFOutputEnabled = false;
                m_SG.RFPower = -100;
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int SaveDataToFile(List<offsetFreq> data,string fileName)
        {
            int res = 0;
            try
            {
                //string param = System.Text.Encoding.UTF8.GetString(paramByte, 0, paramByte.Length);
                
                using (StreamWriter sw = new StreamWriter(fileName, false))
                {
                    foreach (offsetFreq temp in data)
                    {
                        sw.Write(temp.FreqInMHz*1e6);
                        sw.Write(",");
                        sw.Write(temp.ReceiverOffsetFreqInMHz);
                        sw.Write(",");
                    }
                   
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
                //return 0; 
                //throw;
            }
            return res; 
        }
    }
}
