using RackSys.TestLab.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RAC_Test.View
{
    /// <summary>
    /// DlgDevicePropertyManage.xaml 的交互逻辑
    /// </summary>
    public partial class DlgDevicePropertyManage : Window
    {
        #region 私有成员
        private InstrumentInfo instrumentInfo = null;
        private InstrumentInfo tempInstrumentInfo = new InstrumentInfo();
        //
        private InstrumentAddress instrumentAddress = new InstrumentAddress();
        #endregion

        #region 构造函数和初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public DlgDevicePropertyManage()
        {
            InitializeComponent();
            //this.InitUI(0);

        }

        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="instrument">设备信息</param>
        /// <param name="imagesource">图片源</param>
        public DlgDevicePropertyManage(ref InstrumentInfo instrument, ImageSource imagesource)
        {
            InitializeComponent();
            // initCom();
            // this.ImageDevice.AddImage(imagesource);
            if (instrument != null)
            {
                if (instrument.InstrumentTypeID == InstrumentType.DCPowerAnalyzer || instrument.InstrumentTypeID == InstrumentType.DCPower)
                {
                    this.rowDCPower.Height = new GridLength(40, GridUnitType.Pixel);
                    this.labDevName.Content = "电源名称:";
                }
                if (instrument.InstrumentTypeID == InstrumentType.TemperatureMonitor)
                {
                    this.rowDCPower.Height = new GridLength(40, GridUnitType.Pixel);
                    // this.labDevName.Content = "电源名称:";
                }
                instrumentInfo = instrument;
                tempInstrumentInfo = instrumentInfo;
                this.ImageDevice.Source = imagesource;
                //this.InitUI(imagesource);
                this.LabelDeviceType.Content = instrument.InstrumentName;
                this.LabelDeviceModelNo.Content = instrument.ModelNo;
                this.txtInnerNum.Text = instrument.IDInInstitute;
                this.txtDevName.Text = instrument.InstrumentName;
                this.txtChanneNum.Text = instrument.DCModleNum.ToString();
                // this.txtInputAddress.Text = instrument.IpAddress;

                instrumentAddress.IsApply = true;
                //if (instrument.InstrumentTypeID == InstrumentType.MultiChannelController)
                //{
                //    instrumentAddress.InstrumentAddType = addrType.Other;
                //    instrumentAddress.InstrumentAddr = instrument.IpAddress;
                //    //this.rbInputAddress.IsEnabled = false;
                //    // this.rbIOLibAddr.IsEnabled = false;
                //    // this.comLanOrGpib.IsEnabled = false;
                //    // this.txtInputAddress.IsEnabled = false;
                //}
                //else
                //{
                //   initAddr(instrument.IpAddress);
                // }

                this.visaAddrSel.Init(instrument.IpAddress);

                if (this.instrumentInfo.DevInfoState == devState.connect)
                {
                    this.txtConRes.Text = "已连接 ";
                    this.txtConRes.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    this.txtConRes.Text = "未连接 ";
                    this.txtConRes.Foreground = new SolidColorBrush(Colors.Red);
                }
                this.txtIdetn.Text = this.instrumentInfo.Identity;

                this.mainGrid.DataContext = this.instrumentAddress;

            }
        }
        /// <summary>
        /// 初始化地址
        /// </summary>
        /// <param name="IPOrGpib"></param>
        //protected void initAddr(string IPOrGpib)
        //{
        //    if (string.IsNullOrEmpty(IPOrGpib))
        //    {
        //        return;
        //    }
        //    if (IPOrGpib.Contains("GPIB"))
        //    {
        //        this.comLanOrGpib.SelectedIndex = 1;

        //        int bordnum = -1;
        //        int mainnum = -1;
        //        int nextnum = -1;
        //        VisaServices.GetGPIBConnectionINfo(IPOrGpib, out bordnum, out mainnum, out nextnum);

        //        instrumentAddress.InstrumentAddType = addrType.GPIB;
        //        instrumentAddress.FNum = bordnum;
        //        instrumentAddress.SNum = mainnum;
        //        instrumentAddress.TNum = nextnum;

        //        this.comBordNum.SelectedValue = bordnum;
        //        this.comMainNum.SelectedValue = mainnum;
        //        this.comNextNum.SelectedValue = nextnum;
        //        this.txtVisaAddr.Text = IPOrGpib;
        //    }
        //    else
        //    {
        //        this.comLanOrGpib.SelectedIndex = 0;

        //        // this.txtInputAddress.Text = IPOrGpib;
        //        ////  this.txtVisaAddr.Text = VisaServices.CreateLanVisaResourceString(IPOrGpib);
        //        this.txtVisaAddr.Text = IPOrGpib;
        //        string ip;
        //        int port;
        //        VisaServices.GetLanConnectionInfo(this.txtVisaAddr.Text, out ip, out   port);
        //        instrumentAddress.InstrumentAddType = addrType.LAN;
        //        instrumentAddress.InstrumentIP = ip;
        //        instrumentAddress.IPPort = port;
        //        this.txtInputAddress.Text = ip;
        //    }

        //}

        protected void initBind()
        {
            Binding bd = new Binding();
            bd.Source = instrumentAddress;
            bd.Path = new PropertyPath("GPIBAddr");
            // bd.Path=new
            this.txtVisaAddr.SetBinding(System.Windows.Controls.TextBox.TextProperty, bd);
        }

        /// <summary>
        /// 设置显示仪表图片
        /// </summary>
        /// <param name="path"></param>
        private void InitUI(ImageSource path = null)
        {
            ImageSource[] Img;

            Img = new ImageSource[] { path, path };
            this.ImageDevice.Source = Img[0];

        }

        public int ImageIndex = 99;

        /// <summary>
        /// 初始化Combobox
        /// </summary>
        //protected void initCom()
        //{
        //    Dictionary<int, string> comDic = new Dictionary<int, string>()
        //    {
        //        {-1,"无"},
        //        {0,"0"},
        //        {1,"1"},
        //        {2,"2"},
        //        {3,"3"},
        //        {4,"4"},
        //        {5,"5"},
        //        {6,"6"},
        //        {7,"7"},
        //        {8,"8"},
        //        {9,"9"},
        //        {10,"10"},
        //        {11,"11"},
        //        {13,"12"},
        //    };

        //    this.comBordNum.ItemsSource = DictionaryCollection.BoardNumDic;

        //    this.comBordNum.DisplayMemberPath = "Value";
        //    this.comBordNum.SelectedValuePath = "Key";
        //    this.comMainNum.ItemsSource = DictionaryCollection.MainAndSecNumDic;
        //    this.comMainNum.SelectedValuePath = "Key";
        //    this.comMainNum.DisplayMemberPath = "Value";
        //    this.comNextNum.ItemsSource = DictionaryCollection.MainAndSecNumDic;
        //    this.comNextNum.SelectedValuePath = "Key";
        //    this.comNextNum.DisplayMemberPath = "Value";

        //    // this.comLanOrGpib.SelectedIndex = 0;
        //}
        #endregion

        #region 设备名称或者所内编号修改
        /// <summary>
        /// 设备名称
        /// </summary>
        private void txtDevName_GotFocus(object sender, RoutedEventArgs e)
        {
            instrumentAddress.IsApply = true;
        }

        /// <summary>
        /// 通道数量
        /// </summary>
        private void txtChanneNum_GotFocus(object sender, RoutedEventArgs e)
        {
            instrumentAddress.IsApply = true;
        }
        #endregion

        #region 输入地址配置--输入地址
        ///// <summary>
        ///// 输入地址选择radiobox
        ///// </summary>
        //private void rbInputAddress_Click(object sender, ActiproSoftware.Windows.Controls.Ribbon.Controls.ExecuteRoutedEventArgs e)
        //{
        //    if (this.rbInputAddress.IsChecked == true)
        //    {
        //        this.gbAddress.IsEnabled = true;
        //        this.rbIOLibAddr.IsChecked = false;
        //        this.ioLibsInterfacesComboBox.IsEnabled = false;
        //        if (0 == this.comLanOrGpib.SelectedIndex)
        //        {
        //            instrumentAddress.InstrumentAddType = addrType.LAN;
        //        }
        //        else if (1 == this.comLanOrGpib.SelectedIndex)
        //        {
        //            instrumentAddress.InstrumentAddType = addrType.GPIB;

        //        }
        //    }
        //    else
        //    {
        //        this.gbAddress.IsEnabled = false;
        //        this.rbIOLibAddr.IsChecked = true;
        //        this.ioLibsInterfacesComboBox.IsEnabled = true;
        //    }
        //}

        ///// <summary>
        ///// 切换地址显示方式LAN或者GPIB
        ///// </summary>
        //private void comLanOrGpib_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //    if (this.comLanOrGpib.SelectedIndex == 0)
        //    {
        //        this.txtInputAddress.Visibility = Visibility.Visible;
        //        this.txtInputAddressPort.Visibility = Visibility.Visible;
        //        this.labPort.Visibility = Visibility.Visible;
        //        this.grGPIB.Visibility = Visibility.Hidden;
        //        instrumentAddress.InstrumentAddType = addrType.LAN;
        //    }
        //    else if (this.comLanOrGpib.SelectedIndex == 1)
        //    {
        //        this.txtInputAddress.Visibility = Visibility.Hidden;
        //        this.txtInputAddressPort.Visibility = Visibility.Hidden;
        //        this.labPort.Visibility = Visibility.Hidden;
        //        this.grGPIB.Visibility = Visibility.Visible;
        //        instrumentAddress.InstrumentAddType = addrType.GPIB;

        //    }
        //}
        #endregion

        #region 输入地址配置--从IOLIB中选择地址
        ///// <summary>
        ///// 选择从IOLIB中选择地址的radiobox
        ///// </summary>
        //private void rbIOLibAddr_Click(object sender, ActiproSoftware.Windows.Controls.Ribbon.Controls.ExecuteRoutedEventArgs e)
        //{
        //    if (this.rbIOLibAddr.IsChecked == true)
        //    {
        //        this.rbInputAddress.IsChecked = false;
        //        this.gbAddress.IsEnabled = false;

        //        instrumentAddress.InstrumentAddType = addrType.IOAddr;

        //        initLib();
        //        this.ioLibsInterfacesComboBox.IsEnabled = true;
        //        if (ioLibsInterfacesComboBox.Items.Count > 0)
        //        {
        //            this.ioLibsInterfacesComboBox.SelectedIndex = 0;
        //            instrumentAddress.IOAddr = this.ioLibsInterfacesComboBox.SelectedValue.ToString();

        //        }

        //    }
        //    else
        //    {
        //        this.rbInputAddress.IsChecked = true;
        //        this.gbAddress.IsEnabled = true;
        //        this.ioLibsInterfacesComboBox.IsEnabled = false;

        //    }
        //}

        ///// <summary>
        ///// 从IO库选择通讯地址的commbox，改变选择值
        ///// </summary>
        //private void ioLibsInterfacesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (this.ioLibsInterfacesComboBox.SelectedIndex >= 0)
        //    {
        //        instrumentAddress.IOAddr = this.ioLibsInterfacesComboBox.SelectedValue.ToString();
        //    }
        //}

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
        #endregion

        #region 测试连接

        delegate bool TestConDelegate(string connectAddr);
        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestCon_Click(object sender, RoutedEventArgs e)
        {
            this.btnTestCon.IsEnabled = false;
            this.txtConRes.Text = "正在连接……";
            //去连接 设备
            if (!string.IsNullOrEmpty(txtVisaAddr.Text))
            {
                //  SystemHardware.SysHardware.TryConnect(txtVisaAddr.Text);
                TestConDelegate task = delegate (string connectAddr)
                {
                    return this.doTryTest(connectAddr);
                };
                task.BeginInvoke(txtVisaAddr.Text, EndTryTest, this);

            }
            else
            {
                MessageBox.Show("VISA地址为空", "消息提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        protected bool doTryTest(string connectAddr)
        {
            //Thread.Sleep(1000);
            return SystemHardware.SysHardware.TryConnect(connectAddr);

        }

        public void EndTryTest(IAsyncResult ar)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {

                this.btnTestCon.IsEnabled = true;
                TestConDelegate handler = (TestConDelegate)((AsyncResult)ar).AsyncDelegate;

                if (handler.EndInvoke(ar))
                {
                    this.instrumentInfo.DevInfoState = devState.connect;
                    this.txtConRes.Text = "测试连接成功！ ";
                    this.txtConRes.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    this.instrumentInfo.DevInfoState = devState.error;
                    this.txtConRes.Text = "测试连接失败！ ";
                    this.txtConRes.Foreground = new SolidColorBrush(Colors.Red);
                }
            });

        }

        #endregion

        #region 识别设备
        delegate string TryIdentifyDelegate(string connAddr, out bool idResult);
        /// <summary>
        /// 识别设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDevID_Click(object sender, RoutedEventArgs e)
        {
            this.btnDevID.IsEnabled = false;
            this.txtIdetn.Text = "正在识别……";
            bool idResult;
            //  string idInfo;
            if (!string.IsNullOrEmpty(txtVisaAddr.Text))
            {
                TryIdentifyDelegate tryIdentifyDelegate = doTryIdentify;
                tryIdentifyDelegate.BeginInvoke(txtVisaAddr.Text, out idResult, EndTryIdentify, this);

            }
            else
            {
                MessageBox.Show("VISA地址为空", "消息提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        protected string doTryIdentify(string connaddr, out bool idResult)
        {
            return SystemHardware.SysHardware.TryIdentifyInstrument(connaddr, out idResult);

        }

        protected void EndTryIdentify(IAsyncResult ar)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                this.btnDevID.IsEnabled = true;
                bool idResult;
                string idInfo;
                TryIdentifyDelegate handler = (TryIdentifyDelegate)((AsyncResult)ar).AsyncDelegate;

                idInfo = handler.EndInvoke(out idResult, ar);
                if (idResult)
                {

                    this.txtIdetn.Text = idInfo;
                    this.txtIdetn.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    this.txtIdetn.Text = "识别失败！" + idInfo;
                    this.txtIdetn.Foreground = new SolidColorBrush(Colors.Red);
                }
            });

        }
        #endregion

        #region 确定，应用，取消，恢复默认
        /// <summary>
        /// 取消设置
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 确定更改
        /// </summary>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtDevName.Text.Trim()))
            {
                MessageBox.Show("已更新设备名称为空,请先添写设备名称！", "提示消息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                var snList = (from q in InstrumentInfoList.getInstence()
                              where q != this.instrumentInfo &&
                              (q.InstrumentTypeID == InstrumentType.DCPower || q.InstrumentTypeID == InstrumentType.DCPowerAnalyzer)
                              select q.InstrumentName).ToList();
                if (snList != null && snList.Contains(this.txtDevName.Text))
                {
                    MessageBox.Show("电源名称不能相同，请修改！", "提示消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;


                }
            }
            if (!string.IsNullOrEmpty(this.txtInnerNum.Text.Trim()))
            {
                var snList = (from q in InstrumentInfoList.getInstence()
                              where q != this.instrumentInfo
                              select q.IDInInstitute).ToList();
                if (snList != null && snList.Contains(this.txtInnerNum.Text))
                {
                    if (MessageBox.Show("当前输入的所内编号已存在，是否强制修改？", "提示消息", MessageBoxButton.OKCancel, MessageBoxImage.Information)
                           == MessageBoxResult.OK)
                    {
                        SaveChange();
                        instrumentAddress.IsApply = false;
                    }
                    else
                    {
                        return;
                    }

                }
            }
            SaveChange();
            this.Close();
            //  MessageBox.Show("保存成功！","提示消息",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        /// <summary>
        /// 恢复默认
        /// </summary>
        //private void btnDefault_Click(object sender, RoutedEventArgs e)
        //{
        //    this.txtInnerNum.Text = tempInstrumentInfo.IDInInstitute;
        //    this.rbInputAddress.IsChecked = true;
        //    ActiproSoftware.Windows.Controls.Ribbon.Controls.ExecuteRoutedEventArgs ee = new ActiproSoftware.Windows.Controls.Ribbon.Controls.ExecuteRoutedEventArgs(0);

        //    rbInputAddress_Click(this.rbInputAddress, ee);

        //    initAddr(tempInstrumentInfo.IpAddress);
        //}


        //是否编辑完成
        public bool IsOk = false;

        /// <summary>
        /// 应用
        /// </summary>
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtDevName.Text.Trim()))
            {
                MessageBox.Show("已更新设备名称为空,请先添写设备名称！", "提示消息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                var snList = (from q in InstrumentInfoList.getInstence()
                              where q != this.instrumentInfo &&
                              (q.InstrumentTypeID == InstrumentType.DCPower || q.InstrumentTypeID == InstrumentType.DCPowerAnalyzer)
                              select q.InstrumentName).ToList();
                if (snList != null && snList.Contains(this.txtDevName.Text))
                {
                    MessageBox.Show("电源名称不能相同，请修改！", "提示消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;


                }
            }
            if (!string.IsNullOrEmpty(this.txtInnerNum.Text.Trim()))
            {
                var snList = (from q in InstrumentInfoList.getInstence()
                              where q != this.instrumentInfo
                              select q.IDInInstitute).ToList();
                if (snList != null && snList.Contains(this.txtInnerNum.Text))
                {
                    if (MessageBox.Show("当前输入的所内编号已存在，是否强制修改？", "提示消息", MessageBoxButton.OKCancel, MessageBoxImage.Information)
                           == MessageBoxResult.OK)
                    {
                        SaveChange();
                        instrumentAddress.IsApply = false;
                    }
                    else
                    {
                        return;
                    }

                }
            }

            SaveChange();
            instrumentAddress.IsApply = false;

        }

        /// <summary>
        /// 保存
        /// </summary>
        protected void SaveChange()
        {
            IsOk = true;
            this.instrumentInfo.IpAddress = txtVisaAddr.Text;
            if (!string.IsNullOrEmpty(this.txtInnerNum.Text))
            {
                this.instrumentInfo.IDInInstitute = this.txtInnerNum.Text;
            }
            else
            {
                this.instrumentInfo.IDInInstitute = "";
            }
            if (!string.IsNullOrEmpty(this.txtDevName.Text.ToString()) && this.txtDevName.Text.ToString() != this.instrumentInfo.InstrumentName)
            {
                this.instrumentInfo.InstrumentName = this.txtDevName.Text.Trim();
            }
            if (!string.IsNullOrEmpty(this.txtChanneNum.Text))
            {
                this.instrumentInfo.DCModleNum = Convert.ToInt32(this.txtChanneNum.Text.Trim());

            }
            InstrumentInfoList.getInstence().SaveParameterToXMLFile();
            ///巡检，以初始化设备信息
            SystemHardware.SysHardware.CreateInstrumentInstanceByInstrumentType(this.instrumentInfo.InstrumentTypeID);
        }

        private void txtInnerNum_GotFocus(object sender, RoutedEventArgs e)
        {
            instrumentAddress.IsApply = true;
        }
        #endregion

        private void visaAddrSel_VisaAddrChanged(object sender, EventArgs e)
        {
            //更改连接状态
            txtVisaAddr.Text = visaAddrSel.GetVisaAddr();
            txtConRes.Text = string.Empty;
            txtIdetn.Text = string.Empty;
            //更改“确定”按钮状态
            // BtnApplyEnable = true;
            instrumentAddress.IsApply = true;
        }
    }
}

