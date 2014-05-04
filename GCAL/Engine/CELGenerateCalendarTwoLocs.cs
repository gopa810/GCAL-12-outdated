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
    }
}
