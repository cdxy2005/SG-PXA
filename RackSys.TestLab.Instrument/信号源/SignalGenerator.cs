using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using System.Collections;

namespace RackSys.TestLab.Instrument
{
    public abstract class SignalGenerator : ScpiInstrument
    {
        protected static SignalGenerator.ValidateSupportDelegate m_validateSupportDelegate;

        protected static string m_FirmwareVersion;

        #region ALC控制相关部分

        public virtual double AlcBW
        {
            get
            {
                return base.QueryNumber(":POWer:ALC:BWIDth?");
            }
            set
            {
                this.Send(string.Concat(":POWer:ALC:BWIDth ", value));
            }
        }

        public virtual bool AlcBwAuto
        {
            get
            {
                return base.QueryNumber(":POWer:ALC:BWIDth:AUTO?") > 0;
            }
            set
            {
                this.Send(string.Concat(":POWer:ALC:BWIDth:AUTO ", (value ? "ON" : "OFF")));
            }
        }

        public virtual bool AlcEnabled
        {
            get
            {
                return base.QueryNumber(":POWer:ALC:STATe?") > 0;
            }
            set
            {
                if (value != this.AlcEnabled)
                {
                    this.Send(string.Concat(":POWer:ALC:STATe ", (value ? "ON" : "OFF")));
                }
            }
        }

        public virtual bool ALCHoldLineEnable
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        /// <summary>
        /// ALC hold线的控制，可以不用
        /// </summary>
        public virtual int ALCHoldRouting
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }


        /// <summary>
        /// 在Alc保持时才有效
        /// </summary>
        public virtual double AlcLevel
        {
            get
            {
                return base.QueryNumber(":POWer:ALC:LEVel?");
            }
            set
            {
                if (value != this.AlcLevel)
                {
                    base.SendNumber(":POWer:ALC:LEVel ", value);
                }
            }
        }

        #endregion

        /// <summary>
        /// RF输出使能
        /// </summary>
        public virtual bool RFOutputEnabled
        {
            get
            {
                return base.QueryNumber(":OUTPut:STATe?") > 0;
            }
            set
            {
                this.Send(string.Concat(":OUTPut:STATe ", (value ? "ON" : "OFF")));
                Thread.Sleep(1000);
            }
        }

        private double m_RFPowerLimitMax = 20;

        public double RFPowerLimitMax
        {
            get 
            { 
                return m_RFPowerLimitMax; 
            }
            set 
            { 
                m_RFPowerLimitMax = value;
                ///限制最大值
                //if (this.RFPower > this.m_RFPowerLimitMax)
                //{
                //    this.RFPower = this.m_RFPowerLimitMax;
                //}
            }
        }

        /// <summary>
        /// RF输出功率大小
        /// </summary>
        public virtual double RFPower
        {
            get
            {
                return base.QueryNumber(":SOURce:POWer:LEVel:IMMediate:AMPLitude?");
            }
            set
            {
                ///限制最大值低于某一阀值
                if (value > this.m_RFPowerLimitMax)
                {
                    value = this.m_RFPowerLimitMax;
                }

                if (value != this.RFPower)
                {
                    base.SendNumber(":SOURce:POWer:LEVel:IMMediate:AMPLitude ", value);
                }
            }
        }

        /// <summary>
        /// 是否不稳幅
        /// </summary>
        public virtual bool Unleveled
        {
            get
            {
                uint num = Convert.ToUInt32(this.Query(":STATus:QUEStionable:POWer:CONDition?"));
                return (num & 2) > 0;
            }
        }

        /// <summary>
        /// 幅度偏移量
        /// </summary>
        public virtual double AmplOffset
        {
            get
            {
                return base.QueryNumber(":SOURce:pow:offset?");
            }
            set
            {
                base.SendNumber(":SOURce:pow:offset ", value);
            }
        }
     

        /// <summary>
        /// 输出衰减保持
        /// </summary>
        public virtual bool AttenHold
        {
            get
            {
                return base.QueryNumber(":POWer:ATT:AUTO?") == 0;
            }
            set
            {
                if (value != this.AttenHold)
                {
                    this.Send(string.Concat(":POWer:ATT:AUTO ", (!value ? "ON" : "OFF")));
                }
            }
        }

        /// <summary>
        /// 输出衰减电平
        /// </summary>
        public virtual double AttenLevel
        {
            get
            {
                return base.QueryNumber(":POWer:ATT?");
            }
            set
            {
                if (value != this.AttenLevel)
                {
                    base.SendNumber(":POWer:ATT ", value);
                }
            }
        }

        public override string FirmwareVersion
        {
            get
            {
                return SignalGenerator.m_FirmwareVersion;
            }
        }

        /// <summary>
        /// 调制开关
        /// </summary>
        public virtual bool ModOutputEnabled
        {
            get
            {
                return base.QueryNumber(":OUTPut:MODulation?") > 0;
            }
            set
            {
                if (value != this.ModOutputEnabled)
                {
                    string powerSearchRef = this.PowerSearchRef;
                    this.Send(string.Concat(":OUTPut:MODulation ", (value ? "ON" : "OFF")));
                    Thread.Sleep(1000);
                    this.PowerSearchRef = powerSearchRef;
                    this.PowerSearchRef = powerSearchRef;
                }
            }
        }

        #region 功率搜索
        public virtual bool PowerSearchMode
        {
            get
            {
                return base.QueryNumber(":POWer:ALC:SEARch?") > 0;
            }
            set
            {
                if (value != this.PowerSearchMode)
                {
                    this.Send(string.Concat(":POWer:ALC:SEARch ", (value ? "ON" : "OFF")));
                }
            }
        }

        public virtual string PowerSearchRef
        {
            get
            {
                return this.Query(":POWer:ALC:SEARch:REFerence?");
            }
            set
            {
                this.Send(string.Concat(":POWer:ALC:SEARch:REFerence ", value));
            }
        }

        public virtual void ExecutePowerSearch()
        {
            bool powerSearchMode = this.PowerSearchMode;
            this.Send(":POWer:ALC:SEARch ONCE");
            this.Query("*OPC?");
            string errorQueue = base.GetErrorQueue();
            if (errorQueue != null)
            {
                while (errorQueue != null)
                {
                    errorQueue = base.GetErrorQueue();
                }
            }
            this.PowerSearchMode = powerSearchMode;
        }

        public virtual bool PowerSearchRf
        {
            get
            {
                return base.QueryNumber(":POWer:PROT?") > 0;
            }
            set
            {
                this.Send(string.Concat(":POWer:PROT ", (value ? "ON" : "OFF")));
            }
        }

        #endregion

        #region 脉冲调制
        public virtual bool PulseModulationEnabled
        {
            get
            {
                return base.QueryNumber(":SOURce:PULM:STATe?") > 0;
            }
            set
            {
                if (value != this.PulseModulationEnabled)
                {
                    this.Send(string.Concat(":SOURce:PULM:STATe ", (value ? "ON" : "OFF")));
                }
            }
        }

        /// <summary>
        /// 设置脉冲源：对内部ARB而言，设置为Internal
        /// </summary>
        public virtual string PulseModulationSource
        {
            get
            {
                return this.Query(":SOURce:PULM:SOURce?");
            }
            set
            {
                if (this.IsEVSGC() || this.IsPVSGC() || this.IsAnalogE8257D() || this.HasOption("UNU") || this.HasOption("UNW") || this.HasOption("UNS") || this.HasOption("1E6"))
                {
                    this.Send(string.Concat(":PULM:SOURce ", value));
                }
            }
        }

        /// <summary>
        /// 设置脉冲调制器使能信号的来源：M1，M2，M3，M4等。
        /// </summary>
        public virtual int PulseRouting
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        #endregion

        #region 10MHz参照时钟
        public virtual bool RFExternalReference
        {
            get
            {
                return this.Query(":ROSCillator:SOURCe?") == "EXT";
            }
        }

        #endregion


        #region 输出频率相关
        public virtual double RFFreqOffset
        {
            get
            {
                return base.QueryNumber(":SOURce:FREQuency:OFFset?");
            }
            set
            {
                base.SendNumber(":SOURce:FREQuency:OFFset ", value);
            }
        }

        public virtual double RFFrequency
        {
            get
            {
                return base.QueryNumber(":SOURce:FREQuency:FIXed?");
            }
            set
            {
                if (value != this.RFFrequency)
                {
                    base.SendNumber(":SOURce:FREQuency:FIXed ", value);
                }
            }
        }

        /// <summary>
        /// 档位
        /// </summary>
        public virtual short RFFrequencyBand
        {
            get
            {
                double rFFrequency = this.RFFrequency;
                if (this.Model == "E4438C")
                {
                    if (rFFrequency < 185000000)
                    {
                        return 0;
                    }
                    if (rFFrequency <= 250000000)
                    {
                        return 1;
                    }
                    return 2;
                }
                if (this.Model != "E8267C")
                {
                    return 0;
                }
                if (rFFrequency <= 250000000)
                {
                    return 0;
                }
                if (rFFrequency < 950000000)
                {
                    return 1;
                }
                if (rFFrequency <= 1732500000)
                {
                    return 2;
                }
                if (rFFrequency < 2012000000)
                {
                    return 3;
                }
                if (rFFrequency <= 2188200000)
                {
                    return 4;
                }
                if (rFFrequency <= 3200000000)
                {
                    return 5;
                }
                return 6;
            }
        }

        /// <summary>
        /// 最高频率范围
        /// </summary>
        public virtual double RFFrequencyMax
        {
            get
            {
                if (this.HasOption("544"))
                {
                    return 44000000000;
                }
                if (this.HasOption("532"))
                {
                    return 31800000000;
                }
                if (this.HasOption("520"))
                {
                    return 20000000000;
                }
                if (this.HasOption("506"))
                {
                    return 6000000000;
                }
                if (this.HasOption("504"))
                {
                    return 4000000000;
                }
                if (this.HasOption("503"))
                {
                    return 3000000000;
                }
                if (this.HasOption("502"))
                {
                    return 2000000000;
                }
                if (this.HasOption("501"))
                {
                    return 1000000000;
                }
                return 4000000000;
            }
        }

        #endregion

 

        static SignalGenerator()
        {
            SignalGenerator.m_validateSupportDelegate = null;
        }

        protected SignalGenerator(string address)
            : base(address)
        {
        }

        #region 信号源连接和创建等
        public static SignalGenerator Connect(string currentAddress, SignalGenerator.ValidateSupportDelegate supportDelegate, bool interactive)
        {
            SignalGenerator AnalogSignalGenerator = null;
            string str = (currentAddress != null ? currentAddress : "GPIB0::19::INSTR");
            SignalGenerator.m_validateSupportDelegate = supportDelegate;
            if (interactive)
            {
                throw new Exception("不支持交互模式");
            }
            try
            {
                string str1 = SignalGenerator.DetermineSupport(str);
                if (str1 != null)
                {
                    throw new Exception(str1);
                }
                AnalogSignalGenerator = SignalGenerator.CreateDetectedAnalogSignalGenerator(str);
            }
            catch
            {
                throw;
            }
            SignalGenerator.m_validateSupportDelegate = null;
            if (AnalogSignalGenerator != null)
            {
                AnalogSignalGenerator.Connected = true;
                SignalGenerator.m_FirmwareVersion = AnalogSignalGenerator.GetFirmwareVersion(AnalogSignalGenerator);
            }
            return AnalogSignalGenerator;
        }

        private string GetFirmwareVersion(SignalGenerator sg)
        {
            string str = sg.Query("*IDN?");
            char[] chrArray = new char[] { ',' };
            string str1 = str.Split(chrArray)[3];
            if (str1.Length == 0)
            {
                str1 = null;
            }
            return str1;
        }


        public static SignalGenerator CreateDetectedAnalogSignalGenerator(string address)
        {
            SignalGenerator PsgEsg;
            try
            {
                string str = ScpiInstrument.DetermineModel(address);
                if (SignalGenerator.IsEVSGC(str) || SignalGenerator.IsPVSG(str))
                {
                    PsgEsg = new AgilentEsgAndPsg(address);
                }
                else if (SignalGenerator.IsAnalogE8257D(str))
                {
                    ///E8257D的代码
                    PsgEsg = new AgilentE8257D(address);
                }
                else if (SignalGenerator.IsAV1464B(str))
                {
                    PsgEsg = new AV1464BSignalGenerator(address);
                }
                else if (!SignalGenerator.IsEVSGB(str))
                {
                    return null;
                }
                else
                {
                    PsgEsg = null;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("连接至模拟信号源出错: ", exception.Message));
            }
            return PsgEsg;
        }

        private static string DetermineSupport(string address)
        {
            if (VectorSignalGenerator.m_validateSupportDelegate == null)
            {
                return null;
            }
            SignalGenerator AnalogSignalGenerator = null;
            try
            {
                AnalogSignalGenerator = SignalGenerator.CreateDetectedAnalogSignalGenerator(address);
            }
            catch
            {
                throw;
            }
            if (AnalogSignalGenerator == null)
            {
                return "无法识别对应的模拟信号源";
            }
            return SignalGenerator.m_validateSupportDelegate(AnalogSignalGenerator);
        }
        
        #endregion

        protected override void DetermineOptions()
        {
            this.m_options = this.Query("DIAG:CPU:INFO:OPT?");
        }


        /// <summary>
        /// 列出各类目录下的文件等信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ListDictionary GetCatalog(SignalGenerator.CatalogType type)
        {
            string str = "WFM1";
            switch (type)
            {
                case SignalGenerator.CatalogType.Waveform:
                    {
                        str = "WFM1";
                        break;
                    }
                case SignalGenerator.CatalogType.NVWaveform:
                    {
                        str = "NVWFM";
                        break;
                    }
                case SignalGenerator.CatalogType.Binary:
                    {
                        str = "BINARY";
                        break;
                    }
            }
            return base.GetCatalog(str);
        }

        public virtual bool HasArb()
        {
            return false;
        }

        public virtual bool HasOption(string option)
        {
            bool flag = this.m_options.ToUpper().IndexOf(option.ToUpper()) != -1;
            if (!flag)
            {
                bool flag1 = true;
                try
                {
                    int.Parse(option);
                }
                catch
                {
                    flag1 = false;
                }
                if (flag1 && !this.IsPVSGC())
                {
                    flag = base.QueryNumber(string.Concat(":DIAG:INFO:WLIC? ", option)) != 0;
                }
            }
            return flag;
        }

        #region 型号识别
        public bool IsEVSGB()
        {
            return SignalGenerator.IsEVSGB(this.Model);
        }

        public static bool IsEVSGB(string model)
        {
            if (model == "E4430B" || model == "E4431B" || model == "E4432B" || model == "E4433B" || model == "E4434B" || model == "E4435B" || model == "E4436B" || model == "E4437B" || model == "ESG-D1000B" || model == "ESG-D2000B" || model == "ESG-D3000B")
            {
                return true;
            }
            return model == "ESG-D4000B";
        }

        public bool IsEVSGC()
        {
            return SignalGenerator.IsEVSGC(this.Model);
        }

        public static bool IsEVSGC(string model)
        {
            if (model == "E4438C")
            {
                return true;
            }
            return model == "N5182A";
        }

        public bool IsMxg()
        {
            return SignalGenerator.IsMxg(this.Model);
        }

        public static bool IsMxg(string model)
        {
            return model == "N5182A";
        }

        public bool IsPVSG()
        {
            return SignalGenerator.IsPVSG(this.Model);
        }

        public static bool IsPVSG(string model)
        {
            if (SignalGenerator.IsPVSGC(model))
            {
                return true;
            }
            return SignalGenerator.IsPVSGD(model);
        }

        public bool IsPVSGC()
        {
            return SignalGenerator.IsPVSGC(this.Model);
        }

        public static bool IsPVSGC(string model)
        {
            return model == "E8267C";
        }

        public bool IsPVSGD()
        {
            return SignalGenerator.IsPVSGD(this.Model);
        }

        public static bool IsPVSGD(string model)
        {
            if (model == "E8267D")
            {
                return true;
            }
            return model == "N8212A";
        }
        public bool IsAnalogSG()
        {
            return this.IsAnalogE8257D();
        }

        public bool IsAnalogE8257D()
        {
            return SignalGenerator.IsAnalogE8257D(this.Model);
        }
        /// <summary>
        /// 是否为PSG的模拟信号源
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsAnalogE8257D(string model)
        {
            if (model == "E8257D")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsAV1464B(string model)
        {
            if (model == "AV1464B")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsPVSGD_SI()
        {
            return SignalGenerator.IsPVSGD_SI(this.Model);
        }

        public static bool IsPVSGD_SI(string model)
        {
            return model == "N8212A";
        }
        
        #endregion

        #region 状态保存和读出
        public virtual void LoadState(byte sequence, byte register)
        {
            object[] objArray = new object[] { "*RCL ", register, ",", sequence };
            this.Send(string.Concat(objArray));
            this.Query("*OPC?", 60000);
            base.GetErrorQueue();

        }

        public void DeleteState(byte sequence, byte register)
        {
            object[] objArray = new object[] { ":MMEMory:DELete:NAME \"", sequence, "_", register.ToString("D2"), "\",\"STATE:\"" };
            this.Send(string.Concat(objArray));
            this.Query("*OPC?", 10000);
            base.GetErrorQueue();
        }

        public virtual void SaveState(byte sequence, byte register)
        {
            object[] objArray = new object[] { "*SAV ", register, ",", sequence };
            this.Send(string.Concat(objArray));
            this.Query("*OPC?", 60000);
        }
        
        #endregion

        public void Preset()
        {
            this.Send(":SYSTem:PRESet");
        }

        public override void GoToLocal()
        {
            base.Send(":SYSTem:COMMunicate:GTLocal");
        }


        public enum CatalogType
        {
            Waveform,
            NVWaveform,
            Binary
        }

        public delegate string ValidateSupportDelegate(SignalGenerator sg);




        //郝佳添加的代码 2013.1.22

        public virtual double MultitoneFreqSpace
        {
            get
            {
                return base.QueryNumber(":RADio:MTONe:ARB:SETup:TABLe:FSPacing?");
            }
            set
            {
                if (value != this.MultitoneFreqSpace)
                {
                    base.SendNumber(":RADio:MTONe:ARB:SETup:TABLe:FSPacing ", value);
                }
            }
        }


        public virtual double MultitoneCarryNum
        {
            get
            {
                return base.QueryNumber(":RADio:MTONe:ARB:SETup:TABLe:NTONes?");
            }
            set
            {
                if (value != this.MultitoneCarryNum)
                {
                    base.SendNumber(":RADio:MTONe:ARB:SETup:TABLe:NTONes ", value);
                }
            }
        }


        public virtual double MultitoneState
        //1 表示开启，0表示关闭
        {
            get
            {
                return base.QueryNumber(":RADio:MTONe:ARB?");
            }
            set
            {
                if (value != this.MultitoneState)
                {
                    base.SendNumber(":RADio:MTONe:ARB ", value);
                }
            }
        }


        public void LoadWaveForm(string m_SignalName)
        {
            this.Send(":SOURce:RADio:ARB:WAVeform 'WFM1:" + m_SignalName + "'");
        }

        public virtual bool ARBEnabled
        {
            get
            {
                return base.QueryNumber(":SOURce:RADio:ARB:STATe?") > 0;
            }
            set
            {
                this.Send(string.Concat(":SOURce:RADio:ARB:STATe ", (value ? "ON" : "OFF")));
                Thread.Sleep(100);
            }
        }





    }
}
