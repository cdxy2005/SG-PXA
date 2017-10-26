using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    internal class InputMatrix_AV3638AD : Matrix
    {
        public InputMatrix_AV3638AD(string inAddress)
            : base(inAddress)
        {
        }

        //输入开关矩阵连接函数，采用列举形式，列举了8x8种连接状态
        //输入参数Source Port，GoalPort取值为position1~8的字符串数，其余会触发报错。

        //开关配置初始化
        public override void MatrixInitial()
        {
            base.MatrixInitial();

            this.Send("*RST");

        }

        public override void MatrixConnection(int SourcePortNum, int GoalPortNum)
        {
            string SourcePort = "position" + SourcePortNum.ToString();
            string GoalPort = "position" + GoalPortNum.ToString();
            switch (SourcePort)
            {

                #region SourcePort1
                //输入端口接PORT1的情况
                case "position1":
                    switch (GoalPort)
                    {
                        case "position1":
                            this.Send("connect 8 16");
                            //System.Threading.Thread.Sleep(1000);
                            this.Send("*IDN?");
                            break;
                        case "position2":
                            this.Send("connect 8 17");
                            break;
                        case "position3":
                            this.Send("connect 8 20");
                            break;
                        case "position4":
                            this.Send("connect 8 21");
                            break;
                        case "position5":
                            this.Send("connect 8 18");
                            break;

                        case "position6":
                            this.Send("connect 8 19");
                            break;
                        case "position7":
                            this.Send("connect 8 22");
                            break;
                        case "position8":
                            this.Send("connect 8 23");
                            break;
                    }; break;
                #endregion SourcePort1

                #region SourcePort2

                //输入端口接PORT2的情况
                case "position2":
                    switch (GoalPort)
                    {
                        case "position1":
                            this.Send("connect 7 16");
                            break;
                        case "position2":
                            this.Send("connect 7 17");
                            break;
                        case "position3":
                            this.Send("connect 7 20");
                            break;
                        case "position4":
                            this.Send("connect 7 21");
                            break;
                        case "position5":
                            this.Send("connect 7 18");
                            break;

                        case "position6":
                            this.Send("connect 7 19");
                            break;
                        case "position7":
                            this.Send("connect 7 22");
                            break;
                        case "position8":
                            this.Send("connect 7 23");
                            break;
                    }; break;

                #endregion SourcePort2

                #region SourcePort3
                //输入端口接PORT3的情况
                case "position3":
                    switch (GoalPort)
                    {
                        case "position1":
                            this.Send("connect 9 16");
                            break;
                        case "position2":
                            this.Send("connect 9 17");
                            break;
                        case "position3":
                            this.Send("connect 9 20");
                            break;
                        case "position4":
                            this.Send("connect 9 21");
                            break;
                        case "position5":
                            this.Send("connect 9 18");
                            break;

                        case "position6":
                            this.Send("connect 9 19");
                            break;
                        case "position7":
                            this.Send("connect 9 22");
                            break;
                        case "position8":
                            this.Send("connect 9 23");
                            break;
                    }; break;
                #endregion SourcePort3

                #region SourcePort4
                //输入端口接PORT4的情况
                case "position4":
                    switch (GoalPort)
                    {
                        case "position1":
                            this.Send("connect 6 16");
                            break;
                        case "position2":
                            this.Send("connect 6 17");
                            break;
                        case "position3":
                            this.Send("connect 6 20");
                            break;
                        case "position4":
                            this.Send("connect 6 21");
                            break;
                        case "position5":
                            this.Send("connect 6 18");
                            break;

                        case "position6":
                            this.Send("connect 6 19");
                            break;
                        case "position7":
                            this.Send("connect 6 22");
                            break;
                        case "position8":
                            this.Send("connect 6 23");
                            break;
                    }; break;
                #endregion SourcePort4

                #region SourcePort5
                //输入端口接PORT5的情况
                case "position5":
                    switch (GoalPort)
                    {
                        case "position1":
                            this.Send("connect 12 16");
                            break;
                        case "position2":
                            this.Send("connect 12 17");
                            break;
                        case "position3":
                            this.Send("connect 12 20");
                            break;
                        case "position4":
                            this.Send("connect 12 21");
                            break;
                        case "position5":
                            this.Send("connect 12 18");
                            break;

                        case "position6":
                            this.Send("connect 12 19");
                            break;
                        case "position7":
                            this.Send("connect 12 22");
                            break;
                        case "position8":
                            this.Send("connect 12 23");
                            break;
                    }; break;
                #endregion SourcePort5

                #region SourcePort6
                //输入端口接PORT6的情况
                case "position6":
                    switch (GoalPort)
                    {
                        case "position1":
                            this.Send("connect 11 16");
                            break;
                        case "position2":
                            this.Send("connect 11 17");
                            break;
                        case "position3":
                            this.Send("connect 11 20");
                            break;
                        case "position4":
                            this.Send("connect 11 21");
                            break;
                        case "position5":
                            this.Send("connect 11 18");
                            break;

                        case "position6":
                            this.Send("connect 11 19");
                            break;
                        case "position7":
                            this.Send("connect 11 22");
                            break;
                        case "position8":
                            this.Send("connect 11 23");
                            break;
                    }; break;

                #endregion SourcePort6

                #region SourcePort7
                //输入端口接PORT7的情况
                case "position7":
                    switch (GoalPort)
                    {
                        case "position1":
                            this.Send("connect 13 16");
                            break;
                        case "position2":
                            this.Send("connect 13 17");
                            break;
                        case "position3":
                            this.Send("connect 13 20");
                            break;
                        case "position4":
                            this.Send("connect 13 21");
                            break;
                        case "position5":
                            this.Send("connect 13 18");
                            break;

                        case "position6":
                            this.Send("connect 13 19");
                            break;
                        case "position7":
                            this.Send("connect 13 22");
                            break;
                        case "position8":
                            this.Send("connect 13 23");
                            break;
                    }; break;
                #endregion SourcePort7

                #region SourcePort8
                //输入端口接PORT8的情况
                case "position8":
                    switch (GoalPort)
                    {
                        case "position1":
                            this.Send("connect 10 16");
                            break;
                        case "position2":
                            this.Send("connect 10 17");
                            break;
                        case "position3":
                            this.Send("connect 10 20");
                            break;
                        case "position4":
                            this.Send("connect 10 21");
                            break;
                        case "position5":
                            this.Send("connect 10 18");
                            break;

                        case "position6":
                            this.Send("connect 10 19");
                            break;
                        case "position7":
                            this.Send("connect 10 22");
                            break;
                        case "position8":
                            this.Send("connect 10 23");
                            break;
                    }; break;


                #endregion SourcePort8


                default: throw new Exception("开关切换超出设置范围，请检查。");
            }
        }

        

    }
}
