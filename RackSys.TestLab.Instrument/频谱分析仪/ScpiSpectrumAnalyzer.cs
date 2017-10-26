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
    internal class ScpiSpectrumAnalyzer : SpectrumAnalyzer
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
                return this.QueryNumber(":SENSe:POWer:RF:ATTenuation?");
            }
            set
            {
                this.SendNumber(":SENSe:POWer:RF:ATTenuation ", (double)((int)Math.Round(value)));
                this.WaitOpc();
                Thread.Sleep(1000);
            }
        }

        public override void Set_RefLvlOffsetByWindow(int WindowNumber, double OffsetValue)
        {
            //:DISPlay:WINDow[1]:TRACe:Y[:SCALe]:RLEVel:OFFSet <rel_ampl>
            this.Send(":DISP:WIND" + WindowNumber.ToString() + ":TRAC:Y:RLEV:OFFS " + OffsetValue.ToString());
        }


        public override bool AutoAlignEnabled
        {
            get
            {
                if (this.Query(":CALibration:AUTO?").ToUpper().StartsWith("ON"))
                {
                    return true;
                }
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
        public override void NoiseFigureCalNow(string ENRTable)
        {
            if (!string.IsNullOrEmpty(ENRTable))
            {
                this.NFENRMode = NoiseFigureENRMode.TABLe;
                this.strCalENRTable = ENRTable;
            }
            this.Send(":SENSe:NFIGure:CALibration:INITiate");
        }
        public override SpectrumAnalyzer.NoiseFigureENRMode NFENRMode
        {
            get
            {
                string strResult = "";
                strResult = base.Query(":NFIGure:CORRection:ENR:MODE?");
                foreach (SpectrumAnalyzer.NoiseFigureENRMode item in Enum.GetValues(typeof(SpectrumAnalyzer.NoiseFigureENRMode)))
                {
                    if (item.ToString() == strResult)
                    { return item; }
                }
                throw new Exception("未找到对应ENR模式！");
            }
            set
            {
                base.Send(":NFIGure:CORRection:ENR:MODE " + value.ToString());
            }
        }

        public override string strCalENRTable
        {
            get
            {
                return base.Query(":NFIGure:CORRection:ENR:CALibration:TABLe:DATA?");
            }
            set
            {
                base.Send(string.Concat(":NFIGure:CORRection:ENR:CALibration:TABLe:DATA ", value));
            }
        }

        public ScpiSpectrumAnalyzer.SA_Alignment AutoAlignment
        {
            get
            {
                ScpiSpectrumAnalyzer.SA_Alignment sAAlignment = ScpiSpectrumAnalyzer.SA_Alignment.Alert;
                if (!SpectrumAnalyzer.IsESA(this.Model))
                {
                    string str = this.Query(this.GetAutoAlignmentCmd());
                    if (str.ToUpper() == "ON")
                    {
                        sAAlignment = ScpiSpectrumAnalyzer.SA_Alignment.On;
                    }
                    else if (str.ToUpper() == "OFF")
                    {
                        sAAlignment = ScpiSpectrumAnalyzer.SA_Alignment.Off;
                    }
                }
                else
                {
                    string str1 = this.Query(this.GetAutoAlignmentCmd());
                    if (str1.ToUpper() == "1")
                    {
                        sAAlignment = ScpiSpectrumAnalyzer.SA_Alignment.On;
                    }
                    else if (str1.ToUpper() == "0")
                    {
                        sAAlignment = ScpiSpectrumAnalyzer.SA_Alignment.Off;
                    }
                }
                return sAAlignment;
            }
            set
            {
                this.Send(this.SetAutoAlignmentCmd(value));
                this.WaitOpc();
            }
        }

        public override void SetPowerAvgType()
        {
            
                this.Send("AVER:TYPE RMS");//改变平均类型为功率平均，不采用默认的视频平均
                this.WaitOpc();
            
        }

        /// <summary>
        /// 关闭所有marker显示
        /// </summary>
        public override void AllMarkerOFF()
        {
            this.Send(":CALCulate:MARKer:AOFF");
            this.WaitOpc();

        }

        public override uint Averages
        {
            get
            {
                double aver = this.QueryNumber(":SENSe:AVERage:COUNt?");
                return (uint)aver;
            }
            set
            {
                
                
                this.Send(string.Concat(":SENSe:AVERage:STATe ", (value > 0 ? "ON" : "OFF")));
                this.WaitOpc();
                if (value > 0)
                {
                    this.SendNumber(":SENSe:AVERage:COUNt ", (double)((float)value));
                    this.WaitOpc();
                    //this.SendSCPI("TRAC:TYPE WRIT");
                }
            }
        }
        public override void Set_CorrectionState(int DataCollectionNum, bool State)
        {
            this.Send(":CORRection:CSET" + DataCollectionNum.ToString() + " " +  (State ? "1" : "0"));
            WaitOpc();
        }


        /// <summary>
        /// 设置频谱仪的中心频率
        /// </summary>
        public override double CenterFrequency
        {
            get
            {
                return this.QueryNumber(":SENSe:FREQuency:CENTer?");
            }
            set
            {
                this.SendNumber(":SENSe:FREQuency:CENTer ", value);
                this.WaitOpc();
            }
        }

        public override bool ContinuousSweepEnabled
        {
            get
            {
                return this.QueryNumber(":INITiate:CONTinuous?") > 0;
            }
            set
            {
                this.Send(string.Concat(":INITiate:CONTinuous ", (value ? "ON" : "OFF")));
                this.WaitOpc();
            }
        }

        public override double dBperDiv
        {
            get
            {
                return this.QueryNumber(":DISP:WIND:TRAC:Y:PDIV?");
            }
            set
            {
                this.Send(string.Concat(":DISP:WIND:TRAC:Y:PDIV ", value.ToString(NumberFormatInfo.InvariantInfo), " DB"));
                this.WaitOpc();
                Thread.Sleep(1000);
            }
        }

        public override  void MarkerToCenterFreq(int markerIndex)
        { 
        
            this.Send(string.Concat ("CALC:MARK{0}:CENT",markerIndex ));
            this.WaitOpc();
        }
    


        //2014.3.1 苏渊红添加创建文件目录驱动功能
        public override void CreatFile(string Filename)
        {
            this.Send(string.Concat("mmemory:mdirectory '" + Filename + "'"));
            this.WaitOpc();
            //     this.Send(string.Concat("mmemory:mdirectory ", Filename);
        }


        //2014.3.5 苏渊红添加从ENR Meas Table读取值的功能。
        //public override string ReadENRMeasTable
        //{
        //    get
        //    {
        //        this.Send(string.Concat("INST:SEL NFIGURE"));   
        //        this.Send(string.Concat(":NFIGure:CORRection:ENR:TABLe:DATA:DELete"));      //删除已有的Table
        //        this.Send(string.Concat(":NFIG:CORR:ENR:TABL:SNS"));                        //从SENSOR读取默认值至频谱仪

        //        return "";
        //    }
        //    set
        //    {
                

        //    }
            
        //}


        /// <summary>
        /// 变频模式支持
        /// </summary>
        /// <param name="fixLOFreq"></param>
        /// <param name="isHighConverter"></param>
        public override void NoiseFigureMixerModeSetup(double fixLOFreq, bool isHighConverter)
        {
            throw new Exception("安捷伦频谱仪噪声系数变频模式设置待补充");

        }


        public override bool DisplayEnabled
        {
            get
            {
                return this.QueryNumber(":DISPlay:ENABle?") > 0;
            }
            set
            {
                this.Send(string.Concat(":DISPlay:ENABle ", (value ? "ON" : "OFF")));
                this.WaitOpc();
            }
        }

        public override double FrequencyMax
        {
            get
            {
                SAMode();
                return this.QueryNumber(":SENSe:FREQuency:CENTer? MAX");
            }
        }

        public override bool InputOverload
        {
            get
            {
                bool flag;
                try
                {
                    int num = Convert.ToInt32(this.QueryNumber("STAT:QUES:INT?"));
                    flag = (num & 16) == 16;
                }
                catch
                {
                    flag = false;
                }
                return flag;
            }
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
                        str = "BDEN";
                        break;
                    }
                
                case SpectrumAnalyzer.MarkerFunctionType.BandIntervalPower:
                    {
                        str = "BPOWer";
                        break;
                    }
                case SpectrumAnalyzer.MarkerFunctionType.Noise:
                    {
                        str = "NOISe";
                        break;
                    }
                case SpectrumAnalyzer.MarkerFunctionType.None:
                    {
                        str = "OFF";
                        break;
                    }
                default:
                    {
                        goto case SpectrumAnalyzer.MarkerFunctionType.None;
                    }
            }
            this.Send(string.Format(":CALCulate:MARKer{0}:FUNCtion {1}", new object[] { MarkerIndex, str }));
            this.WaitOpc();
        }

        public override MarkerFunctionType GetMakerFunctionByIndex(int MarkerIndex)
        {
            string str = this.Query(string.Format(":CALCulate:MARKer{0}:FUNCtion?", MarkerIndex));
            if (str == "BPOW")
            {
                return SpectrumAnalyzer.MarkerFunctionType.BandIntervalPower;
            }
            if (str == "NOIS")
            {
                return SpectrumAnalyzer.MarkerFunctionType.Noise;
            }
            return SpectrumAnalyzer.MarkerFunctionType.None;
        }
        public override void SetDetectorMode(int TraceNumber, SpectrumAnalyzer.DetectorMode ModeOfDetector)
        {
            //[:SENSe]:DETector:TRACe[1] | 2 | ...6 AVERage | NEGative | NORMal | POSitive | SAMPle | QPEak | EAVerage | RAVerage
            this.Send(":DET:TRAC" + TraceNumber.ToString() + " " + ModeOfDetector.ToString());
            this.WaitOpc();
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
                case SpectrumAnalyzer.MarkerModeType.Delta:
                    {
                        str = "DELTa";
                        break;
                    }
                case SpectrumAnalyzer.MarkerModeType.Band:
                    {
                        str = "BAND";
                        break;
                    }
                case SpectrumAnalyzer.MarkerModeType.Span:
                    {
                        str = "SPAN";
                        break;
                    }
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
            this.Send(string.Format(":CALCulate:MARKer{0}:MODE {1}", new object[] { MarkerIndex, str }));
            this.WaitOpc();
        }

        public override MarkerModeType GetMarkerModeByIndex(int MarkerIndex)
        {
            string str = this.Query(string.Format(":CALCulate:MARKer{0}:MODE?", MarkerIndex));
            if (str == "POS")
            {
                return SpectrumAnalyzer.MarkerModeType.Position;
            }
            if (str == "DELT")
            {
                return SpectrumAnalyzer.MarkerModeType.Delta;
            }
            if (str == "BAND")
            {
                return SpectrumAnalyzer.MarkerModeType.Band;
            }
            if (str == "SPAN")
            {
                return SpectrumAnalyzer.MarkerModeType.Span;
            }
            return SpectrumAnalyzer.MarkerModeType.Off;
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
            this.Send(string.Format(":CALCulate:MARKer{0}:FCOunt ON", MarkerIndex));
            this.SendNumber(string.Format(":CALCulate:MARKer{0}:X ", MarkerIndex), value);
            this.WaitOpc();
        }

        public override double GetMarkerPositionByIndex(int MarkerIndex)
        {
            return this.QueryNumber(string.Format(":CALCulate:MARKer{0}:X?", MarkerIndex));
        }


        //增加markercount功能
        public override double GetMarkerFCountByIndex(int MarkerIndex)
        {
            this.Send(string.Format(":CALCulate:MARKer{0}:FCOunt ON", MarkerIndex));
            this.WaitOpc();
            System.Threading.Thread.Sleep(1000);
            return this.QueryNumber(string.Format(":CALCulate:MARKer{0}:FCOunt:X?", MarkerIndex));
        }

        

        public override double MarkerPositionPoints
        {
            get
            {
                return this.QueryNumber(":CALC:MARK:X:POS:CENT?");
            }
            set
            {
                this.SendNumber(":CALC:MARK:X:POS:CENT ", value);
                this.WaitOpc();
            }
        }

        public override double MarkerSpanPoints
        {
            get
            {
                return this.QueryNumber(":CALC:MARK:X:POS:SPAN?");
            }
            set
            {
                this.SendNumber(":CALC:MARK:X:POS:SPAN ", value);
                this.WaitOpc();
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

            if (this.QueryNumber(string.Format(":CALCulate:MARKer{0}:STATe?", 1))<1)
            {
                this.Send(string.Format(":CALCulate:MARKer{0}:STATe 1", MarkerIndex)); 
            }
            return this.QueryNumber(string.Format(":CALC:MARK{0}:Y?", MarkerIndex));
        }

        public override double GetMarkerFuntionBPOwerValueByIndex(int MarkerIndex)
        {

           return GetMarkerValueByIndex(MarkerIndex);
        }

        public override double GetMarkerFuntionNoiseValueByIndex(int MarkerIndex)
        {

            return GetMarkerValueByIndex(MarkerIndex);
        }
        public override bool NeedsAlignment
        {
            get
            {
                bool flag;
                try
                {
                    int num = Convert.ToInt32(this.QueryNumber("STAT:QUES:CAL:COND?"));
                    flag = (num & 16384) == 16384;
                }
                catch
                {
                    flag = false;
                }
                return flag;
            }
        }

        public override double RBW
        {
            get
            {
                return this.QueryNumber(":SENSe:BANDwidth:RESolution?");
            }
            set
            {
                this.SendNumber(":SENSe:BANDwidth:RESolution ", value);
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
                return this.QueryNumber(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel?");
            }
            set
            {
                this.SendNumber(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel ", value);
                this.WaitOpc();
            }
        }

        public override bool RFExternalReference
        {
            get
            {
                string str;
                str = (!SpectrumAnalyzer.IsESA(this.Model) ? this.Query(":ROSCillator:SOURCe?") : this.Query(":CAL:FREQ:REF?"));
                return str == "EXT";
            }
        }


        



        public override double Span
        {
            get
            {
                return this.QueryNumber(":SENSe:FREQuency:SPAN?");
            }
            set
            {
                this.SendNumber(":SENSe:FREQuency:SPAN ", value);
                this.WaitOpc();
            }
        }

        public override int SweepPoints
        {
            get
            {
                return (int)this.QueryNumber(":SENSe:SWEep:POInts?");
            }
            set
            {
                this.SendNumber(":SENSe:SWEep:POInts ", (double)value);
                this.WaitOpc();
            }
        }

        public override double SweepTime
        {
            get
            {
                return this.QueryNumber(":SENSe:SWEep:TIME?");
            }
            set
            {
                this.SendNumber(":SENSe:SWEep:TIME ", value);
                this.WaitOpc();
            }
        }


        public override double[] TraceXaxis
        {
            get
            {
                double step = 0;
                double start = StartFrequency;
                if (SweepPoints>1)
                {
                step = Math.Abs(StopFrequency - StartFrequency) / (SweepPoints - 1);
                }
                double[] xValue = new double[SweepPoints];
                for (int i = 0; i < xValue.Length; i++)
                {
                    xValue[i] = start + step * i;

                }
                return xValue;


            }

        }
        //public override uint 
        public override double[] TraceDataByIndex(int TraceIndex)
        {
                byte[] numArray;
                double[] numArray1 = null;
                this.Send("FORMat:TRACE:DATA REAL,32");
                this.WaitOpc();
                this.Send("FORMat:BORDer SWAPped");
                this.WaitOpc();
                try
                {
                    this.m_CommMutex.WaitOne();
                    this.IO.ReadIEEEBlock(string.Format("TRACe:DATA? TRACE{0}", TraceIndex), out numArray);
                    float[] singleArray = new float[(int)numArray.Length / Marshal.SizeOf(typeof(float))];
                    Buffer.BlockCopy(numArray, 0, singleArray, 0, (int)numArray.Length);
                    numArray1 = new double[(int)singleArray.Length];
                    for (int i = 0; i < (int)singleArray.Length; i++)
                    {
                        numArray1[i] = (double)singleArray[i];
                    }
                }
                catch (Exception exception)
                {
                    throw new ApplicationException("Could not read trace data", exception);
                }
                finally
                {
                    m_CommMutex.ReleaseMutex();
                }
                return numArray1;
        }

        //public override void ReadTraceXAndYValue(double StartFreq,double StopFreq,out double[] XAxis, out double[] YAxis)
        //{
        //    int SweepNumber =(int) this.QueryNumber("SWE:POIN?");
        //    XAxis=new double[SweepNumber];
        //    YAxis=new double [SweepNumber];
        //    double step=(StartFreq -StopFreq)/(SweepNumber-1);
        //    SetMarkerModeByIndex(SpectrumAnalyzer.MarkerModeType.Position, 1);
        //    for (int i = 0; i <SweepNumber; i++)
        //    {
        //        SetMarkerPositionByIndex(StartFreq + i * step, 1);
        //        XAxis[i] = GetMarkerPositionByIndex(1);
        //        YAxis[i] = GetMarkerValueByIndex(1);
        //    }
        //}

        /// <summary>
        /// 根据不同的设备，选择不同的文件名和后缀，并拷贝屏幕
        /// </summary>
        private string SaveScreeImageToFile()
        {
            string filename = "";
            if (SpectrumAnalyzer.IsPSA(this.Model) || SpectrumAnalyzer.IsESA(this.Model))
            {
                this.m_ImageDirectory = "C:";
                this.m_ImageExtension = ".GIF";
                filename = "SSA";
            }
            else
            {
                this.m_ImageDirectory = "D:\\User_RackSys\\ScreenImage";
                filename = "SSA";
                this.m_ImageExtension = ".png";
                //filename = "1";
            }

            
            string FullFileName = string.Concat("\"",this.m_ImageDirectory, "\\", filename.ToUpper(), this.m_ImageExtension, "\"");

            this.Send("MMEMory:STORe:SCReen " + FullFileName);
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
            return this.ReadBlock("MMEMory:DATA? " + inImageFileName);
        }


        public override double VBW
        {
            get
            {
                return this.QueryNumber(":SENSe:BANDwidth:VIDeo?");
            }
            set
            {
                this.SendNumber(":SENSe:BANDwidth:VIDeo ", value);
                this.WaitOpc();
            }
        }

        public ScpiSpectrumAnalyzer(string address)
            : base(address)
        {
        }

        public override void DeleteState(string filename)
        {
            if (SpectrumAnalyzer.IsPSA(this.Model) || SpectrumAnalyzer.IsESA(this.Model))
            {
                this.m_stateDirectory = "C:";
                this.m_stateExtension = ".STA";
            }
            else
            {
                this.m_stateDirectory = "D:\\User_My_Documents\\Instrument\\My Documents\\SA\\state";
                this.m_stateExtension = ".State";
            }
            string str = string.Concat(this.m_stateDirectory, "\\", filename.ToUpper(), this.m_stateExtension);
            this.Send(string.Concat(":MMEMory:DELete \"", str, "\""));
            this.WaitOpc();
            this.m_saAlignment = string.Empty;
        }

        protected virtual string GetAutoAlignmentCmd()
        {
            return ":CAL:AUTO?";
        }

        public override void LoadState(string filename)
        {
            if (SpectrumAnalyzer.IsPSA(this.Model) || SpectrumAnalyzer.IsESA(this.Model))
            {
                this.m_stateDirectory = "C:";
                this.m_stateExtension = ".STA";
            }
            else
            {
                this.m_stateDirectory = "D:\\User_My_Documents\\Instrument\\My Documents\\SA\\state";
                this.m_stateExtension = ".State";
            }
            string str = string.Concat(this.m_stateDirectory, "\\", filename.ToUpper(), this.m_stateExtension);
            this.Send(string.Concat(":MMEMory:LOAD:STATe 1,\"", str, "\""));
            if (this.m_saAlignment.Length > 0)
            {
                this.Send(string.Concat(":CALibration:AUTO ", this.m_saAlignment));
                this.WaitOpc();
                this.m_saAlignment = string.Empty;
            }
        }




        public override void TOIMode()
        {
            this.Query("INITiate:TOI;*OPC?");

        }
        public override double TOI_ReferenceLevel
        {
            get
            {
                if (this.IsXA())
                {
                    return this.QueryNumber("DISPlay:TOI:VIEW:WINDow:TRACe:Y:SCALe:RLEVel? ");
                }
                else
                {
                    return this.dBperDiv;
                }
            }
            set
            {
                if (this.IsXA())
                {
                    this.SendNumber("DISPlay:TOI:VIEW:WINDow:TRACe:Y:SCALe:RLEVel " , value);
                    this.WaitOpc();
                }
                else
                {
                    this.ReferenceLevel = value;
                }
            }
        }
        public override double TOI_RBW
        {
            get
            {
                if (this.IsXA())
                {
                    return this.QueryNumber(":TOI:BWID? ");
                }
                else
                {
                    return this.RBW;
                }
            }
            set
            {
                if (this.IsXA())
                {
                    this.SendNumber(":TOI:BWID " , value);
                    this.WaitOpc();
                }
                else
                {
                    this.RBW = value;
                }

            }
        }
        public override double TOI_Span
        {
            get
            {
                if (this.IsXA())
                {
                    return this.QueryNumber("SENSe:TOI:FREQuency:SPAN?");
                }
                else
                {
                    return this.Span;
                }
            }
            set
            {
                if (this.IsXA())
                {
                    this.SendNumber("SENSe:TOI:FREQuency:SPAN ", value);
                    this.WaitOpc();
                }
                else
                {
                    this.Span = value;
                }
            }
        }
        public override double TOI_VBW
        {
            get
            {
                if (this.IsXA())
                {
                    return this.QueryNumber("SENSe:TOI:BANDwidth:VIDeo? ");
                }
                else
                {
                    return this.VBW;
                }
            }
            set
            {
                if (this.IsXA())
                {
                    this.SendNumber("SENSe:TOI:BANDwidth:VIDeo ", value);
                    this.WaitOpc();
                }
                else
                {
                    this.VBW = value;
                }
            }
        }

        public override double TOI_dBperDiv
        {
            get
            {
                if (this.IsXA())
                {
                    return this.QueryNumber("DISP:TOI:VIEW:WIND:TRAC:Y:PDIV? ");
                }
                else
                {
                    return this.dBperDiv;
                }
            }
            set
            {
                if (this.IsXA())
                {
                    this.SendNumber("DISP:TOI:VIEW:WIND:TRAC:Y:PDIV ", value);
                    this.WaitOpc();
                }
                else
                {
                    this.dBperDiv = value;
                }

            }
        }

        public override double StartFrequency
        {
            get
            {
                return this.QueryNumber(":SENSe:FREQuency:STARt?");
            }
            set
            {
                this.SendNumber(":SENSe:FREQuency:STARt ", value);
                this.WaitOpc();
            }
        }

        public override double StopFrequency
        {
            get
            {
                return this.QueryNumber(":SENSe:FREQuency:STOP?");
            }
            set
            {
                this.SendNumber(":SENSe:FREQuency:STOP ", value);
                this.WaitOpc();
            }
        }

        public override void MarkerPeakSearch(int MarkerIndex)
        {
            this.Send(string.Format( ":CALCulate:MARKer{0}:MAXimum",MarkerIndex));
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
            int timeout = this.Timeout;
            this.Send("INIT:CONT ON");
            this.WaitOpc();
            this.Timeout = timeout;
        }

        public override void MarkerToRefLev(int MarkerNum)
        {
            int timeout = this.Timeout;
            this.Send(string.Format("CALCulate:MARKer"+MarkerNum+":SET:RLEVel"));
            this.WaitOpc();
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
                //this.Send("POW:ATT:STEP 2");//todo
                //this.WaitOpc();
            }
            else if(SpectrumAnalyzer.IsPXA(this.Model)) //2014.3.3苏渊红添加代码
            {
                this.Send("INST:SEL SA");
                this.WaitOpc();
            }
        }

        //2014.3.11苏渊红添加根据MARKER的编号读取NF驱动
        public override double GetMarkerNFValueByIndex(int MarkerIndex)
        {
            this.Send(":CALC:NFIG:MARK1:TRAC TRAC1");
            this.WaitOpc();
            return this.GetMarkerValueByIndex(MarkerIndex);
        }

        //2014.3.11苏渊红添加根据MARKER的编号读取NF的Gain的驱动
        public override double GetMarkerGainValueByIndex(int MarkerIndex)
        {
            this.Send(":CALC:NFIG:MARK1:TRAC TRAC2");
            this.WaitOpc();
            return this.GetMarkerValueByIndex(MarkerIndex);
        }

        public override void NoiseFigureMode()// 郝佳添加驱动,2013.12.2
        {
            this.Send(":INST NFIG");
            this.WaitOpc();
        }

        //public override void NoiseFigureCalNow(string ENRTable)
        //{
        //    if (!string.IsNullOrEmpty(ENRTable))
        //    {
        //        throw new Exception("需针对不能自动读取ENR的部分添加完整");
            
        //    }
        //    this.Send(":SENSe:NFIGure:CALibration:INITiate");
        //    this.WaitOpc();
        //}

        public override void NFCALStatON()
        {

            this.Send(":NFIG:CAL:STAT ON");
            this.WaitOpc();
        }

        public override void BandIntervalPowerMarkerSpan(uint MarkerNumber,double Span)// 郝佳添加驱动,2013.12.3
        {
            this.Send(":CALC:MARK" + MarkerNumber + ":FUNC:BAND:SPAN " + Span);
            this.WaitOpc();
        }

        public override void NoiseFigureLossCompModeBeforeDUT(SpectrumAnalyzer.NoiseFigureLossCompMode Mode)// 郝佳添加驱动,2013.12.4
        {
            this.Send("NFIG:CORR:LOSS:BEF:MODE " + Mode);
            this.WaitOpc();
        }

        public override void ClearLossCompTableBeforeDUT()// 郝佳添加驱动,2013.12.4
        {
            this.Send(":NFIGure:CORRection:LOSS:BEFore:TABLe:DATA:DELete");
            this.WaitOpc();
        }
        public override void SaveLossCompTableBeforeDUT(string FileAddressAndName)// 郝佳添加驱动,2013.12.4
        {
            this.Send(":MMEM: LOAD:LOSS BEF,FileAddressAndName");
            this.WaitOpc();
        }




        //输入路径补偿数据表格BeforeDUT
        public override void SetLossCompTableBeforeDUT(string FreqAndLossList)// 郝佳添加驱动,2013.12.4
        {
            this.Send(":NFIG:CORR:LOSS:BEF:TABL:DATA " + FreqAndLossList);
            this.WaitOpc();
        }


        //设置噪声系数测试的RBW
        public override double NF_RBW
        {
            get
            {
                if (this.IsXA())
                {
                    return this.QueryNumber(":NFIG:BWID? ");
                }
                else
                {
                    return this.RBW;
                }
            }
            set
            {
                if (this.IsXA())
                {
                    this.SendNumber(":NFIG:BWID ", value);
                    this.WaitOpc();
                }
                else
                {
                    this.RBW = value;
                }

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
            this.Send(":NFIG:CORR:LOSS:AFT:MODE " + Mode);
            this.WaitOpc();
        }
        public override void ClearLossCompTableAfterDUT()// 郝佳添加驱动,2013.12.4
        {
            this.Send(":NFIGure:CORRection:LOSS:AFTer:TABLe:DATA:DELete");
            this.WaitOpc();
        }

        //输入路径补偿数据表格AfterDUT
        public override void SetLossCompTableAfterDUT(string FreqAndLossList)// 郝佳添加驱动,2013.12.4
        {
            this.Send(":NFIG:CORR:LOSS:AFT:TABL:DATA " + FreqAndLossList);
            this.WaitOpc();
        }






        public override TOITestResult TOI_GetTestResult()
        {
            string RetResult = this.Query(":READ:TOI2?");
            
            string[] TestResultStrs = RetResult.Split(new char[]{','});
            TOITestResult tmpTOITestResult = new TOITestResult();
            tmpTOITestResult.LowerBaseFreq = double.Parse(TestResultStrs[4]);
            tmpTOITestResult.LowerBasePower = double.Parse(TestResultStrs[5]);
            tmpTOITestResult.UpperBaseFreq = double.Parse(TestResultStrs[6]);
            tmpTOITestResult.UpperBasePower = double.Parse(TestResultStrs[7]);

            tmpTOITestResult.Lower_InterModulate_Freq = double.Parse(TestResultStrs[8]);
            tmpTOITestResult.Lower_InterModulate_Power = double.Parse(TestResultStrs[9]);

            tmpTOITestResult.Upper_InterModulate_Freq = double.Parse(TestResultStrs[11]);
            tmpTOITestResult.Upper_IntreModulate_Power = double.Parse(TestResultStrs[12]);
            return tmpTOITestResult;
        }

        public override void SaveState(string filename)
        {
            if (SpectrumAnalyzer.IsPSA(this.Model) || SpectrumAnalyzer.IsESA(this.Model))
            {
                this.m_stateDirectory = "C:";
                this.m_stateExtension = ".STA";
            }
            else
            {
                this.m_stateDirectory = "D:\\User_My_Documents\\Instrument\\My Documents\\SA\\state";
                this.m_stateExtension = ".State";
            }
            string str = string.Concat(this.m_stateDirectory, "\\", filename.ToUpper(), this.m_stateExtension);
            string str1 = this.Query(string.Concat(":MMEMory:CATalog? \"", this.m_stateDirectory, "\""));
            char[] charArray = ",".ToCharArray();
            if (Convert.ToDouble(str1.Split(charArray, 3)[1]) < 30000)
            {
                throw new ApplicationException("Not enough memory to store spectrum analyzer state.  Make more room so that application can store current state.");
            }
            if (str1.IndexOf(filename.ToUpper()) != -1)
            {
                this.Send(string.Concat(":MMEMory:DELete \"", str, "\""));
            }
            this.Send(string.Concat(":MMEMory:STORe:STATe 1,\"", str, "\""));
            this.WaitOpc();
            this.m_saAlignment = this.Query(":CALibration:AUTO?");
        }

        //2014.3.3 苏渊红添加代码，保存state至新建路径
        public override string SaveStateToPath(string filename)
        {
            if (!filename.Contains(".state"))
            {
            filename = filename + ".state";
            }
            this.Send(string.Concat(":MMEMory:STORe:STATe 1,\"", filename, "\""));
            this.WaitOpc();
            return filename;
            
        }
        public override void LoadStateFromPath(string filename)
        {
            this.Send(string.Concat(":MMEMory:LOAD:STATe 1,\"", filename, "\""));
            this.WaitOpc();
        }
    

        protected virtual string SetAutoAlignmentCmd(ScpiSpectrumAnalyzer.SA_Alignment val)
        {
            if (SpectrumAnalyzer.IsESA(this.Model))
            {
                string str = "OFF";
                if (val == ScpiSpectrumAnalyzer.SA_Alignment.On)
                {
                    str = "ON";
                }
                return string.Concat(":CAL:AUTO ", str);
            }
            string str1 = "ALERT";
            if (val == ScpiSpectrumAnalyzer.SA_Alignment.On)
            {
                str1 = "ON";
            }
            else if (val == ScpiSpectrumAnalyzer.SA_Alignment.Off)
            {
                str1 = "OFF";
            }
            return string.Concat(":CAL:AUTO ", str1);
        }

        /// <summary>
        /// 触发单次扫描
        /// </summary>
        public override void Sweep(int timeMS= 5000)
        {
            this.Send(":INITiate:IMMediate");
            this.WaitOpc((int)timeMS);
        }

        public enum SA_Alignment
        {
            Off,
            On,
            Alert
        }


        public override void SelectDisplayWindow(uint WindowNumber)
        {
            this.Send(":DISP:WIND " + WindowNumber);
            this.WaitOpc();
        }
        public override void SetNFWindowAutoScale(uint WindowNumber,bool value)
        {
            this.Send(":DISP:NFIG:VIEW:WIND"+WindowNumber+":TRAC:Y:COUP "+(value ? "ON" : "OFF"));
            this.WaitOpc();
        }

        public override void SetWindowZoom(bool value)
        {
            this.Send(":DISPlay:WINDow:FORMat:" + (value ? "ZOOM" : "TILE"));
            this.WaitOpc();
        }

        public override void SetDisPlayFormat(DisPlayModeType value)
        {
            this.Send(":DISPlay:NFIGure:FORMat " + value);
            this.WaitOpc();
        }

        public override double Inter_Attenuation
        {
            get
            {

                return this.QueryNumber("POW:ATT? ");

            }
            set
            {
                this.Send(string.Concat("POW:ATT ", value));
                this.WaitOpc();
            }
        }

        public override double AMP_Offset
        {
            get
            { return -1*this.QueryNumber("CORR:SA:GAIN?"); }
            set
            { 
                this.Send(string.Concat("CORR:SA:GAIN ",-1*value ));
            this.WaitOpc();
            }
        
        
        }


        /// <summary>
        /// 关闭所有correction
        /// </summary>
        public void AllCorrectionOFF()
        {
            for (int i = 1; i < 7; i++)
            {
                this.Send(string.Concat("SENS:CORR:CSET",i,"OFF"));
                this.WaitOpc();
            }

        
        }


        /// <summary>
        /// 设置table的值例子：CORR:CSET1:DATA 10000000, –1.0, 20000000, 1.0
        /// </summary>
        public override string CorrectionTable1
        {
            get
            { return this.Query("CORR:CSET1:DATA?"); }
            set 
            {
                AllCorrectionOFF();

                this.Send(string.Concat("CORR:CSET1:DATA ",value ));
                this.WaitOpc();
                this.Send(string.Concat("SENS:CORR:CSET1 ON"));
                this.WaitOpc();

            }
        
        
        }




        ///频谱仪参数结构类型
        //public struct SpecTrumAnalyzerSetting 
        //{
        //    public bool NFStateEnable;
        //    public bool SingleSweep;
        //    public double FreqStart;
        //    public double FreqStop;
        //    public double RBW;
        //    public double ReferenceLevel;
        //    public double InterAttenuation;
        //    public double Yscale;
        //    public double Offset;
        //    public uint AverageNumber;
        //    public string NFStateFilePathAndName ;
        //    public uint SweepPoints;          


        //}


        //public class SpecTrumAnalyzerSetting
        //{
        //    private bool m_NFStateEnable=false;

        //    /// <summary>
        //    /// true为NF模式，false为扫频模式
        //    /// </summary>
        //    public bool NFStateEnable
        //    {
        //        get { return m_NFStateEnable; }
        //        set { m_NFStateEnable = value; }
        //    }
        //    private bool m_SingleSweep=false;

        //    /// <summary>
        //    /// true为单次扫描，false为连续扫描
        //    /// </summary>
        //    public bool SingleSweep
        //    {
        //        get { return m_SingleSweep; }
        //        set { m_SingleSweep = value; }
        //    }



        //    private double m_FreqStart=1e9;

        //    /// <summary>
        //    /// 起始频率，以Hz为单位
        //    /// </summary>
        //    public double FreqStart
        //    {
        //        get { return m_FreqStart; }
        //        set { m_FreqStart = value; }
        //    }
        //    private double m_FreqStop=2e9;
        //    /// <summary>
        //    /// 终止频率，以hz为单位
        //    /// </summary>
        //    public double FreqStop
        //    {
        //        get { return m_FreqStop; }
        //        set { m_FreqStop = value; }
        //    }
        //    private double m_RBW=1e6;
        //    /// <summary>
        //    ///RBW设置，扫频模式为RBW，NF模式为分析贷款的值
        //    /// </summary>
        //    public double RBW
        //    {
        //        get { return m_RBW; }
        //        set { m_RBW = value; }
        //    }
        //    private double m_ReferenceLevel=0;

        //    /// <summary>
        //    /// 参考电平，扫频模式有效
        //    /// </summary>
        //    public double ReferenceLevel
        //    {
        //        get { return m_ReferenceLevel; }
        //        set { m_ReferenceLevel = value; }
        //    }
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    private double m_InterAttenuation=10;

        //    public double InterAttenuation
        //    {
        //        get { return m_InterAttenuation; }
        //        set { m_InterAttenuation = value; }
        //    }
        //    /// <summary>
        //    /// 0为auto，大于0为自定义
        //    /// </summary>
        //    private double m_Yscale;

        //    public double Yscale
        //    {
        //        get { return m_Yscale; }
        //        set { m_Yscale = value; }
        //    }
        //    private double m_Offset=0;
        //    /// <summary>
        //    /// Y轴偏置
        //    /// </summary>
        //    public double Offset
        //    {
        //        get { return m_Offset; }
        //        set { m_Offset = value; }
        //    }
        //    private uint m_AverageNumber=1;
        //    /// <summary>
        //    /// 读数平均次数
        //    /// </summary>
        //    public uint AverageNumber
        //    {
        //        get { return m_AverageNumber; }
        //        set { m_AverageNumber = value; }
        //    }


        //    private string m_NFStateFilePathAndName=string.Empty;
        //    /// <summary>
        //    /// 噪声系数状态文件路径及名称
        //    /// </summary>
        //    public string NFStateFilePathAndName
        //    {
        //        get { return m_NFStateFilePathAndName; }
        //        set { m_NFStateFilePathAndName = value; }
        //    }
        //    private uint m_SweepPoints=11;

        //    public uint SweepPoints
        //    {
        //        get { return m_SweepPoints; }
        //        set { m_SweepPoints = value; }
        //    }


        //}


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
                if(!(SASetting.NFStateFilePathAndName==string.Empty))
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
                this.WaitOpc();
                SASettingResult.FreqStop = this.StopFrequency;
                this.WaitOpc();
                SASettingResult.RBW = this.RBW;
                this.WaitOpc();
                SASettingResult.AverageNumber = this.Averages;
                this.WaitOpc();
                SASettingResult.InterAttenuation = this.Inter_Attenuation;
                this.WaitOpc();
                SASettingResult.ReferenceLevel = this.ReferenceLevel;
                this.WaitOpc();
                SASettingResult.Yscale = this.dBperDiv;
                this.WaitOpc();
                SASettingResult.Offset = this.AMP_Offset;
                this.WaitOpc();
                SASettingResult.SingleSweep = !this.ContinuousSweepEnabled;
                this.WaitOpc();
                
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
                SASettingResult.NFStateFilePathAndName = "temp.state";
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
            SASetting.NFStateFilePathAndName = @"D:\RackSys\Temp.state";
            this.SaveStateToPath(SASetting.NFStateFilePathAndName);
            SpectrumAnalyzerConfigurationRead(isNFMode,  SASetting);





            return true;
        }


        /// <summary>
        /// 使用频谱仪指定的状态文件
        /// </summary>
        /// <returns></returns>
        public override bool SpectrumAnalyzerUseSelectedState(bool isNFMode, string stateFilepPathAndName, SpecTrumAnalyzerSetting SASetting)
        {
            this.LoadStateFromPath(stateFilepPathAndName);
            SASetting.NFStateFilePathAndName = stateFilepPathAndName;
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

                calsetResult.Add(calsets[i].Remove(0, 1));

            }




        }


        public override void ReadNFTraceData(out double[] noisefigure)
        {
            double start = this.StartFrequency;
            double stop = this.StopFrequency;
            double points = this.SweepPoints;
            double step = (stop - start) / (points - 1);
            this.WaitOpc();
            noisefigure = new double[(int)points];
            for (int i = 0; i < points; i++)
            {
                noisefigure[i] = this.QueryNumber(string.Concat(":TRACe:CORRected:AMPLitude? NFIGure,", (start + step * i)));
                noisefigure[i] = Math.Round(noisefigure[i], 2);

            }


        }

        public override void ReadGainTraceData(out double[] gain)
        {
            double start = this.StartFrequency;
            double stop = this.StopFrequency;
            double points = this.SweepPoints;
            double step = (stop - start) / (points - 1);
            this.WaitOpc();


            gain = new double[(int)points];
            for (int i = 0; i < points; i++)
            {
                gain[i] = this.QueryNumber(string.Concat(":TRACe:CORRected:AMPLitude? GAIN,", (start + step * i)));
                gain[i] = Math.Round(gain[i], 2);

            }


        }

        //杨飞添加驱动 2016.01.07
        #region marker在不同迹线以及迹线状态的设置相关

        /// <summary>
        /// 读取Marker在哪条迹线上
        /// </summary>
        /// <param name="MarkerIndex">Marker序号</param>
        /// <returns></returns>
        public override int GetTraceNumberByIndex(int MarkerIndex)
        {
            int TraceNumber;
            TraceNumber = (int)this.QueryNumber(string.Format(":CALCulate:MARKer{0}:TRACe?", MarkerIndex));
            return TraceNumber;
        }

        /// <summary>
        /// 设置迹线的状态
        /// </summary>
        /// <param name="value">迹线的状态值</param>
        /// <param name="TraceIndex">迹线序号</param>
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
            //this.Send(string.Format(":TRACe{0}:MODE {1}", new object[] { TraceIndex, str }));
            this.Send(string.Format(":TRACe{0}:TYPE {1}", new object[] { TraceIndex, str }));
            //this.Send(":TRAC" + TraceNumber.ToString() + ":TYPE " + ModeOfTrace.ToString());
            this.WaitOpc();
        }
        //public override void SetTraceMode(int TraceNumber, SpectrumAnalyzer.TraceMode ModeOfTrace)
        //{
        //    //:TRACe[1]|2|...6:TYPE WRITe|AVERage|MAXHold|MINHold
        //    this.Send(":TRAC" + TraceNumber.ToString() + ":TYPE " + ModeOfTrace.ToString());
        //}
        /// <summary>
        /// 查询迹线的状态
        /// </summary>
        /// <param name="TraceIndex">迹线序号</param>
        /// <returns></returns>
        public override TraceState GetTraceStateByIndex(int TraceIndex)
        {
            string str = this.Query(string.Format(":TRACe{0}:MODE?", TraceIndex));
            if (str == "MAXH")
            {
                return SpectrumAnalyzer.TraceState.Maxhold;
            }
            if (str == "MINH")
            {
                return SpectrumAnalyzer.TraceState.Minhold;
            }
            if (str == "AVER")
            {
                return SpectrumAnalyzer.TraceState.Average;
            }
            if (str == "WRIT")
            {
                return SpectrumAnalyzer.TraceState.Write;
            }
            if (str == "VIEW")
            {
                return SpectrumAnalyzer.TraceState.View;
            }
            return SpectrumAnalyzer.TraceState.Blank;
        }

        /// <summary>
        /// 触发模式
        /// </summary>
        public override TriggerMode TriggerModeState
        {
            get;
            set;
        }

        /// <summary>
        /// 设置触发模式
        /// </summary>
        /// <param name="value">状态模式</param>
        //public override void SetTriggerMode(TriggerMode value)
        //{
        //    throw new Exception("该型号仪表驱动暂未实现！");

        //}

        /// <summary>
        /// 获取Marker的开关状态
        /// </summary>
        /// <param name="MarkerIndex">Marker的序号</param>
        public override bool GetMarkerStateByIndex(int MarkerIndex)
        {
            if (this.Query(":CALC:MARK" + MarkerIndex.ToString() + ":STAT?").Contains("1") || this.Query(":CALC:MARK" + MarkerIndex.ToString() + ":STAT?").ToUpper().StartsWith("ON"))
            {
                return true;
            }
            return false;
        }

        public override void SetMarkerStateByIndex(int MarkerIndex, bool value)
        {
            this.Send(string.Format(":CALC:MARK{0}:STAT {1}",MarkerIndex,(value ? "1" : "0")));
            this.WaitOpc();
        }

        public override void DoAlignNow(AlignmentsNow AlignMeasure)
        {
            string str;
            switch (AlignMeasure)
            {
                case SpectrumAnalyzer.AlignmentsNow.All:
                    {
                        str = "";
                        break;
                    }
                case SpectrumAnalyzer.AlignmentsNow.AllbutRF:
                    {
                        str = ":NRF";
                        break;
                    }
                case SpectrumAnalyzer.AlignmentsNow.ExternalMixer:
                    {
                        str = ":EMIXer";
                        break;
                    }
                case SpectrumAnalyzer.AlignmentsNow.RF:
                    {
                        str = ":RF";
                        break;
                    }
                default:
                    {
                        goto case SpectrumAnalyzer.AlignmentsNow.All;
                    }
            }
            this.Send(string.Concat(":CALibration" + str));
            this.WaitOpc();
        }

        public override void SetMarkerToTraceIndex(int MarkerIndex, int TraceIndex)
        {
            this.Send(string.Format(":CALCulate::MARKer{0}:TRACe {1}", MarkerIndex, TraceIndex));
            this.WaitOpc();
        }

        public override bool EnabledDisplayLine
        {
            get
            {
                if (this.Query("DISP:WIND:TRAC:Y:DLIN:STAT?").Contains("1") || this.Query("DISP:WIND:TRAC:Y:DLIN:STAT?").Contains("ON"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                this.Send(string.Format("DISP:WIND:TRAC:Y:DLIN:STAT {0}", (value ? "ON" : "OFF")));
                this.WaitOpc();
            }
        }

        public override void DisplayLineValue(double value)
        {
            this.Send(string.Format("DISP:WIND:TRAC:Y:DLIN {0}", value));
            this.WaitOpc();
        }

        public override double TriggerDelay
        {
            get
            {
                return this.QueryNumber("TRIG:DEL?");
            }
            set
            {
                if (value == 0)
                {
                    this.Send("TRIG:DEL:STAT OFF");
                    this.WaitOpc();
                }
                else
                {
                    this.Send("TRIG:DEL:STAT ON");
                    this.WaitOpc();
                    this.Send(string.Format("TRIG:DEL {0}", value));
                    this.WaitOpc();
                }
            }
        }

        //public override void MarkerEnabelForE4446()
        //{ }
        public override bool PreAmplifier
        {
            get
            {
                if (this.Query(":POWer:GAIN?").ToUpper().StartsWith("1"))
                { return true; }
                else
                { return false; }
            }
            set
            {
                this.Send(string.Concat(":POWer:GAIN ", (value ? "ON" : "OFF")));
            }
        }

        #endregion



    }
}
