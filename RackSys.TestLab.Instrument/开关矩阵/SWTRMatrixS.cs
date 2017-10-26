//****************************
//四威TR组件测试系统使用开关矩阵驱动
//****************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    public class SWTRMatrixS : Matrix
    {
        /// <summary>
        /// 输入开关路径枚举
        /// </summary>
        public enum InputThroughMatix
        {
            /// <summary>
            /// 发射-输入普通通道（无放大）
            /// </summary>
            T_Normal,
            /// <summary>
            /// 发射-输入大功率通道（有放大）
            /// </summary>
            T_PA,
            /// <summary>
            /// 接收-输入通道
            /// </summary>
            R_Normal
        }

        /// <summary>
        /// 输出开关路径枚举
        /// </summary>
        public enum OutputThroughMatix
        {
            /// <summary>
            /// 发射-输出Att1路径
            /// </summary>
            T_ATT1 = 11,
            /// <summary>
            /// 发射-输出Att2路径
            /// </summary>
            T_Att2,
            /// <summary>
            /// 发射-输出 no Att路径
            /// </summary>
            T_Normal,
            /// <summary>
            /// 发射-输出无衰减-放大（示波器--暂时没有）
            /// </summary>
            T_Normal_PA,
            /// <summary>
            /// 接收-输出-noATT-通道
            /// </summary>
            R_Noraml,
            /// <summary>
            /// 接收-输出-放大-通道（通道）
            /// </summary>
            R_PA
        }

        public SWTRMatrixS(string inAddress)
            : base(inAddress)
        {
        }

        //开关配置初始化
        public override void MatrixInitial()
        {
            base.MatrixInitial();

          //  this.Send("*RST");

        }
        /// <summary>
        /// 开关矩阵安全路径
        /// </summary>
        public void SWSecurity()
        {
            int[] swValue = new int[] { 0, 2, 2, 2, 2, 2, 0, 2, 3 };
            this.SetSwList(swValue);

        }
        public override void MatrixConnection(int SourcePortNum, int GoalPortNum)
        {

        }

        public override void MatrixConnection(int SourcePortNum, int ThroughPathNum, int GoalPortNum)
        {
            //输出部分
            if (ThroughPathNum > 10)
            {
                OutputThroughMatix tempOutput = (OutputThroughMatix)ThroughPathNum;
                switch (tempOutput)
                {
                    case OutputThroughMatix.T_ATT1:
                        connectATT1(SourcePortNum, GoalPortNum);
                        break;
                    case OutputThroughMatix.T_Att2:
                        connectATT2(SourcePortNum, GoalPortNum);
                        break;
                    case OutputThroughMatix.T_Normal:
                        connectNormal(SourcePortNum, GoalPortNum);
                        break;
                    case OutputThroughMatix.T_Normal_PA:

                        break;
                    case OutputThroughMatix.R_Noraml:
                        RInConnect(GoalPortNum, ThroughPathNum);
                        break;
                    case OutputThroughMatix.R_PA:
                        RInConnect(GoalPortNum, ThroughPathNum);
                        break;

                    default:
                        throw new Exception("开关切换路径有误！");
                }

            }
            else //输入部分
            {
                InputThroughMatix tempInput = (InputThroughMatix)ThroughPathNum;
                switch (tempInput)
                {
                    case InputThroughMatix.T_PA:
                        TPNASourceOutputChan_PM();
                        
                        break;
                    case InputThroughMatix.T_Normal:
                        TPNASourceOutputChan();
                        break;
                    case InputThroughMatix.R_Normal:

                        if (SourcePortNum == 4)
                        {
                            connetRNFIn(GoalPortNum);
                        }
                        else if (SourcePortNum == 5)
                        {
                            connectRPNAIn(GoalPortNum);
                        }
                        else if (SourcePortNum == 1 || SourcePortNum == 2 || SourcePortNum == 3)
                        {
                            RSGINCHAll_PM();
                        }
                        break;
                    default:
                        throw new Exception("开关切换路径有误！");

                }
            }

        }

        private void connectRPNAIn(int GoalPortNum)
        {
            if (GoalPortNum == 1)
            {
                RPNACH1();
            }
            else if (GoalPortNum == 2)
            {
                RPNACH2();
            }
            else if (GoalPortNum == 3)
            {
                RPNACH3();
            }
            else if (GoalPortNum == 4)
            {
                RPNACH4();
            }
        }

        private void connetRNFIn(int GoalPortNum)
        {
            if (GoalPortNum == 1)
            {
                RNFCH1();
            }
            else if (GoalPortNum == 2)
            {
                RNFCH2();
            }
            else if (GoalPortNum == 3)
            {
                RNFCH3();
            }
            else if (GoalPortNum == 4)
            {
                RNFCH4();
            }
        }

        /// <summary>
        /// 从被测件端口到仪表
        /// </summary>
        /// <param name="dutChan"></param>
        /// <param name="toSensorPort"></param>
        private void connectATT1(int dutChan, int toSensorPort)
        {
            //PXA
            if (toSensorPort == 1)
            {
                connectToPXA(dutChan);
            }
            //PW
            else if (toSensorPort == 2)
            {
                connectToPW(dutChan);
            }
            //PNAPORT2
            else if (toSensorPort == 5)
            {
                connectToPNAPort2(dutChan);
            }
            //CH1
            else if (toSensorPort == 6)
            {
                connectToOSCCh1(dutChan);
            }
            //CH2
            else if (toSensorPort == 7)
            { }
        }

        /// <summary>
        /// 从被测件端口到仪表
        /// </summary>
        /// <param name="dutChan"></param>
        /// <param name="toSensorPort"></param>
        private void connectATT2(int dutChan, int toSensorPort)
        {
            //PXA
            if (toSensorPort == 1)
            {
                connectToPXA(dutChan + 4);
            }
            //PW
            else if (toSensorPort == 2)
            {
                connectToPW(dutChan + 4);
            }
            //PNAPORT2
            else if (toSensorPort == 5)
            {
                connectToPNAPort2(dutChan + 4);
            }
            //CH1
            else if (toSensorPort == 6)
            {
                connectToOSCCh1(dutChan + 4);
            }
            //CH2
            else if (toSensorPort == 7)
            { }
        }


        /// <summary>
        /// 从被测件端口到仪表
        /// </summary>
        /// <param name="dutChan"></param>
        /// <param name="toSensorPort"></param>
        private void connectNormal(int dutChan, int toSensorPort)
        {
            //PXA
            if (toSensorPort == 1)
            {
                connectToPXA(dutChan + 4 * 2);
            }
            //PW
            else if (toSensorPort == 2)
            {
                connectToPW(dutChan + 4 * 2);
            }
            //PNAPORT2
            else if (toSensorPort == 5)
            {
                connectToPNAPort2(dutChan + 4 * 2);
            }
            //CH1
            else if (toSensorPort == 6)
            {
                connectToOSCCh1(dutChan + 4 * 2);
            }
            //CH2
            else if (toSensorPort == 7)
            { }
        }


        /// <summary>
        /// 连接到 接收输出 
        /// </summary>
        /// <param name="dutChanAndAtt"> dutchannel + Att*4</param>
        private void RInConnect(int toSensorPort, int ThroughPort)
        {
            //PXA
            if (toSensorPort == 1)
            {
                //RNFCH1();
                RNFReceive();
            }
            //PNAPORT2
            else if (toSensorPort == 5)
            {
                RPNAReceive();
            }
            //CH2
            else if (toSensorPort == 7)
            {
                OutputThroughMatix tempOut = (OutputThroughMatix)ThroughPort;
                if (tempOut == OutputThroughMatix.R_PA)
                {
                    ROSCReceive_PM();
                }
                else
                {
                    ROSCReceive();
                }

            }


        }

        /// <summary>
        /// 连接到PXA 被测物通道*ATT dutChan +1*4 or dutChan+2*4 Or dutChan+3*4
        /// </summary>
        /// <param name="dutChanAndAtt"> dutchannel + Att*4</param>
        private void connectToPXA(int dutChanAndAtt)
        {
            switch (dutChanAndAtt)
            {
                case 1:
                    TPNASACH1_30dB();
                    break;
                case 2:
                    TPNASACH2_30dB();
                    break;
                case 3:
                    TPNASACH3_30dB();
                    break;
                case 4:
                    TPNASACH4_30dB();
                    break;
                case 5:
                    TPNASACH1_20dB();
                    break;
                case 6:
                    TPNASACH2_20dB();
                    break;
                case 7:
                    TPNASACH3_20dB();
                    break;
                case 8:
                    TPNASACH4_20dB();
                    break;
                case 9:
                    TPNASACH1_0dB();
                    break;
                case 10:
                    TPNASACH2_0dB();
                    break;
                case 11:
                    TPNASACH3_0dB();
                    break;
                case 12:
                    TPNASACH4_0dB();
                    break;
                default:
                    throw new Exception("开关被测物路径有误！");
            }


        }

        /// <summary>
        /// 连接到PW 被测物通道*ATT dutChan +1*4 or dutChan+2*4 Or dutChan+3*4
        /// </summary>
        /// <param name="dutChanAndAtt"> dutchannel + Att*4</param>
        private void connectToPW(int dutChanAndAtt)
        {
            switch (dutChanAndAtt)
            {
                case 1:
                    TPNAPMCH1_30dB();
                    break;
                case 2:
                    TPNAPMCH2_30dB();
                    break;
                case 3:
                    TPNAPMCH3_30dB();
                    break;
                case 4:
                    TPNAPMCH4_30dB();
                    break;
                case 5:
                    TPNAPMCH1_20dB();
                    break;
                case 6:
                    TPNAPMCH2_20dB();
                    break;
                case 7:
                    TPNAPMCH3_20dB();
                    break;
                case 8:
                    TPNAPMCH4_20dB();
                    break;
                case 9:
                    TPNAPMCH1_0dB();
                    break;
                case 10:
                    TPNAPMCH2_0dB();
                    break;
                case 11:
                    TPNAPMCH3_0dB();
                    break;
                case 12:
                    TPNAPMCH4_0dB();
                    break;
                default:
                    throw new Exception("开关被测物路径有误！");
            }


        }

        /// <summary>
        /// 连接到PNA Port2 被测物通道*ATT dutChan +1*4 or dutChan+2*4 Or dutChan+3*4
        /// </summary>
        /// <param name="dutChanAndAtt"> dutchannel + Att*4</param>
        private void connectToPNAPort2(int dutChanAndAtt)
        {
            switch (dutChanAndAtt)
            {
                case 1:
                    TPNACH1_30dB();
                    break;
                case 2:
                    TPNACH2_30dB();
                    break;
                case 3:
                    TPNACH3_30dB();
                    break;
                case 4:
                    TPNACH4_30dB();
                    break;
                case 5:
                    TPNACH1_20dB();
                    break;
                case 6:
                    TPNACH2_20dB();
                    break;
                case 7:
                    TPNACH3_20dB();
                    break;
                case 8:
                    TPNACH4_20dB();
                    break;
                case 9:
                    TPNACH1_0dB();
                    break;
                case 10:
                    TPNACH2_0dB();
                    break;
                case 11:
                    TPNACH3_0dB();
                    break;
                case 12:
                    TPNACH4_0dB();
                    break;
                default:
                    throw new Exception("开关被测物路径有误！");
            }


        }


        /// <summary>
        /// 连接到OSCCh1 Port2 被测物通道*ATT dutChan +1*4 or dutChan+2*4 Or dutChan+3*4
        /// </summary>
        /// <param name="dutChanAndAtt"> dutchannel + Att*4</param>
        private void connectToOSCCh1(int dutChanAndAtt)
        {
            switch (dutChanAndAtt)
            {
                case 1:
                    TPNAOSCCH1_30dB();
                    break;
                case 2:
                    TPNAOSCCH2_30dB();
                    break;
                case 3:
                    TPNAOSCCH3_30dB();
                    break;
                case 4:
                    TPNAOSCCH4_30dB();
                    break;
                case 5:
                    TPNAOSCCH1_20dB();
                    break;
                case 6:
                    TPNAOSCCH2_20dB();
                    break;
                case 7:
                    TPNAOSCCH3_20dB();
                    break;
                case 8:
                    TPNAOSCCH4_20dB();
                    break;
                case 9:
                    TPNAOSCCH1_0dB();
                    break;
                case 10:
                    TPNAOSCCH2_0dB();
                    break;
                case 11:
                    TPNAOSCCH3_0dB();
                    break;
                case 12:
                    TPNAOSCCH4_0dB();
                    break;
                default:
                    throw new Exception("开关被测物路径有误！");
            }


        }

        #region 发射网络参数开关路径(输入部分)

        #region PNA作为源输出路径 NApPORT1-AMP?-Tin/Rout
        /// <summary>
        /// 发射-PNA只作为源-不加放大器输出路径 NApPORT1-NOAMP-Tin/Rout
        /// </summary>
        public void TPNASourceOutputChan()
        {
           // int[] swValue = new int[] { 4, 2 };
            int[] swValue = new int[] { 4, 4 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射-PNA只作为源-加放大器输出路径 NApPORT1-AMP-Tin/Rout
        /// </summary>
        public void TPNASourceOutputChan_PM()
        {
            int[] swValue = new int[] { 4, 2 };
           // int[] swValue = new int[] { 4, 4 };
            this.SetSwList(swValue);
        }
        #endregion

        #endregion

        #region 发射输出路径
        #region 发射网络参数-接收路径  Tout/RinX-ATTX-NAPort2

        #region  NA接收通道1 Tout/Rin1-ATTX-NAPort2
        /// <summary>
        /// 发射网络参数 接收通道1-不加放大器-30dB衰减 NAPort1-NOAMP-Tin/Rout-Tout/Rin1-ATT1-NA Port2
        /// </summary>
        public void TPNACH1_30dB()
        {
            int[] swValue = new int[] { 0, 0, 3, 1, 1, 1, 1, 2, 3, 2, 2, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射网络参数 接收通道1-20dB衰减 Tout/Rin1-ATT2-NAPort2
        /// </summary>
        public void TPNACH1_20dB()
        {
            int[] swValue = new int[] { 0, 0, 3, 1, 1, 1, 1, 3, 5, 2, 2, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射网络参数 接收通道1-无衰减 Tout/Rin1-NOATT-NAPort2
        /// </summary>
        public void TPNACH1_0dB()
        {
            int[] swValue = new int[] { 0, 0, 3, 1, 1, 1, 1, 4, 6, 2, 2, 0, 0 };
            this.SetSwList(swValue);
        }

        #endregion

        #region NA接收通道2 Tout/Rin2-ATT?-NAPort2
        /// <summary>
        /// 发射网络参数 接收通道2-30dB衰减 Tout/Rin2-ATT1-NAPort2
        /// </summary>
        public void TPNACH2_30dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 3, 1, 1, 2, 2, 3, 2, 2, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射网络参数 接收通道2-20dB衰减 Tout/Rin2-ATT2-NAPort2
        /// </summary>
        public void TPNACH2_20dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 3, 1, 1, 2, 3, 5, 2, 2, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射网络参数 接收通道2-无衰减 Tout/Rin2-NOATT-NAPort2
        /// </summary>
        public void TPNACH2_0dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 3, 1, 1, 2, 4, 6, 2, 2, 0, 0 };
            this.SetSwList(swValue);
        }

        #endregion

        #region NA接收通道3 Tout/Rin3-ATT?-NAPort2
        /// <summary>
        /// 发射网络参数 接收通道3-30dB衰减 Tout/Rin3-ATT1-NAPort2
        /// </summary>
        public void TPNACH3_30dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 3, 1, 3, 2, 3, 2, 2, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射网络参数 接收通道3-20dB衰减 Tout/Rin3-ATT2-NAPort2
        /// </summary>
        public void TPNACH3_20dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 3, 1, 3, 3,5, 2, 2, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射网络参数 接收通道3-无衰减 Tout/Rin3-NOATT-NAPort2
        /// </summary>
        public void TPNACH3_0dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 3, 1, 3, 4, 6, 2, 2, 0, 0 };
            this.SetSwList(swValue);
        }

        #endregion

        #region NA接收通道4 Tout/Rin4-ATTX-NAPort2
        /// <summary>
        /// 发射网络参数 接收通道4-30dB衰减 Tout/Rin4-ATT1-NAPort2
        /// </summary>
        public void TPNACH4_30dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 1, 3, 4, 2, 3, 2, 2, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射网络参数 接收通道4-20dB衰减Tout/Rin4-ATT2-NAPort2
        /// </summary>
        public void TPNACH4_20dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 1, 3, 4, 3, 5, 2, 2, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射网络参数 接收通道4-无衰减 Tout/Rin4-NOATT-NAPort2
        /// </summary>
        public void TPNACH4_0dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 1, 3, 4, 4, 6, 2, 2, 0, 0 };
            this.SetSwList(swValue);
        }
        #endregion

        #endregion

        #region  发射杂散、谐波测试-接收路径  Tout/Rin1-ATTX-SA
        #region 频谱仪接收-通道1 Tout/Rin1-ATTX-SA
        /// <summary>
        /// 发射杂谐波通道1-30dB衰减路径 Tout/Rin1-ATT1-SA
        /// </summary>
        public void TPNASACH1_30dB()
        {
            int[] swValue = new int[] { 0, 0, 3, 1, 1, 1, 1, 2, 3, 5 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射杂谐波通道1-20dB衰减路径 Tout/Rin1-ATT2-SA
        /// </summary>
        public void TPNASACH1_20dB()
        {
            int[] swValue = new int[] { 0, 0, 3, 1, 1, 1, 1, 3,5, 5 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射杂谐波通道1-无衰减路径 Tout/Rin1-NOATT-SA
        /// </summary>
        public void TPNASACH1_0dB()
        {
            int[] swValue = new int[] { 0, 0, 3, 1, 1, 1, 1, 4,6, 5 };
            this.SetSwList(swValue);
        }
        #endregion

        #region  频谱仪接收-通道2 Tout/Rin2-ATTX-SA
        /// <summary>
        /// 发射杂谐波通道2-30dB衰减路径 Tout/Rin2-ATT1-SA
        /// </summary>
        public void TPNASACH2_30dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 3, 1, 1, 2, 2, 3, 5 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射杂谐波通道2-20dB衰减路径 Tout/Rin2-ATT2-SA
        /// </summary>
        public void TPNASACH2_20dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 3, 1, 1, 2, 3, 5, 5 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射杂谐波通道2-无衰减路径 Tout/Rin2-NOATT-SA
        /// </summary>
        public void TPNASACH2_0dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 3, 1, 1, 2, 4, 6, 5 };
            this.SetSwList(swValue);
        }
        #endregion

        #region 频谱仪接收-通道3 Tout/Rin3-ATTX-SA
        /// <summary>
        /// 发射杂谐波通道3-30dB衰减路径 Tout/Rin3-ATT1-SA
        /// </summary>
        public void TPNASACH3_30dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 3, 1, 3, 2, 3, 5 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射杂谐波通道3-20dB衰减路径Tout/Rin3-ATT2-SA
        /// </summary>
        public void TPNASACH3_20dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 3, 1, 3, 3, 5, 5 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射杂谐波通道3-无衰减路径 Tout/Rin3-NOATT-SA
        /// </summary>
        public void TPNASACH3_0dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 3, 1, 3, 4, 6, 5};
            this.SetSwList(swValue);
        }
        #endregion

        #region 频谱仪接收-通道4 Tout/Rin4-ATTX-SA
        /// <summary>
        /// 发射杂谐波通道4-30dB衰减路径 Tout/Rin4-ATT1-SA
        /// </summary>
        public void TPNASACH4_30dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 1, 3, 4, 2, 3, 5 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射杂谐波通道4-20dB衰减路径 Tout/Rin4-ATT2-SA
        /// </summary>
        public void TPNASACH4_20dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 1, 3, 4, 3, 5, 5 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射杂谐波通道4-无衰减路径 Tout/Rin4-NOATT-SA
        /// </summary>
        public void TPNASACH4_0dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 1, 3, 4, 4, 6, 5 };
            this.SetSwList(swValue);
        }
        #endregion

        #endregion

        #region 发射功率计接收路径 Tout/RinX-ATTX-PM
        #region 功率计接收-通道1 Tout/Rin1-ATTX-PM
        /// <summary>
        /// 发射输出功率-通道1-30dB衰减路径 Tout/Rin1-ATT1-PM
        /// </summary>
        public void TPNAPMCH1_30dB()
        {
            int[] swValue = new int[] { 0, 0, 3, 1, 1, 1, 1, 2, 3, 6 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射输出功率-通道1-20dB衰减路径 Tout/Rin1-ATT2-PM
        /// </summary>
        public void TPNAPMCH1_20dB()
        {
            int[] swValue = new int[] { 0, 0, 3, 1, 1, 1, 1, 3, 5, 6 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射输出功率-通道1-无衰减路径 Tout/Rin1-NOATT-PM
        /// </summary>
        public void TPNAPMCH1_0dB()
        {
            int[] swValue = new int[] { 0, 0, 3, 1, 1, 1, 1, 4, 6, 6 };
            this.SetSwList(swValue);
        }
        #endregion

        #region 功率计接收-通道2 Tout/Rin2-ATTX-PM
        /// <summary>
        /// 发射输出功率-通道2-30dB衰减路径 Tout/Rin2-ATT1-PM
        /// </summary>
        public void TPNAPMCH2_30dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 3, 1, 1, 2, 2, 3, 6 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射输出功率-通道2-20dB衰减路径 Tout/Rin2-ATT2-PM
        /// </summary>
        public void TPNAPMCH2_20dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 3, 1, 1, 2, 3, 5, 6 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射输出功率-通道2-无衰减路径 Tout/Rin2-NOATT-PM
        /// </summary>
        public void TPNAPMCH2_0dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 3, 1, 1, 2, 4, 6, 6 };
            this.SetSwList(swValue);
        }
        #endregion

        #region 功率计接收-通道3 Tout/Rin3-ATTX-PM
        /// <summary>
        /// 发射输出功率-通道3-30dB衰减路径 Tout/Rin3-ATT1-PM
        /// </summary>
        public void TPNAPMCH3_30dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 3, 1, 3, 2, 3, 6 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射输出功率-通道3-20dB衰减路径 Tout/Rin3-ATT2-PM
        /// </summary>
        public void TPNAPMCH3_20dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 3, 1, 3, 3, 5, 6 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射输出功率-通道3-无衰减路径 Tout/Rin3-NOATT-PM
        /// </summary>
        public void TPNAPMCH3_0dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 3, 1, 3, 4, 6, 6 };
            this.SetSwList(swValue);
        }
        #endregion

        #region 功率计接收-通道4 Tout/Rin4-ATTX-PM
        /// <summary>
        /// 发射输出功率-通道4-30dB衰减路径 Tout/Rin4-ATT1-PM
        /// </summary>
        public void TPNAPMCH4_30dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 1, 3, 4, 2, 3, 6 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射输出功率-通道4-20dB衰减路径 Tout/Rin4-ATT2-PM
        /// </summary>
        public void TPNAPMCH4_20dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 1, 3, 4, 3, 5, 6 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射输出功率-通道4-无衰减路径 Tout/Rin4-NOATT-PM
        /// </summary>
        public void TPNAPMCH4_0dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 1, 3, 4, 4, 6,6 };
            this.SetSwList(swValue);
        }
        #endregion

        #endregion


        #region 发射时间参数测试接收路径  Tout/RinX-ATTX-CH1

        #region 通道1 Tout/Rin1-ATTX-CH1
        /// <summary>
        /// 发射时间参数测试-通道1-30dB衰减路径Tout/Rin1-ATT1-CH1
        /// </summary>
        public void TPNAOSCCH1_30dB()
        {
            int[] swValue = new int[] { 0, 0, 3, 1, 1, 1, 1, 2, 3, 3 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射时间参数测试-通道1-20dB衰减路径 Tout/Rin1-ATT2-CH1
        /// </summary>
        public void TPNAOSCCH1_20dB()
        {
            int[] swValue = new int[] { 0, 0, 3, 1, 1, 1, 1, 3, 5, 3 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射时间参数测试-通道1-无衰减路径 Tout/Rin1-NOATT-CH1
        /// </summary>
        public void TPNAOSCCH1_0dB()
        {
            int[] swValue = new int[] { 0, 0, 3, 1, 1, 1, 1, 4, 6, 3 };
            this.SetSwList(swValue);
        }
        #endregion

        #region 通道2 Tout/Rin2-ATTX-CH1
        /// <summary>
        /// 发射时间参数测试-通道2-30dB衰减路径 Tout/Rin2-ATT1-CH1
        /// </summary>
        public void TPNAOSCCH2_30dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 3, 1, 1, 2, 2, 3, 3 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射时间参数测试-通道2-20dB衰减路径 Tout/Rin2-ATT2-CH1
        /// </summary>
        public void TPNAOSCCH2_20dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 3, 1, 1, 2, 3, 5, 3 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射时间参数测试-通道2-无衰减路径 Tout/Rin2-NOATT-CH1
        /// </summary>
        public void TPNAOSCCH2_0dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 3, 1, 1, 2, 4, 6, 3 };
            this.SetSwList(swValue);
        }
        #endregion

        #region 通道3 Tout/Rin3-ATTX-CH1
        /// <summary>
        /// 发射时间参数测试-通道3-30dB衰减路径 Tout/Rin3-ATT1-CH1
        /// </summary>
        public void TPNAOSCCH3_30dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 3, 1, 3, 2, 3, 3 };
            this.SetSwList(swValue);
        }
        /// <summary>
        ///发射时间参数测试-通道3-20dB衰减路径 Tout/Rin3-ATT2-CH1
        /// </summary>
        public void TPNAOSCCH3_20dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 3, 1, 3, 3, 5, 3 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射时间参数测试-通道3-无衰减路径 Tout/Rin3-NOATT-CH1
        /// </summary>
        public void TPNAOSCCH3_0dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 3, 1, 3, 4, 6, 3 };
            this.SetSwList(swValue);
        }
        #endregion

        #region 通道4 Tout/Rin4-ATTX-CH1
        /// <summary>
        /// 发射时间参数测试-通道4-30dB衰减路径  Tout/Rin4-ATT1-CH1
        /// </summary>
        public void TPNAOSCCH4_30dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 1, 3, 4, 2, 3, 3 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射时间参数测试-通道4-20dB衰减路径 Tout/Rin4-ATT2-CH1
        /// </summary>
        public void TPNAOSCCH4_20dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 1, 3, 4, 3, 5, 3 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 发射时间参数测试-通道4-无衰减路径 Tout/Rin4-NOATT-CH1
        /// </summary>
        public void TPNAOSCCH4_0dB()
        {
            int[] swValue = new int[] { 0, 0, 1, 1, 1, 3, 4, 4, 6, 3 };
            this.SetSwList(swValue);
        }
        #endregion

        #endregion

        #endregion

        #region 接收态输入路径 NAPORT1-Tout/RinX
        #region 接收态PNA输入路径 NAPORT1-Tout/RinX
        /// <summary>
        /// 接收态-网络参数通道1发射路径 NAport1-Noatt-Tout/Rin1
        /// </summary>
        public void RPNACH1()
        {
            int[] swValue = new int[] { 2, 4, 3, 3, 3, 3, 1, 4, 6, 2, 0, 0, 0 };

            this.SetSwList(swValue);
        }
        /// <summary>
        /// 接收态-网络参数通道2发射路径 NAPort1-NoAtt-Tout/Rin2
        /// </summary>
        public void RPNACH2()
        {
            int[] swValue = new int[] { 2, 4, 3, 3, 3, 3, 2, 4, 6, 2, 0, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 接收态-网络参数通道3发射路径 NAPort1-NoAtt-Tout/Rin3
        /// </summary>
        public void RPNACH3()
        {
            int[] swValue = new int[] { 2, 4, 3, 3, 3, 3, 3, 4, 6, 2, 0, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 接收态-网络参数通道4发射路径 NAPort1-NoAtt-Tout/Rin4
        /// </summary>
        public void RPNACH4()
        {
            int[] swValue = new int[] { 2, 4, 3, 3, 3, 3, 4, 4, 6, 2, 0, 0, 0 };
            this.SetSwList(swValue);
        }

        #endregion

        #region 接收单通道噪声系数测试 NFS1-Tout/RinX
        /// <summary>
        /// 单通道噪声系数-通道1 NFS1-Tout/Rin1
        /// </summary>
        public void RNFCH1()
        {
            int[] swValue = new int[] { 2, 4, 3, 3, 3, 3, 1, 1, 0, 0,3, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 单通道噪声系数-通道2 NFS1-Tout/Rin2
        /// </summary>
        public void RNFCH2()
        {
            int[] swValue = new int[] { 2, 4, 3, 3, 3, 3, 2, 1, 0, 0, 3, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 单通道噪声系数-通道3 NFS1-Tout/Rin3
        /// </summary>
        public void RNFCH3()
        {
            int[] swValue = new int[] { 2, 4, 3, 3, 3, 3, 3, 1, 0, 0,3, 0, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 单通道噪声系数-通道4 NFS1-Tout/Rin4
        /// </summary>
        public void RNFCH4()
        {
            int[] swValue = new int[] { 2, 4, 3, 3, 3, 3, 4, 1, 0, 0, 3, 0, 0 };
            this.SetSwList(swValue);
        }

        #endregion

        #region 全通道噪声测试输出路径 NFS2-Tout/Rin1234
        /// <summary>
        /// 全通道噪声系数测试路径
        /// </summary>
        public void RSNFAllCH()
        {
            int[] swValue = new int[] { 2, 4, 2, 2, 2, 2, 0, 0, 0, 0, 3, 0, 1 };
            this.SetSwList(swValue);
        }

        #endregion

        #region 接收指令测试输出路径 SG-Tout/RinX
        /// <summary>
        /// 接收指令时间延时路径-通道1+接收放大 SG-Tout/Rin1
        /// </summary>
        public void RSGINCHAll_PM()
        {
            int[] swValue = new int[] { 0, 4, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 2 };
            this.SetSwList(swValue);
        }
        #endregion

      

        #endregion

        #region 接收态接收路径

        #region 接收态路径-NA接收 Tin/Rout-NAPort2
        /// <summary>
        /// 接收态路径-NA接收 Tin/Rout-NAPort2
        /// </summary>
        public void RPNAReceive()
        {
            int[] swValue = new int[] { 2, 4, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0 };
            this.SetSwList(swValue);
        }
        #endregion

        #region 接收态路径-NFA接收 Tin/Rout- NFA
        /// <summary>
        /// 单通道噪声系数-通道4 Tin/Rout- NFA
        /// </summary>
        public void RNFReceive()
        {
            int[] swValue = new int[] { 2, 4, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0 };
            this.SetSwList(swValue);
        }
        #endregion

        #region 接收态路径-OSC接收  Tin/Rout-AMP-CH2
        /// <summary>
        /// 单通道噪声系数-通道4 Tin/Rout-AMP-CH2
        /// </summary>
        public void ROSCReceive_PM()
        {
            int[] swValue = new int[] { 2, 4, 0, 0, 0, 0, 0, 0, 0, 0, 6, 1, 0 };
            this.SetSwList(swValue);
        }
        /// <summary>
        /// 单通道噪声系数-通道4 Tin/Rout-NOAMP-CH2
        /// </summary>
        public void ROSCReceive()
        {
            int[] swValue = new int[] { 2, 4, 0, 0, 0, 0, 0, 0, 0, 0, 5, 2, 0 };
            this.SetSwList(swValue);
        }
        #endregion

        #endregion

        public void SetSecurityPath()
        {
            int[] swValue = new int[] { 0, 0, 2, 2, 2, 2, };
            this.SetSwList(swValue);
        }

        /// <summary>
        /// 开关矩阵链路设置，必须依次输入开关值
        /// </summary>
        /// <param name="swValue">开关对应值</param>
        public void SetSwList(int[] swValue)
        {
            for (int i = 0; i < swValue.Length; i++)
            {
                this.SWControl(i + 1, swValue[i]);
            }

        }
        /// <summary>
        /// 开关控制指令
        /// </summary>
        /// <param name="swID">开关号</param>
        /// <param name="SWValue">开关状态号</param>
        public void  SWControl(int swID, int SWValue)
        {
            //未加参数保护
            if (SWValue != 0)
            {
                string swint = swID.ToString("00");
                string NAME = string.Format("SW{0}:{1}", swint, SWValue);
                {
                    string str = this.Query(NAME);
                    if (str != NAME)
                    {
                         str = this.Query(NAME);  
                    }
                }
            }
        }

        public string SWState(int SwID)
        {
           string  str= this.Query("QueryStateSW:" + SwID.ToString());
           return str;
        }

    }
}

