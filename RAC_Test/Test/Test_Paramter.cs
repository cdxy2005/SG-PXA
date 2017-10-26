
using RackSys.TestLab.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RAC_Test.Test
{
    public class Test_Paramter : INotifyPropertyChanged
    {

        private static Test_Paramter m_CurSystemAlarmInfo;

        private string m_CurrentXMLFileName;

        /// <summary>
        /// 当前测试参数集合
        /// </summary>
        public static Test_Paramter CurPWDCPowerInfo
        {
            get
            {
                if (object.ReferenceEquals(m_CurSystemAlarmInfo, null))
                {
                    Test_Paramter temp = new Test_Paramter();

                    temp.m_CurrentXMLFileName = AppDomain.CurrentDomain.BaseDirectory + "PWDCPowerInfo.config";
                    m_CurSystemAlarmInfo = temp.LoadParameterFromXMLFile(temp.m_CurrentXMLFileName);

                }
                return m_CurSystemAlarmInfo;
            }
        }

        /// <summary>
        /// 加载测试参数数据
        /// </summary>
        /// <param name="inXmlFileName">要加载的测试参数配置文件名，为空时从默认文件中加载</param>
        public Test_Paramter LoadParameterFromXMLFile(string inXmlFileName = null)
        {
            //缺省参数处理
            if ((inXmlFileName == null) || (inXmlFileName == ""))
            {
                inXmlFileName = AppDomain.CurrentDomain.BaseDirectory + "PWDCPowerInfo.config";
            }
            try
            {
                Test_Paramter tmpDUTs = (Test_Paramter)XmlHelper.LoadParameterFromXMLFile(typeof(Test_Paramter), inXmlFileName);
                if (tmpDUTs == null)
                {
                    Test_Paramter temp = new Test_Paramter();


                    return temp;
                }

                return tmpDUTs;
            }
            catch (Exception)
            {
                return new Test_Paramter();
            }
        }



        /// <summary>
        /// 保存进XML文件当中
        /// </summary>
        /// <param name="inXMLFileName"></param>
        public void SaveParameterToXMLFile(string inXMLFileName = null)
        {
            if ((inXMLFileName == null) || (inXMLFileName == ""))
            {
                inXMLFileName = AppDomain.CurrentDomain.BaseDirectory + "PWDCPowerInfo.config";
            }

            XmlHelper.SaveParameterToXMLFile(this.GetType(), this, inXMLFileName);
        }
        #region 构造函数
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string devstate)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(devstate));
            }
        }
        #endregion

        #region DUT
        private string m_DUTName;

        public string DUTName
        {
            get { return m_DUTName; }
            set
            {
                if (m_DUTName != value)
                {
                    NotifyPropertyChanged("DUTName");
                    m_DUTName = value;
                }
            }
        }

        private double m_DUTOutFreq;

        public double DUTOutFreq
        {
            get { return m_DUTOutFreq; }
            set
            {
                if (m_DUTOutFreq != value)
                {
                    NotifyPropertyChanged("DUTOutFreq");
                    m_DUTOutFreq = value;
                }
            }
        }

        /// <summary>
        /// 是否和频，TRUE
        /// </summary>
        public bool isUpConverFreq { get; set; }

        private bool m_isTxWork = false;
        /// <summary>
        /// 是否发射
        /// </summary>
        public bool isTxWork
        {
            get
            { return m_isTxWork; }
            set { m_isTxWork = value; }
        }

        /// <summary>
        /// 等待时间
        /// </summary>
        /// 
        private double m_SleepTime;

        public double sleepTime
        {
            get { return m_SleepTime; }
            set
            {
                //if (m_SleepTime != value)
                //{
                NotifyPropertyChanged("sleepTime");
                m_SleepTime = value;


                //}
            }
        }
        /// <summary>
        /// 通道名称
        /// </summary>
        /// 
        private int m_ChannelNumber;
        /// <summary>
        /// 测试通道
        /// </summary>
        public int ChannelNumber
        {
            get { return m_ChannelNumber; }
            set
            {
                //if (m_ChannelNumber != value)
                //{
                m_ChannelNumber = value;
                NotifyPropertyChanged("ChannelNumber");
                //}
            }
        }
        // public double sleepTime { get; set; }
        #endregion

        #region 测试项目

        private bool m_IsPowerTest;

        public bool IsPowerTest
        {
            get { return m_IsPowerTest; }
            set
            {
                m_IsPowerTest = value;
                NotifyPropertyChanged("IsPowerTest");
            }
        }

        private bool m_IsClutterTest;

        public bool IsClutterTest
        {
            get { return m_IsClutterTest; }
            set
            {
                m_IsClutterTest = value;
                NotifyPropertyChanged("IsClutterTest");
            }
        }

        private bool m_IsNoiseTest;

        public bool IsNoiseTest
        {
            get { return m_IsNoiseTest; }
            set
            {
                m_IsNoiseTest = value;
                NotifyPropertyChanged("IsNoiseTest");
            }
        }


        private bool m_IsNoSourceTest;

        public bool IsNoSourceTest
        {
            get { return m_IsNoSourceTest; }
            set
            {
                m_IsNoSourceTest = value;
                NotifyPropertyChanged("IsNoSourceTest");
            }
        }
        #endregion

        //#region 测试参数
        ///// <summary>
        ///// 起始频率
        ///// </summary>
        //private double m_StartFreq;

        //public double StartFreq
        //{
        //    get { return m_StartFreq; }
        //    set
        //    {
        //        if (value > 0)
        //        {
        //            if (value != m_StartFreq)
        //            {
        //                this.m_StartFreq = value;
        //                NotifyPropertyChanged("StartFreq");
        //            }
        //        }

        //    }
        //}

        //private double m_StopFreq;

        //public double StopFreq
        //{
        //    get { return m_StopFreq; }
        //    set
        //    {
        //        if (value > 0)
        //        {
        //            if (value != m_StopFreq)
        //            {
        //                this.m_StopFreq = value;
        //                NotifyPropertyChanged("StopFreq");
        //            }
        //        }
        //    }
        //}

        //private double m_FreqSpac;

        //public double FreqSpac
        //{
        //    get { return m_FreqSpac; }
        //    set
        //    {
        //        if (value > 0)
        //        {
        //            if (value != m_FreqSpac)
        //            {
        //                this.m_FreqSpac = value;
        //                NotifyPropertyChanged("FreqSpac");
        //            }
        //        }
        //    }
        //}

        //private double m_SimulatePower;

        //public double InPower
        //{
        //    get { return m_SimulatePower; }
        //    set
        //    {
        //        if (value != m_SimulatePower)
        //        {
        //            this.m_SimulatePower = value;
        //            NotifyPropertyChanged("InPower");
        //        }
        //    }
        //}

        //#endregion

        //#region 分析参数
        ///// <summary>
        ///// 起始频率
        ///// </summary>
        //private double m_TestFreq;
        ///// <summary>
        ///// 测试频率带宽
        ///// </summary>
        //public double TestFreq
        //{
        //    get { return m_TestFreq; }
        //    set
        //    {
        //        if (value > 0)
        //        {
        //            if (value != m_TestFreq)
        //            {
        //                this.m_TestFreq = value;
        //                NotifyPropertyChanged("TestFreq");
        //            }
        //        }

        //    }
        //}

        //private double m_IngroeFreq;

        //public double IngroeFreq
        //{
        //    get { return m_IngroeFreq; }
        //    set
        //    {
        //        if (value > 0)
        //        {
        //            if (value != m_IngroeFreq)
        //            {
        //                this.m_IngroeFreq = value;
        //                NotifyPropertyChanged("IngroeFreq");
        //            }
        //        }
        //    }
        //}

        //private double m_VBW;

        //public double VBW
        //{
        //    get { return m_VBW; }
        //    set
        //    {
        //        if (value > 0)
        //        {
        //            if (value != m_VBW)
        //            {
        //                this.m_VBW = value;
        //                NotifyPropertyChanged("VBW");
        //            }
        //        }
        //    }
        //}

        //private double m_RBW;

        //public double RBW
        //{
        //    get { return m_RBW; }
        //    set
        //    {
        //        if (value != m_RBW)
        //        {
        //            this.m_RBW = value;
        //            NotifyPropertyChanged("RBW");
        //        }
        //    }
        //}

        //#endregion

        #region 测试项参数

        private ParamterItem m_SGAndPxaParamter = new ParamterItem();

        /// <summary>
        /// 有源激励参数 
        /// </summary>
        public ParamterItem SGAndPxaParamter
        {
            get { return m_SGAndPxaParamter; }
            set { m_SGAndPxaParamter = value; }
        }

        private ParamterItem m_DUTAndPxaParamter = new ParamterItem();

        /// <summary>
        /// 无源激励参数 
        /// </summary>
        public ParamterItem DUTAndPxaParamter
        {
            get { return m_DUTAndPxaParamter; }
            set { m_DUTAndPxaParamter = value; }
        }


        private ParamterItem m_NoiseAndPxaParamter = new ParamterItem();

        /// <summary>
        /// 噪声参数 
        /// </summary>
        public ParamterItem NoiseAndPxaParamter
        {
            get { return m_NoiseAndPxaParamter; }
            set { m_NoiseAndPxaParamter = value; }
        }
        #endregion

        #region 校准文件


        private List<offsetFreq> m_InPutoffst = new List<offsetFreq>();

        [XmlIgnore]
        public List<offsetFreq> InputFileoffst
        {
            set
            {
                m_InPutoffst = value;
            }
            get
            {
                if (m_InPutoffst != null && m_InPutoffst.Count > 0)
                {
                    return m_InPutoffst;
                }
                m_InPutoffst = new List<offsetFreq>();
                List<offsetFreq> InputFileoffstLIst = new List<offsetFreq>();
                string InputFile1 = @"D:\CalFiles\ActiveUpINPUTLossFile.txt";

                TextReader reader = new StreamReader(File.Open(InputFile1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                string text = reader.ReadToEnd();
                reader.Close();
                string[] textArray = text.Split(',');
                for (int i = 0; i < textArray.Length - 2; i = i + 2)
                {
                    offsetFreq offsetFreq1 = new offsetFreq();
                    offsetFreq1.FreqInMHz = double.Parse(textArray[i]);
                    offsetFreq1.ReceiverOffsetFreqInMHz = double.Parse(textArray[i + 1]);
                    InputFileoffstLIst.Add(offsetFreq1);
                }
                m_InPutoffst = InputFileoffstLIst;
                return InputFileoffstLIst;

            }

        }
        private List<offsetFreq> m_OutPutoffst = new List<offsetFreq>();

        [XmlIgnore]
        public List<offsetFreq> OutputFileoffst
        {
            set
            {
                m_OutPutoffst = value;
            }
            get
            {
                if (m_InPutoffst!=null &&  m_OutPutoffst.Count != 0)
                {
                    return m_OutPutoffst;
                }
                //else
                //{
                m_OutPutoffst = new List<offsetFreq>();
                List<offsetFreq> OutputFileoffstLIst = new List<offsetFreq>();
                string OutPutoffst = @"D:\CalFiles\ActiveUpOUTPUTLossFile.txt";

                TextReader reader = new StreamReader(File.Open(OutPutoffst, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                string text = reader.ReadToEnd();
                reader.Close();
                string[] textArray = text.Split(',');
                for (int i = 0; i < textArray.Length - 2; i = i + 2)
                {
                    offsetFreq offsetFreq1 = new offsetFreq();
                    offsetFreq1.FreqInMHz = double.Parse(textArray[i]);
                    offsetFreq1.ReceiverOffsetFreqInMHz = double.Parse(textArray[i + 1]);
                    OutputFileoffstLIst.Add(offsetFreq1);
                }
                m_OutPutoffst = OutputFileoffstLIst;
                return OutputFileoffstLIst;
                //}
            }
        }
        //private string m_InputFile = "INPUTLossFile.txt";
        private string m_InputFile = @"D:\CalFiles\ActiveUpINPUTLossFile.txt";
        public string InputFile
        {
            get { return m_InputFile; }
            set
            {
                if (m_InputFile != value)
                {
                    m_InputFile = value;
                    NotifyPropertyChanged("InputFile");

                }
            }
        }

        //private string m_OutputFile = "OUTPUTLossFile.txt";
        private string m_OutputFile = @"D:\CalFiles\ActiveUpOUTPUTLossFile.txt";
        public string OutputFile
        {
            get { return m_OutputFile; }
            set
            {
                if (m_OutputFile != value)
                {
                    m_OutputFile = value;
                    NotifyPropertyChanged("OutputFile");

                }
            }
        }

        #endregion

    }

    /// <summary>
    /// 测试项参数（有源，无源，噪声）
    /// </summary>
    public class ParamterItem : INotifyPropertyChanged
    {
        #region 构造函数
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string devstate)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(devstate));
            }
        }
        #endregion

        #region 测试参数
        /// <summary>
        /// 起始频率
        /// </summary>
        private double m_StartFreq;

        public double StartFreq
        {
            get { return m_StartFreq; }
            set
            {
                if (value > 0)
                {
                    if (value != m_StartFreq)
                    {
                        this.m_StartFreq = value;
                        NotifyPropertyChanged("StartFreq");
                    }
                }

            }
        }

        private double m_StopFreq;

        public double StopFreq
        {
            get { return m_StopFreq; }
            set
            {
                if (value > 0)
                {
                    if (value != m_StopFreq)
                    {
                        this.m_StopFreq = value;
                        NotifyPropertyChanged("StopFreq");
                    }
                }
            }
        }

        private double m_FreqSpac;

        public double FreqSpac
        {
            get { return m_FreqSpac; }
            set
            {
                if (value > 0)
                {
                    if (value != m_FreqSpac)
                    {
                        this.m_FreqSpac = value;
                        NotifyPropertyChanged("FreqSpac");
                    }
                }
            }
        }

        private double m_SimulatePower;

        public double InPower
        {
            get { return m_SimulatePower; }
            set
            {
                if (value != m_SimulatePower)
                {
                    this.m_SimulatePower = value;
                    NotifyPropertyChanged("InPower");
                }
            }
        }

        #endregion

        #region 分析参数
        /// <summary>
        /// 起始频率
        /// </summary>
        private double m_AnaStartFreq;
        /// <summary>
        /// 测试频率带宽
        /// </summary>
        public double AnaStartFreq
        {
            get { return m_AnaStartFreq; }
            set
            {
                if (value > 0)
                {
                    if (value != m_AnaStartFreq)
                    {
                        this.m_AnaStartFreq = value;
                        NotifyPropertyChanged("AnaStartFreq");
                    }
                }

            }
        }

        /// <summary>
        /// 起始频率
        /// </summary>
        private double m_AnaStopFreq;
        /// <summary>
        /// 测试频率带宽
        /// </summary>
        public double AnaStopFreq
        {
            get { return m_AnaStopFreq; }
            set
            {
                if (value > 0)
                {
                    if (value != m_AnaStopFreq)
                    {
                        this.m_AnaStopFreq = value;
                        NotifyPropertyChanged("AnaStopFreq");
                    }
                }

            }
        }

        private double m_IngroeFreq;

        public double IngroeFreq
        {
            get { return m_IngroeFreq; }
            set
            {
                if (value > 0)
                {
                    if (value != m_IngroeFreq)
                    {
                        this.m_IngroeFreq = value;
                        NotifyPropertyChanged("IngroeFreq");
                    }
                }
            }
        }

        private double m_VBW;

        public double VBW
        {
            get { return m_VBW; }
            set
            {
                if (value > 0)
                {
                    if (value != m_VBW)
                    {
                        this.m_VBW = value;
                        NotifyPropertyChanged("VBW");
                    }
                }
            }
        }

        private double m_RBW;

        public double RBW
        {
            get { return m_RBW; }
            set
            {
                if (value != m_RBW)
                {
                    this.m_RBW = value;
                    NotifyPropertyChanged("RBW");
                }
            }
        }

        #endregion
    }


    public struct offsetFreq
    {
        /// <summary>
        ///测试频率
        /// </summary>
        public double FreqInMHz;
        /// <summary>
        ///测试误差
        /// </summary>
        public double ReceiverOffsetFreqInMHz;


    }
    public enum TestExecutionResult
    {
        Init = 0,
        Complete = 1,
        Error = 2
    }
}
