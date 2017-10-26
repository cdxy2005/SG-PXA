using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using System.Collections;

namespace RackSys.TestLab.Instrument
{
    public class MultiChannelController : ScpiInstrument
    {
        public MultiChannelController(string inAddress)
            : base(inAddress)
        {
        }

        protected override void DetermineOptions()
        {
            base.m_options = string.Empty;
        }


        public static MultiChannelController Connect(string currentAddress)
        {
            return new MultiChannelController(currentAddress);
        }

        //2014.02.27  苏渊红 驱动 第一版
        //IDN 查询
        public string GetIDN()
         {
             return this.Query("*IDN?");             
         }


        //SN设定


        public string SetIDN(string value)
        {
            if (value.Length != 13)
            {
                throw new Exception("SN长度必须为13个字符。");
            }
            else
            {
                return this.Query("*SN:" + value);
            }
        }
        

        //PORT查询
         public string GetSocktePort
         {
             get 
             {
                 return this.Query("*PORT:?");
             }
           
         }

         #region 新增加
         //串口设置指令6位
         public string SetCom6bit(int ComNumber, uint value)
         {

             string value_string = Convert.ToString(value, 16).PadLeft(2,'0').ToUpper();
             return this.Query("*COMM" + ComNumber.ToString() + "_06_" + value_string + "0101");
         }

         //串口设置指令12位
         public string SetCom12bit(int ComNumber, uint value)
         {
             string value_string = Convert.ToString(value, 16).PadLeft(4, '0').ToUpper(); ;
             return this.Query("*COMM" + ComNumber.ToString() + "_12_" + value_string + "01");
         }

         //串口设置指令16位
         public string SetCom16bit(int ComNumber, uint value)
         {

             uint temp_high_6bit = (value << 16)>>26;
             uint temp_mid_6bit = (value << 22) >> 26;
             uint temp_low_4bit = (value << 28) >> 28;

             string value_string_high_6bit = Convert.ToString(temp_high_6bit, 16).PadLeft(2, '0').ToUpper();
             string value_string_mid_6bit = Convert.ToString(temp_mid_6bit, 16).PadLeft(2, '0').ToUpper();
             string value_string_low_4bit = Convert.ToString(temp_low_4bit, 16).PadLeft(2, '0').ToUpper();
             //string value_string = Convert.ToString(value, 16).PadLeft(6, '0').ToUpper(); 
             return this.Query("*COMM" + ComNumber.ToString() + "_16_" + value_string_high_6bit + value_string_mid_6bit + value_string_low_4bit);
         }



         // 脉冲信号设置
         public string SetPulseSignal(int value)
         {
             if ((value <= 0) || (value > 8))
             {

                 throw new Exception("脉冲端口设置超出边界。");

             }
             else
             {
                 switch (value)
                 {
                     case 1: return this.Query("*PWM1_H");
                     case 2: return this.Query("*PWM2_H");
                     case 3: return this.Query("*PWM3_H");
                     case 4: return this.Query("*PWM4_H");
                     case 5: return this.Query("*PWM5_H");
                     case 6: return this.Query("*PWM6_H");
                     case 7: return this.Query("*PWM7_H");
                     case 8: return this.Query("*PWM8_H");
                     default: return string.Empty;
                 }
             }
         }
         //并口设置指令16位
         public string SetPara(uint value)
         {

             uint temp_high_6bit = (value << 16) >> 26;
             uint temp_mid_6bit = (value << 22) >> 26;
             uint temp_low_4bit = (value << 28) >> 28;

             string value_string_high_6bit = Convert.ToString(temp_high_6bit, 16).PadLeft(2, '0').ToUpper();
             string value_string_mid_6bit = Convert.ToString(temp_mid_6bit, 16).PadLeft(2, '0').ToUpper();
             string value_string_low_4bit = Convert.ToString(temp_low_4bit, 16).PadLeft(2, '0').ToUpper();
              
             string value_string = Convert.ToString(value, 16).PadLeft(6, '0').ToUpper(); ;
             return this.Query("*BIT16_0X" + value_string);
         }
         #endregion

         ////串口设置指令6位
         //public string SetCom6bit(int ComNumber, string value)
         //{
         //     return this.Query("*COMM"+ComNumber.ToString()+"_06_" + value + "0101");             
         //}

         ////串口设置指令12位
         //public string SetCom12bit(int ComNumber, string value)
         //{
         //    return this.Query("*COMM" + ComNumber.ToString() + "_12_" + value + "01");
         //}

         ////串口设置指令16位
         //public string SetCom16bit(int ComNumber, string value)
         //{
         //    return this.Query("*COMM" + ComNumber.ToString() + "_16_" + value );
         //}



      
         ////串口设置指令16位
         //public string SetPara( string value)
         //{
         //    return this.Query("*BIT16_0X" + value);
         //}

        //

        /*
         * 
         * public virtual double AmplOffset
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
       public override void AutoScale()
        {
            base.Send("DISP:WIND:TRAC:Y:AUTO");
        }
         * public override void Average_ON(int count)
        {
            base.SendOpc(string.Concat("SENS:AVER:COUN ", count.ToString()), 50);
            base.Send("SENS:AVER ON");
        }
         * private void AssetChannelID(int ChannelID)
        {
            if ((ChannelID <= 0) || (ChannelID > 4))
            {
                throw new Exception("电源通道编号超界。");
            }
        }
//         */
    }
}
