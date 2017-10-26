using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;


namespace RackSys.TestLab.Instrument
{
    public class AV3985A:NoiseFigureAnalyzer
    {
        public AV3985A(string addr)
            : base(addr)
        { }

        protected override void DetermineOptions()
        {
            //base.DetermineOptions();
        }

        /// <summary>
        /// 截屏：AV3985不支持截屏功能
        /// </summary>
        /// <returns>直接抛出无图异常，无返回图形数据</returns>
        public override System.Drawing.Image CaptureScreenImage()
        {
            //throw new NotImplementedException();
            return null;
        }

        public override void Reset()
        {
            base.Send("*RST");
            WaitOpc();
        }

        /// <summary>
        /// AV3985A没有Preset功能
        /// </summary>
        public override void Preset()
        {
            base.Send("*RST");
            //base.Send("SYSTem:PRES");
            System.Threading.Thread.Sleep(10000);
        }

        //protected override void DetermineOptions()
        //{
        //    //base.DetermineOptions();
        //}

        /// <summary>
        /// 显示格式
        /// </summary>
        public override NoiseFigureAnalyzer.FormatOfDisplay DisplayFormat
        {
            //:DISPlay:FORMat GRAPh|TABLe|METer
            get
            {
                Exception ex = new Exception();
                string Temp_DispForm = base.Query(":DISPlay:FORMat?");
                if (Temp_DispForm == "GRAPh")
                { return FormatOfDisplay.GRAPh; }
                else if (Temp_DispForm == "TABLe")
                { return FormatOfDisplay.TABLe; }
                else if (Temp_DispForm == "METer")
                { return FormatOfDisplay.METer; }
                else
                { throw ex; }
            }
            set
            {
                base.Send(":DISPlay:FORMat " + value.ToString());
            }
        }

        public override bool FullScreenState
        {
            //设置指令语法 :DISPlay:FULLscreen OFF|ON|0|1
            //查询指令语法 :DISPlay:FULLscreen?
            get
            {
                return base.Query(":DISPlay:FULLscreen?") == "1";
            }
            set
            {
                base.Send(":DISPlay:FULLscreen " + (value ? "1" : "0"));
            }
        }

        public override NoiseFigureAnalyzer.StateOfWindowZoom WinZoomState
        {
            //设置指令语法 :DISPlay:ZOOM:WINDow UPPer|LOWer
            //查询指令语法 :DISPlay:ZOOM:WINDow?
            get
            {
                Exception ex = new Exception();
                string Temp_WinZoomState = base.Query(":DISPlay:ZOOM:WINDow?");
                if (Temp_WinZoomState == "OFF")
                { return StateOfWindowZoom.OFF; }
                else if (Temp_WinZoomState == "UPP")
                { return StateOfWindowZoom.UPPer; }
                else if (Temp_WinZoomState == "LOW")
                { return StateOfWindowZoom.LOWer; }
                else
                { throw ex = new Exception("Unidentified Response: " + Temp_WinZoomState); }
            }
            set
            {
                base.Send(":DISPlay:ZOOM:WINDow " + value.ToString());
            }
        }

        public override void SetYScalePerDiv(NoiseFigureAnalyzer.TypeOfResults ResultsType, double ScalePerDiv)
        {
            //:DISPlay:TRACe:Y[:SCALe]:PDIVision <result>,<value>
            string strResultsType = "";
            if (ResultsType == TypeOfResults.YFACtor)
            { strResultsType = "FACtor"; }

            base.Send(":DISPlay:TRACe:Y:PDIVision " + strResultsType + "," + ScalePerDiv.ToString());
        }

        public override void SetGraphUpperLimit(NoiseFigureAnalyzer.TypeOfTrace TraceType, double UpperLimit)
        {
            //:DISPlay:TRACe:Y[:SCALe]:UPPer <trace>,<value>
            string strTraceType = "";
            if (TraceType == TypeOfTrace.YFACtor)
            { strTraceType = "FACtor"; }
            base.Send(":DISPlay:TRACe:Y:UPPer " + strTraceType + "," + UpperLimit.ToString());
        }

        public override void SetGraphLowerLimit(NoiseFigureAnalyzer.TypeOfTrace TraceType, double LowerLimit)
        {
            //:DISPlay:TRACe:Y[:SCALe]:LOWer <trace>,<value>
            string strTraceType = "";
            if (TraceType == TypeOfTrace.YFACtor)
            { strTraceType = "FACtor"; }
            base.Send(":DISPlay:TRACe:Y:LOWer " + strTraceType + "," + LowerLimit.ToString());
        }

        public override void SetReferenceLevelValue(NoiseFigureAnalyzer.TypeOfResults ResultType, double RefLvlValue)
        {
            //:DISPlay:TRACe:Y:RLEVel:VALue <result>,<value>
            //！注意：明确当前激活窗口是上或者下
            string strResultType = "";
            if (ResultType == TypeOfResults.YFACtor)
            { strResultType = "FACtor"; }
            base.Send(":DISPlay:TRACe:Y:RLEVel:VALue " + strResultType + "," + RefLvlValue.ToString());
        }

        /// <summary>
        /// DUT类型
        /// </summary>
        public override NoiseFigureAnalyzer.TypeOfDUT DUTType
        {
            //设置指令语法 :SENSe:CONFigure:MODE:DUT AMPLifier|DOWNconv|UPConv
            //查询指令语法 :SENSe:CONFigure:MODE:DUT?
            get
            {
                Exception ex = new Exception();
                string Temp_DUTMode = base.Query(":SENS:CONF:MODE:DUT?");
                if (Temp_DUTMode == "AMPLifier" || Temp_DUTMode == "AMPL")
                { return TypeOfDUT.AMPLifier; }
                else if (Temp_DUTMode == "DOWNconv" || Temp_DUTMode == "DOWN")
                { return TypeOfDUT.DOWNconv; }
                else if (Temp_DUTMode == "UPConv" || Temp_DUTMode == "UPC")
                { return TypeOfDUT.UPConv; }
                else
                { throw ex; }
            }
            set
            {
                base.Send(":SENSe:CONFigure:MODE:DUT " + value.ToString());
                WaitOpc();
            }
        }

        public override NoiseFigureAnalyzer.TypeOfNoiseSource NoiseSource
        {
            //设置指令语法 :SENSe:SOURce:NOISe:STYLe SNS|NORMal
            //查询指令语法 :SENSe:SOURce:NOISe:STYLe?
            get
            {
                Exception ex = new Exception();
                string Temp_NoiseSource = base.Query(":SENS:SOUR:NOIS:STYL?");
                if (Temp_NoiseSource == "NORMal" || Temp_NoiseSource == "NORM" || Temp_NoiseSource == "NORMAL")
                { return TypeOfNoiseSource.NORMal; }
                else if ((Temp_NoiseSource == "SNS")||(Temp_NoiseSource == "SN"))
                { return TypeOfNoiseSource.SNS; }
                else
                { throw ex; }
            }
            set
            {
                base.Send(":SENSe:SOURce:NOISe:STYLe " + value.ToString());
                WaitOpc();
            }
        }

        public override NoiseFigureAnalyzer.ModeOfFrequency FreqMode
        {
            //设置指令语法 :SENSe:FREQuency:MODE SWEEp|LIST|FIXEd
            //查询指令语法 :SENSe:FREQuency:MODE?
            get
            {
                Exception ex = new Exception();
                string Temp_FreqMode = base.Query(":SENS:FREQ:MODE?");
                if (Temp_FreqMode == "SWEEp" || Temp_FreqMode == "SWEE")
                { return ModeOfFrequency.SWEep; }
                else if (Temp_FreqMode == "LIST")
                { return ModeOfFrequency.LIST; }
                else if (Temp_FreqMode == "FIXEd" || Temp_FreqMode == "FIXE")
                { return ModeOfFrequency.FIXed; }
                else
                { throw ex; }
            }
            set
            {
                string strValue="";
                if (value == ModeOfFrequency.SWEep)
                { strValue = "SWEEp"; }
                else if (value == ModeOfFrequency.FIXed)
                { strValue = "FIXEd"; }
                else
                { strValue = value.ToString(); }
                base.Send(":SENSe:FREQuency:MODE " + strValue);
                WaitOpc();
            }
        }

        public override NoiseFigureAnalyzer.ModeOfAverage AverageMode
        {
            //设置指令语法 :SENSe:AVERage:MODE POINt|SWEep
            //查询指令语法 :SENSe:AVERage:MODE?
            get
            {
                Exception ex = new Exception();
                string Temp_AvgMode = base.Query(":SENS:AVER:MODE?");
                if (Temp_AvgMode == "POINt" || Temp_AvgMode == "POINT")
                { return ModeOfAverage.POINt; }
                else if (Temp_AvgMode == "SWEep"||(Temp_AvgMode == "SWEEP"))
                { return ModeOfAverage.SWEep; }
                else
                { throw ex; }
            }
            set
            {
                base.Send(":SENS:AVER:MODE " + value.ToString());
                WaitOpc();
            }
        }

        public override bool LossCompState_AfterDUT
        {
            //设置指令语法 :SENSe:CORRection:LOSS:AFTer:STATe OFF|ON|0|1
            //查询指令语法 :SENSe:CORRection:LOSS:AFTer:STATe?
            get
            {
                return base.Query(":SENS:CORR:LOSS:AFT:STAT?") == "1";
            }
            set
            {
                base.Send(":SENS:CORR:LOSS:AFT:STAT " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override bool LossCompState_BeforeDUT
        {
            //设置指令语法 :SENSe:CORRection:LOSS:BEFore:STATe OFF|ON|0|1
            //查询指令语法 :SENSe:CORRection:LOSS:BEFore:STATe?
            get
            {
                return base.Query(":SENS:CORR:LOSS:BEF:STAT?") == "1";
            }
            set
            {
                base.Send(":SENS:CORR:LOSS:BEF:STAT " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override NoiseFigureAnalyzer.ModeOfLossCompensation AfterDUTLossCompMode
        {
            //设置指令语法 :SENSe:CORRection:LOSS:AFTer:MODE FIXed|TABLe
            //查询指令语法 :SENSe:CORRection:LOSS:AFTer:MODE?
            get
            {
                Exception ex = new Exception("QueryFailed");
                string Temp_LossCompMode = "";
                Temp_LossCompMode = base.Query(":SENSe:CORRection:LOSS:AFTer:MODE?");
                if (Temp_LossCompMode == "FIXED")
                { return ModeOfLossCompensation.FIXed; }
                else if (Temp_LossCompMode == "TABLE")
                { return ModeOfLossCompensation.TABLe; }
                else
                { throw ex; }
            }
            set
            {
                base.Send(":SENSe:CORRection:LOSS:AFTer:MODE " + value.ToString());
                WaitOpc();
            }
        }

        public override NoiseFigureAnalyzer.ModeOfLossCompensation BeforeDUTLossCompMode
        {
            //设置指令语法 :SENSe:CORRection:LOSS:BEFore:MODE FIXed|TABLe
            //查询指令语法 :SENSe:CORRection:LOSS:BEFore:MODE?
            get
            {
                Exception ex = new Exception("QueryFailed");
                string Temp_LossCompMode = "";
                Temp_LossCompMode = base.Query(":SENSe:CORRection:LOSS:BEFore:MODE?");
                if (Temp_LossCompMode == "FIXED")
                { return ModeOfLossCompensation.FIXed; }
                else if (Temp_LossCompMode == "TABLE")
                { return ModeOfLossCompensation.TABLe; }
                else
                { throw ex; }
            }
            set
            {
                base.Send(":SENSe:CORRection:LOSS:BEFore:MODE " + value.ToString());
                WaitOpc();
            }
        }

        public override void Set_BeforeDUTLossCompData(double[] LossCompData)
        {
            //:SENSe:CORRection:LOSS:BEFore:TABLe:DATA <frequency>,<value>
            //向DUT 前损耗补偿表中输入频率-损耗数据组对，最大可输入201 组数据
            //此命令不能指定单位，频率以Hz 为单位，损耗以dB 为单位
            string Temp_LossCompData = string.Empty;
            for (int i = 0; i < LossCompData.Length; i++)
            {
                Temp_LossCompData = Temp_LossCompData + "," + LossCompData[i];
            }
            Temp_LossCompData = Temp_LossCompData.Remove(0, 1);
            base.Send(":SENSe:CORRection:LOSS:BEFore:TABLe:DATA " + Temp_LossCompData);
            WaitOpc();
        }

        public override void Set_AfterDUTLossCompData(double[] LossCompData)
        {
            //:SENSe:CORRection:LOSS:AFTer:TABLe:DATA <frequency>,<value>
            //向DUT 后损耗补偿表中输入频率-损耗数据组对，最大可输入201 组数据
            //此命令不能指定单位，频率以Hz 为单位，损耗以dB 为单位
            string Temp_LossCompData = string.Empty;
            for (int i = 0; i < LossCompData.Length; i++)
            {
                Temp_LossCompData = Temp_LossCompData + "," + LossCompData[i];
            }
            Temp_LossCompData = Temp_LossCompData.Remove(0, 1);
            base.Send(":SENSe:CORRection:LOSS:AFTer:TABLe:DATA " + Temp_LossCompData);
            WaitOpc();
        }

        public override double StartFreq
        {
            //设置指令语法 :SENSe:FREQuency:STARt <frequency>
            //查询指令语法 :SENSe:FREQuency:STARt?
            get
            {
                string Temp_Value = base.Query(":SENS:FREQ:STAR?");
                Temp_Value = Temp_Value.Remove(Temp_Value.Length - 2);
                return Convert.ToDouble(Temp_Value);
            }
            set
            {
                base.Send(":SENS:FREQ:STAR " + value.ToString());
                WaitOpc();
            }
        }

        public override double StopFreq
        {
            //设置指令语法 :SENSe:FREQuency:STOP <frequency>
            //查询指令语法 :SENSe:FREQuency:STOP?
            get
            {
                string Temp_Value = base.Query(":SENS:FREQ:STOP?");
                Temp_Value = Temp_Value.Remove(Temp_Value.Length - 2);
                return Convert.ToDouble(Temp_Value);
            }
            set
            {
                base.Send(":SENS:FREQ:STOP " + value.ToString());
                WaitOpc();
            }
        }

        public override int SweepPoint
        {
            //设置指令语法 :SENSe:SWEEp:POINts <number>
            //查询指令语法 :SENSe:SWEEp:POINts?
            get
            {
                string Temp_Value = base.Query(":SENS:SWEE:POIN?");
                Temp_Value = Temp_Value.Remove(Temp_Value.Length - 2);
                return Convert.ToInt32(Temp_Value);
            }
            set
            {
                base.Send(":SENS:SWEE:POIN " + value.ToString());
                WaitOpc();
            }
        }

        public override void Set_FreqList(double[] FreqList)
        {
            //:SENSe:FREQuency:LIST:DATA <frequency>
            //频率表中最大可输入401 个值，其中至少要指定2 个频率值
            string FreqListInString = string.Empty;
            for (int i = 0; i < FreqList.Length; i++)
            {
                FreqListInString = FreqListInString + "," + FreqList[i].ToString();
            }
            FreqListInString = FreqListInString.Remove(0, 1);
            base.Send(":SENS:FREQ:LIST:DATA " + FreqListInString);
            WaitOpc();
        }

        public override void Get_FreqList(out double[] FreqList)
        {
            //:SENSe:FREQuency:LIST:DATA?
            string[] Temp_FreqListInString = null;
            Temp_FreqListInString = base.Query(":SENSe:FREQuency:LIST:DATA?").Split(',');
            Array.Resize(ref Temp_FreqListInString, Temp_FreqListInString.Length - 1);//去除最后的空元素
            FreqList = new double[Temp_FreqListInString.Length];
            for (int i = 0; i < Temp_FreqListInString.Length; i++)
            { FreqList[i] = Convert.ToDouble(Temp_FreqListInString[i]); }
        }

        public override bool AverageState
        {
            //设置指令语法 :SENSe:AVERage:STATe OFF|ON|0|1
            //查询指令语法 :SENSe:AVERage:STATe?
            get
            {
                return base.Query(":SENSe:AVERage:STATe?") == "1";
            }
            set
            {
                base.Send(":SENSe:AVERage:STATe " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override uint AverageNumber
        {
            //设置指令语法 :SENSe:AVERage:COUNt <integer>
            //查询指令语法 :SENSe:AVERage:COUNt?
            get
            {
                string Temp_Value = base.Query(":SENS:AVER:COUN?");
                Temp_Value = Temp_Value.Remove(Temp_Value.Length - 2);
                return Convert.ToUInt32(Temp_Value);
            }
            set
            {
                base.Send(":SENS:AVER:COUN " + value.ToString());
                WaitOpc();
            }
        }

        public override bool AutoLoadENRState
        {
            //设置指令语法 :SENSe:CORRection:ENR:AUTO:STATe OFF|ON|0|1
            //查询指令语法 :SENSe:CORRection:ENR:AUTO:STATe?
            get
            {

                string str = base.Query(":SENS:CORR:ENR:AUTO:STAT?");
                return (str == "ON") || (str == "O");

                //return base.Query(":SENS:CORR:ENR:AUTO:STAT?") == "ON";
                
            }
            set
            {
                base.Send(":SENS:CORR:ENR:AUTO:STAT " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override bool CommonENRTableState
        {
            //设置指令语法 :SENSe:CORRection:ENR:COMMon:STASTe OFF|ON|0|1
            //查询指令语法 :SENSe:CORRection:ENR:COMMon:STASTe?
            get
            {
                return base.Query(":SENS:CORR:ENR:COMM:STAST?") == "1";
            }
            set
            {
                base.Send(":SENS:CORR:ENR:COMM:STAST " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override string CalENRTable
        {
            get
            {
                //:SENSe:CORRection:ENR:CALibration:TABLe:DATA?
                string Temp_ENRTable = string.Empty;
                Temp_ENRTable = base.Query(":SENS:CORR:ENR:CAL:TABL:DATA?");
                return Temp_ENRTable;
            }
            set
            {
                //:SENSe:CORRection:ENR:CALibration:TABLe:DATA <frequency>,<value>
                base.Send(":SENS:CORR:ENR:CAL:TABL:DATA " + value);
                WaitOpc();
            }
        }

        public override string MeasENRTable
        {
            get
            {
                //:SENSe:CORRection:ENR:MEASurement:TABLe:DATA?
                string Temp_ENRTable = string.Empty;
                Temp_ENRTable = base.Query(":SENS:CORR:ENR:MEAS:TABL:DATA?");
                return Temp_ENRTable;
            }
            set
            {
                //:SENSe:CORRection:ENR:MEASurement:TABLe:DATA <frequency>,<value>
                base.Send(":SENS:CORR:ENR:MEAS:TABL:DATA " + value);
                WaitOpc();
            }
        }



        public override string BeforeDUTLossCompTable
        {
            get
            {                
                string Temp_BeforeDUTLossCompTable = string.Empty;
                Temp_BeforeDUTLossCompTable = base.Query(":SENSe:CORRection:LOSS:BEFore:TABLe:DATA?");
                return Temp_BeforeDUTLossCompTable;
            }
            set
            {   //:SENSe:CORRection:LOSS:BEFore:TABLe:DATA <frequency>,<value> 
                base.Send(":SENSe:CORRection:LOSS:BEFore:TABLe:DATA " + value);
                WaitOpc();
                              }
        }

        public override string AfterDUTLossCompTable
        {
            get
            {
                string Temp_AfterDUTLossCompTable = string.Empty;
                Temp_AfterDUTLossCompTable = base.Query(":SENSe:CORRection:LOSS:AFTer:TABLe:DATA?");
                return Temp_AfterDUTLossCompTable;
            }
            set
            {   //:SENSe:CORRection:LOSS:AFTer:TABLe:DATA <frequency>,<value>
                base.Send(":SENSe:CORRection:LOSS:AFTer:TABLe:DATA " + value);
                WaitOpc();
            }
        }
        



        /// <summary>
        /// 从智能噪声源中读取ENR表并写入校准ENR表，注意*OPC?对该语句无效
        /// </summary>
        public override void LoadCalENRTableFromSNS()
        {
            //:SENSe:CORRection:ENR:CALibration:TABLe:SNS
            base.Send(":SENS:CORR:ENR:CAL:TABL:SNS");
            WaitOpc();
        }

        public override void LoadMeasENRTableFromSNS()
        {
            //:SENSe:CORRection:ENR:MEASurement:TABLe:SNS
            base.Send(":SENS:CORR:ENR:MEAS:TABL:SNS");
            WaitOpc();
        }

        public override bool CorrectedDataDisplayState
        {
            //Enables or disables the display of corrected data.
            //设置指令语法 :DISPlay:DATA:CORRections OFF|ON|0|1
            //查询指令语法 :DISPlay:DATA:CORRection?
            get
            {
                return base.Query("DISP:DATA:CORR?") == "1";
            }
            set
            {
                base.Send(":DISP:DATA:CORR " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override bool ContinuousMeasState
        {
            //设置指令语法 :INITiate:CONTinuous OFF|ON|0|1|
            //查询指令语法 :INITiate:CONTinuous?
            get
            {
                return base.Query(":INITiate:CONTinuous?") == "1";
            }
            set
            {
                base.Send(":INITiate:CONTinuous " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override void InitiateCalibration()
        {
            //:CALibration:AUTO:CALIbration OFF|ON|0|1
            base.Send(":CALibration:AUTO:CALIbration 1");
            WaitOpc();
            MessageBox.Show("噪声系数分析仪正在执行校准。\n请在仪表校准完成之后点击确定按钮。", "噪声系数分析仪自校准");
        }

        public override void DeleteFile(string FilePathAndName)
        {
            //AV3985A无此指令
        }

        public override void StoreStateFile(string FilePathAndName)
        {
            //:MMEMory:STORe:STATe <file_name>
            //！注意：文件不得有路径，也不需要有后缀名。
            //！注意：无论有没有后缀名，仪表都会在【输入的字符串】后面自动【再】添加后缀名.sta
            FilePathAndName = FilePathAndName.Remove(0, 2);
            FilePathAndName = FilePathAndName.Remove(FilePathAndName.Length - 4);
            base.Send(":MMEM:STOR:STAT " + FilePathAndName);
            WaitOpc();
        }

        public override void LoadStateFile(string FilePathAndName)
        {
            //:MMEMory:LOAD:STATe <file_name>
            //！注意：文件不得有路径，但必须有后缀名
            //举例 :MMEM:LOAD:STATe AV3984a.sta
            FilePathAndName = FilePathAndName.Remove(0, 2);
            base.Send(":MMEMory:LOAD:STATe " + FilePathAndName);
            WaitOpc();
            System.Threading.Thread.Sleep(1000);

            //读取仪表状态后，频率等参数没有刷新，保持读取前的状态，会造成测试错误。以下代码通过刷新频率模式来刷新参数。
            ModeOfFrequency Temp_PresentFreqMode;
            Temp_PresentFreqMode = this.FreqMode;
            if (Temp_PresentFreqMode == ModeOfFrequency.FIXed)
            {
                this.FreqMode = ModeOfFrequency.SWEep;
                System.Threading.Thread.Sleep(1000);
                this.FreqMode = ModeOfFrequency.FIXed;
            }
            else if (Temp_PresentFreqMode == ModeOfFrequency.LIST)
            {
                this.FreqMode = ModeOfFrequency.SWEep;
                System.Threading.Thread.Sleep(1000);
                this.FreqMode = ModeOfFrequency.LIST;
            }
            else if (Temp_PresentFreqMode == ModeOfFrequency.SWEep)
            {
                this.FreqMode = ModeOfFrequency.LIST;
                System.Threading.Thread.Sleep(1000);
                this.FreqMode = ModeOfFrequency.SWEep;
            }
        }

        public override void StoreBeforeDUTLossCompData(string FilePathAndName)
        {
            //语法：MMEMory:STORe:LOSS BEFore|AFTer,<filename>
            //示例：MMEM:STOR:LOSS BEF,’c:myloss.los’
            base.Send("MMEM:STOR:LOSS BEF,\'" + FilePathAndName + "\'");
            WaitOpc();
        }

        public override void StoreAfterDUTLossCompData(string FilePathAndName)
        {
            //语法：MMEMory:STORe:LOSS BEFore|AFTer,<filename>
            //示例：MMEM:STOR:LOSS BEF,’c:myloss.los’
            base.Send("MMEM:STOR:LOSS AFT,\'" + FilePathAndName + "\'");
            WaitOpc();
        }

        public override void LoadBeforeDUTLossCompData(string FilePathAndName)
        {
            //语法：MMEMory:LOAD:LOSS BEFore|AFTer,<file_name>
            //示例：MMEM:LOAD:LOSS AFT,’a:myloss.los’
            base.Send("MMEM:LOAD:LOSS BEF,\'" + FilePathAndName + "\'");
            WaitOpc();
        }

        public override void LoadAfterDUTLossCompData(string FilePathAndName)
        {
            //语法：MMEMory:LOAD:LOSS BEFore|AFTer,<file_name>
            //示例：MMEM:LOAD:LOSS AFT,’a:myloss.los’
            base.Send("MMEM:LOAD:LOSS AFT,\'" + FilePathAndName + "\'");
            WaitOpc();
        }

        public override void SaveScreenImage(NoiseFigureAnalyzer.TypeOfScreenImage ScrImgType, string FilePathAndName)
        {
            //指令示例 MMEM:STOR:SCR REV,’c:myscreen.wmf’
            base.Send("MMEM:STOR:SCR " + ScrImgType.ToString() + ",'" + FilePathAndName + "'");
            WaitOpc();
        }

        public override void Measure()
        {
            //刷新补偿功能状态
            //this.LossCompState_BeforeDUT = false;
            //System.Threading.Thread.Sleep(100);
            //this.LossCompState_AfterDUT = false;
            //System.Threading.Thread.Sleep(100);
            //this.LossCompState_BeforeDUT = true;
            //System.Threading.Thread.Sleep(100);
            //this.LossCompState_AfterDUT = true;
            //System.Threading.Thread.Sleep(100);

            string Temp_BeforeDUTLossCompTable = this.BeforeDUTLossCompTable;
            string Temp_AfterDUTLossCompTable = this.AfterDUTLossCompTable;

            ModeOfLossCompensation Temp_PresentCompMode;
            ////刷新DUT前级补偿模式
            //Temp_PresentCompMode = this.BeforeDUTLossCompMode;
            //if (Temp_PresentCompMode == ModeOfLossCompensation.FIXed)
            //{
            //    this.BeforeDUTLossCompMode = ModeOfLossCompensation.TABLe;
            //    this.WaitOpc(2000);
            //    this.BeforeDUTLossCompMode = ModeOfLossCompensation.FIXed;
            //}
            //else if (Temp_PresentCompMode == ModeOfLossCompensation.TABLe)
            //{
            //    this.BeforeDUTLossCompMode = ModeOfLossCompensation.FIXed;
            //    this.WaitOpc(2000);
            //    this.BeforeDUTLossCompMode = ModeOfLossCompensation.TABLe;
            //}

            ////刷新DUT后级补偿模式
            //Temp_PresentCompMode = this.AfterDUTLossCompMode;
            //if (Temp_PresentCompMode == ModeOfLossCompensation.FIXed)
            //{
            //    this.AfterDUTLossCompMode = ModeOfLossCompensation.TABLe;
            //    this.WaitOpc(2000);
            //    this.AfterDUTLossCompMode = ModeOfLossCompensation.FIXed;
            //}
            //else if (Temp_PresentCompMode == ModeOfLossCompensation.TABLe)
            //{
            //    this.AfterDUTLossCompMode = ModeOfLossCompensation.FIXed;
            //    this.WaitOpc(2000);
            //    this.AfterDUTLossCompMode = ModeOfLossCompensation.TABLe;
            //}

            
            ////uint Temp_AverageNum = this.AverageNumber;
            ////this.AverageNumber = 1;
            //base.Send(":SENSe:RENEw:SWEEp");
            //System.Threading.Thread.Sleep(20000);




            System.Threading.Thread.Sleep(2000);
            //刷新DUT补偿表
            this.BeforeDUTLossCompTable = Temp_BeforeDUTLossCompTable;
            //System.Threading.Thread.Sleep(10000);
            //this.AfterDUTLossCompTable = Temp_AfterDUTLossCompTable;

            System.Threading.Thread.Sleep(10000);


            //this.AverageNumber = Temp_AverageNum;
            

            //使用重新扫描命令作测量命令
            //:SENSe:RENEw:SWEEp
            base.Send(":SENSe:RENEw:SWEEp");
            WaitOpc();
            MessageBox.Show("测试中………………\n\n请观察噪声表AV3985测试扫描，确认测试完毕后，点击“确定”！！！", "噪声系数分析仪测量");

            ////刷新DUT补偿表
            //this.BeforeDUTLossCompTable = Temp_BeforeDUTLossCompTable;
            //this.WaitOpc(20000);
            //this.AfterDUTLossCompTable = Temp_AfterDUTLossCompTable;
            //this.WaitOpc(20000);



        }

        public override void Get_SweptCorrectedNF(out double[] NF_List)
        {
            //:FETCH:CORRected:NFIGure? DB|LINear
            string[] Temp_NFListInString = null;
            Temp_NFListInString = base.Query(":FETCH:CORRected:NFIGure?").Split(',');
            //Temp_NFListInString[Temp_NFListInString.Length - 1] = Temp_NFListInString[Temp_NFListInString.Length - 1].Remove(Temp_NFListInString[Temp_NFListInString.Length - 1].Length - 1);//去除最后的“”字符
            NF_List = new double[Temp_NFListInString.Length];
            for (int i = 0; i < Temp_NFListInString.Length; i++)
            { NF_List[i] = Convert.ToDouble(Temp_NFListInString[i]); }
        }

        public override void BW(double IFBW)
        {
            string temp = "4MHz";    //默认值给4MHz
            if (IFBW == 4000000) temp = "4MHz";
            if (IFBW == 2000000) temp = "2MHz";
            if (IFBW == 1000000) temp = "1MHz";
            if (IFBW == 400000) temp = "400kHz";
            if (IFBW == 200000) temp = "200kHz";
            if (IFBW == 100000) temp = "100kHz";
            base.Send(":SENSe:BANDwidth " + temp);
            WaitOpc();
        }

        /// <summary>
        /// Before DUT 的热力学温度（开氏温度）
        /// </summary>
        public override double BeforeDUTTemperature
        {
            get
            {
                //:SENSe:CORRection:TEMPerature:BEFore?
                return base.QueryNumber(":SENSe:CORRection:TEMPerature:BEFore?");
            }
            set
            {
                //:SENSe:CORRection:TEMPerature:BEFore <temperature>
                base.Send(":SENSe:CORRection:TEMPerature:BEFore " + value.ToString()+"K");
                WaitOpc();
            }
        }

        /// <summary>
        /// After DUT 的热力学温度（开氏温度）
        /// </summary>
        public override double AfterDUTTemperature
        {
            get
            {
                //:SENSe:CORRection:TEMPerature:AFTer?
                return base.QueryNumber(":SENSe:CORRection:TEMPerature:AFTer?");
            }
            set
            {
                //:SENSe:CORRection:TEMPerature:AFTer <temperature>
                base.Send(":SENSe:CORRection:TEMPerature:AFTer " + value.ToString()+"K");
                WaitOpc();
            }
        }

    }
}
