using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing;

namespace RackSys.TestLab.Instrument
{
    internal class ScpiSpectrumAnalyzerFSW : SpectrumAnalyzer
    {
        private string m_stateDirectory = "C:";


        private string m_stateExtension = ".STA";

        private string m_ImageDirectory = "C:";

        private string m_ImageExtension = ".png";

        private string m_saAlignment = string.Empty;

        public override double Attenuation
        {
            get
            {
                return base.QueryNumber("INP:ATT?");
            }
            set
            {
                base.SendNumber("INP:ATT ", (double)((int)Math.Round(value)));
                Thread.Sleep(1000);
                this.WaitOpc();
            }
        }

        public override bool AutoAlignEnabled
        {
            get
            {
                //if (this.Query(":CALibration:AUTO?").ToUpper().StartsWith("ON"))
                //{
                //    return true;
                //}
                return false;
            }
            set
            {
              //  this.Send(string.Concat(":CALibration:AUTO ", (value ? "ON" : "ALERT")));
              //郝佳修改驱动，不适用alert，使用off；
                this.Send(string.Concat(":CALibration:AUTO ", (value ? "ON" : "OFF")));
                this.WaitOpc();
            }
        }




        //public override void ReadTraceXAndYValue(double StartFreq, double StopFreq, out double[] XAxis, out double[] YAxis)
        //{
        //    int SweepNumber = (int)this.QueryNumber("SWE:POIN?");
        //    XAxis = new double[SweepNumber];
        //    YAxis = new double[SweepNumber];
        //    double step = Math.Abs((StartFreq - StopFreq)) / (SweepNumber - 1);
        //    SetMarkerModeByIndex(SpectrumAnalyzer.MarkerModeType.Position, 1);
        //    for (int i = 0; i < SweepNumber; i++)
        //    {
        //        SetMarkerPositionByIndex(StartFreq + i * step, 1);
        //        XAxis[i] = GetMarkerPositionByIndex(1);
        //        YAxis[i] = GetMarkerValueByIndex(1);
        //    }
        //}
        public ScpiSpectrumAnalyzerFSW.SA_Alignment AutoAlignment
        {
            get { return ScpiSpectrumAnalyzerFSW.SA_Alignment.Off; }
            //get
            //{
            //    ScpiSpectrumAnalyzer.SA_Alignment sAAlignment = ScpiSpectrumAnalyzerFSW.SA_Alignment.Alert;
            //    if (!SpectrumAnalyzer.IsESA(this.Model))
            //    {
            //        string str = this.Query(this.GetAutoAlignmentCmd());
            //        if (str.ToUpper() == "ON")
            //        {
            //            sAAlignment = ScpiSpectrumAnalyzerFSW.SA_Alignment.On;
            //        }
            //        else if (str.ToUpper() == "OFF")
            //        {
            //            sAAlignment = ScpiSpectrumAnalyzerFSW.SA_Alignment.Off;
            //        }
            //    }
            //    else
            //    {
            //        string str1 = this.Query(this.GetAutoAlignmentCmd());
            //        if (str1.ToUpper() == "1")
            //        {
            //            sAAlignment = ScpiSpectrumAnalyzerFSW.SA_Alignment.On;
            //        }
            //        else if (str1.ToUpper() == "0")
            //        {
            //            sAAlignment = ScpiSpectrumAnalyzerFSW.SA_Alignment.Off;
            //        }
            //    }
            //    return sAAlignment;
            //}
            //set
            //{
            //    this.Send(this.SetAutoAlignmentCmd(value));
            //}
        }

        public override void SetPowerAvgType()
        {
            this.Send("DET RMS");
            this.WaitOpc();
            
            this.Send("AVER:TYPE POW");//改变平均类型为功率平均，不采用默认的视频平均
            this.WaitOpc();
            
        }

        //待补充
        public override  string CorrectionTable1
        {
            get
            { throw new Exception("罗德频谱仪功能待补充"); }
            set { throw new Exception("罗德频谱仪暂未找到相关功能"); }
        }

        public override double AMP_Offset
        {
            get
            { return this.QueryNumber("DISP:TRAC:Y:RLEV:OFFS?"); }
            set
            { 
                this.Send(string.Concat("DISP:TRAC:Y:RLEV:OFFS ", value));
            this.WaitOpc();
            }


        }
        public override uint Averages
        {
            get
            {
                return (uint)this.QueryNumber(string.Concat("SWE:COUN?"));
            }
            set
            {


                this.Send(string.Concat("SWE:COUN ", (value > 0 ? "ON" : "OFF")));
                this.WaitOpc();
                if (value > 0)
                {
                    base.SendNumber("SWE:COUN ", (double)((float)value));
                    this.WaitOpc();
                    //base.SendSCPI("TRAC:TYPE WRIT");
                }
            }
        }

        /// <summary>
        /// 关闭所有marker显示
        /// </summary>
        public override void AllMarkerOFF()
        {
            this.Send("CALC:MARK:AOFF");
            this.WaitOpc();
        
        }


        /// <summary>
        /// 设置频谱仪的中心频率
        /// </summary>
        public override double CenterFrequency
        {
            get
            {
                return base.QueryNumber("FREQ:CENT?");
            }
            set
            {
                base.SendNumber("FREQ:CENT ", value);
                this.WaitOpc();
            }
        }

        public override bool ContinuousSweepEnabled
        {
            get
            {
                return base.QueryNumber("INIT:CONT?") > 0;
            }
            set
            {
                
                this.Send(string.Concat("INIT:CONT ", (value ? "ON" : "OFF")));
                this.WaitOpc();
            }
        }

        public override double dBperDiv
        {
            get
            {
                return base.QueryNumber("DISP:TRAC:Y?")/10;
            }
            set
            {
                this.Send(string.Concat("DISP:TRAC:Y ", (value*10).ToString(NumberFormatInfo.InvariantInfo), " DB"));
                this.WaitOpc();
            }
        }

        public override void MarkerToCenterFreq(int markerIndex)
        {

            this.Send(string.Concat("CALC:MARK{0}:CENT", markerIndex));
            this.WaitOpc();
        }

        //2014.3.1 苏渊红添加创建文件目录驱动功能
        public override void CreatFile(string Filename)
        {
            base.Send(string.Concat("mmemory:mdirectory '" + Filename + "'"));
            this.WaitOpc();
            //     base.Send(string.Concat("mmemory:mdirectory ", Filename);
        }


        //2014.3.5 苏渊红添加从ENR Meas Table读取值的功能。
        //public override string ReadENRMeasTable
        //{
        //    get
        //    {
        //        base.Send(string.Concat("INST:SEL NFIGURE"));   
        //        base.Send(string.Concat(":NFIGure:CORRection:ENR:TABLe:DATA:DELete"));      //删除已有的Table
        //        base.Send(string.Concat(":NFIG:CORR:ENR:TABL:SNS"));                        //从SENSOR读取默认值至频谱仪

        //        return "";
        //    }
        //    set
        //    {
                

        //    }
            
        //}


        public override bool DisplayEnabled
        {
            get
            {
                return base.QueryNumber("SYST:DISP:UPD?") > 0;
            }
            set
            {
                this.Send(string.Concat("SYST:DISP:UPD  ", (value ? "ON" : "OFF")));
                this.WaitOpc();
            }
        }

        /// <summary>
        /// 变频模式支持
        /// </summary>
        /// <param name="fixLOFreq"></param>
        /// <param name="isHighConverter"></param>
        public override void NoiseFigureMixerModeSetup(double fixLOFreq,bool isHighConverter)
        {
            string tmp;
            if(isHighConverter) tmp="UPConv";
            else tmp = "DOWNconv";
            this.Send(string.Concat("CONF:MODE:DUT ", tmp));
            this.WaitOpc();
            this.Send(string.Concat("CONF:MODE:SYST:LO:FREQ ",fixLOFreq));
            this.WaitOpc();
        
        
        }


        public override double FrequencyMax
        {
            get
            {
                SAMode();
                return base.QueryNumber(":SENSe:FREQuency:CENTer? MAX");
            }
        }

        public override bool InputOverload
        {
            get {return false;}
            //get
            //{
            //    bool flag;
            //    try
            //    {
            //        int num = Convert.ToInt32(base.QueryNumber("STAT:QUES:INT?"));
            //        flag = (num & 16) == 16;
            //    }
            //    catch
            //    {
            //        flag = false;
            //    }
            //    return flag;
            //}
        }

        public override SpectrumAnalyzer.MarkerFunctionType Marker1Function
        {
            get
            {
                int MarkerIndex = 1;

                return GetMakerFunctionByIndex(MarkerIndex);
            }
            set
            {
                int MarkerIndex = 1;
                SetMakerFunctionByIndex(value, MarkerIndex);
            }
        }


        


        public override void SetMakerFunctionByIndex(MarkerFunctionType value, int MarkerIndex)
        {
            string str;
            switch (value)
            {
                case SpectrumAnalyzer.MarkerFunctionType.BandIntervalDensity:   //2014.3.17苏渊红添加
                    {
                        str = "DENSity";
                        this.Send(string.Format("CALC:MARK{0}:FUNC:BPOW:STAT ON", MarkerIndex));
                        this.WaitOpc();
                        this.Send(string.Format("CALC:MARK{0}:FUNC:BPOW:MODE {1}", new object[] { MarkerIndex, str }));
                        this.WaitOpc();
                        break;
                    }

                case SpectrumAnalyzer.MarkerFunctionType.BandIntervalPower:
                    {
                        str = "POWer";
                        this.Send(string.Format("CALC:MARK{0}:FUNC:BPOW:STAT ON", MarkerIndex));
                        this.WaitOpc();
                        this.Send(string.Format("CALC:MARK{0}:FUNC:BPOW:MODE {1}", new object[] { MarkerIndex, str }));
                        this.WaitOpc();
                        break;
                    }


                case SpectrumAnalyzer.MarkerFunctionType.Noise:
                    {

                        this.Send(string.Format("CALC:MARK{0} ON", MarkerIndex));
                        this.WaitOpc();
                        this.Send("CALC:MARK:FUNC:NOIS ON");
                        this.WaitOpc();
                        break;
                    }
                
                default:
                    {
                        goto case SpectrumAnalyzer.MarkerFunctionType.BandIntervalPower;
                    }
            }
            
        }

        public override MarkerFunctionType GetMakerFunctionByIndex(int MarkerIndex)
        {
            string str = this.Query(string.Format("CALC:MARK{0}:FUNC:BPOW:MODE?", MarkerIndex));
            if (str == "POWer")
            {
                return SpectrumAnalyzer.MarkerFunctionType.BandIntervalPower;
            }
            if (str == "DENSity")
            {
                return SpectrumAnalyzer.MarkerFunctionType.BandIntervalDensity;
            }
            return SpectrumAnalyzer.MarkerFunctionType.BandIntervalPower;
        }

        public override SpectrumAnalyzer.MarkerModeType Marker1Mode
        {
            get
            {
                int MarkerIndex = 1;
                return GetMarkerModeByIndex(MarkerIndex);
            }
            set
            {
                int MarkerIndex = 1;
                SetMarkerModeByIndex(value, MarkerIndex);
            }
        }

        /// <summary>
        /// 
        //
        /// </summary>
        /// <param name="value"></param>
        /// <param name="MarkerIndex"></param>
        public override void SetMarkerModeByIndex(MarkerModeType value, int MarkerIndex)
        {
            string str;
            switch (value)
            {
                case SpectrumAnalyzer.MarkerModeType.Position:
                    {
                        str = "POSition";
                        break;
                    }
                //case SpectrumAnalyzer.MarkerModeType.Delta:
                //    {
                //        str = "DELTa";
                //        break;
                //    }
                //case SpectrumAnalyzer.MarkerModeType.Band:
                //    {
                //        str = "BAND";
                //        break;
                //    }
                //case SpectrumAnalyzer.MarkerModeType.Span:
                //    {
                //        str = "SPAN";
                //        break;
                //    }
                case SpectrumAnalyzer.MarkerModeType.Off:
                    {
                        str = "OFF";
                        break;
                    }
                default:
                    {
                        goto case SpectrumAnalyzer.MarkerModeType.Off;
                    }
            }
            this.Send(string.Format("CALC:MARK{0} ON", MarkerIndex));
            this.WaitOpc();
        }

        public override MarkerModeType GetMarkerModeByIndex(int MarkerIndex)
        {
            //string str = this.Query(string.Format(":CALCulate:MARKer{0}:MODE?", MarkerIndex));
            //if (str == "POS")
            //{
            //    return SpectrumAnalyzer.MarkerModeType.Position;
            //}
            //if (str == "DELT")
            //{
            //    return SpectrumAnalyzer.MarkerModeType.Delta;
            //}
            //if (str == "BAND")
            //{
            //    return SpectrumAnalyzer.MarkerModeType.Band;
            //}
            //if (str == "SPAN")
            //{
            //    return SpectrumAnalyzer.MarkerModeType.Span;
            //}
            return SpectrumAnalyzer.MarkerModeType.Position;
        }

        

        public override double Marker1Position
        {
            get
            {
                int MarkerIndex = 1;

                return GetMarkerPositionByIndex(MarkerIndex);
            }
            set
            {
                int MarkerIndex = 1;
                SetMarkerPositionByIndex(value, MarkerIndex);
            }
        }

        public override void SetMarkerPositionByIndex(double value, int MarkerIndex)
        {
            base.SendNumber(string.Format("CALC:MARK{0}:X ", MarkerIndex), value);
            this.WaitOpc();
        }

        public override double GetMarkerFCountByIndex(int MarkerIndex)
        {
            
            base.Send(string.Format("CALC:MARK{0}:COUN ON", MarkerIndex));
            this.WaitOpc();
            return base.QueryNumber(string.Format("CALC:MARK{0}:COUN:FREQ?", MarkerIndex));
        }

        public override double GetMarkerPositionByIndex(int MarkerIndex)
        {
            return base.QueryNumber(string.Format("CALC:MARK{0}:X?", MarkerIndex));
        }

        public override double MarkerPositionPoints
        {
            get
            {

                return 0;
                //return base.QueryNumber(":CALC:MARK:X:POS:CENT?");
            }
            set
            {
                //base.SendNumber(":CALC:MARK:X:POS:CENT ", value);
            }
        }

        public override double MarkerSpanPoints
        {
            get { return 0; }
            //{
            //    return base.QueryNumber(":CALC:MARK:X:POS:SPAN?");
            //}
            set
            {
                //base.SendNumber(":CALC:MARK:X:POS:SPAN ", value);
            }
        }

        public override double Marker1Value
        {
            get
            {
                int MarkerIndex = 1;
                return GetMarkerValueByIndex(MarkerIndex);
            }
        }

        public override double GetMarkerValueByIndex(int MarkerIndex)
        {

            return base.QueryNumber(string.Format("CALC:MARK{0}:Y?", MarkerIndex));
        }

        public override double GetMarkerFuntionBPOwerValueByIndex(int MarkerIndex)
        {

            return base.QueryNumber(string.Format("CALC:MARK{0}:FUNC:BPOW:RES?", MarkerIndex));
        }


        public override double GetMarkerFuntionNoiseValueByIndex(int MarkerIndex)
        {

            return base.QueryNumber(string.Format("CALC:MARK{0}:FUNC:NOIS:RES?", MarkerIndex));
        }


        public override bool NeedsAlignment
        {
            get { return false; }
            //{
            //    bool flag;
            //    try
            //    {
            //        int num = Convert.ToInt32(base.QueryNumber("STAT:QUES:CAL:COND?"));
            //        flag = (num & 16384) == 16384;
            //    }
            //    catch
            //    {
            //        flag = false;
            //    }
            //    return flag;
            //}
        }

        public override double RBW
        {
            get
            {
                return base.QueryNumber("BAND?");
            }
            set
            {
                base.SendNumber("BAND ", value);
                this.WaitOpc();
            }
        }

        /// <summary>
        /// 参考电平
        /// </summary>
        public override double ReferenceLevel
        {
            get
            {
                return base.QueryNumber("DISP:TRAC:Y:RLEV?");
            }
            set
            {
                base.SendNumber("DISP:TRAC:Y:RLEV ", value);
                this.WaitOpc();
            }
        }

        public override bool RFExternalReference
        {
            get { return false; }
            //get
            //{
            //    string str;
            //    str = (!SpectrumAnalyzer.IsESA(this.Model) ? this.Query(":ROSCillator:SOURCe?") : this.Query(":CAL:FREQ:REF?"));
            //    return str == "EXT";
            //}
        }


        



        public override double Span
        {
            get
            {
                return base.QueryNumber("FREQuency:SPAN?");
            }
            set
            {
                base.SendNumber("FREQuency:SPAN ", value);
                this.WaitOpc();
            }
        }

        public override int SweepPoints
        {
            get
            {
                return (int)base.QueryNumber("SWEep:POINts?");
            }
            set
            {
                base.SendNumber("SWEep:POINts ", (double)value);
                this.WaitOpc();
            }
        }

        public override double SweepTime
        {
            get
            {
                return base.QueryNumber("SWE:TIME?");
            }
            set
            {
                base.SendNumber("SWE:TIME ", value);
                this.WaitOpc();
            }
        }
        public override double[] TraceXaxis
        {
            get 
            
            { 
                double start=StartFrequency;
                double step=Math.Abs(StopFrequency-StartFrequency)/(SweepPoints-1);
                double [] xValue=new double [SweepPoints];
                for(int i=0;i<xValue.Length;i++)
                {
                    xValue[i] = start + step * i;
                
                }
                return xValue;
            
            
            }
        
        }
        public override double[] TraceDataByIndex(int TraceIndex)
        {
                byte[] numArray;
                this.Send(string.Concat(@"MMEM:STOR1:TRAC {0},'C:\TESTResult11.ASC'", TraceIndex));
                this.WaitOpc();
                numArray=base.ReadBlock(@"MMEMory:DATA? 'C:\TESTResult11.ASC'");
           
                //byte[] tempByte = System.IO.File.ReadAllBytes("E:\\TESTResult.ASC");
                // char[] chars = new char[tempByte.Length / sizeof(char)];
                //System.Buffer.BlockCopy(tempByte, 0, chars, 0, tempByte.Length);
                var decoded = System.Text.Encoding.UTF8.GetString(numArray);
                string[] tempLines = System.Text.RegularExpressions.Regex.Split(decoded, "\r\n");
                string[] tempStr = new string[tempLines.Length - 30];
                Array.Copy(tempLines, 30, tempStr, 0, tempStr.Length);
                List<double> freqList = new List<double>();
                List<double> ValueList = new List<double>();
                foreach (string str in tempStr)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        string[] tempstr = str.Split(';');
                        freqList.Add(Convert.ToDouble(tempstr[0]));
                        ValueList.Add(Convert.ToDouble(tempstr[1]));
                    }

                }
                return ValueList.ToArray();






                ////byte[] numArray;
                //double[] numArray1 = null;
                //this.Send("FORMat:TRACE:DATA REAL,32");
                //this.Send("FORMat:BORDer SWAPped");
                ////try
                ////{
                ////    this.IO.ReadIEEEBlock("MMEM:STOR1:TRAC 1", out numArray);
                ////    float[] singleArray = new float[(int)numArray.Length / Marshal.SizeOf(typeof(float))];
                ////    Buffer.BlockCopy(numArray, 0, singleArray, 0, (int)numArray.Length);
                ////    numArray1 = new double[(int)singleArray.Length];
                ////    for (int i = 0; i < (int)singleArray.Length; i++)
                ////    {
                ////        numArray1[i] = (double)singleArray[i];
                //    //}
                //}
                //catch (Exception exception)
                //{
                //    throw new ApplicationException("Could not read trace data", exception);
                //}
                //return numArray1;
            
        }

        /// <summary>
        /// 根据不同的设备，选择不同的文件名和后缀，并拷贝屏幕
        /// </summary>
        private string SaveScreeImageToFile()
        {
            string filename = "";
            if (SpectrumAnalyzer.IsPSA(this.Model) || SpectrumAnalyzer.IsESA(this.Model))
            {
                this.m_ImageDirectory = "C:";
                this.m_ImageExtension = ".bmp";
                filename = "SSA";
            }
            else
            {
                this.m_ImageDirectory = "c:\\User_RackSys\\ScreenImage";
                filename = "SSA";
                this.m_ImageExtension = ".jpg";
                //filename = "1";
            }

            
            string FullFileName = string.Concat(this.m_ImageDirectory, "\\", filename.ToUpper(), this.m_ImageExtension);

            this.Send("HCOP:DEST 'MMEM'");
            this.WaitOpc();
            this.Send("HCOP:DEV:LANG JPG");
            this.WaitOpc();
            this.Send(string.Concat("MMEM:NAME \'", FullFileName, "\'"));

            //this.Send(string.Concat("MMEM:NAME '",FullFileName,"'"));
            this.WaitOpc();
            //this.Send(string.Concat("HCOP:ITEM:ALL"));
            //this.Query("*OPC?");
            this.Send(string.Concat("HCOP"));
            this.WaitOpc();



            return FullFileName;
        }

        /// <summary>
        /// 采集图片数据并返回对应的图片对象
        /// </summary>
        /// <returns></returns>
        public override Image CaptureScreenImage()
        {
            string ImageFileName = this.SaveScreeImageToFile();
            byte[] ImageDataInBytes = this.TransferFileToPC(ImageFileName);
            return ImageBytesConvertor.ConvertByteToImg(ImageDataInBytes);
        }

        private byte[] TransferFileToPC(string inImageFileName)
        {
            return base.ReadBlock(string.Concat("MMEMory:DATA? \'", inImageFileName,"\'"));
        }


        public override double VBW
        {
            get
            {
                return base.QueryNumber("BAND:VID?");
            }
            set
            {
                base.SendNumber("BAND:VID ", value);
                this.WaitOpc();
            }
        }

        public ScpiSpectrumAnalyzerFSW(string address)
            : base(address)
        {
        }

        public override void DeleteState(string filename)
        {
            //if (SpectrumAnalyzer.IsPSA(this.Model) || SpectrumAnalyzer.IsESA(this.Model))
            //{
            //    this.m_stateDirectory = "C:";
            //    this.m_stateExtension = ".STA";
            //}
            //else
            //{
            //    this.m_stateDirectory = "D:\\User_My_Documents\\Instrument\\My Documents\\SA\\state";
            //    this.m_stateExtension = ".State";
            //}
            //string str = string.Concat(this.m_stateDirectory, "\\", filename.ToUpper(), this.m_stateExtension);
            //this.Send(string.Concat(":MMEMory:DELete \"", str, "\""));
            //this.m_saAlignment = string.Empty;
        }

        protected virtual string GetAutoAlignmentCmd()
        {
            return string.Empty;
            //return ":CAL:AUTO?";
        }

        public override void LoadState(string filename)
        {
            //if (SpectrumAnalyzer.IsPSA(this.Model) || SpectrumAnalyzer.IsESA(this.Model))
            //{
            //    this.m_stateDirectory = "C:";
            //    this.m_stateExtension = ".STA";
            //}
            //else
            //{
            //    this.m_stateDirectory = "D:\\User_My_Documents\\Instrument\\My Documents\\SA\\state";
            //    this.m_stateExtension = ".State";
            //}
            //string str = string.Concat(this.m_stateDirectory, "\\", filename.ToUpper(), this.m_stateExtension);
            //this.Send(string.Concat(":MMEMory:LOAD:STATe 1,\"", str, "\""));
            //if (this.m_saAlignment.Length > 0)
            //{
            //    this.Send(string.Concat(":CALibration:AUTO ", this.m_saAlignment));
            //    this.m_saAlignment = string.Empty;
            //}
        }

        public override void TOIMode()
        {
            //this.Query("INITiate:TOI;*OPC?");

        }
        public override double TOI_ReferenceLevel
        {
            get { return 0; }
            set {}
        //    {
        //        if (this.IsXA())
        //        {
        //            return this.QueryNumber("DISPlay:TOI:VIEW:WINDow:TRACe:Y:SCALe:RLEVel? ");
        //        }
        //        else
        //        {
        //            return this.dBperDiv;
        //        }
        //    }
            //set
            //{
        //        if (this.IsXA())
        //        {
        //            this.SendNumber("DISPlay:TOI:VIEW:WINDow:TRACe:Y:SCALe:RLEVel " , value);
        //        }
        //        else
        //        {
        //            this.ReferenceLevel = value;
        //        }
            //}
        }
        public override double TOI_RBW
        {
            get {return 0;}
            set { }
        //    {
        //        if (this.IsXA())
        //        {
        //            return this.QueryNumber(":TOI:BWID? ");
        //        }
        //        else
        //        {
        //            return this.RBW;
        //        }
        //    }

        //    {
        //        if (this.IsXA())
        //        {
        //            this.SendNumber(":TOI:BWID " , value);
        //        }
        //        else
        //        {
        //            this.RBW = value;
        //        }

            //}
        }
        public override double TOI_Span
        {
            get
            {return 0;}
            set {}
        //        if (this.IsXA())
        //        {
        //            return this.QueryNumber("SENSe:TOI:FREQuency:SPAN?");
        //        }
        //        else
        //        {
        //            return this.Span;
        //        }
            //}
            //set
            //{
        //        if (this.IsXA())
        //        {
        //            this.SendNumber("SENSe:TOI:FREQuency:SPAN ", value);
        //        }
        //        else
        //        {
        //            this.Span = value;
        //        }
            //}
        }
        public override double TOI_VBW
        {
            get { return 0; }
            set {}
        //    get
        //    {
        //        if (this.IsXA())
        //        {
        //            return this.QueryNumber("SENSe:TOI:BANDwidth:VIDeo? ");
        //        }
        //        else
        //        {
        //            return this.VBW;
        //        }
        //    }
        //    set
        //    {
        //        if (this.IsXA())
        //        {
        //            this.SendNumber("SENSe:TOI:BANDwidth:VIDeo ", value);
        //        }
        //        else
        //        {
        //            this.VBW = value;
        //        }
        //    }
        }

        public override double TOI_dBperDiv
        {
            get { return 0; }
            set { }
        //    get
        //    {
        //        if (this.IsXA())
        //        {
        //            return this.QueryNumber("DISP:TOI:VIEW:WIND:TRAC:Y:PDIV? ");
        //        }
        //        else
        //        {
        //            return this.dBperDiv;
        //        }
        //    }
        //    set
        //    {
        //        if (this.IsXA())
        //        {
        //            this.SendNumber("DISP:TOI:VIEW:WIND:TRAC:Y:PDIV ", value);
        //        }
        //        else
        //        {
        //            this.dBperDiv = value;
        //        }

        //    }
        }

        
        public override double StartFrequency
        {
            get
            {
                return base.QueryNumber("FREQ:STAR?");
            }
            set
            {
                base.SendNumber("FREQ:STAR ", value);
                this.WaitOpc();
            }
        }

        public override double StopFrequency
        {
            get
            {
                return base.QueryNumber("FREQ:STOP?");
            }
            set
            {
                base.SendNumber("FREQ:STOP ", value);
                this.WaitOpc();
            }
        }

        public override void MarkerPeakSearch(int MarkerIndex)
        {
            this.Send (string.Format("CALCulate:MARKer{0}:MAXimum", MarkerIndex));
            this.WaitOpc();
        }

        /// <summary>
        /// 找到下一个高点
        /// </summary>
        /// <param name="MarkerIndex"></param>
        public override void MarkerNextPeakSearch(int MarkerIndex)
        {
            if (MarkerIndex > 4)
            {
                MarkerIndex = 4;
            }
            this.Send(string.Format("CALCulate:MARKer{0}:MAXimum:NEXT",MarkerIndex));
            this.WaitOpc();
        }

        /// <summary>
        /// 找到第n个高点
        /// </summary>
        /// <param name="MarkerIndex"></param>
        /// <param name="PeakOrder"></param>
        public override double PeakSearchByOrder(int MarkerIndex, int PeakOrder)
        {
            if (MarkerIndex > 4)
            {
                MarkerIndex = 4;
            }
            this.MarkerPeakSearch(MarkerIndex);
            while (true)
            {
                if (PeakOrder == 0)
                {
                    break ;
                }
                PeakOrder--;
                this.MarkerNextPeakSearch(MarkerIndex);
            }

            return this.GetMarkerValueByIndex(MarkerIndex);
        }

        public override bool MarkerTableState
        {
            get
            {
                if (this.IsXA())
                {
                    return this.QueryNumber("CALC:MARK:TABL?") > 0;
                }
                else
                {
                    return this.QueryNumber(":CALCulate:MARKer:TABLe:STATe?") > 0;
                }
            }
            set
            {
                string OnOffStr = value  ? "ON" : "OFF";
                if (this.IsXA())
                {
                    this.Query(string.Format("CALC:MARK:TABL {0};*OPC?", OnOffStr));
                }
                else
                {
                    this.Query(string.Format(":CALCulate:MARKer:TABLe:STATe {0};*OPC?", OnOffStr));
                }
            }
        }

        public override void Preset()
        {
            int timeout = this.Timeout;
            this.Send("*RST;*WAI");
            this.Query("*IDN?", 30000);
            if (SpectrumAnalyzer.IsMXA(this.Model) || SpectrumAnalyzer.IsEXA(this.Model))
            {
                this.Send("INIT:CONT ON");
                this.WaitOpc();
            }
            this.Timeout = timeout;
        }



        public override void ModePreset()   //郝佳添加代码 2013.11.25
        {
            //int timeout = this.Timeout;
            this.Send("INIT:CONT ON");
            this.WaitOpc();
            //this.Timeout = timeout;
        }

        public override void MarkerToRefLev(int MarkerNum)
        {
            int timeout = this.Timeout;
            this.ReferenceLevel = this.GetMarkerValueByIndex(MarkerNum);           
            this.Timeout = timeout;
        }



        
        
        public override void SAMode()
        {
            if (this.HasOption("266") && !this.Query(":SYST:LANG?").Equals("SCPI"))
            {
                this.Send(":SYST:LANG SCPI");
                this.WaitOpc();
            }
            if (SpectrumAnalyzer.IsPSA(this.Model))
            {
                if (!this.Query(":INST?").Equals("SA"))
                {
                    this.Send(":INST SA");
                    this.WaitOpc();
                    return;
                }
            }
            else if (SpectrumAnalyzer.IsESA(this.Model))
            {
                if (!this.Query(":INST?").Equals("\"SA\""))
                {
                    this.Send(":INST \"SA\"");
                    this.WaitOpc();
                    return;
                }
            }
            else if (SpectrumAnalyzer.IsMXA(this.Model) || SpectrumAnalyzer.IsEXA(this.Model))
            {
                if (!this.Query(":INST?").Equals("SA"))
                {
                    this.Send(":INST SA");
                    this.WaitOpc();
                }
                this.Send("POW:ATT:STEP 2");
                this.WaitOpc();
            }
            else if(SpectrumAnalyzer.IsPXA(this.Model)) //2014.3.3苏渊红添加代码
            {
                this.Send("INST:SEL SA");
                this.WaitOpc();
            }
            else if (SpectrumAnalyzer.IsFSW(this.Model))
            {
                this.Send("INST:SEL SAN");
                this.WaitOpc();
            }
        }

        ///罗德频谱仪不采用这种方式读数
        public override double GetMarkerNFValueByIndex(int MarkerIndex)
        {
            //this.Send(":CALC:NFIG:MARK1:TRAC TRAC1");
            //this.WaitOpc();
            //return this.GetMarkerValueByIndex(MarkerIndex);
            return 0;
        }

        //罗德频谱仪不采用这种方式读数
        public override double GetMarkerGainValueByIndex(int MarkerIndex)
        {
            //this.Send(":CALC:NFIG:MARK1:TRAC TRAC2");
            //this.WaitOpc();
            //return this.GetMarkerValueByIndex(MarkerIndex);
            return 0;
        }

        public override void NoiseFigureMode()
        {
            this.Send("INST:SEL NOISE");
            this.WaitOpc();
        }


        private string ReadENRTable()
        {
            string tmp;
            tmp=this.Query("CORR:ENR:TABL:LIST");
            string [] array = tmp.Split(',');
            for (int i = 0; i < array.Length; i++)
            { 
                if (array[i].Contains("NC346"))
                return array[i];
            
            }
            return string.Empty;
        
        }


        /// <summary>
        /// FSW测噪声系数是，需要用户输入ENRTable；
        /// </summary>
        //public static string ENRTable;


        public override void NoiseFigureCalNow(string ENRTable)
        {
            if (!string.IsNullOrEmpty(ENRTable))
            {
                this.Send("CORR:ENR:MEAS:TABL:DEL 'ENRTable'");
                this.WaitOpc();
                this.Send("CORR:ENR:MEAS:TABL:SEL 'ENRTable'");
                this.WaitOpc();
                this.Send(string.Concat("CORR:ENR:MEAS:TABL:DATA ", ENRTable));
                this.WaitOpc();
                this.Send("CORR:ENR:MODE TABL");
                this.WaitOpc();
            }
            this.Send("conf:corr");
            this.WaitOpc();
            this.Send("init");
            this.WaitOpc();

        }

        public override void NFCALStatON()
        { 
        
        
        }
        

        public override void BandIntervalPowerMarkerSpan(uint MarkerNumber,double Span)// 郝佳添加驱动,2013.12.3
        {
            this.Send(string.Concat("CALC:MARK", MarkerNumber, ":FUNC:BPOW:SPAN ", Span));
            this.WaitOpc();
        }
        

        public override void NoiseFigureLossCompModeBeforeDUT(SpectrumAnalyzer.NoiseFigureLossCompMode Mode)// 郝佳添加驱动,2013.12.4
        {
            
        }
        

        public override void ClearLossCompTableBeforeDUT()// 郝佳添加驱动,2013.12.4
        {

              
        
        }
       

        public override void SaveLossCompTableBeforeDUT(string FileAddressAndName)// 郝佳添加驱动,2013.12.4
        {
            //this.Send(":MMEM: LOAD:LOSS BEF,FileAddressAndName");
        }




        //输入路径补偿数据表格BeforeDUT
        public override void SetLossCompTableBeforeDUT(string FreqAndLossList)// 郝佳添加驱动,2013.12.4
        {
            //删除table
            this.Send("CORR:LOSS:INP:TABL:DEL 'InputLoss'");
            this.WaitOpc();
            //重新建立table
            this.Send("CORR:LOSS:INP:TABL:SEL 'InputLoss'");
            this.WaitOpc();
            //Table赋值
            this.Send("CORR:LOSS:INP:TABL " + FreqAndLossList);
            this.WaitOpc();
            //table生效
            this.Send("CORR:LOSS:INP:MODE TABL");
        }


        //设置噪声系数测试的RBW
        public override double NF_RBW
        {
            get
            {

                return this.QueryNumber("BAND? ");
               
            }
            set
            {
                this.Send(string.Concat("BAND ", value));
                
            }
        }







//        .csv Format
//The .csv format contains the following data:

//13. File Type

//14. Application Name:Measurement Name

//15. Version and Model Number

//16. Loss Comp Data

//Below is an example of a valid .csv Loss Compensation file:

//[Filetype LossCompensation]

//[NF:NFIG]

//Ver. ***, Model ***

//10, 1.0000

//20, 2.0000

//30, 3.0000

//40, 4.0000

//50, 5.0000

//60, 6.0000




        public override void NoiseFigureLossCompModeAfterDUT(SpectrumAnalyzer.NoiseFigureLossCompMode Mode)// 郝佳添加驱动,2013.12.4
        {
            
        }
        public override void ClearLossCompTableAfterDUT()
        {
            
        }

        //输入路径补偿数据表格AfterDUT
        public override void SetLossCompTableAfterDUT(string FreqAndLossList)// 郝佳添加驱动,2013.12.4
        {
            //删除table
            this.Send("CORR:LOSS:OUTP:TABL:DEL 'OutputLoss'");
            this.WaitOpc();
            //重新建立table
            this.Send("CORR:LOSS:OUTP:TABL:SEL 'OutputLoss'");
            this.WaitOpc();
            //Table赋值
            this.Send("CORR:LOSS:OUTP:TABL " + FreqAndLossList);
            this.WaitOpc();
            //table生效
            this.Send("CORR:LOSS:OUTP:MODE TABL");
        }






        public override TOITestResult TOI_GetTestResult()
        {
        //    string RetResult = this.Query(":READ:TOI2?");
            
        //    string[] TestResultStrs = RetResult.Split(new char[]{','});
            TOITestResult tmpTOITestResult = new TOITestResult();
        //    tmpTOITestResult.LowerBaseFreq = double.Parse(TestResultStrs[4]);
        //    tmpTOITestResult.LowerBasePower = double.Parse(TestResultStrs[5]);
        //    tmpTOITestResult.UpperBaseFreq = double.Parse(TestResultStrs[6]);
        //    tmpTOITestResult.UpperBasePower = double.Parse(TestResultStrs[7]);

        //    tmpTOITestResult.Lower_InterModulate_Freq = double.Parse(TestResultStrs[8]);
        //    tmpTOITestResult.Lower_InterModulate_Power = double.Parse(TestResultStrs[9]);

        //    tmpTOITestResult.Upper_InterModulate_Freq = double.Parse(TestResultStrs[11]);
        //    tmpTOITestResult.Upper_IntreModulate_Power = double.Parse(TestResultStrs[12]);
            return tmpTOITestResult;
        }

        public override void SaveState(string filename)
        {
        //    if (SpectrumAnalyzer.IsPSA(this.Model) || SpectrumAnalyzer.IsESA(this.Model))
        //    {
        //        this.m_stateDirectory = "C:";
        //        this.m_stateExtension = ".STA";
        //    }
        //    else
        //    {
        //        this.m_stateDirectory = "D:\\User_My_Documents\\Instrument\\My Documents\\SA\\state";
        //        this.m_stateExtension = ".State";
        //    }
        //    string str = string.Concat(this.m_stateDirectory, "\\", filename.ToUpper(), this.m_stateExtension);
        //    string str1 = this.Query(string.Concat(":MMEMory:CATalog? \"", this.m_stateDirectory, "\""));
        //    char[] charArray = ",".ToCharArray();
        //    if (Convert.ToDouble(str1.Split(charArray, 3)[1]) < 30000)
        //    {
        //        throw new ApplicationException("Not enough memory to store spectrum analyzer state.  Make more room so that application can store current state.");
        //    }
        //    if (str1.IndexOf(filename.ToUpper()) != -1)
        //    {
        //        this.Send(string.Concat(":MMEMory:DELete \"", str, "\""));
        //    }
        //    this.Send(string.Concat(":MMEMory:STORe:STATe 1,\"", str, "\""));
        //    this.m_saAlignment = this.Query(":CALibration:AUTO?");
        }

        
        public override string SaveStateToPath(string filename)
        {
            filename = filename + ".dfl";
            this.Send(string.Concat("MMEM:STOR:STAT 1, \'", filename, "\'"));
            return filename;
            
        }
        public override void LoadStateFromPath(string filename)
        {
            this.Send(string.Concat("MMEM:LOAD:STAT 1, \'", filename, "\'"));
        }


        protected virtual string SetAutoAlignmentCmd(ScpiSpectrumAnalyzer.SA_Alignment val)
        {
            return string.Empty;
            //if (SpectrumAnalyzer.IsESA(this.Model))
            //{
            //    string str = "OFF";
            //    if (val == ScpiSpectrumAnalyzer.SA_Alignment.On)
            //    {
            //        str = "ON";
            //    }
            //    return string.Concat(":CAL:AUTO ", str);
            //}
            //string str1 = "ALERT";
            //if (val == ScpiSpectrumAnalyzer.SA_Alignment.On)
            //{
            //    str1 = "ON";
            //}
            //else if (val == ScpiSpectrumAnalyzer.SA_Alignment.Off)
            //{
            //    str1 = "OFF";
            //}
            //return string.Concat(":CAL:AUTO ", str1);
        }

        /// <summary>
        /// 触发单次扫描
        /// </summary>
        public override void Sweep(int timeMS=5000)
        {
            this.Send("INIT");
            this.WaitOpc(timeMS);
        }

        public enum SA_Alignment
        {
            Off,
            On,
            Alert
        }


        public override void SelectDisplayWindow(uint WindowNumber)
        {
            //this.Send(":DISP:WIND " + WindowNumber);
        }
        public override void SetNFWindowAutoScale(uint WindowNumber, bool value)
        {
            //this.Send(":DISP:NFIG:VIEW:WIND" + WindowNumber + ":TRAC:Y:COUP " + (value ? "ON" : "OFF"));
        }

        public override void SetWindowZoom(bool value)
        {
            //this.Send(":DISPlay:WINDow:FORMat:" + (value ? "ZOOM" : "TILE"));
        }

        public override void SetDisPlayFormat(DisPlayModeType value)
        {
            //this.Send(":DISPlay:NFIGure:FORMat " + value);
        }
        
        
        public override double Inter_Attenuation
        {
            get
            {

                return this.QueryNumber("INP:ATT? ");

            }
            set
            {
                this.Send(string.Concat("INP:ATT ", value));
                this.WaitOpc();
            }
        }


        public override void ReadNFTraceData(out double[] noisefigure)
        {
            string [] nf;

            nf = this.Query("TRAC? TRACE1,NOIS").Split(',');
            noisefigure = new double[nf.Length];
            for (int i = 0; i < nf.Length; i++)
            {
                noisefigure[i] = Math.Round(Convert.ToDouble(nf[i]),2);
            
            }
        
        
        }

        public override void ReadGainTraceData(out double[] gain)
        {
            string[] temgain;

            temgain = this.Query("TRAC? TRACE1,GAIN").Split(',');
            gain = new double[temgain.Length];
            for (int i = 0; i < temgain.Length; i++)
            {
                gain[i] = Math.Round(Convert.ToDouble(temgain[i]), 2);

            }


        }

        
        /// <summary>
        /// 根据界面设置的各个参数，对频谱仪进行基本配置
        /// </summary>
        /// <param name="SASetting"></param>
        /// <returns></returns>
        public override bool SpectrumAnalyzerConfigure(SpecTrumAnalyzerSetting SASetting)
        {
            #region 扫频模式设置
            if (!SASetting.NFStateEnable)
            {
                this.SAMode();
                this.WaitOpc();
                this.StartFrequency = SASetting.FreqStart;
                this.WaitOpc();
                this.StopFrequency = SASetting.FreqStop;
                this.WaitOpc();
                this.RBW = SASetting.RBW;
                this.WaitOpc();
                this.Inter_Attenuation = SASetting.InterAttenuation;
                this.WaitOpc();
                this.ReferenceLevel = SASetting.ReferenceLevel;
                this.WaitOpc();
                this.Averages = SASetting.AverageNumber;
                this.WaitOpc();
                this.AMP_Offset = SASetting.Offset;
                this.WaitOpc();
                if (SASetting.Yscale > 0)
                { this.dBperDiv = SASetting.Yscale; this.WaitOpc(); }
                if (SASetting.SingleSweep)
                { this.ContinuousSweepEnabled = false; this.WaitOpc(); this.Sweep(); this.WaitOpc(); }
                else { this.ContinuousSweepEnabled = true; }




                return true;

            }
            #endregion


            #region NF模式设置
            else
            {
                this.NoiseFigureMode();
                this.WaitOpc();
                if (!(SASetting.NFStateFilePathAndName == string.Empty))
                { this.LoadStateFromPath(SASetting.NFStateFilePathAndName); this.WaitOpc(); }
                this.StartFrequency = SASetting.FreqStart;
                this.WaitOpc();
                this.StopFrequency = SASetting.FreqStop;
                this.WaitOpc();
                this.RBW = SASetting.RBW;
                this.WaitOpc();
                this.SweepPoints = (int)SASetting.SweepPoints;
                this.WaitOpc();
                this.Averages = SASetting.AverageNumber;
                this.WaitOpc();
                if (SASetting.SingleSweep)
                { this.ContinuousSweepEnabled = false; this.WaitOpc(); }

                return true;
            }
            #endregion


        }


        /// <summary>
        /// 频谱仪设置读取
        /// </summary>
        /// <param name="isNFMode"></param>标示是否NF模式，true对应NF，false对应扫频模式
        /// <param name="SASettingResult"></param>
        /// <returns></returns>
        public override bool SpectrumAnalyzerConfigurationRead(bool isNFMode, SpecTrumAnalyzerSetting SASettingResult)
        {
            //SASettingResult = new SpecTrumAnalyzerSetting();
            #region 扫频模式设置读取
            if (!isNFMode)
            {
                SASettingResult.NFStateEnable = false;
                SASettingResult.FreqStart = this.StartFrequency;
                SASettingResult.FreqStop = this.StopFrequency;
                SASettingResult.RBW = this.RBW;
                SASettingResult.AverageNumber = this.Averages;
                SASettingResult.InterAttenuation = this.Inter_Attenuation;
                SASettingResult.ReferenceLevel = this.ReferenceLevel;
                SASettingResult.Yscale = this.dBperDiv;
                SASettingResult.SingleSweep = !this.ContinuousSweepEnabled;
                SASettingResult.Offset = this.AMP_Offset;
                return true;

            }
            #endregion



            #region NF模式设置读取
            else
            {
                SASettingResult.NFStateEnable = true;
                SASettingResult.FreqStart = this.StartFrequency;
                SASettingResult.FreqStop = this.StopFrequency;
                SASettingResult.RBW = this.RBW;
                SASettingResult.AverageNumber = this.Averages;
                SASettingResult.SweepPoints = (uint)this.SweepPoints;

                //当前NF状态保存并返回保存的名称
                SASettingResult.NFStateFilePathAndName = "temp.dfl";
                this.SaveStateToPath(SASettingResult.NFStateFilePathAndName);
                SASettingResult.SingleSweep = this.ContinuousSweepEnabled;

                return true;

            }
            #endregion




        }


        /// <summary>
        /// 使用频谱仪当前的状态文件
        /// </summary>
        /// <returns></returns>
        public override bool SpectrumAnalyzerUseCurrentState(bool isNFMode, out SpecTrumAnalyzerSetting SASetting)
        {
            SASetting = new SpecTrumAnalyzerSetting();
            SASetting.NFStateFilePathAndName = @"D:\User_My_Documents\UserTemp.state";
            this.SaveState(SASetting.NFStateFilePathAndName);
            SpectrumAnalyzerConfigurationRead(isNFMode,  SASetting);
            




            return true;
        }


        /// <summary>
        /// 使用频谱仪指定的状态文件
        /// </summary>
        /// <returns></returns>
        public override bool SpectrumAnalyzerUseSelectedState(bool isNFMode,string stateFilepPathAndName,SpecTrumAnalyzerSetting SASetting)
        {
            this.LoadStateFromPath(stateFilepPathAndName);
            SASetting.NFStateFilePathAndName  = stateFilepPathAndName;
            SpectrumAnalyzerConfigurationRead(isNFMode,  SASetting);
            return true;
        }


        /// <summary>
        /// 根据输入的路径，返回状态文件列表
        /// </summary>
        /// <param name="DefaultDir"></param>
        /// <param name="calsetResult"></param>
        public override void GetAllsCalsetNames(string DefaultDir, out List<string> calsetResult)
        {
            string s = this.Query(":MMEMory:CATalog? \"" + DefaultDir + '\"');
            s = s.Remove(0, 1);
            s = s.Remove(s.Length - 1, 1);
            string[] calsets = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            calsetResult = new List<string>();
            for (int i = 0; i < calsets.Length; i++)
            {
                if (!((calsets[i].Contains("dfl") || calsets[i].Contains("state"))))
                    continue;

                calsetResult.Add(calsets[i]);//.Remove(0, 1)

            }




        }


        // 杨飞添加驱动 2016.01.07  
        #region  频谱仪触发模式

        /// <summary>
        /// 设置频谱仪触发模式
        /// </summary>
        /// <param name="value">状态模式</param>
        private void SetTriggerMode(TriggerMode value)
        {
            string str;
            switch (value)
            {
                case SpectrumAnalyzer.TriggerMode.Immediate:
                    {
                        str = "IMMediate";
                        break;
                    }
                case SpectrumAnalyzer.TriggerMode.Extern:
                    {
                        str = "EXTern";
                        break;
                    }
                case SpectrumAnalyzer.TriggerMode.IFPower:
                    {
                        str = "IFPower";
                        break;
                    }
                case SpectrumAnalyzer.TriggerMode.RFPower:
                    {
                        str = "RFPower";
                        break;
                    }
                case SpectrumAnalyzer.TriggerMode.Video:
                    {
                        str = "VIDeo";
                        break;
                    }
                case SpectrumAnalyzer.TriggerMode.BBPower:
                    {
                        str = "BBPower";
                        break;
                    }
                case SpectrumAnalyzer.TriggerMode.Psen:
                    {
                        str = "PSEN";
                        break;
                    }
                default:
                    {
                        goto case SpectrumAnalyzer.TriggerMode.Immediate;
                    }
            }
            this.Send(string.Format(":TRIG:SOUR {0}", new object[] { str }));
            this.WaitOpc();
        }
        /// <summary>
        /// 获取频谱仪触发模式
        /// </summary>
        private TriggerMode GetTriggerMode()
        {
                string str = this.Query(string.Format(":TRIGger:SOURce? "));
                if (str == "IMMediate")
                {
                    return SpectrumAnalyzer.TriggerMode.Immediate;
                }
                if (str == "EXTern")
                {
                    return SpectrumAnalyzer.TriggerMode.Extern;
                }
                if (str == "IFPower")
                {
                    return SpectrumAnalyzer.TriggerMode.IFPower;
                }
                if (str == "RFPower")
                {
                    return SpectrumAnalyzer.TriggerMode.RFPower;
                }
                if (str == "VIDeo")
                {
                    return SpectrumAnalyzer.TriggerMode.Video;
                }
                if (str == "BBPower")
                {
                    return SpectrumAnalyzer.TriggerMode.BBPower;
                }
                if (str == "PSEN")
                {
                    return SpectrumAnalyzer.TriggerMode.Psen;
                }
                return SpectrumAnalyzer.TriggerMode.Immediate;
           
        }

        public override TriggerMode TriggerModeState
        {
            get
            {
                return this.GetTriggerMode();
            }
            set
            {
                this.SetTriggerMode(value);
                this.WaitOpc();
            }
        }

        public override int GetTraceNumberByIndex(int MarkerIndex)
        {
            int TraceNumber;
            TraceNumber = (int)base.QueryNumber(string.Format(":CALCulate:MARKer{0}:TRACe?", MarkerIndex));
            return TraceNumber;
        }//只有设置代码，没找到原出处，暂定

        public override void SetTraceStateByIndex(TraceState inState, int TraceIndex)
        {
            string str;
            switch (inState)
            {
                case SpectrumAnalyzer.TraceState.Maxhold:
                    {
                        str = "MAXHold";
                        break;
                    }
                case SpectrumAnalyzer.TraceState.Minhold:
                    {
                        str = "MINHold";
                        break;
                    }
                case SpectrumAnalyzer.TraceState.Average:
                    {
                        str = "AVERage";
                        break;
                    }
                case SpectrumAnalyzer.TraceState.Write:
                    {
                        str = "WRITe";
                        break;
                    }
                case SpectrumAnalyzer.TraceState.Blank:
                    {
                        str = "BLANK";
                        break;
                    }
                case SpectrumAnalyzer.TraceState.View:
                    {
                        str = "VIEW";
                        break;
                    }
                default:
                    {
                        goto case SpectrumAnalyzer.TraceState.Blank;
                    }
            }
            this.Send(string.Format(":TRACe{0}:MODE {1}", new object[] { TraceIndex, str }));
            this.WaitOpc();
        }

        /// <summary>
        /// 查询迹线的状态
        /// </summary>
        /// <param name="TraceIndex">迹线序号</param>
        /// <returns></returns>
        public override TraceState GetTraceStateByIndex(int TraceIndex)
        {
            string str = this.Query(string.Format(":TRACe{0}:MODE?", TraceIndex));
            if (str == "MAXHold")
            {
                return SpectrumAnalyzer.TraceState.Maxhold;
            }
            if (str == "MINHold")
            {
                return SpectrumAnalyzer.TraceState.Minhold;
            }
            if (str == "AVERage")
            {
                return SpectrumAnalyzer.TraceState.Average;
            }
            if (str == "WRITe")
            {
                return SpectrumAnalyzer.TraceState.Write;
            }
            if (str == "VIEW")
            {
                return SpectrumAnalyzer.TraceState.View;
            }
            return SpectrumAnalyzer.TraceState.Blank;
        }

        public override bool PreAmplifier
        {
            get
            {
                if (this.Query("INP:GAIN:STAT?").ToUpper().StartsWith("1"))
                { return true; }
                else
                { return false; }
            }
            set
            {
                base.Send(string.Concat("INP:GAIN:STAT ", (value ? "ON" : "OFF")));
                this.WaitOpc();
            }
        }
        #endregion

    }
}
