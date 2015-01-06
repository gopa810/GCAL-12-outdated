using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GCAL.Base;

namespace GCAL.Engine
{
    public class CELGenerateCalendarTravelling : CELGenerateHtml
    {
        public GPLocationProvider provider = null;
        public GPGregorianTime startDateA = null;
        public int nCount = 1;

        public void SetData(GPLocationProvider loa, GPLocationProvider lob, GPGregorianTime sd, double travelDurationHours)
        {
            GPLocationProvider lp = new GPLocationProvider();

            lp.setDefaultLocation(loa.getDefaultLocation());
            GPLocationChange locChange = new GPLocationChange();
            locChange.LocationA = loa.getDefaultLocation();
            locChange.LocationB = lob.getDefaultLocation();
            locChange.TimezoneStart = true;
            locChange.julianStart = sd.getJulianGreenwichTime();
            locChange.julianEnd = locChange.julianStart + travelDurationHours/24.0;
            lp.addChange(locChange);

            provider = lp;

            startDateA = new GPGregorianTime(sd);
            startDateA.setLocationProvider(lp);
            startDateA.AddDays(-6);
            nCount = 15;
        }

        protected override void Execute()
        {
            GPCalendarResults calA = new GPCalendarResults();

            calA.progressReport = this;

            if (startDateA != null)
            {
                calA.CalculateCalendar(startDateA, nCount);
            }

            StringBuilder sb = new StringBuilder();
            FormaterHtml.WriteCalendarHTML(calA, sb);

            HtmlText = sb.ToString();
            CalculatedObject = calA;
        }

        public CELGenerateCalendarTravelling()
        {
        }

        public CELGenerateCalendarTravelling(GCAL.ContentServer content)
        {
            GPLocationProvider locProvA = null;
            GPLocationProvider locProvB = null;
            GPGregorianTime startWesternTime = null;
            //GPGregorianTime endWesternTime = null;

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

            double travelStart = content.getInt("starttravelhr", 12) * 1.0 + content.getInt("starttravelmin", 0)/60.0;
            double travelDuration = content.getInt("durtravelhr", 6) * 1.0 + content.getInt("durtravelmin", 0)/60.0;

            startWesternTime.setDayHours(travelStart/24.0);

            SetData(locProvA, locProvB, startWesternTime, travelDuration);
            SyncExecute();

            StringBuilder sb = new StringBuilder();
            FormaterHtml.WriteCalendarHTML(CalculatedObject as GPCalendarResults, sb);
            HtmlText = sb.ToString();
        }

    }
}
