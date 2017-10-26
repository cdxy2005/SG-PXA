using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RackSys.TestLab.Instrument
{
    public class AgilentN8975A: NoiseFigureAnalyzer
    {

        public AgilentN8975A(string inAddress): base(inAddress)
        {
        }

        public override void Reset()
        {
            base.Send("*RST");
            WaitOpc();
        }

        public override void Preset()
        {
            base.Send("SYSTem:PRES");
            WaitOpc();
        }

        protected override void DetermineOptions()
        {
            //base.DetermineOptions();
        }

        /// <summary>
        /// 显示格式
        /// </summary>
        public override NoiseFigureAnalyzer.FormatOfDisplay DisplayFormat
        {
            get
            {
                Exception ex=new Exception();
                string Temp_DispForm= base.Query("DISPlay:FORMat?");
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
                base.Send("DISPlay:FORMat " + value.ToString());
                WaitOpc();
            }
        }

        public override bool FullScreenState
        {
            //设置指令语法 DISPlay:FULLscreen[:STATe] OFF|ON|0|1
            //查询指令语法 DISPlay:FULLscreen[:STATe]?
            get
            {
                return base.Query("DISP:FULL?") == "1";
            }
            set
            {
                base.Send("DISP:FULL " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override NoiseFigureAnalyzer.StateOfWindowZoom WinZoomState
        {
            get
            {
                //设置指令语法 DISPlay:ZOOM:WINDow?
                Exception ex = new Exception();
                string Temp_WinZoomState = base.Query("DISP:ZOOM:WIND?");
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
                //查询指令语法 DISPlay:ZOOM:WINDow OFF|UPPer|LOWer
                base.Send("DISP:ZOOM:WIND " + value.ToString());
                WaitOpc();
            }
        }

        public override void SetYScalePerDiv(NoiseFigureAnalyzer.TypeOfResults ResultsType, double ScalePerDiv)
        {
            //DISPlay:TRACe:Y[:SCALe]:PDIVision <result>,<value>
            base.Send("DISP:TRAC:Y:PDIV " + ResultsType.ToString() + "," + ScalePerDiv.ToString());
            WaitOpc();
        }

        public override void SetGraphUpperLimit(NoiseFigureAnalyzer.TypeOfTrace TraceType, double UpperLimit)
        {
            //DISPlay:TRACe:Y[:SCALe]:UPPer <trace>,<value>
            base.Send("DISP:TRAC:Y:UPP " + TraceType.ToString() + "," + UpperLimit.ToString());
            WaitOpc();
        }

        public override void SetGraphLowerLimit(NoiseFigureAnalyzer.TypeOfTrace TraceType, double LowerLimit)
        {
            //DISPlay:TRACe:Y[:SCALe]:LOWer <trace>,<value>
            base.Send("DISP:TRAC:Y:LOW " + TraceType.ToString() + "," + LowerLimit.ToString());
            WaitOpc();
        }

        public override void SetReferenceLevelValue(NoiseFigureAnalyzer.TypeOfResults ResultType, double RefLvlValue)
        {
            //DISPlay:TRACe:Y[:SCALe]:RLEVel:VALue <result>,<value>
            base.Send("DISP:TRAC:Y:RLEV:VAL " + ResultType.ToString() + "," + RefLvlValue.ToString());
            WaitOpc();
        }

        /// <summary>
        /// DUT类型
        /// </summary>
        public override NoiseFigureAnalyzer.TypeOfDUT DUTType
        {
            get
            {
                //[:SENSe]:CONFigure:MODE:DUT?
                Exception ex = new Exception();
                string Temp_DUTMode = base.Query(":CONFigure:MODE:DUT?");
                if (Temp_DUTMode == "AMPLifier")
                { return TypeOfDUT.AMPLifier; }
                else if (Temp_DUTMode == "DOWNconv")
                { return TypeOfDUT.DOWNconv; }
                else if (Temp_DUTMode == "UPConv")
                { return TypeOfDUT.UPConv; }
                else
                { throw ex; }
            }
            set
            {
                //[:SENSe]:CONFigure:MODE:DUT AMPLifier|DOWNconv|UPConv
                base.Send(":CONFigure:MODE:DUT " + value.ToString());
                WaitOpc();
            }
        }

        public override NoiseFigureAnalyzer.TypeOfNoiseSource NoiseSource
        {
            get
            {
                //SOURce:NOISe[:PREFerence]?
                Exception ex = new Exception();
                string Temp_NoiseSource = base.Query("SOUR:NOIS?");
                if (Temp_NoiseSource == "NORMal")
                { return TypeOfNoiseSource.NORMal; }
                else if (Temp_NoiseSource == "SNS")
                { return TypeOfNoiseSource.SNS; }
                else
                { throw ex; }
            }
            set
            {
                //SOURce:NOISe[:PREFerence] NORMal|SNS
                base.Send("SOUR:NOIS " + value.ToString());
                WaitOpc();
            }
        }

        public override NoiseFigureAnalyzer.ModeOfFrequency FreqMode
        {
            get
            {
                //[:SENSe]:FREQuency:MODE?
                Exception ex = new Exception();
                string Temp_FreqMode = base.Query(":FREQuency:MODE?");
                if (Temp_FreqMode == "SWEep")
                { return ModeOfFrequency.SWEep; }
                else if (Temp_FreqMode == "FIXed")
                { return ModeOfFrequency.FIXed; }
                else if (Temp_FreqMode == "LIST")
                { return ModeOfFrequency.LIST; }
                else
                { throw ex; }
            }
            set
            {
                base.Send("SENSe:FREQuency:MODE " + value.ToString());
                WaitOpc();
            }
        }

        public override NoiseFigureAnalyzer.ModeOfAverage AverageMode
        {
            get
            {
                //[:SENSe]:AVERage:MODE?
                Exception ex = new Exception();
                string Temp_AvgMode = base.Query(":AVERage:MODE?");
                if (Temp_AvgMode == "POINt")
                { return ModeOfAverage.POINt; }
                else if (Temp_AvgMode == "SWEep")
                { return ModeOfAverage.SWEep; }
                else
                { throw ex; }
            }
            set
            {
                //[:SENSe]:AVERage:MODE POINt|SWEep
                base.Send(":AVERage:MODE " + value.ToString());
                WaitOpc();
            }
        }

        public override bool LossCompState_AfterDUT
        {
            get
            {
                return base.Query("CORR:LOSS:AFT:STAT?") == "1";
            }
            set
            {
                base.Send("CORR:LOSS:AFT:STAT " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override bool LossCompState_BeforeDUT
        {
            get
            {
                return base.Query("CORR:LOSS:BEF:STAT?") == "1";
            }
            set
            {
                base.Send("CORR:LOSS:BEF:STAT " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override NoiseFigureAnalyzer.ModeOfLossCompensation AfterDUTLossCompMode
        {
            get
            {
                Exception ex = new Exception("QueryFailed");
                string Temp_LossCompMode = "";
                Temp_LossCompMode = base.Query(":CORRection:LOSS:AFTer:MODE?");
                if (Temp_LossCompMode == "FIXed")
                { return ModeOfLossCompensation.FIXed; }
                else if (Temp_LossCompMode == "TABLe")
                { return ModeOfLossCompensation.TABLe; }
                else
                { throw ex; }
            }
            set
            {
                base.Send(":CORRection:LOSS:AFTer:MODE " + value.ToString());
                WaitOpc();
            }
        }

        public override NoiseFigureAnalyzer.ModeOfLossCompensation BeforeDUTLossCompMode
        {
            get
            {
                Exception ex = new Exception("QueryFailed");
                string Temp_LossCompMode = "";
                Temp_LossCompMode = base.Query(":CORRection:LOSS:BEFore:MODE?");
                if (Temp_LossCompMode == "FIXed")
                { return ModeOfLossCompensation.FIXed; }
                else if (Temp_LossCompMode == "TABLe")
                { return ModeOfLossCompensation.TABLe; }
                else
                { throw ex; }
            }
            set
            {
                base.Send(":CORRection:LOSS:BEFore:MODE " + value.ToString());
                WaitOpc();
            }
        }

        public override void Set_BeforeDUTLossCompData(double[] LossCompData)
        {
            string Temp_LossCompData = string.Empty;
            for (int i = 0; i < LossCompData.Length; i++)
            {
                Temp_LossCompData = Temp_LossCompData + "," + LossCompData[i];
            }
            Temp_LossCompData = Temp_LossCompData.Remove(0, 1);
            base.Send(":CORRection:LOSS:BEFore:TABLe:DATA " + Temp_LossCompData);
            WaitOpc();
        }

        public override void Set_AfterDUTLossCompData(double[] LossCompData)
        {
            string Temp_LossCompData = string.Empty;
            for (int i = 0; i < LossCompData.Length; i++)
            {
                Temp_LossCompData = Temp_LossCompData + "," + LossCompData[i];
            }
            Temp_LossCompData = Temp_LossCompData.Remove(0, 1);
            base.Send(":CORRection:LOSS:AFTer:TABLe:DATA " + Temp_LossCompData);
            WaitOpc();
        }

        public override double StartFreq
        {
            get
            {
                //[:SENSe]:FREQuency:STARt?
                string Temp_Value = base.Query(":FREQuency:STARt?");
                Temp_Value = Temp_Value.Remove(Temp_Value.Length - 2);
                return Convert.ToDouble(Temp_Value);
            }
            set
            {
                //[:SENSe]:FREQuency:STARt<frequency>|MINimum|MAXimum
                base.Send(":FREQuency:STARt " + value.ToString());
                WaitOpc();
            }
        }

        public override double StopFreq
        {
            get
            {
                //[:SENSe]:FREQuency:STOP?
                string Temp_Value = base.Query(":FREQuency:STOP?");
                Temp_Value = Temp_Value.Remove(Temp_Value.Length - 2);
                return Convert.ToDouble(Temp_Value);
            }
            set
            {
                //[:SENSe]:FREQuency:STOP<frequency>|MINimum|MAXimum
                base.Send(":FREQuency:STOP " + value.ToString());
                WaitOpc();
            }
        }

        public override int SweepPoint
        {
            get
            {
                //[:SENSe]:SWEep:POINts?
                string Temp_Value = base.Query(":SWEep:POINts?");
                Temp_Value = Temp_Value.Remove(Temp_Value.Length - 2);
                return Convert.ToInt32(Temp_Value);
            }
            set
            {
                //[:SENSe]:SWEep:POINts <number>
                base.Send(":SWEep:POINts " + value.ToString());
                WaitOpc();
            }
        }

        public override void Set_FreqList(double[] FreqList)
        {
            string FreqListInString = string.Empty;
            for (int i = 0; i < FreqList.Length; i++)
            {
                FreqListInString = FreqListInString + "," + FreqList[i].ToString();
            }
            FreqListInString = FreqListInString.Remove(0, 1);
            base.Send("FREQuency:LIST:DATA " + FreqListInString);
            WaitOpc();
        }

        public override void Get_FreqList(out double[] FreqList)
        {
            string[] Temp_FreqListInString = null;
            Temp_FreqListInString = base.Query("FREQuency:LIST:DATA?").Split(',');
            Temp_FreqListInString[Temp_FreqListInString.Length - 1] = Temp_FreqListInString[Temp_FreqListInString.Length - 1].Remove(Temp_FreqListInString[Temp_FreqListInString.Length - 1].Length - 1);//去除最后的“”字符
            FreqList = new double[Temp_FreqListInString.Length];
            for (int i = 0; i < Temp_FreqListInString.Length; i++)
            { FreqList[i] = Convert.ToDouble(Temp_FreqListInString[i]); }
        }

        public override bool AverageState
        {
            get
            {
                return base.Query(":AVERage?") == "1";
            }
            set
            {
                base.Send(":AVERage " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override uint AverageNumber
        {
            get
            {
                //[:SENSe]:AVERage:COUNt?
                string Temp_Value = base.Query(":AVERage:COUNt?");
                Temp_Value = Temp_Value.Remove(Temp_Value.Length - 2);
                return Convert.ToUInt32 (Temp_Value);
            }
            set
            {
                //[:SENSe]:AVERage:COUNt <integer>
                base.Send(":AVERage:COUNt " + value.ToString());
                WaitOpc();
            }
        }

        public override bool AutoLoadENRState
        {
            get
            {
                return base.Query("CORRection:ENR:AUTO?") == "1";
            }
            set
            {
                base.Send(":CORR:ENR:AUTO " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override bool CommonENRTableState
        {
            get
            {
                //[:SENSe]:CORRection:ENR:COMMon[:STATe]?
                return base.Query(":CORRection:ENR:COMMon?") == "1";
            }
            set
            {
                //[:SENSe]:CORRection:ENR:COMMon[:STATe] OFF|ON|0|1
                base.Send(":CORRection:ENR:COMMon " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override string CalENRTable
        {
            get
            {
                string Temp_ENRTable = string.Empty;
                Temp_ENRTable = base.Query(":CORRection:ENR:CALibration:TABLe:DATA?");
                Temp_ENRTable = Temp_ENRTable.Remove(Temp_ENRTable.Length - 1);
                return Temp_ENRTable;
            }
            set
            {
                base.Send(":CORRection:ENR:CALibration:TABLe:DATA " + value);
                WaitOpc();
            }
        }

        public override string MeasENRTable
        {
            get
            {
                string Temp_ENRTable = string.Empty;
                Temp_ENRTable = base.Query(":CORRection:ENR:TABLe:DATA?");
                Temp_ENRTable = Temp_ENRTable.Remove(Temp_ENRTable.Length - 1);
                return Temp_ENRTable;
            }
            set
            {
                base.Send(":CORRection:ENR:TABLe:DATA " + value);
                WaitOpc();
            }
        }

        public override string BeforeDUTLossCompTable
        {
            get
            {
                string Temp_BeforeDUTLossCompTable = string.Empty;
                Temp_BeforeDUTLossCompTable = base.Query(":CORRection:LOSS:BEFore:TABLe:DATA?");
                Temp_BeforeDUTLossCompTable = Temp_BeforeDUTLossCompTable.Remove(Temp_BeforeDUTLossCompTable.Length - 1);
                return Temp_BeforeDUTLossCompTable;
            }
            set
            {
                base.Send(":CORRection:LOSS:BEFore:TABLe:DATA " + value);
                WaitOpc();
            }
        }

        public override string AfterDUTLossCompTable
        {
            get
            {
                string Temp_AfterDUTLossCompTable = string.Empty;
                Temp_AfterDUTLossCompTable = base.Query(":CORRection:LOSS:AFTer:TABLe:DATA?");
                Temp_AfterDUTLossCompTable = Temp_AfterDUTLossCompTable.Remove(Temp_AfterDUTLossCompTable.Length - 1);
                return Temp_AfterDUTLossCompTable;
            }
            set
            {
                base.Send(":CORRection:LOSS:AFTer:TABLe:DATA " + value);
                WaitOpc();
            }
        }

        /// <summary>
        /// 从智能噪声源中读取ENR表并写入校准ENR表，该*OPC?对该语句无效
        /// </summary>
        public override void LoadCalENRTableFromSNS()
        {
            base.Send(":CORR:ENR:CAL:TABL:SNS");
            WaitOpc();
        }

        public override void LoadMeasENRTableFromSNS()
        {
            base.Send("CORRection:ENR:TABLe:SNS");
            WaitOpc();
        }

        public override bool CorrectedDataDisplayState
        {
            //Enables or disables the display of corrected data.
            get
            {
                return base.Query("DISP:DATA:CORR?") == "1";
            }
            set
            {
                base.Send("DISP:DATA:CORR " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override bool ContinuousMeasState
        {
            get
            {
                return base.Query("INIT:CONT?") == "1";
            }
            set
            {
                base.Send("INIT:CONT " + (value ? "1" : "0"));
                WaitOpc();
            }
        }

        public override void InitiateCalibration()
        {
            base.Send(":CORRection:COLLect:ACQuire STANdard");
            WaitOpc();
        }

        public override void DeleteFile(string FilePathAndName)
        {
            ///举例：
            ///base.Send("MMEM:DEL \'c:Test01.sta\'");
            base.Send("MMEM:DEL \'" + FilePathAndName + "\'");
            WaitOpc();
        }

        public override void StoreStateFile(string FilePathAndName)
        {
            ///举例：
            ///base.Send("MMEM:STOR:STAT 1, " + "\'c:Test01.sta\'");
            base.Send("MMEM:STOR:STAT 1, \'" + FilePathAndName + "\'");
            WaitOpc();
        }

        public override void LoadStateFile(string FilePathAndName)
        {
            base.Send("MMEMory:LOAD:STATe 1,\'" + FilePathAndName + "\'");
            WaitOpc();
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
            base.Send("INIT");
            WaitOpc();
        }

        public override void Get_SweptCorrectedNF(out double[] NF_List)
        {
            string[] Temp_NFListInString = null;
            Temp_NFListInString = base.Query("FETC:CORR:NFIG?").Split(',');
            Temp_NFListInString[Temp_NFListInString.Length - 1] = Temp_NFListInString[Temp_NFListInString.Length - 1].Remove(Temp_NFListInString[Temp_NFListInString.Length - 1].Length - 1);//去除最后的“”字符
            NF_List = new double[Temp_NFListInString.Length];
            for (int i = 0; i < Temp_NFListInString.Length;i++ )
            { NF_List[i] = Convert.ToDouble(Temp_NFListInString[i]); }
        }

        public override Image CaptureScreenImage()
        {
            base.Send("MMEM:DEL \'c:SNFA.gif\'");
            base.Query("*OPC?", 30000);
            base.Send("MMEM:STOR:SCR \'c:SNFA.gif\'");
            base.Query("*OPC?",30000);
            byte[] ImageDataInBytes = base.ReadBlock("MMEMory:DATA? 'c:SNFA.gif'");
            return ImageBytesConvertor.ConvertByteToImg(ImageDataInBytes);
        }


        public override void BW(double IFBW)
        {
            base.Send(":BAND " + IFBW.ToString());
            WaitOpc();
        }

        /// <summary>
        /// Before DUT 的热力学温度（开氏温度）
        /// </summary>
        public override double BeforeDUTTemperature
        {
            get
            {
                //[:SENSe]:CORRection:TEMPerature:BEFore?
                return base.QueryNumber(":CORR:TEMP:BEF?");
            }
            set
            {
                //[:SENSe]:CORRection:TEMPerature:BEFore <temperature>
                base.Send(":CORR:TEMP:BEF " + value.ToString());
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
                //[:SENSe]:CORRection:TEMPerature:AFTer?
                return base.QueryNumber(":CORR:TEMP:AFT?");
            }
            set
            {
                //[:SENSe]:CORRection:TEMPerature:AFTer <temperature>
                base.Send(":CORR:TEMP:AFT " + value.ToString());
                WaitOpc();
            }
        }
        public override void WaitOpc()
        {
            base.WaitOpc(20000);
        }
    }
}
