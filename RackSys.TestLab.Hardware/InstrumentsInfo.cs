using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using RackSys.TestLab.DataAccess;
using System.Xml.Serialization;
using System.ComponentModel;

namespace RackSys.TestLab.Hardware
{
    /// <summary>
    /// 设备类型
    /// </summary>
    public enum InstrumentType
    {
        /// <summary>
        /// 矢量信号源
        /// </summary>
        VectorSg,
        /// <summary>
        /// 模拟信号源
        /// </summary>
        AnalogSg,
        /// <summary>
        /// 模拟信号源
        /// </summary>
        AnalogSg2,
        /// <summary>
        /// 网络分析仪
        /// </summary>
        Pna,
        /// <summary>
        /// 频谱分析仪
        /// </summary>
        SA,
        /// <summary>
        /// 功率计（机柜内）
        /// </summary>
        PowerMeterInCabinet,
        /// <summary>
        /// 功率计（机柜外）
        /// </summary>
        PowerMeterOutofCabinet,
        /// <summary>
        /// 输入开关矩阵
        /// </summary>
        InputMatrix,
        /// <summary>
        /// 输出开关矩阵
        /// </summary>
        OutputMatrix,
        /// <summary>
        /// 电源
        /// </summary>
        DCPower,
        /// <summary>
        /// 电源的组合（用于界面上的显示）
        /// </summary>
        DCPowerList,
        /// <summary>
        /// 多通道控制器
        /// </summary>
        MultiChannelController,
        /// <summary>
        /// 多路数据采集单元
        /// </summary>
        MultiChannelAcquistitor,

        /// <summary>
        /// 高功率放大器
        /// </summary>
        HighPowerDriver,
        /// <summary>
        /// 
        /// </summary>
        DCPowerAnalyzer,

        /// <summary>
        /// 噪声源
        /// </summary>
        NoiceSource,
        /// <summary>
        /// 温箱
        /// </summary>
        Incubator,
        /// <summary>
        /// 温度巡检仪
        /// </summary>
        TemperatureMonitor,
        /// <summary>
        /// 频率计（计数器）
        /// </summary>
        FrequencyCounter,

        /// <summary>
        /// 报警器
        /// </summary>
        Alarm,

        /// <summary>
        /// CSB设备
        /// </summary>
        CSBDevice,

        /// <summary>
        /// 噪声系数分析仪
        /// </summary>
        NoiseFigure,
        /// <summary>
        /// 被测物
        /// </summary>
         DUT
    }

    /// <summary>
    /// 矢量网络分析仪类型
    /// </summary>
    public enum PnaCalType
    {
        /// <summary>
        /// 夹具校准
        /// </summary>
        TRLCal,
        /// <summary>
        /// 同轴校准
        /// </summary>
        CoaxialCal,
        no
    }

    /// <summary>
    /// 矢量网络分析仪同轴校准类型
    /// </summary>
    public enum CoaxialCal
    {
        /// <summary>
        /// 源功率
        /// </summary>
        powCal,
        /// <summary>
        /// S参数校准
        /// </summary>
        SParaCal,
        no

    }

    /// <summary>
    /// 信号源
    /// </summary>
    public enum SingnalSource
    {
        AnalogSg,
        VectorSg,
        no
    }

    /// <summary>
    /// 设备状态
    /// </summary>
    public enum devState
    {
        /// <summary>
        /// 巡检状态
        /// </summary>
        ischeck,

        /// <summary>
        /// 正常状态
        /// </summary>
        normal,

        /// <summary>
        /// 警告状态
        /// </summary>
        alart,

        /// <summary>
        /// 已连接状态
        /// </summary>
        connect,

        /// <summary>
        /// 出错状态
        /// </summary>
        error,

        /// <summary>
        /// 不可用状态
        /// </summary>
        disenable
    }

    /// <summary>
    /// 地址类形
    /// </summary>
    public enum addrType
    {
        LAN,
        GPIB,
        IOAddr,
        Other
    }

    /// <summary>
    /// 仪表信息内容
    /// </summary>
    public class InstrumentInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string devstate)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(devstate));
            }
        }

        private InstrumentType m_InstrumentType;

        public InstrumentType InstrumentTypeID
        {
            get { return m_InstrumentType; }
            set
            {
                m_InstrumentType = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("InstrumentTypeID");
                }
            }
        }

        public InstrumentInfo Clone()
        {
            InstrumentInfo Result = new InstrumentInfo()
            {
                ConnectOK = this.ConnectOK,
                IDInInstitute = this.IDInInstitute,
                Identity = this.Identity,
                DevInfoState = this.DevInfoState,
                Enabled = this.Enabled,
                ErrorMsgs = this.ErrorMsgs,
                InstrumentName = this.InstrumentName,
                InstrumentTypeID = this.InstrumentTypeID,
                IpAddress = this.IpAddress,
                Manufacter = this.Manufacter,
                ModelNo = this.ModelNo,
                OptionList = this.OptionList,
                SN = this.SN,
            };

            return Result;
        }

        /// <summary>
        /// 需要直接引用Hardware部分完成相关数据的提取
        /// </summary>
        [XmlIgnore]
        public bool Calibrated
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// 是否启用该设备
        /// </summary>
        private bool m_Enabled = true;
        [XmlElement("是否启用设备")]
        public bool Enabled
        {
            get { return m_Enabled; }
            set
            {
                m_Enabled = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("Enabled");
                }
            }
        }

        private string m_IDInInstitute;

        /// <summary>
        /// 所内编号
        /// </summary>
        [XmlElement("设备所内编号")]
        public string IDInInstitute
        {
            get { return m_IDInInstitute; }
            set
            {
                m_IDInInstitute = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("IDInInstitute");
                }
            }
        }

        private string m_InstrumentName;

        /// <summary>
        /// 仪器名称
        /// </summary>
        [XmlElement("设备名称")]
        public string InstrumentName
        {
            get { return m_InstrumentName; }
            set
            {
                m_InstrumentName = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("InstrumentName");
                }
            }
        }

        private string m_ModelNo;

        /// <summary>
        /// 型号
        /// </summary>
        [XmlElement("型号")]
        public string ModelNo
        {
            get { return m_ModelNo; }
            set
            {
                m_ModelNo = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("ModelNo");
                }
            }
        }

        private string m_SN;

        /// <summary>
        /// 序号
        /// </summary>
        [XmlElement("序号")]
        public string SN
        {
            get { return m_SN; }
            set
            {
                m_SN = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("SN");
                }
            }
        }

        private string m_OptionList;

        /// <summary>
        /// 选件列表
        /// </summary>
        [XmlElement("选件")]
        public string OptionList
        {
            get { return m_OptionList; }
            set
            {
                m_OptionList = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("OptionList");
                }
            }
        }

        private string m_Manufacter;

        /// <summary>
        /// 制造商
        /// </summary>
        [XmlElement("厂家")]
        public string Manufacter
        {
            get { return m_Manufacter; }
            set
            {
                m_Manufacter = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("Manufacter");
                }
            }
        }


        private devState m_ConnectOK;

        /// <summary>
        /// 连接状态
        /// </summary>
        [XmlIgnore()]
        public devState ConnectOK
        {
            get { return m_ConnectOK; }
            set
            {
                m_ConnectOK = value;

                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("ConnectOK");
                }
            }
        }

        private string m_IpAddress;

        /// <summary>
        /// ip地址
        /// </summary>
        [XmlElement("IP地址")]
        public string IpAddress
        {
            get { return m_IpAddress; }
            set
            {
                m_IpAddress = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("IpAddress");
                }
            }

        }

        private string m_ErrorMsgs;

        /// <summary>
        /// 错误信息
        /// </summary>
        [XmlIgnore()]
        public string ErrorMsgs
        {
            get { return m_ErrorMsgs; }
            set
            {
                m_ErrorMsgs = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("ErrorMsgs");
                    NotifyPropertyChanged("CommentInfo");
                }
            }
        }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string CommentInfo
        {
            get
            {
                if (string.IsNullOrEmpty(this.m_Identity))
                {
                    return this.ErrorMsgs;
                }
                else
                {
                    return this.Identity;
                }
            }

        }

        /// <summary>
        /// 标识信息：例如：Agilent;E4438;MY123456;Firmware222等信息
        /// </summary>
        private string m_Identity;

        [XmlElement("标识")]
        public string Identity
        {
            get { return m_Identity; }
            set
            {
                m_Identity = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("Identity");
                    NotifyPropertyChanged("CommentInfo");
                }
            }
        }

        private devState devinfoState;
        /// <summary>
        /// 设备状态
        /// </summary>
        public devState DevInfoState
        {
            get { return devinfoState; }
            set
            {
                if (value != devinfoState)
                {
                    this.devinfoState = value;
                    NotifyPropertyChanged("DevInfoState");
                    //if (this.m_InstrumentType == InstrumentType.DCPower || this.m_InstrumentType == InstrumentType.DCPowerAnalyzer)
                    //{
                    //    this.PowerListState = value;
                    //}
                }
              
            }
        }


        //private devState powerListState;
        ///// <summary>
        ///// 设备状态
        ///// </summary>
        //public devState PowerListState
        //{
        //    get { return powerListState; }
        //    set
        //    {
        //        if (value != powerListState)
        //        {
        //            this.powerListState = value;
        //            NotifyPropertyChanged("PowerListState");
        //            //if (this.m_InstrumentType != InstrumentType.DCPowerList)
        //            //{

        //            //}
        //            //else 
        //            //{
        //            //    foreach (InstrumentInfo instrumnet in InstrumentInfoList.getInstence())
        //            //    {
        //            //        if (instrumnet.DevInfoState == devState.ischeck)
        //            //        {
        //            //            this.devinfoState = devState.ischeck;
        //            //            NotifyPropertyChanged("DevInfoState");

        //            //        }
        //            //        else if (instrumnet.DevInfoState == devState.error)
        //            //        {
        //            //            this.devinfoState = devState.error;
        //            //            NotifyPropertyChanged("DevInfoState");
        //            //        }
        //            //        else if (instrumnet.DevInfoState == devState.alart)
        //            //        {
        //            //            this.devinfoState = devState.alart;
        //            //            NotifyPropertyChanged("DevInfoState");
        //            //        }
        //            //        else if (instrumnet.DevInfoState == devState.disenable)
        //            //        {
        //            //            this.devinfoState = devState.disenable;
        //            //            NotifyPropertyChanged("DevInfoState");
        //            //        }
        //            //        else if (instrumnet.DevInfoState == devState.normal)
        //            //        {
        //            //            this.devinfoState = devState.normal;
        //            //            NotifyPropertyChanged("DevInfoState");
        //            //        }
        //            //    }
        //            // }
        //        }
               
        //    }
        //}

        private int m_dcModleNum = 0;

        /// <summary>
        /// 电源的模块数量
        /// </summary>
        public int DCModleNum
        {
            get
            {
                if (this.m_InstrumentType == InstrumentType.DCPower && m_dcModleNum == 0)
                {
                    return 1;
                }
                else if (this.m_InstrumentType == InstrumentType.DCPowerAnalyzer && m_dcModleNum == 0)
                {
                    return 4;
                }
                else
                {
                    return m_dcModleNum;
                }
            }
            set
            {
                if (value != m_dcModleNum)
                {
                    this.m_dcModleNum = value;
                    NotifyPropertyChanged("DCModleNum");
                }
            }
        }


        private int m_DeviceID;
        /// <summary>
        /// 设备ID编号；可选属性[串口通讯时，用于表示设备地址]
        /// </summary>
        public int DeviceID
        {
            get { return m_DeviceID; }
            set 
            { 
                m_DeviceID = value;
                NotifyPropertyChanged("DeviceID");
            }
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public InstrumentInfo()
        {

        }
        /// <summary>
        /// 重写ToString函数
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return InstrumentName;
        }

        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="InIDInInstitute"></param>
        /// <param name="InInstrumentName"></param>
        /// <param name="InModelNo"></param>
        /// <param name="InSN"></param>
        /// <param name="InOptionList"></param>
        /// <param name="InManufacter"></param>
        /// <param name="InConnectOK"></param>
        /// <param name="InIpAddress"></param>
        /// <param name="InErrorMsgs"></param>
        public InstrumentInfo(
            InstrumentType inInstrumentType,
            string InIDInInstitute, string InInstrumentName, string InModelNo,
            string InSN,
            string InOptionList,
             string InManufacter,
             devState InConnectOK,
             string InIpAddress,
             string InErrorMsgs)
        {
            this.m_InstrumentType = inInstrumentType;
            this.m_ConnectOK = InConnectOK;
            this.m_ErrorMsgs = InErrorMsgs;
            this.m_IDInInstitute = InIDInInstitute;
            this.m_InstrumentName = InInstrumentName;
            this.m_IpAddress = InIpAddress;
            this.m_Manufacter = InManufacter;
            this.m_ModelNo = InModelNo;
            this.m_OptionList = InOptionList;
            this.m_SN = InSN;
            this.devinfoState = devState.normal;
        }


    }

    public class InstrumentInfoList : ObservableCollection<InstrumentInfo>
    {
        private static InstrumentInfoList _instrumentInfoList;

        public static InstrumentInfoList getInstence()
        {
            if (_instrumentInfoList == null)
            {
                _instrumentInfoList = new InstrumentInfoList();
                _instrumentInfoList = _instrumentInfoList.LoadParameterFromXMLFile();

            }
            return _instrumentInfoList;
        }
        private InstrumentInfoList()
            : base()
        {
            //_instrumentInfoList = new InstrumentInfoList();
        }

        public string GetIpAddressByDeviceName(string inDeviceName)
        {
            foreach (InstrumentInfo AInstrumentInfo in this)
            {
                if (AInstrumentInfo.InstrumentName.IndexOf(inDeviceName) >= 0)
                {
                    return AInstrumentInfo.IpAddress;
                }
            }
            return "";
        }

        /// <summary>
        /// 依据TYPE 获取IP
        /// </summary>
        /// <param name="instrumentType"></param>
        /// <returns></returns>
        public string GetIpAddressByDeviceName(InstrumentType instrumentType)
        {
            foreach (InstrumentInfo AInstrumentInfo in this)
            {
                if (AInstrumentInfo.InstrumentTypeID==instrumentType)
                {
                    return AInstrumentInfo.IpAddress;
                }
            }
            return "";
        }

        /// <summary>
        /// 保存进XML文件当中
        /// </summary>
        /// <param name="inXMLFileName"></param>
        public void SaveParameterToXMLFile(string inXMLFileName = null)
        {
            if ((inXMLFileName == null) || (inXMLFileName == ""))
            {
                inXMLFileName = AppDomain.CurrentDomain.BaseDirectory + "SystemInstrument.config";
            }

            XmlHelper.SaveParameterToXMLFile(typeof(InstrumentInfoList), this, inXMLFileName);
        }

        public InstrumentInfoList GetToDetectInstrumentList(InstrumentType[] inToDetectDeviceType)
        {
            InstrumentInfoList Result = new InstrumentInfoList();
            foreach (InstrumentInfo tmpInstrumentInfo in this)
            {
                foreach (InstrumentType tmpInstrumentType in inToDetectDeviceType)
                {
                    if (tmpInstrumentInfo.InstrumentTypeID == tmpInstrumentType)
                    {
                        Result.Add(tmpInstrumentInfo);
                        break;
                    }
                }
            }
            return Result;
        }




        public InstrumentInfoList GetDCPowerInstrumentList()
        {
            InstrumentInfoList Result = new InstrumentInfoList();
            foreach (InstrumentInfo tmpInstrumentInfo in this)
            {
                if (tmpInstrumentInfo.InstrumentTypeID == InstrumentType.DCPower || tmpInstrumentInfo.InstrumentTypeID== InstrumentType.DCPowerAnalyzer  )
                {
                    Result.Add(tmpInstrumentInfo);
                }
            }
            return Result;
        }


        public InstrumentInfoList GetInstrumentList( InstrumentType type)
        {
            InstrumentInfoList Result = new InstrumentInfoList();
            foreach (InstrumentInfo tmpInstrumentInfo in this)
            {
                if (tmpInstrumentInfo.InstrumentTypeID == type)
                {
                    Result.Add(tmpInstrumentInfo);
                }
            }
            return Result;
        }

        //public List<string> GetDCPowerInstrumentNameList()
        //{
        //    List<string> Result = new List<string>();
        //    foreach (InstrumentInfo tmpInstrumentInfo in this)
        //    {
        //        if (tmpInstrumentInfo.InstrumentTypeID == InstrumentType.DCPower || tmpInstrumentInfo.InstrumentTypeID == InstrumentType.DCPowerAnalyzer)
        //        {
        //            Result.Add(tmpInstrumentInfo.InstrumentName);
        //        }
        //    }
        //    return Result;
        //}


        public void RestoreDefaults()
        {
            this.Clear();
            ///暁斐补充：todo
            this.Add(new InstrumentInfo(InstrumentType.Pna, "", "矢量网络分析仪", "", "", "", "", devState.connect, "TCPIP0::192.168.1.101::5025", ""));
            
            //this.Add(new InstrumentInfo(InstrumentType.VectorSg, "", "矢量信号源", "", "", "", "", devState.connect, "TCPIP0::192.168.1.103::5025", ""));
            //this.Add(new InstrumentInfo(InstrumentType.AnalogSg, "", "模拟信号源", "", "", "", "", devState.connect, "TCPIP0::192.168.1.103::5025", ""));
           // this.Add(new InstrumentInfo(InstrumentType.AnalogSg2, "", "模拟信号源2", "", "", "", "", devState.connect, "GPIB1::6::INSTR", ""));
            //this.Add(new InstrumentInfo(InstrumentType.PowerMeterInCabinet, "", "功率计", "", "", "", "", devState.connect, "TCPIP0::192.168.1.102::5025", ""));
            //this.Add(new InstrumentInfo(InstrumentType.MultiChannelAcquistitor, "", "多路数据采集单元", "", "", "", "", devState.connect, "TCPIP0::192.168.1.107::5025", ""));
            //this.Add(new InstrumentInfo(InstrumentType.DCPowerAnalyzer, "", "直流电源分析仪", "", "", "", "", devState.connect, "TCPIP0::192.168.1.120::5025", ""));

           //   this.Add(new InstrumentInfo(InstrumentType.FrequencyCounter, "", "频率计数器", "", "", "", "", devState.connect, "TCPIP0::192.168.1.104::5025", ""));
            this.Add(new InstrumentInfo(InstrumentType.InputMatrix, "", "输入开关矩阵箱", "", "", "", "", devState.connect, "TCPIP0::192.168.1.114::INSTR", ""));
            //this.Add(new InstrumentInfo(InstrumentType.OutputMatrix, "", "输出开关矩阵箱", "", "", "", "", devState.connect, "TCPIP0::192.168.1.114::INSTR", ""));
            //this.Add(new InstrumentInfo(InstrumentType.MultiChannelController, "", "多通道控制箱", "", "", "", "", devState.connect, "TCPIP0::192.168.1.108::2208::SOCKET", ""));
            //this.Add(new InstrumentInfo(InstrumentType.CSBDevice, "", "地检设备", "", "", "", "", devState.connect, "TCPIP0::192.168.1.108::2208::SOCKET", ""));
    
            //this.Add(new InstrumentInfo(InstrumentType.Incubator, "", "温箱", "", "", "", "", devState.connect, "COM::COM3::19200::1::8::None::1::inst0::INSTR", ""));
            //this.Add(new InstrumentInfo(InstrumentType.TemperatureMonitor, "", "温度巡检仪", "", "", "", "", devState.connect, "COM::COM4::9600::1::8::None::1::inst0::INSTR", ""));

            //this.Add(new InstrumentInfo(InstrumentType.Alarm, "", "报警器", "", "", "", "", devState.connect, "COM::COM5::9600::1::8::None::1::inst0::INSTR", ""));

            //高压测试仪，高压佩电箱

            //TODO：后面添加
            //噪声系数测试仪
            //this.Add(new InstrumentInfo(InstrumentType.NoiseFigure, "", "噪声系数分析仪", "", "", "", "", devState.connect, "TCPIP0::192.168.1.115::INSTR" , ""));
            //噪声源 不用控
            //this.Add(new InstrumentInfo(InstrumentType.NoiceSource, "", "噪声源", "", "", "", "", devState.connect, "TCPIP0::192.168.1.116::INSTR", ""));
            //this.Add(new InstrumentInfo(InstrumentType.SA, "", "频谱仪", "", "", "", "", devState.connect, "TCPIP0::192.168.1.117::5025", ""));
            //数字表
        }

        /// <summary>
        /// 加载测试参数数据
        /// </summary>
        /// <param name="inXmlFileName">要加载的测试参数配置文件名，为空时从默认文件中加载</param>
        public InstrumentInfoList LoadParameterFromXMLFile(string inXmlFileName = null)
        {
            //缺省参数处理
            if ((inXmlFileName == null) || (inXmlFileName == ""))
            {
                inXmlFileName = AppDomain.CurrentDomain.BaseDirectory + "SystemInstrument.config";
            }
            try
            {
                InstrumentInfoList tmpDUTs = (InstrumentInfoList)XmlHelper.LoadParameterFromXMLFile(typeof(InstrumentInfoList), inXmlFileName);
                if (tmpDUTs == null)
                {
                    tmpDUTs = new InstrumentInfoList();
                }
                if (tmpDUTs.Count < 1)
                {
                    tmpDUTs.RestoreDefaults();
                }
                return tmpDUTs;
            }
            catch (Exception)
            {
                return new InstrumentInfoList();
            }
        }

        /// <summary>
        /// 获取仪表名称
        /// </summary>
        /// <param name="instrumentType"></param>
        /// <returns></returns>
        public string GetInstrumentName( InstrumentType instrumentType)
        {
            foreach (InstrumentInfo thisInstrument in this)
            {
                if (thisInstrument.InstrumentTypeID == instrumentType)
                {
                    return thisInstrument.InstrumentName;
                }
            }
            return "";
        }

        /// <summary>
        /// 获取仪表名称
        /// </summary>
        /// <param name="instrumentType"></param>
        /// <returns></returns>
        public InstrumentInfo GetInstrument(InstrumentType instrumentType,)
        {
            foreach (InstrumentInfo thisInstrument in this)
            {
                if (thisInstrument.InstrumentTypeID == instrumentType)
                {
                    return thisInstrument;
                }
            }
            return null;
        }

        /// <summary>
        /// 拷贝 信息更新内存中当前对像信息（因为使用绑定，不能进行对像的替换）
        /// </summary>
        /// <param name="m_CurInstrumentInfoList"></param>
        public void Clone(InstrumentInfoList m_CurInstrumentInfoList)
        {
          
           
            foreach (InstrumentInfo instrumentInfo in m_CurInstrumentInfoList)
            {
                foreach (InstrumentInfo thisInstrument in this)
                {
                   
                    if (instrumentInfo.InstrumentTypeID == thisInstrument.InstrumentTypeID
                        && (instrumentInfo.InstrumentTypeID!= InstrumentType.DCPower || instrumentInfo.InstrumentTypeID != InstrumentType.DCPowerAnalyzer))
                    {
                        //更新信息
                        // thisInstrument.CommentInfo = instrumentInfo.CommentInfo;
                        thisInstrument.ConnectOK = instrumentInfo.ConnectOK;
                        thisInstrument.DCModleNum = instrumentInfo.DCModleNum;
                        thisInstrument.DevInfoState = instrumentInfo.DevInfoState;
                        thisInstrument.Enabled = instrumentInfo.Enabled;
                        thisInstrument.ErrorMsgs = instrumentInfo.ErrorMsgs;
                        thisInstrument.Identity = instrumentInfo.Identity;
                        thisInstrument.IDInInstitute = instrumentInfo.IDInInstitute;
                        thisInstrument.InstrumentName = instrumentInfo.InstrumentName;
                        thisInstrument.IpAddress = instrumentInfo.IpAddress;
                        thisInstrument.Manufacter = instrumentInfo.Manufacter;
                        thisInstrument.ModelNo = instrumentInfo.ModelNo;
                        thisInstrument.OptionList = instrumentInfo.OptionList;
                        thisInstrument.SN = instrumentInfo.SN;
                    }
                }
            }

            for (int i = this.Count-1; i>=0; i--)
            {
                 if (this[i].InstrumentTypeID == InstrumentType.DCPower || this[i].InstrumentTypeID == InstrumentType.DCPowerAnalyzer)
                 {
                     this.Remove(this[i]);
                 }
 
            }
            //foreach (InstrumentInfo thisInstrument in this)
            //{
            //    if (thisInstrument.InstrumentTypeID == InstrumentType.DCPower || thisInstrument.InstrumentTypeID == InstrumentType.DCPowerAnalyzer)
            //    {
            //        this.Remove(thisInstrument);
            //    }
            //}

            foreach (InstrumentInfo instrumentInfo in m_CurInstrumentInfoList)
            {
                if (instrumentInfo.InstrumentTypeID == InstrumentType.DCPower || instrumentInfo.InstrumentTypeID == InstrumentType.DCPowerAnalyzer)
                {
                    this.Add(instrumentInfo);
                }
            }
           // throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 设备地址
    /// </summary>
    public class InstrumentAddress : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string devstate)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(devstate));
            }
        }


        /// <summary>
        /// 地址类型
        /// </summary>
        private addrType m_instrumentAddType;

        public addrType InstrumentAddType
        {
            get { return m_instrumentAddType; }
            set { 
                m_instrumentAddType = value;
                IsApply = true;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("InstrumentAddr");
                    NotifyPropertyChanged("IsApply");
                    
                }
            }
        }

        /// <summary>
        /// 实际地址
        /// </summary>
        private string m_InstrumentAddr;

        public string InstrumentAddr
        {
            get
            {
                if (InstrumentAddType == addrType.LAN)
                {
                    m_InstrumentAddr = IPAddr;
                }
                else if (InstrumentAddType == addrType.GPIB)
                {
                    m_InstrumentAddr = GPIBAddr;
                }
                else if (InstrumentAddType == addrType.IOAddr)
                {
                    m_InstrumentAddr = IOAddr;
                }


                return m_InstrumentAddr;
            }
            set {
                m_InstrumentAddr = value;
                
                if (this.PropertyChanged != null)
                {
                   
                    NotifyPropertyChanged("InstrumentAddr");
                    
                }
            }

        }


        //IO地址
        private string  m_IOAddr;

        public string  IOAddr
        {
            get 
            { 
                return m_IOAddr; 
            }
            set 
            {
                m_IOAddr = value;
                this.IsApply = true;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("IOAddr");
                    // NotifyPropertyChanged("IPAddr");
                    NotifyPropertyChanged("InstrumentAddr");
                    NotifyPropertyChanged("IsApply");
                }
            }
        }
        
        

        /// <summary>
        /// IP地址
        /// </summary>
        private string m_instrumentIP;

        public string InstrumentIP
        {
            get
            {

                return m_instrumentIP;
            }
            set
            {
                m_instrumentIP = value;
                this.IsApply = true;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("InstrumentIP");
                   // NotifyPropertyChanged("IPAddr");
                    NotifyPropertyChanged("InstrumentAddr");
                    NotifyPropertyChanged("IsApply");
                }

            }
        }

        /// <summary>
        /// GPIB地址
        /// </summary>
        private string m_GPIBAddr;

        public string GPIBAddr
        {
            get
            {
                string gpib = "GPIB" + m_FNum.ToString() + "::" + m_SNum.ToString() + "::" + m_TNum.ToString() + "::INSTR";
                m_GPIBAddr = gpib;
                return m_GPIBAddr;
            }
            set
            {
                m_GPIBAddr = value;
                NotifyPropertyChanged("GPIBAddr");

            }
        }

        /// <summary>
        /// IP地址
        /// </summary>
        private string m_IPAddr;

        public string IPAddr
        {
            get
            {
                string ip = "TCPIP0::" + m_instrumentIP +"::"+ m_IPPort.ToString()+"::INSTR";
                m_IPAddr = ip;
                return m_IPAddr;
            }
            set { m_IPAddr = value; }
        }

        private int  m_IPPort=5025;

        public int IPPort
        {
            get { return m_IPPort; }
            set { m_IPPort = value; }
        }
        
        /// <summary>
        /// 主卡号
        /// </summary>
        private int m_FNum;

        public int FNum
        {
            get { return m_FNum; }
            set
            {
                m_FNum = value;
                this.IsApply = true;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("FNum");
                  //  NotifyPropertyChanged("GPIBAddr");
                    NotifyPropertyChanged("InstrumentAddr");
                    NotifyPropertyChanged("IsApply");
                }
            }
        }

        /// <summary>
        /// 从卡号
        /// </summary>
        private int m_SNum;

        public int SNum
        {
            get { return m_SNum; }
            set
            {
                m_SNum = value;
                this.IsApply = true;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("SNum");
                  //  NotifyPropertyChanged("GPIBAddr");
                    NotifyPropertyChanged("InstrumentAddr");
                    NotifyPropertyChanged("IsApply");
                }
            }
        }

        /// <summary>
        /// 三卡号
        /// </summary>
        private int m_TNum;

        public int TNum
        {
            get { return m_TNum; }
            set
            {
                m_TNum = value;
                this.IsApply = true;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("TNum");
                   // NotifyPropertyChanged("GPIBAddr");
                    NotifyPropertyChanged("InstrumentAddr");
                    NotifyPropertyChanged("IsApply");
                }
            }
        }

        private bool m_isApply;

        public bool IsApply
        {
            get { return m_isApply; }
            set { 
                m_isApply = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("IsApply");
                }
            }
        }
        

    }
}
