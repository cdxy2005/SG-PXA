using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    internal class InputMatrix_HighPower_L4491 : Matrix
    {
        public InputMatrix_HighPower_L4491(string inAddress)
            : base(inAddress)
        {
        }

        //开关配置初始化
        public override void MatrixInitial()
        {
            base.MatrixInitial();

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

        private void ClosePort(string portString)
        {
            this.Send("ROUT:RMOD:DRIV:SOUR OFF,(@1200)");
            this.Send("ROUT:CHAN:DRIV:PULS ON,(@1261,1262,1263,1264,1265,1266,1271,1272)");
            this.Send("ROUT:CHAN:DRIV:PULS OFF,(@" + portString + ")");
            this.Send("ROUT:RMOD:DRIV:SOUR EXT,(@1200)");
            this.Send("ROUT:CLOS (@" + portString + ")");
        }

        public override void MatrixConnection(int SourcePortNum, int GoalPortNum)
        {
            string SourcePort = "position" + SourcePortNum.ToString();
            string GoalPort = "position" + GoalPortNum.ToString();
            switch (GoalPort)
            {
                case "position1": ClosePort("1261"); this.WaitOpc(); break;
                case "position2": ClosePort("1262"); this.WaitOpc(); break;
                case "position3": ClosePort("1263"); this.WaitOpc(); break;
                case "position4": ClosePort("1264"); this.WaitOpc(); break;
                case "position5": ClosePort("1265"); this.WaitOpc(); break;
                case "position6": ClosePort("1266"); this.WaitOpc(); break;
                case "position7": ClosePort("1271"); this.WaitOpc(); break;
                case "position8": ClosePort("1272"); this.WaitOpc(); break;
            }
        }

    }
}
