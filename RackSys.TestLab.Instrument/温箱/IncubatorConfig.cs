//**********************************************
//*类名：IncubatorConfig
//*作者: Gavin
//*创建时间: 2014/6/24 15:12:39
//*功能: 温箱配置选择类，主要负责当前温箱的选择
//***********************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RackSys.TestLab.DataAccess;
using System.IO;
using System.Xml.Serialization;

namespace RackSys.TestLab.Instrument
{
    /// <summary>
    /// 温箱类型
    /// </summary>
    public enum IncubatorType 
    {
        /// <summary>
        /// 施耐德ws110c型号，西安504现场
        /// </summary>
        ACS_WS110C_XIAN504,
        /// <summary>
        /// 富奇7011-5,西安504现场
        /// </summary>
        VOTSCH_7011_5XIAN504,

    }

    [XmlRoot("温箱配置")]
    public class IncubatorConfig
    {
        #region 对外接口
        private static IncubatorConfig m_CurIncubatorConfig;

        public static IncubatorConfig CurIncubatorConfig
        {
            get 
            {
                if (object.ReferenceEquals(null, m_CurIncubatorConfig)) 
                {
                    m_CurIncubatorConfig = new IncubatorConfig().LoadParameterFromXMLFile();
                }
                return IncubatorConfig.m_CurIncubatorConfig; 
            }
        }
        #endregion

        #region 可用温箱集合
        
        private Dictionary<IncubatorType, string> m_AvaiableIncubators;
        [XmlIgnore()]
        public Dictionary<IncubatorType, string> AvaiableIncubators
        {
            get 
            {
                if (object.ReferenceEquals(null, m_AvaiableIncubators)) 
                {
                    m_AvaiableIncubators = new Dictionary<IncubatorType, string>();
                    m_AvaiableIncubators.Add(IncubatorType.ACS_WS110C_XIAN504, "施耐德 WS10C");
                    m_AvaiableIncubators.Add(IncubatorType.VOTSCH_7011_5XIAN504, "富奇 7011-5");
                }
                return m_AvaiableIncubators; 
            }
        }
        #endregion

        #region 当前温箱选择

        private IncubatorType m_CurIncubatorType = IncubatorType.ACS_WS110C_XIAN504;
        [XmlElement("当前温箱选择")]
        public IncubatorType CurIncubatorType
        {
            get {return m_CurIncubatorType;}
            set { m_CurIncubatorType = value; }
        }

        #endregion

        #region 构造函数
        public IncubatorConfig() { }
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
                return tmpDir + "IncubatorConfig.config";
            }
        }

        public IncubatorConfig LoadParameterFromXMLFile(string inXmlFileName = null)
        {
            //缺省参数处理
            if ((inXmlFileName == null) || (inXmlFileName == ""))
                inXmlFileName = DefaultPath;

            try
            {
                IncubatorConfig tmp = (IncubatorConfig)XmlHelper.LoadParameterFromXMLFile(typeof(IncubatorConfig), inXmlFileName);
                if (object.ReferenceEquals(null, tmp))
                    return new IncubatorConfig();
                return tmp;
            }
            catch (Exception)
            {
                return new IncubatorConfig();
            }
        }

        public void SaveParameterToXMLFile(string inXMLFileName = null)
        {
            if ((inXMLFileName == null) || (inXMLFileName == ""))
                inXMLFileName = DefaultPath;

            XmlHelper.SaveParameterToXMLFile(typeof(IncubatorConfig), this, inXMLFileName);
        }

        #endregion
    }
}
