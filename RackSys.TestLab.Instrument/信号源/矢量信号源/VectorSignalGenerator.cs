using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using System.Collections;

namespace RackSys.TestLab.Instrument
{
    public abstract class VectorSignalGenerator : SignalGenerator
    {

        protected bool HasInternalArb
        {
            get
            {
                if (this.HasOption("001") || this.HasOption("002")
                    || this.HasOption("601") || this.HasOption("602")
                    || this.HasOption("651") || this.HasOption("652")
                    || this.HasOption("654"))
                {
                    return true;
                }
                return false;
            }
        }

        public virtual bool InternalArbEnabled
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        //矢量相关部分
        #region 矢量信号源部分接口
        /// <summary>
        /// 和Marker有关的部分，使用Marker来
        /// </summary>
        public virtual int AlternateAmplitudeRouting
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }


        #region IQ有关的操作


        /// <summary>
        /// 正交角度误差
        /// </summary>
        public virtual double QuadSkew
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public virtual bool IQAdjustments
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public abstract bool IQAttenAuto
        {
            get;
            set;
        }

        public abstract double IQAttenuator
        {
            get;
            set;
        }

        public double IQBandwidthMax
        {
            get
            {
                //仅考虑内部模式
                return 80e6;
            }
        }

        public virtual bool IQEnabled
        {
            get
            {
                return base.QueryNumber(":SOURce:DM:STATe?") > 0;
            }
            set
            {
                if (value != this.IQEnabled)
                {
                    string powerSearchRef = this.PowerSearchRef;
                    try
                    {
                        string str = value ? "ON" : "OFF";
                        this.Send(string.Concat(":SOURce:DM:STATe ", str));
                        this.Query("*OPC?", 10000);
                    }
                    finally
                    {
                        this.PowerSearchRef = powerSearchRef;
                        this.PowerSearchRef = powerSearchRef;
                    }
                }
            }
        }

        public abstract double IQIOffset
        {
            get;
            set;
        }

        public abstract double IQModFilter
        {
            get;
            set;
        }

        public abstract bool IQModFilterAuto
        {
            get;
            set;
        }

        public abstract double IQQOffset
        {
            get;
            set;
        }

        public virtual double IQSkew
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

        public bool RFBlankingEnabled
        {
            get
            {
                if (!this.HasInternalArb)
                {
                    return false;
                }
                return base.QueryNumber(":SOURce:RADio:ARB:MARKer:RFBLank?") > 0;
            }
            set
            {
                if (this.HasInternalArb)
                {
                    this.Send(string.Concat(":SOURce:RADio:ARB:MARKer:RFBLank ", (value ? "ON" : "OFF")));
                }
            }
        }
        #region 宽带相关
        /// <summary>
        /// 使用频率转换开关
        /// </summary>
        public bool UseShiftRFFilterSwitch
        {
            get
            {
                if (this.IsMxg())
                {
                    return false;
                }
                if ("WID" != this.Query(":OUTPut:MODulation:LOWBand:BWIDth?").ToUpper())
                {
                    return false;
                }
                return true;
            }
            set
            {
                if (!this.IsMxg())
                {
                    this.Send(string.Concat(":OUTPut:MODulation:LOWBand:BWIDth ", (value ? "WIDe" : "NORMal")));
                }
            }
        }

        public bool UseWidebandIQBelow3_2G
        {
            get
            {
                if (!this.HasOption("H18") && !this.HasOption("018"))
                {
                    return false;
                }
                if ("WIDE" != this.Query(":SOUR:FREQ:LBP?").ToUpper())
                {
                    return false;
                }
                return true;
            }
            set
            {
                if (this.HasOption("H18") || this.HasOption("018"))
                {
                    this.Send(string.Concat(":SOUR:FREQ:LBP ", (value ? "WIDE" : "NORM")));
                }
            }
        }

        #endregion

        public void PresetAndKeepWidebandSetting()
        {
            bool useWidebandIQBelow32G = this.UseWidebandIQBelow3_2G;
            bool useShiftRFFilterSwitch = this.UseShiftRFFilterSwitch;
            this.Preset();
            this.UseWidebandIQBelow3_2G = useWidebandIQBelow32G;
            this.UseShiftRFFilterSwitch = useShiftRFFilterSwitch;
        }

        #endregion

        protected VectorSignalGenerator(string address)
            : base(address)
        {
        }

         private string GetFirmwareVersion(VectorSignalGenerator sg)
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
         #region 信号源连接和创建等
         public static VectorSignalGenerator ConnectVecorSG(string currentAddress, SignalGenerator.ValidateSupportDelegate supportDelegate, bool interactive)
         {
             VectorSignalGenerator VectorSigGen = null;
             string str = (currentAddress != null ? currentAddress : "GPIB0::19::INSTR");
             SignalGenerator.m_validateSupportDelegate = supportDelegate;
             if (interactive)
             {
                 throw new Exception("不支持交互模式");
             }
             try
             {
                 string str1 = VectorSignalGenerator.DetermineSupport(str);
                 if (str1 != null)
                 {
                     throw new Exception(str1);
                 }
                 VectorSigGen = VectorSignalGenerator.CreateDetectedVectorSignalGenerator(str);
             }
             catch
             {
                 throw;
             }
             VectorSignalGenerator.m_validateSupportDelegate = null;
             if (VectorSigGen != null)
             {
                 VectorSigGen.Connected = true;
                 VectorSigGen.m_firmwareVersion = VectorSigGen.GetFirmwareVersion(VectorSigGen);
             }
             return VectorSigGen;
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


         public static VectorSignalGenerator CreateDetectedVectorSignalGenerator(string address)
         {
             VectorSignalGenerator PsgEsg;
             try
             {
                 string str = ScpiInstrument.DetermineModel(address);
                 if (string.IsNullOrEmpty(str))
                 {

                 }
                 if (VectorSignalGenerator.IsEVSGC(str) || VectorSignalGenerator.IsPVSG(str) || VectorSignalGenerator.IsMxg(str))
                 {
                     PsgEsg = new AgilentE8267D(address);
                 }
                 else
                 {
                     PsgEsg = null;
                 }
             }
             catch (Exception exception)
             {
                 throw new Exception(string.Concat("连接到矢量信号源出错: ", exception.Message));
             }
             return PsgEsg;
         }

         private static string DetermineSupport(string address)
         {
             if (SignalGenerator.m_validateSupportDelegate == null)
             {
                 return null;
             }
             VectorSignalGenerator VectorSignalGenerator = null;
             try
             {
                 VectorSignalGenerator = VectorSignalGenerator.CreateDetectedVectorSignalGenerator(address);
             }
             catch
             {
                 throw;
             }
             if (VectorSignalGenerator == null)
             {
                 return "无法识别信号源";
             }
             return SignalGenerator.m_validateSupportDelegate(VectorSignalGenerator);
         }

         #endregion

        /// <summary>
        /// 矢量信号源参数设置
        /// </summary>
        /// <param name="SGsetting"></param>
        /// <returns></returns>
         public virtual bool VectorSignalGeratorConfigure(VectorSignalGeneratorSetting SGsetting)
         {
             SGsetting = new VectorSignalGeneratorSetting();
             return false;
         
         
         }


        /// <summary>
        /// 矢量信号源参数读取
        /// </summary>
        /// <param name="SGsetting"></param>
        /// <param name="SGsettingResult"></param>
        /// <returns></returns>
         public virtual bool VectorSignalGeratorConfigurationRead(out VectorSignalGeneratorSetting SGsettingResult)
         {
             SGsettingResult = new VectorSignalGeneratorSetting();
             return false;
         }
         


     }
}
