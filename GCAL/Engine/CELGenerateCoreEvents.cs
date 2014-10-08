using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GCAL.Base;

namespace GCAL.Engine
{
    public class CELGenerateCoreEvents: CELGenerateHtml
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
            GPCoreEventResults evnts = new GPCoreEventResults();

            evnts.progressReport = this;

            if (location != null && startDate != null && endDate != null)
            {
                evnts.CalculateEvents(location, startDate, endDate);
            }

            StringBuilder sb = new StringBuilder();

            FormatOutput(sb, evnts);

            HtmlText = sb.ToString();
            CalculatedObject = evnts;

        }


        public void FormatOutput(StringBuilder sb, GPCoreEventResults cal)
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
                    FormaterPlain.FormatEventsText(cal, fout);
                    sb.Append(fout);

                    sb.AppendLine("</pre></body></html>");
                    done = true;
                }
            }
            if (!done)
            {
                FormaterHtml.WriteEventsHTML(cal, sb);
            }
        }

        public CELGenerateCoreEvents()
        {
        }

        public CELGenerateCoreEvents(GCAL.ContentServer content)
        {
            GPLocationProvider locProv = null;
            GPGregorianTime startWesternTime = null;
            GPGregorianTime endWesternTime = null;

            /*if (content.getString("locationtype") == "selected")
            {
                GPLocation loc = GPLocationList.getShared().findLocationById(content.getInt("locationid"));
                if (loc != null)
                    locProv = new GPLocationProvider(loc);
            }*/

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

            nCount = Convert.ToInt32(endWesternTime.getJulianGreenwichNoon() - startWesternTime.getJulianGreenwichNoon());

            SetData(locProv, startWesternTime, endWesternTime);
            SyncExecute();

            StringBuilder sb = new StringBuilder();
            FormaterHtml.WriteEventsHTML_BodyTable(CalculatedObject as GPCoreEventResults, sb);
            HtmlText = sb.ToString();
        }

    }
}
