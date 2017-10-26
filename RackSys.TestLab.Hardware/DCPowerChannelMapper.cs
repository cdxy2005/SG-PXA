using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RackSys.TestLab.DataAccess;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RackSys.TestLab.Hardware
{
    /// <summary>
    /// 电源映射详情
    /// </summary>
    public class DCPowerChannelInfo
    {
        private bool m_ConfiguredDCPower;
        /// <summary>
        /// 标识是否未配置任何电源
        /// </summary>
        public bool ConfiguredDCPower
        {
            get { return m_ConfiguredDCPower; }
            set { m_ConfiguredDCPower = value; }
        }
        /// <summary>
        /// 电源名称
        /// </summary>
        private string m_DCPowerDeviceName;
        public string DCPowerDeviceName
        {
            get { return m_DCPowerDeviceName; }
            set { m_DCPowerDeviceName = value; }
        }

        /// <summary>
        /// 电源通道号
        /// </summary>
        private uint m_ChannelNoInDevice;
        /// <summary>
        /// 从1开始编号;0代表没有配置该路电源
        /// </summary>
        public uint ChannelNoInDevice
        {
            get { return m_ChannelNoInDevice; }
            set { m_ChannelNoInDevice = value; }
        }

        /// <summary>
        /// 备注名
        /// </summary>
        private string m_DCPowerChanneldName;
        public string DCPowerChannelName
        {
            get { return m_DCPowerChanneldName; }
            set { m_DCPowerChanneldName = value; }
        }

        private string m_InstituteID;

        /// <summary>
        /// 设备的所内编号
        /// </summary>
        public string InstituteID
        {
            get { return m_InstituteID; }
            set { m_InstituteID = value; }
        }

        private string m_Model;

        /// <summary>
        /// 设备型号
        /// </summary>
        public string Model
        {
            get { return m_Model; }
            set { m_Model = value; }
        }

        //private 

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public DCPowerChannelInfo()
        { }
        #endregion

        #region 重写ToString
        public override string ToString()
        {
            return DCPowerChannelName + ";" + DCPowerDeviceName + ":" + ChannelNoInDevice.ToString();
        }
        #endregion
    }

    /// <summary>
    /// 电源映射详情
    /// </summary>
    [Serializable]
    public class DCChannelConfig : INotifyPropertyChanged
    {
        #region 属性更新消息
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string para)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(para));
            }
        }
        #endregion

        private bool  m_IsSelect;

        /// <summary>
        /// 是否选择
        /// </summary>
        public bool  IsSelect
        {
            get { return m_IsSelect; }
            set
            {
                if (m_IsSelect != value)
                {
                    m_IsSelect = value;
                    NotifyPropertyChanged("IsSelect");
                }
            }
        }
        

        private bool m_ConfiguredDCPower;
        /// <summary>
        /// 标识是否未配置任何电源
        /// </summary>
        public bool ConfiguredDCPower
        {
            get { return m_ConfiguredDCPower; }
            set
            {
                if (m_ConfiguredDCPower != value)
                {
                    m_ConfiguredDCPower = value;
                    NotifyPropertyChanged("ConfiguredDCPower");
                }
            }
        }

        private bool m_CanSetPower;
        /// <summary>
        ///是否可以设置电压电流
        /// </summary>
        public bool CanSetPower
        {
            get { return m_CanSetPower; }
            set
            {
                if (m_CanSetPower != value)
                {
                    m_CanSetPower = value;
                    NotifyPropertyChanged("CanSetPower");
                }
            }
        }


        private bool m_IsSetBatch;
        /// <summary>
        ///是否配制了批次
        /// </summary>
        public bool IsSetBatch
        {
            get { return m_IsSetBatch; }
            set
            {
                if (m_IsSetBatch != value)
                {
                    m_IsSetBatch = value;
                    NotifyPropertyChanged("IsSetBatch");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string m_LableName;
        public string LableName
        {
            get { return m_LableName; }
            set
            {
                if (m_LableName != value)
                {
                    m_LableName = value;
                    NotifyPropertyChanged("LableName");
                }
            }
        }
        /// <summary>
        /// 电源名称
        /// </summary>
        private string m_DCPowerDeviceName;
        public string DCPowerDeviceName
        {
            get { return m_DCPowerDeviceName; }
            set
            {
                if (m_DCPowerDeviceName != value)
                {
                    m_DCPowerDeviceName = value;
                    NotifyPropertyChanged("DCPowerDeviceName");
                    NotifyPropertyChanged("ChannelNameList");
                }
            }
        }

        /// <summary>
        /// 电源通道号
        /// </summary>
        private uint m_ChannelNoInDevice;
        /// <summary>
        /// 从1开始编号;0代表没有配置该路电源
        /// </summary>
        public uint ChannelNoInDevice
        {
            get { return m_ChannelNoInDevice; }
            set
            {
                if (m_ChannelNoInDevice != value)
                {
                    m_ChannelNoInDevice = value;
                    NotifyPropertyChanged("ChannelNoInDevice");
                }
            }
        }

        /// <summary>
        /// 备注名
        /// </summary>
        private string m_DCPowerChanneldName;
        public string DCPowerChannelName
        {
            get { return m_DCPowerChanneldName; }
            set
            {
                if (m_DCPowerChanneldName != value)
                {
                    m_DCPowerChanneldName = value;
                    NotifyPropertyChanged("DCPowerChannelName");
                }
            }
        }

        private string m_InstituteID;

        /// <summary>
        /// 设备的所内编号
        /// </summary>
        public string InstituteID
        {
            get { return m_InstituteID; }
            set
            {
                if (m_InstituteID != value)
                {
                    m_InstituteID = value;
                    NotifyPropertyChanged("InstituteID");
                }
            }
        }

        private string m_Model;

        /// <summary>
        /// 设备型号
        /// </summary>
        public string Model
        {
            get { return m_Model; }
            set
            {
                if (m_Model != value)
                {
                    m_Model = value;
                    NotifyPropertyChanged("Model");
                }
            }
        }



        /// <summary>
        /// 可用的电源
        /// </summary>
        [XmlIgnore()]
        public InstrumentInfoList DCPowerInstrumentList
        {
            get
            {
                InstrumentInfoList Result = InstrumentInfoList.getInstence().GetDCPowerInstrumentList();
                // Result.Insert(0, new InstrumentInfo() { InstrumentName = "空" });
                return Result;
            }
        }

        [XmlIgnore()]
        public List<string> DCPowerInstrumentNameList
        {
            get
            {
                List<string> Result = new List<string>();
                foreach (InstrumentInfo instrument in this.DCPowerInstrumentList)
                {
                    Result.Add(instrument.InstrumentName);
                }
                // InstrumentInfoList Result = InstrumentInfoList.getInstence().GetDCPowerInstrumentList();
                // Result.Insert(0, new InstrumentInfo() { InstrumentName = "空" });
                return Result;
            }
        }

        private Dictionary<uint, string> m_ChannelNameList;
        [XmlIgnore()]
        public Dictionary<uint, string> ChannelNameList
        {
            get
            {
                m_ChannelNameList.Clear();
                foreach (InstrumentInfo instrument in this.DCPowerInstrumentList)
                {
                    if (instrument.InstrumentName == this.DCPowerDeviceName)
                    {
                        for (int i = 0; i < instrument.DCModleNum; i++)
                        {
                            this.m_ChannelNameList.Add((uint)i + 1, string.Format("第{0}路", i + 1));
                        }

                    }
                }

                return m_ChannelNameList;
            }

        }

        public int ChannelNumber = 0;
        public int GetChannelNumber()
        {
            if (ChannelNumber != 0)
                return ChannelNumber;

            if (string.IsNullOrEmpty(LableName))            
                return 0;


            char[] tmpBuf = LableName.ToCharArray();
            //for (int index = 1; index < tmpBuf.Length - 1; index++)
            //{
            //    两位数时
            //    tmp +=  int.Parse(tmpBuf[index].ToString()) * 10e(index - 1);
            //}
            ChannelNumber = int.Parse(tmpBuf[1].ToString());
            return ChannelNumber;
        }
        //private 

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public DCChannelConfig()
        {
            this.m_ChannelNameList = new Dictionary<uint, string>();
        }
        #endregion

        #region 重写ToString
        public override string ToString()
        {
            return DCPowerChannelName + ";" + DCPowerDeviceName;
        }
        //public override string ToString()
        //{
        //    return DCPowerChannelName + ";" + DCPowerDeviceName + ":" + ChannelNoInDevice.ToString();
        //}

        public override bool Equals(object obj)
        {
            DCChannelConfig compareDC = obj as DCChannelConfig;
            if (compareDC != null)
            {
                return this.LableName == compareDC.LableName;                
            } 
            else
            {
                return false;
            }
        }


        #endregion

        /// <summary>
        /// 克隆电源设置 
        /// </summary>
        /// <param name="pearentDC"></param>
        /// <returns></returns>
        public static DCChannelConfig Clone(DCChannelConfig pearentDC)
        {
            DCChannelConfig sonDC = new DCChannelConfig();
            sonDC.CanSetPower = pearentDC.CanSetPower;
            sonDC.ChannelNoInDevice = pearentDC.ChannelNoInDevice;
            sonDC.ChannelNumber = pearentDC.ChannelNumber;
            sonDC.ConfiguredDCPower = pearentDC.ConfiguredDCPower;
            sonDC.DCPowerChannelName = pearentDC.DCPowerChannelName;
            sonDC.DCPowerDeviceName = pearentDC.DCPowerDeviceName;
            sonDC.InstituteID = pearentDC.InstituteID;
            sonDC.IsSelect = pearentDC.IsSelect;
            sonDC.IsSetBatch = pearentDC.IsSetBatch;
            sonDC.LableName = pearentDC.LableName;

            return sonDC;
        }
    }

    /// <summary>
    /// 电源映射关系
    /// </summary>
    [XmlRoot("电源映射关系")]
    public class DCPowerChannelMapper
    {
        #region 变量
        private const uint DCPOWERS_COUNT = 2;
        #endregion

        #region 构造函数
        public DCPowerChannelMapper()
        {
        }
        #endregion

        #region 属性
        /// <summary>
        /// 系统配置的电源路数
        /// </summary>
        [XmlIgnore()]
        public uint DCPowersCount
        {
            get { return DCPOWERS_COUNT; }
        }

        /// <summary>
        /// 可用的电源
        /// </summary>
        [XmlIgnore()]
        public InstrumentInfoList DCPowerInstrumentList
        {
            get
            {
                InstrumentInfoList Result = InstrumentInfoList.getInstence().GetDCPowerInstrumentList();
                Result.Insert(0, new InstrumentInfo() { InstrumentName = "空" });
                return Result;
            }
        }

        /// <summary>
        /// 系统选择的电源配置
        /// </summary>
        private DCPowerChannelInfo[] m_DCPowerChannelInfos = new DCPowerChannelInfo[DCPOWERS_COUNT];
        [XmlElement("6路电源")]
        public DCPowerChannelInfo[] DCPowerChannelInfos
        {
            get { return m_DCPowerChannelInfos; }
            set { m_DCPowerChannelInfos = value; }
        }

        /// <summary>
        /// 通道间延时（单位），毫秒
        /// </summary>
        private int m_DelayBetweenChannelInMs = 100;

        /// <summary>
        /// 通道间延时（单位），毫秒
        /// </summary>
        [XmlElement("加电延时")]
        public int DelayBetweenChannelInMs
        {
            get { return m_DelayBetweenChannelInMs; }
            set { m_DelayBetweenChannelInMs = value; }
        }

        /// <summary>
        /// 通道间延时（单位），毫秒
        /// </summary>
        private int m_DelayPowerOffInMs = 100;

        /// <summary>
        /// 通道间延时（单位），毫秒
        /// </summary>
        [XmlElement("下电延时")]
        public int DelayPowerOffInMs
        {
            get { return m_DelayPowerOffInMs; }
            set { m_DelayPowerOffInMs = value; }
        }

        #endregion

        #region 数据操作
        /// <summary>
        /// 恢复默认参数
        /// </summary>
        public void RestoreDefault()
        {
            m_DCPowerChannelInfos = new DCPowerChannelInfo[2];
            //电源数量
            int k = 0;
            for (int i = 0; i < DCPowerInstrumentList.Count; i++)
            {
                //电源通道数量
                for (int j = 0; j < DCPowerInstrumentList[i].DCModleNum; j++)
                {
                    //超过6路电源则不再使用多余电源
                    if (k > 1)
                    {
                        break;
                    }
                    this.m_DCPowerChannelInfos[k] = new DCPowerChannelInfo()
                        {
                            DCPowerDeviceName = DCPowerInstrumentList[i].InstrumentName,
                            ChannelNoInDevice = (uint)(j + 1),
                            DCPowerChannelName = "",
                            InstituteID = DCPowerInstrumentList[i].IDInInstitute,
                            Model = DCPowerInstrumentList[i].ModelNo
                        };
                    k++;
                }
            }
        }

        /// <summary>
        /// 检查当前配置是否存在于DCPowerChannelInfos当中
        /// </summary>
        public void CheckDCPowerChannelInfos()
        {
            //DCPowerChannelInfo tmpDCPowerChannelInfo in DCPowerChannelInfos
            for (int i = 0; i < DCPowerChannelInfos.Length; i++)
            {
                if (null != DCPowerChannelInfos[i])
                {
                    var result = from InstrumentInfo instrumentInfo in DCPowerInstrumentList
                                 where instrumentInfo.InstrumentName == DCPowerChannelInfos[i].DCPowerDeviceName
                                 && (instrumentInfo.DCModleNum >= DCPowerChannelInfos[i].ChannelNoInDevice)//字符串是以A开头，并且长度为4位的
                                 select instrumentInfo;
                    //查不到该条记录,则删除该记录
                    if (null == result)
                    {
                        //赋值为空
                        DCPowerChannelInfos[i] = null;
                    }
                }
            }
        }
        #endregion

        #region 读取和保存参数

        public void SaveToXMLFile(string inXMLFileName = null)
        {
            if ((inXMLFileName == null) || (inXMLFileName == ""))
            {
                inXMLFileName = AppDomain.CurrentDomain.BaseDirectory + "DCPowerMapping.config";
            }

            XmlHelper.SaveParameterToXMLFile(typeof(DCPowerChannelMapper), this, inXMLFileName);
        }

        public DCPowerChannelMapper LoadFromFile(string inXmlFileName)
        {
            //缺省参数处理
            if ((inXmlFileName == null) || (inXmlFileName == ""))
            {
                inXmlFileName = AppDomain.CurrentDomain.BaseDirectory + "DCPowerMapping.config";
            }
            try
            {
                DCPowerChannelMapper tmpDUTs = (DCPowerChannelMapper)XmlHelper.LoadParameterFromXMLFile(typeof(DCPowerChannelMapper), inXmlFileName);
                if (tmpDUTs == null)
                {
                    tmpDUTs = new DCPowerChannelMapper();
                    tmpDUTs.RestoreDefault();
                }
                if (tmpDUTs.m_DCPowerChannelInfos.Count() < 2)
                {
                    tmpDUTs.RestoreDefault();
                }
                return tmpDUTs;
            }
            catch (Exception ex)
            {
                return new DCPowerChannelMapper();
            }
        }
        #endregion

        #region 按照通道号获取映射关系
        /// <summary>
        /// 按通道号返回对应的设备
        /// </summary>
        /// <param name="InChannelID">1开始</param>
        /// <returns></returns>
        public DCPowerChannelInfo GetDCPowerChannelInfoByChannelID(int InChannelID)
        {
            if ((InChannelID < 1) || (InChannelID > this.m_DCPowerChannelInfos.Count()))
            {
                throw new Exception("参数错误，直流电源通道编号超界。");
            }
            return this.m_DCPowerChannelInfos[InChannelID - 1];
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string[] DCPowerChannelNames
        {
            get
            {
                string[] tmpReuslt = new string[this.DCPowerChannelInfos.Length];
                for (int i = 0; i < this.DCPowerChannelInfos.Length; i++)
                {
                    tmpReuslt[i] = string.Format("{0} {1} Channel{2}\n {3}",
                        new object[]{
                                this.DCPowerChannelInfos[i].DCPowerDeviceName,
                                this.DCPowerChannelInfos[i].Model,
                                this.DCPowerChannelInfos[i].ChannelNoInDevice,
                                this.DCPowerChannelInfos[i].DCPowerChannelName});
                }
                return tmpReuslt;
            }
        }

        public string[] PowerInfoLabels
        {
            get
            {
                string[] DCLabels = this.DCPowerChannelNames;
                List<string> tmpResult = new List<string>();
                for (int i = 0; i < Math.Min(6, DCLabels.Length); i++)
                {
                    tmpResult.Add(DCLabels[i]);
                    tmpResult.Add(DCLabels[i]);
                    tmpResult.Add(DCLabels[i]);
                }

                return tmpResult.ToArray();
            }
        }


        /// <summary>
        /// 拷贝 信息更新内存中当前对像信息（因为使用绑定，不能进行对像的替换）
        /// </summary>
        /// <param name="value"></param>
        public void Clone(DCPowerChannelMapper value)
        {
            if ((this.DCPowerChannelInfos.Length != value.DCPowerChannelInfos.Length) || this.DCPowerInstrumentList.Count != this.DCPowerInstrumentList.Count)
            {
                throw new Exception("保存文件中电源映射详情信息有误，请检查信息");

            }
            for (int i = 0; i < this.DCPowerChannelInfos.Length; i++)
            {
                this.DCPowerChannelInfos[i].ChannelNoInDevice = value.DCPowerChannelInfos[i].ChannelNoInDevice;
                this.DCPowerChannelInfos[i].ConfiguredDCPower = value.DCPowerChannelInfos[i].ConfiguredDCPower;
                this.DCPowerChannelInfos[i].DCPowerChannelName = value.DCPowerChannelInfos[i].DCPowerChannelName;
                this.DCPowerChannelInfos[i].DCPowerDeviceName = value.DCPowerChannelInfos[i].DCPowerDeviceName;
                this.DCPowerChannelInfos[i].InstituteID = value.DCPowerChannelInfos[i].InstituteID;
                this.DCPowerChannelInfos[i].Model = value.DCPowerChannelInfos[i].Model;
            }
            //this.DCPowersCount = value.DCPowersCount;
            // throw new NotImplementedException();

        }
    }

    /// <summary>
    /// 电源映射关系
    /// </summary>
    [XmlRoot("电源映射关系")]
    [Serializable]
    public class DCChannelConfigMapper : ICloneable
    {


        #region 构造函数
        public DCChannelConfigMapper()
        {
        }
        #endregion

        #region ICloneable 成员
        public object Clone()
        {


            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Seek(0, 0);
            object value = bf.Deserialize(ms);
            ms.Close();
            return value;
            //this.MemberwiseClone();
        }
        #endregion


        #region 访问句柄
        private static DCChannelConfigMapper m_CurrentDCMapper = null;

        /// <summary>
        /// 当前测试参数集合
        /// </summary>
        public static DCChannelConfigMapper CurrentDCMapper
        {
            get
            {
                if (object.ReferenceEquals(m_CurrentDCMapper, null))
                {
                    DCChannelConfigMapper temp = new DCChannelConfigMapper();
                    //temp.m_CurrentXMLFileName = DefaultFilePath;
                    m_CurrentDCMapper = temp.LoadFromFile();


                }
                /*判断是否需要更新
                 * 更新条件为：
                 * 1.如果配置了最多的提供路数，以最多和仪表最大的提供能力为基础
                 * 2.如果全部配置，以仪表能力检查

                {

                }
                 * */

                return m_CurrentDCMapper;
            }
        }
        #endregion

        #region 属性


        /// <summary>
        /// 可用的电源
        /// </summary>
        [XmlIgnore()]
        public InstrumentInfoList DCPowerInstrumentList
        {
            get
            {
                InstrumentInfoList Result = InstrumentInfoList.getInstence().GetDCPowerInstrumentList();
                // Result.Insert(0, new InstrumentInfo() { InstrumentName = "空" });
                return Result;
            }
        }

        /// <summary>
        /// 系统选择的电源配置
        /// </summary>
        private List<DCChannelConfig> m_DCChannelConfigList = new List<DCChannelConfig>();

        public List<DCChannelConfig> DCChannelConfigList
        {
            get { return m_DCChannelConfigList; }
            set { m_DCChannelConfigList = value; }
        }

        /// <summary>
        /// 通道间延时（单位），毫秒
        /// </summary>
        private int m_DelayBetweenChannelInMs = 100;

        /// <summary>
        /// 通道间延时（单位），毫秒
        /// </summary>
        [XmlElement("加电延时")]
        public int DelayBetweenChannelInMs
        {
            get { return m_DelayBetweenChannelInMs; }
            set { m_DelayBetweenChannelInMs = value; }
        }

        /// <summary>
        /// 通道间延时（单位），毫秒
        /// </summary>
        private int m_DelayPowerOffInMs = 100;

        /// <summary>
        /// 通道间延时（单位），毫秒
        /// </summary>
        [XmlElement("下电延时")]
        public int DelayPowerOffInMs
        {
            get { return m_DelayPowerOffInMs; }
            set { m_DelayPowerOffInMs = value; }
        }

        #endregion

        #region 数据操作
        /// <summary>
        /// 恢复默认参数
        /// </summary>
        public void RestoreDefault()
        {

            //电源数量
            int k = 0;
            for (int i = 0; i < DCPowerInstrumentList.Count; i++)
            {
                //电源通道数量
                for (int j = 0; j < DCPowerInstrumentList[i].DCModleNum; j++)
                {
                    k++;
                    DCChannelConfig config = new DCChannelConfig();
                    config.LableName = string.Format("第{0}路", k);
                    config.DCPowerDeviceName = DCPowerInstrumentList[i].InstrumentName;
                    config.ChannelNoInDevice = (uint)(j + 1);
                    config.DCPowerChannelName = string.Format("{0}第{1}路", DCPowerInstrumentList[i].InstrumentName, j + 1);
                    config.InstituteID = DCPowerInstrumentList[i].IDInInstitute;
                    config.Model = DCPowerInstrumentList[i].ModelNo;
                    config.ConfiguredDCPower = true;

                    this.DCChannelConfigList.Add(config);

                }
            }
        }

        /// <summary>
        /// 恢复默认参数
        /// </summary>
        public int  GetSupplyDcNum()
        {

            //电源数量
            int k = 0;
            for (int i = 0; i < DCPowerInstrumentList.Count; i++)
            {
                //电源通道数量
                for (int j = 0; j < DCPowerInstrumentList[i].DCModleNum; j++)
                {
                    k++;
                 
                }
            }
            return k;
        }


        /// <summary>
        /// 检查当前配置是否存在于DCPowerChannelInfos当中
        /// </summary>
        public void CheckDCPowerChannelInfos()
        {
            //DCPowerChannelInfo tmpDCPowerChannelInfo in DCPowerChannelInfos
            for (int i = 0; i < DCChannelConfigList.Count; i++)
            {
                if (null != DCChannelConfigList[i])
                {
                    var result = from InstrumentInfo instrumentInfo in DCPowerInstrumentList
                                 where instrumentInfo.InstrumentName == DCChannelConfigList[i].DCPowerDeviceName
                                 && (instrumentInfo.DCModleNum >= DCChannelConfigList[i].ChannelNoInDevice)//字符串是以A开头，并且长度为4位的
                                 select instrumentInfo;
                    //查不到该条记录,则删除该记录
                    if (null == result)
                    {
                        //赋值为空
                        DCChannelConfigList[i] = null;
                    }
                }
            }
        }
        #endregion

        #region 读取和保存参数

        public void SaveToXMLFile(string inXMLFileName = null)
        {
            if ((inXMLFileName == null) || (inXMLFileName == ""))
            {
                inXMLFileName = AppDomain.CurrentDomain.BaseDirectory + "DCPowerMapping.config";
            }

            XmlHelper.SaveParameterToXMLFile(typeof(DCChannelConfigMapper), this, inXMLFileName);
        }

        public DCChannelConfigMapper LoadFromFile(string inXmlFileName = null)
        {
            //缺省参数处理
            if ((inXmlFileName == null) || (inXmlFileName == ""))
            {
                inXmlFileName = AppDomain.CurrentDomain.BaseDirectory + "DCPowerMapping.config";
            }
            try
            {
                DCChannelConfigMapper tmpDUTs = (DCChannelConfigMapper)XmlHelper.LoadParameterFromXMLFile(typeof(DCChannelConfigMapper), inXmlFileName);
                if (tmpDUTs == null)
                {
                    tmpDUTs = new DCChannelConfigMapper();
                    tmpDUTs.RestoreDefault();
                }
                else
                {
                    int supplyDcNum = this.GetSupplyDcNum();
                    if (tmpDUTs.DCChannelConfigList.Count != supplyDcNum)
                    {
                        tmpDUTs = new DCChannelConfigMapper();
                        tmpDUTs.RestoreDefault();
 
                    }
                }
                /*判断是否需要更新
              * 更新条件为：
              * 1.如果配置了最多的提供路数，以最多和仪表最大的提供能力为基础
              * 2.如果全部配置，以仪表能力检查

             {

             }
              * */

                //if (tmpDUTs.m_DCPowerChannelInfos.Count() < 2)
                //{
                //    tmpDUTs.RestoreDefault();
                //}
                return tmpDUTs;
            }
            catch (Exception ex)
            {
                return new DCChannelConfigMapper();
            }
        }
        #endregion

        #region 按照通道号获取映射关系
        /// <summary>
        /// 按通道号返回对应的设备
        /// </summary>
        /// <param name="InChannelID">1开始</param>
        /// <returns></returns>
        public DCChannelConfig GetDCPowerChannelInfoByChannelID(int InChannelID)
        {
            if ((InChannelID < 1) || (InChannelID > this.DCChannelConfigList.Count()))
            {
                throw new Exception("参数错误，直流电源通道编号超界。");
            }
            return this.DCChannelConfigList[InChannelID - 1];
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string[] DCPowerChannelNames
        {
            get
            {
                string[] tmpReuslt = new string[this.DCChannelConfigList.Count];
                for (int i = 0; i < this.DCChannelConfigList.Count; i++)
                {
                    tmpReuslt[i] = string.Format("{0} {1} Channel{2}\n {3}",
                        new object[]{
                                this.DCChannelConfigList[i].DCPowerDeviceName,
                                this.DCChannelConfigList[i].Model,
                                this.DCChannelConfigList[i].ChannelNoInDevice,
                                this.DCChannelConfigList[i].DCPowerChannelName});
                }
                return tmpReuslt;
            }
        }

        public string[] PowerInfoLabels
        {
            get
            {
                string[] DCLabels = this.DCPowerChannelNames;
                List<string> tmpResult = new List<string>();
                for (int i = 0; i < Math.Min(6, DCLabels.Length); i++)
                {
                    tmpResult.Add(DCLabels[i]);
                    tmpResult.Add(DCLabels[i]);
                    tmpResult.Add(DCLabels[i]);
                }

                return tmpResult.ToArray();
            }
        }


        /// <summary>
        /// 拷贝 信息更新内存中当前对像信息（因为使用绑定，不能进行对像的替换）
        /// </summary>
        /// <param name="value"></param>
        public void Clone(DCChannelConfigMapper value)
        {
            if ((this.DCChannelConfigList.Count != value.DCChannelConfigList.Count) || this.DCPowerInstrumentList.Count != this.DCPowerInstrumentList.Count)
            {
                throw new Exception("保存文件中电源映射详情信息有误，请检查信息");

            }
            for (int i = 0; i < this.DCChannelConfigList.Count; i++)
            {
                this.DCChannelConfigList[i].ChannelNoInDevice = value.DCChannelConfigList[i].ChannelNoInDevice;
                this.DCChannelConfigList[i].ConfiguredDCPower = value.DCChannelConfigList[i].ConfiguredDCPower;
                this.DCChannelConfigList[i].DCPowerChannelName = value.DCChannelConfigList[i].DCPowerChannelName;
                this.DCChannelConfigList[i].DCPowerDeviceName = value.DCChannelConfigList[i].DCPowerDeviceName;
                this.DCChannelConfigList[i].InstituteID = value.DCChannelConfigList[i].InstituteID;
                this.DCChannelConfigList[i].Model = value.DCChannelConfigList[i].Model;
            }
            //this.DCPowersCount = value.DCPowersCount;
            // throw new NotImplementedException();

        }
    }
}
