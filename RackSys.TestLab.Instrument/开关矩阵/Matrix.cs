using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public abstract class Matrix : ScpiInstrument
    {
        public Matrix(string inAddress)
            : base(inAddress)
        {
        }

        protected override void DetermineOptions()
        {
            base.m_options = "";
        }

        public static Matrix Connect(string currentAddress, bool isInputMatrix, bool isHighPower)
        {
            //创建临时会话
            //ScpiInstrument tmpIo = new ScpiInstrument(currentAddress);
            //获取开关矩阵型号
            //string model = tmpIo.Model.Trim();
            string model = ScpiInstrument.DetermineModel(currentAddress).Trim();
            if (isInputMatrix)
            {
                //输入开关矩阵
                if (isHighPower)
                {
                    switch (model) 
                    {
                        //待测试
                        case "L4491A":
                            Matrix tmpMatrix = new InputMatrix_HighPower_L4491(currentAddress);
                            tmpMatrix.MatrixInitial();
                            return tmpMatrix;

                        case "AV3638AD":
                            Matrix tmpMatrix1 = new InputMatrix_AV3638AD(currentAddress);
                            tmpMatrix1.MatrixInitial();
                            return tmpMatrix1;
                        case "RAC1110":
                            Matrix tmpMatrix2= new SWTRMatrixS(currentAddress);
                            tmpMatrix2.MatrixInitial();
                            return tmpMatrix2;
                        case "SiWeeTR":
                            Matrix tmpMatrix3 = new SWTRMatrixS(currentAddress);
                            tmpMatrix3.MatrixInitial();//RAC-8003B
                            return tmpMatrix3;
                        case "RAC-8003B":
                            Matrix tmpMatrix4 = new SWTRMatrixS(currentAddress);
                            tmpMatrix4.MatrixInitial();//RAC-8003B
                            return tmpMatrix4;
                        default: throw new Exception("不能识别的设备");
                        //Racksystem,SiWeeTR,CNSWTR201601
                    }
                }
                else
                {
                    switch (model)
                    {
                        //待测试
                        case "L4491A":
                            //输入开关矩阵
                            Matrix tmpMatrix = new InputMatrix_L4491(currentAddress);
                            tmpMatrix.MatrixInitial();
                            return tmpMatrix;

                        case "AV3638AD":
                            Matrix tmpMatrix1 = new InputMatrix_AV3638AD(currentAddress);
                            tmpMatrix1.MatrixInitial();
                            return tmpMatrix1;
                        case "RAC1110":
                            Matrix tmpMatrix2 = new SWTRMatrixS(currentAddress);
                            tmpMatrix2.MatrixInitial();
                            return tmpMatrix2;
                        case "SiWeeTR":
                            Matrix tmpMatrix3 = new SWTRMatrixS(currentAddress);
                            tmpMatrix3.MatrixInitial();
                            return tmpMatrix3;
                        case "RAC-8003B":
                            Matrix tmpMatrix4 = new SWTRMatrixS(currentAddress);
                            tmpMatrix4.MatrixInitial();//RAC-8003B
                            return tmpMatrix4;
                        default: throw new Exception("不能识别的设备");

                    }
                }
            }
            else 
            {
                //输出开关矩阵
                switch (model)
                {
                    //待测试
                    case "L4491A":
                        Matrix tmpMatrix = new OutputMatrix_L4491(currentAddress);
                        tmpMatrix.MatrixInitial();
                        return tmpMatrix;

                    case "AV3638AD":
                        Matrix tmpMatrix1 = new OutputMatrix_AV3638AD(currentAddress);
                        tmpMatrix1.MatrixInitial();
                        return tmpMatrix1;
                    case "RAC1110":
                        Matrix tmpMatrix2 = new SWTRMatrixS(currentAddress);
                        tmpMatrix2.MatrixInitial();
                        return tmpMatrix2;
                    case "SiWeeTR":
                        Matrix tmpMatrix3 = new SWTRMatrixS(currentAddress);
                        tmpMatrix3.MatrixInitial();
                        return tmpMatrix3;
                    case "RAC-8003B":
                        Matrix tmpMatrix4 = new SWTRMatrixS(currentAddress);
                        tmpMatrix4.MatrixInitial();//RAC-8003B
                        return tmpMatrix4;
                    default: throw new Exception("不能识别的设备");

                }
            }
        }

        private bool m_IsInit;

        public virtual void MatrixInitial()
        {
            if (m_IsInit) 
            {
                return;
            }
            m_IsInit = true;
        }
        /// <summary>
        /// 开关矩阵切换
        /// </summary>
        /// <param name="SourcePort"></param>
        /// <param name="GoalPort"></param>
        public abstract void MatrixConnection(int SourcePortNum, int GoalPortNum);


        /// <summary>
        /// 开关矩阵切换由三个节点标明
        /// </summary>
        /// <param name="SourcePort"></param>
        /// <param name="GoalPort"></param>
        public virtual void MatrixConnection(int SourcePortNum, int ThroughPathNum, int GoalPortNum)
        {

        }

        /// <summary>
        /// 开关矩阵切换通过名称切换
        /// </summary>
        /// <param name="RoutePathName"></param>
        public virtual void MatrixConnection(string RoutePathName)
        {

        }
    }
}
