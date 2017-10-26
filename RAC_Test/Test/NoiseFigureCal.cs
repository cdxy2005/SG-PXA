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

namespace RAC_Test.Test
{


    public struct NoiseFigureCalStructParameter
    {
        public double m_StartFerq;
        public double m_StopFerq;
        public uint m_AverageNum;
        public int m_SweepPoints;
        public string m_StateSaveFileName;
        public double m_RBW;
    }
    public class NoiseFigureCal
    {
        public static string noiseResFile = @"C:\CalFiles\noiseCal.state";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inSys"></param>
        /// <param name="para"></param>
        public static bool runExecute(SystemHardware inSys, Test_Paramter para)
        {

            bool res = false;
            //转化校准参数
            NoiseFigureCalStructParameter noiseFigureCalStructParameter = new NoiseFigureCalStructParameter();
            noiseFigureCalStructParameter.m_StartFerq = para.NoiseAndPxaParamter.StartFreq;
            noiseFigureCalStructParameter.m_StopFerq = para.NoiseAndPxaParamter.StopFreq;
            noiseFigureCalStructParameter.m_SweepPoints = (int)Math.Floor(1 + (para.NoiseAndPxaParamter.StopFreq - para.NoiseAndPxaParamter.StartFreq) / para.NoiseAndPxaParamter.FreqSpac);
            noiseFigureCalStructParameter.m_AverageNum = 1;
            noiseFigureCalStructParameter.m_RBW = 4000000;
            //noiseFigureCalStructParameter.m_StateSaveFileName = NoiseFigureCalKeyAndResultList.path+System.IO.Path.GetFileNameWithoutExtension(para.SpectrumStateFileName) + ".state";
            noiseFigureCalStructParameter.m_StateSaveFileName = noiseResFile;

            /* 执行校准*/
            ExecuteTest(inSys, noiseFigureCalStructParameter);

            return res;
        }

        public static int ExecuteTest(SystemHardware inSys, NoiseFigureCalStructParameter m_NFParameter)
        {
            try
            {
                //频谱仪复位
                inSys.SpectrumAnalyzer.AutoAlignEnabled = false;
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.Preset();
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.NoiseFigureMode();
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.ModePreset();
                inSys.SpectrumAnalyzer.WaitOpc();

                ////频谱仪设置
                inSys.SpectrumAnalyzer.StartFrequency = m_NFParameter.m_StartFerq;
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.StopFrequency = m_NFParameter.m_StopFerq;
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.Averages = m_NFParameter.m_AverageNum;
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.SweepPoints = m_NFParameter.m_SweepPoints;
                inSys.SpectrumAnalyzer.WaitOpc();
                inSys.SpectrumAnalyzer.NF_RBW = m_NFParameter.m_RBW;
                inSys.SpectrumAnalyzer.WaitOpc();

                //执行频谱仪噪声测试校准
                inSys.SpectrumAnalyzer.NoiseFigureCalNow("");
                //System.Threading.Thread.time

                //inSys.SpectrumAnalyzer.WaitOpc();
                //inSys.SpectrumAnalyzer.WaitOpc();
                //inSys.SpectrumAnalyzer.WaitOpc();
                //inSys.SpectrumAnalyzer.WaitOpc();
                //inSys.SpectrumAnalyzer.WaitOpc();

                inSys.SpectrumAnalyzer.WaitOpc(Convert.ToInt32(m_NFParameter.m_SweepPoints * m_NFParameter.m_AverageNum * 10000));

                inSys.SpectrumAnalyzer.WaitOpc(Convert.ToInt32(m_NFParameter.m_SweepPoints * m_NFParameter.m_AverageNum * 10000));

                //关闭连续扫描
                inSys.SpectrumAnalyzer.ContinuousSweepEnabled = false;
                inSys.SpectrumAnalyzer.WaitOpc(Convert.ToInt32(m_NFParameter.m_SweepPoints * m_NFParameter.m_AverageNum * 10000));
                //启动扫描一次并保存状态文件

                //inSys.SpectrumAnalyzer.
                //inSys.SpectrumAnalyzer.SendSCPI(":INIT:CONT OFF");

                inSys.SpectrumAnalyzer.Sweep();







                inSys.SpectrumAnalyzer.WaitOpc(Convert.ToInt32(m_NFParameter.m_SweepPoints * m_NFParameter.m_AverageNum * 10000));

                inSys.SpectrumAnalyzer.SendSCPI(":NFIG:CAL:STAT ON");
                inSys.SpectrumAnalyzer.WaitOpc();

                try
                {
                    inSys.SpectrumAnalyzer.SaveStateToPath(m_NFParameter.m_StateSaveFileName);
                    inSys.SpectrumAnalyzer.WaitOpc();

                }
                catch
                {
                    string[] paths = m_NFParameter.m_StateSaveFileName.Split('\\');
                    string creatFloder = paths[0];

                    for (int i = 1; i < paths.Length - 1; i++)
                    {
                        creatFloder += "\\";
                        creatFloder += paths[i];
                        inSys.SpectrumAnalyzer.CreatFile(creatFloder);
                        inSys.SpectrumAnalyzer.WaitOpc();

                    }

                    inSys.SpectrumAnalyzer.SaveStateToPath(m_NFParameter.m_StateSaveFileName);
                    inSys.SpectrumAnalyzer.WaitOpc();

                }



                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
