using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GCAL.Base;

namespace GCAL.Engine
{
    public class CELGenerateAppearanceDay: CELGenerateHtml
    {
        public GPLocationProvider Location = null;
        public GPGregorianTime DateTime = null;


        public void SetData(GPLocationProvider aloc, GPGregorianTime dt)
        {
            Location = aloc;
            DateTime = new GPGregorianTime(dt);
        }

        protected override void Execute()
        {
            GPAppDayResults app = new GPAppDayResults();

            app.calculateAppearanceDayData(Location, DateTime);

            StringBuilder sb = new StringBuilder();

            FormatOutput(sb, app);

            HtmlText = sb.ToString();
            CalculatedObject = app;
        }

        public void FormatOutput(StringBuilder sb, GPAppDayResults cal)
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
                    FormaterPlain.FormatAppDayText(cal, fout);
                    sb.Append(fout);

                    sb.AppendLine("</pre></body></html>");
                    done = true;
                }
            }
            if (!done)
            {
                FormaterHtml.WriteAppDayHTML(cal, sb);
            }
        }

        public CELGenerateAppearanceDay()
        {
        }

        public CELGenerateAppearanceDay(GCAL.ContentServer content)
        {
            GPLocationProvider locProv = null;
            GPGregorianTime startWesternTime = null;
            GPGregorianTime endWesternTime = null;

            if (content.getString("locationtype") == "selected")
            {
                GPLocation loc = GPLocationList.getShared().findLocationById(content.getInt("locationid"));
                if (loc != null)
                    locProv = new GPLocationProvider(loc);
            }

            if (locProv == null)
            {
                HtmlText = "<p>Error: location provider is null";
                return;
            }

            startWesternTime = new GPGregorianTime(locProv);
            startWesternTime.setDate(content.getInt("startyear"), content.getInt("startmonth"), content.getInt("startday"));
            startWesternTime.setDayHours(content.getInt("starthour") / 24.0 + content.getInt("startmin") / 1440.0);


            GPVedicTime startVedicTime, endVedicTime;
            int unitType = content.getInt("endperiodtype");
            int nCount = content.getInt("endperiodlength");

            GPEngine.VCTIMEtoVATIME(startWesternTime, out startVedicTime, locProv);

            GPEngine.CalcEndDate(locProv, startWesternTime, startVedicTime, out endWesternTime, out endVedicTime, unitType, GPEngine.CorrectedCount(unitType, nCount));

            nCount = Convert.ToInt32(endWesternTime.getJulianGreenwichNoon() - startWesternTime.getJulianGreenwichNoon());

            SetData(locProv, startWesternTime);
            SyncExecute();

            StringBuilder sb = new StringBuilder();
            FormaterHtml.WriteAppDayHTML_BodyTable(CalculatedObject as GPAppDayResults, sb);
            HtmlText = sb.ToString();
        }


    }
}
