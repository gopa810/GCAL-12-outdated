using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPSankranti: GPAstroIterator
    {
        private static int _sankMethod = 2;

        public static int getCurrentSankrantiMethod()
        {
            return _sankMethod;
        }

        public static void setCurrentSankrantiMethod(int value)
        {
            _sankMethod = value;
        }

        public static string getCurrentSankrantiMethodName()
        {
            return getMethodName(_sankMethod);
        }

        public static string getMethodName(int i)
        {
            return GPStrings.getSharedStrings().getString(950 + i);
        }

        public static int getCount()
        {
            return 12;
        }

        public static string getName(int i)
        {
            switch (GPDisplays.General.SankrantiNameFormat())
            {
                case 0:
                    return GPStrings.getSharedStrings().getString(688 + i);
                case 1:
                    return GPStrings.getSharedStrings().getString(700 + i);
                case 2:
                    return string.Format("{0} ({1})", GPStrings.getSharedStrings().getString(688 + i), GPStrings.getSharedStrings().getString(700 + i));
                case 3:
                    return string.Format("{0} ({1})", GPStrings.getSharedStrings().getString(700 + i), GPStrings.getSharedStrings().getString(688 + i));
                default:
                    return GPStrings.getSharedStrings().getString(688 + i);
            }
        }

        public static string GetNameSan(int i)
        {
            return GPStrings.getSharedStrings().getString(688 + i);
        }

        public static string GetNameEng(int i)
        {
            return GPStrings.getSharedStrings().getString(700 + i);
        }

        public const int MESHA_SANKRANTI = 0;
        public const int VRSABHA_SANKRANTI = 1;
        public const int MITHUNA_SANKRANTI = 2;
        public const int KATAKA_SANKRANTI = 3;
        public const int SIMHA_SANKRANTI = 4;
        public const int KANYA_SANKRANTI = 5;
        public const int TULA_SANKRANTI = 6;
        public const int VRSCIKA_SANKRANTI = 7;
        public const int DHANUS_SANKRANTI = 8;
        public const int MAKARA_SANKRANTI = 9;
        public const int KUMBHA_SANKRANTI = 10;
        public const int MINA_SANKRANTI = 11;


        public override int getUnitCount()
        {
            return 12;
        }

        public override double getUnitAverageLength()
        {
            return 30;
        }

        public override double calculatePosition()
        {
            double prev = GPMath.putIn360(GPSun.GetSunLongitude(pJulianDate) - GPAyanamsa.GetAyanamsa(pJulianDate));
            return prev / 30.0;
        }


        /*********************************************************************/
        /*  Finds next time when rasi is changed                             */
        /*                                                                   */
        /*  startDate - starting date and time, timezone member must be valid */
        /*  zodiac [out] - found zodiac sign into which is changed           */
        /*                                                                   */
        /*********************************************************************/

        public static GPGregorianTime GetNextSankranti(GPGregorianTime startDate, out int zodiac)
        {
            GPGregorianTime d;
            double step = 1.0;
            int count = 0;
            double ld, prev;
            int prev_rasi, new_rasi;
            GPGregorianTime prevday = new GPGregorianTime(startDate.getLocation());
            zodiac = 0;
            d = new GPGregorianTime(startDate);
            double jdate = d.getJulianGreenwichTime();

            prev = GPMath.putIn360(GPSun.GetSunLongitude(jdate) - GPAyanamsa.GetAyanamsa(jdate));
            prev_rasi = Convert.ToInt32(Math.Floor(prev / 30.0));

            while (count < 20)
            {
                prevday.Copy(d);
                d.setDayHours( d.getDayHours() + step);
                if (d.getDayHours() > 1.0)
                {
                    d.setDayHours( d.getDayHours() - 1.0);
                    d.NextDay();
                }
                jdate = d.getJulianGreenwichTime();
                ld = GPMath.putIn360(GPSun.GetSunLongitude(jdate) - GPAyanamsa.GetAyanamsa(jdate));
                new_rasi = Convert.ToInt32(Math.Floor(ld / 30.0));

                if (prev_rasi != new_rasi)
                {
                    zodiac = new_rasi;
                    //v uplynulom dni je sankranti
                    step *= 0.5;
                    d.Copy(prevday);
                    count++;
                    continue;
                }
            }

            return d;
        }

    }
}
