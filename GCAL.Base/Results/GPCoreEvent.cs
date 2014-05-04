using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPCoreEvent
    {
	    public int nType;
	    public int nData;
	    public GPGregorianTime Time;
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
                    return GPStrings.getSharedStrings().getString(996);
                case GPConstants.CCTYPE_TITHI:
                    return GPStrings.getSharedStrings().getString(997);
                case GPConstants.CCTYPE_NAKS:
                    return GPStrings.getSharedStrings().getString(998);
                case GPConstants.CCTYPE_SANK:
                    return GPStrings.getSharedStrings().getString(1000);
                case GPConstants.CCTYPE_CONJ:
                    return GPStrings.getSharedStrings().getString(999);
                case GPConstants.CoreEventMoonRise:
                    return GPStrings.getSharedStrings().getString(1007);
                case GPConstants.CoreEventMoonSet:
                    return GPStrings.getSharedStrings().getString(1007);
                case GPConstants.CCTYPE_SUNECLIPSE_CENTER:
                case GPConstants.CCTYPE_SUNECLIPSE_FULL_END:
                case GPConstants.CCTYPE_SUNECLIPSE_FULL_START:
                case GPConstants.CCTYPE_SUNECLIPSE_PARTIAL_END:
                case GPConstants.CCTYPE_SUNECLIPSE_PARTIAL_START:
                    return GPStrings.getSharedStrings().getString(1008);
                case GPConstants.CCTYPE_MOONECLIPSE_CENTER:
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_FULL_END:
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_FULL_START:
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_PART_END:
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_PART_START:
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_FULL_END:
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_FULL_START:
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_PART_END:
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_PART_START:
                    return GPStrings.getSharedStrings().getString(1009);
                default:
                    return string.Empty;
            }
        }

        public string getEventTitle()
        {
            switch (nType)
            {
                case 10:
                    return GPStrings.getSharedStrings().getString(42);
                case 11:
                    return GPStrings.getSharedStrings().getString(51);
                case 12:
                    return GPStrings.getSharedStrings().getString(857);
                case 13:
                    return GPStrings.getSharedStrings().getString(52);
                case 20:
                    return string.Format("{0} {1}", GPTithi.getName(nData), GPStrings.getSharedStrings().getString(13));
                case 21:
                    return string.Format("{0} {1}", GPNaksatra.getName(nData), GPStrings.getSharedStrings().getString(15));
                case 22:
                    return string.Format("{0} {1}", GPSankranti.getName(nData), GPStrings.getSharedStrings().getString(56));
                case 23:
                    return string.Format(GPStrings.getSharedStrings().getString(995), GPSankranti.getName(nData));
                case GPConstants.CoreEventMoonRise:
                    return GPStrings.getSharedStrings().getString(53);
                case GPConstants.CoreEventMoonSet:
                    return GPStrings.getSharedStrings().getString(54);
                case GPConstants.CCTYPE_SUNECLIPSE_CENTER:
                    return GPStrings.getSharedStrings().getString(1010);
                case GPConstants.CCTYPE_SUNECLIPSE_FULL_END:
                    return GPStrings.getSharedStrings().getString(1011);
                case GPConstants.CCTYPE_SUNECLIPSE_FULL_START:
                    return GPStrings.getSharedStrings().getString(1012);
                case GPConstants.CCTYPE_SUNECLIPSE_PARTIAL_END:
                    return GPStrings.getSharedStrings().getString(1013);
                case GPConstants.CCTYPE_SUNECLIPSE_PARTIAL_START:
                    return GPStrings.getSharedStrings().getString(1014);
                case GPConstants.CCTYPE_MOONECLIPSE_CENTER:
                    return GPStrings.getSharedStrings().getString(1015);
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_FULL_END:
                    return GPStrings.getSharedStrings().getString(1016);
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_FULL_START:
                    return GPStrings.getSharedStrings().getString(1017);
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_PART_END:
                    return GPStrings.getSharedStrings().getString(1018);
                case GPConstants.CCTYPE_MOONECLIPSE_MAIN_PART_START:
                    return GPStrings.getSharedStrings().getString(1019);
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_FULL_END:
                    return GPStrings.getSharedStrings().getString(1020);
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_FULL_START:
                    return GPStrings.getSharedStrings().getString(1021);
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_PART_END:
                    return GPStrings.getSharedStrings().getString(1022);
                case GPConstants.CCTYPE_MOONECLIPSE_PENUM_PART_START:
                    return GPStrings.getSharedStrings().getString(1023);
                default:
                    return string.Empty;
            }
        }
    }
}
