using RAC_Test.DefControl;
using RAC_Test.Test;
using RAC_Test.View;
using RackSys.TestLab;
using RackSys.TestLab.DataAccess;
using RackSys.TestLab.Hardware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Forms;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using RackSys.TestLab.Instrument;

namespace RAC_Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //定义全局的设备列表
        // public static InstrumentInfoList instrumentList = InstrumentInfoList.getInstence();
        private Test_Paramter CurTestParam;
        private Test_DoTest DoTest;
        private DUT CurCtrlDut = null;
        private InstrumentInfo DutInfo = null;
        public MainWindow()
        {
            InitializeComponent();

            //CtrlFreq(1e9);
            InitInstrument();
            this.txtNFCal.Text = NoiseFigureCal.noiseResFile;
           
            CurTestParam = Test_Paramter.CurPWDCPowerInfo;
            //this.txtInCal.Text = CurTestParam.InputFile;
            //this.txtOutCal.Text = CurTestParam.OutputFile;
            if (CurTestParam.isUpConverFreq)
            {
                this.rbUp.IsChecked = true;
                this.rbDown.IsChecked = false;
                this.textBoxActiveInput.Text = InputAndOutputCal.ActiveUpINPUTLossFile;
                this.textBoxActiveOutput.Text = InputAndOutputCal.ActiveUpOUTPUTLossFile;
                //this.PassiveTxtBox.Text = InputAndOutputCal.ActiveDownOUTPUTLossFile;
            }
            else
            {
                this.rbUp.IsChecked = false;
                this.rbDown.IsChecked = true;
                this.textBoxActiveInput.Text = InputAndOutputCal.ActiveUpINPUTLossFile;
                this.textBoxActiveOutput.Text = InputAndOutputCal.ActiveUpOUTPUTLossFile;
                //this.PassiveTxtBox.Text = InputAndOutputCal.ActiveDownOUTPUTLossFile;
            }
            if (CurTestParam.isTxWork)
            {
                this.rbTx.IsChecked = true;
                this.rbRx.IsChecked = false;
            }
            else
            {
                this.rbTx.IsChecked = false;
                this.rbRx.IsChecked = true;
            }
            this.gridParam.DataContext = CurTestParam;
            this.gridSGAndPxa.DataContext = CurTestParam.SGAndPxaParamter;
            this.gridDutAndPxa.DataContext = CurTestParam.DUTAndPxaParamter;
            this.gridNoise.DataContext = CurTestParam.NoiseAndPxaParamter;
            DoTest = new Test_DoTest();
            //double freq = 100 * 1e9;
            //string  str=Convert.ToString((long)freq,2);
            //int abc = 1;
            //byte[] byteValue = BitConverter.GetBytes(abc);
            //char xxx = (Char)byteValue[0];

        }

        private void InitInstrument()
        {
            //初始化控件信息
            foreach (InstrumentInfo instrumnet in InstrumentInfoList.getInstence())
            {

                System.Windows.Data.Binding bd = new System.Windows.Data.Binding("DevInfoState");
                bd.Mode = BindingMode.TwoWay;
                System.Windows.Data.Binding bdEnable = new System.Windows.Data.Binding("Enabled");
                bdEnable.Mode = BindingMode.TwoWay;

                //
                if (instrumnet.InstrumentTypeID == InstrumentType.SignalSource)
                {
                    this.SG.instrument = instrumnet;
                    this.SG.SetBinding(imageControl.DevEnableProperty, bdEnable);
                    this.SG.SetBinding(imageControl.DevStateProperty, bd);

                    this.SG.DataContext = instrumnet;
                }
                //频谱仪
                if (instrumnet.InstrumentTypeID == InstrumentType.SA)
                {
                    this.PXA.instrument = instrumnet;
                    this.PXA.SetBinding(imageControl.DevEnableProperty, bdEnable);
                    this.PXA.SetBinding(imageControl.DevStateProperty, bd);

                    this.PXA.DataContext = instrumnet;
                }
            }
            DutInfo = InstrumentInfoList.getInstence().GetInstrument("RackSys.TestLab.Instrument.DUT", "1");
            //this.cmbPort.ItemsSource = new List<string>()
            //{
            //    "COM1"
            //};
        }

        ///// <summary>
        ///// 导入输出校准文件
        ///// </summary>
        //private void btnOutput_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog ofd = new OpenFileDialog();
        //    ofd.Filter = "(*.txt)|*.txt";
        //    //ofd.DefaultExt = "*.txt";
        //    ofd.FilterIndex = 1;
        //    //记忆上次打开的文件路径
        //    ofd.RestoreDirectory = true;
        //    DialogResult dialogResult = ofd.ShowDialog();
        //    //if (DialogResult.OK != dialogResult && DialogResult.Yes != dialogResult)
        //    //{
        //    //    return;
        //    //}

        //    //选择的文件名称
        //    string tmpFilePathAndName = ofd.FileName;
        //    textBoxOutput.Text = tmpFilePathAndName;
        //    TextReader reader = null;
        //    try
        //    {
        //        reader = new StreamReader(File.Open(tmpFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        //        string text = reader.ReadToEnd();
        //        reader.Close();
        //        string[] textArray = text.Split(new char[] { ' ', '\t', ',', '\n', '\r' });
        //        ArrayList list = new ArrayList();
        //        foreach (string text2 in textArray)
        //        {
        //            if (text2.Length > 0)
        //            {
        //                list.Add(Convert.ToDouble(text2, NumberFormatInfo.InvariantInfo));
        //            }
        //        }
        //        int num = ((int)Math.Floor((double)(list.Count / 2))) * 2;
        //        int num2 = num / 2;
        //        double[] numArray = (double[])list.ToArray(typeof(double));
        //        double[] DataFreq = new double[num2];
        //        double[] DataFreqOffset = new double[num2];
        //        //DUTHandler.CurDUT.AntennaInfo.OffsetBaseFreqList.Clear();
               
        //         List < offsetFreq > offset=new List<offsetFreq>();
        //        int index = 0;
        //        for (int j = 0; j < num; j += 2)
        //        {
        //            DataFreq[index] = numArray[j];
        //            DataFreqOffset[index] = numArray[j + 1];
        //            index++;
        //            offset.Add
        //                (
        //                    new offsetFreq()
        //                    {
        //                        FreqInMHz = numArray[j],
        //                        ReceiverOffsetFreqInMHz = numArray[j + 1]
        //                    }
        //                );
        //        }
        //        Test_Paramter.CurPWDCPowerInfo.OutputFileoffst = offset;
              
        //    }
        //    catch (Exception)
        //    {
        //        if (reader != null)
        //        {
        //            reader.Close();
        //        }
        //        System.Windows.MessageBox.Show("输出校准文件解析错误，请检查文件内容是否符合相应规范。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}
        
        ///// <summary>
        ///// 导入输入校准文件
        ///// </summary>
        //private void btnInput_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog ofd = new OpenFileDialog();
        //    ofd.Filter = "(*.txt)|*.txt";
        //    //ofd.DefaultExt = "*.txt";
        //    ofd.FilterIndex = 1;
        //    //记忆上次打开的文件路径
        //    ofd.RestoreDirectory = true;
        //    DialogResult dialogResult = ofd.ShowDialog();
        //    //if (DialogResult.OK != dialogResult && DialogResult.Yes != dialogResult)
        //    //{
        //    //    return;
        //    //}

        //    //选择的文件名称
        //    string tmpFilePathAndName = ofd.FileName;
        //    textBoxInput.Text = tmpFilePathAndName;
        //    TextReader reader = null;
        //    try
        //    {
        //        reader = new StreamReader(File.Open(tmpFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        //        string text = reader.ReadToEnd();
        //        reader.Close();
        //        string[] textArray = text.Split(new char[] { ' ', '\t', ',', '\n', '\r' });
        //        ArrayList list = new ArrayList();
        //        foreach (string text2 in textArray)
        //        {
        //            if (text2.Length > 0)
        //            {
        //                list.Add(Convert.ToDouble(text2, NumberFormatInfo.InvariantInfo));
        //            }
        //        }
        //        int num = ((int)Math.Floor((double)(list.Count / 2))) * 2;
        //        int num2 = num / 2;
        //        double[] numArray = (double[])list.ToArray(typeof(double));
        //        double[] DataFreq = new double[num2];
        //        double[] DataFreqOffset = new double[num2];
        //        //DUTHandler.CurDUT.AntennaInfo.OffsetBaseFreqList.Clear();
                
        //         List < offsetFreq >  offlist=new List<offsetFreq>();
        //        int index = 0;
        //        for (int j = 0; j < num; j += 2)
        //        {
        //            DataFreq[index] = numArray[j];
        //            DataFreqOffset[index] = numArray[j + 1];
        //            index++;
        //            offlist.Add
        //                (
        //                    new offsetFreq()
        //                    {
        //                        FreqInMHz = numArray[j],
        //                        ReceiverOffsetFreqInMHz = numArray[j + 1]
        //                    }
        //                );
        //        }
        //        Test_Paramter.CurPWDCPowerInfo.InputFileoffst = offlist;
        //    }
        //    catch (Exception)
        //    {
        //        if (reader != null)
        //        {
        //            reader.Close();
        //        }
        //        System.Windows.MessageBox.Show("输入校准文件解析错误，请检查文件内容是否符合相应规范。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }

        //}

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemHardware systemHardware = SystemHardware.SysHardware;
                systemHardware.Connect();

            }
            catch (TargetInvocationException ex)
            {

                GlobalStatusReport.ReportError(ex);
            }

        }

        TestExecutionResult m_ExecuteResult = TestExecutionResult.Error;
        /// <summary>
        /// 执行测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void doTest_Click(object sender, RoutedEventArgs e)
        {
            Test_DoTest.stopRun = false;
            #region 保存测试参数到集合
            CurTestParam.SaveParameterToXMLFile();
            //string plugName = typeof(TPNATestWizard).Assembly.GetName().Name;
            //ProcedureParam param = new ProcedureParam();
            //param.PluginName = plugName;
            //param.TestTime = DateTime.Now;
            //param.CurrentDUT = this._TestParam.CurDUTInfo;
            //param.TestParamByte = TPNATestParam.SerializeObject(this._TestParam);

            //SystemProcedureParam.CurSystemProcedureParam.TestProduceParamList.Add(param);
            //SystemProcedureParam.CurSystemProcedureParam.SaveParameterToXMLFile();
            #endregion

            ChangeBtnState(TestExecutionResult.Init);

            //图片初始化


            //响应测试进度消息

            GlobalStatusReport.ErrorReported += new EventMessageReport(this.ErrReported);

            try
            {
                Action<SystemHardware, Test_Paramter> task = (inHardwareSys, inDutInfo) =>
                {
                    DoTest.AutoRun(inHardwareSys, CurTestParam, out m_ExecuteResult);
                };
                task.BeginInvoke(SystemHardware.SysHardware, CurTestParam, TestCompleteCallback, this);
            }
            catch (Exception ex)
            {
            }
            finally
            {

            }
        }


        #region 测试结束回调
        /// <summary>
        /// 测试结束
        /// </summary>
        /// <param name="ar"></param>
        private void TestCompleteCallback(IAsyncResult ar)
        {

            GlobalStatusReport.ErrorReported -= new EventMessageReport(this.ErrReported);
            ChangeBtnState(m_ExecuteResult);
        }
        #endregion

        #region 测试中的消息处理
        /// <summary>
        /// 改变按钮状态
        /// </summary>
        /// <param name="enableFlag"></param>
        public delegate void ChangeBtnStateHandler(TestExecutionResult resultFlag);
        private void ChangeBtnState(TestExecutionResult resultFlag)
        {
            if (this.CheckAccess())
            {
                this.pBar.IsIndeterminate = false;
                this.pBar.Value = 100;
                //如果通过
                if (TestExecutionResult.Complete == resultFlag)
                {
                    this.pBar.IsIndeterminate = false;
                    this.pBar.Value = 100;
                    System.Windows.MessageBox.Show("测试完成！", "提示", MessageBoxButton.OK);

                }
                else if (TestExecutionResult.Error == resultFlag)
                {
                    this.pBar.IsIndeterminate = false;
                    this.pBar.Value = 100;
                    System.Windows.MessageBox.Show("测试期间发生错误！", "提示", MessageBoxButton.OK);

                }
                else if (TestExecutionResult.Init == resultFlag)
                {
                    this.pBar.Value = 0;
                    this.pBar.IsIndeterminate = true;
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(new ChangeBtnStateHandler(this.ChangeBtnState), new object[] { resultFlag });
            }
        }


        public delegate void MessageReportedHandler(object sender, string inReportMessage, bool inAddAfterDelete);
        /// <summary>
        /// 报告错误信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="inReportMessage"></param>
        /// <param name="inAddAfterDelete"></param>
        public void ErrReported(object sender, string inReportMessage, bool inAddAfterDelete)
        {
            if (this.CheckAccess())
            {
                //报告错误信息，inReportMessage
                System.Windows.MessageBox.Show(inReportMessage, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                this.Dispatcher.BeginInvoke(new MessageReportedHandler(this.ErrReported), new object[] { sender, inReportMessage, inAddAfterDelete });
            }
        }

        #endregion

        private string tempExcel = AppDomain.CurrentDomain.BaseDirectory + @"\数据模板\Test.xlsx";
        private string reportExcel = "";
        protected void SaveExcel()
        {
            reportExcel = "";
            #region 生成 EXCEL和XML文件
            string rootPath =  @"D:\源+频谱仪TestDATA\";
            string ToSaveXMLFileName = string.Format("{0}\\测试结果{1}.xlsx", rootPath, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

            FileInfo templateFile = new FileInfo(tempExcel);
            if (!templateFile.Exists)
            {
                string ErrorInfo = "错误：" + tempExcel + " 不在指定目录下,请仔细检查并放置于指定目录下.";
                //MessageBox.Show(ErrorInfo, "Excel模板文件丢失", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //GlobalStatusReport.ReportError(ErrorInfo);
                throw new Exception(ErrorInfo);
            }
            else
            {
                //拷贝数据到当前的位置来；
                FileInfo fileinfo = new FileInfo(ToSaveXMLFileName);
                string FilePath = System.IO.Path.GetDirectoryName(fileinfo.FullName);
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }

                templateFile.CopyTo(fileinfo.FullName);

                Test_ResultForExcel inToSaveInfos = new Test_ResultForExcel(DoTest.CurResult);

                //保存结果到XML文件中

                //保存结果到XML文件中
                ExcelHelper.SaveObjectToExcelFile(inToSaveInfos.GetType(), inToSaveInfos, ToSaveXMLFileName);
                
                ///todo：在测试数据库Excel版本中记下一条单指标测试的测试数据记录；

                reportExcel= ToSaveXMLFileName;
                #endregion

            }

        }

        /// <summary>
        /// 打开报告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.pReportBar.Value = 0;
                this.pReportBar.IsIndeterminate = true;
                Action task = () =>
                {
                    SaveExcel();
                   
                };
                task.BeginInvoke(ReportCompleteCallback, this);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                this.pReportBar.IsIndeterminate = false;
            }
        }

        /// <summary>
        /// 测试结束
        /// </summary>
        /// <param name="ar"></param>
        private void ReportCompleteCallback(IAsyncResult ar)
        {
            OpenReport();


        }

        protected  delegate void OpenReportHandler();
        /// <summary>
        /// 打开报告
        /// </summary>
        private void OpenReport()
        {
            if (this.CheckAccess())
            {
                this.pReportBar.IsIndeterminate = false;
                this.pReportBar.Value = 100;
                //DlgResultViewer dlg = new DlgResultViewer(reportExcel);
                //dlg.ShowDialog();
                System.Diagnostics.Process.Start(reportExcel);
            }
            else
            {
                this.Dispatcher.BeginInvoke(new OpenReportHandler(OpenReport));
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

 
        /// <summary>
        /// 执行校准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void doCal_Click(object sender, RoutedEventArgs e)
        {
            Action<SystemHardware, Test_Paramter> task = (inHardwareSys, inDutInfo) =>
            {
                NoiseFigureCal.runExecute(inHardwareSys, CurTestParam);
            };
            this.IsEnabled = false;
            task.BeginInvoke(SystemHardware.SysHardware, CurTestParam, CalCompleteCallback, this);
            System.Windows.MessageBox.Show("正在执行噪声校准……", "校准");
        }

        private void CalCompleteCallback(IAsyncResult ar)
        {
            CloseMessage();
        }

        private void CloseMessage()
        {
            if (this.CheckAccess())
            {
                //查找MessageBox的弹出窗口,注意MessageBox对应的标题
                this.IsEnabled = true;
                IntPtr ptr = FindWindow(null, "校准");
                if (ptr != IntPtr.Zero)
                {
                    //查找到窗口则关闭
              PostMessage(ptr, 0x0010, IntPtr.Zero, IntPtr.Zero);
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(new OpenReportHandler(CloseMessage));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.rbDown.IsChecked = false;
            this.CurTestParam.isUpConverFreq = true;
            this.textBoxActiveInput.Text = InputAndOutputCal.ActiveUpINPUTLossFile;
            this.textBoxActiveOutput.Text = InputAndOutputCal.ActiveUpOUTPUTLossFile;
            //this.PassiveTxtBox.Text = InputAndOutputCal.ActiveDownOUTPUTLossFile;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            this.rbUp.IsChecked = false;
            this.CurTestParam.isUpConverFreq = false;
            this.textBoxActiveInput.Text = InputAndOutputCal.ActiveUpINPUTLossFile;
            this.textBoxActiveOutput.Text = InputAndOutputCal.ActiveUpOUTPUTLossFile;
            //this.PassiveTxtBox.Text = InputAndOutputCal.ActiveDownOUTPUTLossFile;
        }

        private void doInCal_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            Action<SystemHardware, Test_Paramter> task = (inHardwareSys, inDutInfo) =>
            {
                NoiseFigureCal.runExecute(inHardwareSys, CurTestParam);
            };
            task.BeginInvoke(SystemHardware.SysHardware, CurTestParam, CalCompleteCallback, this);
            System.Windows.MessageBox.Show("正在执行输入校准……", "校准");
        }

      //  AppDomain.CurrentDomain.BaseDirectory+"\"+File;
        private void doOutCal_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            Action<SystemHardware, Test_Paramter> task = (inHardwareSys, inDutInfo) =>
            {
                NoiseFigureCal.runExecute(inHardwareSys, CurTestParam);
            };
            task.BeginInvoke(SystemHardware.SysHardware, CurTestParam, CalCompleteCallback, this);
            System.Windows.MessageBox.Show("正在执行输出校准……", "校准");
        }

        /// <summary>
        /// 无源输出校准（和，差变频 ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void btnPassive_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            string file = InputAndOutputCal.ActiveUpINPUTLossFile;
            //Action<SystemHardware, Test_Paramter> task = (inHardwareSys, inDutInfo) =>
            //{
            InputAndOutputCal.runExecute(SystemHardware.SysHardware, CurTestParam, file, false);
          
            //};
            //task.BeginInvoke(SystemHardware.SysHardware, CurTestParam, CalCompleteCallback, this);
            System.Windows.MessageBox.Show("正在执行输出校准……", "校准");
            this.IsEnabled = false;
        }

        /// <summary>
        /// 有源输入路损校准（和，差变频 ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInput_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            //Action<SystemHardware, Test_Paramter> task = (inHardwareSys, inDutInfo) =>
            //{
            string file = InputAndOutputCal.ActiveUpINPUTLossFile;
            SystemHardware inHardwareSys =SystemHardware.SysHardware;
                InputAndOutputCal.runExecute(inHardwareSys, CurTestParam, file, true);
            //};
            //task.BeginInvoke(SystemHardware.SysHardware, CurTestParam, CalCompleteCallback, this);
            System.Windows.MessageBox.Show("输入校准完成！", "校准");
            this.IsEnabled = true;
        }


        /// <summary>
        /// 有源输出路损校准（和，差变频 ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOutput_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            //Action<SystemHardware, Test_Paramter> task = (inHardwareSys, inDutInfo) =>
            //{
            string file = InputAndOutputCal.ActiveUpOUTPUTLossFile;
            SystemHardware inHardwareSys = SystemHardware.SysHardware;
            InputAndOutputCal.runExecute(inHardwareSys, CurTestParam, file, false);
            //};
            //task.BeginInvoke(SystemHardware.SysHardware, CurTestParam, CalCompleteCallback, this);
            System.Windows.MessageBox.Show("输出校准完成！", "校准");
            this.IsEnabled = true;
        }

        private void btnCon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //DUT TRDut = SystemHardware.Hardware<DUT>.GetElmentByName("RackSys.TestLab.Instrument.DUT1");
                //DutInfo.InstrumentTypeID = InstrumentType.;
                CurCtrlDut = DUT.Connect(DutInfo.IpAddress);
                DoTest.CurCtrlDut = CurCtrlDut;
            }
            catch (Exception ex)
            {

               System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
          

        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            DlgDevicePropertyManage DlgDev = new DlgDevicePropertyManage(ref DutInfo,null);
            DlgDev.ShowDialog();
        }

        private void rbTx_Checked(object sender, RoutedEventArgs e)
        {
            this.rbRx.IsChecked = false;
            this.CurTestParam.isTxWork = true;
         
        }

        private void rbRx_Checked(object sender, RoutedEventArgs e)
        {
            this.rbTx.IsChecked = false;
            this.CurTestParam.isTxWork = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            int startIndex = 0;
            int testLength = 0;
            if (!Int32.TryParse(this.txtStartIndex.Text, out startIndex))
            {
               System.Windows.Forms.MessageBox.Show("起始位置输入错误！");
                return;
            }
            if (!Int32.TryParse(this.txtTestLength.Text, out testLength))
            {
                System.Windows.Forms.MessageBox.Show("数据长度错误！");
                return;
            }
            if (DoTest.CurResult == null)
            {
                return;
            }

            var outTestList =( from q in DoTest.CurResult.TestResultList
                               orderby q.TestFreq ascending
                               select q).ToList();
            var outminGain = (from q in DoTest.CurResult.TestResultList
                                orderby q.TestFreq descending
                                select q.Gain).Min();
            
            int dataLength = 0;
            CurCtrlDut.SETChannel(this.CurTestParam.ChannelNumber);
            if (outTestList != null && outTestList.Count > startIndex)
            {
                try
                {
                    double inPutPower=this.CurTestParam.SGAndPxaParamter.InPower;
                    foreach(var res in outTestList)
                    {
                        dataLength++;
                        if (dataLength < testLength)
                        {
                            CurCtrlDut.CtrlGain(res.TestFreq, res.Gain- outminGain, false);
                        }
                        else
                        {
                            CurCtrlDut.CtrlGain(res.TestFreq, res.Gain - outminGain, false);
                            CurCtrlDut.CtrlGain(res.TestFreq, res.Gain-outminGain, true);
                            System.Windows.MessageBox.Show("增益调节传入数据完成！");
                        }
                    }
                   
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }
                               

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void doStop_Click(object sender, RoutedEventArgs e)
        {
            Test_DoTest.stopRun = true;
        }
    }
}
