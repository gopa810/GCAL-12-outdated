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

        public void SetData(GPLocationProvider loa, GPLocationProvider lob, GPGregorianTime sd, int cn)
        {
            startDateA = new GPGregorianTime(sd);
            startDateA.setLocationProvider(loa);
            startDateB = new GPGregorianTime(sd);
            startDateB.setLocationProvider(lob);
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
            GPLocationProvider locProvA = null;
            GPLocationProvider locProvB = null;
            GPGregorianTime startWesternTime = null;
            GPGregorianTime endWesternTime = null;

            if (content.getString("locationtypea") == "selected")
            {
                GPLocation loc = GPLocationList.getShared().findLocationById(content.getInt("locationida"));
                if (loc != null)
                    locProvA = new GPLocationProvider(loc);
            }

            if (locProvA == null)
            {
                HtmlText = "<p>Error: location provider A is null";
                return;
            }

            if (content.getString("locationtypeb") == "selected")
            {
                GPLocation loc = GPLocationList.getShared().findLocationById(content.getInt("locationidb"));
                if (loc != null)
                    locProvB = new GPLocationProvider(loc);
            }

            if (locProvB == null)
            {
                HtmlText = "<p>Error: location provider B is null";
                return;
            }

            startWesternTime = new GPGregorianTime(locProvA);
            startWesternTime.setDate(content.getInt("startyear"), content.getInt("startmonth"), content.getInt("startday"));

            GPVedicTime startVedicTime, endVedicTime;
            int unitType = content.getInt("endperiodtype");
            int nCount = content.getInt("endperiodlength");

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
