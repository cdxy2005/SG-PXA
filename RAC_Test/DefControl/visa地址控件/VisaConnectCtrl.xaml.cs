using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace RAC_Test.DefControl
{
    /// <summary>
    /// VisaConnectCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class VisaConnectCtrl :INotifyPropertyChanged
    {
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

        #region 构造函数

        private ConnectCtrlHandle m_ConnectCtrlHandle;

        public VisaConnectCtrl()
        {
            InitializeComponent();
        }

        public void Init(string visaAddr)
        {
            m_ConnectCtrlHandle = new ConnectCtrlHandle(visaAddr);
            //初始化
            CurrentVisaAddress = m_ConnectCtrlHandle.CurrentVisaAddress;

            //数据绑定
            this.DataContext = this;

            #region 类型选择
            //类型选择
            this._cbConnectionTypeItems.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("ConnectionTypeItems"), Source = this });
            this._cbConnectionTypeItems.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("CurrentVisaAddress"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = this });
            #endregion

        }

        public string GetVisaAddr()
        {
            return m_ConnectCtrlHandle.GetVisaAddr();
        }
        #endregion

        #region  Visa地址改变的事件
        //定义委托
        public delegate void VisaAddrChangedHandle(object sender, EventArgs e);
        //定义事件
        public event VisaAddrChangedHandle VisaAddrChanged;
        private void _cbVisaAddrChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VisaAddrChanged != null)
                VisaAddrChanged(sender, new EventArgs());//把按钮自身作为参数传递
        }

        private void VisaAddr_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (VisaAddrChanged != null)
                VisaAddrChanged(sender, new EventArgs());//把按钮自身作为参数传递
        }
        #endregion



        #region 连接方式选择

        public List<VisaAddress> ConnectionTypeItems
        {
            get
            {
                return m_ConnectCtrlHandle.VisaAddressCollection.Values.ToList();
            }
        }

        public VisaAddress CurrentVisaAddress
        {
            get
            {
                return m_ConnectCtrlHandle.CurrentVisaAddress;
            }
            set
            {
                m_ConnectCtrlHandle.CurrentVisaAddress = value;
                //根据选择的项目初始化
                ShowStaticPanel(m_ConnectCtrlHandle.CurrentVisaAddress.CurPortType);
                NotifyPropertyChanged("CurrentVisaAddress");
            }
        }
        #endregion

        #region 界面配置显示控制
        #endregion

        private void ShowStaticPanel(VisaAddress.PortType PortType)
        {
            switch (PortType)
            {
                case VisaAddress.PortType.Lan:
                    #region 网口
                    _spLan.Visibility = Visibility.Visible;
                    _spSerialPort.Visibility = Visibility.Collapsed;
                    _spGpib.Visibility = Visibility.Collapsed;
                    _spUsb.Visibility = Visibility.Collapsed;
                    _spIoLib.Visibility = Visibility.Collapsed;
                    _spUserDefine.Visibility = Visibility.Collapsed;
                    this._ipAddr.SetBinding(TextBox.TextProperty, new Binding() { Path = new PropertyPath("Ip"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[LanAddr.AddrFlag] });
                    this._lanPort.SetBinding(TextBox.TextProperty, new Binding() { Path = new PropertyPath("Port"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[LanAddr.AddrFlag] });
                    #endregion
                    break;

                case VisaAddress.PortType.SerialsPort:
                    #region 串口
                    _spLan.Visibility = Visibility.Collapsed;
                    _spSerialPort.Visibility = Visibility.Visible;
                    _spGpib.Visibility = Visibility.Collapsed;
                    _spUsb.Visibility = Visibility.Collapsed;
                    _spIoLib.Visibility = Visibility.Collapsed;
                    _spUserDefine.Visibility = Visibility.Collapsed;
                    //串口号
                    this._cbComId.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("COMIDItems"), Source = m_ConnectCtrlHandle.VisaAddressCollection[SerialPortAddr.AddrFlag] });
                    this._cbComId.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("CurCOMIDSelectItem"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[SerialPortAddr.AddrFlag] });
                    //波特率
                    this._cbBounds.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("SerialPortBounds"), Source = m_ConnectCtrlHandle.VisaAddressCollection[SerialPortAddr.AddrFlag] });
                    this._cbBounds.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("CurSerialPortBound"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[SerialPortAddr.AddrFlag] });
                    //数据位
                    this._cbDataBit.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("SerialPortDataBit"), Source = m_ConnectCtrlHandle.VisaAddressCollection[SerialPortAddr.AddrFlag] });
                    this._cbDataBit.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("CurSerialPortDataBit"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[SerialPortAddr.AddrFlag] });
                    //校验位
                    this._cbCheckBit.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("SerialPortCheckBit"), Source = m_ConnectCtrlHandle.VisaAddressCollection[SerialPortAddr.AddrFlag] });
                    this._cbCheckBit.SetBinding(ComboBox.SelectedIndexProperty, new Binding() { Path = new PropertyPath("CurrentCheckBitIndex"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[SerialPortAddr.AddrFlag] });
                    //停止位
                    this._cbStopBit.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("SerialPortStopBit"), Source = m_ConnectCtrlHandle.VisaAddressCollection[SerialPortAddr.AddrFlag] });
                    this._cbStopBit.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("CurSerialPortStopBit"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[SerialPortAddr.AddrFlag] });
                    #endregion
                    break;

                case VisaAddress.PortType.GPIB:
                    #region GPIB
                    _spLan.Visibility = Visibility.Collapsed;
                    _spSerialPort.Visibility = Visibility.Collapsed;
                    _spGpib.Visibility = Visibility.Visible;
                    _spUsb.Visibility = Visibility.Collapsed;
                    _spIoLib.Visibility = Visibility.Collapsed;
                    _spUserDefine.Visibility = Visibility.Collapsed;

                    //板卡编号
                    this._cbGPIBCardNums.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("CardNumCollection"), Source = m_ConnectCtrlHandle.VisaAddressCollection[GPIBAddr.AddrFlag] });
                    this._cbGPIBCardNums.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("CurCadNum"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[GPIBAddr.AddrFlag] });
                    //主板卡地址
                    //_cbGPIBMainAddr
                    this._cbGPIBMainAddr.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("AddrCollection"), Source = m_ConnectCtrlHandle.VisaAddressCollection[GPIBAddr.AddrFlag] });
                    this._cbGPIBMainAddr.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("MainAddr"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[GPIBAddr.AddrFlag] });
                    //从板卡地址
                    //_cbGPIBViceAddr
                    this._cbGPIBViceAddr.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("AddrCollection"), Source = m_ConnectCtrlHandle.VisaAddressCollection[GPIBAddr.AddrFlag] });
                    this._cbGPIBViceAddr.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("ViceAddr"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[GPIBAddr.AddrFlag] });
                    #endregion
                    break;

                //case VisaAddress.PortType.USB:
                //    #region USB
                //    _spLan.Visibility = Visibility.Collapsed;
                //    _spSerialPort.Visibility = Visibility.Collapsed;
                //    _spGpib.Visibility = Visibility.Collapsed;
                //    _spUsb.Visibility = Visibility.Visible;
                //    _spIoLib.Visibility = Visibility.Collapsed;
                //    _spUserDefine.Visibility = Visibility.Collapsed;
                //    #endregion
                //    break;

                case VisaAddress.PortType.IOLib:
                    #region 从IO库中进行查找
                    _spLan.Visibility = Visibility.Collapsed;
                    _spSerialPort.Visibility = Visibility.Collapsed;
                    _spGpib.Visibility = Visibility.Collapsed;
                    _spUsb.Visibility = Visibility.Collapsed;
                    _spIoLib.Visibility = Visibility.Visible;
                    _spUserDefine.Visibility = Visibility.Collapsed;
                    this._cbIOLib.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("VisaAddrCollection"), Source = m_ConnectCtrlHandle.VisaAddressCollection[IOLibAddr.AddrFlag] });
                    this._cbIOLib.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("CurSelectedVisaAddr"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[IOLibAddr.AddrFlag] });
                    #endregion
                    break;

                //case VisaAddress.PortType.UserDefine:
                //#region 自定义
                //_spLan.Visibility = Visibility.Collapsed;
                //_spSerialPort.Visibility = Visibility.Collapsed;
                //_spGpib.Visibility = Visibility.Collapsed;
                //_spUsb.Visibility = Visibility.Collapsed;
                //_spIoLib.Visibility = Visibility.Collapsed;
                //_spUserDefine.Visibility = Visibility.Visible;
                //this._tbVisaAddr.SetBinding(TextBox.TextProperty, new Binding() { Path = new PropertyPath("VisaAddr"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[UserDefineAddr.AddrFlag] });
                //#endregion
                //    break;
            }
        }

    }
}
