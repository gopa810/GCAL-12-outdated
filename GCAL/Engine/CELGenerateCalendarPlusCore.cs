using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GCAL.Base;

namespace GCAL.Engine
{
    public class CELGenerateCalendarPlusCore: CELGenerateHtml
    {
        public GPLocationProvider location = null;
        public GPGregorianTime startDate = null;
        public GPGregorianTime endDate = null;

        public void SetData(GPLocationProvider lo, GPGregorianTime sd, GPGregorianTime ed)
        {
            location = lo;
            startDate = sd;
            endDate = ed;
        }

        protected override void Execute()
        {
            GPCalendarResults cal = new GPCalendarResults();
            GPCoreEventResults evn = new GPCoreEventResults();

            cal.progressReport = this;
            evn.progressReport = this;

            if (location != null && startDate != null)
            {
                cal.CalculateCalendar(startDate, Convert.ToInt32(endDate.getJulianLocalNoon() - startDate.getJulianLocalNoon()));
                evn.CalculateEvents(location, startDate, endDate);
            }

            GPCalendarPlusEventsResults calev = new GPCalendarPlusEventsResults();
            calev.theCalendar = cal;
            calev.theEvents = evn;

            StringBuilder sb = new StringBuilder();

            FormatOutput(sb, calev);

            HtmlText = sb.ToString();
            CalculatedObject = calev;
        }

        public void FormatOutput(StringBuilder sb, GPCalendarPlusEventsResults cal)
        {
            bool done = false;
            if (Template != null)
            {
                if (Template == "default:plain")
                {
                    sb.AppendLine("<html><head><title>Calendar</title>");
                    sb.AppendLine("<style>");
                    sb.AppendLine("<!--");
                    sb.AppendLine(FormaterHtml.CurrentFormattingStyles);
                    sb.AppendLine("-->");
                    sb.AppendLine("</style>");
                    sb.AppendLine("</head>");
                    sb.AppendLine("<body>");
                    sb.AppendLine("<pre>");
                    StringBuilder fout = new StringBuilder();
                    FormaterPlain.FormatCalendarPlusCorePlain(cal, fout);
                    sb.Append(fout);

                    sb.AppendLine("</pre></body></html>");
                    done = true;
                }
            }
            if (!done)
            {
                FormaterHtml.WriteCalendarPlusCoreHTML(cal, sb);
            }
        }


        public CELGenerateCalendarPlusCore()
        {
        }

        public CELGenerateCalendarPlusCore(GCAL.ContentServer content)
        {
            GPLocationProvider locProv = null;
            GPGregorianTime startWesternTime = null;
            GPGregorianTime endWesternTime = null;

            locProv = content.getLocationWithPostfix("");

            if (locProv == null)
            {
                HtmlText = "<p>Error: location provider is null";
                return;
            }

            startWesternTime = new GPGregorianTime(locProv);
            startWesternTime.setDate(content.getInt("startyear"), content.getInt("startmonth"), content.getInt("startday"));

            GPVedicTime startVedicTime, endVedicTime;
            int unitType = content.getInt("endperiodtype");
            int nCount = content.getInt("endperiodlength");

            GPEngine.VCTIMEtoVATIME(startWesternTime, out startVedicTime, locProv);

            GPEngine.CalcEndDate(locProv, startWesternTime, startVedicTime, out endWesternTime, out endVedicTime, unitType, GPEngine.CorrectedCount(unitType, nCount));

            SetData(locProv, startWesternTime, endWesternTime);
            SyncExecute();

            StringBuilder sb = new StringBuilder();
            FormaterHtml.WriteCalendarPlusCoreHTML_BodyTable(CalculatedObject as GPCalendarPlusEventsResults, sb);
            HtmlText = sb.ToString();
        }

    }
}
