using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GCAL.Base;

namespace GCAL.Engine
{
    public class CELGenerateCalendarTwoLocs: CELGenerateHtml
    {
        public GPGregorianTime startDateA = null;
        public GPGregorianTime startDateB = null;
        public int nCount = 1;

        public void SetData(GPLocation loa, GPLocation lob, GPGregorianTime sd, int cn)
        {
            startDateA = new GPGregorianTime(sd);
            startDateA.setLocation(loa);
            startDateB = new GPGregorianTime(sd);
            startDateB.setLocation(lob);
            nCount = cn;
        }

        protected override void Execute()
        {
            GPCalendarResults calA = new GPCalendarResults();
            GPCalendarResults calB = new GPCalendarResults();

            calA.progressReport = this;
            calB.progressReport = this;

            if (startDateA != null && startDateB != null)
            {
                calA.CalculateCalendar(startDateA, nCount);
                calB.CalculateCalendar(startDateB, nCount);
            }

            StringBuilder sb = new StringBuilder();

            GPCalendarTwoLocResults cals = new GPCalendarTwoLocResults();
            cals.CalendarA = calA;
            cals.CalendarB = calB;

            FormaterHtml.WriteCompareCalendarHTML(cals, sb);

            HtmlText = sb.ToString();
            CalculatedObject = cals;

        }

        public CELGenerateCalendarTwoLocs()
        {
        }

        public CELGenerateCalendarTwoLocs(GCAL.ContentServer content)
        {
            GPLocation locProvA = null;
            GPLocation locProvB = null;
            GPGregorianTime startWesternTime = null;
            GPGregorianTime endWesternTime = null;

            locProvA = content.getLocationWithPostfix("a");

            if (locProvA == null)
            {
                locProvA = GPAppHelper.getMyLocation();
            }

            locProvB = content.getLocationWithPostfix("b");

            if (locProvB == null)
            {
                locProvB = GPAppHelper.getMyLocation();
            }

            startWesternTime = new GPGregorianTime(locProvA);
            startWesternTime.setDate(content.getInt("startyear", startWesternTime.getYear()),
                content.getInt("startmonth", startWesternTime.getMonth()),
                content.getInt("startday", startWesternTime.getDay()));

            GPVedicTime startVedicTime, endVedicTime;
            int unitType = content.getInt("endperiodtype", 3);
            int nCount = content.getInt("endperiodlength", 1);

            GPEngine.VCTIMEtoVATIME(startWesternTime, out startVedicTime, locProvA);

            GPEngine.CalcEndDate(locProvA, startWesternTime, startVedicTime, out endWesternTime, out endVedicTime, unitType, GPEngine.CorrectedCount(unitType, nCount));

            nCount = Convert.ToInt32(endWesternTime.getJulianGreenwichNoon() - startWesternTime.getJulianGreenwichNoon());

            SetData(locProvA, locProvB, startWesternTime, nCount);
            SyncExecute();

            StringBuilder sb = new StringBuilder();
            FormaterHtml.WriteCompareCalendarHTML_BodyTable(CalculatedObject as GPCalendarTwoLocResults, sb);
            HtmlText = sb.ToString();
        }

    }
}
