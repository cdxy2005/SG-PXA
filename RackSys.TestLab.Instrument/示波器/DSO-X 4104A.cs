using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;





namespace RackSys.TestLab.Instrument
{

    /// <summary>
    /// 
    /// </summary>
    public class DSO_X_4104A : OSCBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="inAddress">visa地址</param>
        public DSO_X_4104A(string inAddress)
            : base(inAddress)
        {
            Timeout = 40000;
        }

        public override void Preset()
        {
            base.Send(":SYSTem:PRESet");
            base.WaitOpc(100000);
        }

        /// <summary>
        /// 判断通道是否打开
        /// </summary>
        /// <param name="channelnumber"></param>
        /// <returns></returns>
        public override bool IsChannelDisplayOn(int channelnumber)
        {
            return base.QueryNumber(":CHANnel" + channelnumber.ToString() + ":DISPlay?") > 0;
        }

        /// <summary>
        /// 设置通道开启或关闭
        /// </summary>
        /// <param name="channelnumber"></param>
        /// <param name="state"></param>
        public override void SetChannelDisplayOn(int channelnumber, bool state)
        {
            base.Send(":CHANNEL" + channelnumber.ToString() + ":DISPLAY" + (state ? " ON" : " OFF"));
        }
        /// <summary>
        /// 设置Function打开
        /// </summary>
        /// <param name="FunctionNO">Function数</param>
        /// <param name="state">打开还是关闭？</param>
        public override void SetFunctionDisplayOn(int FunctionNO, bool state)
        {
            base.Send(":FUNCTION" + FunctionNO.ToString() + ":DISPLAY" + (state ? " ON" : " OFF"));
        }
        /// <summary>
        /// 时间
        /// </summary>
        public override double TimebasePosition
        {
            get
            {
                return base.QueryNumber(":TIMebase:POSition?");
            }
            set
            {
                base.SendNumber(":TIMebase:POSition ", value);
                WaitOpc();
            }
        }


        public override void CHANnelINVert(int chan, bool IsInVert)
        {
            int INVert = 0;
            if (IsInVert)
            {
                INVert = 1;
            }
            this.Send(string.Format(":CHANnel{0}:INVert {1}", chan, INVert));
        }
        public override bool RefClock
        {
            get
            {

                return base.QueryNumber(":TIMebase:REFClock?") > 0;
            }
            set
            {
                base.Send(":TIMebase:REFClock" + (value ? " ON" : " OFF"));
                base.WaitOpc();
            }

        }

        /// <summary>
        /// 设置水平时间范围（/div*10）
        /// </summary>
        public override double TimeRange
        {
            get
            {
                return base.QueryNumber(":TIMebase:RANGe?");
            }
            set
            {
                base.SendNumber(":TIMEBASE:RANGE ", value);
                WaitOpc();
            }
        }

        /// <summary>
        /// 设置水平时间刻度（/div）；注意与timeRange的区别
        /// </summary>
        public override double TimeScale
        {
            get
            {
                double TimeScale = base.QueryNumber(":TIMebase:SCALe?");
                return TimeScale;
            }
            set
            {
                base.SendNumber(":TIMebase:SCALe ", value);
                WaitOpc();
            }
        }
        public override double MEASurePERiod (int chanNumber)
    {
        double TimeScale = base.QueryNumber(":MEASure:PERiod? CHANnel" + chanNumber);
                return TimeScale;
    
    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public override void LoadStateFile(string fileName)
        {
            string name = string.Format("{0}{1}{2}", "\"", fileName, "\"");
            string[] str = new string[] { ":DISK:LOAD ", name, ",set " };
            base.Send(string.Concat(str));
            base.WaitOpc();
        }


        /// <summary>
        /// 获取通道的垂直偏移量
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetChannelVerticalOffset(int ChannelNumber)
        {
            double offset = base.QueryNumber(":CHANnel" + ChannelNumber.ToString() + ":OFFSet?");
            return offset;
        }

        /// <summary>
        /// 设置通道的垂直便宜量
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <param name="offset"></param>
        public override void SetChannelVerticalOffset(int ChannelNumber, double offset)
        {
            base.Send(":CHANnel" + ChannelNumber.ToString() + ":OFFSet " + offset.ToString());
            WaitOpc();
        }

        /// <summary>
        /// 获取通道的垂直标尺
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <returns></returns>
        public override double GetChannelVerticalScale(int ChannelNumber)
        {
            double scale = base.QueryNumber(":CHANnel" + ChannelNumber.ToString() + ":SCALe?");
            return scale;
        }

        public override double GetFunctionVerticalRange(int FunctionNO)
        {
            double range = base.QueryNumber(":FUNCtion" + FunctionNO.ToString() + ":RANGe?");
            return range;
        }

        public override void SetFunctionVerticalRange(int FunctionNO, double range)
        {
            base.Send(":FUNCtion" + FunctionNO.ToString() + ":RANGe " + range.ToString());
            WaitOpc();
        }
        /// <summary>
        /// 设置通道的垂直标尺
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <param name="scale"></param>
        public override void SetChannelVerticalScale(int ChannelNumber, double scale)
        {
            base.Send(":CHANnel" + ChannelNumber.ToString() + ":SCALe " + scale.ToString());
            WaitOpc();
        }

        public override double PowerToScale(double dBm)
        {
            double power = Math.Pow(10, dBm / 10) / 1000;         //将对数功率dBm转换为线性功率W
            double Vpp = 2 * Math.Sqrt(100 * power);                //将功率值换算为负载匹配条件下的电压峰峰值
            double[] NormalScale = new double[] { 5, 10, 20, 50, 100, 200, 500 };
            double scale = (Vpp / 5.6) * 1000;                 //按照此信号在示波器上占70%屏幕的条件下计算垂直刻度
            double[] delta = new double[NormalScale.Length];
            for (int i = 0; i < NormalScale.Length; i++)
            {
                delta[i] = Math.Abs(scale - NormalScale[i]);

            }
            double min = delta[0];
            //double scale = NormalScale[0];
            for (int i = 0; i < delta.Length; i++)
            {
                if (min >= delta[i])
                {
                    min = delta[i];
                    scale = NormalScale[i];
                }
            }
            return scale * 1e-3;
        }
        /// <summary>
        /// 测量相位差
        /// </summary>
        /// <param name="ref_1"></param>
        /// <param name="meas_1"></param>
        /// <returns></returns>
        public override void MeasureDeltaPhase(int Ref_ChannelNumber, int Meas_ChannelNumber)
        {
            //string reference = SourceForMeasSwitchToStr(ref_1);
            //string measurement = SourceForMeasSwitchToStr(meas_1);
            //this.AverageEnable = true;               //打开平均
            //this.AvgNumber = 256;                         //设置平均次数为256
            //this.WaitOpc();
            this.Send(string.Format(":MEASure:PHASe CHANnel{0}, CHANnel{1}, RISing", Ref_ChannelNumber, Meas_ChannelNumber));
            this.WaitOpc();
        }

        public override double GetDeltaPhase(int Ref_ChannelNumber, int Meas_ChannelNumber)
        {
            double temp = this.QueryNumber(string.Format(":MEASure:PHASe? CHANnel{0}, CHANnel{1}, RISing", Ref_ChannelNumber, Meas_ChannelNumber));
            this.WaitOpc();
            return temp;
        }

        public override void SqureFunction(int FunctionNO, int ChannleNumber)
        {
            base.Send(":FUNCtion" + FunctionNO.ToString() + ":SQUare CHANNEL" + ChannleNumber.ToString());
            this.WaitOpc();
        }

        public override void LowPassFilter(int FunctionNO, int FunctionTotest, double bandwidth)
        {
            base.Send(":FUNCtion" + FunctionNO.ToString() + ":LOWPASS FUNCtion" + FunctionTotest.ToString() + "," + bandwidth.ToString());
            this.WaitOpc();
        }


        /// <summary>
        /// 获取轴测试数据
        /// </summary>
        /// <param name="ChanNumber"></param>
        /// <returns></returns>
        public override double[] GetTraceData(SourceForMeas tracename)
        {

            this.Send(string.Format(":WAVeform:SOURce " + tracename));
            WaitOpc();
            this.Send(":WAVeform:FORMat WORD");
            WaitOpc();
            this.Send(":WAVeform:BYTeorder LSBFirst");
            WaitOpc();
            this.Send(":WAVeform:STReaming 0");
            WaitOpc();
            double yInc = this.QueryNumber(":WAVeform:YINCrement?");//:WAVeform:YINCrement
            double yOrg = this.QueryNumber(":WAVeform:YORigin?");
            //tmpRawData:第一个字节是#，第二个字符为0，其余为波形数据
            //:WAVeform:VIEW 选取类型
            this.Send(":WAVeform:VIEW ALL");
            WaitOpc();
            byte[] tmpRawData = this.ReadBlock(":WAVeform:DATA?");

            //第一个点？
            int tmpTraceDataLen = (tmpRawData.Length) / 2;
            double[] getdata = new double[tmpTraceDataLen];
            //double pointData;
            for (int i = 0; i < getdata.Length; i++)
            {
                getdata[i] = (Int16)((UInt16)tmpRawData[2 * i] + (((UInt16)tmpRawData[2 * i + 1]) << 8));
                getdata[i] = getdata[i] * yInc + yOrg;
            }
            return getdata;
        }
        /// <summary>
        /// 获取X轴时间数据
        /// </summary>
        /// <param name="tracename"></param>
        /// <returns></returns>
        public override double[] GetTimePosition(SourceForMeas tracename)
        {
            this.Send(string.Format(":WAVeform:SOURce " + tracename));
            WaitOpc();
            //double XRange = this.QueryNumber(":WAVeform:XRANge?");//:WAVeform:XINCrement

            double step = this.QueryNumber(":WAVeform:XINCrement?");//:WAVeform:XINCrement
            double XStart = this.QueryNumber(":WAVeform:XORigin?");
            double[] YData = this.GetTraceData(tracename);
            int number = YData.Length;
            double[] XData = new double[number];
            for (int xi = 0; xi < number; xi++)
            {
                XData[xi] = XStart + step * xi;
            }
            return XData;
        }

        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="ImageFileName"></param>
        /// <returns></returns>
        public override Image CaptureScreenImage(string ImageFileName)//Int截取示波器图片k ik
        {
            //string name = string.Format("{0}{1}{2}", "\"", ImageFileName, "\"");
            //string[] strSend = new string[] { ":DISK:SAVE:IMAGe ", name, "BMP" };

            byte[] ImageDataInBytes = base.ReadBlock(":DISPlay:DATA? PNG");
            return ImageBytesConvertor.ConvertByteToImg(ImageDataInBytes);

        }


        /// <summary>
        /// 平均次数
        /// </summary>
        public override double AvgNumber
        {
            get
            {

                return base.QueryNumber(":ACQuire:COUNt?");
            }
            set
            {
                base.SendNumber(":ACQuire:COUNt", value);
                base.WaitOpc();
            }
        }
        /// <summary>
        /// 是否开启平均
        /// </summary>
        public override bool AverageEnable
        {
            get
            {

                return base.QueryNumber(":ACQuire:AVERage?") > 0;
            }
            set
            {
                base.Send(":ACQuire:AVERage" + (value ? " ON" : " OFF"));
                base.WaitOpc();
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        public override void Stop()
        {
            this.Send(":STOP");
            WaitOpc();
        }
        /// <summary>
        /// 运行
        /// </summary>
        public override void Run()
        {
            this.Send(":RUN");
            WaitOpc();
        }
        /// <summary>
        /// 单次扫描
        /// </summary>
        public override void Single(int time)
        {
            this.Send(":SINGle");
            WaitOpc(time);
        }
        public override void Single()
        {
            this.Send(":SINGle");
        }
        public override bool ReadAcquisitionDoneEvent
        {
            get
            {
                double dd = this.QueryNumber(":ADER?");//:PDER?
                if (dd > 0)
                {
                    return true;
                }
                else
                { return false; }
            }
        }
        /// <summary>
        /// 幅度调制，包络检波
        /// </summary>
        /// <param name="FunctionNumber"></param>
        /// <param name="ChannelNumber"></param>

        public override void AmplitudeDemodulation(int FunctionNumber, int ChannelNumber)
        {
            this.Send(string.Format(":FUNCtion{0}:ADEMod Channel{1}", FunctionNumber, ChannelNumber));
            this.Send(string.Format(":FUNCtion{0}:DISPlay 1", FunctionNumber));
            this.WaitOpc();
        }

        /// <summary>
        /// 测量时延差
        /// </summary>
        /// <param name="ref_1">参考</param>
        /// <param name="meas_1">测试</param>
        /// <returns></returns>
        public override void MeasureDeltaTime(SourceForMeas ref_1, SourceForMeas meas_1)
        {
            string reference = SourceForMeasSwitchToStr(ref_1);
            string measurement = SourceForMeasSwitchToStr(meas_1);
            this.Send(":MEASure:DELTatime:DEFine RISing,1,MIDDle,RISing,1,MIDDle");
            this.WaitOpc();
            this.Send(string.Format(":MEASure:DELTatime {0},{1}", reference, measurement));
            this.WaitOpc();

        }
        /// <summary>
        /// 测试指定迹线的时延差
        /// </summary>
        /// <param name="ref_1">参考迹线</param>
        /// <param name="startEdgeType">参考迹线沿类型</param>
        /// <param name="statEdegePosition">参考位置</param>
        /// <param name="statPulseNumber">参考脉冲数</param>
        /// <param name="meas_1">测试迹线</param>
        /// <param name="stopEdgeType">测试边沿类型</param>
        /// <param name="stopEdegePosition">测试迹线测试位置</param>
        /// <param name="stopPulseNumber">测试脉冲边缘数</param>
        public override void SetMeasureDeltaTime(SourceForMeas ref_1, TestEdgeType startEdgeType, TestEdgePosition statEdegePosition, int statPulseNumber,
            SourceForMeas meas_1, TestEdgeType stopEdgeType, TestEdgePosition stopEdegePosition, int stopPulseNumber)
        {
            string reference = SourceForMeasSwitchToStr(ref_1);
            string measurement = SourceForMeasSwitchToStr(meas_1);
            string refname = "";
            string measname = "";
            if (startEdgeType == TestEdgeType.FALLing)
            {
                refname = "-" + statPulseNumber;
                statEdegePosition = TestEdgePosition.LOWer;
            }
            else
            {
                refname = "+" + statPulseNumber;
                statEdegePosition = TestEdgePosition.UPPer;
            }
            if (stopEdgeType == TestEdgeType.FALLing)
            {
                measname = "-" + stopPulseNumber;
                stopEdegePosition = TestEdgePosition.LOWer;
            }
            else
            {
                measname = "+" + stopPulseNumber;
                stopEdegePosition = TestEdgePosition.UPPer;
            }

            this.Send(string.Format(":MEASure:DEFine DELay,{0},{1}", refname, measname));
            this.WaitOpc();
            this.Send(string.Format(":MEASure:DEFine THResholds,{0},{1}", statEdegePosition, stopEdegePosition));
            this.WaitOpc();
            this.Send(string.Format(":MEASure:DELTatime {0},{1}", reference, measurement));
            this.WaitOpc();

        }
        //
        public override void setChannelDelay(int channel, double time)
        {
            this.Send(string.Format(":CHANnel{0}:PROBe:SKEW {1}", channel, time));
        }
        /// <summary>
        /// 获取去时间间隔
        /// </summary>
        /// <param name="ref_1">参考迹线</param>
        /// <param name="meas_1">测试迹线</param>
        /// <returns></returns>
        public override double GetDeltaTime(SourceForMeas ref_1, SourceForMeas meas_1)
        {
            //this.Single();
            //this.WaitOpc();
            //System.Threading.Thread.Sleep(10000);
            double DeltaTime = base.QueryNumber(string.Format(":MEASure:DELTatime? {0},{1}", ref_1, meas_1));
            return DeltaTime;
        }
        /// <summary>
        /// 将通道变成string名称
        /// </summary>
        /// <param name="source_1">测试通道名称</param>
        /// <returns></returns>
        private string SourceForMeasSwitchToStr(SourceForMeas source_1)
        {
            string str = "Channel1";
            //str = source_1.ToString();
            switch (source_1)
            {
                case SourceForMeas.Channel1:
                    str = "CHANnel1";
                    break;
                case SourceForMeas.Channel2:
                    str = "CHANnel2";
                    break;
                case SourceForMeas.Channel3:
                    str = "CHANnel3";
                    break;
                case SourceForMeas.Channel4:
                    str = "CHANnel4";
                    break;
                case SourceForMeas.Function1:
                    str = "FUNCtion1";
                    break;
                case SourceForMeas.Function2:
                    str = "FUNCtion2";
                    break;
                case SourceForMeas.Function3:
                    str = "FUNCtion3";
                    break;
                default:
                    str = "FUNCtion4";
                    break;
            }
            return str;
        }
        /// <summary>
        /// 采样率
        /// </summary>
        public override double SampleRate
        {
            get
            {

                return base.QueryNumber(":ACQuire:SRATe:ANALog?");

            }
            set
            {
                base.SendNumber(":ACQuire:SRATe:ANALog", value);
                WaitOpc();
            }
        }
        /// <summary>
        /// 模拟带宽
        /// </summary>
        public override double BandWidth
        {
            get
            {

                return base.QueryNumber(":ACQuire:BANDwidth?");

            }
            set
            {
                base.SendNumber(":ACQuire:BANDwidth ", value);
                WaitOpc();
            }
        }
        /// <summary>
        /// 触发电平一半？
        /// </summary>
        public override void TriggerlevelFifty()
        {
            base.Send(":TRIGger:LEVel:FIFTy");
            WaitOpc();
        }


        /// <summary>
        /// 触发电平
        /// </summary>
        public override double TriggerLevel
        {
            get
            {

                return base.QueryNumber(":TRIGger:LEVel?");

            }
            set
            {
                base.Send(":TRIGger:LEVel CHANnel1," + value);          //根据白老师的建议，将每个通道的触发电平设置为一样
                WaitOpc();
                base.Send(":TRIGger:LEVel CHANnel2," + value);
                WaitOpc();
                base.Send(":TRIGger:LEVel CHANnel3," + value);
                WaitOpc();
                base.Send(":TRIGger:LEVel CHANnel4," + value);
                WaitOpc();
                base.Send(":TRIGger:LEVel AUX," + value);
                WaitOpc();
            }
        }
        /// <summary>
        /// 触发极性
        /// </summary>
        public override Polarity TriggerPolarity
        {
            get
            {
                return this.GetTriggerPolarity();
            }
            set
            {
                string str = PolaritySwitch(value);
                base.Send(":TRIGger:EDGE:SLOPe " + str);
                WaitOpc();
            }
        }
        /// <summary>
        /// 扫描类型
        /// </summary>
        public override SweepType TriggerSweep
        {
            get
            {
                return this.GetTriggerSweepType();
            }
            set
            {
                string str = SweepTypeSwitch(value);
                base.Send(":TRIGger:SWEep " + str);
                WaitOpc();
            }
        }
        /// <summary>
        /// 获取突发扫描类型
        /// </summary>
        private SweepType GetTriggerSweepType()
        {
            string string_1 = base.Query(":TRIGger:SWEep?");
            string str = string_1.ToUpper();
            if (str.Contains("AUTO"))    //改为包含
            {
                return SweepType.Auto;
            }
            return SweepType.Triggered;
        }
        private string SweepTypeSwitch(SweepType tmpType)
        {
            string str = "TRIGgered";
            switch (tmpType)
            {
                case SweepType.Auto:
                    str = "AUTO";
                    break;

                default:
                    str = "TRIGgered";
                    break;
            }
            return str;
        }
        /// <summary>
        /// 将触发极性的由枚举型转换为字符型
        /// </summary>
        /// <param name="tmpPolarity"></param>
        /// <returns></returns>
        private string PolaritySwitch(Polarity tmpPolarity)
        {
            string str = "POS";
            switch (tmpPolarity)
            {
                case Polarity.Negative:
                    str = "NEG";
                    break;

                default:
                    str = "POS";
                    break;
            }
            return str;
        }

        private Polarity GetTriggerPolarity()
        {
            string string_1 = base.Query(":TRIGger:EDGE:SLOPe?");
            string str = string_1.ToUpper();
            if (str.Contains("NEG"))    //改为包含
            {
                return Polarity.Negative;
            }
            return Polarity.Positive;
        }
        /// <summary>
        /// 定义将TriggerSource枚举类型变量转变为字符类型的函数
        /// </summary>
        /// <param name="source_1"></param>
        /// <returns></returns>
        private string TriggerSoureSwitch(TriggerSource source_1)
        {
            string str = "Channel1";
            //str = source_1.ToString();
            switch (source_1)
            {
                case TriggerSource.Channel1:
                    str = "CHANnel1";
                    break;
                case TriggerSource.Channel2:
                    str = "CHANnel2";
                    break;
                case TriggerSource.Channel3:
                    str = "CHANnel3";
                    break;
                case TriggerSource.Channel4:
                    str = "CHANnel4";
                    break;
                case TriggerSource.Aux:
                    str = "AUX";
                    break;
                default:
                    str = "LINE";
                    break;
            }
            return str;
        }
        public override TriggerSource Source
        {
            get
            {
                return this.GetTriggerSource();
            }
            set
            {
                string str = TriggerSoureSwitch(value);
                base.Send(":TRIGger:EDGE:SOURce " + str);
                WaitOpc();
            }
        }

        private TriggerSource GetTriggerSource()
        {
            string string_2 = base.Query(":TRIGger:EDGE:SOURce?");
            string str = string_2.ToUpper();
            if (str.Equals("CHAN1"))
            {
                return TriggerSource.Channel1;
            }
            if (str.Equals("CHAN2"))
            {
                return TriggerSource.Channel2;
            }
            if (str.Equals("CHAN3"))
            {
                return TriggerSource.Channel3;
            }
            if (str.Equals("CHAN4"))
            {
                return TriggerSource.Channel4;
            }
            if (str.Equals("AUX"))
            {
                return TriggerSource.Aux;
            }
            return TriggerSource.Line;
        }

        public override void SetInputImpedance(int Channel, double impedance)
        {
            if (impedance == 50)
                this.Send(string.Format(":CHANnel{0}:INPut DC50", Channel));
            else
                this.Send(string.Format(":CHANnel{0}:INPut ONEM", Channel));
        }

        #region
        public override void ClearAllMeasure()
        {
            this.Send(":MEASURE:CLEAR");
        }


        public override void MeasureFallTime(SourceForMeas meas_1)
        {
            this.Send(":MEASure:FALLTIME " + meas_1.ToString());
            this.WaitOpc();
        }
        public override double GetFallTme(SourceForMeas meas_1)
        {
            double fall = this.QueryNumber(":MEASURE:FALLTIME? " + meas_1.ToString());
            return fall;
        }


        public override void MeasureOverShoot(SourceForMeas meas_1)
        {
            this.Send(":MEASure:VOVershoot " + meas_1.ToString());
            this.WaitOpc();
        }

        public override double GetOverShotData(SourceForMeas meas_1)
        {
            double VOVershoot = this.QueryNumber(":MEASURE:VOVershoot? " + meas_1.ToString());
            return VOVershoot;
        }


        public override double GetRiseTimeData(SourceForMeas meas_1)
        {
            double shot = this.QueryNumber(":MEASURE:RiseTimeData? " + meas_1.ToString());
            return shot;
        }
        public override void MeasureRiseTime(SourceForMeas meas_1)
        {
            this.Send(":MEASure:RISetime " + meas_1.ToString());
            this.WaitOpc();
        }

        public override void MeasureVMax(SourceForMeas meas_1)
        {
            this.Send(":MEASure:VMAX " + meas_1.ToString());
            this.WaitOpc();
        }


        public override void MeasureVTop(SourceForMeas meas_1)
        {
            this.Send(":MEASure:VTOP " + meas_1.ToString());
            this.WaitOpc();
        }

        public override double GetVMaxData(SourceForMeas meas_1)
        {
            double max = this.QueryNumber(":MEASURE:VMAX? " + meas_1.ToString());
            return max;
        }

        public override double GetVTopData(SourceForMeas meas_1)
        {
            double vtop = this.QueryNumber(":MEASURE:VTOP? " + meas_1.ToString());
            return vtop;
        }
        #endregion

    }
}
