using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using RackSys.TestLab.Visa;
using System.Threading;
namespace RackSys.TestLab.Instrument
{
    public class CtrlUnitTR29 : CtrlUnit
    {
        public CtrlUnitTR29(string address)
            : base(address)
        {
            this.Timeout = 30000;
        }

        protected override void DetermineOptions()
        {

        }
        protected override void DetermineIdentity()
        {
            this.Connected = true;
        }
        /// <summary>
        /// 系统命令 设置电平类型
        /// </summary>
        /// <param name="LevelType">TTL；LvTTL；LVDS</param>
        /// <returns></returns>
        public override string SetLevelType(string LevelType)
        {
            try
            {
                switch (LevelType)
                {
                    case "TTL": return this.Query("Level:1");
                    case "LVTTL": return this.Query("Level:2");
                    case "LVDS": return this.Query("Level:3");
                    default: return "ERROR";
                }
            }
            catch (Exception)
            {
                return "ERROR";
            }
        }

        /// <summary>
        /// 串口命令 设置时钟个数
        /// </summary>
        /// <param name="CLKNo">时钟个数范围：1~40 </param>
        /// <returns></returns>
        public override string SetCLKNo(int CLKNo)
        {
            try
            {
                if (CLKNo >= 10 && CLKNo <= 40)
                {
                    return this.Query("SetclkNO:" + Convert.ToString(CLKNo));
                }
                else if (CLKNo >= 1 && CLKNo <= 9)
                {
                    return this.Query("SetclkNO:0" + Convert.ToString(CLKNo));
                }
                else return "ERROR";

            }
            catch (Exception)
            {
                return "ERROR";
            }
        }

        /// <summary>
        /// 串口命令 设置时钟频率
        /// </summary>
        /// <param name="freq">频率范围 1~30 000 000 </param>
        /// <returns></returns>
        public override string SetFreq(double freq)
        {
            try
            {
                if (freq > 0 && freq <= 30 * 1e6)
                {
                    double X = 300000000 / (freq * 2) - 1;
                    string str = Convert.ToString((long)X, 16).PadLeft(8, '0').ToUpper();
                    str = "Frq:" + str;
                    return this.Query(str);
                }
                else return "ERROR";

            }
            catch (Exception)
            {
                return "ERROR";
            }
        }

        /// <summary>
        /// 串口设置 设置上升沿有效或者下降沿有效
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        public override string SetRisingEdgeEnable(bool enable)
        {
            try
            {
                if (enable)
                {
                    return this.Query("SETClk:1");
                }
                else return this.Query("SETClk:0");

            }
            catch (Exception)
            {
                return "ERROR";
            }
        }

        /// <summary>
        /// 设置串口DATA
        /// </summary>
        /// <param name="LineNo">2~10</param>
        /// <param name="Data">10位16进制数</param>
        /// <returns></returns>

        public override string SetSeriesData(int LineNo, string Data)
        {
            try
            {
                //string str1 = Convert.ToString((long)X, 16).PadLeft(2, '0').ToUpper();
                string str1 = Convert.ToString((long)LineNo, 16).PadLeft(2, '0').ToUpper();
                string str = "SetL" + str1 + ":" + Data;
                return this.Query(str);
            }
            catch (Exception)
            {
                return "ERROR";
            }

        }

        public override void TR_reset()
        {
            this.Query("TR_reset");
        }
        /// <summary>
        /// 设置串口通道开启和关闭
        /// </summary>
        /// <param name="Port">3位16进制数</param>
        /// <returns></returns>
        public override string SetSeriesPortEnable(string Port)
        {
            try
            {
                return this.Query("CEN:" + Port);
            }
            catch (Exception)
            {
                return "ERROR";
            }
        }

        /// <summary>
        /// 串口输出触发
        /// </summary>
        /// <returns></returns>
        public override string TriggerSeriesOutput()
        {
            try
            {
                return this.Query("Send");
            }
            catch (Exception)
            {
                return "ERROR";
            }
        }

        /// <summary>
        /// 串口输出控制
        /// </summary>
        /// <param name="LineNo"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public override string ParallelOutput(int LineNo, bool level)
        {
            try
            {
                if (LineNo >= 11 && LineNo <= 30)
                {
                    string str2 = null;
                    if (level) { str2 = "1"; } else str2 = "0";
                    // string str1 = Convert.ToString((long)LineNo, 16).PadLeft(2, '0').ToUpper();
                    string str = "SetL" + LineNo.ToString() + ":" + str2;
                    return this.Query(str);

                }
                else return "ERROR";
            }
            catch (Exception)
            {
                return "ERROR";
            }
        }

        //protected override string ParallelOutput(int LineNo, bool level)
        //{
        //    try
        //    {
        //        if (LineNo >= 11 && LineNo <= 30)
        //        {
        //            string str2 = null;
        //            if (level) { str2 = "1"; } else str2 = "0";
        //            string str1 = Convert.ToString((long)LineNo, 16).PadLeft(2, '0').ToUpper();
        //            string str = "SetL" + str1 + ":" + str2;
        //            return this.Query(str);

        //        }
        //        else return "ERROR";
        //    }
        //    catch (Exception)
        //    {
        //        return "ERROR";
        //    }
        //}

        /// <summary>
        /// 选择内部斩波或者外部斩波
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        public override string IsInternChopping(bool enable)
        {
            try
            {

                if (enable) { return this.Query("SourceIN_en"); }
                else { return this.Query("SourceIN_dis"); }
            }
            catch (Exception)
            {
                return "ERROR";
            }
        }

        /// <summary>
        /// 设置外部斩波是否反向
        /// </summary>
        /// <param name="str"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public override string SetExternalChoppingOpposition(string str, bool enable)
        {
            try
            {
                switch (str)
                {
                    case "T": if (enable) { return this.Query("SET_T:1"); } else return this.Query("SET_T:0");
                    case "R": if (enable) { return this.Query("SET_R:1"); } else return this.Query("SET_R:0");
                    case "TR1": if (enable) { return this.Query("SET_TR1:1"); } else return this.Query("SET_TR1:0");
                    case "TR2": if (enable) { return this.Query("SET_TR2:1"); } else return this.Query("SET_TR2:0");
                    default: return "ERROR";
                }
            }
            catch (Exception)
            {
                return "ERROR";
            }
        }

        /// <summary>
        /// 设置内部斩波置高或者置低
        /// </summary>
        /// <param name="str"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public override string SetInternalChoppingHigh(string str, bool enable)
        {
            try
            {
                switch (str)
                {
                    case "T": if (enable) { return this.Query("SetSourceT:1"); } else return this.Query("SetSourceT:0");
                    case "R": if (enable) { return this.Query("SetSourceR:1"); } else return this.Query("SetSourceR:0");
                    case "TR1": if (enable) { return this.Query("SetSourceTR1:1"); } else return this.Query("SetSourceTR1:0");
                    case "TR2": if (enable) { return this.Query("SetSourceTR2:1"); } else return this.Query("SetSourceTR2:0");
                    default: return "ERROR";
                }
            }
            catch (Exception)
            {
                return "ERROR";
            }
        }
        public override string SetSynchronousProt(int port1, int port2)
        {
            try
            {
                string str1 = Convert.ToString((port1 - 1));
                string str2 = Convert.ToString((port2 - 1));

                if ((port1 >= 10 && port1 <= 14))
                {
                    str1 = Convert.ToString((port1), 16).ToUpper();
                }
                if (port2 >= 10 && port2 <= 14)
                {
                    str2 = Convert.ToString((port2 ), 16).ToUpper();
                }
                string str = "SetNET:" + str1 + str2;
                return this.Query(str);
            }
            catch (Exception)
            {
                return "ERROR";
            }
        }


        public override string GetTemp()
        {
            try
            {
                return this.Query("GetTemp?");

            }
            catch (Exception)
            {
                return "ERROR";
            }
        }



        public override string Query(string inQueryCmd)
        {

            //Timeout = 30000;
            this.m_CommMutex.WaitOne();
            try
            {
                this.Send(inQueryCmd);
                //Thread.Sleep(10);
                return this.RemoveTrailingLineFeed(this.IO.ReadLine());
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }
        }


        /// <summary>
        /// 去掉回车符
        /// </summary>
        /// <param name="inToRemoveStr"></param>
        /// <returns></returns>
        private string RemoveTrailingLineFeed(string inToRemoveStr)
        {
            char[] trimChars = new char[] { '\n' };
            return inToRemoveStr.Trim(trimChars);
        }



    }
}
