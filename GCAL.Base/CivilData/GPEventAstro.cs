using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPEventAstro: GPEvent
    {
        //  - tithi
        public static readonly int AT_TITHI = 1;
        //  - naksatra start / end
        public static readonly int AT_NAKSATRA = 2;
        //  - yoga start / end
        public static readonly int AT_YOGA = 3;
        //  - sunrise
        public static readonly int AT_SUNRISE = 4;
        //  - noon
        public static readonly int AT_NOON = 5;
        //  - sunset
        public static readonly int AT_SUNSET = 6;
        //  - rasi of the moon
        public static readonly int AT_MOONRASI = 7;
        //  - rahukalam start / end
        public static readonly int AT_RAHUKALA = 8;
        //  - yama ghantam start / end
        public static readonly int AT_YAMAGHANTAM = 9;
        //  - guli kalam start / end
        public static readonly int AT_GULIKALAM = 10;

        // type of astro event
        public int nAstroType = 1;

        // number of naksatra, yoga, etc...
        public int nData = 0;

        public override string getShortDesc()
        {
            return "Event based on astronomical event";
        }
    }
}
