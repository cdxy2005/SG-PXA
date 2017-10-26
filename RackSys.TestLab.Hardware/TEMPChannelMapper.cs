#region Author & Version
/* =============================================
 * Copyright @ 2013 北京中创锐科技术有限公司 
 * 名    称：TEMPChannelMapper 
 * 功    能：TEMPChannelMapper 
 * 作    者：Administrator 
 * 添加时间：2015-07-23 10:17:44 
 * =============================================*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RackSys.TestLab.DataAccess;

namespace RackSys.TestLab.Hardware
{
    /// <summary>
    /// 温检仪映射详情
    /// </summary>
    [Serializable]
    public class TEMPChannelConfig : INotifyPropertyChanged
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
        private bool m_ConfiguredDCPower;
        /// <summary>
        /// 标识是否未配置任何温检仪
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
        /// 温检仪名称
        /// </summary>
        private string m_TEMPDeviceName;
        public string TEMPDeviceName
        {
            get { return m_TEMPDeviceName; }
            set
            {
                if (m_TEMPDeviceName != value)
                {
                    m_TEMPDeviceName = value;
                    NotifyPropertyChanged("TEMPDeviceName");
                    NotifyPropertyChanged("ChannelNameList");
                }
            }
        }

        /// <summary>
        /// 温检仪通道号
        /// </summary>
        private uint m_ChannelNoInDevice;
        /// <summary>
        /// 从1开始编号;0代表没有配置该路温检仪
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
        private string m_TEMPChanneldName;
        public string TEMPChannelName
        {
            get { return m_TEMPChanneldName; }
            set
            {
                if (m_TEMPChanneldName != value)
                {
                    m_TEMPChanneldName = value;
                    NotifyPropertyChanged("TEMPChannelName");
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
        /// 可用的温检仪
        /// </summary>
        [XmlIgnore()]
        public InstrumentInfoList TEMPInstrumentList
        {
            get
            {
                InstrumentInfoList Result = InstrumentInfoList.getInstence().GetInstrumentList(InstrumentType.TemperatureMonitor);
                // Result.Insert(0, new InstrumentInfo() { InstrumentName = "空" });
                return Result;
            }
        }

        [XmlIgnore()]
        public List<string> TEMPInstrumentNameList
        {
            get
            {
                List<string> Result = new List<string>();
                foreach (InstrumentInfo instrument in this.TEMPInstrumentList)
                {
                    Result.Add(instrument.InstrumentName);
                }
                // InstrumentInfoList Result = InstrumentInfoList.getInstence().GetTEMPInstrumentList();
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
                foreach (InstrumentInfo instrument in this.TEMPInstrumentList)
                {
                    if (instrument.InstrumentName == this.TEMPDeviceName)
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

        //private 

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public TEMPChannelConfig()
        {
            this.m_ChannelNameList = new Dictionary<uint, string>();
        }
        #endregion

        #region 重写ToString
        public override string ToString()
        {
            return TEMPChannelName + ";" + TEMPDeviceName + ":" + ChannelNoInDevice.ToString();
        }
        #endregion
    }

    

    /// <summary>
    /// 温检仪映射关系
    /// </summary>
    [XmlRoot("温检仪映射关系")]
    [Serializable]
    public class TEMPChannelMapper : ICloneable
    {
        public static int NeedChannel = 12;

        #region 构造函数
        public TEMPChannelMapper()
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
        private static TEMPChannelMapper m_CurrentTEMPMapper = null;

        /// <summary>
        /// 当前测试参数集合
        /// </summary>
        public static TEMPChannelMapper CurrentTEMPMapper
        {
            get
            {
                if (object.ReferenceEquals(m_CurrentTEMPMapper, null))
                {
                    TEMPChannelMapper temp = new TEMPChannelMapper();
                    //temp.m_CurrentXMLFileName = DefaultFilePath;
                    m_CurrentTEMPMapper = temp.LoadFromFile();


                }
                /*判断是否需要更新
                 * 更新条件为：
                 * 1.如果配置了最多的提供路数，以最多和仪表最大的提供能力为基础
                 * 2.如果全部配置，以仪表能力检查

                {

                }
                 * */

                return m_CurrentTEMPMapper;
            }
        }
        #endregion

        #region 属性


        /// <summary>
        /// 可用的温检仪
        /// </summary>
        [XmlIgnore()]
        public InstrumentInfoList TEMPInstrumentList
        {
            get
            {
                InstrumentInfoList Result = InstrumentInfoList.getInstence().GetInstrumentList(InstrumentType.TemperatureMonitor);
                // Result.Insert(0, new InstrumentInfo() { InstrumentName = "空" });
                return Result;
            }
        }
        /// <summary>
        /// 温检仪通道配置
        /// </summary>
        private List<TEMPChannelConfig> m_TEMPChannelConfigList = new List<TEMPChannelConfig>();

        public List<TEMPChannelConfig> TEMPChannelConfigList
        {
            get { return m_TEMPChannelConfigList; }
            set { m_TEMPChannelConfigList = value; }
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
            for (int i = 0; i < TEMPInstrumentList.Count; i++)
            {
                //电源通道数量
                for (int j = 0; j < TEMPInstrumentList[i].DCModleNum; j++)
                {
                 
                    k++;
                    TEMPChannelConfig config = new TEMPChannelConfig();
                    config.LableName = string.Format("第{0}路", k);
                    config.TEMPDeviceName = TEMPInstrumentList[i].InstrumentName;
                    config.ChannelNoInDevice = (uint)(j + 1);
                    config.TEMPChannelName = string.Format("{0}第{1}路", TEMPInstrumentList[i].InstrumentName, j + 1);
                    config.InstituteID = TEMPInstrumentList[i].IDInInstitute;
                    config.Model = TEMPInstrumentList[i].ModelNo;


                    this.TEMPChannelConfigList.Add(config);

                    if (this.TEMPChannelConfigList.Count >= NeedChannel)
                    {
                        return;
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
                inXMLFileName = AppDomain.CurrentDomain.BaseDirectory + "TEMPMapping.config";
            }

            XmlHelper.SaveParameterToXMLFile(typeof(TEMPChannelMapper), this, inXMLFileName);
        }

        public TEMPChannelMapper LoadFromFile(string inXmlFileName = null)
        {
            //缺省参数处理
            if ((inXmlFileName == null) || (inXmlFileName == ""))
            {
                inXmlFileName = AppDomain.CurrentDomain.BaseDirectory + "TEMPMapping.config";
            }
            try
            {
                TEMPChannelMapper tmpDUTs = (TEMPChannelMapper)XmlHelper.LoadParameterFromXMLFile(typeof(TEMPChannelMapper), inXmlFileName);
                if (tmpDUTs == null)
                {
                    tmpDUTs = new TEMPChannelMapper();
                    tmpDUTs.RestoreDefault();
                }
                /*判断是否需要更新
              * 更新条件为：
              * 1.如果配置了最多的提供路数，以最多和仪表最大的提供能力为基础
              * 2.如果全部配置，以仪表能力检查

             {

             }
              * */
 
                return tmpDUTs;
            }
            catch (Exception ex)
            {
                return new TEMPChannelMapper();
            }
        }
        #endregion

        #region 按照通道号获取映射关系
        /// <summary>
        /// 按通道号返回对应的设备
        /// </summary>
        /// <param name="InChannelID">1开始</param>
        /// <returns></returns>
        public TEMPChannelConfig GetDCPowerChannelInfoByChannelID(int InChannelID)
        {
            if ((InChannelID < 1) || (InChannelID > this.TEMPChannelConfigList.Count()))
            {
                throw new Exception("参数错误，直流电源通道编号超界。");
            }
            return this.TEMPChannelConfigList[InChannelID - 1];
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string[] DCPowerChannelNames
        {
            get
            {
                string[] tmpReuslt = new string[this.TEMPChannelConfigList.Count];
                for (int i = 0; i < this.TEMPChannelConfigList.Count; i++)
                {
                    tmpReuslt[i] = string.Format("{0} {1} Channel{2}\n {3}",
                        new object[]{
                                this.TEMPChannelConfigList[i].TEMPDeviceName,
                                this.TEMPChannelConfigList[i].Model,
                                this.TEMPChannelConfigList[i].ChannelNoInDevice,
                                this.TEMPChannelConfigList[i].TEMPChannelName});
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
        public void Clone(TEMPChannelMapper value)
        {
            if ((this.TEMPChannelConfigList.Count != value.TEMPChannelConfigList.Count) || this.TEMPInstrumentList.Count != this.TEMPInstrumentList.Count)
            {
                throw new Exception("保存文件中电源映射详情信息有误，请检查信息");

            }
            for (int i = 0; i < this.TEMPChannelConfigList.Count; i++)
            {
                this.TEMPChannelConfigList[i].ChannelNoInDevice = value.TEMPChannelConfigList[i].ChannelNoInDevice;
                this.TEMPChannelConfigList[i].ConfiguredDCPower = value.TEMPChannelConfigList[i].ConfiguredDCPower;
                this.TEMPChannelConfigList[i].TEMPChannelName = value.TEMPChannelConfigList[i].TEMPChannelName;
                this.TEMPChannelConfigList[i].TEMPDeviceName = value.TEMPChannelConfigList[i].TEMPDeviceName;
                this.TEMPChannelConfigList[i].InstituteID = value.TEMPChannelConfigList[i].InstituteID;
                this.TEMPChannelConfigList[i].Model = value.TEMPChannelConfigList[i].Model;
            }
            //this.DCPowersCount = value.DCPowersCount;
            // throw new NotImplementedException();

        }
    }
}
