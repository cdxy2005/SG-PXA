using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class OutputMatrix : ScpiInstrument
    {
        public OutputMatrix(string inAddress)
            : base(inAddress)
        {
        }

       

        public static OutputMatrix Connect(string currentAddress)
        {
            return new OutputMatrix(currentAddress);
        }

        protected override void DetermineOptions()
        {
            base.m_options = "";
        }
    

        ////输出开关矩阵连接函数，采用列举形式，列举了8x6种连接状态
        ////输入参数Source Port取值为1~8的整数，GoalPort取值为1~6的整数，其余会触发报错。
        //public void outputMatrixConnection(int SourcePort, int GoalPort)
        //{
        //    switch (GoalPort)
        //    {

        //        #region GoalPort1
        //        //输出端口接PORT1的情况
        //        case 1:
        //            switch (SourcePort)
        //            {
        //                case 1:
        //                    {
        //                        SW6(6);
        //                        SW8(1);
        //                        SW9(1);
        //                    }
        //                    break;
        //                case 2:
        //                    {
        //                        SW6(2);
        //                        SW8(1);
        //                        SW9(1);
        //                    }
        //                    break;
        //                case 3:
        //                    {
        //                        SW7(6);
        //                        SW8(2);
        //                        SW9(1);
        //                    }
        //                    break;
        //                case 4:
        //                    {
        //                        SW7(2);
        //                        SW8(2);
        //                        SW9(1);
        //                    }
        //                    break;
        //                case 5:
        //                    {
        //                        SW6(5);
        //                        SW8(1);
        //                        SW9(1);
        //                    }
        //                    break;

        //                case 6:
        //                    {
        //                        SW6(3);
        //                        SW8(1);
        //                        SW9(1);
        //                    }
        //                    break;
        //                case 7:
        //                    {
        //                        SW7(5);
        //                        SW8(2);
        //                        SW9(1);
        //                    }
        //                    break;
        //                case 8:
        //                    {
        //                        SW7(3);
        //                        SW8(2);
        //                        SW9(1);
        //                    }
        //                    break;
                       
        //            }; break;
        //        #endregion GoalPort1

        //        #region GoalPort2

        //        //输出端口接PORT2的情况
        //        case 2:
        //            switch (SourcePort)
        //            {
        //                case 1:
        //                    {
        //                        SW6(6);
        //                        SW8(1);
        //                        SW9(2);
        //                    }
        //                    break;
        //                case 2:
        //                    {
        //                        SW6(2);
        //                        SW8(1);
        //                        SW9(2);
        //                    }
        //                    break;
        //                case 3:
        //                    {
        //                        SW7(6);
        //                        SW8(2);
        //                        SW9(2);
        //                    }
        //                    break;
        //                case 4:
        //                    {
        //                        SW7(2);
        //                        SW8(2);
        //                        SW9(2);
        //                    }
        //                    break;
        //                case 5:
        //                    {
        //                        SW6(5);
        //                        SW8(1);
        //                        SW9(2);
        //                    }
        //                    break;

        //                case 6:
        //                    {
        //                        SW6(3);
        //                        SW8(1);
        //                        SW9(2);
        //                    }
        //                    break;
        //                case 7:
        //                    {
        //                        SW7(5);
        //                        SW8(2);
        //                        SW9(2);
        //                    }
        //                    break;
        //                case 8:
        //                    {
        //                        SW7(3);
        //                        SW8(2);
        //                        SW9(2);
        //                    }
        //                    break;
                       
        //            }; break;

        //        #endregion GoalPort2

        //        #region GoalPort3
        //        //输出端口接PORT3的情况
        //        case 3:
        //            switch (SourcePort)
        //            {
        //                case 1:
        //                    {
        //                        SW6(6);
        //                        SW8(1);
        //                        SW9(3);
        //                    }
        //                    break;
        //                case 2:
        //                    {
        //                        SW6(2);
        //                        SW8(1);
        //                        SW9(3);
        //                    }
        //                    break;
        //                case 3:
        //                    {
        //                        SW7(6);
        //                        SW8(2);
        //                        SW9(3);
        //                    }
        //                    break;
        //                case 4:
        //                    {
        //                        SW7(2);
        //                        SW8(2);
        //                        SW9(3);
        //                    }
        //                    break;
        //                case 5:
        //                    {
        //                        SW6(5);
        //                        SW8(1);
        //                        SW9(3);
        //                    }
        //                    break;

        //                case 6:
        //                    {
        //                        SW6(3);
        //                        SW8(1);
        //                        SW9(3);
        //                    }
        //                    break;
        //                case 7:
        //                    {
        //                        SW7(5);
        //                        SW8(2);
        //                        SW9(3);
        //                    }
        //                    break;
        //                case 8:
        //                    {
        //                        SW7(3);
        //                        SW8(2);
        //                        SW9(3);
        //                    }
        //                    break;
                       
        //            }; break;
        //        #endregion GoalPort3

        //        #region GoalPort4
        //        //输出端口接PORT4的情况
        //        case 4:
        //           switch (SourcePort)
        //            {
        //                case 1:
        //                    {
        //                        SW6(6);
        //                        SW8(1);
        //                        SW9(4);
        //                    }
        //                    break;
        //                case 2:
        //                    {
        //                        SW6(2);
        //                        SW8(1);
        //                        SW9(4);
        //                    }
        //                    break;
        //                case 3:
        //                    {
        //                        SW7(6);
        //                        SW8(2);
        //                        SW9(4);
        //                    }
        //                    break;
        //                case 4:
        //                    {
        //                        SW7(2);
        //                        SW8(2);
        //                        SW9(4);
        //                    }
        //                    break;
        //                case 5:
        //                    {
        //                        SW6(5);
        //                        SW8(1);
        //                        SW9(4);
        //                    }
        //                    break;

        //                case 6:
        //                    {
        //                        SW6(3);
        //                        SW8(1);
        //                        SW9(4);
        //                    }
        //                    break;
        //                case 7:
        //                    {
        //                        SW7(5);
        //                        SW8(2);
        //                        SW9(4);
        //                    }
        //                    break;
        //                case 8:
        //                    {
        //                        SW7(3);
        //                        SW8(2);
        //                        SW9(4);
        //                    }
        //                    break;
                       
        //            }; break;
        //        #endregion GoalPort4

        //        #region GoalPort5
        //        //输出端口接PORT5的情况
        //        case 5:
        //            switch (SourcePort)
        //            {
        //                case 1:
        //                    {
        //                        SW6(6);
        //                        SW8(1);
        //                        SW9(5);
        //                    }
        //                    break;
        //                case 2:
        //                    {
        //                        SW6(2);
        //                        SW8(1);
        //                        SW9(5);
        //                    }
        //                    break;
        //                case 3:
        //                    {
        //                        SW7(6);
        //                        SW8(2);
        //                        SW9(5);
        //                    }
        //                    break;
        //                case 4:
        //                    {
        //                        SW7(2);
        //                        SW8(2);
        //                        SW9(5);
        //                    }
        //                    break;
        //                case 5:
        //                    {
        //                        SW6(5);
        //                        SW8(1);
        //                        SW9(5);
        //                    }
        //                    break;

        //                case 6:
        //                    {
        //                        SW6(3);
        //                        SW8(1);
        //                        SW9(5);
        //                    }
        //                    break;
        //                case 7:
        //                    {
        //                        SW7(5);
        //                        SW8(2);
        //                        SW9(5);
        //                    }
        //                    break;
        //                case 8:
        //                    {
        //                        SW7(3);
        //                        SW8(2);
        //                        SW9(5);
        //                    }
        //                    break;
                       
        //            }; break;
        //        #endregion GoalPort5

        //        #region GoalPort6
        //        //输出端口接PORT6的情况
        //        case 6:
        //            switch (SourcePort)
        //            {
        //                case 1:
        //                    {
        //                        SW6(6);
        //                        SW8(1);
        //                        SW9(6);
        //                    }
        //                    break;
        //                case 2:
        //                    {
        //                        SW6(2);
        //                        SW8(1);
        //                        SW9(6);
        //                    }
        //                    break;
        //                case 3:
        //                    {
        //                        SW7(6);
        //                        SW8(2);
        //                        SW9(6);
        //                    }
        //                    break;
        //                case 4:
        //                    {
        //                        SW7(2);
        //                        SW8(2);
        //                        SW9(6);
        //                    }
        //                    break;
        //                case 5:
        //                    {
        //                        SW6(5);
        //                        SW8(1);
        //                        SW9(6);
        //                    }
        //                    break;

        //                case 6:
        //                    {
        //                        SW6(3);
        //                        SW8(1);
        //                        SW9(6);
        //                    }
        //                    break;
        //                case 7:
        //                    {
        //                        SW7(5);
        //                        SW8(2);
        //                        SW9(6);
        //                    }
        //                    break;
        //                case 8:
        //                    {
        //                        SW7(3);
        //                        SW8(2);
        //                        SW9(6);
        //                    }
        //                    break;
                       
        //            }; break;

        //        #endregion GoalPort6

        //        default: throw new Exception("开关切换超出设置范围，请检查。");
        //    }
        //}




        ////单个开关切换底层，输入参数为字符串，取值只能为position2,3,5或者6，代表需要将开关的那一路打通
        //public void SW6(string value)
        //{

        //    switch (value)
        //    {
        //        case "position2": this.Send("ROUT:CLOS (@1172)"); break;
        //        case "position3": this.Send("ROUT:CLOS (@1173)"); break;
        //        case "position5": this.Send("ROUT:CLOS (@1175)"); break;
        //        case "position6": this.Send("ROUT:CLOS (@1176)"); break;
        //        default: throw new Exception("开关切换超出设置范围，请检查。");
        //    }
        //}



        ////单个开关切换底层，输入参数为字符串，取值只能为position2,3,5或者6，代表需要将开关的那一路打通
        //public void SW7(string value)
        //{

        //    switch (value)
        //    {
        //        case "position2": this.Send("ROUT:CLOS (@1162)"); break;
        //        case "position3": this.Send("ROUT:CLOS (@1163)"); break;
        //        case "position5": this.Send("ROUT:CLOS (@1165)"); break;
        //        case "position6": this.Send("ROUT:CLOS (@1166)"); break;
        //        default: throw new Exception("开关切换超出设置范围，请检查。");
        //    }
        //}


        ////单个开关切换底层，输入参数为字符串，取值只能为position1，2,3,4，5或者6，代表需要将开关的那一路打通
        //public void SW9(string value)
        //{

        //    switch (value)
        //    {
        //        case "position1": this.Send("ROUT:CLOS (@1101)"); break;
        //        case "position2": this.Send("ROUT:CLOS (@1102)"); break;
        //        case "position3": this.Send("ROUT:CLOS (@1103)"); break;
        //        case "position4": this.Send("ROUT:CLOS (@1104)"); break;
        //        case "position5": this.Send("ROUT:CLOS (@1105)"); break;
        //        case "position6": this.Send("ROUT:CLOS (@1106)"); break;
        //        default: throw new Exception("开关切换超出设置范围，请检查。");
        //    }
        //}

        ////单个开关切换底层，输入参数为整数，取值只能为1或者2，代表需要将开关的那一路打通
        //public void SW8(int value)
        //{

        //    switch (value)
        //    {
        //        case 1: this.Send("ROUT:OPEN (@1203)"); break;
        //        case 2: this.Send("ROUT:CLOS (@1203)"); break;
                
        //        default: throw new Exception("开关切换超出设置范围，请检查。");
        //    }
        //}





    }
}
