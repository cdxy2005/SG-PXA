using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using RackSys.TestLab.Visa;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    /// <summary>
    /// IO管理器
    /// </summary>
    public class IOManager
    {
        private static ListDictionary m_InstrumentSessions = new ListDictionary();

        private static bool IsItDeadYet()
        {
            return false;
        }

        public static void Remove(string inInstAddr)
        {
            object obj2 = m_InstrumentSessions[inInstAddr];
            if (obj2 != null)
            {
                InstrumentIOSession session;
                try
                {
                    session = (InstrumentIOSession)obj2;
                }
                catch
                {
                    m_InstrumentSessions.Remove(inInstAddr);
                    return;
                }
                try
                {
                    session.Close();
                }
                catch
                {
                }
                m_InstrumentSessions.Remove(inInstAddr);
            }
        }

        public static void RemoveAll()
        {
            string[] array = new string[m_InstrumentSessions.Keys.Count];
            m_InstrumentSessions.Keys.CopyTo(array, 0);
            foreach (string text in array)
            {
                Remove(text);
            }
        }

       private static Mutex m_CommMutex = new Mutex(false, "IOManager");

        /// <summary>
        /// 为某地址提供一个数据连接通道
        /// </summary>
        /// <param name="inInstAddr"></param>
        /// <returns></returns>
        public static InstrumentIOSession ProvideDirectIO(string inInstAddr)
        {
            try
            {
                m_CommMutex.WaitOne();
                InstrumentIOSession session;
                if (IsItDeadYet())
                {
                    throw new InvalidOperationException();
                }
                object obj2 = m_InstrumentSessions[inInstAddr];
                if (obj2 == null)
                {
                    session = InstrumentIOManager.CreateSession(inInstAddr);
                    if ((session == null) || session.IsConnected)
                    {
                        return session;
                    }
                    if (session.Connect())
                    {
                        session.TimeoutInMs = 10000;
                        m_InstrumentSessions.Add(inInstAddr, session);
                        return session;
                    }
                    throw new Exception(string.Format("打开{0}设备失败！请检查设备连接", inInstAddr));
                    return null;
                }
                session = (InstrumentIOSession)obj2;
                if (session.IsConnected)
                {
                    return session;
                }
                if (session.Connect())
                {
                    session.TimeoutInMs = 10000;// 0x1388;
                    return session;
                }
                Remove(inInstAddr);
                return null;
            }
            finally
            {
                m_CommMutex.ReleaseMutex();
            }
        }
    }


}
