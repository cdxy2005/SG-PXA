
using RackSys.TestLab.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAC_Test.Test
{
    public class Test_ResultForExcel
    {
        private Test_Result m_TestResult;


        public Test_ResultForExcel(Test_Result inTestResult)
        {
            this.m_TestResult = inTestResult;
        }

        #region 条件
        [ExcelCell("C3")]
        public string TestStartTime
        {
            get
            {
                if (this.m_TestResult != null)
                {
                    return this.m_TestResult.TestTime.ToString("yyyy-MM-dd HH:mm");
                }
                return "";

            }
        }

        [ExcelCell("E3")]
        public string DUTName
        {
            get
            {
                if (this.m_TestResult != null && this.m_TestResult.TestParam != null)
                {
                    return this.m_TestResult.TestParam .DUTName;
                }
                return "";

            }
        }

        [ExcelCell("G3")]
        public object TestPower
        {
            get
            {
                if (this.m_TestResult != null && this.m_TestResult.TestParam != null)
                {
                    return this.m_TestResult.TestParam.SGAndPxaParamter.InPower;
                }
                return "";

            }
        }

        [ExcelCell("C4")]
        public object RBW
        {
            get
            {
                if (this.m_TestResult != null && this.m_TestResult.TestParam != null)
                {
                    return this.m_TestResult.TestParam.SGAndPxaParamter.RBW * 1e-6;
                }
                return "";

            }
        }
        [ExcelCell("E4")]
        public object VBW
        {
            get
            {
                if (this.m_TestResult != null && this.m_TestResult.TestParam != null)
                {
                    return this.m_TestResult.TestParam.SGAndPxaParamter.VBW * 1e-6;
                }
                return "";

            }
        }
        //[ExcelCell("G4")]
        //public object TestFreq
        //{
        //    get
        //    {
        //        if (this.m_TestResult != null && this.m_TestResult.TestParam != null)
        //        {
        //            return this.m_TestResult.TestParam.SGAndPxaParamter.TestFreq * 1e-6;
        //        }
        //        return "";

        //    }
        //}

        [ExcelCell("C5")]
        public object IngroeFreq
        {
            get
            {
                if (this.m_TestResult != null && this.m_TestResult.TestParam != null)
                {
                    return this.m_TestResult.TestParam.SGAndPxaParamter.IngroeFreq * 1e-6;
                }
                return "";

            }
        }
        [ExcelCell("E5")]
        public string InputFile
        {
            get
            {
                if (this.m_TestResult != null && this.m_TestResult.TestParam != null)
                {
                    return this.m_TestResult.TestParam.InputFile;
                }
                return "";

            }
        }
        [ExcelCell("G5")]
        public string OutputFile
        {
            get
            {
                if (this.m_TestResult != null && this.m_TestResult.TestParam != null)
                {
                    return this.m_TestResult.TestParam.OutputFile;
                }
                return "";

            }
        }
        #endregion

        #region 结果

        [ExcelCol("B8")]
        public int[] Order
        {
            get
            {
                int[] order = new int[0];
                if (this.m_TestResult != null )
                {
                    order = new int[m_TestResult.TestResultList.Count];
                    for (int i = 0; i < m_TestResult.TestResultList.Count; i++)
                    {
                        order[i] = i + 1;
                    }
                }
                return order;

            }
        }
        /// <summary>
        /// 测试频率
        /// </summary>
        [ExcelRow("C8")]
        public object[,] Result
        {
            get
            {
                object[,] tempResult = new object[0, 5];
                
                if (this.m_TestResult != null && this.m_TestResult.TestParam != null)
                {
                    var minGain = (from q in m_TestResult.TestResultList
                                   select q.Gain).Min();
                    tempResult = new object[m_TestResult.TestResultList.Count,7];
                    for (int i = 0; i < m_TestResult.TestResultList.Count; i++)
                    {
                        tempResult[i,0] = m_TestResult.TestResultList[i].TestFreq*1e-6;
                        if (m_TestResult.TestParam.IsPowerTest)
                        {
                            tempResult[i, 1] = m_TestResult.TestResultList[i].OutPower;
                            tempResult[i, 2] = m_TestResult.TestResultList[i].MaxClutter;
                            tempResult[i, 3] = m_TestResult.TestResultList[i].ClutterRejection;
                            tempResult[i, 4] = m_TestResult.TestResultList[i].Gain;
                            tempResult[i, 5] = m_TestResult.TestResultList[i].Gain - minGain;
                            tempResult[i, 6] = Convert.ToString((byte)((m_TestResult.TestResultList[i].Gain- minGain)/0.25 +0.5),16);
                        }
                        else
                        {
                            tempResult[i, 1] = "";
                        }
                        //if (m_TestResult.TestParam.IsClutterTest)
                        //{
                        //    tempResult[i, 2] = m_TestResult.TestResultList[i].MaxClutter;
                        //    tempResult[i, 3] = m_TestResult.TestResultList[i].ClutterRejection;
                        //}
                        //else
                        //{
                        //    tempResult[i, 2] = "";
                        //    tempResult[i, 3] = "";
                        //}
                        //if (m_TestResult.TestParam.IsNoiseTest)
                        //{
                        //    tempResult[i, 4] = m_TestResult.TestResultList[i].Noise;
                        //}
                        //else
                        //{
                        //    tempResult[i, 4] = "";
                        //}
                    }
                }
                return tempResult;

            }
        }

        #endregion
    }
}
