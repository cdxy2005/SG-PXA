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
using RackSys.TestLab.Visa;

namespace RAC_Test.DefControl
{
    /// <summary>
    /// VisaAddrControl.xaml 的交互逻辑
    /// </summary>
    public partial class VisaAddrControl : UserControl ,INotifyPropertyChanged
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

       private IOLibAddr iolibAddr;

        public VisaAddrControl()
        {
            InitializeComponent();
        }

        public void Init(string visaAddr) 
        {
            if (!string.IsNullOrEmpty(visaAddr))
            {
                m_ConnectCtrlHandle = new ConnectCtrlHandle(visaAddr);
                iolibAddr = new IOLibAddr();
                //初始化
                CurrentVisaAddress = m_ConnectCtrlHandle.CurrentVisaAddress;

                //数据绑定
                this.DataContext = this;

                #region 类型选择
                //类型选择
                this.comLanOrGpib.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("ConnectionTypeItems"), Source = this });
                this.comLanOrGpib.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("CurrentVisaAddress"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = this });

                #endregion
            }
                       
        }

        public string GetVisaAddr() 
        {
            //if (this.rbIOLibAddr.IsChecked == true)
            //{
            //    return this.ioLibsInterfacesComboBox.SelectedValue.ToString();
            //}
            //else
            //{
                return m_ConnectCtrlHandle.GetVisaAddr();
           // }
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


        #region 界面配置显示控制
        #endregion

        private void ShowStaticPanel(VisaAddress.PortType PortType)
        {
            switch (PortType)
            {
                case VisaAddress.PortType.Lan:
                    #region 网口
                    this.grLan.Visibility = Visibility.Visible;
                    
                    this.grCom.Visibility = Visibility.Collapsed;
                    this.grGPIB.Visibility = Visibility.Collapsed;

                    this.txtInputAddress.SetBinding(TextBox.TextProperty, new Binding() { Path = new PropertyPath("Ip"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[LanAddr.AddrFlag] });
                    this.txtInputAddressPort.SetBinding(TextBox.TextProperty, new Binding() { Path = new PropertyPath("Port"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[LanAddr.AddrFlag] });
                    #endregion
                    break;

                case VisaAddress.PortType.SerialsPort:
                    #region 串口
                    this.grLan.Visibility = Visibility.Collapsed;

                    this.grCom.Visibility = Visibility.Visible;
                    this.grGPIB.Visibility = Visibility.Collapsed;
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
                    this.grLan.Visibility = Visibility.Collapsed;

                    this.grCom.Visibility = Visibility.Collapsed;
                    this.grGPIB.Visibility = Visibility.Visible;

                    //板卡编号
                    this.comBordNum.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("CardNumCollection"), Source = m_ConnectCtrlHandle.VisaAddressCollection[GPIBAddr.AddrFlag] });
                    this.comBordNum.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("CurCadNum"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[GPIBAddr.AddrFlag] });
                    //主板卡地址
                    //_cbGPIBMainAddr
                    this.comMainNum.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("AddrCollection"), Source = m_ConnectCtrlHandle.VisaAddressCollection[GPIBAddr.AddrFlag] });
                    this.comMainNum.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("MainAddr"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[GPIBAddr.AddrFlag] });
                    //从板卡地址
                    //_cbGPIBViceAddr
                    this.comNextNum.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("AddrCollection"), Source = m_ConnectCtrlHandle.VisaAddressCollection[GPIBAddr.AddrFlag] });
                    this.comNextNum.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("ViceAddr"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[GPIBAddr.AddrFlag] });
                    #endregion
                    break;

                //case VisaAddress.PortType.USB:
                //    #region USB
                //    //_spLan.Visibility = Visibility.Collapsed;
                //    //_spSerialPort.Visibility = Visibility.Collapsed;
                //    //_spGpib.Visibility = Visibility.Collapsed;
                //    //_spUsb.Visibility = Visibility.Visible;
                //    //_spIoLib.Visibility = Visibility.Collapsed;
                //    //_spUserDefine.Visibility = Visibility.Collapsed;
                //    #endregion
                //    break;

                case VisaAddress.PortType.IOLib:
                    #region 从IO库中进行查找

                    this.ioLibsInterfacesComboBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Path = new PropertyPath("VisaAddrCollection"), Source = iolibAddr });
                    this.ioLibsInterfacesComboBox.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("CurSelectedVisaAddr"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = iolibAddr });
                    #endregion
                    break;

                //case VisaAddress.PortType.UserDefine:
                //    #region 自定义
                //    //_spLan.Visibility = Visibility.Collapsed;
                //    //_spSerialPort.Visibility = Visibility.Collapsed;
                //    //_spGpib.Visibility = Visibility.Collapsed;
                //    //_spUsb.Visibility = Visibility.Collapsed;
                //    //_spIoLib.Visibility = Visibility.Collapsed;
                //    //_spUserDefine.Visibility = Visibility.Visible;
                //    //this._tbVisaAddr.SetBinding(TextBox.TextProperty, new Binding() { Path = new PropertyPath("VisaAddr"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = m_ConnectCtrlHandle.VisaAddressCollection[UserDefineAddr.AddrFlag] });
                //    #endregion
                //    break;
            }
        }


        private void rbInputAddress_Click(object sender, RoutedEventArgs e)
        {
            if (this.rbInputAddress.IsChecked == true)
            {
                this.gbAddress.IsEnabled = true;
                this.rbIOLibAddr.IsChecked = false;
                this.ioLibsInterfacesComboBox.IsEnabled = false;
                CurrentVisaAddress = (VisaAddress)this.comLanOrGpib.SelectedValue;
                if (VisaAddrChanged != null)
                    VisaAddrChanged(sender, new EventArgs());//把按钮自身作为参数传递
            }
            else
            {
                this.gbAddress.IsEnabled = false;
                this.rbIOLibAddr.IsChecked = true;
                this.ioLibsInterfacesComboBox.IsEnabled = true;
            }
        }

        private void rbIOLibAddr_Click(object sender, RoutedEventArgs e)
        {
            if (this.rbIOLibAddr.IsChecked == true)
            {
                this.rbInputAddress.IsChecked = false;
                this.gbAddress.IsEnabled = false;

              // m_ConnectCtrlHandle.CurrentVisaAddress=  m_ConnectCtrlHandle.VisaAddressCollection[IOLibAddr.AddrFlag];
               // m_ConnectCtrlHandle.CurrentVisaAddress=ConnectionTypeItems[3];
               // initLib();
                m_ConnectCtrlHandle.CurrentVisaAddress = iolibAddr;
                ShowStaticPanel(VisaAddress.PortType.IOLib);
                this.ioLibsInterfacesComboBox.IsEnabled = true;
                if (ioLibsInterfacesComboBox.Items.Count > 0)
                {
                    if (this.ioLibsInterfacesComboBox.SelectedIndex == 0)
                    {
                        if (VisaAddrChanged != null)
                            VisaAddrChanged(sender, new EventArgs());//把按钮自身作为参数传递
                    }
                    else
                    {
                        this.ioLibsInterfacesComboBox.SelectedIndex = 0;
                    }
                    
                }

            }
            else
            {
                this.rbInputAddress.IsChecked = true;
                this.gbAddress.IsEnabled = true;
                this.ioLibsInterfacesComboBox.IsEnabled = false;

            }

        }

 




        ///// <summary>
        ///// 初始化IO库
        ///// </summary>
        //private void initLib()
        //{
        //    this.ioLibsInterfacesComboBox.Items.Clear();
        //    foreach (VisaDeviceResource resource in VisaDeviceExplorer.DiscoverLANResources())
        //    {
        //        this.ioLibsInterfacesComboBox.Items.Add(resource.Address.VisaResourceName);
        //    }
        //    foreach (VisaDeviceResource resource3 in VisaDeviceExplorer.DiscoverGPIBResources())
        //    {
        //        this.ioLibsInterfacesComboBox.Items.Add(resource3.Address.VisaResourceName);
        //    }
        //    foreach (VisaDeviceResource resource4 in VisaDeviceExplorer.DiscoverUSBResources())
        //    {
        //        this.ioLibsInterfacesComboBox.Items.Add(resource4.Address.VisaResourceName);
        //    }

        //}



    }
}
