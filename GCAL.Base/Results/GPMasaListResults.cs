using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPMasaListResults
    {
        public List<GPMasaDays> arr = new List<GPMasaDays>();
        public GPGregorianTime vc_end = null;
        public GPGregorianTime vc_start = null;
        public int n_countYears = 0;
        public int n_startYear = 0;
        public GPLocation m_location;
        public IReportProgress progressReport = null;
        
        public GPMasaListResults()
        {
            n_countYears = 0;
            n_startYear = 0;
        }

        public int CalcMasaList(GPLocation loc, int nYear, int nCount)
        {
            GPMasaListResults mlist = this;
            GPAstroData day = new GPAstroData();
            GPGregorianTime d = new GPGregorianTime(loc);
            GPGregorianTime de = new GPGregorianTime(loc);
            GPGregorianTime t = new GPGregorianTime(loc);
            GPMasaDays current = null;
            vc_end = new GPGregorianTime(loc);
            vc_start = new GPGregorianTime(loc);
            int lm = -1;

            mlist.n_startYear = nYear;
            mlist.n_countYears = nCount;
            d.Copy(GPGaurabdaYear.getFirstDayOfYear(loc, nYear));
            de.Copy(GPGaurabdaYear.getFirstDayOfYear(loc, nYear + nCount));
            mlist.vc_start.Copy(d);
            mlist.vc_end.Copy(de);
            mlist.m_location = loc;

            int prev_paksa = -1;

            while (d.IsBeforeThis(de))
            {
                day.calculateDayData(d, loc);
                if (prev_paksa != day.nPaksa)
                {
                    day.nMasa = day.determineMasa(d, out day.nGaurabdaYear);
                    prev_paksa = day.nPaksa;

                    if (lm != day.nMasa)
                    {
                        if (current != null)
                        {
                            t.Copy(d);
                            t.PreviousDay();
                            current.vc_end.Copy(t);
                        }
                        lm = day.nMasa;
                        current = new GPMasaDays();
                        current.masa = day.nMasa;
                        current.year = day.nGaurabdaYear;
                        current.vc_start.Copy(d);
                        arr.Add(current);
                    }

                    d.AddDays(12);
                }

                d.NextDay();
            }

            current.vc_end.Copy(d);

            return 1;
        }


        public int Count
        {
            get
            {
                return arr.Count;
            }
        }
    }
}
