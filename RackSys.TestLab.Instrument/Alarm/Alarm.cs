/* =============================================
 * Copyright @ 2013 北京中创锐科技术有限公司 
 * 名    称：Alarm
 * 功    能：Alarm
 * 作    者：Chen xf Administrator
 * 添加时间：2015-04-23 16:53:35
 * =============================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public delegate void EventAlarmMessage(object sender, string alarmInfo,int index);
    public class Alarm : ScpiInstrument 
    {
     
        public static event EventAlarmMessage MessageAlarmReport;

        public static void ReportAlarm(string tip,int index)
        {
            if (MessageAlarmReport != null)
            {
                MessageAlarmReport(null,tip,index);
            }
        }

        public Alarm(string inAddress)
            : base(inAddress)
        {

        }

        //
        public static Alarm Connect(string inAddress)
        {
            try
            {
                Alarm obj = new Alarm(inAddress);
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


        public void RedLight()
        {
            // 1：亮红灯+蜂鸣器响
            //02 30 31 57 4f 4e 4e 20 0d 0a 
            // byte[] ToSendData = new byte[] { 0x02, 0x30, 0x31,0x57,0x4f,0x4e,0x4e,0x20,0x0d,0x0a };
            // base.Send(ToSendData);
            string id = string.Format("{0}01WONN  {1}{2}", AscllToString(2), AscllToString(13), AscllToString(10));
            base.Send(id);

        }

        public void GreenLight()
        {
            //            3：亮绿灯
            //02 30 33 57 4f 4e 4e 20 0d 0a 
            //byte[] ToSendData = new byte[] { 0x02, 0x30, 0x33, 0x57, 0x4f, 0x4e, 0x4e, 0x20, 0x0d, 0x0a };

            //base.Send(ToSendData);

            string id = string.Format("{0}03WONN  {1}{2}", AscllToString(2), AscllToString(13), AscllToString(10));
            base.Send(id);

        }

        public void YellowLight()
        {
            //            2：亮黄灯
            //02 30 32 57 4f 4e 4e 20 0d 0a 
            //byte[] ToSendData = new byte[] { 0x02, 0x30, 0x32, 0x57, 0x4f, 0x4e, 0x4e, 0x20, 0x0d, 0x0a };

            //base.Send(ToSendData);
            string id = string.Format("{0}02WONN  {1}{2}", AscllToString(2), AscllToString(13), AscllToString(10));
            base.Send(id);
        }

        public void Reset()
        {
            //            4：复位指令
            //02 30 30 57 52 45 45 20 0D 0A 
            //byte[] ToSendData = new byte[] { 0x02, 0x30, 0x30, 0x57, 0x52, 0x45, 0x45, 0x20, 0x0d, 0x0a };

            //base.Send(ToSendData);
            string id = string.Format("{0}00WREE  {1}{2}", AscllToString(2), AscllToString(13), AscllToString(10));
            base.Send(id);

        }


        /// <summary>
        /// 将ASCII码转换为字符
        /// </summary>
        /// <param name="asciiCode"></param>
        /// <returns></returns>
        public static string AscllToString(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }

    }
}