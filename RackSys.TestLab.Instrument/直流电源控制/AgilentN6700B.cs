using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    internal class AgilentN6700B : DCPowerBase
    {
        public AgilentN6700B(string inAddress)
            : base(inAddress)
        {
            //string STR = "";
            //  STR=  this.Query("OUTP:COUP:CHAN?");
            
        }

        /// <summary>
        /// 读取电流
        /// </summary>
        /// <param name="inChannelId">通道号</param>
        /// <param name="outCurrentNow">电流</param>
        /// <param name="IsOK">是否成功</param>
        public override void Get_CurrentByChannelID(int inChannelId, out double outCurrentNow, out bool IsOK)
        {
            this.AssetChannelID(inChannelId);

            try
            {
                outCurrentNow = base.QueryNumber(string.Format("MEAS:CURR? (@{0})", new object[] { inChannelId }));
                IsOK = true;
            }
            catch
            {
                IsOK = false;
                outCurrentNow = 0;
            }
        }

        public override double Get_MaxCurrent(int ChannelID)
        {
            try
            {
             return   this.QueryNumber(string.Format("SENS:CURR:RANG? (@{0})",ChannelID));
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        /// <summary>
        /// 设置限流
        /// </summary>
        /// <param name="ChannelID">通道号</param>
        /// <param name="inCurrentLimit">电流</param>
        /// <param name="IsOK">是否成功</param>
        public override void Set_CurrentLimitByChannelID(int ChannelID, double inCurrentLimit, out bool IsOK)
        {
            AssetChannelID(ChannelID);
            IsOK = false;
            try
            {
                base.Send(string.Format("CURRent {0},(@{1})", new object[] { inCurrentLimit, ChannelID }));
                base.WaitOpc();

                    IsOK = true;
            }
            catch
            {
                IsOK = false;
            }
            //Thread.Sleep(500);
        }

        private void AssetChannelID(int ChannelID)
        {
            if ((ChannelID <= 0) || (ChannelID > 4))
            {
                throw new Exception("电源通道编号超界。");
            }
        }
        /// <summary>
        /// 设置输出
        /// </summary>
        /// <param name="ChannelID">通道号</param>
        /// <param name="inOnOffState">是否开启</param>
        /// <param name="IsOK">是否成功</param>
        public override void Set_OutputStateByChannelID(int ChannelID, bool inOnOffState, out bool IsOK)
        {
            string OutputStateStr = inOnOffState ? "ON" : "OFF";
            try
            {
                base.Send(string.Format("OUTPut {0},(@{1})", new object[] { OutputStateStr, ChannelID }));
                this.Query("*OPC?", 2000);
                IsOK = true;
            }
            catch
            {
                IsOK = false;
            }
        }
        /// <summary>
        /// 读取输出状态
        /// </summary>
        /// <param name="ChannelID">通道号</param>
        /// <returns>是否开启</returns>
        public override bool Get_OutputStateByChannelID(int ChannelID)
        {
            bool OutputOnOffState = false;
            try
            {
                if (base.QueryNumber(string.Format("OUTPut? (@{0})", new object[] { ChannelID })) == 1.0)
                {
                    OutputOnOffState = true;
                }
                else
                {
                    OutputOnOffState = false;
                }
            }
            catch
            {
                OutputOnOffState = false;
            }

            return OutputOnOffState;
        }

        /// <summary>
        /// 读取电压
        /// </summary>
        /// <param name="ChannelID">通道号</param>
        /// <param name="OutputVoltage">输出电压</param>
        /// <param name="IsOK">是否成功</param>
        public override void Get_OutputVoltageByChannelID(int ChannelID, out double OutputVoltage, out bool IsOK)
        {
            try
            {
                OutputVoltage = base.QueryNumber(string.Format("MEAS:VOLT? (@{0})", ChannelID));
                IsOK = true;
            }
            catch
            {
                IsOK = false;
                OutputVoltage = 0.0;
            }
        }

        public override double Get_MaxVoltage(int ChannelID)
        {
             try
            {
                double M_voltage = this.QueryNumber(string.Format("VOLT:RANG? (@{0})", ChannelID));
            return M_voltage;
            }
             catch (Exception ex)
             {
                 return 0.0;
             }
        }
        //CURRent:LIMit:NEGative[:IMMediate][:AMPLitude]? [MIN|MAX,] (@<chanlist>)

        /// <summary>
        /// 设置电压
        /// </summary>
        /// <param name="ChannelID">通道号</param>
        /// <param name="inOutputVoltage">输出电压</param>
        /// <param name="IsOK">是否成功</param>
        public override void Set_OutputVoltageByChannelID(int ChannelID, double inOutputVoltage, out bool IsOK)
        {
            try
            {
                base.Send(string.Format("VOLT {0},(@{1})", new object[] { inOutputVoltage, ChannelID }));
                base.WaitOpc();
                IsOK = true;
            }
            catch
            {
                IsOK = false;
            }
            //Thread.Sleep(500);
        }

        //该指令本电源没有
        public override void Get_IsOCPTripped(int ChannelID, out bool isOCP, out bool IsOK)
        {
            isOCP = false;

            ////AssertChannelID(ChannelID);
            //try
            //{
            //    int tmpTimeOut = base.Timeout;
            //    base.Timeout = 2000;
            //    double state = 0.0;
            //    try
            //    {
            //        state = base.QueryNumber("CURRent:PROTection:TRIPped?");
            //    }
            //    finally
            //    {
            //        base.Timeout = tmpTimeOut;
            //    }
            //    isOCP = state == 1;

            //    IsOK = true;
            //}
            //catch
            //{
            IsOK = false;
            //}
        }
        /// <summary>
        /// 清除过流保护
        /// </summary>
        /// <param name="ChannelID">通道号</param>
        public override void ClearOCPInfochan(int ChannelID)
        {
            try
            {
                int tmpTimeOut = base.Timeout;
                base.Timeout = 50000;
                base.Send(string.Format("OUTP:PROT:CLE (@{0})", ChannelID));
                base.Timeout = tmpTimeOut;
                //isOCP = state == 1;

                //IsOK = true;
            }
            catch
            {
                //IsOK = false;
            }
        }
        public override void ClearOCPInfo()
        {
            this.ClearOCPInfochan(1);
        }

        /// <summary>
        /// 读取当前通道模块类型
        /// </summary>
        /// <param name="ChannelID">通道号</param>
        /// <param name="IsConnectModel">是否连接模块</param>
        /// <param name="PowerModel">模块类型</param>
        public override void Get_OutputChannelByChannelID(int ChannelID,out bool IsConnectModel ,out string PowerModel)
        {
            PowerModel = null;
            try
            {
                PowerModel = base.Query(string.Format("SYST:CHAN:MOD? (@{0})", ChannelID));
                IsConnectModel = true;
            }
            catch
            {
                IsConnectModel = false;
            }
        }
        //public override void Get_IsOCPTripped(int ChannelID, out bool isOCP, out bool IsOK)
        //{
        //    throw new NotImplementedException();
        //}
    }

}
