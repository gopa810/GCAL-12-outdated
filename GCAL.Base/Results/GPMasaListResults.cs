using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPMasaListResults
    {
        private void alloc(int nCountYears)
        {
            arr.Clear();
            for (int i = 0; i < nCountYears * 14; i++)
            {
                arr.Add(new GPMasaDays());
            }
        }

        public List<GPMasaDays> arr = new List<GPMasaDays>();
        public GPGregorianTime vc_end;
        public GPGregorianTime vc_start;
        public int n_countYears = 0;
        public int n_countMasa = 0;
        public int n_startYear = 0;
        public GPLocationProvider m_location;
        public IReportProgress progressReport = null;
        
        public GPMasaListResults()
        {
            n_countMasa = 0;
            n_countYears = 0;
            n_startYear = 0;
            vc_end = new GPGregorianTime(m_location);
            vc_start = new GPGregorianTime(m_location);
        }

        public int CalcMasaList(GPLocationProvider loc, int nYear, int nCount)
        {
            GPMasaListResults mlist = this;
            GPAstroData day = new GPAstroData();
            GPGregorianTime d = new GPGregorianTime(loc);
            GPGregorianTime de = new GPGregorianTime(loc);
            GPGregorianTime t = new GPGregorianTime(loc);
            int lm = -1;

            mlist.n_startYear = nYear;
            mlist.n_countYears = nCount;
            d.Copy(GPGaurabdaYear.getFirstDayOfYear(loc, nYear));
            de.Copy(GPGaurabdaYear.getFirstDayOfYear(loc, nYear + nCount));
            mlist.vc_start.Copy(d);
            mlist.vc_end.Copy(de);
            mlist.m_location = loc;

            alloc(nCount);

            int i = 0;
            int prev_paksa = -1;
            int current = 0;


            while (d.IsBeforeThis(de))
            {
                day.calculateDayData(d, loc);
                if (prev_paksa != day.nPaksa)
                {
                    day.nMasa = day.determineMasa(d, out day.nGaurabdaYear);

                    if (lm != day.nMasa)
                    {
                        if (lm >= 0)
                        {
                            t.Copy(d);
                            t.PreviousDay();
                            mlist.arr[current].vc_end.Copy(t);
                            current++;
                        }
                        lm = day.nMasa;
                        mlist.arr[current].masa = day.nMasa;
                        mlist.arr[current].year = day.nGaurabdaYear;
                        mlist.arr[current].vc_start.Copy(d);
                    }
                }
                prev_paksa = day.nPaksa;
                d.NextDay();
                i++;
            }

            t.Copy(d);
            mlist.arr[current].vc_end.Copy(t);
            current++;
            mlist.n_countMasa = current;

            return 1;
        }

    }
}
