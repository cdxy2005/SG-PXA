//**********************************************
//*类名：TemperatureMonitorConfig
//*作者: Gavin
//*创建时间: 2014/8/4 10:35:46
//*功能: 温度巡检仪配置
//***********************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using RackSys.TestLab.DataAccess;

namespace RackSys.TestLab.Instrument
{
    /// <summary>
    /// 温度巡检仪类型
    /// </summary>
    public enum TemperatureMonitorType
    {
        /// <summary>
        /// XSL/D-16RS2P0V0，西安504现场
        /// </summary>
        XSL_D_16RS2P0V0_XIAN504,
        /// <summary>
        /// KSL,西安504现场
        /// </summary>
        KSL_XIAN504,

    }

    [XmlRoot("温度巡检仪配置")]
    public class TemperatureMonitorConfig
    {
        #region 对外接口
        private static TemperatureMonitorConfig m_CurTemperatureMonitorConfig;

        public static TemperatureMonitorConfig CurTemperatureMonitorConfig
        {
            get 
            {
                if (object.ReferenceEquals(null, m_CurTemperatureMonitorConfig)) 
                {
                    m_CurTemperatureMonitorConfig = new TemperatureMonitorConfig().LoadParameterFromXMLFile();
                }
                return TemperatureMonitorConfig.m_CurTemperatureMonitorConfig; 
            }
        }
        #endregion

        #region 可用温箱集合

        private Dictionary<TemperatureMonitorType, string> m_AvaiableTemperatureMonitor;
        [XmlIgnore()]
        public Dictionary<TemperatureMonitorType, string> AvaiableTemperatureMonitor
        {
            get 
            {
                if (object.ReferenceEquals(null, m_AvaiableTemperatureMonitor)) 
                {
                    m_AvaiableTemperatureMonitor = new Dictionary<TemperatureMonitorType, string>();
                    m_AvaiableTemperatureMonitor.Add(TemperatureMonitorType.XSL_D_16RS2P0V0_XIAN504, "XSL/D-16RS2P0V0");
                    m_AvaiableTemperatureMonitor.Add(TemperatureMonitorType.KSL_XIAN504, "KSL");
                }
                return m_AvaiableTemperatureMonitor; 
            }
        }
        #endregion

        #region 当前温箱选择

        private TemperatureMonitorType m_CurTemperatureMonitorType = TemperatureMonitorType.XSL_D_16RS2P0V0_XIAN504;
        [XmlElement("当前温度巡检仪选择")]
        public TemperatureMonitorType CurTemperatureMonitorType
        {
            get {return m_CurTemperatureMonitorType;}
            set { m_CurTemperatureMonitorType = value; }
        }

        #endregion

        #region 构造函数
        public TemperatureMonitorConfig() { }
        #endregion

        #region 文件读写服务

        private string DefaultPath
        {
            get
            {
                string tmpDir = AppDomain.CurrentDomain.BaseDirectory + "Config\\";
                if (!Directory.Exists(tmpDir)) 
                {
                    Directory.CreateDirectory(tmpDir);
                }
                return tmpDir + "TemperatureMonitorConfig.config";
            }
        }

        public TemperatureMonitorConfig LoadParameterFromXMLFile(string inXmlFileName = null)
        {
            //缺省参数处理
            if ((inXmlFileName == null) || (inXmlFileName == ""))
                inXmlFileName = DefaultPath;

            try
            {
                TemperatureMonitorConfig tmp = (TemperatureMonitorConfig)XmlHelper.LoadParameterFromXMLFile(typeof(TemperatureMonitorConfig), inXmlFileName);
                if (object.ReferenceEquals(null, tmp))
                    return new TemperatureMonitorConfig();
                return tmp;
            }
            catch (Exception)
            {
                return new TemperatureMonitorConfig();
            }
        }

        public void SaveParameterToXMLFile(string inXMLFileName = null)
        {
            if ((inXMLFileName == null) || (inXMLFileName == ""))
                inXMLFileName = DefaultPath;

            XmlHelper.SaveParameterToXMLFile(typeof(TemperatureMonitorConfig), this, inXMLFileName);
        }

        #endregion
    }
}
