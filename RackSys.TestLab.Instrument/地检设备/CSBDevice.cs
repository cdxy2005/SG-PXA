#region Author & Version
/* =============================================
 * Copyright @ 2013 北京中创锐科技术有限公司 
 * 名    称：CSBDevice 
 * 功    能：CSBDevice 
 * 作    者：Administrator 
 * 添加时间：2015-08-07 10:54:02 
 * =============================================*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace RackSys.TestLab.Instrument
{

    public abstract  class CSBDevice : ScpiInstrument
    {
        /// <summary>
        /// 脉冲选型 
        /// </summary>
        public enum PlusTypes
        {
            enum12V=12,
            enum28V=28,
            enum_28V=-28

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="inAddress">visa地址</param>
        public CSBDevice(string inAddress)
            : base(inAddress, true)
        {

        }

        //
        public static CSBDevice Connect(string inAddress)
        {
            try
            {
                RAC_CSBDevice obj = new RAC_CSBDevice(inAddress);
                if (obj.IO != null)
                {
                    obj.IO.Connect();
                }
                else
                {
                    throw new Exception(string.Format("无法建立连接，连接参数：{0}", inAddress));
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 设备识别方法的重写
        protected override void DetermineIdentity()
        {
            string sysName = string.Empty;
            string copyRight = string.Empty;
            base.m_identity = this.Query("*IDN?");
            char[] separator = new char[] { ',' };
            string[] strArrays = base.Identity.Split(separator);

            base.m_Manufactor = strArrays[0].Trim();
            if (strArrays.Length > 1)
            {
                base.m_model = strArrays[1].Trim();
            }
            if (strArrays.Length > 2)
            {
                base.m_serial = strArrays[2].Trim();
            }
            if (strArrays.Length > 3)
            {
                base.m_firmwareVersion = strArrays[3].Trim();
            }
            if (strArrays.Length > 4)
            {
                sysName = strArrays[4].Trim();
            }
            if (strArrays.Length > 5)
            {
                copyRight = strArrays[5].Trim();
            }

            //SysParams.SysCfg.DeviceInfo.CompanyInfo = this.m_Manufactor;
            //SysParams.SysCfg.DeviceInfo.CopyRight = copyRight;
            //SysParams.SysCfg.DeviceInfo.HardwareSn = this.m_serial;
            //SysParams.SysCfg.DeviceInfo.HardwareVer = this.m_model;
            //SysParams.SysCfg.DeviceInfo.SoftVer = this.m_firmwareVersion;
            //SysParams.SysCfg.DeviceInfo.SysName = sysName;
            //SysParams.SysCfg.DeviceInfo.StationName =
            //SysParams.SysCfg.LogManager.GetStationName(this.m_serial);

            ////读取log的路径
            //SysParams.SysCfg.SysInfo.FtpLogDir = this.QueryLogPath();
        }

        protected override void DetermineOptions()
        {

        }
        #endregion

        #region 系统

        #region 主备份切换
        ///// <summary>
        ///// 主备份切换
        ///// </summary>
        ///// <param name="devicePrimaryBackType">切换类型</param>
        //public void ActiveDevicePrimaryBack(DevicePrimaryBackTypes devicePrimaryBackType)
        //{
        //    if (DevicePrimaryBackTypes.Primary == devicePrimaryBackType)
        //    {
        //        //激活主份
        //        this.Query("*CSB_AB:A");
        //    }
        //    else
        //    {
        //        //激活备份
        //        this.Query("*CSB_AB:B");
        //    }
        //}
        ///// <summary>
        ///// 读取当前主备份状态
        ///// </summary>
        //public DevicePrimaryBackTypes QueryPrimaryBackState()
        //{
        //    string result = this.Query("*CSB_AB?").Trim();
        //    return (DevicePrimaryBackTypes)Enum.Parse(typeof(DevicePrimaryBackTypes), result);
        //}
        #endregion

        #region 时钟速率设置
        /// <summary>
        /// 设置时钟速率
        /// </summary>
        /// <param name="rate">速率</param>
        public void SetClockRate(double rate)
        {
            this.Query(string.Format(
                "*Fre:{0}",
                rate.ToString()
                ));
        }
        /// <summary>
        /// 读取时钟速率
        /// </summary>
        public double QueryClockRate()
        {
            return
                double.Parse(this.Query("*Fre?").Trim());
        }
        #endregion

        #region 连续采集周期
        /// <summary>
        /// 获取CSB读取间隔
        /// </summary>
        public double QueryCSBInterval()
        {
            return double.Parse(
                this.Query("*CSB_InterVal?").Trim()
                );
        }
        /// <summary>
        /// 设置CSB间隔
        /// </summary>
        public void SetCSBInterval(double interval)
        {
            this.Query(
                string.Format("*CSB_InterVal:{0}",
                interval.ToString())
                );
        }
        #endregion

        #region FGM档位上限
        /// <summary>
        /// FGM档位上限
        /// </summary>
        public byte QueryFGMGears()
        {
            return byte.Parse(this.Query("*FGM_Gears?").Trim());
        }

        public void FGMGear(byte va)
        {
            this.Query(
                string.Format("*FGM_Gears:{0}",
                va.ToString())
                );
        }
        #endregion

        #region ALC档位上限
        /// <summary>
        /// ALC档位上限
        /// </summary>
        public byte QueryALCGears()
        {
            return byte.Parse(this.Query("*ALC_Gears?").Trim());
        }

        public void ALCGear(byte va)
        {
            this.Query(
                string.Format("*ALC_Gears:{0}",
                va.ToString())
                );
        }
        #endregion

        #region OPA档位上限
        /// <summary>
        /// OPA档位上限
        /// </summary>
        public byte QueryOPAGears()
        {
            return byte.Parse(this.Query("*OPA_Gears?").Trim());
        }

        public void OPAGear(byte va)
        {
            this.Query(
                string.Format("*OPA_Gears:{0}",
                va.ToString())
                );
        }
        #endregion

        #region 地址
        /// <summary>
        /// OPA档位上限
        /// </summary>
        public byte QueryCSBAddr()
        {
            return byte.Parse(this.Query("*CSB_Addr?").Trim());
        }

        public void CSBAddr(byte va)
        {
            this.Query(
                string.Format("*CSB_Addr:{0}",
                va.ToString())
                );
        }
        #endregion

        #region 事件配置
        ///// <summary>
        ///// 事件配置
        ///// </summary>
        //public void QueryCSBEventCfg(EventTypes evts, out bool isenable, out double upper, out double lower)
        //{
        //    string result = this.Query("*CSB_Event?:" + evts.ToString()).Trim();
        //    string[] arr = result.Split(new char[] { ',', '，' });
        //    isenable = bool.Parse(arr[0].Trim());
        //    upper = double.Parse(arr[1].Trim());
        //    lower = double.Parse(arr[2].Trim());
        //}

        //public void CSBEventCfg(EventTypes evts, bool isenable, double upper, double lower)
        //{
        //    this.Query(
        //        string.Format("*CSB_Event:{0}:{1}:{2}:{3}",
        //        evts.ToString(),
        //        isenable.ToString(),
        //        upper.ToString(),
        //        lower.ToString()
        //        )
        //        );
        //}
        #endregion

        #region 获取log记录的文件路径
        public string QueryLogPath()
        {
            return this.Query("*LogPath?");
        }
        #endregion

        #endregion

        #region CSB遥控

        #region FGM

        /// <summary>
        /// 加载FG命令
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="gain">增益值</param>
        /// <param name="parity">校验方式</param>
        public void LoadFG(byte addr, byte gain, Parity parity= Parity.Even)
        {
            this.Query(string.Format(
                "*FGM_Load:{0}:{1}:{2}",
                addr.ToString(),
                gain.ToString(),
                parity.ToString()
                ));

            string checkStr = parity == Parity.Even
                ? "偶校验"
                : "奇校验";
           // GlobalStatusReport.Report(string.Format("设置FGM--值：{0},校验方式：{1}", gain, checkStr));
        }

        /// <summary>
        /// 加载并执行FG命令
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="gain">增益值:0-2^7</param>
        /// <param name="isEvenCheck">奇偶校验</param>
        public void LoadAndExcuteFG(byte addr, byte gain, Parity parity= Parity.Even)
        {
            this.Query(string.Format(
                "*FGM_Load_Excute:{0}:{1}:{2}",
                addr.ToString(),
                gain.ToString(),
                parity.ToString()
                ));

            string checkStr = parity == Parity.Even
                ? "偶校验"
                : "奇校验";

          //  GlobalStatusReport.Report(string.Format("设置并执行FGM--值：{0},校验方式：{1}", gain, checkStr));
        }

        #endregion

        #region ALC
        /// <summary>
        /// 加载ALC命令
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="alcData">ALC命令数据</param>
        /// <param name="parity">校验方式</param>
        public void LoadALC(byte addr, byte alcData, Parity parity= Parity.Even)
        {
            this.Query(string.Format(
                "*ALC_Load:{0}:{1}:{2}",
                addr.ToString(),
                alcData.ToString(),
                parity.ToString()
                ));

            string checkStr = parity == Parity.Even
                ? "偶校验"
                : "奇校验";
           // GlobalStatusReport.Report(string.Format("设置ALC--值：{0},校验方式：{1}", alcData, checkStr));
        }

        /// <summary>
        /// 加载并执行ALC命令
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="alcData">alc数据</param>
        /// <param name="parity">奇偶校验</param>
        public void LoadAndExcuteALC(byte addr, byte alcData, Parity parity= Parity.Even)
        {
            this.Query(string.Format(
                "*ALC_Load_Excute:{0}:{1}:{2}",
                addr.ToString(),
                alcData.ToString(),
                parity.ToString()
                ));

            string checkStr = parity == Parity.Even
                ? "偶校验"
                : "奇校验";
           // GlobalStatusReport.Report(string.Format("设置并执行ALC--值：{0},校验方式：{1}", alcData, checkStr));
        }

        #endregion

        #region OPA设置
        /// <summary>
        /// 加载opa命令
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="opaData">opa命令数据</param>
        /// <param name="parity">校验方式</param>
        public void LoadOPA(byte addr, byte opaData, Parity parity= Parity.Even)
        {
            this.Query(string.Format(
                "*OPA_Load:{0}:{1}:{2}",
                addr.ToString(),
                opaData.ToString(),
                parity.ToString()
                ));

            string checkStr = parity == Parity.Even
                ? "偶校验"
                : "奇校验";
          //  GlobalStatusReport.Report(string.Format("设置OPA--值：{0},校验方式：{1}", opaData, checkStr));
        }

        #endregion

        #region 命令执行/命令禁止执行
        /// <summary>
        /// 执行命令-静默开关使能
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="isExcuteCmd">是否执行命令</param>
        /// <param name="isMuteOpen">静默开关是否开启</param>
        /// <param name="parity">校验方式</param>
        public void ExcuteCmd(byte addr, bool isExcuteCmd, bool isMuteOpen, bool isArcOpen, Parity parity= Parity.Even)
        {
            this.Query(string.Format(
                "*Excute:{0}:{1}:{2}:{3}:{4}",
                addr.ToString(),
                isExcuteCmd.ToString(),
                isMuteOpen.ToString(),
                isArcOpen.ToString(),
                parity.ToString()
                ));

            string mutexStr = isMuteOpen
                ? "开"
                : "关";
            string excuteStr = isExcuteCmd
                ? "开"
                : "关";
            string arcStr = isArcOpen
                ? "开"
                : "关";
            string checkStr = parity == Parity.Even
                ? "偶校验"
                : "奇校验";

          //  GlobalStatusReport.Report(string.Format("执行命令--ARC：{0}，静默：{1}，使能：{2}，校验：{3}",
          //      arcStr, mutexStr, excuteStr, checkStr));
        }

        /// <summary>
        /// 禁用命令
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="parity">校验方式</param>
        public void BanExcuteCmd(byte addr, Parity parity)
        {
            this.Query(string.Format(
                "*BanExcute:{0}:{1}",
                addr.ToString(),
                parity.ToString()
                ));

            string checkStr = parity == Parity.Even
                ? "偶校验"
                : "奇校验";
           // GlobalStatusReport.Report(string.Format("禁止命令--校验方式:{0}", checkStr));
        }
        #endregion

        #region 任意命令
        public void SetCSBCmd(ushort data)
        {
            this.Query(
                string.Format("*CSBCmd:{0}", data));

            string cmd = SetCmdStr(data);
          //  GlobalStatusReport.Report(string.Format("发送任意命令：{0}", cmd));
        }

        private string SetCmdStr(ushort cmdWord)
        {
            return String.Format("0x{0}", cmdWord.ToString("X").PadLeft(4, '0'));

        }
        #endregion

        /// <summary>
        /// 设置脉冲
        /// </summary>
        /// <param name="type">脉冲类型:+-28V，12V</param>
        /// <param name="chn">通道：1-8</param>
        /// <param name="pulseWidth">脉冲宽度1-1000ms</param>
        /// <param name="pulseGain">脉冲增益.0~12，或者10~32，-10~-32</param>
        public void SendPulse(PlusTypes type, int chn, double pulseWidth, double pulseGain)
        {
            this.Query(
                string.Format("*Pulse:{0}:{1}:{2}:{3}",
                type,
                chn,
                pulseWidth,
                pulseGain)
                );
            //string pulseName = string.Empty;
            //if (PlusTypes.enum12V == type)
            //{
            //    pulseName = "+12V脉冲";
            //}
            //else if (PlusTypes.enum28V == type)
            //{
            //    pulseName = "+28V脉冲";
            //}
            //else if (PlusTypes.enum_28V == type)
            //{
            //    pulseName = "-28V脉冲";
            //}

            //GlobalStatusReport.Report(string.Format("发送{0}:脉冲通道{1}，脉冲电平{2}，脉冲宽度{3}",
            //    pulseName,
            //    chn,
            //    pulseGain,
            //    pulseWidth));
        }


        //public void SendPulse12V( int chn, double pulseWidth, double pulseGain)
        //{
        //    this.SendPulse("enum12V",chn,pulseWidth,pulseGain);
        //}

        //public void SendPulse28V(int chn, double pulseWidth, double pulseGain)
        //{
        //    this.SendPulse("enum28V", chn, pulseWidth, pulseGain);
        //}

        //public void SendPulse_28V(int chn, double pulseWidth, double pulseGain)
        //{
        //    this.SendPulse("enum_28V", chn, pulseWidth, pulseGain);
        //}
        #endregion

        #region CSB遥测
        public string RequestCSB()
        {
            return this.Query("*Request_CSB?");
        }

        public string RequestCSB1()
        {
            return this.Query("*Request_CSB1?");
        }

        /// <summary>
        /// 启用CSB的连续采集
        /// </summary>
        public void EnableMonitorCSB(bool isEn)
        {
            this.Query(string.Format(
                "*En_Monitor_CSB:{0}",
                isEn
                ));
        }
        /// <summary>
        /// 启用CSB的连续采集
        /// </summary>
        public void EnableMonitorCSB1(bool isEn)
        {
            this.Query(string.Format(
                "*En_Monitor_CSB1:{0}",
                isEn
                ));
        }
        /// <summary>
        /// 查询CSB监控状态
        /// </summary>
        public bool MonitorStateCSB
        {
            get
            {
                return bool.Parse(this.Query("*Monitor_State_CSB?"));
            }
        }
        /// <summary>
        /// 查询CSB监控状态
        /// </summary>
        public bool MonitorStateCSB1
        {
            get
            {
                return bool.Parse(this.Query("*Monitor_State_CSB1?"));
            }
        }

        /// <summary>
        /// 获取CSB连续采集的缓存数据
        /// </summary>
        public string RequestCSBCacheData()
        {
            return this.Query("*Cache_CSB?");
        }
        /// <summary>
        /// 获取CSB1连续采集的缓存数据
        /// </summary>
        public string RequestCSB1CacheData()
        {
            return this.Query("*Cache_CSB1?");
        }

        /// <summary>
        /// 专为功率管项目添加
        /// </summary>
        /// <param name="fW">取枚举值 除10 取整</param>
        /// <param name="sW">取枚举值 除10 取余</param>
        /// <param name="res">返回字符串数组</param>
        public void Monitor_WORD(int fW,int sW,out string[] res)
        {
            string quryOrder = string.Format("*Monitor_Word{0}+Word{1}?",fW,sW);
            string Result = this.Query(quryOrder);
            //格式化字符串
            res  = Result.Split(new char[] { ',', '，' });
 
        }
        public void Monitor_WORD1_WORD1(out string result1, out string result2, out string addrResult, out string cmi,
            out string data, out string dataTile, out string checkBit)
        {
            string Result = this.Query("*Monitor_Word1+Word1?");
            //格式化字符串
            string[] arr = Result.Split(new char[] { ',', '，' });
            result1 = string.Empty;
            if (arr.Length > 0)
                result1 = arr[0];
            result2 = string.Empty;
            if (arr.Length > 1)
                result2 = arr[1];
            addrResult = string.Empty;
            if (arr.Length > 2)
                addrResult = arr[2];
            cmi = string.Empty;
            if (arr.Length > 3)
                cmi = arr[3];
            data = string.Empty;
            if (arr.Length > 4)
                data = arr[4];
            dataTile = string.Empty;
            if (arr.Length > 5)
                dataTile = arr[5];
            checkBit = string.Empty;
            if (arr.Length > 6)
                checkBit = arr[6];

        }

        public void Monitor_WORD2_WORD2(out string result1, out string result2, out string modle,
            out string gainDescription, out string gain, out string voltageDescription, out string voltage)
        {
            string Result = this.Query("*Monitor_Word2+Word2?");
            //格式化字符串
            string[] arr = Result.Split(new char[] { ',', '，' });
            result1 = string.Empty;
            if (arr.Length > 0)
                result1 = arr[0];
            result2 = string.Empty;
            if (arr.Length > 1)
                result2 = arr[1];
            modle = string.Empty;
            if (arr.Length > 2)
                modle = arr[2];
            gainDescription = string.Empty;
            if (arr.Length > 3)
                gainDescription = arr[3];
            gain = string.Empty;
            if (arr.Length > 4)
                gain = arr[4];
            voltageDescription = string.Empty;
            if (arr.Length > 5)
                voltageDescription = arr[5];
            voltage = string.Empty;
            if (arr.Length > 6)
                voltage = arr[6];
        }

        public void Monitor_WORD3_WORD3(out string result1, out string result2, out string voltage, out string arcState,
            out string mutexState, out string temperature)
        {
            string Result = this.Query("*Monitor_Word3+Word3?");
            //格式化字符串
            string[] arr = Result.Split(new char[] { ',', '，' });
            result1 = string.Empty;
            if (arr.Length > 0)
                result1 = arr[0];
            result2 = string.Empty;
            if (arr.Length > 1)
                result2 = arr[1];
            voltage = string.Empty;
            if (arr.Length > 2)
                voltage = arr[2];
            arcState = string.Empty;
            if (arr.Length > 3)
                arcState = arr[3];
            mutexState = string.Empty;
            if (arr.Length > 4)
                mutexState = arr[4];
            temperature = string.Empty;
            if (arr.Length > 5)
                temperature = arr[5];
        }

        public void Monitor_WORD4_WORD4(out string result1, out string result2, out string mainCurrent, out string screwCurrent)
        {
            string Result = this.Query("*Monitor_Word4+Word4?");
            //格式化字符串
            string[] arr = Result.Split(new char[] { ',', '，' });
            result1 = string.Empty;
            if (arr.Length > 0)
                result1 = arr[0];
            result2 = string.Empty;
            if (arr.Length > 1)
                result2 = arr[1];
            mainCurrent = string.Empty;
            if (arr.Length > 2)
                mainCurrent = arr[2];
            screwCurrent = string.Empty;
            if (arr.Length > 3)
                screwCurrent = arr[3];
        }

        public void Monitor_WORD5_WORD5(out string result1, out string result2, out string opa,
            out string ARCstate, out string mutexState)
        {
            string Result = this.Query("*Monitor_Word5+Word5?");
            //格式化字符串
            string[] arr = Result.Split(new char[] { ',', '，' });
            result1 = string.Empty;
            if (arr.Length > 0)
                result1 = arr[0];
            result2 = string.Empty;
            if (arr.Length > 1)
                result2 = arr[1];
            opa = string.Empty;
            if (arr.Length > 2)
                opa = arr[2];
            ARCstate = string.Empty;
            if (arr.Length > 3)
                ARCstate = arr[3];
            mutexState = string.Empty;
            if (arr.Length > 4)
                mutexState = arr[4];
        }

        public void Monitor_WORD1_WORD2(out string result1, out string addr, out string cmi, out string data, out string dataTile, out string checkBit,
            out string result2, out string modle, out string gainDescription, out string gain, out string voltageDescription, out string voltage)
        {
            string Result = this.Query("*Monitor_Word1+Word2?");
            //格式化字符串
            string[] arr = Result.Split(new char[] { ',', '，' });
            result1 = string.Empty;
            if (arr.Length > 0)
                result1 = arr[0];
            addr = string.Empty;
            if (arr.Length > 1)
                addr = arr[1];
            cmi = string.Empty;
            if (arr.Length > 2)
                cmi = arr[2];
            data = string.Empty;
            if (arr.Length > 3)
                data = arr[3];
            dataTile = string.Empty;
            if (arr.Length > 4)
                dataTile = arr[4];
            checkBit = string.Empty;
            if (arr.Length > 5)
                checkBit = arr[5];

            result2 = string.Empty;
            if (arr.Length > 6)
                result2 = arr[6];
            modle = string.Empty;
            if (arr.Length > 7)
                modle = arr[7];
            gainDescription = string.Empty;
            if (arr.Length > 8)
                gainDescription = arr[8];
            gain = string.Empty;
            if (arr.Length > 9)
                gain = arr[9];
            voltageDescription = string.Empty;
            if (arr.Length > 10)
                voltageDescription = arr[10];
            voltage = string.Empty;
            if (arr.Length > 11)
                voltage = arr[11];

        }

        public void Monitor_WORD3_WORD4(out string result1, out string voltage, out string arcState, out string mutexState, out string temperature,
            out string result2, out string mainCurrent, out string screwCurrent
            )
        {
            string Result = this.Query("*Monitor_Word3+Word4?");
            //格式化字符串
            string[] arr = Result.Split(new char[] { ',', '，' });
            result1 = string.Empty;
            if (arr.Length > 0)
                result1 = arr[0];
            voltage = string.Empty;
            if (arr.Length > 1)
                voltage = arr[1];
            arcState = string.Empty;
            if (arr.Length > 2)
                arcState = arr[2];
            mutexState = string.Empty;
            if (arr.Length > 3)
                mutexState = arr[3];
            temperature = string.Empty;
            if (arr.Length > 4)
                temperature = arr[4];
            result2 = string.Empty;
            if (arr.Length > 5)
                result2 = arr[5];
            mainCurrent = string.Empty;
            if (arr.Length > 6)
                mainCurrent = arr[6];
            screwCurrent = string.Empty;
            if (arr.Length > 7)
                screwCurrent = arr[7];
        }

        public void Monitor_WORD4_WORD5(out string result1, out string mainCurrent, out string screwCurrent,
            out string result2, out string opa, out string ARCstate, out string mutexState)
        {
            string Result = this.Query("*Monitor_Word4+Word5?");
            //格式化字符串
            string[] arr = Result.Split(new char[] { ',', '，' });

            result1 = string.Empty;
            if (arr.Length > 0)
                result1 = arr[0];
            mainCurrent = string.Empty;
            if (arr.Length > 1)
                mainCurrent = arr[1];
            screwCurrent = string.Empty;
            if (arr.Length > 2)
                screwCurrent = arr[2];

            result2 = string.Empty;
            if (arr.Length > 3)
                result2 = arr[3];
            opa = string.Empty;
            if (arr.Length > 4)
                opa = arr[4];
            ARCstate = string.Empty;
            if (arr.Length > 5)
                ARCstate = arr[5];
            mutexState = string.Empty;
            if (arr.Length > 6)
                mutexState = arr[6];
        }

        #endregion

        #region 脉冲遥控
        /// <summary>
        /// 获取脉冲参数
        /// </summary>
        /// <param name="pulseWidth12">12V脉冲宽度</param>
        /// <param name="pulseGain12">12V脉冲值</param>
        /// <param name="pulseWidth28">28V脉冲宽度</param>
        /// <param name="pulseGain28">28V脉冲值</param>
        /// <param name="pulseWidth_28">-28V脉冲宽度</param>
        /// <param name="pulseGain_28">-28V脉冲值</param>
        public void GetPulseParam(out double[] pulseWidth12, out double[] pulseGain12,
            out double[] pulseWidth28, out double[] pulseGain28,
            out double[] pulseWidth_28, out double[] pulseGain_28)
        {
            List<double> pulseWidth12Lst = new List<double>();
            List<double> pulseGain12Lst = new List<double>();
            List<double> pulseWidth28Lst = new List<double>();
            List<double> pulseGain28Lst = new List<double>();
            List<double> pulseWidth_28Lst = new List<double>();
            List<double> pulseGain_28Lst = new List<double>();

            //获取脉冲设置
            string result = this.Query("*Pulse？");
            string[] resultArr = result.Trim().Split(new char[] { ',', ';' });
            for (int i = 0; i < 24; i++)
            {
                double width = double.Parse(resultArr[2 * i]);     //脉宽
                double gain = double.Parse(resultArr[2 * i + 1]);     //幅度
                if (i < 8)
                {
                    //0-8:12V
                    pulseWidth12Lst.Add(width);
                    pulseGain12Lst.Add(gain);
                }
                else if (i >= 8 && i < 16)
                {
                    //8-16:28V
                    pulseWidth28Lst.Add(width);
                    pulseGain28Lst.Add(gain);
                }
                else
                {
                    //16-23:-28V
                    pulseWidth_28Lst.Add(width);
                    pulseGain_28Lst.Add(gain);
                }
            }
            //记录值
            pulseWidth12 = pulseWidth12Lst.ToArray();
            pulseGain12 = pulseGain12Lst.ToArray();
            pulseWidth28 = pulseWidth28Lst.ToArray();
            pulseGain28 = pulseGain28Lst.ToArray();
            pulseWidth_28 = pulseWidth_28Lst.ToArray();
            pulseGain_28 = pulseGain_28Lst.ToArray();
        }
        #endregion

        #region ADC遥测
        /// <summary>
        /// 查询ADC
        /// </summary>
        public List<double> RequestADC()
        {
            List<double> resultLst = new List<double>();
            string resultStr = this.Query("*Request_ADC?");
            resultStr = resultStr.Trim(new char[] { ' ', '\t', '\r', '\n', ';', '；' });
            string[] resultArr = resultStr.Split(',');
            foreach (string tmp in resultArr)
            {
                resultLst.Add(double.Parse(tmp.Trim()));
            }
            return resultLst;
        }
        /// <summary>
        /// 启用ADC连续采集
        /// </summary>
        public void EnableMonitorADC(bool isEn, int interval)
        {
            this.Query(string.Format("*En_Monitor_ADC:{0}:{1}", isEn, interval));
        }

        /// <summary>
        /// 连续采集读取
        /// </summary>
        /// <returns></returns>
        public List<double> RequestADCCacheData()
        {
            string resultStr = this.Query("*Cache_ADC?");
            if (string.IsNullOrEmpty(resultStr))
            {
                return null;
            }
            //拆分数据
            resultStr = resultStr.Trim(new char[] { ' ', '\t', '\r', '\n', ';', '；' });
            string[] resultArr = resultStr.Split(',');
            List<double> resultLst = new List<double>();
            foreach (string tmp in resultArr)
            {
                resultLst.Add(double.Parse(tmp.Trim()));
            }
            return resultLst;
        }

        #endregion

        public override string Query(string inQueryCmd)
        {
            return QueryWithoutLineFeed(inQueryCmd + "\r\n");
        }

        public override string QueryWithoutLineFeed(string inQueryCmd)
        {
            this.m_CommMutex.WaitOne();
            try
            {
                this.SendWithoutLineFeed(inQueryCmd);
#if SerialPort
                byte[] results;
                this.Read(out results);
                return System.Text.Encoding.Default.GetString(results);
#endif
                //Thread.Sleep(10);
               // return this.Read();
                return "";
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }
        }
    }
}
