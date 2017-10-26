using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    internal class OutputMatrix_AV3638AD : Matrix
    {
        public OutputMatrix_AV3638AD(string inAddress)
            : base(inAddress)
        {
        }

        //开关配置初始化
        public override void MatrixInitial()
        {
            base.MatrixInitial();

            this.Send("*RST");

        }

        //输入开关矩阵连接函数，采用列举形式，列举了8x6种连接状态
        //输入参数Source Port取值为position1~8的字符串数，Goalport为position1~6的字符串其余会触发报错。
        public override void MatrixConnection(int SourcePortNum, int GoalPortNum)
        {
            string SourcePort = "position" + SourcePortNum.ToString();
            string GoalPort = "position" + GoalPortNum.ToString();
            switch (GoalPort)
            {

                #region GoalPort1
                //输出端口接PORT1的情况
                case "position1":
                    switch (SourcePort)
                    {
                        case "position1":
                            this.Query("connect 4 24");
                            //System.Threading.Thread.Sleep(1000);
                            this.Send("*IDN?");
                            break;
                        case "position2":
                            this.Send("connect 4 25");
                            break;
                        case "position3":
                            this.Send("connect 4 28");
                            break;
                        case "position4":
                            this.Send("connect 4 29");
                            break;
                        case "position5":
                            this.Send("connect 4 26");
                            break;

                        case "position6":
                            this.Send("connect 4 27");
                            break;
                        case "position7":
                            this.Send("connect 4 30");
                            break;
                        case "position8":
                            this.Send("connect 4 31");
                            break;

                    }; break;
                #endregion GoalPort1

                #region GoalPort2

                //输出端口接PORT2的情况
                case "position2":
                switch (SourcePort)
                    {
                        case "position1":
                            this.Send("connect 5 24");
                            break;
                        case "position2":
                            this.Send("connect 5 25");
                            break;
                        case "position3":
                            this.Send("connect 5 28");
                            break;
                        case "position4":
                            this.Send("connect 5 29");
                            break;
                        case "position5":
                            this.Send("connect 5 26");
                            break;

                        case "position6":
                            this.Send("connect 5 27");
                            break;
                        case "position7":
                            this.Send("connect 5 30");
                            break;
                        case "position8":
                            this.Send("connect 5 31");
                            break;

                    }; break;

                #endregion GoalPort2

                #region GoalPort3
                //输出端口接PORT3的情况
                case "position3":
                    switch (SourcePort)
                    {
                        case "position1":
                            this.Send("connect 3 24");
                            break;
                        case "position2":
                            this.Send("connect 3 25");
                            break;
                        case "position3":
                            this.Send("connect 3 28");
                            break;
                        case "position4":
                            this.Send("connect 3 29");
                            break;
                        case "position5":
                            this.Send("connect 3 26");
                            break;

                        case "position6":
                            this.Send("connect 3 27");
                            break;
                        case "position7":
                            this.Send("connect 3 30");
                            break;
                        case "position8":
                            this.Send("connect 3 31");
                            break;

                    }; break;
                #endregion GoalPort3

                #region GoalPort4
                //输出端口接PORT4的情况
                case "position4":
                    switch (SourcePort)
                    {
                        case "position1":
                            this.Send("connect 0 24");
                            break;
                        case "position2":
                            this.Send("connect 0 25");
                            break;
                        case "position3":
                            this.Send("connect 0 28");
                            break;
                        case "position4":
                            this.Send("connect 0 29");
                            break;
                        case "position5":
                            this.Send("connect 0 26");
                            break;

                        case "position6":
                            this.Send("connect 0 27");
                            break;
                        case "position7":
                            this.Send("connect 0 30");
                            break;
                        case "position8":
                            this.Send("connect 0 31");
                            break;

                    }; break;
                #endregion GoalPort4

                #region GoalPort5
                //输出端口接PORT5的情况
                case "position5":
                    switch (SourcePort)
                    {
                        case "position1":
                            this.Send("connect 1 24");
                            break;
                        case "position2":
                            this.Send("connect 1 25");
                            break;
                        case "position3":
                            this.Send("connect 1 28");
                            break;
                        case "position4":
                            this.Send("connect 1 29");
                            break;
                        case "position5":
                            this.Send("connect 1 26");
                            break;

                        case "position6":
                            this.Send("connect 1 27");
                            break;
                        case "position7":
                            this.Send("connect 1 30");
                            break;
                        case "position8":
                            this.Send("connect 1 31");
                            break;

                    }; break;
                #endregion GoalPort5

                #region GoalPort6
                //输出端口接PORT6的情况
                case "position6":
                    switch (SourcePort)
                    {
                        case "position1":
                            this.Send("connect 2 24");
                            break;
                        case "position2":
                            this.Send("connect 2 25");
                            break;
                        case "position3":
                            this.Send("connect 2 28");
                            break;
                        case "position4":
                            this.Send("connect 2 29");
                            break;
                        case "position5":
                            this.Send("connect 2 26");
                            break;

                        case "position6":
                            this.Send("connect 2 27");
                            break;
                        case "position7":
                            this.Send("connect 2 30");
                            break;
                        case "position8":
                            this.Send("connect 2 31");
                            break;

                    }; break;

                #endregion GoalPort6

                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }

        
    }
}
