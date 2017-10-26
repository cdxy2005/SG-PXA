/* =============================================
 * Copyright @ 2017 北京中创锐科技术有限公司 
 * 名    称：Test_Result  
 * 功    能：Test_Result  
 * 作    者：CHEN XF Administrator
 * 添加时间：2017/7/23 9:50:15
 * =============================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAC_Test.Test
{
    public class Test_Result
    {
        public Test_Result(Test_Paramter parm)
        {
            this.TestParam = parm;
            this.TestTime = DateTime.Now;
        }

        /// <summary>
        /// 测试时间
        /// </summary>
        public DateTime TestTime { get; set; }

        /// <summary>
        /// 测试参数
        /// </summary>
        public Test_Paramter TestParam { get; set; }

        private List<TestResultByFreq> m_TestResultList=new List<TestResultByFreq>();

        public List<TestResultByFreq> TestResultList
        {
            get { return m_TestResultList; }
            set { m_TestResultList = value; }
        }

        private List<TestResultByFreq> m_NoSourceTestResultList = new List<TestResultByFreq>();

        /// <summary>
        /// 无源测试结果
        /// </summary>
        public List<TestResultByFreq> NoSourceTestResultList
        {
            get { return m_NoSourceTestResultList; }
            set { m_NoSourceTestResultList = value; }
        }

    }

    public class TestResultByFreq
    {
        public double TestFreq { get; set; }
        /// <summary>
        /// 输出功率
        /// </summary>
        public double OutPower { get; set; }
        /// <summary>
        /// 无源测试功率
        /// </summary>
        public double MaxClutter { get; set; }
        /// <summary>
        /// 杂散抑制
        /// </summary>
        public double ClutterRejection { get; set; }
        /// <summary>
        /// 发射增益
        /// </summary>
        public double Gain {
            get; set;
        }
        public double Noise { get; set; }

    }
}
