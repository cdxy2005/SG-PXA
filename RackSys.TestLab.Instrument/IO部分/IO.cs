using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.IO;

namespace RackSys.TestLab.Instrument
{
    public class IO
    {
        protected FormattedIO488Class m_IO488;

        protected SerialPort m_SerialPort;

        private bool m_Writer;

        public IO()
        {
        }

        public void Clear()
        {
            try
            {
                if (this.m_IO488 != null)
                {
                    this.m_IO488.IO.Clear();
                }
                else if (this.m_SerialPort != null)
                {
                    this.m_SerialPort.DiscardInBuffer();
                    this.m_SerialPort.DiscardOutBuffer();
                }
            }
            catch
            {
            }
        }

        public void Close()
        {
            try
            {
                if (this.m_IO488 != null)
                {
                    this.m_IO488.IO.Close();
                }
                else if (this.m_SerialPort != null && this.m_SerialPort.IsOpen)
                {
                    this.m_SerialPort.Close();
                }
            }
            catch
            {
            }
        }

        public bool InitiateCom(string m_IOName, int m_BaudRate, int m_DataBits, StopBits m_StopBits, Parity m_Parity, bool m_LogFileOn)
        {
            bool flag;
            try
            {
                if (this.m_SerialPort != null && this.m_SerialPort.IsOpen)
                {
                    this.m_SerialPort.Close();
                }
                this.m_SerialPort = new SerialPort(m_IOName)
                {
                    BaudRate = m_BaudRate,
                    DataBits = m_DataBits,
                    StopBits = m_StopBits,
                    Parity = m_Parity,
                    WriteTimeout = 2000,
                    ReadTimeout = 2000,
                    Encoding = Encoding.ASCII
                };
                this.m_SerialPort.Open();
                flag = true;
            }
            catch
            {
                if (this.m_SerialPort != null)
                {
                    this.m_SerialPort.Close();
                }
                flag = false;
            }
            return flag;
        }

        public bool InitiateIO488(string m_IOName, bool m_LogFileOn)
        {
            bool flag;
            try
            {
                string empty = string.Empty;
                ResourceManagerClass resourceManagerClass = new ResourceManagerClass();
                this.m_IO488 = new FormattedIO488Class()
                {
                    IO = (IMessage)resourceManagerClass.Open(m_IOName, AccessMode.NO_LOCK, 20000, "")
                };
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public string Read(string m_Command)
        {
            string empty = string.Empty;
            try
            {
                this.Write(m_Command);
                if (this.m_IO488 != null)
                {
                    empty = this.m_IO488.ReadString();
                }
                else if (this.m_SerialPort != null)
                {
                    Thread.Sleep(100);
                    empty = this.m_SerialPort.ReadExisting();
                }
            }
            catch (IOException oException)
            {
                throw oException;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (exception.Message == "VI_ERROR_NCIC: Not the controller-in-charge")
                {
                    throw exception;
                }
            }
            return empty;
        }

        public string Read(byte[] m_Command)
        {
            string empty = string.Empty;
            try
            {
                this.Write(m_Command);
                if (this.m_IO488 != null)
                {
                    empty = this.m_IO488.ReadString();
                }
                else if (this.m_SerialPort != null)
                {
                    Thread.Sleep(100);
                    empty = this.m_SerialPort.ReadExisting();
                }
            }
            catch (IOException oException)
            {
                throw oException;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (exception.Message == "VI_ERROR_NCIC: Not the controller-in-charge")
                {
                    throw exception;
                }
            }
            return empty;
        }

        public string Read()
        {
            string empty = string.Empty;
            try
            {
                if (this.m_IO488 != null)
                {
                    empty = this.m_IO488.ReadString();
                }
                else if (this.m_SerialPort != null)
                {
                    Thread.Sleep(100);
                    empty = this.m_SerialPort.ReadLine();
                }
            }
            catch (IOException oException)
            {
                throw oException;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (exception.Message == "VI_ERROR_NCIC: Not the controller-in-charge")
                {
                    throw exception;
                }
            }
            return empty;
        }

        public byte[] ReadByte(string m_Command)
        {
            byte[] numArray = new byte[0];
            try
            {
                this.Write(m_Command);
                if (this.m_IO488 != null)
                {
                    numArray = this.m_IO488.IO.Read(1);
                }
                else if (this.m_SerialPort != null)
                {
                    Thread.Sleep(100);
                    this.m_SerialPort.Read(numArray, 0, 1);
                }
            }
            catch (IOException oException)
            {
                throw oException;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (exception.Message == "VI_ERROR_NCIC: Not the controller-in-charge")
                {
                    throw exception;
                }
            }
            return numArray;
        }

        public byte[] ReadByte(byte[] m_Command)
        {
            byte[] numArray = new byte[0];
            try
            {
                this.Write(m_Command);
                if (this.m_IO488 != null)
                {
                    numArray = this.m_IO488.IO.Read(1);
                }
                else if (this.m_SerialPort != null)
                {
                    Thread.Sleep(100);
                    this.m_SerialPort.Read(numArray, 0, 1);
                }
            }
            catch (IOException oException)
            {
                throw oException;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (exception.Message == "VI_ERROR_NCIC: Not the controller-in-charge")
                {
                    throw exception;
                }
            }
            return numArray;
        }

        public byte[] ReadByte()
        {
            byte[] numArray = new byte[0];
            try
            {
                if (this.m_IO488 != null)
                {
                    numArray = this.m_IO488.IO.Read(1);
                }
                else if (this.m_SerialPort != null)
                {
                    Thread.Sleep(100);
                    this.m_SerialPort.Read(numArray, 0, 1);
                }
            }
            catch (IOException oException)
            {
                throw oException;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (exception.Message == "VI_ERROR_NCIC: Not the controller-in-charge")
                {
                    throw exception;
                }
            }
            return numArray;
        }

        public static void ScanIOStatus(ref string[] m_IOAddress, ref string[] m_Description)
        {
            string empty = string.Empty;
            string str = string.Empty;
            try
            {
                ResourceManagerClass resourceManagerClass = new ResourceManagerClass();
                string[] portNames = new string[1];
                FormattedIO488Class formattedIO488Class = new FormattedIO488Class();
                portNames = resourceManagerClass.FindRsrc("GPIB?*");
                for (int i = 1; i < (int)portNames.Length; i++)
                {
                    try
                    {
                        string empty1 = string.Empty;
                        formattedIO488Class = new FormattedIO488Class()
                        {
                            IO = (IMessage)resourceManagerClass.Open(portNames[i], AccessMode.NO_LOCK, 20000, "")
                        };
                        formattedIO488Class.IO.Clear();
                        try
                        {
                            formattedIO488Class.WriteString("*IDN?", true);
                            Thread.Sleep(100);
                            empty1 = formattedIO488Class.ReadString();
                        }
                        catch
                        {
                        }
                        if (!(empty1 != string.Empty) || !(empty1 != "\r\n"))
                        {
                            try
                            {
                                formattedIO488Class.WriteString("ID?", true);
                                Thread.Sleep(100);
                                empty1 = formattedIO488Class.ReadString();
                            }
                            catch
                            {
                            }
                            if (empty1 != string.Empty)
                            {
                                if (empty != string.Empty)
                                {
                                    empty = string.Concat(empty, ";");
                                    str = string.Concat(str, ";");
                                }
                                empty = string.Concat(empty, portNames[i]);
                                str = string.Concat(str, empty1);
                            }
                        }
                        else
                        {
                            if (empty != string.Empty)
                            {
                                empty = string.Concat(empty, ";");
                                str = string.Concat(str, ";");
                            }
                            empty = string.Concat(empty, portNames[i]);
                            str = string.Concat(str, empty1);
                        }
                        formattedIO488Class.IO.Close();
                    }
                    catch
                    {
                    }
                }
                try
                {
                    portNames = resourceManagerClass.FindRsrc("TCPIP0::?*");
                }
                catch
                {
                    portNames = new string[0];
                }
                for (int j = 0; j < (int)portNames.Length; j++)
                {
                    try
                    {
                        formattedIO488Class = new FormattedIO488Class()
                        {
                            IO = (IMessage)resourceManagerClass.Open(portNames[j], AccessMode.NO_LOCK, 20000, "")
                        };
                        formattedIO488Class.IO.Timeout = 2000;
                        formattedIO488Class.WriteString("*IDN?", true);
                        Thread.Sleep(100);
                        string str1 = formattedIO488Class.ReadString();
                        if (str1 != string.Empty)
                        {
                            if (empty != string.Empty)
                            {
                                empty = string.Concat(empty, ";");
                                str = string.Concat(str, ";");
                            }
                            empty = string.Concat(empty, portNames[j]);
                            str = string.Concat(str, str1);
                        }
                    }
                    catch
                    {
                    }
                }
                try
                {
                    portNames = resourceManagerClass.FindRsrc("ASRL?*");
                }
                catch
                {
                    portNames = new string[0];
                }
                for (int k = 0; k < (int)portNames.Length; k++)
                {
                    try
                    {
                        formattedIO488Class = new FormattedIO488Class()
                        {
                            IO = (IMessage)resourceManagerClass.Open(portNames[k], AccessMode.NO_LOCK, 20000, "")
                        };
                        formattedIO488Class.WriteString("*IDN?", true);
                        Thread.Sleep(100);
                        string str2 = formattedIO488Class.ReadString();
                        if (str2 != string.Empty)
                        {
                            if (empty != string.Empty)
                            {
                                empty = string.Concat(empty, ";");
                                str = string.Concat(str, ";");
                            }
                            empty = string.Concat(empty, portNames[k]);
                            str = string.Concat(str, str2);
                        }
                        formattedIO488Class.IO.Close();
                    }
                    catch
                    {
                    }
                }
                try
                {
                    portNames = resourceManagerClass.FindRsrc("USB?*");
                }
                catch
                {
                    portNames = new string[0];
                }
                for (int l = 0; l < (int)portNames.Length; l++)
                {
                    try
                    {
                        formattedIO488Class = new FormattedIO488Class()
                        {
                            IO = (IMessage)resourceManagerClass.Open(portNames[l], AccessMode.NO_LOCK, 20000, "")
                        };
                        formattedIO488Class.WriteString("*IDN?", true);
                        Thread.Sleep(100);
                        string str3 = formattedIO488Class.ReadString();
                        if (str3 != string.Empty)
                        {
                            if (empty != string.Empty)
                            {
                                empty = string.Concat(empty, ";");
                                str = string.Concat(str, ";");
                            }
                            empty = string.Concat(empty, portNames[l]);
                            str = string.Concat(str, str3);
                        }
                        formattedIO488Class.IO.Close();
                    }
                    catch
                    {
                    }
                }
                portNames = SerialPort.GetPortNames();
                for (int m = 0; m < (int)portNames.Length; m++)
                {
                    try
                    {
                        int num = 0;
                        while (true)
                        {
                            string str4 = "*IDN?";
                            SerialPort serialPort = new SerialPort(portNames[m])
                            {
                                BaudRate = 9600,
                                DataBits = 8,
                                Parity = Parity.None
                            };
                            serialPort.Open();
                            Thread.Sleep(300);
                            serialPort.Write("\r\n");
                            for (int n = 0; n < str4.Length; n++)
                            {
                                serialPort.Write(str4.Substring(n, 1));
                                Thread.Sleep(20);
                                if (n == str4.Length - 1)
                                {
                                    serialPort.Write("\r");
                                }
                            }
                            Thread.Sleep(300);
                            string str5 = serialPort.ReadExisting();
                            serialPort.Close();
                            if (!(str5 != string.Empty) || str5.IndexOf("*IDN?") >= 0)
                            {
                                if (num >= 1)
                                {
                                    break;
                                }
                                num++;
                            }
                            else
                            {
                                if (empty != string.Empty)
                                {
                                    empty = string.Concat(empty, ";");
                                    str = string.Concat(str, ";");
                                }
                                empty = string.Concat(empty, portNames[m]);
                                str = string.Concat(str, str5);
                                break;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                    }
                }
            }
            catch
            {
            }
            char[] chrArray = new char[] { ';' };
            m_IOAddress = empty.Split(chrArray);
            char[] chrArray1 = new char[] { ';' };
            m_Description = str.Split(chrArray1);
        }

        public void Timeout(int m_Timeout)
        {
            try
            {
                if (this.m_IO488 != null)
                {
                    this.m_IO488.IO.Timeout = m_Timeout * 1000;
                }
                else if (this.m_SerialPort != null)
                {
                    this.m_SerialPort.ReadTimeout = m_Timeout * 1000;
                    this.m_SerialPort.WriteTimeout = m_Timeout * 1000;
                }
            }
            catch
            {
            }
        }

        public void Write(string m_Command)
        {
            try
            {
                if (this.m_IO488 != null)
                {
                    this.m_IO488.WriteString(m_Command, true);
                }
                else if (this.m_SerialPort != null)
                {
                    for (int i = 0; i < m_Command.Length; i++)
                    {
                        this.m_SerialPort.Write(m_Command.Substring(i, 1));
                        Thread.Sleep(50);
                        if (i == m_Command.Length - 1)
                        {
                            this.m_SerialPort.Write("\r");
                        }
                    }
                }
            }
            catch (IOException oException)
            {
                throw oException;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (exception.Message == "VI_ERROR_NCIC: Not the controller-in-charge")
                {
                    throw exception;
                }
            }
        }

        public void Write(byte[] m_Command)
        {
            try
            {
                if (this.m_IO488 != null)
                {
                    this.m_IO488.WriteNumber(m_Command, IEEEASCIIType.ASCIIType_Any, true);
                }
                else if (this.m_SerialPort != null)
                {
                    this.m_SerialPort.Write(m_Command, 0, (int)m_Command.Length);
                }
            }
            catch (IOException oException)
            {
                throw oException;
            }
        }

        public void Write(byte m_Command)
        {
            try
            {
                if (this.m_IO488 != null)
                {
                    this.m_IO488.WriteNumber(m_Command, IEEEASCIIType.ASCIIType_Any, true);
                }
                else if (this.m_SerialPort != null)
                {
                    this.m_SerialPort.Write(new byte[] { m_Command }, 0, 1);
                }
            }
            catch (IOException oException)
            {
                throw oException;
            }
        }

        public virtual void WriteOpc(string m_Command, int m_Timeout)
        {
            try
            {
                if (this.m_IO488 != null)
                {
                    this.m_IO488.WriteString(m_Command, true);
                    this.m_IO488.WriteString("*OPC?", true);
                }
                string empty = string.Empty;
                int num = 0;
                while (num < m_Timeout * 10)
                {
                    if (this.Read().Length <= 0)
                    {
                        Thread.Sleep(100);
                        num++;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch
            {
            }
        }

        public enum IOType
        {
            GPIB,
            LAN,
            COM,
            USB
        }
    }
}
