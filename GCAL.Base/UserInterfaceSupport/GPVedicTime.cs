using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPVedicTime
    {
        public int tithi;
        public int masa;
        public int gyear;

        public GPVedicTime() 
        {
        }

        public GPVedicTime(int t, int m, int y)
        {
            tithi = t;
            masa = m;
            gyear = y;
        }

        public GPVedicTime(GPVedicTime va)
        {
            tithi = va.tithi;
            masa = va.masa;
            gyear = va.gyear;
        }


        public void next()
        {
            tithi++;
            if (tithi >= 30)
            {
                tithi %= 30;
                masa++;
            }
            if (masa >= 12)
            {
                masa %= 12;
                gyear++;
            }
        }
        public void prev()
        {
            if (tithi == 0)
            {
                if (masa == 0)
                {
                    masa = 11;
                    tithi = 29;
                }
                else
                {
                    masa--;
                    tithi = 29;
                }
            }
            else
            {
                tithi--;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}, {2} {3}, {4} {5}, {6} {7}", GPTithi.getName(tithi), GPStrings.getString(13),
                GPPaksa.getName(tithi/15), GPStrings.getString(20),
                GPMasa.GetName(masa), GPStrings.getString(22),
                GPStrings.getString(994), gyear);
        }
    }
}
