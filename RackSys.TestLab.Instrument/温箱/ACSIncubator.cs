using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class ACSIncubator: Incubator
    {
        /// <summary>
        /// 默认安全温度，25度
        /// </summary>
        public static float SafetyTemp = 25;
        /// <summary>
        /// 默认安全湿度，50%
        /// </summary>
       public static float SafetyHumidity=50;

       protected override void DetermineIdentity()
       {
           List<byte> remoteCmdArr = new List<byte>() 
           {
                0x01, 
                0x06, 
                0x01, 
                0x04, 
                0x00, 
                0x40
           };

           //对数组进行CRC校验
           CheckAlgorithmLib.CRC16.ExecuteCheck(remoteCmdArr.ToArray());
           remoteCmdArr.Add(CheckAlgorithmLib.CRC16.LowByte);
           remoteCmdArr.Add(CheckAlgorithmLib.CRC16.HighByte);
           //将温箱控制方式置为远程方式
           byte[] CmdResult;
           this.Read(out CmdResult);
           this.Send(remoteCmdArr.ToArray());
           //判断是否控制成功
           //byte[] CmdResult;
           this.Read(out CmdResult);
           //如果出错；抛出异常
           List<byte> CorrectResultLs = new List<byte>()
           {
                0x01,
                0x06,
                0x01,
                0x04,
                0x00,
                0x00,
                0xC9,
                0xF7
           };
           if (!CheckComSendRight(remoteCmdArr, CmdResult))
           {
               throw new Exception("温箱识别失败！");
           }
           return;
       }

       protected override void DetermineOptions()
       {
           base.m_options = "";
       }

       public override string Identity
       {
           get
           {
               return "施耐德温箱 型号：WS10C";
           }
       }

    /// <summary>
    /// 默认程序异常时间5小时
    /// </summary>
       public static float DeadTimeInHour = 8;

        public ACSIncubator(string inAddress)
            : base(inAddress)
        {
        }


        /// <summary>
        /// 几个温箱的默认保护参数值更新程序
        /// </summary>
        /// <param name="mSafetyTemp"></param>
        /// <param name="mSafetyHumidity"></param>
        /// <param name="mDeadTimeInHour"></param>
        public override void SetDefaultVauleForProtection(float mSafetyTemp,  float mDeadTimeInHour)
        {
            SafetyTemp = mSafetyTemp;
            
            DeadTimeInHour = mDeadTimeInHour;
        
        }

        /// <summary>
        /// 检验串口命令是否正确发送
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="ReturnVaule"></param>
        /// <returns></returns>
        public bool CheckComSendRight(List<byte> Command, byte [] ReturnVaule)
        {
            if ((Command[0] == ReturnVaule[0]) && (Command[1] == ReturnVaule[1]))//当返回值的地址码和功能码均和发送一样，认为发送正确
            {return true;}
            else
            {
                return false;
            }
        
        
        }
        /// <summary>
        /// 温度设置程序，包含两部分，1.设定温度是多少：expectedTemp；2.正常情况下，多少小时会来下一条指令，如超时，这自动运行恢复常温的动作
        /// 思路：以程序段的方式执行设定温度的动作，第一段是程序设定温度，第一段会保持DeadTimeInHour的时间长度，正常情况下应在DeadTimeInHour内
        /// 完成该段温度所有操作，并被重新调用，执行新的温度程序。如果时间超时，则认为通讯异常，会运行第二段恢复常温的动作，以免损坏被测物
        /// 温箱默认地址为1，程序不提供接口修改
        /// </summary>
        /// <param name="ExepectedTemp"></param>
        /// <param name="DeadTimeInHour"></param>
        public override bool StartIncubator(float ExpectedTemp)
        {
            
                //指令是否被温箱正确接收并和执行的判断需现场调试。
                byte[] CmdResult;
            


                #region 温箱设置为远程
                List<byte> remoteCmdArr = new List<byte>() 
           {
                0x01, 
                0x06, 
                0x01, 
                0x04, 
                0x00, 
                0x40
           };

                //对数组进行CRC校验
                CheckAlgorithmLib.CRC16.ExecuteCheck(remoteCmdArr.ToArray());
                remoteCmdArr.Add(CheckAlgorithmLib.CRC16.LowByte);
                remoteCmdArr.Add(CheckAlgorithmLib.CRC16.HighByte);
                //将温箱控制方式置为远程方式
                this.Read(out CmdResult);
                this.Send(remoteCmdArr.ToArray());
                //读取返回值

                this.Read(out CmdResult);
                if (!CheckComSendRight(remoteCmdArr, CmdResult))
               {return false;}

                #endregion

                #region 停止正在执行的任务
                List<byte> StopCurrentTaskCMdArr = new List<byte>() 
               {
                   0x01, 0x10 ,0x01,0x04,0x00,0x02,0x04,0x00,0x40,0x00,0x00
               };


                //对数组进行CRC校验
                CheckAlgorithmLib.CRC16.ExecuteCheck(StopCurrentTaskCMdArr.ToArray());
                StopCurrentTaskCMdArr.Add(CheckAlgorithmLib.CRC16.LowByte);
                StopCurrentTaskCMdArr.Add(CheckAlgorithmLib.CRC16.HighByte);
                //将温箱控制方式置为远程方式
                this.Read(out CmdResult);
                this.Send(StopCurrentTaskCMdArr.ToArray());
                //读取返回值

                this.Read(out CmdResult);
                if (!CheckComSendRight(StopCurrentTaskCMdArr, CmdResult))
                { return false; }

                #endregion

                #region 设置新的温度
                byte[] TempWanted = new byte[4];
                byte[] TempSafe = new byte[4];
                float TemWanted_10=0;
                byte[] TemWantedByte_10 = new byte[4];
                
                FloatValueConvertToStringWithHEXtype(ExpectedTemp, out TempWanted);
                FloatValueConvertToStringWithHEXtype(SafetyTemp, out TempSafe);
                if (ExpectedTemp>=50&&ExpectedTemp <200)
                {
                    TemWanted_10 = ExpectedTemp - 10;   //高温保护，高于50度的设置，降低10度
                }
                else if (ExpectedTemp < 0 && ExpectedTemp>-100)
                {
                    TemWanted_10 = ExpectedTemp + 5;//低温保护，低于0度的色泽，提升10度
                }
                else if (ExpectedTemp >= 0 && ExpectedTemp<=50)
                {
                    TemWanted_10 = ExpectedTemp;    //0~50，人为是安全温度，不做处理
                }
                else
                {
                    throw new Exception("设置的温度超过-100~+200的范围！");
                }

                FloatValueConvertToStringWithHEXtype(TemWanted_10, out TemWantedByte_10);
            
               
                //温度控制思路：
                ///温度：期望保护值，期望值，期望值，保护值----期望保护值，大于25的，为期望值-10，低于25的，为期望值+5
                ///时间：5分钟，5分钟，保护时间，保护时间*10
                //byte[] NewTempCmdArr = new byte[] { 0x01, 0x10, 0x03, 0xE8, 0x00, 0x06, 0x0C, TempWanted[2], TempWanted[3], TempWanted[0], TempWanted[1], TempWanted[2], TempWanted[3], TempWanted[0], TempWanted[1], TempSafe[2], TempSafe[3], TempSafe[0], TempSafe[1] };
                List<byte> NewTempCmdArr = new List<byte>() 
               {
                   0x01, 0x10, 0x03, 0xE8, 0x00, 0x06, 0xc, 
                   //TemWantedByte_10[2], TemWantedByte_10[3], TemWantedByte_10[0], TemWantedByte_10[1],
                   TempWanted[2], TempWanted[3], TempWanted[0], TempWanted[1], TempWanted[2], TempWanted[3], TempWanted[0], TempWanted[1], TempSafe[2], TempSafe[3], TempSafe[0], TempSafe[1]
               };
                

                    //对数组进行CRC校验
                    CheckAlgorithmLib.CRC16.ExecuteCheck(NewTempCmdArr.ToArray());
                    NewTempCmdArr.Add(CheckAlgorithmLib.CRC16.LowByte);
                    NewTempCmdArr.Add(CheckAlgorithmLib.CRC16.HighByte);
                    //将温箱控制方式置为远程方式
                    this.Read(out CmdResult);
                    this.Send(NewTempCmdArr.ToArray());
                    //读取返回值

                    this.Read(out CmdResult);
                    if (!CheckComSendRight(NewTempCmdArr, CmdResult))
                    { return false; };
                #endregion

                #region 设置时间
                byte[] DeadTimeByte = new byte[4];
                byte[] SafetTimeByte = new byte[4];
                byte[] TimetToWantedTemp = new byte[4];
                FloatValueConvertToStringWithHEXtype(1800, out TimetToWantedTemp);//十分钟时间，从温箱当前温度，至期望温度，
                FloatValueConvertToStringWithHEXtype(DeadTimeInHour * 3600, out DeadTimeByte);//底层接收以秒为单位，故乘以3600
                FloatValueConvertToStringWithHEXtype(DeadTimeInHour * 3600*10, out SafetTimeByte);//安全温度的时间，程序默认取故障时间的十倍
                
                ///温度：期望值，期望值，期望值，保护值
                ///时间：5分钟，5分钟，保护时间，保护时间*10
                List<byte> TimeCmdArr = new List<byte>() 
               {
                   0x01, 0x10, 0x05, 0x78, 0x00, 0x06, 0x0C,
                   //TimetToWantedTemp[2], TimetToWantedTemp[3],TimetToWantedTemp[0],TimetToWantedTemp[1],
                   TimetToWantedTemp[2], TimetToWantedTemp[3],TimetToWantedTemp[0],TimetToWantedTemp[1],
                   DeadTimeByte[2], DeadTimeByte[3], DeadTimeByte[0], DeadTimeByte[1],
                   SafetTimeByte[2], SafetTimeByte[3], SafetTimeByte[0], SafetTimeByte[1],
               };
               
        

                //对数组进行CRC校验
                CheckAlgorithmLib.CRC16.ExecuteCheck(TimeCmdArr.ToArray());
                TimeCmdArr.Add(CheckAlgorithmLib.CRC16.LowByte);
                TimeCmdArr.Add(CheckAlgorithmLib.CRC16.HighByte);
                //将温箱控制方式置为远程方式
                this.Read(out CmdResult);
                this.Send(TimeCmdArr.ToArray());
                //读取返回值

                this.Read(out CmdResult);

                if (!CheckComSendRight(TimeCmdArr, CmdResult))
                { return false; };
                #endregion
                #region 设置程序控制段数
                List<byte> LoopSetCmdArr = new List<byte>() 
               {
                  0x01, 0x10, 0x01, 0x32, 0x00, 0x04,0x08,0x00,0x01,0x00,
                  0x03,//程序段数
                  0x00,0x01,0x00,0x01
               };


                //对数组进行CRC校验
                CheckAlgorithmLib.CRC16.ExecuteCheck(LoopSetCmdArr.ToArray());
                LoopSetCmdArr.Add(CheckAlgorithmLib.CRC16.LowByte);
                LoopSetCmdArr.Add(CheckAlgorithmLib.CRC16.HighByte);
                this.Read(out CmdResult);
                this.Send(LoopSetCmdArr.ToArray());
                //读取返回值

                this.Read(out CmdResult);

                if (!CheckComSendRight(LoopSetCmdArr, CmdResult))
                { return false; }

                #endregion

                #region 执行
                List<byte> StartCmdArr = new List<byte>() 
               {
                  0x01, 0x10, 0x01, 0x04, 0x00, 0x02,0x04,0x00,0x40,0x00,0x01
               };
                

                    //对数组进行CRC校验
                CheckAlgorithmLib.CRC16.ExecuteCheck(StartCmdArr.ToArray());
                    StartCmdArr.Add(CheckAlgorithmLib.CRC16.LowByte);
                    StartCmdArr.Add(CheckAlgorithmLib.CRC16.HighByte);
                    this.Read(out CmdResult);
                    this.Send(StartCmdArr.ToArray());
                    //读取返回值

                    this.Read(out CmdResult);

                    if (!CheckComSendRight(StartCmdArr, CmdResult))
                    { return false; }

                #endregion


                return true;
            

            




        }


        public override bool StopChamber()
        {
            byte[] CmdResult;

            #region 停止正在执行的任务
            List<byte> StopCurrentTaskCMdArr = new List<byte>() 
               {
                   0x01, 0x10 ,0x01,0x04,0x00,0x02,0x04,0x00,0x40,0x00,0x00
               };


            //对数组进行CRC校验
            CheckAlgorithmLib.CRC16.ExecuteCheck(StopCurrentTaskCMdArr.ToArray());
            StopCurrentTaskCMdArr.Add(CheckAlgorithmLib.CRC16.LowByte);
            StopCurrentTaskCMdArr.Add(CheckAlgorithmLib.CRC16.HighByte);
            //将温箱控制方式置为远程方式
            this.Read(out CmdResult);
            this.Send(StopCurrentTaskCMdArr.ToArray());
            //读取返回值

            this.Read(out CmdResult);
            if (!CheckComSendRight(StopCurrentTaskCMdArr, CmdResult))
            { return false; }

            #endregion
            return true;
        }

        public override bool TurnoffChamber()
        {
            #region 停止正在执行的任务
            byte[] CmdResult;
            List<byte> StopCurrentTaskCMdArr = new List<byte>() 
               {
                   0x01, 0x10 ,0x01,0x04,0x00,0x02,0x04,0x00,0x40,0x00,0x00
               };


            //对数组进行CRC校验
            CheckAlgorithmLib.CRC16.ExecuteCheck(StopCurrentTaskCMdArr.ToArray());
            StopCurrentTaskCMdArr.Add(CheckAlgorithmLib.CRC16.LowByte);
            StopCurrentTaskCMdArr.Add(CheckAlgorithmLib.CRC16.HighByte);
            //将温箱控制方式置为远程方式
            this.Read(out CmdResult);
            this.Send(StopCurrentTaskCMdArr.ToArray());
            //读取返回值

            this.Read(out CmdResult);
            if (!CheckComSendRight(StopCurrentTaskCMdArr, CmdResult))
            { return false; }
            return true;
            #endregion
            
        }

        /// <summary>
        /// 将十进制的Float的值，转换为IEEE754单精度数，并按照低位在前，高位在后的方式凑成字符串并返回
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string FloatValueConvertToStringWithHEXtype(float value,out byte[] ByteResult)
        {
            byte[] byteArray = BitConverter.GetBytes(value);
            Array.Reverse(byteArray);

            string highWord = Convert.ToString(byteArray[0], 16).PadLeft(2, '0').ToUpper() + Convert.ToString(byteArray[1], 16).PadLeft(2, '0').ToUpper();
            string LowWord = Convert.ToString(byteArray[2], 16).PadLeft(2, '0').ToUpper() + Convert.ToString(byteArray[3], 16).PadLeft(2, '0').ToUpper();
            string Result = LowWord + highWord;
            ByteResult = byteArray;


            return Result;
        
        }
    }
}
