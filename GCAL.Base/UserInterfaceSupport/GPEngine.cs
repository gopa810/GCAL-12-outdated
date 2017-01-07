using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPEngine
    {
        private static int[] maxCounts = new int[] { 0, 30240, 4320, 1080, 90, 30240, 1080, 90 };

        public static int CorrectedCount(int unitType, int count)
        {
            return Math.Min(maxCounts[unitType], count);
        }

        public static int CalcEndDate(GPLocation m_earth, GPGregorianTime vcStart, GPVedicTime vaStart, out GPGregorianTime vcEnd, out GPVedicTime vaEnd, int nType, int nCount)
        {
            vcEnd = new GPGregorianTime(vcStart);
            vaEnd = new GPVedicTime(vaStart);
            switch (nType)
            {
                case 1:
                    //vcEnd = vcStart;
                    if (nCount > 30240) nCount = 30240;
                    vcEnd.AddDays(nCount);
                    VCTIMEtoVATIME(vcEnd, out vaEnd, m_earth);
                    break;
                case 2:
                    //vcEnd = vcStart;
                    if (nCount > 4320) nCount = 4320;
                    vcEnd.AddDays(nCount * 7);
                    VCTIMEtoVATIME(vcEnd, out vaEnd, m_earth);
                    break;
                case 3:
                    //vcEnd = vcStart;
                    if (nCount > 1080) nCount = 1080;
                    vcEnd.AddMonths(nCount);
                    VCTIMEtoVATIME(vcEnd, out vaEnd, m_earth);
                    break;
                case 4:
                    //vcEnd = vcStart;
                    if (nCount > 90) nCount = 90;
                    vcEnd.AddYears(nCount);
                    VCTIMEtoVATIME(vcEnd, out vaEnd, m_earth);
                    break;
                case 5:
                    //vaEnd = vaStart;
                    if (nCount > 30240) nCount = 30240;
                    vaEnd.tithi += nCount;
                    while (vaEnd.tithi >= 30)
                    {
                        vaEnd.tithi -= 30;
                        vaEnd.masa++;
                    }
                    while (vaEnd.masa >= 12)
                    {
                        vaEnd.masa -= 12;
                        vaEnd.gyear++;
                    }
                    VATIMEtoVCTIME(vaEnd, out vcEnd, m_earth);
                    break;
                case 6:
                    //vaEnd = vaStart;
                    if (nCount > 1080) nCount = 1080;
                    vaEnd.masa = GPAppHelper.MasaToComboMasa(vaEnd.masa);
                    if (vaEnd.masa == GPMasa.ADHIKA_MASA)
                    {
                        vcEnd = new GPGregorianTime(vcStart);
                        vcEnd.AddMonths(nCount);
                        VCTIMEtoVATIME(vcEnd, out vaEnd, m_earth);
                        vaEnd.tithi = vaStart.tithi;
                        VATIMEtoVCTIME(vaEnd, out vcEnd, m_earth);
                    }
                    else
                    {
                        vaEnd.masa += nCount;
                        while (vaEnd.masa >= 12)
                        {
                            vaEnd.masa -= 12;
                            vaEnd.gyear++;
                        }
                        vaEnd.masa = GPAppHelper.ComboMasaToMasa(vaEnd.masa);
                        VATIMEtoVCTIME(vaEnd, out vcEnd, m_earth);
                    }
                    break;
                case 7:
                    //vaEnd = vaStart;
                    if (nCount > 90) nCount = 90;
                    vaEnd.gyear += nCount;
                    VATIMEtoVCTIME(vaEnd, out vcEnd, m_earth);
                    break;
            }

            return 1;
        }

        /*********************************************************************/
        /*                                                                   */
        /* Calculation of Rasi from sun-logitude and ayanamsa                */
        /*                                                                   */
        /*********************************************************************/

        public static int GetRasi(double SunLongitude, double Ayanamsa)
        {
            return Convert.ToInt32(Math.Floor(GPMath.putIn360(SunLongitude - Ayanamsa) / 30.0) + 0.1);
        }

        //===========================================================================
        //
        //===========================================================================

        public static void VATIMEtoVCTIME(GPVedicTime va, out GPGregorianTime vc, GPLocation earth)
        {
            vc = GPTithi.CalcTithiDate(va.gyear, va.masa, va.tithi / 15, va.tithi % 15, earth);
        }

        //===========================================================================
        //
        //===========================================================================

        public static void VCTIMEtoVATIME(GPGregorianTime vc, out GPVedicTime va, GPLocation earth)
        {
            GPAstroData day = new GPAstroData();
            va = new GPVedicTime();

            day.calculateDayData(vc, earth);
            va.masa = day.determineMasa(vc, out va.gyear);
            va.tithi = day.nTithi;
        }

    }
}
