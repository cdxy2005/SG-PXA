using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class AgilentEsgAndPsg : VectorSignalGenerator
    {
        private const string DataDir = "Data";

        private const string SaCalStateName = "CALSAVED";

        private const byte sgStateSequence = 8;

        private const byte sgStateRegister = 50;

        private const double m_calBandwidth = 80000000;

        private const double m_calFundamentalFrequency = 100000;

        private const int m_calFrequencyCount = 10;

        private double m_modAtten = 0;

        private string m_modref = "Mod";

        public override bool ALCHoldLineEnable
        {
            get
            {
                if (!base.IsPVSGD() || !this.IsFirmwareVersionSupported(this.FirmwareVersion, "C.04.71"))
                {
                    return false;
                }
                return base.QueryNumber(":SOURce:POWer:ALCHold:EXTernal?") > 0;
            }
            set
            {
                if (base.IsPVSGD() && this.IsFirmwareVersionSupported(this.FirmwareVersion, "C.04.71"))
                {
                    this.Send(string.Concat(":SOURce:POWer:ALCHold:EXTernal ", (value ? "ON" : "OFF")));
                }
            }
        }

        public override int ALCHoldRouting
        {
            get
            {
                if (base.HasInternalArb)
                {
                    string str = this.Query("RAD:ARB:MDES:ALCH?");
                    if (string.Compare("m1", str, true) == 0)
                    {
                        return 1;
                    }
                    if (string.Compare("m2", str, true) == 0)
                    {
                        return 2;
                    }
                    if (string.Compare("m3", str, true) == 0)
                    {
                        return 3;
                    }
                    if (string.Compare("m4", str, true) == 0)
                    {
                        return 4;
                    }
                }
                return 0;
            }
            set
            {
                if (base.HasInternalArb)
                {
                    switch (value)
                    {
                        case 0:
                            {
                                this.Send("RAD:ARB:MDES:ALCH None");
                                return;
                            }
                        case 1:
                            {
                                this.Send("RAD:ARB:MDES:ALCH M1");
                                return;
                            }
                        case 2:
                            {
                                this.Send("RAD:ARB:MDES:ALCH M2");
                                return;
                            }
                        case 3:
                            {
                                this.Send("RAD:ARB:MDES:ALCH M3");
                                return;
                            }
                        case 4:
                            {
                                this.Send("RAD:ARB:MDES:ALCH M4");
                                return;
                            }
                        default:
                            {
                                this.Send("RAD:ARB:MDES:ALCH None");
                                break;
                            }
                    }
                }
            }
        }

        public override int AlternateAmplitudeRouting
        {
            get
            {
                if (base.HasInternalArb && !base.IsMxg())
                {
                    string str = this.Query(":SOURce:RADio:ARB:MDEStination:AAMPlitude?");
                    if (string.Compare("m1", str, true) == 0)
                    {
                        return 1;
                    }
                    if (string.Compare("m2", str, true) == 0)
                    {
                        return 2;
                    }
                    if (string.Compare("m3", str, true) == 0)
                    {
                        return 3;
                    }
                    if (string.Compare("m4", str, true) == 0)
                    {
                        return 4;
                    }
                }
                return 0;
            }
            set
            {
                if (base.HasInternalArb && !base.IsMxg())
                {
                    switch (value)
                    {
                        case 0:
                            {
                                this.Send(":SOURce:RADio:ARB:MDEStination:AAMPlitude None");
                                return;
                            }
                        case 1:
                            {
                                this.Send(":SOURce:RADio:ARB:MDEStination:AAMPlitude M1");
                                return;
                            }
                        case 2:
                            {
                                this.Send(":SOURce:RADio:ARB:MDEStination:AAMPlitude M2");
                                return;
                            }
                        case 3:
                            {
                                this.Send(":SOURce:RADio:ARB:MDEStination:AAMPlitude M3");
                                return;
                            }
                        case 4:
                            {
                                this.Send(":SOURce:RADio:ARB:MDEStination:AAMPlitude M4");
                                return;
                            }
                        default:
                            {
                                this.Send(":SOURce:RADio:ARB:MDEStination:AAMPlitude None");
                                break;
                            }
                    }
                }
            }
        }

        public double ExternalFilter
        {
            get
            {
                if (this.IsAnalogSG())
                {
                    return 0.0;
                }
                return base.QueryNumber("ARB:IQ:EXTernal:FILTer?");
            }
            set
            {
                if (this.IsAnalogSG())
                {
                    return;
                }
                string str;
                str = (value > 40000000 ? "THRough" : "40E6");
                this.Send(string.Concat("ARB:IQ:EXTernal:FILTer ", str));
            }
        }

        public override bool InternalArbEnabled
        {
            get
            {
                if (base.HasInternalArb && string.Compare("1", this.Query(":SOURce:RADio:ARB:STATe?"), true) == 0)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (base.HasInternalArb)
                {
                    if (value)
                    {
                        this.Send(":SOURce:RADio:ARB:STATe ON");
                        return;
                    }
                    this.Send(":SOURce:RADio:ARB:STATe OFF");
                }
            }
        }

        public override bool IQAdjustments
        {
            get
            {
                if (this.IsAnalogSG())
                {
                    return false;
                }
                return base.QueryNumber(":SOURce:DM:IQADjustment:STATE?") > 0;
            }
            set
            {
                if (this.IsAnalogSG())
                {
                    return ;
                }
                if (value != this.IQAdjustments)
                {
                    this.Send(string.Concat(":SOURce:DM:IQADjustment:STATE ", (value ? "ON" : "OFF")));
                }
            }
        }

        public override bool IQAttenAuto
        {
            get
            {
                if (this.IsAnalogSG())
                {
                    return false;
                }
                return base.QueryNumber(":RADio:ARB:IQ:MOD:ATT:AUTO?") > 0;
            }
            set
            {
                if (this.IsAnalogSG())
                {
                    return ;
                }
                if (value != this.IQAttenAuto)
                {
                        this.Send(string.Concat(":RADio:ARB:IQ:MOD:ATT:AUTO ", (value ? "ON" : "OFF")));
                        return;
                }
            }
        }

        public override double IQAttenuator
        {
            get
            {
                if (this.IsAnalogSG())
                {
                    return 0.0;
                }
                return base.QueryNumber(":RAD:ARB:IQ:MOD:ATT?");
            }
            set
            {
                if (this.IsAnalogSG())
                {
                    return ;
                }
                if (value != this.IQAttenuator)
                {
                        base.SendNumber(":RAD:ARB:IQ:MOD:ATT ", value);
                        return;
                }
            }
        }

        public override double IQIOffset
        {
            get
            {
                if (this.IsAnalogSG())
                {
                    return 0;
                }
                return base.QueryNumber(":SOURce:DM:IQADjustment:IOFFset?");
            }
            set
            {
                if (value != this.IQIOffset)
                {
                    base.SendNumber(":SOURce:DM:IQADjustment:IOFFset ", value);
                }
            }
        }

        public override double IQModFilter
        {
            get
            {
                string str = ":SOURce:RADio:ARB:IQ:MODulation:FILTer?";
                string str1 = this.Query(str);
                if (str1 == "THR")
                {
                    return 9.9E+37;
                }
                if (str1.StartsWith("40."))
                {
                    return 40000000;
                }
                if (str1.StartsWith("2.1"))
                {
                    return 2100000;
                }
                return 0;
            }
            set
            {
                if (this.IsAnalogSG())
                {
                    return ;
                }
                string str;
                if (value > 2100000 || !(this.Model == "E4438C"))
                {
                    str = (value > 40000000 ? "THRough" : "40E6");
                }
                else
                {
                    str = "2.1E6";
                }
                string str1 = string.Concat(":SOURce:DM:BBFILter ", str);
                str1 = string.Concat(":SOURce:RADio:ARB:IQ:MODulation:FILTer ", str);
                this.Send(str1);
            }
        }

        public override bool IQModFilterAuto
        {
            get
            {
                if (this.IsAnalogSG())
                {
                    return false;
                }
                string str = ":SOURce:DM:MODulation:FILTer:AUTO?";
                    str = ":SOURce:RADio:ARB:IQ:MODulation:FILTer:AUTO?";
                return base.QueryNumber(str) > 0;
            }
            set
            {
                string str = ":SOURce:DM:MODulation:FILTer:AUTO ";
                    str = ":SOURce:RADio:ARB:IQ:MODulation:FILTer:AUTO ";
                this.Send(string.Concat(str, (value ? "ON" : "OFF")));
            }
        }

        public override double IQQOffset
        {
            get
            {
                return base.QueryNumber(":SOURce:WDM:IQADjustment:QOFFset?");
            }
            set
            {
                if (value != this.IQQOffset)
                {
                    base.SendNumber(":SOURce:DM:IQADjustment:QOFFset ", value);
                }
            }
        }

        public override double IQSkew
        {
            get
            {
                return base.QueryNumber(":SOURce:DM:IQADjustment:SKEW?");
            }
            set
            {
                base.SendNumber(":SOURce:DM:IQADjustment:SKEW ", value);
            }
        }

        public override int PulseRouting
        {
            get
            {
                if (base.HasInternalArb)
                {
                    string str = this.Query("RAD:ARB:MDES:PULS?");
                    if (string.Compare("m1", str, true) == 0)
                    {
                        return 1;
                    }
                    if (string.Compare("m2", str, true) == 0)
                    {
                        return 2;
                    }
                    if (string.Compare("m3", str, true) == 0)
                    {
                        return 3;
                    }
                    if (string.Compare("m4", str, true) == 0)
                    {
                        return 4;
                    }
                }
                return 0;
            }
            set
            {
                if (base.HasInternalArb)
                {
                    switch (value)
                    {
                        case 0:
                            {
                                this.Send("RAD:ARB:MDES:PULS None");
                                return;
                            }
                        case 1:
                            {
                                this.Send("RAD:ARB:MDES:PULS M1");
                                return;
                            }
                        case 2:
                            {
                                this.Send("RAD:ARB:MDES:PULS M2");
                                return;
                            }
                        case 3:
                            {
                                this.Send("RAD:ARB:MDES:PULS M3");
                                return;
                            }
                        case 4:
                            {
                                this.Send("RAD:ARB:MDES:PULS M4");
                                return;
                            }
                        default:
                            {
                                this.Send("RAD:ARB:MDES:PULS None");
                                break;
                            }
                    }
                }
            }
        }

        public override double QuadSkew
        {
            get
            {
                return base.QueryNumber(":SOURce:WDM:IQADjustment:QSKew?");
            }
            set
            {
                if (value != this.QuadSkew)
                {
                    base.SendNumber(":SOURce:DM:IQADjustment:QSKew ", value);
                }
            }
        }


        public override short RFFrequencyBand
        {
            get
            {
                double rFFrequency = base.RFFrequency;
                if (this.Model == "E4438C")
                {
                    if (rFFrequency < 185000000)
                    {
                        return 0;
                    }
                    if (rFFrequency <= 250000000)
                    {
                        return 1;
                    }
                    return 2;
                }
                if (this.Model != "E8267C")
                {
                    return 0;
                }
                if (rFFrequency <= 250000000)
                {
                    return 0;
                }
                if (rFFrequency < 950000000)
                {
                    return 1;
                }
                if (rFFrequency <= 1732500000)
                {
                    return 2;
                }
                if (rFFrequency < 2012000000)
                {
                    return 3;
                }
                if (rFFrequency <= 2188200000)
                {
                    return 4;
                }
                if (rFFrequency <= 3200000000)
                {
                    return 5;
                }
                return 6;
            }
        }

        public AgilentEsgAndPsg(string address)
            : base(address)
        {
            string str = ScpiInstrument.DetermineModel(address);
            base.GetErrorQueue();
        }


         public override bool HasArb()
        {
            return true;
        }

        private bool IsFirmwareVersionSupported(string version, string frmVersion)
        {
            bool flag;
            version = version.Replace(" ", "");
            if (frmVersion == null)
            {
                return true;
            }
            char[] chrArray = new char[] { '.' };
            string[] frmVersionParts = frmVersion.Split(chrArray);
            chrArray = new char[] { '.' };
            string[] VersionParts = version.Split(chrArray);
            int CharCount = Math.Min((int)VersionParts.Length, (int)frmVersionParts.Length);
            if (CharCount < 1)
            {
                return false;
            }

            for (int i = 0; i < CharCount; i++ )
            {
                string frmVersionPart = frmVersionParts[i];
                string VersionPart = VersionParts[i];
                try
                {
                    int frmVersionPartInt = int.Parse(frmVersionPart);
                    int VersionPartInt = int.Parse(VersionPart);
                    if (VersionPartInt > frmVersionPartInt)
                    {
                        flag = true;
                    }
                    else if (VersionPartInt >= frmVersionPartInt)
                    {
                        continue;
                        //相等，continue
                    }
                    else
                    {
                        flag = false;
                    }
                }
                catch
                {
                    int num4 = string.Compare(frmVersionPart, VersionPart, true);
                    if (num4 > 0)
                    {
                        flag = true;
                    }
                    else if (num4 >= 0)
                    {
                        continue;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                return flag;
            }
            return true;
        }

        public override void LoadState(byte sequence, byte register)
        {
            base.LoadState(sequence, register);
            this.IQAttenuator = this.m_modAtten;
            base.PowerSearchRef = this.m_modref;
        }

        public override void SaveState(byte sequence, byte register)
        {
            base.SaveState(sequence, register);
            this.m_modAtten = this.IQAttenuator;
            this.m_modref = base.PowerSearchRef;
        }

        private bool SupportsARBHeaders()
        {
            if (this.IsAnalogSG())
            {
                return false;
            }
            if (base.IsEVSGC())
            {
                return base.CompareFirmware("C.03.10") >= 0;
            }
            if (!base.IsPVSG())
            {
                return false;
            }
            return base.CompareFirmware("C.03.30") >= 0;
        }

        public void TurnOffMarkerRoutings()
        {
            this.PulseRouting = 0;
            this.ALCHoldRouting = 0;
            this.AlternateAmplitudeRouting = 0;
        }
    }

}
