using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    internal class AnritsuML2496 : PowerMeter
    {
        public AnritsuML2496(string inAddress)
            : base(inAddress)
        {
        }

        /// <summary>
        /// 获得仪器的型号，序号和固件版本
        /// </summary>
        protected override void DetermineIdentity()
        {
            //这里需要处理一下；用于给base.m_model base.m_serial base.m_firmwareVersion三个赋值即可
            base.m_identity = this.Query("*IDN?");
            char[] separator = new char[] { ',' };
            //测试代码
            base.m_model = "未知";
            base.m_serial = "未知";
            base.m_firmwareVersion = "未知";
            //测试代码
            //string[] strArrays = base.Identity.Split(separator);

            //base.m_Manufactor = strArrays[0].Trim();
            //if (strArrays.Length > 1)
            //{
            //    base.m_model = strArrays[1].Trim();
            //}
            //if (strArrays.Length > 2)
            //{
            //    base.m_serial = strArrays[2].Trim();
            //}
            //if (strArrays.Length > 3)
            //{
            //    base.m_firmwareVersion = strArrays[3].Trim();
            //}
        }

        /// <summary>
        /// 决定仪器的选件
        /// </summary>
        protected override void DetermineOptions()
        {
            //处理一下，想办法给base.m_options赋值
            //base.m_options = this.Query("*OPT?");
            /**测试代码**/
            base.m_options = "未知";
            /**测试代码**/
        }

        public override void AutoGate(int inChannelNum, int GateCode)
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            //string[] str = new string[] { "SENS", inChannelNum.ToString(), ":SWE", GateCode.ToString(), ":AUTO ONCE" };
            //base.SendOpc(string.Concat(str), 5);
        }

        public override void AutoScale(int inChannelID)
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            //base.SendOpc(string.Concat("SENSe", inChannelID.ToString(), ":TRACe:AUToscale"), 5);
        }

        public override void CalPowerMeter(int SensorNumber)
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            // throw new NotImplementedException();原有驱动存在问题，注释掉。
            if(SensorNumber==1)
            {
                base.Send("SNCAL A");
                this.WaitOpc(100000);
            }
            else if (SensorNumber==2)
            {
                base.Send("SNCAL B");
                this.WaitOpc(100000);
            }

        }
        public override void ZeroPowerMeter(int SensorNumber)
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            //throw new NotImplementedException();原有驱动存在问题，注释掉。
            if (SensorNumber == 1)
            {
                base.Send("SNZERO A");
                this.WaitOpc(100000);
            }
            else if (SensorNumber == 2)
            {
                base.Send("SNZERO B");
                this.WaitOpc(100000);
            }
        }

        public override void ZeroAndCalPowerMeter(int SensorNumber)
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            ZeroPowerMeter(SensorNumber);
            this.WaitOpc();
            CalPowerMeter(SensorNumber);
            this.WaitOpc();
        }




        public override double MeasurePower(double m_Frequency, int SensorNumber)
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            this.WaitOpc();
            double num = 0;
            string SensorString = "";
            if (SensorNumber == 1)
            {
                SensorString = "A";
            }
            else if (SensorNumber == 2)
            {
                SensorString = "B";
            }

            
            //base.Send("CHOLD "+ SensorNumber.ToString()+" ON");
            
            base.Send("SNCFRQ " + SensorString +","+ m_Frequency.ToString());
            this.WaitOpc();
            
            try
            {
                num = base.QueryNumber("CWON 2," + " 1");
                this.WaitOpc();
            }
            catch
            {
                num = -999;
            }
            return num;
        }

        public override void MReceiverSystemConfigure()
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            this.WaitOpc();
            //base.SendOpc("SYST:COMM:LAN:ADDR '192.168.100.2'", 100);
            //base.SendOpc("SYST:COMM:LAN:SMAS '255.255.255.0'", 100);
            //base.SendOpc(":CALibration:ALL", 1000);
            //Thread.Sleep(5000);
        }

        public override void OffsetSwitch(int GateCode, bool IsOffsetOn)
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            this.WaitOpc();
            string SensorString = "";
            if (GateCode == 1)
            { SensorString = "A"; }
            else if (GateCode == 2)
            { SensorString = "B"; }
            else
            {
                throw new Exception("错误的sensor编号");
            }
            base.Send(string.Concat("SNOFIX ", SensorString, ",0"));
            //if (!IsOffsetOn)
            //{
            //    base.SendOpc(string.Concat("CALC", GateCode.ToString(), ":GAIN:STAT OFF"), 10);
            //}
            //else
            //{
            //    base.SendOpc(string.Concat("CALC", GateCode.ToString(), ":GAIN:STAT ON"), 10);
            //}
        }

        public override void Reset()
        {

            base.Send("NVAPN 1");
            this.WaitOpc();
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            this.WaitOpc();
        }

        public override double ReturnMeasValue(int SensorNumber)
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            this.WaitOpc();
            double num;
            double num1 = 0;
            try
            {
                num = Convert.ToDouble(base.Query("CWON 2,"+ " 1"));
                this.WaitOpc();
            }
            catch
            {
                double num2 = -999;
                num1 = num2;
                num = num2;
            }
            return num;
        }

        //public override void SetAcquisitionMode(int TunnelCode, bool IsContinuousTrigOn)
        //{
        //    if (!IsContinuousTrigOn)
        //    {
        //        base.SendOpc(string.Concat("INIT", TunnelCode.ToString(), ":CONT OFF"), 10);
        //    }
        //    else
        //    {
        //        base.SendOpc(string.Concat("INIT", TunnelCode.ToString(), ":CONT ON"), 10);
        //    }
        //}

        //public override void SetDispType(int WindowCode, PowerMeter.DispType DisplayTypeCode)
        //{
        //    base.Send(string.Concat("DISP:WIND", WindowCode.ToString(), ":FORM ", DisplayTypeCode.ToString()));
        //}

        public override void SetMeasAverageState(int SensorCode, bool AverageState)
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            this.WaitOpc();
            if (AverageState)
            { base.Send("CWAVG " + SensorCode.ToString() +", AUTO");
            this.WaitOpc();
            }//默认自动品均方式
            else
            { base.Send("CWAVG " + SensorCode.ToString() +", OFF");
            this.WaitOpc();
            }
        }

        public override void SetMeasAutoAverageMode(int SensorCode, bool AverageAutoState)
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            this.WaitOpc();
            if (AverageAutoState)
            { base.Send("CWAVG " + SensorCode.ToString() + ", AUTO");
            this.WaitOpc();
            }//默认自动品均方式
            else
            { base.Send("CWAVG " + SensorCode.ToString() + ", OFF");
            this.WaitOpc();
            }
        }

        public override void SetMeasAverageFactor(int SensorCode, int AvgFactor)
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            this.WaitOpc();
            base.Send("CWAVG " + SensorCode.ToString() + ", AUTO, "+ AvgFactor.ToString());
            this.WaitOpc();
        }

        public override void SetFrequence(int GateCode, double FreqValue)
        {
            this.Send("CHACTIV 2");//以channel 2的类型，数字类型读数
            this.WaitOpc();
            string SensorString = "";
            if (GateCode == 1)
            {
                SensorString = "A";
            }
            else if (GateCode == 2)
            {
                SensorString = "B";
            }

            base.Send("SNCFRQ " + SensorString+"," + FreqValue.ToString());
            this.WaitOpc();
            ////  string[] str = new string[] { "SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()};
            //base.Send(string.Concat("SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()));

            ////string[] str = new string[] { "SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()};
            //base.Send(string.Concat("SENSe", GateCode.ToString(), ":FREQ ", FreqValue.ToString()));
        }

        //public override void SetGate(int TunnelCode, int GateCode, double GateStartTime, double GateTimeLength)
        //{
        //    string[] str = new string[] { "SENS", TunnelCode.ToString(), ":SWE", GateCode.ToString(), ":OFFS:TIME ", GateStartTime.ToString() };
        //    base.SendOpc(string.Concat(str), 10);
        //    str = new string[] { "SENS", TunnelCode.ToString(), ":SWE", GateCode.ToString(), ":TIME ", GateTimeLength.ToString() };
        //    base.SendOpc(string.Concat(str), 10);
        //}

        //public override void SetMeasType(int TunnelCode, int GateCode, PowerMeter.MeasType MeasurementTypeCode)
        //{
        //    object[] str = new object[] { "CALC1:FEED", TunnelCode.ToString(), " \"POW:", MeasurementTypeCode, " ON SWEEP", GateCode.ToString(), "\"" };
        //    base.SendOpc(string.Concat(str), 10);
        //}

        public override void SetOffset(int GateCode, double OffsetValue)
        {
            string SensorString = "";
            if (GateCode == 1)
            {SensorString="A"; }
            else if(GateCode == 2)
            {SensorString="B";}
                else
            {
                throw new Exception("错误的sensor编号");
                }
            base.Send(string.Concat("SNOFIX ", SensorString,",",OffsetValue.ToString()));
        }

        //public override void SetScreenFormat(PowerMeter.ScreenFormat ScrnFmt)
        //{
        //    base.Send(string.Concat("DISP:SCReen:FORM ", ScrnFmt.ToString()));
        //}

        //public override void SetTraceX(int TunnelCode, double XTimeStart, double XTimeLength)
        //{
        //    base.SendOpc(string.Concat("SENSe", TunnelCode.ToString(), ":TRACe:OFFSet:TIME ", XTimeStart.ToString()), 5);
        //    base.SendOpc(string.Concat("SENSe", TunnelCode.ToString(), ":TRACe:TIME ", XTimeLength.ToString()), 5);
        //}

        //public override void SetTrigEdge(PowerMeter.TrigEdge EdgeCode)
        //{
        //    base.SendOpc(string.Concat("TRIG:SEQ:SLOP ", EdgeCode.ToString()), 10);
        //}

        //public override void SetTrigSource(int TunnelCode, PowerMeter.TrigSourMode TrigSourCode)
        //{
        //    base.SendOpc(string.Concat("TRIG", TunnelCode.ToString(), ":SOUR ", TrigSourCode.ToString()), 10);
        //}

        public override void RefSourceEnable(bool value)
        {
            //this.Send(string.Concat("OUTP:ROSC:STAT ", (value ? "ON" : "OFF")));
            //暂时未找到对应的命令，留空待补充0509 SYH
        }

        public override string[] SensorType
        {
            get
            {
                string[] tmp = new string[2];
                tmp[0] = "MA2473D";
                return tmp;
            }
        }

        public override int ChnsCount
        {
            get
            {
                return 2;
            }
        }

        public override void ReturnSensorInfo(int TunnelCode, out SensorInfo m_SensorInfo)
        {
            m_SensorInfo = new SensorInfo();
            m_SensorInfo.m_Type = "QueryFailed";
            m_SensorInfo.m_SN = "QueryFailed";
            m_SensorInfo.m_PowerRangeMax = "QueryFailed";
            m_SensorInfo.m_PowerRangeMin = "QueryFailed";
            m_SensorInfo.m_FreqRangeMax = "QueryFailed";
            m_SensorInfo.m_FreqRangeMin = "QueryFailed";
            string channelGate = "";
            if (TunnelCode == 1)
            {
                channelGate = "A";
            }
            else if (TunnelCode == 2)
            {
                channelGate = "B";
            }
            else{throw new Exception("错误的探头编号！");}

            try
            { m_SensorInfo.m_Type = this.Query("SNTYPE " + channelGate);
            this.WaitOpc();
            }
            catch
            { m_SensorInfo.m_Type = "QueryFailed";
                m_SensorInfo.m_SN = "QueryFailed";
            
            }

            string[] typeAndSN = m_SensorInfo.m_Type.Split(',');
            m_SensorInfo.m_Type = typeAndSN[0];
            m_SensorInfo.m_SN = typeAndSN[1];
            
            if (m_SensorInfo.m_Type == "E4413A")
            {
                m_SensorInfo.m_PowerRangeMax = "20";
                m_SensorInfo.m_PowerRangeMin = "-70";
                m_SensorInfo.m_FreqRangeMax = "26.5e9";
                m_SensorInfo.m_FreqRangeMin = "50e6";
            }
            else if (m_SensorInfo.m_Type == "N8481B")
            {
                m_SensorInfo.m_PowerRangeMax = "44";
                m_SensorInfo.m_PowerRangeMin = "-5";
                m_SensorInfo.m_FreqRangeMax = "18e9";
                m_SensorInfo.m_FreqRangeMin = "10e6";
            }
            else if (m_SensorInfo.m_Type=="MA2473D")
            {
                m_SensorInfo.m_PowerRangeMax = "20";
                m_SensorInfo.m_PowerRangeMin = "-70";
                m_SensorInfo.m_FreqRangeMax = "32e9";
                m_SensorInfo.m_FreqRangeMin = "10e6";

            }
        }
    }
}
