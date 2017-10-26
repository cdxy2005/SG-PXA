/* =============================================
 * Copyright @ 2017 北京中创锐科技术有限公司 
 * 名    称：DUT  
 * 功    能：DUT  
 * 作    者：CHEN XF Administrator
 * 添加时间：2017/9/5 17:14:04
 * =============================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{

    public class DUT : ScpiInstrument
    {

        public static event EventAlarmMessage MessageAlarmReport;

        public static void ReportAlarm(string tip, int index)
        {
            if (MessageAlarmReport != null)
            {
                MessageAlarmReport(null, tip, index);
            }
        }

        public DUT(string inAddress)
            : base(inAddress)
        {

        }
        //string m_identity=",,,,";
        // public override string  Identity;
        // {
        //return m_identity
        // }
        protected override void DetermineIdentity()
        {
                base.m_model = "1";
                base.m_serial = "1";
                base.m_firmwareVersion = "1";
        }

        protected override void DetermineOptions()
        {
            this.m_options = "111";
        }
        //
        public static DUT Connect(string inAddress)
        {
            try
            {
                DUT obj = new DUT(inAddress);
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

        /// <summary>
        /// 控制频率
        /// </summary>
        /// <param name="freq"></param>
        public void CtrlFreq( double freq)
        {
            try
            {
                freq =Math.Floor( freq * 1e-3);
                byte[] sendByte = new byte[12];
                sendByte[0] = 0xff;
                sendByte[1] = 0x09;
                sendByte[2] = 0x04;
                sendByte[11] = 0xee;
                //byte[] freqByte = BitConverter.GetBytes(freq);
                byte[] freqByte = BitConverter.GetBytes((Int64)freq);
                //double aa = BitConverter.ToDouble(freqByte,8);
                //Array.Reverse(freqByte);
                int abc = 0;
                byte[] byteABC = BitConverter.GetBytes(abc);
                char xxx = (Char)byteABC[0];
                if (xxx == 1)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        sendByte[3 + i] = freqByte[i ];
                    }
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        sendByte[3 + i] = freqByte[7 - i];
                    }
                }

                byte[] readByte = new byte[12]; ;
              int  count=0;
                while (true)
                {
                    base.Send(sendByte);
                    base.Read(out readByte);
                    if (readByte.Length == 0 && count < 15)
                    {
                        count++;
                        Thread.Sleep(100);
                    }

                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

        public void SETChannel(int chan)
        {
            byte[] sendByte = new byte[12];
            sendByte[0] = 0xff;
            sendByte[1] = 0x09;
            sendByte[2] = 0x01;
            sendByte[3] = (byte)chan;
            sendByte[11] = 0xEE;
            base.Send(sendByte);
            byte[] readByte = new byte[12]; ;
            base.Read(out readByte);
            int count = 0;
            while (true)
            {
                base.Send(sendByte);
                base.Read(out readByte);
                if (readByte.Length == 0&& count<15)
                {
                    count++;
                    Thread.Sleep(100);
                }
                else
                {
                    break;
                }
            }

        }
        /// <summary>
        /// 控制增益
        /// </summary>
        /// <param name="gain"></param>
        public void CtrlGain(double freq,double gain,bool IsEnd)
        {
            try
            {
                freq = freq * 1e-3;
                byte[] sendByte = new byte[12];
                sendByte[0] = 0xff;
                sendByte[1] = 0x09;
                sendByte[2] = 0x00;
                if (IsEnd)
                {
                    sendByte[10] = 0x01;//结束的时候传01
                }
                else
                {
                    sendByte[10] = 0x00;
                }
                 sendByte[11] = 0xee;
                
                byte[] freqByte = BitConverter.GetBytes((Int64)freq);
                
                //int abc = 0;
                //byte[] byteABC = BitConverter.GetBytes(abc);
                //char xxx = (Char)byteABC[0];
                if (true)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        sendByte[8 - i] = freqByte[i];
                    }
                }
                //else
                //{
                //    for (int i = 0; i < 6; i++)
                //    {
                //        sendByte[3 + i] = freqByte[7 - i];
                //    }
                //}

                sendByte[9] = (byte)((int)(gain / 0.25+0.5));

                byte[] readByte = new byte[12]; ;
                int count = 0;
                while (true)
                {
                    base.Send(sendByte);
                    base.Read(out readByte);
                    if (readByte.Length == 0 && count < 15)
                    {
                        count++;
                        Thread.Sleep(100);
                    }
              
                    else
                    {
                        break;
                    }
                }
         

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void CtrlTWork()
        {
            try
            {
                byte[] readByte = new byte[12]; ;
                byte[] sendByte = new byte[12];
                sendByte[0] = 0xff;
                sendByte[1] = 0x09;
                sendByte[2] = 0x02;
                sendByte[11] = 0xee;
                int count = 0;
                while (true)
                {
                    base.Send(sendByte);
                    base.Read(out readByte);
                    if (readByte.Length == 0 && count < 15)
                    {
                        count++;
                        Thread.Sleep(100);
                    }
                    else
                    {
                        break;
                    }
                }
                //this.Send(sendByte);
                //this.Read(out readByte);
                //if (readByte.Length == sendByte.Length)
                //{
                //    for (int i = 0; i < readByte.Length; i++)
                //    {
                //        if (readByte[i] != sendByte[i])
                //        {
                //            throw new Exception("回读数据与发送数据不符合!");
                //        }
                //    }
                //}
                //else
                //{
                //    throw new Exception("回读数据与发送数据长度不符合!");
                //}

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void CtrlRWork()
        {
            try
            {
                byte[] sendByte = new byte[12];
                byte[] readByte = new byte[12]; ;
                sendByte[0] = 0xff;
                sendByte[1] = 0x09;
                sendByte[2] = 0x03;
                sendByte[11] = 0xee;
                int count = 0;
                while (true)
                {
                    base.Send(sendByte);
                    base.Read(out readByte);
                    if (readByte.Length == 0 && count < 15)
                    {
                        count++;
                        Thread.Sleep(100);
                    }
               
                    else 
                    {
                        break;
                    }
                }
            
                //if (readByte.Length == sendByte.Length)
                //{
                //    for (int i = 0; i < readByte.Length; i++)
                //    {
                //        if (readByte[i] != sendByte[i])
                //        {
                //            throw new Exception("回读数据与发送数据不符合!");
                //        }
                //    }
                //}
                //else
                //{
                //    throw new Exception("回读数据与发送数据长度不符合!");
                //}

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
