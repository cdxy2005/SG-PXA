//**********************************************
//*类名：ConnectCtrlHandle
//*作者: Gavin
//*创建时间: 2014/6/20 12:08:56
//*功能: 
//***********************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using RackSys.TestLab.Visa;
using System.Windows;

namespace RAC_Test.DefControl
{


    #region 对外接口
    /// <summary>
    /// 对外提供服务的类
    /// </summary>
    public class ConnectCtrlHandle : VisaAddress
    {
        #region 构造函数
        public ConnectCtrlHandle(string visaAddr)
        {
            //初始化visa地址
            SetVisaAddr(visaAddr);
        }
        #endregion

        private VisaAddress m_CurrentVisaAddress;

        public VisaAddress CurrentVisaAddress
        {
            get { return m_CurrentVisaAddress; }
            set
            {
                m_CurrentVisaAddress = value;
                NotifyPropertyChanged("CurrentVisaAddress");
            }
        }

        private Dictionary<string, VisaAddress> m_VisaAddressCollection;

        public Dictionary<string, VisaAddress> VisaAddressCollection
        {
            get
            {
                if (object.ReferenceEquals(null, m_VisaAddressCollection))
                {
                    m_VisaAddressCollection = new Dictionary<string, VisaAddress>();
                    //网口
                    m_VisaAddressCollection.Add(LanAddr.AddrFlag, new LanAddr());
                    //串口
                    m_VisaAddressCollection.Add(SerialPortAddr.AddrFlag, new SerialPortAddr());
                    //GPIB
                    m_VisaAddressCollection.Add(GPIBAddr.AddrFlag, new GPIBAddr());
                    //USB
                    //m_VisaAddressCollection.Add(USBAddr.AddrFlag, new USBAddr());
                    //IOLib
                    // m_VisaAddressCollection.Add(IOLibAddr.AddrFlag, new IOLibAddr());
                    //自定义
                    //  m_VisaAddressCollection.Add(UserDefineAddr.AddrFlag, new UserDefineAddr());
                }
                return m_VisaAddressCollection;
            }
        }

        #region 设置获取visa地址

        public override string GetVisaAddr()
        {
            return CurrentVisaAddress.GetVisaAddr();
        }

        public override void SetVisaAddr(string visaAddr)
        {

            try
            {

                //根据visa地址初始化CurrentVisaAddress
                string[] textArray = visaAddr.Split(new char[] { ':' });

                if (textArray.Length > 1)
                {
                    if (textArray[0].IndexOf(LanAddr.AddrFlag) >= 0)
                    {
                        //LanAddr
                        CurrentVisaAddress = VisaAddressCollection[LanAddr.AddrFlag];
                    }
                    else if (textArray[0].IndexOf(SerialPortAddr.AddrFlag) >= 0)
                    {
                        //SerialPortAddr
                        CurrentVisaAddress = VisaAddressCollection[SerialPortAddr.AddrFlag];
                    }
                    else if (textArray[0].IndexOf(GPIBAddr.AddrFlag) >= 0)
                    {
                        //GPIBAddr
                        CurrentVisaAddress = VisaAddressCollection[GPIBAddr.AddrFlag];
                    }
                    //else if (textArray[0].IndexOf(USBAddr.AddrFlag) >= 0)
                    //{
                    //    //USB
                    //    CurrentVisaAddress = VisaAddressCollection[USBAddr.AddrFlag];
                    //}
                }
                else
                {
                    //自定义
                    // CurrentVisaAddress = VisaAddressCollection[UserDefineAddr.AddrFlag];
                }

            }
            catch
            {
                //自定义
                //CurrentVisaAddress = VisaAddressCollection[UserDefineAddr.AddrFlag];
            }
            CurrentVisaAddress.SetVisaAddr(visaAddr);

        }
        #endregion
    }
    #endregion

    #region 基类
    public abstract class VisaAddress : INotifyPropertyChanged
    {
        public enum PortType
        {
            Lan,
            SerialsPort,
            GPIB,
            //USB,
            IOLib,
            //UserDefine
        }

        #region 属性更新消息

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string para)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(para));
            }
        }
        #endregion

        /// <summary>
        /// 设置visa地址
        /// </summary>
        /// <param name="visaAddr"></param>
        public abstract void SetVisaAddr(string visaAddr);
        /// <summary>
        /// 获取visa地址
        /// </summary>
        /// <returns></returns>
        public abstract string GetVisaAddr();

        public virtual PortType CurPortType
        {
            get
            {
                return PortType.Lan;
            }
        }
    }
    #endregion

    #region 网口
    /// <summary>
    /// 网口
    /// </summary>
    public class LanAddr : VisaAddress
    {
        /// <summary>
        /// 地址标识
        /// </summary>
        public static string AddrFlag = "TCPIP0";

        public override PortType CurPortType
        {
            get
            {
                return PortType.Lan;
            }
        }

        private string m_Ip = "192.168.0.1";
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip
        {
            get { return m_Ip; }
            set
            {
                m_Ip = value;
                NotifyPropertyChanged("Ip");
            }
        }

        private string m_Port = "5025";
        /// <summary>
        /// 端口号
        /// </summary>
        public string Port
        {
            get { return m_Port; }
            set
            {
                m_Port = value;
                NotifyPropertyChanged("Port");
            }
        }

        #region 设置获取visa地址
        public override string GetVisaAddr()
        {
            return string.Format("TCPIP0::{0}::{1}::INSTR", Ip, Port);
        }

        public override void SetVisaAddr(string visaAddr)
        {
            // string[] textArray = visaAddr.Split(new char[] { ':' });
            //if (textArray.Length > 5)
            //{
            //    Ip = textArray[2];
            //    Port = textArray[4];
            //}

            Ip = "";
            Port = "-1";
            string[] textArray = visaAddr.Split(new char[] { ':' });
            if (textArray.Length >= 5)
            {
                Ip = textArray[2];
                string text = textArray[4];
                Port = "5025";//0x13a1;
                try
                {
                    Port = text;
                }
                catch
                {
                }
            }

        }
        #endregion

        public override string ToString()
        {
            return "网口";
        }
    }
    #endregion

    #region 串口
    /// <summary>
    /// 串口
    /// </summary>
    public class SerialPortAddr : VisaAddress
    {
        /// <summary>
        /// 地址标识
        /// </summary>
        public static string AddrFlag = "COM";

        public override PortType CurPortType
        {
            get
            {
                return PortType.SerialsPort;
            }
        }

        #region 串口号
        private List<string> m_COMIDItems;
        /// <summary>
        /// 串口号集合
        /// </summary>
        public List<string> COMIDItems
        {
            get
            {
                if (object.ReferenceEquals(null, m_COMIDItems))
                {
                    m_COMIDItems = new List<string>();
                    for (int i = 0; i < 255; i++)
                    {
                        m_COMIDItems.Add("COM" + (i + 1));
                    }
                }
                return m_COMIDItems;
            }
        }

        private string m_CurCOMIDSelectItem = "COM1";
        /// <summary>
        /// 当前选择的串口号
        /// </summary>
        public string CurCOMIDSelectItem
        {
            get { return m_CurCOMIDSelectItem; }
            set
            {
                m_CurCOMIDSelectItem = value;
                NotifyPropertyChanged("CurCOMIDSelectItem");
            }
        }
        /// <summary>
        /// 当前端口号
        /// </summary>
        public int CurCOMID
        {
            get
            {
                return COMIDItems.FindIndex(FindItem) + 1;
            }
        }

        /// <summary>
        /// 查找字符串
        /// </summary>
        private bool FindItem(string para)
        {
            if (string.Equals(CurCOMIDSelectItem, para))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 波特率
        /// <summary>
        /// 波特率
        /// </summary>
        private readonly List<int> m_SerialPortBounds = new List<int>() 
        {
            {110},
            {300},
            {600},
            {1200},
            {2400},
            {4800},
            {9600},
            {14400},
            {19200},
            {38400},
            {43000},
            {56000},
            {57600},
            {115200},
            {128000},
            {256000}
        };
        /// <summary>
        /// 波特率
        /// </summary>
        public List<int> SerialPortBounds
        {
            get { return m_SerialPortBounds; }
        }

        /// <summary>
        /// 当前选择波特率
        /// </summary>
        private int m_CurSerialPortBound = 9600;
        public int CurSerialPortBound
        {
            get { return m_CurSerialPortBound; }
            set
            {
                m_CurSerialPortBound = value;
                NotifyPropertyChanged("CurSerialPortBound");
            }
        }
        #endregion

        #region 起始位
        private int m_StartBit = 1;

        public int StartBit
        {
            get { return m_StartBit; }
            set
            {
                m_StartBit = value;
                NotifyPropertyChanged("StartBit");
            }
        }
        #endregion

        #region 数据位
        /// <summary>
        /// 数据位
        /// </summary>
        private readonly List<int> m_SerialPortDataBit = new List<int>() 
        {
            {5},
            {6},
            {7},
            {8}
        };
        /// <summary>
        /// 数据位
        /// </summary>
        public List<int> SerialPortDataBit
        {
            get { return m_SerialPortDataBit; }
        }

        private int m_CurSerialPortDataBit = 8;
        public int CurSerialPortDataBit
        {
            get { return m_CurSerialPortDataBit; }
            set
            {
                m_CurSerialPortDataBit = value;
                NotifyPropertyChanged("CurSerialPortDataBit");
            }
        }
        #endregion

        #region 校验位
        /// <summary>
        /// 校验位
        /// </summary>
        private readonly List<string> m_SerialPortCheckBit = new List<string>() 
        { 
            {"N 无"},
            {"O 奇"},
            {"E 偶"},
            {"M 标志"},
            {"S 空格"}
        };
        /// <summary>
        /// 校验位
        /// </summary>
        public List<string> SerialPortCheckBit
        {
            get { return m_SerialPortCheckBit; }
        }

        private int m_CurrentCheckBitIndex = 0;

        public int CurrentCheckBitIndex
        {
            get { return m_CurrentCheckBitIndex; }
            set
            {
                m_CurrentCheckBitIndex = value;
                NotifyPropertyChanged("CurrentCheckBitIndex");
            }
        }

        public string GetCurSerialPortCheckBit()
        {
            if (0 == CurrentCheckBitIndex)
            {
                return "None";
            }
            else if (1 == CurrentCheckBitIndex)
            {
                return "Odd";
            }
            else if (2 == CurrentCheckBitIndex)
            {
                return "Even";
            }
            else if (3 == CurrentCheckBitIndex)
            {
                return "Mark";
            }
            else if (4 == CurrentCheckBitIndex)
            {
                return "Space";
            }
            else
            {
                return "None";
            }
        }
        #endregion

        #region 停止位
        /// <summary>
        /// 停止位
        /// </summary>
        private readonly List<double> m_SerialPortStopBit = new List<double>() 
        {
            {1},
            {1.5},
            {2}
        };
        /// <summary>
        /// 停止位
        /// </summary>
        public List<double> SerialPortStopBit
        {
            get { return m_SerialPortStopBit; }
        }

        private double m_CurSerialPortStopBit;
        /// <summary>
        /// 当前选择的停止位
        /// </summary>
        public double CurSerialPortStopBit
        {
            get
            {
                if (!m_SerialPortStopBit.Contains(m_CurSerialPortStopBit))
                {
                    m_CurSerialPortStopBit = m_SerialPortStopBit[0];
                }
                return m_CurSerialPortStopBit;
            }
            set
            {
                m_CurSerialPortStopBit = value;
                NotifyPropertyChanged("CurSerialPortStopBit");
            }
        }
        #endregion

        #region 设置获取visa地址
        public override string GetVisaAddr()
        {
            return string.Format("COM::{0}::{1}::{2}::{3}::{4}::{5}::inst0::INSTR",
                CurCOMIDSelectItem,
                CurSerialPortBound,
                StartBit,
                CurSerialPortDataBit,
                GetCurSerialPortCheckBit(),
                CurSerialPortStopBit);
        }

        public override void SetVisaAddr(string visaAddr)
        {
            ///COM::COM12::9600::1::8::EVEN::1::inst0::INSTR
            string[] textArray = visaAddr.Split(new char[] { ':' });
            if (textArray.Length >= 13)
            {
                CurCOMIDSelectItem = textArray[2];
                string tmpBaudRateStr = textArray[4];
                CurSerialPortBound = 9600;
                try
                {
                    CurSerialPortBound = int.Parse(tmpBaudRateStr); ;
                }
                catch
                {
                }

                StartBit = 1;
                try
                {
                    StartBit = int.Parse(textArray[6]); ;
                }
                catch
                {
                }

                CurSerialPortDataBit = 8;
                try
                {
                    CurSerialPortDataBit = int.Parse(textArray[8]); ;
                }
                catch
                {
                }
                string tmpCheckBitAsStr = textArray[10];
                if (tmpCheckBitAsStr.ToUpper() == "NONE")
                {
                    //奇校验
                    CurrentCheckBitIndex = 0;
                }
                else if (tmpCheckBitAsStr.ToUpper() == "ODD")
                {
                    //奇校验
                    CurrentCheckBitIndex = 1;
                }
                else if (tmpCheckBitAsStr.ToUpper() == "EVEN")
                {
                    //偶校验
                    CurrentCheckBitIndex = 2;
                }
                else if (tmpCheckBitAsStr.ToUpper() == "MARK")
                {
                    //标志校验
                    CurrentCheckBitIndex = 3;
                }
                else if (tmpCheckBitAsStr.ToUpper() == "SPACE")
                {
                    //空格校验
                    CurrentCheckBitIndex = 4;
                }
                CurSerialPortStopBit = 1;
                try
                {
                    CurSerialPortStopBit = double.Parse(textArray[12]); ;
                }
                catch
                {
                }
            }
        }
        #endregion

        public override string ToString()
        {
            return "串口";
        }
    }
    #endregion

    #region GPIB
    /// <summary>
    /// GPIB
    /// </summary>
    public class GPIBAddr : VisaAddress
    {
        /// <summary>
        /// 地址标识
        /// </summary>
        public static string AddrFlag = "GPIB";

        public override PortType CurPortType
        {
            get
            {
                return PortType.GPIB;
            }
        }

        #region 板卡编号0-12
        private List<int> m_CardNumCollection;
        /// <summary>
        /// 板卡编号
        /// </summary>
        public List<int> CardNumCollection
        {
            get
            {
                if (object.ReferenceEquals(null, m_CardNumCollection))
                {
                    m_CardNumCollection = new List<int>();
                    for (int i = 0; i < 13; i++)
                    {
                        m_CardNumCollection.Add(i);
                    }
                }
                return m_CardNumCollection;
            }
        }

        private int m_CurCadNum = 0;
        /// <summary>
        /// 当前板卡编号
        /// </summary>
        public int CurCadNum
        {
            get { return m_CurCadNum; }
            set
            {
                m_CurCadNum = value;
                NotifyPropertyChanged("CurCadNum");
            }
        }
        #endregion

        #region  主地址0-32
        private List<int> m_AddrCollection;
        /// <summary>
        /// 板卡编号
        /// </summary>
        public List<int> AddrCollection
        {
            get
            {
                if (object.ReferenceEquals(null, m_AddrCollection))
                {
                    m_AddrCollection = new List<int>();
                    for (int i = 0; i < 33; i++)
                    {
                        m_AddrCollection.Add(i);
                    }
                }
                return m_AddrCollection;
            }
        }

        private int m_MainAddr = 0;
        /// <summary>
        /// 当前板卡编号
        /// </summary>
        public int MainAddr
        {
            get { return m_MainAddr; }
            set
            {
                m_MainAddr = value;
                NotifyPropertyChanged("MainAddr");
            }
        }
        #endregion

        #region  从地址0-32

        private int m_ViceAddr = 0;
        /// <summary>
        /// 当前板卡编号
        /// </summary>
        public int ViceAddr
        {
            get { return m_ViceAddr; }
            set
            {
                m_ViceAddr = value;
                NotifyPropertyChanged("ViceAddr");
            }
        }
        #endregion

        #region 设置获取visa地址
        public override string GetVisaAddr()
        {
            return string.Format("GPIB{0}::{1}::{2}::INSTR",
                CurCadNum,
                MainAddr,
                ViceAddr);
        }

        public override void SetVisaAddr(string visaAddr)
        {
            int bordnum = -1;
            int mainnum = -1;
            int nextnum = -1;

            string[] textArray = visaAddr.Split(new char[] { ':' });
            try
            {
                if (textArray.Length >= 1)
                {
                    bordnum = int.Parse(textArray[0].Substring(4, 1));
                }
                if (textArray.Length >= 3)
                {
                    mainnum = int.Parse(textArray[2]);
                }
                if (textArray.Length >= 6)
                {
                    nextnum = int.Parse(textArray[4]);
                }
            }
            catch
            { }


            CurCadNum = bordnum;
            MainAddr = mainnum;
            ViceAddr = nextnum;
            //string[] textArray = visaAddr.Split(new char[] { ':' });
            //if (textArray.Length > 1)
            //{
            //    //板卡
            //    try
            //    {
            //        CurCadNum = int.Parse(textArray[0]); ;
            //    }
            //    catch
            //    {
            //    }
            //    //主地址
            //    try
            //    {
            //        MainAddr = int.Parse(textArray[2]); ;
            //    }
            //    catch
            //    {
            //    }
            //    //从地址
            //    try
            //    {
            //        ViceAddr = int.Parse(textArray[4]); ;
            //    }
            //    catch
            //    {
            //    }
            //}
        }
        #endregion

        public override string ToString()
        {
            return "GPIB";
        }
    }
    #endregion

    //#region USB
    //public class USBAddr : VisaAddress
    //{
    //    /// <summary>
    //    /// 地址标识
    //    /// </summary>
    //    public static string AddrFlag = "USB";

    //    public override PortType CurPortType
    //    {
    //        get
    //        {
    //            return PortType.USB;
    //        }
    //    }

    //    #region 设置获取visa地址
    //    public override string GetVisaAddr()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void SetVisaAddr(string visaAddr)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    #endregion

    //    public override string ToString()
    //    {
    //        return "USB";
    //    }
    //}
    //#endregion

    #region IOLib库
    /// <summary>
    /// IOLib库
    /// </summary>
    public class IOLibAddr : VisaAddress
    {
        /// <summary>
        /// 地址标识
        /// </summary>
        public static string AddrFlag = "IOLib";

        public override PortType CurPortType
        {
            get
            {
                return PortType.IOLib;
            }
        }

        List<string> m_VisaAddrCollection;
        /// <summary>
        /// Visa地址集合
        /// </summary>
        public List<string> VisaAddrCollection
        {
            get
            {
                //if (object.ReferenceEquals(null, m_VisaAddrCollection))
                //{
                m_VisaAddrCollection = new List<string>();
                //从IO库里面进行查找
                foreach (VisaDeviceResource resource in VisaDeviceExplorer.DiscoverLANResources())
                {
                    m_VisaAddrCollection.Add(resource.Address.VisaResourceName);
                }
                foreach (VisaDeviceResource resource in VisaDeviceExplorer.DiscoverGPIBResources())
                {
                    m_VisaAddrCollection.Add(resource.Address.VisaResourceName);
                }
                foreach (VisaDeviceResource resource in VisaDeviceExplorer.DiscoverUSBResources())
                {
                    m_VisaAddrCollection.Add(resource.Address.VisaResourceName);
                }
                //}
                return m_VisaAddrCollection;
            }
        }

        private string m_CurSelectedVisaAddr;
        /// <summary>
        /// 当前选择的visa地址
        /// </summary>
        public string CurSelectedVisaAddr
        {
            get { return m_CurSelectedVisaAddr; }
            set
            {
                m_CurSelectedVisaAddr = value;
                NotifyPropertyChanged("CurSelectedVisaAddr");
            }
        }

        #region 设置获取visa地址
        public override string GetVisaAddr()
        {
            return CurSelectedVisaAddr;
        }

        public override void SetVisaAddr(string visaAddr)
        {
            CurSelectedVisaAddr = visaAddr;
        }
        #endregion

        public override string ToString()
        {
            return "IOLib库";
        }
    }
    #endregion

    //#region 用户自定义Visa地址
    ///// <summary>
    ///// 自定义
    ///// </summary>
    //public class UserDefineAddr : VisaAddress
    //{
    //    /// <summary>
    //    /// 地址标识
    //    /// </summary>
    //    public static string AddrFlag = "VisaUserDefine";

    //    public override PortType CurPortType
    //    {
    //        get
    //        {
    //            return PortType.UserDefine;
    //        }
    //    }

    //    private string m_VisaAddr;
    //    /// <summary>
    //    /// 自定义visa地址
    //    /// </summary>
    //    public string VisaAddr
    //    {
    //        get { return m_VisaAddr; }
    //        set
    //        {
    //            m_VisaAddr = value;
    //            NotifyPropertyChanged("VisaAddr");
    //        }
    //    }

    //    #region 设置获取visa地址
    //    public override string GetVisaAddr()
    //    {
    //        return this.VisaAddr;
    //    }

    //    public override void SetVisaAddr(string visaAddr)
    //    {
    //        this.VisaAddr = visaAddr;
    //    }
    //    #endregion

    //    public override string ToString()
    //    {
    //        return "自定义";
    //    }
    //}
    //#endregion
}
