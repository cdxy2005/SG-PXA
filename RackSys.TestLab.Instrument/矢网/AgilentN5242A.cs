using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;


namespace RackSys.TestLab.Instrument
{
    public class AgilentPNA : NetworkAnalyzer
    {
        public override bool OutPutState
        {
            get
            {
                bool flag;
                flag = (Convert.ToInt32(base.Query("OUTP:STAT?")) != 1 ? false : true);
                return flag;
            }
            set
            {
                if (!value)
                {
                    base.Send("OUTP:STAT OFF");
                }
                else
                {
                    base.Send("OUTP:STAT ON");
                }
            }
        }

 
        public AgilentPNA(string inAddress)
            : base(inAddress)
        {
        }

        public override void AutoScale()
        {
            base.Send("DISP:WIND:TRAC:Y:AUTO");
        }

        public override void Average_OFF()
        {
            base.Send("SENS:AVER OFF");
        }

        public override bool ErrorMessageDisplay
        {
            get
            {
             double str=   this.QueryNumber("DISP:ANN:MESS:STAT?");
             if (str > 0)
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
                string str="1";
                if (value)
	{
        str = "1";
	}
                else
                {
                    str = "0";
                }
                this.Send(string.Format("DISP:ANN:MESS:STAT {0}", str));
            }

        }
        public override void Average_ON(int count)
        {
            base.SendOpc(string.Concat("SENS:AVER:COUN ", count.ToString()), 50);
            base.Send("SENS:AVER ON");
        }

        public override void CentFreq(double mhz)
        {
            base.Send(string.Concat("SENS:FREQ:CENT ", mhz.ToString(), "MHz"));
        }

        public override void Div_dB(double range)
        {
            base.Send(string.Concat("DISP:WIND:TRAC:Y:PDIV ", range.ToString()));
        }

        public override byte[] GetS2pFileData()
        {
            base.Send("MMEM:DEL 'D:\\DecenTest.S2P'");
            this.WaitOpc();
            base.Send("MMEM:STOR 'D:\\DecenTest.S2P'");
            this.WaitOpc();
            byte[] getS2pFile = this.ReadBlock("MMemory:Transfer? 'D:\\DecenTest.S2P'");
          
            return getS2pFile;
        }
        public override byte[] GetFileData(string fileName)
        {
            byte[] getS2pFile = this.ReadBlock(string.Format("MMemory:Transfer? {0}",fileName));
            return getS2pFile;
        }
        public override bool EcalAutoOrient
        {
            get
            {
               double dd= this.QueryNumber("SENS:CORR:PREF:ECAL:ORI?");
               if (dd > 0)
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
                if (value)
                {
                    this.SendSCPI("SENS:CORR:PREF:ECAL:ORI ON");
                }
                else
                {
                    this.SendSCPI("SENS:CORR:PREF:ECAL:ORI OFF");
                }
            }
        }
        public override double Div_dB()
        {
            return base.QueryNumber("DISP:WIND:TRAC:Y:PDIV?");
        }

        public override void CLearAllMeasure()
        {
            this.Send("CALC:PAR:DEL:ALL");
            this.WaitOpc();
        }
        //郝佳添加代码2014.2.27
        public override void CreatFile(string Filename)
        {
            base.Send(string.Concat("mmemory:mdirectory '" + Filename + "'"));
            //     base.Send(string.Concat("mmemory:mdirectory ", Filename);
        }
        //苏渊红2014.3.1添加代码
        public override string GetFileNameinPath(string FilePath)
        {
            base.Send(string.Concat("MMEMory:CDIRectory '" + FilePath + "'"));
            return base.Query(string.Concat("MMEM:CAT?"));

        }

        /// <summary>
        /// 迹线最大化 张学超
        /// </summary>
        /// <param name="m_TraceMax"></param>
        public override void TraceMax(bool m_TraceMax)
        {
            if (m_TraceMax)
            {
                this.Send("DISP:TMAX ON ");
                this.WaitOPC();
            }
            else
            {
                this.Send("DISP:TMAX OFF ");
                this.WaitOPC();
            }

        }
        public void PNASettingForPhase()
        {
            this.SendSCPI(string.Concat("calculate1:parameter:define:extended ", "ch1_a", "B/A,0"));
            this.SendSCPI(string.Concat("calculate1:parameter:define:extended ", "ch1_a", "B/A,0"));
            this.SendSCPI(string.Concat("calculate1:parameter:define:extended ", "ch1_a", "B/A,0"));
            this.SendSCPI(string.Concat("calculate1:parameter:define:extended ", "ch1_a", "B/A,0"));


        }

        public override void ECALS11(string MeasmentName, bool IsCalKit, int CalKitNum, int docNum, double rfPow, double startFreq, double stopFreq, int SweepPoint, double ifbw)
        {
            string empty = string.Empty;
            double num = 0;
            base.SendOpc("SYSTem:PRESet", 10);
            base.Send("CALC:PAR:DEL:ALL");
            base.Send(string.Concat("CALC:PAR:DEF '", MeasmentName, "',S11"));
            base.Send(string.Concat("DISP:WIND:TRACE:FEED '", MeasmentName, "'"));
            base.Send(string.Concat("CALC:PAR:SEL '", MeasmentName, "'"));
            if (base.Query("CALCulate:FORMat?").IndexOf("SWR") < 0)
            {
                base.Send("CALC:FORM SWR");
            }
            base.Send(string.Concat("SENS:FREQ:STAR ", startFreq.ToString(), " MHz"));
            base.Send(string.Concat("SENS:FREQ:STOP ", stopFreq.ToString(), " MHz"));
            base.Send(string.Concat("SOUR:POW ", rfPow.ToString()));
            Thread.Sleep(200);
            base.Send("OUTP ON");
            Thread.Sleep(200);
            base.Send(string.Concat("SENS:BWID ", ifbw.ToString(), " kHz"));
            Thread.Sleep(200);
            base.Send(string.Concat("SENS:SWE:POIN ", SweepPoint.ToString()));
            num = Convert.ToDouble(base.Query("SENSe:SWEep:TIME?"));
            base.SendOpc("DISP:WIND:TRAC:Y:AUTO", 20);
            Thread.Sleep(Convert.ToInt32(num * 1000));
            base.Send("CALC:SMO OFF");
            base.Send("SENS:AVER OFF");
            if (!IsCalKit)
            {
                base.SendOpc("SENS:CORR:COLL:METH REFL3", 1000);
                base.SendOpc("INITiate:CONTinuous OFF", 10);
                base.SendOpc(string.Concat("SENSe:CORRection:COLLect:CKIT:SELect ", CalKitNum.ToString()), 10);
                this.SendNAStatus(2, "机械校准S11_开路");
                base.SendOpc("SENSe:CORRection:COLLect:ACQuire STAN1", 100);
                this.SendNAStatus(2, "机械校准S11_短路");
                base.SendOpc("SENSe:CORRection:COLLect:ACQuire STAN2", 100);
                this.SendNAStatus(2, "机械校准S11_负载");
                base.SendOpc("SENSe:CORRection:COLLect:ACQuire STAN3", 100);
                Thread.Sleep(5000);
                base.SendOpc("SENSe:CORR:COLL:SAVE", 10);
                base.Send("DISP:WIND:SIZE MAX");
            }
            else
            {
                this.SendNAStatus(2, "电子校准S11");
                while (true)
                {
                    base.SendOpc(string.Concat("SENSe:CORRection:COLLect:CKIT:SELect ", CalKitNum.ToString()), 1000);
                    base.Send("SENSe:CORRection:COLLect:METHod REFL3");
                    base.SendOpc("SENSe:CORRection:COLLect:ACQuire ECALA", 10000);
                    if (base.Query("SYST:ERROR?").IndexOf("Could not configure the Electronic Calibration system. Check to see if the module is plugged into the proper connector") <= 0)
                    {
                        break;
                    }
                }
            }
            base.Send("OUTP:STAT OFF");
            base.Send("SOUR:POW -30");
            base.Send("MMEM:MDIR 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile'");
            if (base.Query("MMEM:CAT:CSAR? 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile'").IndexOf(string.Concat("DecenTest", docNum.ToString())) <= 0)
            {
                Thread.Sleep(2000);
                base.SendOpc(string.Concat("MMEM:STOR 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile\\DecenTest", docNum.ToString(), ".csa'"), 10000);
            }
            else
            {
                Thread.Sleep(1000);
                base.SendOpc(string.Concat("MMEM:DEL 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile\\DecenTest", docNum.ToString(), ".csa'"), 10000);
                Thread.Sleep(3000);
                base.SendOpc(string.Concat("MMEM:STOR 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile\\DecenTest", docNum.ToString(), ".csa'"), 10000);
            }
        }

        public override void ECALS11(string MeasmentName, bool IsCalKit, int CalKitNum, string CalKitType, int docNum, double rfPow, double startFreq, double stopFreq, int SweepPoint, double ifbw)
        {
            string empty = string.Empty;
            double num = 0;
            base.SendOpc("SYSTem:PRESet", 10);
            base.Send("CALC:PAR:DEL:ALL");
            base.Send(string.Concat("CALC:PAR:DEF '", MeasmentName, "',S11"));
            base.Send(string.Concat("DISP:WIND:TRACE:FEED '", MeasmentName, "'"));
            base.Send(string.Concat("CALC:PAR:SEL '", MeasmentName, "'"));
            if (base.Query("CALCulate:FORMat?").IndexOf("SWR") < 0)
            {
                base.Send("CALC:FORM SWR");
            }
            base.Send(string.Concat("SENS:FREQ:STAR ", startFreq.ToString(), " MHz"));
            base.Send(string.Concat("SENS:FREQ:STOP ", stopFreq.ToString(), " MHz"));
            base.Send(string.Concat("SOUR:POW ", rfPow.ToString()));
            Thread.Sleep(200);
            base.Send("OUTP ON");
            Thread.Sleep(200);
            base.Send(string.Concat("SENS:BWID ", ifbw.ToString(), " kHz"));
            Thread.Sleep(200);
            base.Send(string.Concat("SENS:SWE:POIN ", SweepPoint.ToString()));
            num = Convert.ToDouble(base.Query("SENSe:SWEep:TIME?"));
            base.SendOpc("DISP:WIND:TRAC:Y:AUTO", 20);
            Thread.Sleep(Convert.ToInt32(num * 1000));
            base.Send("CALC:SMO OFF");
            base.Send("SENS:AVER OFF");
            if (!IsCalKit)
            {
                base.SendOpc("SENS:CORR:COLL:METH REFL3", 1000);
                base.SendOpc(string.Concat("SENSe:CORRection:COLLect:CKIT:SELect ", CalKitNum.ToString()), 10);
                string str = base.Query("SENSe:CORRection:COLLect:CKIT:PORT:SELECT?");
                this.SendNAStatus(2, "机械校准S11_开路");
                if (str.IndexOf("85054D") >= 0)
                {
                    if (!(CalKitType == "Female"))
                    {
                        base.Send("SENS:CORR:COLL:GUID:CONN:PORT1 'Type N (50) male'");
                    }
                    else
                    {
                        base.Send("SENS:CORR:COLL:GUID:CONN:PORT1 'Type N (50) female'");
                    }
                }
                else if (str.IndexOf("85052D") >= 0)
                {
                    if (!(CalKitType == "Female"))
                    {
                        base.Send("SENS:CORR:COLL:GUID:CONN:PORT1 'APC 3.5 male'");
                    }
                    else
                    {
                        base.Send("SENS:CORR:COLL:GUID:CONN:PORT1 'APC 3.5 female'");
                    }
                }
                else if (str.IndexOf("85056K01") >= 0)
                {
                    if (!(CalKitType == "Female"))
                    {
                        base.Send("SENS:CORR:COLL:GUID:CONN:PORT1 'APC 2.4 male'");
                    }
                    else
                    {
                        base.Send("SENS:CORR:COLL:GUID:CONN:PORT1 'APC 2.4 female'");
                    }
                }
                else if (str.IndexOf("85056K02") >= 0)
                {
                    if (!(CalKitType == "Female"))
                    {
                        base.Send("SENS:CORR:COLL:GUID:CONN:PORT1 '2.92 mm male'");
                    }
                    else
                    {
                        base.Send("SENS:CORR:COLL:GUID:CONN:PORT1 '2.92 mm female'");
                    }
                }
                base.SendOpc("SENSe:CORRection:COLLect:ACQuire STAN1", 100);
                this.SendNAStatus(2, "机械校准S11_短路");
                base.SendOpc("SENSe:CORRection:COLLect:ACQuire STAN2", 100);
                this.SendNAStatus(2, "机械校准S11_负载");
                base.SendOpc("SENSe:CORRection:COLLect:ACQuire STAN3", 100);
                base.SendOpc("SENSe:CORR:COLL:SAVE", 10);
                base.Send("DISP:WIND:SIZE MAX");
            }
            else
            {
                this.SendNAStatus(2, "电子校准S11");
                while (true)
                {
                    base.SendOpc(string.Concat("SENSe:CORRection:COLLect:CKIT:SELect ", CalKitNum.ToString()), 1000);
                    base.Send("SENSe:CORRection:COLLect:METHod REFL3");
                    base.SendOpc("SENSe:CORRection:COLLect:ACQuire ECALA", 10000);
                    if (base.Query("SYST:ERROR?").IndexOf("Could not configure the Electronic Calibration system. Check to see if the module is plugged into the proper connector") <= 0)
                    {
                        break;
                    }
                }
            }
            base.Send("OUTP:STAT OFF");
            base.Send("SOUR:POW -30");
            base.Send("MMEM:MDIR 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile'");
            if (base.Query("MMEM:CAT:CSAR? 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile'").IndexOf(string.Concat("DecenTest", docNum.ToString())) <= 0)
            {
                Thread.Sleep(2000);
                base.SendOpc(string.Concat("MMEM:STOR 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile\\DecenTest", docNum.ToString(), ".csa'"), 10000);
            }
            else
            {
                Thread.Sleep(1000);
                base.SendOpc(string.Concat("MMEM:DEL 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile\\DecenTest", docNum.ToString(), ".csa'"), 10000);
                Thread.Sleep(3000);
                base.SendOpc(string.Concat("MMEM:STOR 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile\\DecenTest", docNum.ToString(), ".csa'"), 10000);
            }
        }

        public override void ECALS21(string MeasmentName, bool IsCalKit, int CalKitNum, int docNum, double rfPow, double startFreq, double stopFreq, int SweepPoint, double ifbw)
        {
            string empty = string.Empty;
            double num = 0;
            base.Send("*RST");
            base.Send("CALC:PAR:DEL:ALL");
            base.Send(string.Concat("CALC:PAR:DEF '", MeasmentName, "',S21"));
            base.Send(string.Concat("DISP:WIND:TRACE:FEED '", MeasmentName, "'"));
            base.Send(string.Concat("CALC:PAR:SEL '", MeasmentName, "'"));
            if (base.Query("CALCulate:FORMat?").IndexOf("MLOG") < 0)
            {
                base.Send("CALC:FORM MLOG");
            }
            base.Send(string.Concat("SENS:FREQ:STAR ", startFreq.ToString(), " MHz"));
            base.Send(string.Concat("SENS:FREQ:STOP ", stopFreq.ToString(), " MHz"));
            base.Send(string.Concat("SOUR:POW ", rfPow.ToString()));
            Thread.Sleep(200);
            base.Send("OUTP ON");
            Thread.Sleep(200);
            base.Send(string.Concat("SENS:BWID ", ifbw.ToString(), " kHz"));
            Thread.Sleep(200);
            base.Send(string.Concat("SENS:SWE:POIN ", SweepPoint.ToString()));
            num = Convert.ToDouble(base.Query("SENSe:SWEep:TIME?"));
            base.Send("DISP:WIND:TRAC:Y:AUTO");
            Thread.Sleep(Convert.ToInt32(num * 1000));
            base.Send("CALC:SMO ON");
            base.Send("SENS:AVER ON");
            if (!IsCalKit)
            {
                base.SendOpc("SENS:CORR:COLL:METH TRAN1", 1000);
                base.SendOpc(string.Concat(":SENS1:CORR:COLL:CKIT ", CalKitNum.ToString()), 1000);
                base.SendOpc(string.Concat("SENSe:CORRection:COLLect:CKIT:SELect ", CalKitNum.ToString()), 10);
                this.SendNAStatus(2, "机械校准S21");
                base.SendOpc("SENSe:CORRection:COLLect:ACQuire STAN4", 10);
                Thread.Sleep(5000);
                base.Send("SENS:CORR:COLL:SAVE");
                base.Send("DISP:WIND:SIZE MAX");
            }
            else
            {
                this.SendNAStatus(2, "电子校准S21");
                base.Send("SENSe:CORRection:COLLect:CKIT:STANdard:TYPE THRU");
                base.Send("SENSe:CORRection:COLLect:METHod REFL3");
                base.SendOpc(string.Concat("SENSe:CORRection:COLLect:CKIT:SELect ", CalKitNum.ToString()), 1000);
                base.SendOpc("SENSe:CORRection:COLLect:METHod TRAN1", 1000);
                base.SendOpc("SENSe:CORRection:COLLect:ACQuire STAN4", 1000);
                base.Send("SENS:CORR:COLL:SAVE");
                base.Send("DISP:WIND:SIZE MAX");
            }
            base.Send("OUTP:STAT OFF");
            base.Send("SOUR:POW -30");
            base.Send("MMEM:MDIR 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile'");
            if (base.Query("MMEM:CAT:CSAR? 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile'").IndexOf(string.Concat("DecenTest", docNum.ToString())) <= 0)
            {
                Thread.Sleep(2000);
                base.SendOpc(string.Concat("MMEM:STOR 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile\\DecenTest", docNum.ToString(), ".csa'"), 10000);
            }
            else
            {
                Thread.Sleep(1000);
                base.SendOpc(string.Concat("MMEM:DEL 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile\\DecenTest", docNum.ToString(), ".csa'"), 10000);
                Thread.Sleep(3000);
                base.SendOpc(string.Concat("MMEM:STOR 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile\\DecenTest", docNum.ToString(), ".csa'"), 10000);
            }
        }

        public override string GetTrace()
        {
            return base.Query("CALC:DATA? FDATA");
        }

        public override void IFBW(double khz)
        {
            base.Send(string.Concat("SENS:BWID ", khz.ToString(), "kHz"));
        }

        public override void IniTestSta(NetworkAnalyzer.MeasMode mode, NetworkAnalyzer.Format format)
        {
            base.Send("*RST");
            base.Send("DISP:WIND:TRAC:DEL");
            base.Send("CALC:PAR:DEL 'CH1_S11_1'");
            base.Send(string.Concat("CALCulate:PARameter:DEFine 'DecenTest',", mode.ToString()));
            base.Send("DISPlay:WINDow1:TRACe1:FEED 'DecenTest'");
            base.Send(string.Concat("CALC:FORM ", format.ToString()));
            base.Send("OUTP OFF");
        }

        public override bool MinPowQuery(double Power)
        {
            bool flag;
            base.Send("OUTP:STAT OFF");
            base.Send(string.Concat("SOUR:POW ", Power.ToString()));
            flag = (Convert.ToDouble(base.Query("SOUR:POW?")) > Convert.ToDouble(Power) + 1 ? false : true);
            return flag;
        }

        public override void OutBandRestrain(double rfPow, double startFreq, double stopFreq, double ifbw, bool smo_OnOff, int smooth, int sweepPoint, bool avg_OnOff, int avgCount, ref string trace, ref double val)
        {
            double num = 0;
            string[] strArrays = new string[0];
            string empty = string.Empty;
            base.Send("CALC:PAR:DEL:ALL");
            base.Send("CALC:PAR:DEF 'Decentest_S21',S21");
            base.Send("DISP:WIND:TRACE:FEED 'Decentest_S21'");
            base.Send("CALC:PAR:SEL 'Decentest_S21'");
            base.Send(string.Concat("CALC:FORM ", NetworkAnalyzer.Format.MLOGarithmic.ToString()));
            base.Send(string.Concat("SENS:FREQ:STAR ", startFreq.ToString(), "MHz"));
            base.Send(string.Concat("SENS:FREQ:STOP ", stopFreq.ToString(), "MHz"));
            base.Send(string.Concat("SOUR:POW ", rfPow.ToString()));
            Thread.Sleep(200);
            base.Send("OUTP ON");
            Thread.Sleep(200);
            base.Send(string.Concat("SENS:BWID ", ifbw.ToString(), "kHz"));
            base.Send(string.Concat("SENS:SWE:POIN ", sweepPoint.ToString()));
            num = Convert.ToDouble(base.Query("SENSe:SWEep:TIME?"));
            base.SendOpc("DISP:WIND:TRAC:Y:AUTO", 100);
            switch (smo_OnOff)
            {
                case false:
                    {
                        base.Send("CALC:SMO OFF");
                        break;
                    }
                case true:
                    {
                        base.Send(string.Concat("CALC:SMO:APER ", smooth.ToString()));
                        base.Send("CALC:SMO ON");
                        break;
                    }
            }
            switch (avg_OnOff)
            {
                case false:
                    {
                        base.Send("SENS:AVER OFF");
                        break;
                    }
                case true:
                    {
                        base.Send(string.Concat("SENS:AVER:COUN ", avgCount.ToString()));
                        base.Send("SENS:AVER ON");
                        break;
                    }
            }
            Thread.Sleep(Convert.ToInt32(num * (double)avgCount * 1500));
            base.SendOpc("DISP:WIND:TRAC:Y:AUTO", 100);
            if (Convert.ToDouble(base.Query("DISP:WIND:TRAC:Y:PDIV?")) < 0.1)
            {
                base.Send("DISP:WIND:TRAC:Y:PDIV 0.1");
            }
            Thread.Sleep(1000);
            base.Send("CALC:MARK ON");
            base.Send("CALC:MARK:FUNC MAX");
            base.Send("CALC:MARK:FUNC:EXEC MAX");
            strArrays = base.Query("CALC:MARK:Y?").Split(new char[] { ',' });
            val = Convert.ToDouble(strArrays[0]);
            Thread.Sleep(400);
            trace = base.Query("CALC:DATA? FDATA");
            Thread.Sleep(100);
            base.Send("OUTP OFF");
            base.Send("CALC:SMO OFF");
            base.Send("SENS:AVER OFF");
        }

        public override void PowOutput(double dbm)
        {
            if (dbm <= -25)
            {
                dbm = -25;
                throw new ApplicationException("射频输出端无法输出满足要求的功率,请安装适当的选件!");
            }
            base.Send(string.Concat("SOUR:POW ", dbm.ToString()));
        }

        public override bool RecallFromFile(int num)
        {
            bool flag;
            string empty = string.Empty;
            try
            {
                if (base.Query("MMEM:CAT:CSAR? 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile'").IndexOf(string.Concat("DecenTest", num.ToString(), ".csa")) < 0)
                {
                    flag = false;
                }
                else
                {
                    base.Send(string.Concat("MMEM:LOAD 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile\\DecenTest", num.ToString(), ".csa'"));
                    flag = true;
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public override void RecallReg(int num)
        {
            base.Send(string.Concat("sense:correction:cset:select USER", num.ToString("00")));
        }

        public override double Rel_dB()
        {
            return base.QueryNumber("DISP:WIND:TRAC:Y:RLEV?");
        }

        public override void Reset()
        {
            base.Send("*RST");
        }

        public override double ReturnMarkX()
        {
            base.Send("CALC:MARK ON");
            string[] strArrays = base.Query("CALC:MARK:X?").Split(new char[] { ',' });
            return Convert.ToDouble(strArrays[0]) / 1000000;
        }

        public override double ReturnMarkY(double MarkX_MHz)
        {
            base.Send("CALC:MARK ON");
            base.Send(string.Concat("CALC:MARK:X ", MarkX_MHz.ToString(), "MHz"));
            ;
            string[] strArrays = base.Query("CALC:MARK:Y?").Split(new char[] { ',' });
            return Convert.ToDouble(strArrays[0]);
        }

        public override double ReturnPeakMax()
        {
            base.Send("CALC:MARK ON");
            base.Send("CALC:MARK:FUNC MAX");
            base.Send("CALC:MARK:FUNC:EXEC MAX");
            ;
            string[] strArrays = base.Query("CALC:MARK:Y?").Split(new char[] { ',' });
            return Convert.ToDouble(strArrays[0]);
        }

        public override double ReturnPeakMin()
        {
            base.Send("CALC:MARK ON");
            base.Send("CALC:MARK:FUNC MIN");
            base.Send("CALC:MARK:FUNC:EXEC MIN");
            string[] strArrays = base.Query("CALC:MARK:Y?").Split(new char[] { ',' });
            return Convert.ToDouble(strArrays[0]);
        }

        public override void RippleInBand(double rfPow, double startFreq, double stopFreq, double ifbw, bool smo_OnOff, int smooth, int sweepPoint, bool avg_OnOff, int avgCount, ref string trace, ref double val)
        {
            double num = 0;
            string[] strArrays = null;
            double[] numArray = new double[2];
            string empty = string.Empty;
            base.Send("CALC:PAR:DEL:ALL");
            base.Send("CALC:PAR:DEF 'Decentest_S21',S21");
            base.Send("DISP:WIND:TRACE:FEED 'Decentest_S21'");
            base.Send("CALC:PAR:SEL 'Decentest_S21'");
            base.Send(string.Concat("CALC:FORM ", NetworkAnalyzer.Format.MLOGarithmic.ToString()));
            base.Send(string.Concat("SENS:FREQ:STAR ", startFreq.ToString(), "MHz"));
            base.Send(string.Concat("SENS:FREQ:STOP ", stopFreq.ToString(), "MHz"));
            base.Send(string.Concat("SOUR:POW ", rfPow.ToString()));
            Thread.Sleep(200);
            base.Send("OUTP ON");
            Thread.Sleep(200);
            base.Send(string.Concat("SENS:BWID ", ifbw.ToString(), "kHz"));
            base.Send(string.Concat("SENS:SWE:POIN ", sweepPoint.ToString()));
            num = Convert.ToDouble(base.Query("SENSe:SWEep:TIME?"));
            Thread.Sleep(Convert.ToInt32(num * 2000));
            switch (smo_OnOff)
            {
                case false:
                    {
                        base.Send("CALC:SMO OFF");
                        break;
                    }
                case true:
                    {
                        base.Send(string.Concat("CALC:SMO:APER ", smooth.ToString()));
                        base.Send("CALC:SMO ON");
                        break;
                    }
            }
            switch (avg_OnOff)
            {
                case false:
                    {
                        base.Send("SENS:AVER OFF");
                        break;
                    }
                case true:
                    {
                        base.Send(string.Concat("SENS:AVER:COUN ", avgCount.ToString()));
                        base.Send("SENS:AVER ON");
                        break;
                    }
            }
            base.SendOpc("DISP:WIND:TRAC:Y:AUTO", 100);
            Thread.Sleep(Convert.ToInt32(num * (double)avgCount * 1500));
            base.Send("CALC:MARK ON");
            base.Send("CALC:MARK:FUNC MAX");
            base.Send("CALC:MARK:FUNC:EXEC MAX");
            string str = base.Query("CALC:MARK:Y?");
            char[] chrArray = new char[] { ',' };
            strArrays = str.Split(chrArray);
            numArray[0] = Convert.ToDouble(strArrays[0]);
            Thread.Sleep(400);
            base.Send("CALC:MARK:FUNC MIN");
            base.Send("CALC:MARK:FUNC:EXEC MIN");
            string str1 = base.Query("CALC:MARK:Y?");
            chrArray = new char[] { ',' };
            strArrays = str1.Split(chrArray);
            numArray[1] = Convert.ToDouble(strArrays[0]);
            Thread.Sleep(400);
            trace = base.Query("CALC:DATA? FDATA");
            Thread.Sleep(100);
            val = numArray[0] - numArray[1];
            base.Send("OUTP OFF");
            base.Send("CALC:SMO OFF");
            base.Send("SENS:AVER OFF");
        }

        public override void SaveReg(int num, string name)
        {
            base.Send(string.Concat("SENS:CORR:CSET:SAVE USER", num.ToString("00")));
        }

        public override void SaveToFile(int num)
        {
            base.Send(string.Concat("MMEM:STOR:CST 'C:\\Program Files\\Agilent\\Network Analyzer\\Documents\\DecenTestFile\\DecenTest", num.ToString("00"), "'"));
        }

        public override void Smooth_OFF()
        {
            base.Send("CALC:SMO OFF");
        }

        public override void Smooth_ON(int count)
        {
            base.Send(string.Concat("CALC:SMO:APER ", count.ToString()));
            base.Send("CALC:SMO ON");
        }

        public override void SpanFreq(double mhz)
        {
            base.Send(string.Concat("SENS:FREQ:SPAN ", mhz.ToString(), "MHz"));
        }

        public override void StarFreq(double mhz)
        {
            base.Send(string.Concat("SENS:FREQ:STAR ", mhz.ToString(), "MHz"));
        }

        public override void StdStyle_N50()
        {
            base.Send("CALKN50");
        }

        public override void StopFreq(double mhz)
        {
            base.Send(string.Concat("SENS:FREQ:STOP ", mhz.ToString(), "MHz"));
        }

        public override void TestPathLoss(double rfPow, double startFreq, double stopFreq, double ifbw, bool smo_OnOff, int smooth, int sweepPoint, bool avg_OnOff, int avgCount, string[] Marks, ref string trace, ref string[] PathLoss)
        {
            double num = 0;
            string[] strArrays = null;
            double[] numArray = new double[2];
            string empty = string.Empty;
            base.Send("CALC:PAR:DEL:ALL");
            base.Send("CALC:PAR:DEF 'Decentest_S21',S21");
            base.Send("DISP:WIND:TRACE:FEED 'Decentest_S21'");
            base.Send("CALC:PAR:SEL 'Decentest_S21'");
            base.Send(string.Concat("CALC:FORM ", NetworkAnalyzer.Format.MLOGarithmic.ToString()));
            base.Send(string.Concat("SENS:FREQ:STAR ", startFreq.ToString(), "MHz"));
            base.Send(string.Concat("SENS:FREQ:STOP ", stopFreq.ToString(), "MHz"));
            base.Send(string.Concat("SOUR:POW ", rfPow.ToString()));
            Thread.Sleep(20);
            base.Send("OUTP ON");
            Thread.Sleep(20);
            base.Send(string.Concat("SENS:BWID ", ifbw.ToString(), "kHz"));
            base.Send(string.Concat("SENS:SWE:POIN ", sweepPoint.ToString()));
            num = Convert.ToDouble(base.Query("SENSe:SWEep:TIME?"));
            Thread.Sleep((int)(num * 1000));
            switch (smo_OnOff)
            {
                case false:
                    {
                        base.Send("CALC:SMO OFF");
                        break;
                    }
                case true:
                    {
                        base.Send(string.Concat("CALC:SMO:APER ", smooth.ToString()));
                        base.Send("CALC:SMO ON");
                        break;
                    }
            }
            switch (avg_OnOff)
            {
                case false:
                    {
                        base.Send("SENS:AVER OFF");
                        break;
                    }
                case true:
                    {
                        base.Send(string.Concat("SENS:AVER:COUN ", avgCount.ToString()));
                        base.Send("SENS:AVER ON");
                        break;
                    }
            }
            Thread.Sleep((int)(num * 1000));
            base.SendOpc("DISP:WIND:TRAC:Y:AUTO", 1000);
            Thread.Sleep((int)(num * 1000) * avgCount);
            try
            {
                for (int i = 0; i < (int)Marks.Length; i++)
                {
                    base.Send(string.Concat("CALC:MARK:X ", Marks[i], "MHz"));
                    Thread.Sleep(10);
                    strArrays = base.Query("CALC:MARK:Y?").Split(new char[] { ',' });
                    string marks = Marks[i];
                    double num1 = -Convert.ToDouble(strArrays[0]);
                    PathLoss[i] = string.Concat(marks, ",", num1.ToString());
                    base.Send("CALC:MARK:AOFF");
                }
                trace = base.Query("CALC:DATA? FDATA");
                Thread.Sleep(20);
            }
            catch
            {
            }
            base.Send("OUTP OFF");
            base.Send("CALC:SMO OFF");
            base.Send("SENS:AVER OFF");
        }

        public override void TracePointNum(int num)
        {
            base.Send(string.Concat("SENS:SWE:POIN", num.ToString()));
        }

        public override void TransTimeDelay(double rfPow, double startFreq, double stopFreq, double ifbw, bool smo_OnOff, int smooth, int sweepPoint, bool avg_OnOff, int avgCount, ref string trace, ref double val)
        {
            double num = 0;
            string[] strArrays = null;
            string empty = string.Empty;
            base.Send("CALC:PAR:DEL:ALL");
            base.Send("CALC:PAR:DEF 'Decentest_S21',S21");
            base.Send("DISP:WIND:TRACE:FEED 'Decentest_S21'");
            base.Send("CALC:PAR:SEL 'Decentest_S21'");
            base.Send(string.Concat("CALC:FORM ", NetworkAnalyzer.Format.GDELay.ToString()));
            base.Send(string.Concat("SENS:FREQ:STAR ", startFreq.ToString(), "MHz"));
            base.Send(string.Concat("SENS:FREQ:STOP ", stopFreq.ToString(), "MHz"));
            base.Send(string.Concat("SOUR:POW ", rfPow.ToString()));
            Thread.Sleep(200);
            base.Send("OUTP ON");
            Thread.Sleep(200);
            base.Send(string.Concat("SENS:BWID ", ifbw.ToString(), "kHz"));
            base.Send(string.Concat("SENS:SWE:POIN ", sweepPoint.ToString()));
            Thread.Sleep(1000);
            switch (smo_OnOff)
            {
                case false:
                    {
                        base.Send("CALC:SMO OFF");
                        break;
                    }
                case true:
                    {
                        base.Send(string.Concat("CALC:SMO:APER ", smooth.ToString()));
                        base.Send("CALC:SMO ON");
                        break;
                    }
            }
            switch (avg_OnOff)
            {
                case false:
                    {
                        base.Send("SENS:AVER OFF");
                        break;
                    }
                case true:
                    {
                        base.Send(string.Concat("SENS:AVER:COUN ", avgCount.ToString()));
                        base.Send("SENS:AVER ON");
                        break;
                    }
            }
            num = Convert.ToDouble(base.Query("SENSe:SWEep:TIME?"));
            Thread.Sleep(Convert.ToInt32(num * 1300));
            base.SendOpc("DISP:WIND:TRAC:Y:AUTO", 1000);
            Thread.Sleep(Convert.ToInt32(num * (double)avgCount * 1000));
            base.Send("CALC:MARK ON");
            base.Send("CALC:MARK:FUNC MAX");
            base.Send("CALC:MARK:FUNC:EXEC MAX");
            strArrays = base.Query("CALC:MARK:Y?").Split(new char[] { ',' });
            val = Convert.ToDouble(strArrays[0]) * 1000000;
            Thread.Sleep(400);
            trace = base.Query("CALC:DATA? FDATA");
            base.Send("OUTP OFF");
            base.Send("CALC:SMO OFF");
            base.Send("SENS:AVER OFF");
        }

        public override void UnitFormat(NetworkAnalyzer.Format format)
        {
            base.Send(string.Concat("CALC:FORM ", format.ToString()));
        }

        public override void VSWR(double rfPow, double startFreq, double stopFreq, double ifbw, bool smo_OnOff, int smooth, int sweepPoint, bool avg_OnOff, int avgCount, ref string trace, ref double val)
        {
            double num = 0;
            string[] strArrays = null;
            string empty = string.Empty;
            base.Send("CALC:PAR:DEL:ALL");
            base.Send("CALC:PAR:DEF 'Decentest_S11',S11");
            base.Send("DISP:WIND:TRACE:FEED 'Decentest_S11'");
            base.Send("CALC:PAR:SEL 'Decentest_S11'");
            base.Send(string.Concat("CALC:FORM ", NetworkAnalyzer.Format.SWR.ToString()));
            base.Send(string.Concat("SENS:FREQ:STAR ", startFreq.ToString(), "MHz"));
            base.Send(string.Concat("SENS:FREQ:STOP ", stopFreq.ToString(), "MHz"));
            base.Send(string.Concat("SOUR:POW ", rfPow.ToString()));
            Thread.Sleep(200);
            base.Send("OUTP ON");
            base.Send(string.Concat("SENS:BWID ", ifbw.ToString(), "kHz"));
            Thread.Sleep(500);
            base.Send(string.Concat("SENS:SWE:POIN ", sweepPoint.ToString()));
            num = Convert.ToDouble(base.Query("SENSe:SWEep:TIME?"));
            base.SendOpc("DISP:WIND:TRAC:Y:AUTO", 1000);
            Thread.Sleep(Convert.ToInt32(num * 1000));
            switch (smo_OnOff)
            {
                case false:
                    {
                        base.Send("CALC:SMO OFF");
                        break;
                    }
                case true:
                    {
                        base.Send(string.Concat("CALC:SMO:APER ", smooth.ToString()));
                        base.Send("CALC:SMO ON");
                        break;
                    }
            }
            switch (avg_OnOff)
            {
                case false:
                    {
                        base.Send("SENS:AVER OFF");
                        break;
                    }
                case true:
                    {
                        base.Send(string.Concat("SENS:AVER:COUN ", avgCount.ToString()));
                        base.Send("SENS:AVER ON");
                        break;
                    }
            }
            Thread.Sleep(Convert.ToInt32(num * 1000));
            Thread.Sleep(Convert.ToInt32(num * (double)avgCount * 1000));
            base.SendOpc("DISP:WIND:TRAC:Y:AUTO", 100);
            try
            {
                base.Send("CALC:MARK ON");
                base.Send("CALC:MARK:FUNC MAX");
                base.Send("CALC:MARK:FUNC:EXEC MAX");
                strArrays = base.Query("CALC:MARK:Y?").Split(new char[] { ',' });
                val = Convert.ToDouble(strArrays[0]);
                Thread.Sleep(200);
                trace = base.Query("CALC:DATA? FDATA");
            }
            catch
            {
            }
            base.Send("OUTP OFF");
        }

        public override event NetworkAnalyzer.SendNAStatusDelegate SendNAStatus;


        public override Image CaptureScreenImage(string strDIR)
        {
            byte[] ImageDataInBytes = base.ReadBlock("HCOPY:SDUMP:DATA?");
            return ImageBytesConvertor.ConvertByteToImg(ImageDataInBytes);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="FloderPathAndName">文件夹的路径与名称</param>
        public override void CreatFloder(string FloderPathAndName)
        {
            base.Send("MMEMory:MDIRectory '" + FloderPathAndName + "'");
        }







        /// <summary>
        /// 非变频模式，基于校准文件的矢网设置
        /// </summary>
        /// <param name="PNA"></param>
        /// <param name="PNAsetting"></param>
        /// <returns></returns>
        public static bool PNAConfigureNormalMode(AgilentPNA835x.Application PNA, PNAsetting PNAsetting)
        {

            if (!string.IsNullOrEmpty(PNAsetting.PNAStateFilePathAndName))
            {
                PNA.Reset();
                PNA.Recall(PNAsetting.PNAStateFilePathAndName);
            }
            else

            { 
                PNA.Reset();
                PNA.Preset();
            
            }
            PNA.SourcePowerState = false;
            AgilentPNA835x.Channel Chan = (AgilentPNA835x.Channel)PNA.ActiveChannel;
            AgilentPNA835x.NAWindow Win = (AgilentPNA835x.NAWindow)PNA.ActiveNAWindow;
            AgilentPNA835x.ITrace ActiveTrace = PNA.ActiveMeasurement.Trace;
            AgilentPNA835x.IMeasurement ActiveMeas = PNA.ActiveMeasurement;

            #region 扫频设置
            if (PNAsetting.Type == SweepType.FreqSweep)
            {
                Chan.SweepType = AgilentPNA835x.NASweepTypes.naLinearSweep;
                Chan.StartFrequency = PNAsetting.StartFreq;
                Chan.StopFrequency = PNAsetting.StopFreq;
                Chan.CouplePorts = AgilentPNA835x.NAStates.naOFF;//关闭端口耦合，默认只对端口1设置功率端口2关闭
                Chan.set_SourcePortMode(2, AgilentPNA835x.NASourcePortMode.naSourcePortOff);

                Chan.set_TestPortPower(1, PNAsetting.TestPower);
                Chan.NumberOfPoints = (int)PNAsetting.SweepPoints;
                Chan.IFBandwidth = PNAsetting.IBW;
                if (PNAsetting.AverageNumber > 0)
                {
                    Chan.Averaging = true;
                }
                Chan.AverageMode = AgilentPNA835x.NAAverageMode.naAverageModePoint;               
                Chan.AveragingFactor = (int)PNAsetting.AverageNumber;
                if (PNAsetting.Yscale == 0)
                { Win.AutoScale(); }
                else
                {
                    ActiveTrace.YScale = PNAsetting.Yscale; ;
                    ActiveTrace.ReferencePosition = 5;//将测试曲线至于中部
                }
                Chan.Continuous();

            }

            #endregion

            #region 扫功率设置
            else if (PNAsetting.Type == SweepType.PowerSweep)
            {
                //先关闭校准，改变扫描模式后开启校准
                ActiveMeas.ErrorCorrection = false;
                Chan.SweepType = AgilentPNA835x.NASweepTypes.naPowerSweep;//设置为功率扫描
                Chan.centerFrequency = PNAsetting.CWFreq;
                System.Threading.Thread.Sleep(500);
                //ActiveMeas.ErrorCorrection = true;

                Chan.CouplePorts = AgilentPNA835x.NAStates.naOFF;//关闭端口耦合，默认只对端口1设置功率端口2关闭
                Chan.set_SourcePortMode(2, AgilentPNA835x.NASourcePortMode.naSourcePortOff);
                Chan.StartPower = PNAsetting.StartPower;
                Chan.StopPower = PNAsetting.StopPower;

                Chan.NumberOfPoints = (int)PNAsetting.SweepPoints;
                Chan.IFBandwidth = PNAsetting.IBW;
                if (PNAsetting.AverageNumber > 0)
                {
                    Chan.Averaging = true;
                }
                Chan.AverageMode = AgilentPNA835x.NAAverageMode.naAverageModePoint;
                Chan.AveragingFactor = (int)PNAsetting.AverageNumber;
                if (PNAsetting.Yscale == 0)
                { Win.AutoScale(); }
                else
                {
                    ActiveTrace.YScale = PNAsetting.Yscale; ;
                    ActiveTrace.ReferencePosition = 5;//将测试曲线至于中部
                }
                Chan.Continuous();



            }
            #endregion




            return true;
        }


        /// <summary>
        /// 非变频模式，基于校准文件的矢网设置读取
        /// </summary>
        /// <param name="PNA"></param>
        /// <param name="PNAsettingResult"></param>
        /// <returns></returns>
        public static bool PNAConfigrationReadNormalMode(AgilentPNA835x.Application PNA,  PNAsetting PNAsettingResult)
        {
            PNAsettingResult.ISconverterMode = false;
            
            AgilentPNA835x.Channel Chan = (AgilentPNA835x.Channel)PNA.ActiveChannel;
            AgilentPNA835x.NAWindow Win = (AgilentPNA835x.NAWindow)PNA.ActiveNAWindow;
            AgilentPNA835x.ITrace ActiveTrace = PNA.ActiveMeasurement.Trace;
            AgilentPNA835x.IMeasurement ActiveMeas = PNA.ActiveMeasurement;


            //固定一个临时位置作为state保存
            if (string.IsNullOrEmpty(PNAsettingResult.PNAStateFilePathAndName))
            {
                PNAsettingResult.PNAStateFilePathAndName = @"c:/RackSys/myTempState.csa";
                PNA.Save(PNAsettingResult.PNAStateFilePathAndName);
            }
            
            PNA.SourcePowerState = false;

            AgilentPNA835x.NASweepTypes tempType = Chan.SweepType;

            #region 扫频模式读取
            if (tempType == AgilentPNA835x.NASweepTypes.naLinearSweep)
            {
                PNAsettingResult.Type = SweepType.FreqSweep;
                PNAsettingResult.StartFreq = Chan.StartFrequency;
                PNAsettingResult.StopFreq = Chan.StopFrequency;
                PNAsettingResult.TestPower = Chan.get_TestPortPower(1);
                PNAsettingResult.SweepPoints = (uint)Chan.NumberOfPoints;
                PNAsettingResult.IBW = Chan.IFBandwidth;
                if (Chan.Averaging == true)
                {
                    PNAsettingResult.AverageNumber = (uint)Chan.AveragingFactor;
                }
                else
                {
                    PNAsettingResult.AverageNumber = 1;
                }
                
                PNAsettingResult.Yscale = ActiveTrace.YScale;

            }

            #endregion


            #region 扫功率模式读取
            else if (tempType == AgilentPNA835x.NASweepTypes.naPowerSweep)
            {
                PNAsettingResult.Type = SweepType.PowerSweep;
                PNAsettingResult.StartPower = Chan.StartPower;
                PNAsettingResult.StopPower = Chan.StopPower;
                PNAsettingResult.CWFreq = Chan.centerFrequency;
                PNAsettingResult.SweepPoints = (uint)Chan.NumberOfPoints;
                PNAsettingResult.IBW = Chan.IFBandwidth;
                if (Chan.Averaging == true)
                {
                    PNAsettingResult.AverageNumber = (uint)Chan.AveragingFactor;
                }
                else
                {
                    PNAsettingResult.AverageNumber = 1;
                }
                PNAsettingResult.Yscale = ActiveTrace.YScale;
            }

            #endregion


            return true;
        }







        /// <summary>
        /// 变频模式，基于校准文件的矢网设置
        /// </summary>
        /// <param name="PNA"></param>
        /// <param name="PNAsetting"></param>
        /// <returns></returns>
        public static bool PNAConfigureConverterMode(AgilentPNA835x.Application PNA, PNAsetting PNAsetting)
        {
            if (!string.IsNullOrEmpty(PNAsetting.PNAStateFilePathAndName))
            {
                PNA.Reset();
                PNA.Recall(PNAsetting.PNAStateFilePathAndName);
            }
            else
            {
                PNA.Reset();
                PNA.Preset();

            }
            PNA.SourcePowerState = false;
            AgilentPNA835x.IMeasurement ActiveMeasmix = PNA.CreateCustomMeasurementEx(1, "Vector Mixer/Converter", "VC21");
            AgilentPNA835x.Channel Chan = (AgilentPNA835x.Channel)PNA.ActiveChannel;
            AgilentPNA835x.NAWindow Win = (AgilentPNA835x.NAWindow)PNA.ActiveNAWindow;
            AgilentPNA835x.ITrace ActiveTrace = PNA.ActiveMeasurement.Trace;
            AgilentPNA835x.IMeasurement ActiveMeas = PNA.ActiveMeasurement;
            AgilentPNA835x.IConverter mixer = Chan.Converter;


            #region 扫频设置
            if (PNAsetting.Type == SweepType.FreqSweep)
            {
                mixer.InputStartFrequency = PNAsetting.StartFreq;
                mixer.InputStopFrequency = PNAsetting.StopFreq;
                #region 固定中频模式
                if (PNAsetting.IsFixedLOMode)
                {
                    mixer.set_LOFixedFrequency(1, PNAsetting.FixedLOFreq);
                    if (PNAsetting.IsHighConverter)
                    {
                        mixer.OutputSideband = AgilentPNA835x.ConverterSideBand.naHighSide;
                        mixer.Calculate(AgilentPNA835x.ConverterCalculation.naCalculateOUTPUT);
                    }
                    else
                    {
                        mixer.OutputSideband = AgilentPNA835x.ConverterSideBand.naLowSide;
                        mixer.Calculate(AgilentPNA835x.ConverterCalculation.naCalculateOUTPUT);

                    }
                }
                #endregion

                #region 固定输出模式

                else
                {
                    mixer.OutputFixedFrequency = PNAsetting.FixedOutputFreq;
                    if (PNAsetting.IsHighConverter)
                    {
                        mixer.OutputSideband = AgilentPNA835x.ConverterSideBand.naHighSide;
                        mixer.Calculate(AgilentPNA835x.ConverterCalculation.naCalculateLO1);
                    }
                    else
                    {
                        mixer.OutputSideband = AgilentPNA835x.ConverterSideBand.naLowSide;
                        mixer.Calculate(AgilentPNA835x.ConverterCalculation.naCalculateLO1);

                    }


                }
                #endregion
                Chan.CouplePorts = AgilentPNA835x.NAStates.naOFF;//关闭端口耦合，默认只对端口1设置功率端口2关闭
                Chan.set_SourcePortMode(2, AgilentPNA835x.NASourcePortMode.naSourcePortOff);

                Chan.set_TestPortPower(1, PNAsetting.TestPower);
                Chan.NumberOfPoints = (int)PNAsetting.SweepPoints;
                Chan.IFBandwidth = PNAsetting.IBW;
                if (PNAsetting.AverageNumber > 0)
                {
                    Chan.Averaging = true;
                }
                Chan.AverageMode = AgilentPNA835x.NAAverageMode.naAverageModePoint;
                Chan.AveragingFactor = (int)PNAsetting.AverageNumber;
                if (PNAsetting.Yscale == 0)
                { Win.AutoScale(); }
                else
                {
                    ActiveTrace.YScale = PNAsetting.Yscale; ;
                    ActiveTrace.ReferencePosition = 5;//将测试曲线至于中部
                }
                Chan.Continuous();

            }

            #endregion

            #region 扫功率设置
            else if (PNAsetting.Type == SweepType.PowerSweep)
            {
                #region 固定中频模式
                if (PNAsetting.IsFixedLOMode)
                {
                    mixer.set_LOFixedFrequency(1, PNAsetting.FixedLOFreq);
                    if (PNAsetting.IsHighConverter)
                    {
                        mixer.OutputSideband = AgilentPNA835x.ConverterSideBand.naHighSide;
                        mixer.Calculate(AgilentPNA835x.ConverterCalculation.naCalculateOUTPUT);
                    }
                    else
                    {
                        mixer.OutputSideband = AgilentPNA835x.ConverterSideBand.naLowSide;
                        mixer.Calculate(AgilentPNA835x.ConverterCalculation.naCalculateOUTPUT);

                    }
                }
                #endregion

                #region 固定输出模式

                else
                {
                    mixer.OutputFixedFrequency = PNAsetting.FixedOutputFreq;
                    if (PNAsetting.IsHighConverter)
                    {
                        mixer.OutputSideband = AgilentPNA835x.ConverterSideBand.naHighSide;
                        mixer.Calculate(AgilentPNA835x.ConverterCalculation.naCalculateLO1);
                    }
                    else
                    {
                        mixer.OutputSideband = AgilentPNA835x.ConverterSideBand.naLowSide;
                        mixer.Calculate(AgilentPNA835x.ConverterCalculation.naCalculateLO1);

                    }


                }
                #endregion
                //先关闭校准，改变扫描模式后开启校准
                ActiveMeas.ErrorCorrection = false;
                Chan.SweepType = AgilentPNA835x.NASweepTypes.naPowerSweep;//设置为功率扫描
                Chan.centerFrequency = PNAsetting.CWFreq;
                System.Threading.Thread.Sleep(2000);
                ActiveMeas.ErrorCorrection = true;

                Chan.CouplePorts = AgilentPNA835x.NAStates.naOFF;//关闭端口耦合，默认只对端口1设置功率端口2关闭
                Chan.set_SourcePortMode(2, AgilentPNA835x.NASourcePortMode.naSourcePortOff);
                Chan.StartPower = PNAsetting.StartPower;
                Chan.StopPower = PNAsetting.StopPower;

                Chan.NumberOfPoints = (int)PNAsetting.SweepPoints;
                Chan.IFBandwidth = PNAsetting.IBW;
                if (PNAsetting.AverageNumber > 0)
                {
                    Chan.Averaging = true;
                }
                Chan.AverageMode = AgilentPNA835x.NAAverageMode.naAverageModePoint;
                Chan.AveragingFactor = (int)PNAsetting.AverageNumber;
                if (PNAsetting.Yscale == 0)
                { Win.AutoScale(); }
                else
                {
                    ActiveTrace.YScale = PNAsetting.Yscale; ;
                    ActiveTrace.ReferencePosition = 5;//将测试曲线至于中部
                }
                Chan.Continuous();



            }
            #endregion




            return true;
        }


        /// <summary>
        /// 变频模式，基于校准文件的矢网设置读取
        /// </summary>
        /// <param name="PNA"></param>
        /// <param name="PNAsettingResult"></param>
        /// <returns></returns>
        public static bool PNAConfigrationReadConverterMode(AgilentPNA835x.Application PNA,  PNAsetting PNAsettingResult)
        {
            PNAsettingResult.ISconverterMode = true;
            AgilentPNA835x.Channel Chan = (AgilentPNA835x.Channel)PNA.ActiveChannel;
            AgilentPNA835x.NAWindow Win = (AgilentPNA835x.NAWindow)PNA.ActiveNAWindow;
            AgilentPNA835x.ITrace ActiveTrace = PNA.ActiveMeasurement.Trace;
            AgilentPNA835x.IMeasurement ActiveMeas = PNA.ActiveMeasurement;



            //固定一个临时位置作为state保存
            //固定一个临时位置作为state保存
            if (string.IsNullOrEmpty(PNAsettingResult.PNAStateFilePathAndName))
            {
                PNAsettingResult.PNAStateFilePathAndName = @"c:/RackSys/myTempState.csa";
                PNA.Save(PNAsettingResult.PNAStateFilePathAndName);
            }
            
            PNA.SourcePowerState = false;


            AgilentPNA835x.NASweepTypes tempType = Chan.SweepType;
            PNA.Save(PNAsettingResult.PNAStateFilePathAndName);
            AgilentPNA835x.IConverter mixer = Chan.Converter;

            #region 扫频模式读取
            if (tempType == AgilentPNA835x.NASweepTypes.naLinearSweep)
            {
                PNAsettingResult.Type = SweepType.FreqSweep;
                if (mixer.OutputSideband == AgilentPNA835x.ConverterSideBand.naHighSide)
                {
                    PNAsettingResult.IsHighConverter = true;
                }
                else { PNAsettingResult.IsHighConverter = false; }

                PNAsettingResult.StartFreq = mixer.InputStartFrequency;
                PNAsettingResult.StopFreq = mixer.InputStopFrequency;
                PNAsettingResult.TestPower = Chan.get_TestPortPower(1);
                PNAsettingResult.SweepPoints = (uint)Chan.NumberOfPoints;
                PNAsettingResult.IBW = Chan.IFBandwidth;
                if (Chan.Averaging == true)
                {
                    PNAsettingResult.AverageNumber = (uint)Chan.AveragingFactor;
                }
                else
                {
                    PNAsettingResult.AverageNumber = 1;
                }
                PNAsettingResult.Yscale = ActiveTrace.YScale;
                PNAsettingResult.FixedLOFreq = mixer.get_LOFixedFrequency(1);
                PNAsettingResult.FixedOutputFreq = mixer.OutputFixedFrequency;
                //此处先固定返回固定本振模式，需在仪表试验后，方可实现完整判断
                PNAsettingResult.IsFixedLOMode = true;


            }

            #endregion


            #region 扫功率模式读取
            else if (tempType == AgilentPNA835x.NASweepTypes.naPowerSweep)
            {
                PNAsettingResult.Type = SweepType.PowerSweep;
                if (mixer.OutputSideband == AgilentPNA835x.ConverterSideBand.naHighSide)
                {
                    PNAsettingResult.IsHighConverter = true;
                }
                else { PNAsettingResult.IsHighConverter = false; }
                PNAsettingResult.StartPower = mixer.InputStartPower;
                PNAsettingResult.StopPower = mixer.InputStopPower;
                PNAsettingResult.CWFreq = Chan.centerFrequency;
                PNAsettingResult.SweepPoints = (uint)Chan.NumberOfPoints;
                PNAsettingResult.IBW = Chan.IFBandwidth;
                if (Chan.Averaging == true)
                {
                    PNAsettingResult.AverageNumber = (uint)Chan.AveragingFactor;
                }
                else
                {
                    PNAsettingResult.AverageNumber = 1;
                }
                PNAsettingResult.Yscale = ActiveTrace.YScale;
                PNAsettingResult.FixedLOFreq = mixer.get_LOFixedFrequency(1);
                PNAsettingResult.FixedOutputFreq = mixer.OutputFixedFrequency;
                //此处先固定返回固定本振模式，需在仪表试验后，方可实现完整判断
                PNAsettingResult.IsFixedLOMode = true;
            }

            #endregion


            return true;
        }


        //根据指定的路径，返回文件列表

        public override void GetAllsCalsetNames(string DefaultDir, out List<string> calsetResult)
        {

            string s = this.Query(":MMEMory:CATalog? \"" + DefaultDir + '\"');
            s = s.Remove(0, 1);
            s = s.Remove(s.Length - 1, 1);
            string[] calsets = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            calsetResult = new List<string>();
            for (int i = 0; i < calsets.Length; i++)
            {
                if (!((calsets[i].Contains("pcs") || calsets[i].Contains("csa"))))
                    continue;

                calsetResult.Add(calsets[i]);//.Remove(0, 1)

            }

            //calsetResult = new string[CorrectCalset.Count];
            //for (int j = 0; j < CorrectCalset.Count; j++)
            //{ calsetResult[j] = CorrectCalset[j]; }

        }

        /// <summary>
        /// 使用当前状态文件，返回状态文件路径，及基本设置
        /// </summary>
        /// <param name="PNA"></param>
        /// <param name="stateFilePathAndName"></param>
        /// <param name="PNAsetting"></param>
        /// <returns></returns>
        public override bool UseCurrentState(AgilentPNA835x.Application PNA, out PNAsetting PNAsetting)
        {
            PNAsetting = new NetworkAnalyzer.PNAsetting();
            ReadPNAConfiguration(PNA,  PNAsetting);

            return true;
        }

        /// <summary>
        /// 读取当前PNA设置，自动判断是否变频模式
        /// </summary>
        /// <returns></returns>
        public override bool ReadPNAConfiguration(AgilentPNA835x.Application PNA,  PNAsetting PNAsettingResult)
        {
            
            if (this.QueryNumber("SENSe:FOM:STATe?") > 0)
            {
                PNAConfigrationReadConverterMode(PNA,  PNAsettingResult);

            }
            else
            {
                PNAConfigrationReadNormalMode(PNA,  PNAsettingResult);
            }

            return true;



        }

        /// <summary>
        /// 设置PNA设置，不用区分是否变频模式
        /// </summary>
        /// <returns></returns>
        public override bool PNAConfiguration(AgilentPNA835x.Application PNA, PNAsetting PNAsetting)
        {
            if (PNAsetting.ISconverterMode)
            { PNAConfigureConverterMode(PNA, PNAsetting); }
            else
            { PNAConfigureNormalMode(PNA, PNAsetting); }
            return true;

        }


        /// <summary>
        /// 使用用户选择的状态文件，并返回普遍设置
        /// </summary>
        /// <param name="PNA"></param>
        /// <param name="FilePathAndName"></param>
        /// <param name="PNAsetting"></param>
        /// <returns></returns>
        public override bool PNACalSetApply(AgilentPNA835x.Application PNA, string FilePathAndName, out PNAsetting PNAsetting)
        {
            PNAsetting = new NetworkAnalyzer.PNAsetting();
            PNA.Recall(FilePathAndName);
            PNAsetting.PNAStateFilePathAndName = FilePathAndName;

            ReadPNAConfiguration(PNA,  PNAsetting);
            return true;
        }

    }
}
