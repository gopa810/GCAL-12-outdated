using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPCoreEvent
    {
	    public int nType = 0;
	    public int nData = 0;
	    public GPGregorianTime Time = null;
        public string strData = String.Empty;

        public int getDstFlag()
        {
            return Time.getDaylightTimeON() ? 1 : 0;
        }

        public string getTypeTitle()
        {
            switch (nType)
            {
                case GPConstants.CCTYPE_S_ARUN:
                case GPConstants.CCTYPE_S_RISE:
                case GPConstants.CCTYPE_S_NOON:
                case GPConstants.CCTYPE_S_SET:
                    return GPStrings.getString(996);
                case GPConstants.CCTYPE_TITHI:
                    return GPStrings.getString(997);
                case GPConstants.CCTYPE_NAKS:
                    return GPStrings.getString(998);
                case GPConstants.CCTYPE_SANK:
                    return GPStrings.getString(1000);
                case GPConstants.CCTYPE_CONJ:
                    return GPStrings.getString(999);
                case GPConstants.CoreEventMoonRise:
                    return GPStrings.getString(1007);
                case GPConstants.CoreEventMoonSet:
                    return GPStrings.getString(1007);
                case GPConstants.CCTYPE_SUNECLIPSE_CENTER:
                case GPConstants.CCTYPE_SUNECLIPSE_FULL_END:
                case GPConstants.CCTYPE_SUNECLIPSE_FULL_START:
                case GPConstants.CCTYPE_SUNECLIPSE_PARTIAL_END:
                case GPConstants.CCTYPE_SUNECLIPSE_PARTIAL_START:
                    return GPStrings.getString(1008);
                case GPConstants.CCTYPE_MOONECLIPSE_CENTER:
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_FULL_END:
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_FULL_START:
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_PART_END:
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_PART_START:
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_FULL_END:
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_FULL_START:
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_PART_END:
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_PART_START:
                    return GPStrings.getString(1009);
                case GPConstants.CCTYPE_TRAVELLING_START:
                case GPConstants.CCTYPE_TRAVELLING_END:
                    return GPStrings.getString(1030);
                default:
                    return string.Empty;
            }
        }

        public string getEventTitle()
        {
            switch (nType)
            {
                case 10:
                    return GPStrings.getString(42);
                case 11:
                    return GPStrings.getString(51);
                case 12:
                    return GPStrings.getString(857);
                case 13:
                    return GPStrings.getString(52);
                case 20:
                    return string.Format("{0} {1}", GPTithi.getName(nData), GPStrings.getString(13));
                case 21:
                    return string.Format("{0} {1}", GPNaksatra.getName(nData), GPStrings.getString(15));
                case 22:
                    return string.Format("{0} {1}", GPSankranti.getName(nData), GPStrings.getString(56));
                case 23:
                    return string.Format(GPStrings.getString(995), GPSankranti.getName(nData));
                case GPConstants.CoreEventMoonRise:
                    return GPStrings.getString(53);
                case GPConstants.CoreEventMoonSet:
                    return GPStrings.getString(54);
                case GPConstants.CCTYPE_SUNECLIPSE_CENTER:
                    return GPStrings.getString(1010);
                case GPConstants.CCTYPE_SUNECLIPSE_FULL_END:
                    return GPStrings.getString(1011);
                case GPConstants.CCTYPE_SUNECLIPSE_FULL_START:
                    return GPStrings.getString(1012);
                case GPConstants.CCTYPE_SUNECLIPSE_PARTIAL_END:
                    return GPStrings.getString(1013);
                case GPConstants.CCTYPE_SUNECLIPSE_PARTIAL_START:
                    return GPStrings.getString(1014);
                case GPConstants.CCTYPE_MOONECLIPSE_CENTER:
                    return GPStrings.getString(1015);
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_FULL_END:
                    return GPStrings.getString(1016);
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_FULL_START:
                    return GPStrings.getString(1017);
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_PART_END:
                    return GPStrings.getString(1018);
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_PART_START:
                    return GPStrings.getString(1019);
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_FULL_END:
                    return GPStrings.getString(1020);
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_FULL_START:
                    return GPStrings.getString(1021);
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_PART_END:
                    return GPStrings.getString(1022);
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_PART_START:
                    return GPStrings.getString(1023);
                case GPConstants.CCTYPE_TRAVELLING_START:
                    return GPStrings.getString(1031);
                case GPConstants.CCTYPE_TRAVELLING_END:
                    return GPStrings.getString(1032);
                default:
                    return string.Empty;
            }
        }
    }
}
