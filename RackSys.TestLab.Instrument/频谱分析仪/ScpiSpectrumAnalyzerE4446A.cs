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
    internal class ScpiSpectrumAnalyzerE4446A : ScpiSpectrumAnalyzer
    {


        public ScpiSpectrumAnalyzerE4446A(string address)
            : base(address)
        {
        }

        /*
         * 这个驱动的参考文件为，E4440-90353.PDF
         * */

        public override void NoiseFigureMode()
        {
            this.Send(":INSTrument:NSELect 219");
            this.WaitOpc();
        }

        public override double NF_RBW
        {
            get
            {
                return this.QueryNumber(":BWIDth?");
            }
            set
            {
               this.Send(string.Concat(":BWIDth", value));
               this.WaitOpc();
            }
        }


        /// <summary>
        /// 变频模式支持
        /// </summary>
        /// <param name="fixLOFreq"></param>
        /// <param name="isHighConverter"></param>
        public override void NoiseFigureMixerModeSetup(double fixLOFreq, bool isHighConverter)
        {
            string tmp;
            if (isHighConverter) tmp = "UPConv";
            else tmp = "DOWNconv";
            this.Send(string.Concat(":CONF:MODE:DUT ", tmp));
            this.WaitOpc();
            this.Send(string.Concat(":CONF:MODE:SYST:LOSC:FREQ ", fixLOFreq));
            this.WaitOpc();

            
        }

        public override void NoiseFigureCalNow(string ENRTable)
        {
            if (!string.IsNullOrEmpty(ENRTable))
            {
                this.Send("CORR:ENR:MEAS:TABL:ID:DATA 'ENRTable'");
                this.WaitOpc();
                
                this.Send(string.Concat("CORR:ENR:MEAS:TABL:DATA ", ENRTable));
                this.WaitOpc();
                this.Send("CORR:ENR:MODE TABL");
                this.WaitOpc();
            }
            this.Send("CORR:COLL STAN");
            this.WaitOpc();
            ;

           
        }

        public override void SetLossCompTableBeforeDUT(string FreqAndLossList)
        {
            ////删除table
            //this.Send("CORR:LOSS:INP:TABL:DEL 'InputLoss'");
            //this.WaitOpc();
            ////重新建立table
            //this.Send("CORR:LOSS:INP:TABL:SEL 'InputLoss'");
            //this.WaitOpc();
            ////Table赋值
            this.Send(":CORRection:LOSS:BEFore:TABLe:DATA " + FreqAndLossList);
            this.WaitOpc();
            //table生效
            this.Send(":CORRection:LOSS:BEFore:MODE TABL");
            this.WaitOpc();
            this.Send(":CORRection:LOSS:BEFore ON");
            this.WaitOpc();
        }


        
        public override void SetLossCompTableAfterDUT(string FreqAndLossList)// 郝佳添加驱动,2013.12.4
        {
            ////删除table
            //this.Send("CORR:LOSS:OUTP:TABL:DEL 'OutputLoss'");
            //this.WaitOpc();
            ////重新建立table
            //this.Send("CORR:LOSS:OUTP:TABL:SEL 'OutputLoss'");
            //this.WaitOpc();
            ////Table赋值
            this.Send(":CORRection:LOSS:AFTer:TABLe:DATA " + FreqAndLossList);
            this.WaitOpc();
            //table生效
            this.Send(":CORRection:LOSS:AFTer:MODE TABL");
            this.WaitOpc();
            
            this.Send(":CORRection:LOSS:AFTer on");
            this.WaitOpc();
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
                noisefigure[i] = this.QueryNumber(string.Concat(":TRACe:CORRected:AMPLitude? NFIGure,",(start+step*i)));
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

        /// <summary>
        /// 使用频谱仪当前的状态文件
        /// </summary>
        /// <returns></returns>
        /*
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
        public override bool SpectrumAnalyzerUseSelectedState(bool isNFMode, string stateFilepPathAndName, SpecTrumAnalyzerSetting SASetting)
        {
            this.LoadStateFromPath(stateFilepPathAndName);
            SASetting.NFStateFilePathAndName = stateFilepPathAndName;
            SpectrumAnalyzerConfigurationRead(isNFMode,  SASetting);
            return true;
        }
        
        */




        //杨飞添加驱动 2016.01.07 

        #region  频谱仪触发模式

        /// <summary>
        /// 设置频谱仪触发模式
        /// </summary>
        /// <param name="value">状态模式</param>
        private  void SetTriggerMode(TriggerMode value)
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
                        str = "EXTernal";
                        break;
                    }
                case SpectrumAnalyzer.TriggerMode.Ext1:
                    {
                        str = "EXTernal";
                        break;
                    }
                case SpectrumAnalyzer.TriggerMode.Ext2:
                    {
                        str = "EXTernal2";
                        break;
                    }
                case SpectrumAnalyzer.TriggerMode.RFBurst:
                    {
                        str = "RFBurst";
                        break;
                    }
                case SpectrumAnalyzer.TriggerMode.Video:
                    {
                        str = "VIDeo";
                        break;
                    }
                case SpectrumAnalyzer.TriggerMode.Line:
                    {
                        str = "LINE";
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

        private TriggerMode GetTriggerMode()
        {
                string str = this.Query(string.Format(":TRIGger:SOURce?"));
                if (str == "IMMediate")
                {
                    return SpectrumAnalyzer.TriggerMode.Immediate;
                }
                if (str == "EXTernal")
                {
                    return SpectrumAnalyzer.TriggerMode.Ext1;
                }
                if (str == "EXTernal2")
                {
                    return SpectrumAnalyzer.TriggerMode.Ext2;
                }
                if (str == "RFBurst")
                {
                    return SpectrumAnalyzer.TriggerMode.RFBurst;
                }
                if (str == "VIDeo")
                {
                    return SpectrumAnalyzer.TriggerMode.Video;
                }
                if (str == "LINE")
                {
                    return SpectrumAnalyzer.TriggerMode.Line;
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

        public override bool PreAmplifier
        {
            get
            {
                if (this.Query(":POWer:GAIN?").ToUpper().StartsWith("ON") || this.Query(":POWer:GAIN?").ToUpper().StartsWith("1"))
                { return true; }
                else { return false; }
            }
            set
            {
                if (value)
                {
                    this.Send(string.Concat(":POWer:GAIN ON"));
                    this.WaitOpc();
                }
                else
                {
                    this.Send(string.Concat(":POWer:GAIN OFF"));
                    this.WaitOpc();
                }
            }
        }

 
        #endregion
    }
    
}
