using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace RackSys.TestLab.Instrument
{
    public class FUQIIncubator: Incubator
    {

        public FUQIIncubator(string inAddress)
            : base(inAddress)
        {
        }

       protected override void DetermineIdentity()
       {
           string Result = this.Query("$01?\r\n");
           if (Result.IndexOf("ASCII-2 PROTOCOL CONFIGURATION") >= 0 && Result.IndexOf("Configuration type: DYNAMIC") >= 0)
           {
               base.m_model = "VTS";
               base.m_serial = "7011-5";
               return;
           }
           else
           {
               throw new Exception("富奇温箱识别失败！");
           }
           
       }

       public override string Identity
       {
           get
           {
               return "富奇温箱 型号：7011-5";
           }
       }

       protected override void DetermineOptions()
       {
           base.m_options = "";
       }

       /// <summary>
       /// 停止
       /// </summary>
       /// <returns></returns>
       public override bool StopChamber()
       {
           this.Send("$01E 0000.0 0000.0 0000.0 00000000000000000000000000000000\r\n");
           string Result = this.Query("$01I\r\n");
           if (Result.Length > 30) 
           {
               if (Result[29] == '0')
               {
                   return true;
               }
           }
           return false;
       }

       /// <summary>
       /// 关闭
       /// </summary>
       /// <returns></returns>
       public override bool TurnoffChamber()
       {
           return StopChamber();
       }

       /// <summary>
       /// 几个温箱的默认保护参数值更新程序
       /// </summary>
       /// <param name="mSafetyTemp"></param>
       /// <param name="mSafetyHumidity"></param>
       /// <param name="mDeadTimeInHour"></param>
       public override void SetDefaultVauleForProtection(float mSafetyTemp, float mDeadTimeInHour)
       {

       }

       /// <summary>
       /// 通过
       /// </summary>
       public override bool StartIncubator(float ExpectedTemp)
       {
           string temString = ExpectedTemp.ToString("0000.0");
           string CmdArray = string.Format("$01E {0} 0000.0 0000.0 01000000000000000000000000000000\r\n", temString);
           this.Send(CmdArray);
           //校验温度是否设置成功，并且已经开始运行
           string Result = this.Query("$01I\r\n");

           bool TemFlag = false;
           bool StartFlag = false;
           if (Result.Length > 30)
           {
               StartFlag = (Result[29] == '1');
               //0026.0 0025.7 0050.0 0001.0 01100000000000000000000000000000
               double TemDoubleFlag = double.MaxValue;
               try
               {
                   string[] Str = Result.Split(' ');
                   TemDoubleFlag = double.Parse(Str[0]);
               }
               catch 
               {
                   //TemFlag = false;
               }

               TemFlag = (Math.Round( TemDoubleFlag,1) ==Math.Round( ExpectedTemp,1));
           }
           if (!(TemFlag && StartFlag))
           {
               //MessageBox.Show(string.Format
               //    (
               //        "温箱设置出错!，{0}请确保温箱已经设置为外温控制模式{1}并且为手动运行模式。{2}如果温箱已经运行，请关闭温箱，检查错误！",
               //        "\r\n",
               //        "\r\n",
               //        "\r\n"
               //    )
               //    , "错误", MessageBoxButton.OK, MessageBoxImage.Error);
           }
           return TemFlag && StartFlag;
       }

       /// <summary>
       /// 温度
       /// </summary>
       public virtual double Temprature
       {
           get
           {
               //校验温度是否设置成功，并且已经开始运行
               string Result = this.Query("$01I\r\n");
               if (Result.Length > 30)
               {
                   //0026.0 0025.7 0050.0 0001.0 01100000000000000000000000000000
                   try
                   {
                       string[] Str = Result.Split(' ');
                       return double.Parse(Str[1]);
                   }
                   catch
                   {
                       return double.NaN;
                   }
               }
               return double.NaN;
           }
           set
           {
               string temString = value.ToString("0000.0");
               string Result = this.Query("$01I\r\n");
               if (Result.Length > 30)
               {
                   string CmdArray = string.Format("$01E {0} 0000.0 0000.0 0{1}000000000000000000000000000000\r\n", temString, Result[29]);
                   this.Send(CmdArray);
               }
           }
       }

        /// <summary>
        /// 查询，并返回字符串
        /// </summary>
        /// <param name="inQueryCmd"></param>
        /// <returns></returns>
        public override string Query(string inQueryCmd)
        {
            this.m_CommMutex.WaitOne();
            try
            {
                this.Send(inQueryCmd);
                byte[] data;
                //返回字节数
                int resultBytesCount = 100000000;//最大支持读取100000000字节
                string Result = string.Empty;
                this.IO.Read(out data, ref resultBytesCount);
                if (data.Length > 0)
                {
                    Result = Encoding.Default.GetString(data);
                }
                return Result;
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }

        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="inToSendCmd"></param>
        protected override void Send(string inToSendCmd)
        {
            this.m_CommMutex.WaitOne();
            try
            {
                try
                {
                    this.IO.Write(inToSendCmd);
                }
                catch
                {
                    ///出现意外时重新初始化连接
                    IOManager.Remove(this.m_address);
                    this.IO.Write(inToSendCmd);
                }
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }
        }
    }
}
