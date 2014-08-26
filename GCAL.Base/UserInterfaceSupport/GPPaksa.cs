using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPPaksa
    {
        public static int getCount()
        {
            return 2;
        }

        public static string getName(int i)
        {
            return (i > 0 
                ? GPStrings.getString(712)
                : GPStrings.getString(713));
        }

        public static string getAbbreviation(int i)
        {
            return (i > 0
                ? GPStrings.getString(714)
                : GPStrings.getString(715));
        }


        public const int KRSNA_PAKSA = 0;
        public const int GAURA_PAKSA = 1;

    }
}
