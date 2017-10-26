using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RackSys.TestLab.Instrument;
using System.Threading;
using System.Windows.Forms;
using RackSys.TestLab.Visa;
using System.ComponentModel;

//版权所有 (C), 2013, , 北京中创锐科信息技术有限公司.
//文件名: SystemHardware.cs
//版本: V1.0 
//日期: 2013-3-10
//描述:
//  RAC-1200TRM 微波T/R组件自动测试系统——系统参数配置窗口
//版本: 
//  2013-01-31 创建



namespace RackSys.TestLab.Hardware
{
    /// <summary>
    /// 硬件系统类：封装所有设备驱动供调用
    /// </summary>
    public class SystemHardware
    {
        /// <summary>
        /// 直流电源映射
        /// </summary>
        private DCChannelConfigMapper m_DCPowerMapper;

        public DCChannelConfigMapper DCPowerMapper
        {
            get { return m_DCPowerMapper; }
            set { m_DCPowerMapper = value; }
        }

        public SystemHardware()
        {
            this.m_SystemCapability = new SystemHardwareCapability();
        }

        private SystemHardwareCapability m_SystemCapability;

        public SystemHardwareCapability SystemCapability
        {
            get { return m_SystemCapability; }
            set { m_SystemCapability = value; }
        }

        /// <summary>
        /// 初始化系统能力
        /// </summary>
        private void InitSysCapability()
        {
            if (this.m_AnalogSignalGenerator != null)
            {
                this.m_SystemCapability.AnalogSGRange.Max_RF_FREQ = this.m_AnalogSignalGenerator.RFFrequencyMax;
                this.m_SystemCapability.AnalogSGRange.Min_RF_FREQ = this.m_AnalogSignalGenerator.RFFrequencyMin;
            }

            if (this.m_VectorSignalGenerator != null)
            {
                this.m_SystemCapability.VectorSGRange.Max_RF_FREQ = this.m_VectorSignalGenerator.RFFrequencyMax;
                this.m_SystemCapability.VectorSGRange.Min_RF_FREQ = this.m_VectorSignalGenerator.RFFrequencyMin;
            }

            if (this.m_SpectrumAnalyzer != null)
            {
                this.m_SystemCapability.PXARange.MaxCenterFrequency = this.m_SpectrumAnalyzer.FrequencyMax;
                this.m_SystemCapability.PXARange.MinCenterFrequency = 3000;
            }

        }



        public DCPowerBase GetDCPowerBaseByChannelID(int inChannelID, out uint outDCChIDInDevice)
        {
            DCChannelConfig tmpChannelInfo = this.m_DCPowerMapper.GetDCPowerChannelInfoByChannelID(inChannelID);
            if (tmpChannelInfo != null)
            {
                outDCChIDInDevice = tmpChannelInfo.ChannelNoInDevice;
                string DeviceName = tmpChannelInfo.DCPowerDeviceName;
                string tmpIpAddress = this.m_CurrentInstruments.GetIpAddressByDeviceName(DeviceName);
                if ((tmpIpAddress != "") && (tmpIpAddress != null))
                {
                    return this.GetDCPowerDeviceByIpAddress(tmpIpAddress);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                outDCChIDInDevice = 0;
                return null;
            }

        }

        private DCPowerBase GetDCPowerDeviceByIpAddress(string inIpAddress)
        {
            foreach (DCPowerBase tmpDCPowerDevice in this.m_DCPowers)
            {
                if (tmpDCPowerDevice != null)
                {
                    if (tmpDCPowerDevice.ResourceName.IndexOf(inIpAddress) >= 0)
                    {
                        return tmpDCPowerDevice;
                    }
                }
            }
            return null;

        }

        // private InstrumentInfoList m_CurrentInstruments = new InstrumentInfoList();
        private InstrumentInfoList m_CurrentInstruments = InstrumentInfoList.getInstence();
        public InstrumentInfoList CurrentInstruments
        {
            get { return m_CurrentInstruments; }
            set { m_CurrentInstruments = value; }
        }

        private static SystemHardware m_SystemHardware;


        public static SystemHardware SysHardware
        {
            get
            {

                if (object.ReferenceEquals(m_SystemHardware, null))
                {
                    SystemHardware temp = new SystemHardware();
                    if (temp.m_CurrentInstruments == null)
                    {
                        temp.m_CurrentInstruments = temp.m_CurrentInstruments.LoadParameterFromXMLFile("");
                    }

                    m_SystemHardware = temp;
                    m_SystemHardware.Connect();

                    ///直流电源映射器对象
                    m_SystemHardware.m_DCPowerMapper = new DCChannelConfigMapper();
                    m_SystemHardware.m_DCPowerMapper = m_SystemHardware.m_DCPowerMapper.LoadFromFile(null);
                }
                return m_SystemHardware;
            }
            set
            {
                m_SystemHardware = value;
                if (value == null)
                {
                    IOManager.RemoveAll();
                }
            }
        }

        /// <summary>
        /// 系统中的频谱分析仪
        /// </summary>
        private SpectrumAnalyzer m_SpectrumAnalyzer;

        /// <summary>
        /// 系统中的频谱仪对象
        /// </summary>
        public SpectrumAnalyzer SpectrumAnalyzer
        {
            get { return m_SpectrumAnalyzer; }
        }

        /// <summary>
        /// 系统中的模拟信号源对象1
        /// </summary>
        private SignalGenerator m_AnalogSignalGenerator;

        /// <summary>
        /// 系统当中的模拟信号源1
        /// </summary>
        public SignalGenerator AnalogSignalGenerator
        {
            get { return m_AnalogSignalGenerator; }
        }

        /// <summary>
        /// 系统中的模拟信号源对象2
        /// </summary>
        private SignalGenerator m_AnalogSignalGenerator2;

        /// <summary>
        /// 系统当中的模拟信号源2
        /// </summary>
        public SignalGenerator AnalogSignalGenerator2
        {
            get { return m_AnalogSignalGenerator2; }
        }

        /// <summary>
        /// 系统当中的矢量信号源
        /// </summary>
        private VectorSignalGenerator m_VectorSignalGenerator;

        private NprSignalGenerator m_NprSignalGenerator;

        public NprSignalGenerator NprSignalGenerator
        {
            get { return m_NprSignalGenerator; }
        }

        /// <summary>
        /// 系统当中的矢量信号源 E4438C
        /// </summary>
        public VectorSignalGenerator VectorSignalGenerator
        {
            get { return m_VectorSignalGenerator; }
        }

        /// <summary>
        /// 机柜内连接输出开关矩阵的功率计
        /// </summary>
        private PowerMeter m_PowerMeterInCabinet;

        /// <summary>
        /// 机柜内连接输出开关矩阵的功率计
        /// </summary>
        public PowerMeter PowerMeterInCabinet
        {
            get { return m_PowerMeterInCabinet; }
        }

        /// <summary>
        /// 机柜外直接连接被测物的功率计
        /// </summary>
        private PowerMeter m_PowerMeterOutofCabinet;

        /// <summary>
        /// 机柜外直接连接被测物的功率计
        /// </summary>
        public PowerMeter PowerMeterOutofCabinet
        {
            get { return m_PowerMeterOutofCabinet; }
        }

        /// <summary>
        /// 频率计
        /// </summary>
        private FrequencyCounter m_FrequencyCounter;

        /// <summary>
        /// 频率计
        /// </summary>
        public FrequencyCounter FrequencyCounter
        {
            get { return m_FrequencyCounter; }
        }

        /// <summary>
        /// 报警器
        /// </summary>
        private Alarm m_Alarm;

        /// <summary>
        /// 报警器
        /// </summary>
        public Alarm Alarm
        {
            get { return m_Alarm; }
        }

        /// <summary>
        /// 中创地检设备
        /// </summary>
        private CSBDevice m_CSBDevice;

        /// <summary>
        /// 中创地检设备
        /// </summary>
        public CSBDevice CSBDevice
        {
            get { return m_CSBDevice; }
        }

        NoiseFigureAnalyzer m_NoiseFigure;
        /// <summary>
        /// 噪声系数分析仪
        /// </summary>
        public NoiseFigureAnalyzer NoiseFigure
        {
            get { return m_NoiseFigure; }
        }

        /// <summary>
        /// 输入开关矩阵
        /// </summary>
        private Matrix m_InputMatrixForHighPower;

        /// <summary>
        /// 输入开关矩阵
        /// </summary>
        public Matrix InputMatrixForHighPower
        {
            get
            {
                return m_InputMatrixForHighPower;
            }
        }


        /// <summary>
        /// 输入开关矩阵
        /// </summary>
        private Matrix m_InputMatrix;

        /// <summary>
        /// 输入开关矩阵
        /// </summary>
        public Matrix InputMatrix
        {
            get { return m_InputMatrix; }
        }

        /// <summary>
        /// 输出开关矩阵
        /// </summary>
        private Matrix m_OutputMatrix;

        /// <summary>
        /// 输出开关矩阵
        /// </summary>
        public Matrix OutputMatrix
        {
            get { return m_OutputMatrix; }
        }

        private PowerAmplefierBase m_PowerAmplefier;

        public PowerAmplefierBase PowerAmplifier
        {
            get { return m_PowerAmplefier; }
        }

        private Incubator m_Incubator;

        /// <summary>
        /// 温箱
        /// </summary>
        public Incubator Incubator
        {
            get { return m_Incubator; }
            set { m_Incubator = value; }
        }

        private TemperatureMonitorBase m_TemperatureMonitor;
        /// <summary>
        /// 温度巡检仪
        /// </summary>
        public TemperatureMonitorBase TemperatureMonitorInstance
        {
            get { return m_TemperatureMonitor; }
            set { m_TemperatureMonitor = value; }
        }


        /// <summary>
        /// 程控电源设备列表（多台程控电源）
        /// </summary>
        private DCPowerBase[] m_DCPowers;

        /// <summary>
        /// 程控电源设备列表（多台程控电源）
        /// </summary>
        public DCPowerBase[] DCPowers
        {
            get { return m_DCPowers; }
        }

        /// <summary>
        /// 多通道控制器
        /// </summary>
        private MultiChannelController m_MultiChannelController;

        /// <summary>
        /// 多通道多功能控制器
        /// </summary>
        public MultiChannelController MultiChannelController
        {
            get { return m_MultiChannelController; }
        }

        /// <summary>
        /// 多通道数据采集
        /// </summary>
        private MutiChannelDataAcquisition m_MutiChannelDataAcquisition;


        /// <summary>
        /// 多通道数据采集
        /// </summary>
        public MutiChannelDataAcquisition MutiChannelDataAcquisition
        {
            get { return m_MutiChannelDataAcquisition; }
        }

        private AgilentPNA835x.Application m_PNAApp;

        public AgilentPNA835x.Application PNA
        {
            get { return m_PNAApp; }
        }

        AgilentPNA835x.ScpiStringParser m_PNASCPIParser;

        NetworkAnalyzer m_PNAScpiObj;

        public NetworkAnalyzer PNAScpiInstrument
        {
            get { return m_PNAScpiObj; }
        }
        private delegate void Connect_Delegate(
             SystemHardware inHardware);


        public void Connect()
        {
            //tmpForm = new ProgressForm();
            //Action task = () => 
            //{ 
            //    this.ConnectInner();
            //};
            //IAsyncResult ConnectResult = task.BeginInvoke(null, null);
            ////Connect_Delegate task = ConnectInner_Static;
            //IAsyncResult ConnectResult = task.BeginInvoke(this, null, null);
            //task.EndInvoke(ConnectResult);

            this.ConnectInner();
        }

        /// <summary>
        /// 系统连接过程
        /// </summary>
        public void ConnectInner()
        {
            ProgressShower tmpTaskShower = ProgressShower.CurrentShower;
            tmpTaskShower.ShowMe();
            try
            {
                List<DCPowerBase> tmpDCPowers = new List<DCPowerBase>();

                int tmpCount = this.m_CurrentInstruments.Count;
                int tmpIndex = 0;
                tmpTaskShower.SetTaskAndProgress("开始连接测试系统硬件", 0);

                IAsyncResult[] tmpTasksResult = new IAsyncResult[tmpCount];
                Action<List<DCPowerBase>, InstrumentInfo>[] tmpActions = new Action<List<DCPowerBase>, InstrumentInfo>[tmpCount];
                foreach (InstrumentInfo tmpInstrumentInfo in this.m_CurrentInstruments)
                {
                    //tmpTaskShower.SetTaskAndProgress("开始连接" + tmpInstrumentInfo.InstrumentName + "......", tmpIndex * 100 / tmpCount);
                    tmpTaskShower.SetTaskAndProgress("开始连接" + tmpInstrumentInfo.InstrumentName + "......", 2);
                    if (tmpInstrumentInfo.Enabled)
                    {
                        Action<List<DCPowerBase>, InstrumentInfo> task = (inDCPowers, inInstrumentInfo) =>
                        {
                            this.CreateInstrumentCtrlObj(inDCPowers, inInstrumentInfo);
                            //this.ConnectInner();
                        };

                        //task.Invoke(tmpDCPowers, tmpInstrumentInfo);
                        tmpTasksResult[tmpIndex] = task.BeginInvoke(tmpDCPowers, tmpInstrumentInfo, null, null);
                        //tmpTasksResult[tmpIndex] = null;
                        //task.Invoke(tmpDCPowers, tmpInstrumentInfo);
                        tmpActions[tmpIndex] = task;
                        //this.CreateInstrumentCtrlObj(tmpDCPowers, tmpInstrumentInfo);
                    }
                    else
                    {
                        tmpTasksResult[tmpIndex] = null;
                        tmpActions[tmpIndex] = null;
                    }
                    tmpIndex++;
                    //tmpTaskShower.SetTaskAndProgress("仪表" + tmpInstrumentInfo.InstrumentName + "连接完毕", tmpIndex * 100 / tmpCount);
                }

                //等待测试结束
                for (int i = 0; i < tmpCount; i++)
                {
                    InstrumentInfo tmpInstrumentInfo = this.m_CurrentInstruments[i];
                    if (tmpActions[i] != null)
                    {
                        tmpTaskShower.SetTaskAndProgress("等待仪表" + tmpInstrumentInfo.InstrumentName + "结束连接", i * 100 / tmpCount);
                        tmpActions[i].EndInvoke(tmpTasksResult[i]);
                        tmpTaskShower.SetTaskAndProgress("仪表" + tmpInstrumentInfo.InstrumentName + "连接完毕", i * 100 / tmpCount);
                    }
                }


                tmpTaskShower.SetTaskComplete(null, null);

                this.m_DCPowers = tmpDCPowers.ToArray();

                ///初始化系统能力
                this.InitSysCapability();
            }
            finally
            {
                tmpTaskShower.HideMe();
                GC.Collect();
            }
        }

        /// <summary>
        /// 仪表识别
        /// </summary>
        /// <param name="inInstrumentAddress">仪表地址</param>
        /// <param name="outConnected"> 是否连接</param>
        /// <returns></returns>
        public string TryIdentifyInstrument(string inInstrumentAddress, out bool outConnected)
        {
            outConnected = true;
            try
            {
                ScpiInstrument tmpInstr = new ScpiInstrument(inInstrumentAddress, true);
                return tmpInstr.Identity;
            }
            catch (Exception ex)
            {
                outConnected = false;
                return "";
            }

        }
        private double m_RFPowerLimitMax;

        public double RFPowerLimitMax
        {
            get { return m_RFPowerLimitMax; }
            set { m_RFPowerLimitMax = value; }
        }

        public void Set_SG_RFPowerLimitMax(double inRFPower)
        {
            if (this.m_VectorSignalGenerator != null)
            {
                this.m_VectorSignalGenerator.RFPowerLimitMax = inRFPower;
            }

            if (this.m_AnalogSignalGenerator != null)
            {
                this.m_AnalogSignalGenerator.RFPowerLimitMax = inRFPower;
            }
        }

        /// <summary>
        /// 关闭会话
        /// </summary>
        /// <param name="inAddress"></param>
        /// <returns></returns>
        public bool TryConnect(string inAddress)
        {
            try
            {

                InstrumentIOSession tmpIO = IOManager.ProvideDirectIO(inAddress);
                if (tmpIO.IsConnected)
                {
                    tmpIO.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 单个仪表连接
        /// </summary>
        /// <param name="inToDetectDeviceType"></param>
        public void CreateInstrumentInstanceByInstrumentType(InstrumentType inToDetectDeviceType)
        {
            ProgressShower tmpShower = ProgressShower.CurrentShower;
            tmpShower.ShowMe();
            try
            {
                int tmpIndex = 0;
                int tmpCount = this.m_CurrentInstruments.Count;

                IAsyncResult[] tmpTasksResult = new IAsyncResult[tmpCount];
                Action<InstrumentInfo>[] tmpActions = new Action<InstrumentInfo>[tmpCount];

                List<DCPowerBase> tmpDCPowers = new List<DCPowerBase>();
                foreach (InstrumentInfo tmpInstrumentInfo in this.m_CurrentInstruments)
                {
                    //tmpShower.SetTaskAndProgress("正在巡检" + tmpInstrumentInfo.InstrumentName + "......，地址：" + tmpInstrumentInfo.IpAddress, tmpIndex * 100 / tmpCount);
                    if (tmpInstrumentInfo.Enabled)
                    {
                        if (inToDetectDeviceType == tmpInstrumentInfo.InstrumentTypeID)
                        {
                            tmpShower.SetTaskAndProgress("正在巡检" + tmpInstrumentInfo.InstrumentName + "......，地址：" + tmpInstrumentInfo.IpAddress, 0);
                            Action<InstrumentInfo> task = (inInstrumentInfo) =>
                            {
                                CreateInstrumentCtrlObj(tmpDCPowers, inInstrumentInfo);
                            };
                            tmpTasksResult[tmpIndex] = task.BeginInvoke(tmpInstrumentInfo, null, null);
                            tmpActions[tmpIndex] = task;
                        }
                    }

                    tmpIndex++;
                    //tmpShower.SetTaskAndProgress("巡检" + tmpInstrumentInfo.InstrumentName + "......完毕", tmpIndex * 100 / tmpCount);
                }
                for (int i = 0; i < tmpCount; i++)
                {
                    InstrumentInfo tmpInstrumentInfo = this.m_CurrentInstruments[i];
                    if (tmpActions[i] != null)
                    {
                        tmpShower.SetTaskAndProgress("等待仪表" + tmpInstrumentInfo.InstrumentName + "结束巡检，地址：" + tmpInstrumentInfo.IpAddress, i * 100 / tmpCount);
                        tmpActions[i].EndInvoke(tmpTasksResult[i]);
                        tmpShower.SetTaskAndProgress("仪表" + tmpInstrumentInfo.InstrumentName + "连接完毕", i * 100 / tmpCount);
                    }
                }

                if ((inToDetectDeviceType == InstrumentType.DCPower) || (inToDetectDeviceType == InstrumentType.DCPowerAnalyzer))
                {
                    this.m_DCPowers = tmpDCPowers.ToArray();
                }

                this.InitSysCapability();


                //List<string> tmpIpAddressList = new List<string>();
                //string tmpIpAddress = tmpDCPowers[0].ResourceName;
                //foreach(DCPowerBase APowerBase in tmpDCPowers)
                //{
                //    tmpIpAddressList.Add(APowerBase.ResourceName);
                //}

                //int index = 0;
                //foreach(DCPowerBase APowerBase in this.m_DCPowers)
                //{
                //    if (APowerBase != null)
                //    {
                //        if (tmpIpAddressList.Contains(tmpIpAddress))
                //        {
                //            this.m_DCPowers[index] = tmpDCPowers[0];
                //            break;

                //        }
                //    }
                //    index++;
                //}

                //if (tmpDCPowers.Count() > 0)
                //{
                //    this.m_DCPowers = tmpDCPowers.ToArray();
                //    this.m_DCPowers.
                //}

            }
            finally
            {
                tmpShower.HideMe();
                GC.Collect();
            }
        }


        /// <summary>
        /// 仪器巡检：仅巡检部分设备，不去更新控制对象。
        /// </summary>
        /// <param name="inToDetectDeviceType"></param>
        public void InstrumentDetect(InstrumentType[] inToDetectDeviceType)
        {
            ProgressShower tmpShower = ProgressShower.CurrentShower;
            tmpShower.ShowMe();
            try
            {
                int tmpIndex = 0;
                int tmpCount = this.m_CurrentInstruments.Count;

                IAsyncResult[] tmpTasksResult = new IAsyncResult[tmpCount];
                //(WWG 加入ISOPT)
                Action<InstrumentInfo>[] tmpActions = new Action<InstrumentInfo>[tmpCount];

                foreach (InstrumentInfo tmpInstrumentInfo in this.m_CurrentInstruments)
                {
                    tmpShower.SetTaskAndProgress("正在巡检" + tmpInstrumentInfo.InstrumentName + "......，地址：" + tmpInstrumentInfo.IpAddress, 0);
                    //tmpShower.SetTaskAndProgress("正在巡检" + tmpInstrumentInfo.InstrumentName + "......，地址：" + tmpInstrumentInfo.IpAddress, tmpIndex * 100 / tmpCount);
                    if (tmpInstrumentInfo.Enabled)
                    {

                        if (inToDetectDeviceType.Contains(tmpInstrumentInfo.InstrumentTypeID))
                        {
                            Action<InstrumentInfo> task = (inInstrumentInfo) =>
                            {
                                UpdateInstrumentInfo(inInstrumentInfo);
                            };
                            tmpTasksResult[tmpIndex] = task.BeginInvoke(tmpInstrumentInfo, null, null);
                            tmpActions[tmpIndex] = task;
                        }
                    }

                    tmpIndex++;
                    //tmpShower.SetTaskAndProgress("巡检" + tmpInstrumentInfo.InstrumentName + "......完毕", tmpIndex * 100 / tmpCount);
                }
                for (int i = 0; i < tmpCount; i++)
                {
                    InstrumentInfo tmpInstrumentInfo = this.m_CurrentInstruments[i];
                    if (tmpActions[i] != null)
                    {
                        tmpShower.SetTaskAndProgress("等待仪表" + tmpInstrumentInfo.InstrumentName + "结束巡检，地址：" + tmpInstrumentInfo.IpAddress, i * 100 / tmpCount);
                        tmpActions[i].EndInvoke(tmpTasksResult[i]);
                        tmpShower.SetTaskAndProgress("仪表" + tmpInstrumentInfo.InstrumentName + "连接完毕", i * 100 / tmpCount);
                    }
                }

            }
            finally
            {
                tmpShower.HideMe();
                GC.Collect();
            }
        }

        //public void InstrumentDetect()
        //{
        //    ProgressShower tmpShower = ProgressShower.CurrentShower;
        //    tmpShower.ShowMe();
        //    try
        //    {
        //        List<DCPowerBase> tmpDCPowers = new List<DCPowerBase>();
        //        int tmpIndex = 0;
        //        int tmpCount = this.m_CurrentInstruments.Count;
        //        foreach (InstrumentInfo tmpInstrumentInfo in this.m_CurrentInstruments)
        //        {
        //            tmpShower.SetTaskAndProgress("正在巡检" + tmpInstrumentInfo.InstrumentName + "......，地址："+ tmpInstrumentInfo.IpAddress, tmpIndex * 100 / tmpCount);
        //            if (tmpInstrumentInfo.Enabled)
        //            {
        //                UpdateInstrumentInfo(tmpInstrumentInfo);
        //                if (tmpInstrumentInfo.ConnectOK != devState.connect)
        //                {
        //                    continue;
        //                }

        //                CreateInstrumentCtrlObj(tmpDCPowers, tmpInstrumentInfo);
        //            }
        //            tmpIndex++;
        //            tmpShower.SetTaskAndProgress("巡检" + tmpInstrumentInfo.InstrumentName + "......完毕" , tmpIndex * 100 / tmpCount);

        //        }

        //        this.m_DCPowers = tmpDCPowers.ToArray();
        //    }
        //    finally
        //    {
        //        tmpShower.HideMe();
        //    }
        //}

        private enum LicenseCheckingResult
        {
            PNAPassed,
            PNAFailed,
            SGPassed,
            SGFailed
        }


        /// <summary>
        /// 检查矢网连接选件
        /// </summary>
        /// <returns></returns>
        private LicenseCheckingResult Check_Pna_Connectivity_Licenses(string inModelNumber, string inSerialNumber)
        {
            try
            {
                Rac1200_PNAConnectivityChecker checker = new Rac1200_PNAConnectivityChecker();
                bool flag2 = checker.Verify(
                    inModelNumber,
                    inSerialNumber);

                if (flag2)
                {
                    return LicenseCheckingResult.PNAPassed;
                }
                else
                {
                    return LicenseCheckingResult.PNAFailed;
                }
            }
            catch (System.Exception e)
            {
                return LicenseCheckingResult.PNAFailed;
            }
        }

        /// <summary>
        /// 检查矢网连接选件
        /// </summary>
        /// <returns></returns>
        private LicenseCheckingResult Check_SG_Connectivity_Licenses(string inModelNumber, string inSerialNumber)
        {
            try
            {
                Rac1200_SGConnectivityChecker checker = new Rac1200_SGConnectivityChecker();
                bool flag2 = checker.Verify(
                    inModelNumber,
                    inSerialNumber);

                if (flag2)
                {
                    return LicenseCheckingResult.SGPassed;
                }
                else
                {
                    return LicenseCheckingResult.SGFailed;
                }
            }
            catch (System.Exception e)
            {
                return LicenseCheckingResult.SGFailed;
            }
        }

        /// <summary>
        /// 更新仪表控制对象
        /// </summary>
        /// <param name="tmpDCPowers"></param>
        /// <param name="tmpInstrumentInfo"></param>
        private void CreateInstrumentCtrlObj(List<DCPowerBase> tmpDCPowers, InstrumentInfo tmpInstrumentInfo)
        {

            GlobalStatusReport.Report(string.Format("UpdateInstrumentInfo 仪表名称：{0} 地址：{1},开始连接", tmpInstrumentInfo.InstrumentName, tmpInstrumentInfo.IpAddress));
            switch (tmpInstrumentInfo.InstrumentTypeID)
            {
                case InstrumentType.Pna:
                    try
                    {
                        //置为巡检状态
                        tmpInstrumentInfo.DevInfoState = devState.ischeck;

                        this.m_PNAScpiObj = NetworkAnalyzer.Connect(tmpInstrumentInfo.IpAddress, null
                            , false);

                        if (this.m_PNAScpiObj != null)
                        {
                            //判断对应的连接License选件是否有，如果有，则继续，没有，报告错误；
                            LicenseCheckingResult tmpResult = this.Check_Pna_Connectivity_Licenses(this.m_PNAScpiObj.Model, this.m_PNAScpiObj.SerialNumber);

                            ///
                            tmpResult = LicenseCheckingResult.PNAPassed;

                            ///
                            if (tmpResult != LicenseCheckingResult.PNAPassed)
                            {
                                ///设置异常信息
                                this.SetInstrumentErrorInfo(
                                    tmpInstrumentInfo,
                                    string.Format("没有找到矢网{0}-{1}连接选件RAC1200-001",
                                    new object[] { this.m_PNAScpiObj.Model, this.m_PNAScpiObj.SerialNumber }));

                                this.m_PNAScpiObj = null;
                                this.m_PNAApp = null;
                                this.m_PNASCPIParser = null;

                                throw new Exception(string.Format("没有找到矢网{0}-{1}的连接选件RAC1200-001",
                                    new object[] { this.m_PNAScpiObj.Model, this.m_PNAScpiObj.SerialNumber }));
                            }
                            else
                            {
                                ///创建PNA对象
                                string tmpPnaIpAddr;
                                int port = 5025;
                                VisaServices.GetLanConnectionInfo(tmpInstrumentInfo.IpAddress, out tmpPnaIpAddr, out port);
                                Type t = Type.GetTypeFromProgID("AgilentPNA835x.Application", tmpPnaIpAddr, true);
                                m_PNAApp = (AgilentPNA835x.Application)Activator.CreateInstance(t);
                                m_PNASCPIParser = (AgilentPNA835x.ScpiStringParser)m_PNAApp.ScpiStringParser;


                                UpdateInstrumentIdentity(tmpInstrumentInfo, this.m_PNAScpiObj);
                            }

                            //设置PNASource mathmatical on/off
                            if (PowerMode.NormalPower == ProjectName.CurrentPowerMode)
                            {
                                this.m_PNAScpiObj.SendSCPI("system:preferences:item:offset:src ON");
                            }
                            else if (PowerMode.HighPower == ProjectName.CurrentPowerMode)
                            {
                                this.m_PNAScpiObj.SendSCPI("system:preferences:item:offset:src OFF");
                            }
                            else
                            {

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        this.SetInstrumentErrorInfo(tmpInstrumentInfo, /*"连接错误，异常信息："+*/ ex.Message);
                        //置为错误状态
                        tmpInstrumentInfo.DevInfoState = devState.error;
                    }
                    break;
                case InstrumentType.InputMatrix:
                    try
                    {
                        //置为巡检状态
                        tmpInstrumentInfo.DevInfoState = devState.ischeck;

                        this.m_InputMatrix = Matrix.Connect(tmpInstrumentInfo.IpAddress, true, false);

                        UpdateInstrumentIdentity(tmpInstrumentInfo, m_InputMatrix);
                    }
                    catch (Exception ex)
                    {
                        this.SetInstrumentErrorInfo(tmpInstrumentInfo, /*"连接错误，异常信息："+*/ ex.Message);
                        //置为错误状态
                        tmpInstrumentInfo.DevInfoState = devState.error;
                    }                 
                    break;

                case InstrumentType.OutputMatrix:                   
                    try
                    {
                        //置为巡检状态
                        tmpInstrumentInfo.DevInfoState = devState.ischeck;
                        this.m_OutputMatrix = Matrix.Connect(tmpInstrumentInfo.IpAddress, false, false);
                        //this.m_OutputMatrix = Matrix.Connect(tmpInstrumentInfo.IpAddress, false);
                        UpdateInstrumentIdentity(tmpInstrumentInfo, this.m_OutputMatrix);
                    }
                    catch (Exception ex)
                    {
                        this.SetInstrumentErrorInfo(tmpInstrumentInfo, /*"连接错误，异常信息："+*/ ex.Message);
                        //置为错误状态
                        tmpInstrumentInfo.DevInfoState = devState.error;
                    }
                    break;  

            }
            GlobalStatusReport.Report(string.Format("UpdateInstrumentInfo 仪表名称：{0} 地址：{1},连接结束", tmpInstrumentInfo.InstrumentName, tmpInstrumentInfo.IpAddress));

        }

        /// <summary>
        /// 更新仪器标识信息
        /// </summary>
        /// <param name="inInstrumentInfo"></param>
        /// <param name="inInstrumentObj"></param>
        private void UpdateInstrumentIdentity(InstrumentInfo inInstrumentInfo, InstrumentBase inInstrumentObj)
        {
            if (inInstrumentObj == null)
            {
                inInstrumentInfo.ModelNo = "";
                inInstrumentInfo.Identity = "";
                inInstrumentInfo.SN = "";
                inInstrumentInfo.OptionList = "";
                inInstrumentInfo.ConnectOK = devState.error;
                inInstrumentInfo.DevInfoState = devState.error;
                inInstrumentInfo.ErrorMsgs = "未连接";
            }
            else
            {
                inInstrumentInfo.ModelNo = inInstrumentObj.Model;
                inInstrumentInfo.Identity = inInstrumentObj.Identity;
                inInstrumentInfo.SN = inInstrumentObj.SerialNumber;
                inInstrumentInfo.OptionList = inInstrumentObj.Options;

                inInstrumentInfo.Manufacter = inInstrumentObj.Manufactor;

                inInstrumentInfo.ConnectOK = devState.connect;
                inInstrumentInfo.DevInfoState = devState.connect;
                inInstrumentInfo.ErrorMsgs = "";
            }
        }

        private void SetInstrumentErrorInfo(InstrumentInfo inInstrumentInfo, string inErrorMsg)
        {
            inInstrumentInfo.ModelNo = "";
            inInstrumentInfo.Manufacter = "";
            inInstrumentInfo.Identity = "";
            inInstrumentInfo.SN = "";
            inInstrumentInfo.OptionList = "";
            inInstrumentInfo.ConnectOK = devState.error;
            inInstrumentInfo.ErrorMsgs = inErrorMsg;
        }

        /// <summary>
        /// 更新仪表控制对象
        /// </summary>
        /// <param name="tmpDCPowers"></param>
        /// <param name="tmpInstrumentInfo"></param>
        private void UpdateInstrumentCtrlObjIfNeeded(List<DCPowerBase> tmpDCPowers, InstrumentInfo tmpInstrumentInfo)
        {
            switch (tmpInstrumentInfo.InstrumentTypeID)
            {
                case InstrumentType.Pna:
                    try
                    {
                        bool bNeedUpdateCtrlObj = IfNeedUpdateCtrlObj(tmpInstrumentInfo.IpAddress, this.m_PNAScpiObj);
                        if (bNeedUpdateCtrlObj)
                        {
                            ///创建PNA对象
                            string tmpPnaIpAddr;
                            int port = 5025;
                            VisaServices.GetLanConnectionInfo(tmpInstrumentInfo.IpAddress, out tmpPnaIpAddr, out port);
                            Type t = Type.GetTypeFromProgID("AgilentPNA835x.Application", tmpPnaIpAddr, true);
                            m_PNAApp = (AgilentPNA835x.Application)Activator.CreateInstance(t);
                            m_PNASCPIParser = (AgilentPNA835x.ScpiStringParser)m_PNAApp.ScpiStringParser;

                            this.m_PNAScpiObj = NetworkAnalyzer.Connect(tmpInstrumentInfo.IpAddress, null
                                , false);
                        }

                    }
                    catch (Exception ex)
                    {
                        tmpInstrumentInfo.ConnectOK = devState.error;
                        tmpInstrumentInfo.ErrorMsgs = "型号不匹配";
                    }
                    break;                                
                case InstrumentType.InputMatrix:
                    try
                    {
                        bool bNeedUpdateCtrlObj = IfNeedUpdateCtrlObj(tmpInstrumentInfo.IpAddress, this.m_InputMatrix);
                        if (bNeedUpdateCtrlObj)
                        {
                            this.m_InputMatrix = Matrix.Connect(tmpInstrumentInfo.IpAddress, true, false);
                            //this.m_InputMatrixForHighPower = Matrix.Connect(tmpInstrumentInfo.IpAddress, true, true);
                            //this.m_InputMatrix = Matrix.Connect(tmpInstrumentInfo.IpAddress, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        tmpInstrumentInfo.ConnectOK = devState.error;
                        tmpInstrumentInfo.ErrorMsgs = "型号不匹配";
                    }
                    try
                    {
                        bool bNeedUpdateCtrlObj = IfNeedUpdateCtrlObj(tmpInstrumentInfo.IpAddress, this.m_OutputMatrix);
                        if (bNeedUpdateCtrlObj)
                        {
                            this.m_OutputMatrix = Matrix.Connect(tmpInstrumentInfo.IpAddress, false, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        tmpInstrumentInfo.ConnectOK = devState.error;
                        tmpInstrumentInfo.ErrorMsgs = "型号不匹配";
                    }
                    break;                

            }
        }

        private static bool IfNeedUpdateCtrlObj(string tmpNewAddress, ScpiInstrument tmpCtrlObj)
        {
            bool bNeedUpdateCtrlObj = (tmpCtrlObj == null) || (tmpCtrlObj.ResourceName != tmpNewAddress);
            return bNeedUpdateCtrlObj;
        }


        /// <summary>
        /// 更新设备信息
        /// </summary>
        /// <param name="tmpInstrumentInfo"></param>
        private static void UpdateInstrumentInfo(InstrumentInfo tmpInstrumentInfo)
        {
            try
            {
                GlobalStatusReport.Report(string.Format("UpdateInstrumentInfo 仪表名称：{0} 地址：{1},开始连接", tmpInstrumentInfo.InstrumentName, tmpInstrumentInfo.IpAddress));
                tmpInstrumentInfo.DevInfoState = devState.ischeck;
                string Identity = "";
                string ModelNo = "";
                string InstSN = "";
                string Firmware = "";
                string Options = "";
                string Manufactor = "";
                ModelNo = ScpiInstrument.DetermineInstrumentInfo(tmpInstrumentInfo.IpAddress, out Identity, out InstSN, out Options, out Firmware, out Manufactor);
                if ((ModelNo == null) || (ModelNo == string.Empty))
                {
                    tmpInstrumentInfo.ConnectOK = devState.error;
                    tmpInstrumentInfo.DevInfoState = devState.error;
#if Simulate
                    tmpInstrumentInfo.ConnectOK = devState.connect;
                    tmpInstrumentInfo.DevInfoState = devState.connect;
#endif
                }
                else
                {
                    tmpInstrumentInfo.ConnectOK = devState.connect;
                    tmpInstrumentInfo.DevInfoState = devState.connect;

                    tmpInstrumentInfo.ModelNo = ModelNo;
                    tmpInstrumentInfo.Identity = Identity;
                    tmpInstrumentInfo.SN = InstSN;
                    tmpInstrumentInfo.OptionList = Options;
                    tmpInstrumentInfo.Manufacter = Manufactor;
                }
                GlobalStatusReport.Report(string.Format("UpdateInstrumentInfo 仪表名称：{0} 地址：{1},连接结束!", tmpInstrumentInfo.InstrumentName, tmpInstrumentInfo.IpAddress));
            }
            catch (Exception ex)
            {
                tmpInstrumentInfo.ConnectOK = devState.error;
            }
        }

    }
}
