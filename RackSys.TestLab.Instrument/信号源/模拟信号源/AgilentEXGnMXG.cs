using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
  public  class AgilentEXGnMXG:SignalGenerator
    {
      public AgilentEXGnMXG(string address)
            : base(address)
        {
            string str = ScpiInstrument.DetermineModel(address);
            base.GetErrorQueue();
        }

      public override string PulseModulationSource
      {
          get
          {
              string PulMSrc = base.Query(":PULM:SOURce?");
              if (PulMSrc == "INTernal")
              {
                  string PulMIntSrc = base.Query(":PULM:SOURce:INTernal?");
                  if (PulMIntSrc == "SQUare"
                      || PulMIntSrc == "FRUN"
                      || PulMIntSrc == "TRIGgered"
                      || PulMIntSrc == "ADOublet"
                      || PulMIntSrc == "DOUBlet"
                      || PulMIntSrc == "GATEd"
                      || PulMIntSrc == "PTRain")
                  { return PulMIntSrc; }
                  else
                  { return "---"; }
              }
              else if (PulMSrc == "EXTernal")
              {
                  return "EXTernal";
              }
              return "---";
          }
          set
          {
              if (value == "SQUare" || value == "SQU"
                      || value == "FRUN"
                      || value == "TRIGgered" || value == "TRIG"
                      || value == "ADOublet" || value == "ADO"
                      || value == "DOUBlet" || value == "DOUB"
                      || value == "GATEd" || value == "GATE"
                      || value == "PTRain" || value == "PTR")
              {
                  base.Send(":PULM:SOURce INTernal");
                  this.WaitOpc();
                  base.Send(":PULM:SOURce:INTernal " + value);
              }
              else if (value == "EXTernal" || value == "EXT")
              {
                  base.Send(":PULM:SOURce EXTernal");
              }
          }
      }

      public override double PulsePeriod
      {
          get
          {
              double PPeriod = Convert.ToDouble(base.Query(":PULM:INTernal:PERiod?"));
              return PPeriod;
          }
          set
          {
              base.Send(":PULM:INTernal:PERiod " + value.ToString());
          }
      }

      public override double PulseWidth
      {
          get
          {
              double PWidth = Convert.ToDouble(base.Query(":PULM:INTernal:PWIDth?"));
              return PWidth;
          }
          set
          {
              base.Send(":PULM:INTernal:PWIDth " + value.ToString() + "s");
          }
      }


    }
}
