using System;
using System.Collections.Specialized;
using RackSys.TestLab.Visa;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    /// <summary>
    /// 代表支持SCPI指令系统的仪器操作方法
    /// </summary>
    public class ScpiInstrument : InstrumentBase
    {
        /// <summary>
        /// 如果是SCPI仪器，检测其型号和选件等
        /// </summary>
        /// <param name="Address"></param>
        public ScpiInstrument(string Address)
            : base(Address)
        {
            if (this.IsScpiInstrument())
            {
                this.DetermineIdentity();


                this.DetermineOptions();
            }
        }

        public ScpiInstrument(string Address, bool ifSkipOptCmd)
            : base(Address)
        {
            if (this.IsScpiInstrument())
            {
                this.DetermineIdentity();

                if (!ifSkipOptCmd)
                {
                    this.DetermineOptions();
                }
            }
        }
 

        /// <summary>
        /// 单独创建一个会话，并通过它获得其型号，之后将对应的Session放入到Session字典当中。并返回设备型号
        /// </summary>
        /// <param name="inInstAddress"></param>
        /// <returns></returns>
        public static string DetermineModel(string inInstAddress)
        {
            Mutex tmpMutex = new Mutex(false, inInstAddress);

            tmpMutex.WaitOne();
            try
            {
                InstrumentIOSession session = IOManager.ProvideDirectIO(inInstAddress);
                if (session == null)
                {
                    throw new Exception("无法连接到地址：" + inInstAddress);
                    return string.Empty;
                }
                try
                {
                    //session.WriteLine("*RST");
                    session.WriteLine("*IDN?");
                }
                catch
                {
                    ///出错时移除对应的IO连接对象
                    IOManager.Remove(inInstAddress);

                    //再次探测
                    session = IOManager.ProvideDirectIO(inInstAddress);
                    if (session != null)
                    {
                        session.WriteLine("*IDN?");
                    }
                    else
                    {
                        throw new Exception(string.Format("无法连接到地址： {0}", inInstAddress));
                    }
                }

                string text = session.ReadLine();
                char[] separator = new char[] { ',' };
                return text.Split(separator)[1].Trim();
            }
            finally
            {
                tmpMutex.ReleaseMutex();
            }
        }

        public static string DetermineInstrumentInfo(string inInstAddress, out string outInstIdentity, out string outInstSN, out string outInstOptions, out string outFirmwareVersion, out string outManufator,bool isIdn=true)
        {
            Mutex tmpMutex = new Mutex(false, inInstAddress);

            tmpMutex.WaitOne();
            try
            {
                outInstIdentity = "";
                outInstOptions = "";
                outInstSN = "";
                outFirmwareVersion = "";
                outManufator = "";
                InstrumentIOSession session = null;
                try
                {
                    session = IOManager.ProvideDirectIO(inInstAddress);
                    if (session == null)
                    {
                        return string.Empty;
                    }
                    if (isIdn)
                    {
                        try
                        {
                            session.WriteLine("*IDN?");
                        }
                        catch
                        {
                            ///出错时移除对应的IO连接对象
                            IOManager.Remove(inInstAddress);
                            //再次探测
                            session = IOManager.ProvideDirectIO(inInstAddress);
                            session.WriteLine("*IDN?");
                        }
                        string text = session.ReadLine();
                        char[] separator = new char[] { ',' };
                        string[] strArrays = text.Split(separator);
                        if (strArrays.Length > 2)
                        {
                            outInstSN = strArrays[2].Trim();
                        }
                        if (strArrays.Length > 3)
                        {
                            outFirmwareVersion = strArrays[3].Trim();
                        }
                        string outModelNo = text.Split(separator)[1].Trim();
                        if (strArrays.Length > 0)
                        {
                            outManufator = strArrays[0].Trim();
                        }

                        outInstIdentity = text;

                        outInstOptions = "";
                        return outModelNo;
                    }
                    else
                    {
                        return "用户自定义设备";
                    }
                }
                catch
                {
                    return "";
                }
            }
            finally
            {
                tmpMutex.ReleaseMutex();
            }
        }

        //public static string DetermineInstrumentInfo(string inInstAddress, bool isopt,out string outInstIdentity, out string outInstSN, out string outInstOptions, out string outFirmwareVersion, out string outManufator)
        //{
        //    Mutex tmpMutex = new Mutex(false, inInstAddress);

        //    tmpMutex.WaitOne();
        //    try
        //    {
        //        outInstIdentity = "";
        //        outInstOptions = "";
        //        outInstSN = "";
        //        outFirmwareVersion = "";
        //        outManufator = "";
        //        InstrumentIOSession session = null;
        //        try
        //        {
        //            session = IOManager.ProvideDirectIO(inInstAddress);
        //            if (session == null)
        //            {
        //                return string.Empty;
        //            }
        //            try
        //            {
        //                session.WriteLine("*IDN?");
        //            }
        //            catch
        //            {
        //                ///出错时移除对应的IO连接对象
        //                IOManager.Remove(inInstAddress);
        //                //再次探测
        //                session = IOManager.ProvideDirectIO(inInstAddress);
        //                session.WriteLine("*IDN?");
        //            }
        //            string text = session.ReadLine();
        //            char[] separator = new char[] { ',' };
        //            string[] strArrays = text.Split(separator);
        //            if (strArrays.Length > 2)
        //            {
        //                outInstSN = strArrays[2].Trim();
        //            }
        //            if (strArrays.Length > 3)
        //            {
        //                outFirmwareVersion = strArrays[3].Trim();
        //            }
        //            string outModelNo = text.Split(separator)[1].Trim();
        //            if (strArrays.Length > 0)
        //            {
        //                outManufator = strArrays[0].Trim();
        //            }

        //            outInstIdentity = text;
        //            if (isopt)
        //            {
        //                session.WriteLine("*OPT?");

        //                outInstOptions = session.ReadLine();
        //            }
        //            else
        //            {
        //                outInstOptions = "";
        //            }
        //            return outModelNo;
        //        }
        //        catch
        //        {
        //            return "";
        //        }
        //    }
        //    finally
        //    {
        //        tmpMutex.ReleaseMutex();
        //    }
        //}

        /// <summary>
        /// 返回是否为SCPI仪器
        /// </summary>
        /// <returns></returns>
        public override bool IsScpiInstrument()
        {
            return true;
        }

        /// <summary>
        /// 获得错误堆栈
        /// </summary>
        /// <returns></returns>
        protected string GetErrorQueue()
        {
            return null;
        }

        /// <summary>
        /// 获得下一条错误信息
        /// </summary>
        /// <returns></returns>
        public string GetErrors()
        {
            return this.Query(":SYST:ERR?");
        }

        /// <summary>
        /// 获得仪器的型号，序号和固件版本
        /// </summary>
        protected virtual void DetermineIdentity()
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                base.m_identity = "设备不支持IDN命令";
                throw ex;
            }
        }

        /// <summary>
        /// 发送SCPI命令
        /// </summary>
        /// <param name="A_0"></param>
        public void SendSCPI(string SCPI)
        {
            this.Send(SCPI);
            this.Query("*OPC?", 10000);
        }

        /// <summary>
        /// 删除内部的某波形文件
        /// </summary>
        /// <param name="fileName"></param>
        public virtual void Delete(string fileName)
        {
            this.Send(":MMEMory:DELete \"" + fileName + "\"");
            //查询操作是否已经完成。
            this.Query("*OPC?", 10000);
            this.GetErrorQueue();
        }

        /// <summary>
        /// 返回某位置存储器的当前内容和存储媒体的状态。
        /// This command returns information about the current contents and state of the
        /// mass storage media.
        /// </summary>
        /// <param name="msus">mass storage unit specifieris one of the following:
        /// MAIN: The internal hard disk drive
        /// FLOPPY: The internal floppy disk drive
        /// NET1,NET2,NET3:The network drive 1, 2, or 3 (specified with</param>
        /// <returns></returns>
        public ListDictionary GetCatalog(string msus)
        {
            //2008.12.28 如果没有指明路径，则以当前目录为准。
            string text = "";
            if (msus == "")
            {
                text = this.Query(":MMEMory:CATalog? ");
            }
            else
            {
                text = this.Query(":MMEMory:CATalog? \"" + msus + "\"");
            }
            ListDictionary dictionary = new ListDictionary();
            char[] separator = new char[] { '"' };
            string[] textArray = text.Split(separator);
            for (int i = 1; i < textArray.Length; i += 2)
            {
                char[] chArray2 = new char[] { ',' };
                string[] textArray2 = textArray[i].Split(chArray2);
                string key = textArray2[0];
                uint num2 = uint.Parse(textArray2[2]);
                dictionary.Add(key, num2);
            }
            return dictionary;
        }

        /// <summary>
        /// 读取IEEE块数据
        /// </summary>
        /// <param name="A_0"></param>
        /// <returns></returns>
        public byte[] ReadBlock(string A_0)
        {
            try
            {
                this.m_CommMutex.WaitOne();
                byte[] data;
                this.IO.ReadIEEEBlock(A_0, out data);
                return data;
            }
            finally {
                this.m_CommMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// 决定仪器的选件
        /// </summary>
        protected virtual void DetermineOptions()
        {
            base.m_options = this.Query("*OPT?");
        }

        public virtual void WaitOpc()
        {
            this.Query("*OPC?");
        }
        public virtual void WaitOpc(int inTimeOutInMs)
        {
            if (inTimeOutInMs<this.Timeout)
            {
                inTimeOutInMs = this.Timeout;
            }
            this.Query("*OPC?", inTimeOutInMs);
        }

        protected virtual void SendOpc(string m_Command, int inTimeOutInSec)
        {
            //base.Query(string.Format("{0};OPC?", m_Command), inTimeOutInSec * 1000);
            this.Send(m_Command);
            WaitOpc(inTimeOutInSec * 1000);
        }

    }
}
