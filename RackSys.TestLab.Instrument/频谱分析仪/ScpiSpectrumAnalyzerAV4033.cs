using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public enum TraceNumber
    {
        TRA=1,
        TRB=2
    }

    internal class ScpiSpectrumAnalyzerAV4033 : ScpiSpectrumAnalyzer
    {
        public ScpiSpectrumAnalyzerAV4033(string address)
            : base(address)
        { }
        protected override void DetermineIdentity()
        {
            base.m_identity = this.QueryWithoutLineFeed("ID?;");
            char[] separator = new char[] { ',' };
            string[] strArrays = base.Identity.Split(separator);
            base.m_Manufactor = "41所";//strArrays[0].Trim();
            if (strArrays.Length > 1)
            {
                base.m_model = strArrays[0].Trim();
            }
            //if (strArrays.Length > 2)
            //{
            //    base.m_serial = strArrays[2].Trim();
            //}
            //if (strArrays.Length > 3)
            //{
            //    base.m_firmwareVersion = strArrays[3].Trim();
            //}
        }
        protected override void DetermineOptions()
        {
            //if (!SpectrumAnalyzer.IsAV4003(this.Model))
            //{
            //    base.m_options = this.Query("*OPT?");
            //    return;
            //}
            this.m_options = "";
        }

        public override double CenterFrequency
        {
            get
            {
               return double.Parse(base.QueryWithoutLineFeed("CF?;"));
            }
            set
            {
                base.SendWithoutLineFeed(string.Format("CF {0};", value));
                this.WaitOpc();
            }
        }

        public override double Span
        {
            get
            {
                return double.Parse(QueryWithoutLineFeed("SP?;"));
            }
            set
            {
                base.SendWithoutLineFeed(string.Format("SP {0};", value));
                this.WaitOpc();
            }
        }

        public override double ReferenceLevel
        {
            get
            {
                return double.Parse(QueryWithoutLineFeed("RL?;"));
            }
            set
            {
                base.SendWithoutLineFeed(string.Format("RL {0} BDM;", value));
                this.WaitOpc();
            }
        }

        public override double Attenuation
        {
            get
            {
                return double.Parse(QueryWithoutLineFeed("AT?;"));
            }
            set
            {
                base.SendWithoutLineFeed(string.Format("AT {0} DB;", value));
                this.WaitOpc();
            }
        }

        public override double SweepTime
        {
            get
            {
                return double.Parse(QueryWithoutLineFeed("ST?;"));
            }
            set
            {
                base.SendWithoutLineFeed(string.Format("ST {0};", value));
                this.WaitOpc();
            }
        }

        public override double TriggerDelay
        {
            get
            {
                return double.Parse(QueryWithoutLineFeed("DLYSWP?;"));
            }
            set
            {
                if (value == 0)
                {
                    this.SendWithoutLineFeed("DLYSWP OFF;");
                    this.WaitOpc();
                }
                else
                {
                    this.SendWithoutLineFeed("DLYSWP ON;");
                    this.WaitOpc();
                    this.SendWithoutLineFeed(string.Format("DLYSWP {0}", value));
                    this.WaitOpc();
                }
            }
        }

        public override double RBW
        {
            get
            {
                return double.Parse(QueryWithoutLineFeed("RB?;"));
            }
            set
            {
                base.SendWithoutLineFeed(string.Format("RB {0} HZ;", value));
                this.WaitOpc();
            }
        }

        public override double VBW
        {
            get
            {
                return double.Parse(QueryWithoutLineFeed("VB?;"));
            }
            set
            {
                base.SendWithoutLineFeed(string.Format("VB {0} HZ;", value));
                this.WaitOpc();
            }
        }

        public override TriggerMode TriggerModeState
        {
            get
            {
                string str = this.QueryWithoutLineFeed("TM?;");
                if (str == "FREE")
                {
                    return SpectrumAnalyzer.TriggerMode.Immediate;
                }
                if (str == "EXT")
                {
                    return SpectrumAnalyzer.TriggerMode.Extern;
                }
                if (str == "VID")
                {
                    return SpectrumAnalyzer.TriggerMode.Video;
                }
                if (str == "LINE")
                {
                    return SpectrumAnalyzer.TriggerMode.Line;
                }
                return SpectrumAnalyzer.TriggerMode.Immediate;
            }
            set
            {
                string str;
                switch (value)
                {
                    case SpectrumAnalyzer.TriggerMode.Immediate:
                        {
                            str = "FREE";
                            break;
                        }
                    case SpectrumAnalyzer.TriggerMode.Extern:
                        {
                            str = "EXT";
                            break;
                        }
                    case SpectrumAnalyzer.TriggerMode.Video:
                        {
                            str = "VID";
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
                this.SendWithoutLineFeed(string.Format("TM {0};", str));
                this.WaitOpc();
            }
        }

        public override void SetMarkerPositionByIndex(double inMarkerPosition, int MarkerIndex)
        {
            base.SendWithoutLineFeed(string.Format("MAK {0} HZ;", inMarkerPosition));
            this.WaitOpc();
        }

        public override double GetMarkerValueByIndex(int MarkerIndex)
        {
            return double.Parse(QueryWithoutLineFeed("MAK?;"));
        }

        public override void Preset()
        {
            int timeout = this.Timeout;
            this.SendWithoutLineFeed("IP:DONE?;");
            this.WaitOpc();
            this.Timeout = timeout;
        }

        public override void SetDisplayLine(string value)
        {
            base.SendWithoutLineFeed(string.Format("DL {0};", value));
            this.WaitOpc();
        }

        public override void DisplayLineValue(double value)
        {
            base.SendWithoutLineFeed(string.Format("DL {0} DBM;", value));
            this.WaitOpc();
        }

        public override void MarkerPeakSearch(int MarkerIndex)
        {
            base.SendWithoutLineFeed("MKPK HI;");
            this.WaitOpc();
        }

        public override double[] TraceDataByIndex(int TraceIndex)
        {
            TraceNumber trace = (TraceNumber)TraceIndex;
            double[] numArray1 = null;
            try
            {
                this.m_CommMutex.WaitOne();
                string rst= QueryWithoutLineFeed(string.Format("{0}?;", trace));
                string []numstr=rst.Split(',');
                 numArray1=new double[numstr.Length];
                 for (int i = 0; i < (int)numArray1.Length; i++)
                {
                    numArray1[i] = double.Parse(numstr[i]);
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
       
        /// <summary>
        /// 设置迹线的状态
        /// </summary>
        /// <param name="inState">迹线的状态值</param>
        /// <param name="TraceIndex">迹线序号</param>
        public override void SetTraceStateByIndex(TraceState inState, int TraceIndex)
        {
            TraceNumber trace = (TraceNumber)TraceIndex;
            string str;
            switch (inState)
            {
                case SpectrumAnalyzer.TraceState.Maxhold:
                    {
                        str = "MXMH";
                        break;
                    }
                case SpectrumAnalyzer.TraceState.Minhold:
                    {
                        str = "MINH";
                        break;
                    }
                case SpectrumAnalyzer.TraceState.Write:
                    {
                        str = "CLRW";
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
            this.SendWithoutLineFeed(string.Format("{0} {1};", str, trace));
            this.WaitOpc();
        }

        //暂时没发现扫描点数设置
   
        //暂时没发现自检功能
    }
}
