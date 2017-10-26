using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class InputMatrix : ScpiInstrument
    {
        public InputMatrix(string inAddress)
            : base(inAddress)
        {
        }

        protected override void DetermineOptions()
        {
            base.m_options = "";
        }

        //public static InputMatrix Connect(string currentAddress)
        //{
        //    return new InputMatrix(currentAddress);
        //}

        //输入开关矩阵连接函数，采用列举形式，列举了8x8种连接状态
        //输入参数Source Port，GoalPort取值为position1~8的字符串数，其余会触发报错。


        //开关配置初始化
        public void inputMatrixInitial()
        {
            this.Send("ROUT:RMOD:DRIV:SOUR OFF,(@1200)");
            this.Send("ROUT:RMOD:DRIV:SOUR OFF,(@1100)");

            //设置N1810TL操作模式为Paired
            this.Send("ROUT:CHAN:DRIV:PAIR:MODE ON,(@1203)");

            //设置87222操作模式为Paired
            this.Send("ROUT:CHAN:DRIV:PAIR:MODE ON,(@1201)");

            //打开34945A
            this.Send("ROUT:RMOD:DRIV:SOUR EXT,(@1200)");
            this.Send("ROUT:RMOD:DRIV:SOUR INT,(@1100)");
        
        }
        
        public void inputMatrixConnection(string SourcePort, string GoalPort)
        {
            switch (SourcePort)
            {

                #region SourcePort1
                //输入端口接PORT1的情况
                case "position1":
                    switch (GoalPort)
                    {
                        case "position1":
                            {
                                SW1("position2");
                                SW3("position12");
                                SW4("position2");
                            }
                            break;
                        case "position2":
                            {
                                SW1("position2");
                                SW3("position12");
                                SW4("position6");
                            }
                            break;
                        case "position3":
                            {
                                SW1("position2");
                                SW3("position14");
                                SW5("position2");
                            }
                            break;
                        case "position4":
                            {
                                SW1("position2");
                                SW3("position14");
                                SW5("position6");
                            }
                            break;
                        case "position5":
                            {
                                SW1("position2");
                                SW3("position12");
                                SW4("position3");
                            }
                            break;

                        case "position6":
                            {
                                SW1("position2");
                                SW3("position12");
                                SW4("position5");
                            }
                            break;
                        case "position7":
                            {
                                SW1("position2");
                                SW3("position14");
                                SW5("position3");
                            }
                            break;
                        case "position8":
                            {
                                SW1("position2");
                                SW3("position14");
                                SW5("position5");
                            }
                            break;
                    }; break;
                #endregion SourcePort1

                #region SourcePort2

                //输入端口接PORT2的情况
                case "position2":
                    switch (GoalPort)
                    {
                        case "position1":
                            {
                                SW1("position3");
                                SW3("position12");
                                SW4("position2");
                            }
                            break;
                        case "position2":
                            {
                                SW1("position3");
                                SW3("position12");
                                SW4("position6");
                            }
                            break;
                        case "position3":
                            {
                                SW1("position3");
                                SW3("position14");
                                SW5("position2");
                            }
                            break;
                        case "position4":
                            {
                                SW1("position3");
                                SW3("position14");
                                SW5("position6");
                            }
                            break;
                        case "position5":
                            {
                                SW1("position3");
                                SW3("position12");
                                SW4("position3");
                            }
                            break;

                        case "position6":
                            {
                                SW1("position3");
                                SW3("position12");
                                SW4("position5");
                            }
                            break;
                        case "position7":
                            {
                                SW1("position3");
                                SW3("position14");
                                SW5("position3");
                            }
                            break;
                        case "position8":
                            {
                                SW1("position3");
                                SW3("position14");
                                SW5("position5");
                            }
                            break;
                    }; break;

                #endregion SourcePort2

                #region SourcePort3
                //输入端口接PORT3的情况
                case "position3":
                    switch (GoalPort)
                    {
                        case "position1":
                            {
                                SW1("position5");
                                SW3("position12");
                                SW4("position2");
                            }
                            break;
                        case "position2":
                            {
                                SW1("position5");
                                SW3("position12");
                                SW4("position6");
                            }
                            break;
                        case "position3":
                            {
                                SW1("position5");
                                SW3("position14");
                                SW5("position2");
                            }
                            break;
                        case "position4":
                            {
                                SW1("position5");
                                SW3("position14");
                                SW5("position6");
                            }
                            break;
                        case "position5":
                            {
                                SW1("position5");
                                SW3("position12");
                                SW4("position3");
                            }
                            break;

                        case "position6":
                            {
                                SW1("position5");
                                SW3("position12");
                                SW4("position5");
                            }
                            break;
                        case "position7":
                            {
                                SW1("position5");
                                SW3("position14");
                                SW5("position3");
                            }
                            break;
                        case "position8":
                            {
                                SW1("position5");
                                SW3("position14");
                                SW5("position5");
                            }
                            break;
                    }; break;
                #endregion SourcePort3

                #region SourcePort4
                //输入端口接PORT4的情况
                case "position4":
                    switch (GoalPort)
                    {
                        case "position1":
                            {
                                SW1("position6");
                                SW3("position12");
                                SW4("position2");
                            }
                            break;
                        case "position2":
                            {
                                SW1("position6");
                                SW3("position12");
                                SW4("position6");
                            }
                            break;
                        case "position3":
                            {
                                SW1("position6");
                                SW3("position14");
                                SW5("position2");
                            }
                            break;
                        case "position4":
                            {
                                SW1("position6");
                                SW3("position14");
                                SW5("position6");
                            }
                            break;
                        case "position5":
                            {
                                SW1("position6");
                                SW3("position12");
                                SW4("position3");
                            }
                            break;

                        case "position6":
                            {
                                SW1("position6");
                                SW3("position12");
                                SW4("position5");
                            }
                            break;
                        case "position7":
                            {
                                SW1("position6");
                                SW3("position14");
                                SW5("position3");
                            }
                            break;
                        case "position8":
                            {
                                SW1("position6");
                                SW3("position14");
                                SW5("position5");
                            }
                            break;
                    }; break;
                #endregion SourcePort4

                #region SourcePort5
                //输入端口接PORT5的情况
                case "position5":
                    switch (GoalPort)
                    {
                        case "position1":
                            {
                                SW2("position2");
                                SW3("position14");
                                SW4("position2");
                            }
                            break;
                        case "position2":
                            {
                                SW2("position2");
                                SW3("position14");
                                SW4("position6");
                            } break;
                        case "position3":
                            {
                                SW2("position2");
                                SW3("position12");
                                SW5("position2");
                            }
                            break;
                        case "position4":
                            {
                                SW2("position2");
                                SW3("position12");
                                SW5("position6");
                            } break;
                        case "position5":
                            {
                                SW2("position2");
                                SW3("position14");
                                SW4("position3");
                            }
                            break;
                        case "position6":
                            {
                                SW2("position2");
                                SW3("position14");
                                SW4("position5");
                            }
                            break;
                        case "position7":
                            {
                                SW2("position2");
                                SW3("position12");
                                SW5("position3");
                            } break;
                        case "position8":
                            {
                                SW2("position2");
                                SW3("position12");
                                SW5("position5");
                            }
                            break;
                    }; break;
                #endregion SourcePort5

                #region SourcePort6
                //输入端口接PORT6的情况
                case "position6":
                    switch (GoalPort)
                    {
                        case "position1":
                            {
                                SW2("position3");
                                SW3("position14");
                                SW4("position2");
                            }
                            break;
                        case "position2":
                            {
                                SW2("position3");
                                SW3("position14");
                                SW4("position6");
                            } break;
                        case "position3":
                            {
                                SW2("position3");
                                SW3("position12");
                                SW5("position2");
                            }
                            break;
                        case "position4":
                            {
                                SW2("position3");
                                SW3("position12");
                                SW5("position6");
                            } break;
                        case "position5":
                            {
                                SW2("position3");
                                SW3("position14");
                                SW4("position3");
                            }
                            break;
                        case "position6":
                            {
                                SW2("position3");
                                SW3("position14");
                                SW4("position5");
                            }
                            break;
                        case "position7":
                            {
                                SW2("position3");
                                SW3("position12");
                                SW5("position3");
                            } break;
                        case "position8":
                            {
                                SW2("position3");
                                SW3("position12");
                                SW5("position5");
                            }
                            break;
                    }; break;

                #endregion SourcePort6

                #region SourcePort7
                //输入端口接PORT7的情况
                case "position7":
                    switch (GoalPort)
                    {
                        case "position1":
                            {
                                SW2("position5");
                                SW3("position14");
                                SW4("position2");
                            }
                            break;
                        case "position2":
                            {
                                SW2("position5");
                                SW3("position14");
                                SW4("position6");
                            } break;
                        case "position3":
                            {
                                SW2("position5");
                                SW3("position12");
                                SW5("position2");
                            }
                            break;
                        case "position4":
                            {
                                SW2("position5");
                                SW3("position12");
                                SW5("position6");
                            } break;
                        case "position5":
                            {
                                SW2("position5");
                                SW3("position14");
                                SW4("position3");
                            }
                            break;
                        case "position6":
                            {
                                SW2("position5");
                                SW3("position14");
                                SW4("position5");
                            }
                            break;
                        case "position7":
                            {
                                SW2("position5");
                                SW3("position12");
                                SW5("position3");
                            } break;
                        case "position8":
                            {
                                SW2("position5");
                                SW3("position12");
                                SW5("position5");
                            }
                            break;
                    }; break;
                #endregion SourcePort7

                #region SourcePort8
                //输入端口接PORT8的情况
                case "position8":
                    switch (GoalPort)
                    {
                        case "position1":
                            {
                                SW2("position6");
                                SW3("position14");
                                SW4("position2");
                            }
                            break;
                        case "position2":
                            {
                                SW2("position6");
                                SW3("position14");
                                SW4("position6");
                            } break;
                        case "position3":
                            {
                                SW2("position6");
                                SW3("position12");
                                SW5("position2");
                            }
                            break;
                        case "position4":
                            {
                                SW2("position6");
                                SW3("position12");
                                SW5("position6");
                            } break;
                        case "position5":
                            {
                                SW2("position6");
                                SW3("position14");
                                SW4("position3");
                            }
                            break;
                        case "position6":
                            {
                                SW2("position6");
                                SW3("position14");
                                SW4("position5");
                            }
                            break;
                        case "position7":
                            {
                                SW2("position6");
                                SW3("position12");
                                SW5("position3");
                            } break;
                        case "position8":
                            {
                                SW2("position6");
                                SW3("position12");
                                SW5("position5");
                            }
                            break;
                    }; break;


                #endregion SourcePort8


                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }



        //单个开关切换底层，输入参数为整数，取值只能为Position2,Position3,Position5或者Position6，代表需要将开关的那一路打通
        public void SW1(string value)
        {
            
            switch (value)
            {
                case "position2": this.Send("ROUT:CLOS (@1122)"); this.WaitOpc(); break;
                case "position3": this.Send("ROUT:CLOS (@1123)"); this.WaitOpc(); break;
                case "position5": this.Send("ROUT:CLOS (@1125)"); this.WaitOpc(); break;
                case "position6": this.Send("ROUT:CLOS (@1126)"); this.WaitOpc(); break;
                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }


        //单个开关切换底层，输入参数为整数，取值只能为Position2,Position3,Position5或者Position6，代表需要将开关的那一路打通
        public void SW2(string value)
        {

            switch (value)
            {
                case "position2": this.Send("ROUT:CLOS (@1132)"); this.WaitOpc(); break;
                case "position3": this.Send("ROUT:CLOS (@1133)"); this.WaitOpc(); break;
                case "position5": this.Send("ROUT:CLOS (@1135)"); this.WaitOpc(); break;
                case "position6": this.Send("ROUT:CLOS (@1136)"); this.WaitOpc(); break;
                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }


        //单个开关切换底层，输入参数为整数，取值只能为Position2,Position3,Position5或者Position6，代表需要将开关的那一路打通
        public void SW4(string value)
        {

            switch (value)
            {
                case "position2": this.Send("ROUT:CLOS (@1142)"); this.WaitOpc(); break;
                case "position3": this.Send("ROUT:CLOS (@1143)"); this.WaitOpc(); break;
                case "position5": this.Send("ROUT:CLOS (@1145)"); this.WaitOpc(); break;
                case "position6": this.Send("ROUT:CLOS (@1146)"); this.WaitOpc(); break;
                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }


        //单个开关切换底层，输入参数为整数，取值只能为Position2,Position3,Position5或者Position6，代表需要将开关的那一路打通
        public void SW5(string value)
        {

            switch (value)
            {
                case "position2": this.Send("ROUT:CLOS (@1152)"); this.WaitOpc(); break;
                case "position3": this.Send("ROUT:CLOS (@1153)"); this.WaitOpc(); break;
                case "position5": this.Send("ROUT:CLOS (@1155)"); this.WaitOpc(); break;
                case "position6": this.Send("ROUT:CLOS (@1156)"); this.WaitOpc(); break;
                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }


        //单个开关切换底层，输入参数为整数，取值只能为position12，或者position14.
        //position12代表开关状态为1-2/3-4
        //position12代表开关状态为1-4/2-3
        public void SW3(string value)
        {

            switch (value)
            {
                case "position12": this.Send("ROUT:OPEN (@1201)"); this.WaitOpc(); break;
                case "position14": this.Send("ROUT:CLOS (@1201)"); this.WaitOpc(); break;
                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }





        //输出开关矩阵部分
        public void SW6(string value)
        {

            switch (value)
            {
                case "position2": this.Send("ROUT:CLOS (@1172)"); break;
                case "position3": this.Send("ROUT:CLOS (@1173)"); break;
                case "position5": this.Send("ROUT:CLOS (@1175)"); break;
                case "position6": this.Send("ROUT:CLOS (@1176)"); break;
                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }
        //单个开关切换底层，输入参数为字符串，取值只能为position2,3,5或者6，代表需要将开关的那一路打通
        public void SW7(string value)
        {

            switch (value)
            {
                case "position2": this.Send("ROUT:CLOS (@1162)"); break;
                case "position3": this.Send("ROUT:CLOS (@1163)"); break;
                case "position5": this.Send("ROUT:CLOS (@1165)"); break;
                case "position6": this.Send("ROUT:CLOS (@1166)"); break;
                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }
        //单个开关切换底层，输入参数为字符串，取值只能为position1或者2，代表需要将开关的那一路打通
        public void SW8(string value)
        {

            switch (value)
            {
                case "position1": this.Send("ROUT:OPEN (@1203)"); break;
                case "position2": this.Send("ROUT:CLOS (@1203)"); break;

                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }

        //单个开关切换底层，输入参数为字符串，取值只能为position1，2,3,4，5或者6，代表需要将开关的那一路打通
        public void SW9(string value)
        {

            switch (value)
            {
                case "position1": this.Send("ROUT:CLOS (@1101)"); break;
                case "position2": this.Send("ROUT:CLOS (@1102)"); break;
                case "position3": this.Send("ROUT:CLOS (@1103)"); break;
                case "position4": this.Send("ROUT:CLOS (@1104)"); break;
                case "position5": this.Send("ROUT:CLOS (@1105)"); break;
                case "position6": this.Send("ROUT:CLOS (@1106)"); break;
                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }

        
        



        //输入开关矩阵连接函数，采用列举形式，列举了8x6种连接状态
        //输入参数Source Port取值为position1~8的字符串数，Goalport为position1~6的字符串其余会触发报错。
        public void outputMatrixConnection(string SourcePort, string GoalPort)
        {
            switch (GoalPort)
            {

                #region GoalPort1
                //输出端口接PORT1的情况
                case "position1":
                    switch (SourcePort)
                    {
                        case "position1":
                            {
                                SW6("position6");
                                SW8("position1");
                                SW9("position1");
                            }
                            break;
                        case "position2":
                            {
                                SW6("position2");
                                SW8("position1");
                                SW9("position1");
                            }
                            break;
                        case "position3":
                            {
                                SW7("position6");
                                SW8("position2");
                                SW9("position1");
                            }
                            break;
                        case "position4":
                            {
                                SW7("position2");
                                SW8("position2");
                                SW9("position1");
                            }
                            break;
                        case "position5":
                            {
                                SW6("position5");
                                SW8("position1");
                                SW9("position1");
                            }
                            break;

                        case "position6":
                            {
                                SW6("position3");
                                SW8("position1");
                                SW9("position1");
                            }
                            break;
                        case "position7":
                            {
                                SW7("position5");
                                SW8("position2");
                                SW9("position1");
                            }
                            break;
                        case "position8":
                            {
                                SW7("position3");
                                SW8("position2");
                                SW9("position1");
                            }
                            break;

                    }; break;
                #endregion GoalPort1

                #region GoalPort2

                //输出端口接PORT2的情况
                case "position2":
                    switch (SourcePort)
                    {
                        case "position1":
                            {
                                SW6("position6");
                                SW8("position1");
                                SW9("position2");
                            }
                            break;
                        case "position2":
                            {
                                SW6("position2");
                                SW8("position1");
                                SW9("position2");
                            }
                            break;
                        case "position3":
                            {
                                SW7("position6");
                                SW8("position2");
                                SW9("position2");
                            }
                            break;
                        case "position4":
                            {
                                SW7("position2");
                                SW8("position2");
                                SW9("position2");
                            }
                            break;
                        case "position5":
                            {
                                SW6("position5");
                                SW8("position1");
                                SW9("position2");
                            }
                            break;

                        case "position6":
                            {
                                SW6("position3");
                                SW8("position1");
                                SW9("position2");
                            }
                            break;
                        case "position7":
                            {
                                SW7("position5");
                                SW8("position2");
                                SW9("position2");
                            }
                            break;
                        case "position8":
                            {
                                SW7("position3");
                                SW8("position2");
                                SW9("position2");
                            }
                            break;

                    }; break;

                #endregion GoalPort2

                #region GoalPort3
                //输出端口接PORT3的情况
                case "position3":
                    switch (SourcePort)
                    {
                        case "position1":
                            {
                                SW6("position6");
                                SW8("position1");
                                SW9("position3");
                            }
                            break;
                        case "position2":
                            {
                                SW6("position2");
                                SW8("position1");
                                SW9("position3");
                            }
                            break;
                        case "position3":
                            {
                                SW7("position6");
                                SW8("position2");
                                SW9("position3");
                            }
                            break;
                        case "position4":
                            {
                                SW7("position2");
                                SW8("position2");
                                SW9("position3");
                            }
                            break;
                        case "position5":
                            {
                                SW6("position5");
                                SW8("position1");
                                SW9("position3");
                            }
                            break;

                        case "position6":
                            {
                                SW6("position3");
                                SW8("position1");
                                SW9("position3");
                            }
                            break;
                        case "position7":
                            {
                                SW7("position5");
                                SW8("position2");
                                SW9("position3");
                            }
                            break;
                        case "position8":
                            {
                                SW7("position3");
                                SW8("position2");
                                SW9("position3");
                            }
                            break;

                    }; break;
                #endregion GoalPort3

                #region GoalPort4
                //输出端口接PORT4的情况
                case "position4":
                    switch (SourcePort)
                    {
                        case "position1":
                            {
                                SW6("position6");
                                SW8("position1");
                                SW9("position4");
                            }
                            break;
                        case "position2":
                            {
                                SW6("position2");
                                SW8("position1");
                                SW9("position4");
                            }
                            break;
                        case "position3":
                            {
                                SW7("position6");
                                SW8("position2");
                                SW9("position4");
                            }
                            break;
                        case "position4":
                            {
                                SW7("position2");
                                SW8("position2");
                                SW9("position4");
                            }
                            break;
                        case "position5":
                            {
                                SW6("position5");
                                SW8("position1");
                                SW9("position4");
                            }
                            break;

                        case "position6":
                            {
                                SW6("position3");
                                SW8("position1");
                                SW9("position4");
                            }
                            break;
                        case "position7":
                            {
                                SW7("position5");
                                SW8("position2");
                                SW9("position4");
                            }
                            break;
                        case "position8":
                            {
                                SW7("position3");
                                SW8("position2");
                                SW9("position4");
                            }
                            break;

                    }; break;
                #endregion GoalPort4

                #region GoalPort5
                //输出端口接PORT5的情况
                case "position5":
                    switch (SourcePort)
                    {
                        case "position1":
                            {
                                SW6("position6");
                                SW8("position1");
                                SW9("position5");
                            }
                            break;
                        case "position2":
                            {
                                SW6("position2");
                                SW8("position1");
                                SW9("position5");
                            }
                            break;
                        case "position3":
                            {
                                SW7("position6");
                                SW8("position2");
                                SW9("position5");
                            }
                            break;
                        case "position4":
                            {
                                SW7("position2");
                                SW8("position2");
                                SW9("position5");
                            }
                            break;
                        case "position5":
                            {
                                SW6("position5");
                                SW8("position1");
                                SW9("position5");
                            }
                            break;

                        case "position6":
                            {
                                SW6("position3");
                                SW8("position1");
                                SW9("position5");
                            }
                            break;
                        case "position7":
                            {
                                SW7("position5");
                                SW8("position2");
                                SW9("position5");
                            }
                            break;
                        case "position8":
                            {
                                SW7("position3");
                                SW8("position2");
                                SW9("position5");
                            }
                            break;

                    }; break;
                #endregion GoalPort5

                #region GoalPort6
                //输出端口接PORT6的情况
                case "position6":
                    switch (SourcePort)
                    {
                        case "position1":
                            {
                                SW6("position6");
                                SW8("position1");
                                SW9("position6");
                            }
                            break;
                        case "position2":
                            {
                                SW6("position2");
                                SW8("position1");
                                SW9("position6");
                            }
                            break;
                        case "position3":
                            {
                                SW7("position6");
                                SW8("position2");
                                SW9("position6");
                            }
                            break;
                        case "position4":
                            {
                                SW7("position2");
                                SW8("position2");
                                SW9("position6");
                            }
                            break;
                        case "position5":
                            {
                                SW6("position5");
                                SW8("position1");
                                SW9("position6");
                            }
                            break;

                        case "position6":
                            {
                                SW6("position3");
                                SW8("position1");
                                SW9("position6");
                            }
                            break;
                        case "position7":
                            {
                                SW7("position5");
                                SW8("position2");
                                SW9("position6");
                            }
                            break;
                        case "position8":
                            {
                                SW7("position3");
                                SW8("position2");
                                SW9("position6");
                            }
                            break;

                    }; break;

                #endregion GoalPort6

                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }







    } 

}
