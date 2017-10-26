using RAC_Test.View;
using RackSys.TestLab.Hardware;
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
using System.Windows.Threading;

namespace RAC_Test.DefControl
{
    /// <summary>
    /// imageControl.xaml 的交互逻辑
    /// </summary>
    public partial class imageControl : UserControl
    {

 
            private DispatcherTimer dptimer = null;
            private scalDirection scaldirection;
            //  private devType devtype = devType.none;

            public InstrumentInfo instrument = new InstrumentInfo();

            public static readonly DependencyProperty DevSourceProperty =
                DependencyProperty.Register("DevSource", typeof(BitmapSource), typeof(imageControl), new PropertyMetadata(null, new PropertyChangedCallback(OnDevSourceChanged)));

            public static readonly DependencyProperty DevStateProperty =
                DependencyProperty.Register("DevState", typeof(devState), typeof(imageControl), new PropertyMetadata(devState.normal, new PropertyChangedCallback(OnDevStateChanged)));
            public static readonly DependencyProperty DevStateHorizProperty =
                DependencyProperty.Register("DevStateHoriz", typeof(stateHorizontal), typeof(imageControl), new PropertyMetadata(stateHorizontal.left, new PropertyChangedCallback(onStateHorizChanged)));
            public static readonly DependencyProperty DevEnableProperty =
                DependencyProperty.Register("DevEnable", typeof(bool), typeof(imageControl), new PropertyMetadata(true, new PropertyChangedCallback(OnDevEnableChanged)));

            public static readonly DependencyProperty leftPNumProperty =
            DependencyProperty.Register("leftPNum", typeof(int), typeof(imageControl), new PropertyMetadata(0, new PropertyChangedCallback(onleftPNumChanged)));
            public static readonly DependencyProperty rightPNumProperty =
                DependencyProperty.Register("rightPNum", typeof(int), typeof(imageControl), new PropertyMetadata(0, new PropertyChangedCallback(onrightPNumChanged)));




            public imageControl()
            {
                InitializeComponent();
                dptimer = new DispatcherTimer();
                dptimer.Interval = TimeSpan.FromMilliseconds(50);
                dptimer.Tick += new EventHandler(dptimer_Tick);
                dptimer.IsEnabled = false;

                this.actLeftPoint = new Point(0, this.Height / 2);
                this.actRightPoint = new Point(this.Width, this.Height / 2);

                this.actUpPoint = new Point(this.Width / 2, 1);
                this.actDownPoint = new Point(this.Width / 2, this.Height);
            }

            /// <summary>
            /// 定时器工作函数
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void dptimer_Tick(object sender, EventArgs e)
            {
                // throw new NotImplementedException();
                changeSize();
            }

            protected void changeSize()
            {
                if (scaldirection == scalDirection.magnify)
                {
                    if (sscale.ScaleX < 1.2)
                    {
                        sscale.ScaleX += 0.05;
                        sscale.ScaleY += 0.05;

                    }
                    else
                    {
                        dptimer.Stop();
                    }
                }
                else
                {
                    if (sscale.ScaleX > 1)
                    {
                        sscale.ScaleX -= 0.05;
                        sscale.ScaleY -= 0.05;
                    }
                    else
                    {
                        dptimer.Stop();
                    }
                }

            }


            #region 设置设备图片
            public ImageSource DevSource
            {
                get
                {
                    return (ImageSource)GetValue(DevSourceProperty);
                }
                set
                {
                    SetValue(DevSourceProperty, value);
                }
            }

            private static void OnDevSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {

                imageControl ic = sender as imageControl;
                ic.dev.Source = ic.DevSource;

            }
            #endregion

            #region 设置设备状态图片
            public devState DevState
            {
                get
                {
                    return (devState)GetValue(DevStateProperty);
                }
                set
                {
                    SetValue(DevStateProperty, value);
                }
            }

            private static void OnDevStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                imageControl ic = sender as imageControl;
                if (ic.DevState == devState.normal)
                {
                Uri imageuri = new Uri("/RAC_Test;component/Img/init.png", UriKind.Relative);
                BitmapImage bitimg = new BitmapImage(imageuri);
                ic.state.Source = bitimg;
            }
                else if (ic.DevState == devState.connect)
                {
                    Uri imageuri = new Uri("/RAC_Test;component/Img/conent1.png", UriKind.Relative);
                    BitmapImage bitimg = new BitmapImage(imageuri);
                    ic.state.Source = bitimg;

                }
                else if (ic.DevState == devState.error)
                {
                    Uri imageuri = new Uri("/RAC_Test;component/Img/close1.png", UriKind.Relative);
                    BitmapImage bitimg = new BitmapImage(imageuri);
                    ic.state.Source = bitimg;
                }
                else if (ic.DevState == devState.alart)
                {
                    // /SchematicDiagramPlugin;component/UserControls/img/alart1.png
                    Uri imageuri = new Uri("/RAC_Test;component/Img/alart1.png", UriKind.Relative);
                    BitmapImage bitimg = new BitmapImage(imageuri);
                    ic.state.Source = bitimg;
                }
                else if (ic.DevState == devState.ischeck)
                {
                    Uri imageuri = new Uri("/RAC_Test;component/Img/indicate.png", UriKind.Relative);
                    BitmapImage bitimg = new BitmapImage(imageuri);
                    ic.state.Source = bitimg;
                }
                else
                {
                    Uri imageuri = new Uri("/RAC_Test;component/Img/close2.png", UriKind.Relative);
                    BitmapImage bitimg = new BitmapImage(imageuri);
                    ic.state.Source = bitimg;

                }

            }

            #endregion

            #region 设置图标位置
            public stateHorizontal DevStateHoriz
            {
                get
                {
                    return (stateHorizontal)GetValue(DevStateHorizProperty);
                }
                set
                {
                    SetValue(DevStateHorizProperty, value);
                }
            }
            private static void onStateHorizChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                imageControl ic = sender as imageControl;

                if (ic.DevStateHoriz == stateHorizontal.left)
                {
                    ic.state.HorizontalAlignment = HorizontalAlignment.Left;
                }
                else
                {
                    ic.state.HorizontalAlignment = HorizontalAlignment.Right;
                }

            }

            #endregion


            private devType m_devType;

            public devType DevType
            {
                get { return m_devType; }
                set { m_devType = value; }
            }


            /// <summary>
            /// 开关左边数量
            /// </summary>
            public int leftPNum
            {
                get { return (int)GetValue(leftPNumProperty); }
                set { SetValue(leftPNumProperty, value); }
            }

            private static void onleftPNumChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                imageControl ic = sender as imageControl;

                ic.actUpPoint = new Point(ic.Width / 2, 1);
                ic.actDownPoint = new Point(ic.Width / 2, ic.Height);

                //  no.leftPoint = new List<Point>();
                double leftHight = ic.Height / (ic.leftPNum + 1);
                for (int i = 0; i < ic.leftPNum; i++)
                {
                    Point pt = new Point();

                    pt.X = 1;    //固定半径为 1 PIX
                    pt.Y = leftHight * i + leftHight;
                    //  sw.leftPoint.Add(pt);

                    //保存结点
                    ic.actLeftPoint = pt;

                    // swCanvas.Children.Add(ellipse1);

                }


            }

            /// <summary>
            /// 开关右边数量
            /// </summary>
            public int rightPNum
            {
                get { return (int)GetValue(rightPNumProperty); }
                set { SetValue(rightPNumProperty, value); }
            }

            private static void onrightPNumChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                imageControl ic = sender as imageControl;
                //  no.rigthPoint = new List<Point>();
                double rightHeight = ic.Height / (ic.rightPNum + 1);

                for (int i = 0; i < ic.rightPNum; i++)
                {
                    Point pt = new Point();

                    pt.X = ic.Width;  //固定半径为 5 PIX
                    pt.Y = rightHeight * i + rightHeight;
                    //  sw.leftPoint.Add(pt);

                    //保存结点
                    ic.actRightPoint = pt;


                }

            }
            public bool DevEnable
            {
                get
                {
                    return (bool)GetValue(DevEnableProperty);
                }
                set
                {
                    SetValue(DevEnableProperty, value);

                }
            }
            protected static void OnDevEnableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                imageControl ic = sender as imageControl;
                if (ic.DevEnable)
                {
                    //设置状态
                    ic.DevState = devState.normal;
                    ic.dev.Opacity = 1;
                    //if (ic.imgs != null)
                    //{
                    //    ic.dev.Source = ic.imgs;
                    //}
                    //ic.devCal.IsEnabled = true;
                    //ic.devAtr.IsEnabled = true;
                    //ic.devEnable.IsChecked = true;

                    ic.instrument.DevInfoState = devState.normal;

                }
                else
                {
                    ic.DevState = devState.disenable;
                    // ic.dev.Opacity = 0.5;
                    //ic.devCal.IsEnabled = false;
                    //ic.devAtr.IsEnabled = false;
                    //ic.devEnable.IsChecked = false;

                    ic.instrument.DevInfoState = devState.disenable;
                    //ic.state.Width = ic.dev.Width;
                    //ic.state.Height = ic.dev.Height;
                    // ic.state.Opacity = 0.6;

                    //if (ic.imgs == null)
                    //{
                    //    ic.imgs = ic.dev.Source;
                    //}
                    // FormatConvertedBitmap bitmap = new FormatConvertedBitmap();
                    // bitmap.BeginInit();
                    // bitmap.Source = (BitmapSource)ic.dev.Source;
                    // bitmap.DestinationFormat = PixelFormats.Gray32Float;

                    // bitmap.EndInit();

                    // ic.dev.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => { ic.dev.Source = bitmap; }));

                }

            }
            /// <summary>
            /// 双击事件 
            /// </summary>
            protected void get_devInfo(object sender, MouseButtonEventArgs e)
            {
                if (DevEnable)
                {
                    if (e.ClickCount == 2)
                    {
                        if (this.DevType == devType.normal)
                        {

                            DlgDevicePropertyManage devAttrDlg = new DlgDevicePropertyManage(ref this.instrument, this.dev.Source);

                            devAttrDlg.ShowDialog();

                            e.Handled = true;
                        }
                        else if (this.DevType == devType.list)
                        {
                            //DCPowerDisplay dcPowerDlg = new DCPowerDisplay();
                            //dcPowerDlg.ShowDialog();
                            //e.Handled = true;

                        }
                    }
                }
                if (e.ChangedButton == MouseButton.Right)
                {
                    if (this.instrument == null)
                    {
                        this.conMenu.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        this.conMenu.Visibility = Visibility.Visible;

                    }
                }
            }

            public Point actLeftPoint
            {
                get;
                set;
            }

            public Point actRightPoint
            {
                get;
                set;
            }


            public Point actUpPoint
            {
                get;
                set;
            }

            public Point actDownPoint
            {
                get;
                set;
            }

            private void Grid_Loaded(object sender, RoutedEventArgs e)
            {


                this.dev.Tag = this.Tag;
                // this.dev.ContextMenu.Visibility = Visibility.Collapsed;

                this.sscale.CenterX = this.Width / 2;
                this.sscale.CenterY = this.Height / 2;


            }
            /// <summary>
            /// 鼠标移入事件
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void Grid_MouseEnter(object sender, MouseEventArgs e)
            {
                scaldirection = scalDirection.magnify;
                if (DevEnable)
                {
                    this.zz.Color = Color.FromArgb(255, 97, 31, 245);
                    this.zz.BlurRadius = 20;
                    Panel.SetZIndex(this, 101);
                    dptimer.Start();
                }
            }

            /// <summary>
            /// 鼠标移走事件 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void Grid_MouseLeave(object sender, MouseEventArgs e)
            {
                scaldirection = scalDirection.reduce;

                this.zz.Color = Colors.White;
                this.zz.BlurRadius = 0;
                Panel.SetZIndex(this, 100);
                dptimer.Start();

            }

            /// <summary>
            /// 设置设备属性
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void devAtr_Click(object sender, RoutedEventArgs e)
            {
                if (instrument.InstrumentTypeID == InstrumentType.TemperatureMonitor)
                {
                    //new DlgVIDXJDeviceProperty(ref this.instrument, this.dev.Source).ShowDialog();
                }
                else
                {
                    DlgDevicePropertyManage devAttrDlg = new DlgDevicePropertyManage(ref this.instrument, this.dev.Source);

                    devAttrDlg.ShowDialog();
                }

            }

            /// <summary>
            /// 校准设备
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void devCal_Click(object sender, RoutedEventArgs e)
            {
                //if (this.instrument.InstrumentTypeID == InstrumentType.AnalogSg)
                //{
                //    RackSys.TestLab.Calibration.VSGOutputPowerWizard wizard = new Calibration.VSGOutputPowerWizard();
                //    wizard.ShowDialog();

                //}
                //else if (this.instrument.InstrumentTypeID == InstrumentType.VectorSg)
                //{
                //    RackSys.TestLab.Calibration.VSGOutputPowerWizard wizard = new Calibration.VSGOutputPowerWizard();
                //    wizard.ShowDialog();

                //}
                //else if (this.instrument.InstrumentTypeID == InstrumentType.NoiceSource)
                //{
                //    RackSys.TestLab.Calibration.NoiseFigureCalWizard wizard = new Calibration.NoiseFigureCalWizard();
                //    wizard.ShowDialog();

                //}
                //else if (this.instrument.InstrumentTypeID == InstrumentType.Pna)
                //{
                //    //RackSys.TestLab.Calibration.NAPowerWizard wizard = new Calibration.NAPowerWizard();
                //    //wizard.ShowDialog();

                //}
                //else if (this.instrument.InstrumentTypeID == InstrumentType.PowerMeterInCabinet)
                //{
                //    RackSys.TestLab.Calibration.PowerMeterZeroAndCalWizard wizard = new Calibration.PowerMeterZeroAndCalWizard();
                //    wizard.ShowDialog();

                //}
                //else if (this.instrument.InstrumentTypeID == InstrumentType.PowerMeterOutofCabinet)
                //{
                //    RackSys.TestLab.Calibration.PowerMeterZeroAndCalWizard wizard = new Calibration.PowerMeterZeroAndCalWizard();
                //    wizard.ShowDialog();

                //}
                //else
                //{
                //    MessageBox.Show("该设备不需要校准", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                //}

            }

            /// <summary>
            /// 是否启用设备
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void devEnable_Click(object sender, RoutedEventArgs e)
            {

                //启用设备
                if (this.devEnable.IsChecked)
                {
                    this.DevEnable = true;
                    //this.instrument.DevInfoState = devState.normal;
                    // this.instrument.Enabled = true;
                    this.dptimer.Start();
                    SystemHardware.SysHardware.CreateInstrumentInstanceByInstrumentType(this.instrument.InstrumentTypeID);
                }
                //停用设备
                else
                {
                    this.DevEnable = false;
                    this.instrument.DevInfoState = devState.disenable;
                    this.instrument.Enabled = false;
                    this.dptimer.Stop();
                }
            }
        }


        /// <summary>
        /// 设备状态图标位置
        /// </summary>
        public enum stateHorizontal
        {
            left,
            right
        }

        /// <summary>
        /// 图片放大或缩小
        /// </summary>
        public enum scalDirection
        {
            magnify,
            reduce
        }

        public enum devType
        {
            normal,
            list,
            none
        }
    
}
