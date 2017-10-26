using System;
using System.Globalization;
using System.Runtime.InteropServices;
using RackSys.TestLab.Visa;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    /// <summary>
    /// 仪器控制基础
    /// </summary>
    public class InstrumentBase  
    {

        /// <summary>
        /// 要控制的仪器的地址
        /// </summary>
        protected string m_address = null;

        /// <summary>
        /// idn查询返回数据，具体包括：厂家，型号，序号和Firmware版本号等；
        /// </summary>
        protected string m_identity = "";

        public virtual string Identity
        {
            get { return m_identity; }
        }

        /// <summary>
        /// 制造商
        /// </summary>
        protected string m_Manufactor = "";

        /// <summary>
        /// 制造商
        /// </summary>
        public string Manufactor
        {
            get { return m_Manufactor; }
        }


        /// <summary>
        /// 仪器型号
        /// </summary>
        protected string m_model = "";

        /// <summary>
        /// 似乎是要获得一个设备得型号
        /// </summary>
        public virtual string Model
        {
            get { return m_model; }
        }

        /// <summary>
        /// 仪器序号
        /// </summary>
        protected string m_serial = "";

        public string SerialNumber
        {
            get { return m_serial; }
        }

        /// <summary>
        /// 选件信息
        /// </summary>
        protected string m_options = "";

        /// <summary>
        /// 已经确认没有的Option选项。
        /// </summary>
        protected string m_MissingOption = "";

        /// <summary>
        /// 已经确认有的Option选项
        /// </summary>
        protected string m_ExistingOption = "";

        /// <summary>
        /// 固件版本号
        /// </summary>
        protected string m_firmwareVersion = "";

        public virtual string FirmwareVersion
        {
            get 
            {
                return m_firmwareVersion; 
            }
        }

        /// <summary>
        /// 连接是否已经验证
        /// </summary>
        private bool m_Connected = false;

        protected bool Connected
        {
            get { return m_Connected; }
            set { m_Connected = value; }
        }


        protected Mutex m_CommMutex;

        public InstrumentBase(string address)
        {
            this.m_address = address;
            this.m_CommMutex = new Mutex(false, this.m_address);
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

        /// <summary>
        /// 发送数据类型的命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="value"></param>
        protected void SendNumber(string command, double value)
        {
            this.Send(command + " " + value.ToString(NumberFormatInfo.InvariantInfo));
        }

        /// <summary>
        /// 查询，若超时未返回，则退出
        /// </summary>
        /// <param name="queryCmd"></param>
        /// <param name="ValTimeOutInMs"></param>
        /// <returns></returns>
        public virtual string Query(string queryCmd, int ValTimeOutInMs)
        {
            this.m_CommMutex.WaitOne();
            try
            {
                ///发送命令
                this.Send(queryCmd);

                ///设置IO的超时时间
                int timeoutInMs = this.IO.TimeoutInMs;
                this.IO.TimeoutInMs = ValTimeOutInMs;

                //读取返回数值
                string text = this.IO.ReadLine();

                //恢复timeout设置
                this.IO.TimeoutInMs = timeoutInMs;

                //去掉回车符，并返回
                return this.RemoveTrailingLineFeed(text);
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// 写入块数据
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        public virtual void WriteBlock(string command, byte[] data)
        {
            this.m_CommMutex.WaitOne();
            try
            {
                ///查询序号，确认仪器有反应
                this.Query("*IDN?");
                //获取原有的超时时间
                int timeoutInMs = this.IO.TimeoutInMs;

                //计算并设置新的超时数值
                int num2 = ((data.Length / 1000000/*0xf4240*/) + 1) * 5000/*0x1388*/;
                if (num2 > timeoutInMs)
                {
                    this.IO.TimeoutInMs = num2;
                }

                //写入数据
                this.IO.WriteBlock(command, data);

                //恢复超时数值设置
                if (this.IO.TimeoutInMs != timeoutInMs)
                {
                    this.IO.TimeoutInMs = timeoutInMs;
                }
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// 写入块数据
        /// </summary>
        /// <param name="A_0"></param>
        /// <param name="A_1"></param>
        public virtual void WriteBlock(string command, short[] data)
        {
            this.m_CommMutex.WaitOne();
            try
            {
                this.Query("*IDN?");
                int count = data.Length * Marshal.SizeOf(typeof(short));
                byte[] dst = new byte[count];
                Buffer.BlockCopy(data, 0, dst, 0, count);
                for (int i = 0; i < count; i += 2)
                {
                    //交换高低位之间的位置
                    byte num3 = dst[i];
                    dst[i] = dst[i + 1];
                    dst[i + 1] = num3;
                }

                //计算并设置超时数值
                int timeoutInMs = this.IO.TimeoutInMs;
                int num5 = ((count / 0xf4240) + 1) * 0x1388;
                if (num5 > timeoutInMs)
                {
                    this.IO.TimeoutInMs = num5;
                }

                ///写入命令
                this.IO.WriteBlock(command, dst);

                //恢复超时数值设置
                if (this.IO.TimeoutInMs != timeoutInMs)
                {
                    this.IO.TimeoutInMs = timeoutInMs;
                }
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// 写入数据，超时返回
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="timeout"></param>
        public virtual void WriteBlock(string command, byte[] data, int timeout)
        {
            this.m_CommMutex.WaitOne();
            try
            {
                this.Query("*IDN?");
                //设置超时数值
                int timeoutInMs = this.IO.TimeoutInMs;
                this.IO.TimeoutInMs = timeout;
                //写入数据
                this.IO.WriteBlock(command, data);
                //查询是否完成
                this.IO.WriteLine("*OPC?");
                this.IO.ReadLine();

                //恢复超时数值
                this.IO.TimeoutInMs = timeoutInMs;
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }

        }

        public virtual void WriteBlock(string inCommandStr, short[] ToSendBlockData, int inTimeOutInMs)
        {
            this.m_CommMutex.WaitOne();
            try
            {

                this.Query("*IDN?");
                int timeoutInMs = this.IO.TimeoutInMs;
                this.IO.TimeoutInMs = inTimeOutInMs;
                int count = ToSendBlockData.Length * Marshal.SizeOf(typeof(short));
                byte[] dst = new byte[count];
                Buffer.BlockCopy(ToSendBlockData, 0, dst, 0, count);
                //调换顺序
                for (int i = 0; i < count; i += 2)
                {
                    byte num4 = dst[i];
                    dst[i] = dst[i + 1];
                    dst[i + 1] = num4;
                }
                this.IO.WriteBlock(inCommandStr, dst);
                this.IO.WriteLine("*OPC?");
                this.IO.ReadLine();
                this.IO.TimeoutInMs = timeoutInMs;
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }
        }

        public virtual string GetSerialNo()
        {
            return this.m_serial;
        }

        public virtual int Timeout
        {
            get
            {
                return this.IO.TimeoutInMs;
            }
            set
            {
                this.IO.TimeoutInMs = value;
            }
        }



        public virtual string Options
        {
            get
            {
                return this.m_options;
            }
        }

        public virtual void GoToLocal()
        {
            this.IO.GotoLocal();
        }

        public virtual string ResourceName
        {
            get
            {
                return this.m_address;
            }
        }

        public virtual bool IsScpiInstrument()
        {
            return false;
        }



        protected virtual InstrumentIOSession IO
        {
            get
            {
                return IOManager.ProvideDirectIO(this.m_address);
            }
        }




        /// <summary>
        /// 查询，并返回字符串
        /// </summary>
        /// <param name="inQueryCmd"></param>
        /// <returns></returns>
        public virtual string Query(string inQueryCmd)
        {
            this.m_CommMutex.WaitOne();
            try
            {
                this.Send(inQueryCmd);
                return this.RemoveTrailingLineFeed(this.IO.ReadLine());
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }

        }

        /// <summary>
        /// 查询，并返回字符串
        /// </summary>
        /// <param name="inQueryCmd"></param>
        /// <returns></returns>
        public virtual string QueryWithoutLineFeed(string inQueryCmd)
        {
            this.m_CommMutex.WaitOne();
            try
            {
                this.SendWithoutLineFeed(inQueryCmd);
                return this.RemoveTrailingLineFeed(this.IO.ReadLine());
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
        protected virtual void Send(string inToSendCmd)
        {
            this.m_CommMutex.WaitOne();
            try
            {
                try
                {
                    this.IO.WriteLine(inToSendCmd);
                }
                catch
                {
                    ///出现意外时重新初始化连接
                    IOManager.Remove(this.m_address);
                    this.IO.WriteLine(inToSendCmd);
                }
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
        protected virtual void SendWithoutLineFeed(string inToSendCmd)
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
                    this.IO.WriteLine(inToSendCmd);
                }
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }
        }

        protected virtual void Send(byte[] inToSendCmd)
        {
            this.m_CommMutex.WaitOne();
            try
            {
                try
                {
                    this.IO.WriteBytes(inToSendCmd);
                }
                catch
                {
                    ///出现意外时重新初始化连接
                    IOManager.Remove(this.m_address);
                    this.IO.WriteBytes(inToSendCmd);
                }
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }
        }

        protected virtual void Read(out byte[] result) 
        {
            this.m_CommMutex.WaitOne();
            result = new byte[0];
            try
            {
                try
                {
                    this.IO.Read(out result);
                }
                catch
                {
                    ///出现意外时重新初始化连接
                    IOManager.Remove(this.m_address);
                }
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }
        }
        protected virtual void Read(out byte[] result, ref int count)
        {
            this.m_CommMutex.WaitOne();
            result = new byte[count];
            try
            {
                try
                {
                    this.IO.Read(out result, ref count);
                }
                catch
                {
                    ///出现意外时重新初始化连接
                    IOManager.Remove(this.m_address);
                }
            }
            finally
            {
                this.m_CommMutex.ReleaseMutex();
            }
        }


        /// <summary>
        /// 查询数字
        /// </summary>
        /// <param name="inQueryCmd"></param>
        /// <returns></returns>
        protected double QueryNumber(string inQueryCmd)
        {
            return double.Parse(this.Query(inQueryCmd), NumberStyles.Float, NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// 固件版本号比校，忽略大小写,返回0为相同
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public int CompareFirmware(string target)
        {
            return string.Compare(this.m_firmwareVersion, target, true);
        }
    }
}
