using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RackSys.TestLab.Instrument
{
    /// <summary>
    /// 扫描类型
    /// </summary>
    public enum SweepType
    {
        Auto,
        Triggered
    }

    public enum Polarity
    {
        Negative,
        Positive
    }
    /// <summary>
    /// 触发类型，上升沿、下降沿、任何一种
    /// </summary>
    public enum TestEdgeType
    {
        RISing,
        FALLing,
        EITHer
    }
    public enum TestEdgePosition
    {
        UPPer,
        MIDDle,
        LOWer
    }
    /// <summary>
    /// 突发通道名称
    /// </summary>
    public enum TriggerSource
    {
        Channel1,
        Channel2,
        Channel3,
        Channel4,
        Aux,
        Line
    }
    /// <summary>
    /// 测试通道名称
    /// </summary>
    public enum SourceForMeas
    {
        Channel1,
        Channel2,
        Channel3,
        Channel4,
        Function1,
        Function2,
        Function3,
        Function4,
        Function5,
        Function6,
        Function7,
        Function8,
        Function9,
        Function10,
        Function11,
        Function12,
        Function13,
        Function14,
        Function15,
        Function16,
        Wmemory1,
        Wmemory2,
        Wmemory3,
        Wmemory4
    }
    public class ResultTracesAndImage
    {
        /// <summary>
        /// 通道1数据
        /// </summary>
        private double[] m_MeasdataChan1;
        public double[] MeasdataChan1
        {
            get { return m_MeasdataChan1; }
            set { m_MeasdataChan1 = value; }
        }

        /// <summary>
        /// 通道2数据
        /// </summary>
        private double[] m_MeasdataChan2;
        public double[] MeasdataChan2
        {
            get { return m_MeasdataChan2; }
            set { m_MeasdataChan2 = value; }
        }

        /// <summary>
        /// 通道3数据
        /// </summary>
        private double[] m_MeasdataChan3;
        public double[] MeasdataChan3
        {
            get { return m_MeasdataChan3; }
            set { m_MeasdataChan3 = value; }
        }

        /// <summary>
        /// 通道4数据
        /// </summary>
        private double[] m_MeasdataChan4;
        public double[] MeasdataChan4
        {
            get { return m_MeasdataChan4; }
            set { m_MeasdataChan4 = value; }
        }
        /// <summary>
        /// 截图数据
        /// </summary>
        private Image m_ScreenImage;
        public Image ScreenImage
        {
            get { return m_ScreenImage; }
            set { m_ScreenImage = value; }
        }
    }

    public class OSCResult
    {
        private string m_Measurementlabel;
        /// <summary>
        /// 测试名称
        /// </summary>
        public string Measurementlabel
        {
            get { return m_Measurementlabel; }
            set { m_Measurementlabel = value; }
        }

        private double m_MeasurementCurrent;
        /// <summary>
        /// 当前测试数据
        /// </summary>
        public double MeasurementCurrent
        {
            get { return m_MeasurementCurrent; }
            set { m_MeasurementCurrent = value; }
        }

        private double m_MeasurementCount;
        /// <summary>
        /// 测试次数
        /// </summary>
        public double MeasurementCount
        {
            get { return m_MeasurementCount; }
            set { m_MeasurementCount = value; }
        }
        private double m_MeasurementMin;
        /// <summary>
        /// 测试最小值
        /// </summary>
        public double MeasurementMin
        {
            get { return m_MeasurementMin; }
            set { m_MeasurementMin = value; }
        }

        private double m_MeasurementMax;
        /// <summary>
        /// 测试最大值
        /// </summary>
        public double MeasurementMax
        {
            get { return m_MeasurementMax; }
            set { m_MeasurementMax = value; }
        }

        private double m_MeasurementMean;
        /// <summary>
        /// 测试平均值
        /// </summary>
        public double MeasurementMean
        {
            get { return m_MeasurementMean; }
            set { m_MeasurementMean = value; }
        }
        private double m_MeasurementStdDev;
        /// <summary>
        /// 测试标准差
        /// </summary>
        public double MeasurementStdDev
        {
            get { return m_MeasurementStdDev; }
            set { m_MeasurementStdDev = value; }
        }

    }

    public abstract class OSCBase : ScpiInstrument
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="inAddress">visa地址</param>
        protected OSCBase(string inAddress)
            : base(inAddress, false)
        {
        }
     


        #region 对外输出的工厂
        /// <summary>
        /// 作为接收机对外公布接口
        /// </summary>
        /// <param name="visaAddr">visa地址</param>
        /// <param name="inModelKey">选型的key值</param>
        /// <returns>创建的对象</returns>
        public static OSCBase CreateObj(string visaAddr)
        {
            OSCBase tmpObj = null;
            try
            {
                
            }
            catch (Exception) { }
            return tmpObj;
        }
        #endregion

        #region 接口
        /// <summary>
        /// 初始化
        /// </summary>
        abstract public void Preset();

        abstract public  double MEASurePERiod(int chanNumber);
   
        /// <summary>
        /// 读取状态文件
        /// </summary>
        /// <param name="fileName">状态文件名字</param>
        abstract public void LoadStateFile(string fileName);
        /// <summary>
        /// 读取峰峰值
        /// </summary>
        /// <param name="ChannelNumber">选取测试通道</param>
        /// <returns>返回通道峰峰值</returns>
        ///abstract public double ReadPeakToPeak(string ChannelNumber);
        
        /// <summary>
        /// 读取通道间相位值
        /// </summary>
        /// <param name="referenceChannel">参考通道</param>
        /// <param name="measurementChannel">测试通道</param>
        /// <returns></returns>
        abstract public double GetDeltaTime(SourceForMeas ref_1, SourceForMeas meas_1);
        /// <summary>
        /// 获取迹线迹线间的相位
        /// </summary>
        /// <param name="Ref_ChannelNumber">参考迹线</param>
        /// <param name="Meas_ChannelNumber">测试迹线</param>
        /// <returns></returns>
        abstract public double GetDeltaPhase(int Ref_ChannelNumber, int Meas_ChannelNumber);
        /// <summary>
        /// 设置通道间相位参考
        /// </summary>
        /// <param name="Ref_ChannelNumber">参考通道</param>
        /// <param name="Meas_ChannelNumber">测试通道</param>
        abstract public void MeasureDeltaPhase(int Ref_ChannelNumber, int Meas_ChannelNumber);

        abstract public void setChannelDelay(int channel, double time);
        /// <summary>
        /// 设置通道垂直偏差
        /// </summary>
        /// <param name="ChannelNumber">通道</param>
        /// <param name="offset">误差值</param>
        abstract public void SetChannelVerticalOffset(int ChannelNumber, double offset);
        /// <summary>
        /// 获取通道垂直偏差
        /// </summary>
        /// <param name="ChannelNumber">通道</param>
        /// <returns>偏差</returns>
        abstract public double GetChannelVerticalOffset(int ChannelNumber);
        /// <summary>
        /// 时间参考位置
        /// </summary>
        abstract public double TimebasePosition { get; set; }
        /// <summary>
        /// 测试曲线是否翻转
        /// </summary>
        /// <param name="chan"></param>
        /// <param name="IsInVert"></param>
        abstract public void CHANnelINVert(int chan, bool IsInVert);
        /// <summary>
        /// 参考时钟
        /// </summary>
        abstract public bool RefClock { get; set; } 
        /// <summary>
        /// 时间范围
        /// </summary>
        abstract public double TimeRange { get; set; } 
        /// <summary>
        /// 时间标尺
        /// </summary>
        abstract public double TimeScale { get; set; }
        /// <summary>
        /// 通道Display是否打开
        /// </summary>
        /// <param name="channelnumber">通道号</param>
        /// <returns></returns>
        abstract public bool IsChannelDisplayOn(int channelnumber);
        /// <summary>
        /// 设置通道Display
        /// </summary>
        /// <param name="channelnumber">通道号</param>
        /// <param name="state">状态</param>
        abstract public void SetChannelDisplayOn(int channelnumber, bool state);
        /// <summary>
        /// 设置函数功能开关
        /// </summary>
        /// <param name="FunctionNO"></param>
        /// <param name="state"></param>
        abstract public void SetFunctionDisplayOn(int FunctionNO, bool state);
        /// <summary>
        /// 获取通道垂直标尺
        /// </summary>
        /// <param name="ChannelNumber">通道号</param>
        /// <returns></returns>
        abstract public double GetChannelVerticalScale(int ChannelNumber);
        /// <summary>
        /// 获取通道垂直范围
        /// </summary>
        /// <param name="FunctionNO"></param>
        /// <returns></returns>
        abstract public double GetFunctionVerticalRange(int FunctionNO);
        /// <summary>
        /// 功率标尺
        /// </summary>
        /// <param name="dBm">—dBm</param>
        /// <returns></returns>
        abstract public double PowerToScale(double dBm);
        /// <summary>
        /// 设置垂直标尺
        /// </summary>
        /// <param name="ChannelNumber">通道号</param>
        /// <param name="scale">标尺</param>
        abstract public void SetChannelVerticalScale(int ChannelNumber, double scale);
        /// <summary>
        /// 设置函数垂直范围
        /// </summary>
        /// <param name="FunctionNO">函数号</param>
        /// <param name="range">范围</param>
        abstract public void SetFunctionVerticalRange(int FunctionNO, double range);
        /// <summary>
        /// 获取时间迹线
        /// </summary>
        /// <param name="tracename"></param>
        /// <returns></returns>
        abstract public double[] GetTimePosition(SourceForMeas tracename);
        /// <summary>
        /// 函数通道平方值函数设置
        /// </summary>
        /// <param name="FunctionNO"></param>
        /// <param name="ChannleNumber"></param>
        abstract public void SqureFunction(int FunctionNO, int ChannleNumber);
        /// <summary>
        /// 低通滤波器设置
        /// </summary>
        /// <param name="FunctionNO">函数号</param>
        /// <param name="FunctionTotest">测试号</param>
        /// <param name="bandwidth">带宽</param>
        abstract public void LowPassFilter(int FunctionNO, int FunctionTotest, double bandwidth);
        //  abstract public void ScreenShots();
        /// <summary>
        /// 保存屏幕截图
        /// </summary>
        /// <param name="ImageFileName">名字</param>
        /// <returns>图片</returns>
        abstract public Image CaptureScreenImage(string ImageFileName);
        /// <summary>
        /// 平均打开
        /// </summary>
        /// <param name="state">打开或关闭</param>
        abstract public bool AverageEnable { get; set; }
        /// <summary>
        /// 设置平均数
        /// </summary>
        abstract public double AvgNumber { get; set; }
        /// <summary>
        /// 读取全部测试数据
        /// </summary>
        /// <returns></returns>
        ///abstract public List<OSCResult> ReadResult();
        /// <summary>
        /// 读取全部原始测试数据
        /// </summary>
        /// <returns></returns>
        abstract public double[] GetTraceData(SourceForMeas tracename);
        ///abstract public string WaveformSource { get; set; }
        abstract public void Stop();
        abstract public void Single(int time);
        abstract public void Single();
        /// <summary>
        /// 事前查看器 在测试前可以具有清理作用，对Stop、single有效
        /// </summary>
        abstract public bool ReadAcquisitionDoneEvent { get;  }
        /// <summary>
        /// 触发打开
        /// </summary>
        abstract public void Run();
        /// <summary>
        /// 读取幅度
        /// </summary>
        /// <param name="Channel">对应通道</param>
        /// <returns>返回测试幅度值</returns>
        ///abstract public double MeasureVAMPlitude(string Channel);
       
        
        abstract public void AmplitudeDemodulation(int FunctionNumber, int ChannelNumber);

        abstract public void MeasureDeltaTime(SourceForMeas ref_1, SourceForMeas meas_1);

        /// <summary>
/// 测试指定迹线的时延差
/// </summary>
/// <param name="ref_1">参考迹线</param>
/// <param name="startEdgeType">参考迹线沿类型</param>
/// <param name="statEdegePosition">参考位置</param>
/// <param name="statPulseNumber">参考脉冲数</param>
/// <param name="meas_1">测试迹线</param>
/// <param name="stopEdgeType">测试边沿类型</param>
/// <param name="stopEdegePosition">测试迹线测试位置</param>
/// <param name="stopPulseNumber">测试脉冲边缘数</param>
       abstract public  void SetMeasureDeltaTime(SourceForMeas ref_1, TestEdgeType startEdgeType, TestEdgePosition statEdegePosition, int statPulseNumber,
            SourceForMeas meas_1, TestEdgeType stopEdgeType, TestEdgePosition stopEdegePosition, int stopPulseNumber);
        /// <summary>
        /// 将触发极性定义为属性
        /// </summary>
        abstract public Polarity TriggerPolarity { get; set; }
        /// <summary>
        /// 设置输入电阻值 50、1M
        /// </summary>
        /// <param name="Channel"></param>
        /// <param name="impedance"></param>
        abstract public void SetInputImpedance(int Channel, double impedance);
        ///abstract public void SetTriggerPolarity(Polarity polarity_1);
        ///abstract public Polarity GetTriggerPolarity();
        /// 将模拟带宽定义为属性
        /// 
        /// 单位为Hz
        abstract public double BandWidth { get; set; }
        /// 采样率定义为属性
        /// 
        /// </summary>
        abstract public double SampleRate { get; set; }
        /// <summary>
        /// 触发电平
        /// </summary>
        abstract public void TriggerlevelFifty();
        /// <summary>
        /// 触发电平
        /// </summary>
        abstract public double TriggerLevel { get; set; }
        /// <summary>
        /// 触发源设置
        /// </summary>
        abstract public TriggerSource Source { get; set; }
        /// <summary>
        /// 扫描模式设置
        /// </summary>
        abstract public SweepType TriggerSweep { get; set; }

        #region 学超添加16.09.01
        /// <summary>
        /// 清理所有迹线
        /// </summary>
        abstract public void ClearAllMeasure();
       /// <summary>
       /// 过冲测试
       /// </summary>
       /// <param name="meas_1"></param>
        abstract public void MeasureOverShoot(SourceForMeas meas_1);
        /// <summary>
        /// 获取过冲测试结果
        /// </summary>
        /// <param name="meas_1"></param>
        /// <returns></returns>
        abstract public double GetOverShotData(SourceForMeas meas_1);
        /// <summary>
        /// 上升沿测试
        /// </summary>
        /// <param name="meas_1"></param>
        abstract public void MeasureFallTime(SourceForMeas meas_1);
        /// <summary>
        /// 获取上升沿时间
        /// </summary>
        /// <param name="meas_1"></param>
        /// <returns></returns>
        abstract public double GetFallTme(SourceForMeas meas_1);
        /// <summary>
        /// 下降沿测试
        /// </summary>
        /// <param name="meas_1"></param>
        abstract public void MeasureRiseTime(SourceForMeas meas_1);
        /// <summary>
        /// 获取下降沿时间
        /// </summary>
        /// <param name="meas_1"></param>
        /// <returns></returns>
        abstract public double GetRiseTimeData(SourceForMeas meas_1);
        /// <summary>
        /// 电压最大值测试
        /// </summary>
        /// <param name="meas_1"></param>
        abstract public void MeasureVMax(SourceForMeas meas_1);
        /// <summary>
        /// 获取电压最大值
        /// </summary>
        /// <param name="meas_1"></param>
        /// <returns></returns>
        abstract public double GetVMaxData(SourceForMeas meas_1);
        /// <summary>
        /// 脉冲带内最大有效值
        /// </summary>
        /// <param name="meas_1"></param>
        abstract public void MeasureVTop(SourceForMeas meas_1);
        /// <summary>
        /// 获取脉冲最大有效值
        /// </summary>
        /// <param name="meas_1"></param>
        /// <returns></returns>
        abstract public double GetVTopData(SourceForMeas meas_1);

        #endregion
        ///abstract public TriggerSource GetTriggerSource();
        ///abstract public void SetTriggerSource(TriggerSource source_1);
        #endregion

        public delegate string ValidateSupportDelegate(OSCBase Oscillo);

        private static OSCBase.ValidateSupportDelegate m_validateSupportDelegate;
        /// <summary>
        /// 示波器连接
        /// </summary>
        /// <param name="currentAddress"></param>
        /// <param name="supportDelegate"></param>
        /// <param name="interactive"></param>
        /// <returns></returns>
        public static OSCBase Connect(string currentAddress=null, OSCBase.ValidateSupportDelegate supportDelegate=null, bool interactive=true)
        {
            OSCBase Oscillo = null;
            string str = (currentAddress != null ? currentAddress : "GPIB0::18::INSTR");
            OSCBase.m_validateSupportDelegate = supportDelegate;
            if (interactive)
            {
                throw new Exception("不支持交互模式");
            }
            try
            {
                if (OSCBase.DetermineSupport(str) == null)
                {
                    Oscillo = OSCBase.CreateDetectedOscillo(str);
                }
            }
            catch
            {
                //throw;
            }
            OSCBase.m_validateSupportDelegate = null;
            if (Oscillo != null)
            {
                Oscillo.Connected = true;
            }
            return Oscillo;
        }
        /// <summary>
        /// 示波器识别（确认支持该仪表）
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        private static string DetermineSupport(string address)
        {
            if (OSCBase.m_validateSupportDelegate == null)
            {
                return null;
            }
            OSCBase Oscillo = null;
            try
            {
                Oscillo = OSCBase.CreateDetectedOscillo(address);
            }
            catch
            {
                throw;
            }
            if (Oscillo == null)
            {
                return "无法识别对应的示波器";
            }
            return OSCBase.m_validateSupportDelegate(Oscillo);
        }
        /// <summary>
        /// 构造函数，示波器对象建立
        /// </summary>
        public static OSCBase CreateDetectedOscillo(string address)
        {
            OSCBase Oscillo;
            try
            {
                string Model = ScpiInstrument.DetermineModel(address);
                //DSO-X 4104A

                if (Model.IndexOf("DSO9") >= 0)
                {
                    Oscillo = new AgilentDSO90000A(address);
                }
                else if (Model.IndexOf("DSO-X") >= 0)
                {
                    Oscillo = new DSO_X_4104A(address);
                    }
                else
                {
                    throw new Exception(string.Concat(Model, " 无法识别对应的示波器,IP地址：" + address));
                }

            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("连接示波器错误: ", exception.Message));
            }
            return Oscillo;
        }


    }


}
