using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPMasa
    {
        public const int MADHUSUDANA_MASA = 0;
        public const int TRIVIKRAMA_MASA = 1;
        public const int VAMANA_MASA = 2;
        public const int SRIDHARA_MASA = 3;
        public const int HRSIKESA_MASA = 4;
        public const int PADMANABHA_MASA = 5;
        public const int DAMODARA_MASA = 6;
        public const int KESAVA_MASA = 7;
        public const int NARAYANA_MASA = 8;
        public const int MADHAVA_MASA = 9;
        public const int GOVINDA_MASA = 10;
        public const int VISNU_MASA = 11;
        public const int ADHIKA_MASA = 12;

        protected int masa = 0;

        public static int getCount()
        {
            return 13;
        }

        public static string GetName(int i)
        {
            switch (GPDisplays.General.NameMasaFormat())
            {
                case 0:
                    // vaisnava
                    return GPStrings.getString(i + 720);
                case 1:
                    // vaisnava (common)
                    return string.Format("{0} ({1})", GPStrings.getString(i + 720), GPStrings.getString(i + 871));
                case 2:
                    // common
                    return GPStrings.getString(i + 871);
                case 3:
                    // common (vaisnava)
                    return string.Format("{0} ({1})", GPStrings.getString(i + 871), GPStrings.getString(i + 720));
            }
            return GPStrings.getString(i + 720);
        }

        public GPMasa()
        {
        }

        public GPMasa(int m)
        {
            setMasa(m);
        }

        public int getMasa()
        {
                return masa;
        }

        public void setMasa(int value)
        {
            masa = value;
            if (masa < 0) masa = 0;
            else if (masa > 13) masa = 13;
        }

        public string getName()
        {
            return GPMasa.GetName(masa);
        }

        public override string ToString()
        {
            return getName();
        }
    }
}
