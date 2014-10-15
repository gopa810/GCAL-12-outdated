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

            locProv = content.getLocationWithPostfix("");

            if (locProv == null)
            {
                locProv = GPAppHelper.getMyLocation();
            }

            startWesternTime = new GPGregorianTime(locProv);
            startWesternTime.setDate(content.getInt("startyear", startWesternTime.getYear()),
                content.getInt("startmonth", startWesternTime.getMonth()),
                content.getInt("startday", startWesternTime.getDay()));
            startWesternTime.setDayHours(content.getInt("starthour", startWesternTime.getHour()) / 24.0 + content.getInt("startmin", startWesternTime.getMinuteRound()) / 1440.0);

            SetData(locProv, startWesternTime);
            SyncExecute();

            StringBuilder sb = new StringBuilder();
            FormaterHtml.WriteAppDayHTML_BodyTable(CalculatedObject as GPAppDayResults, sb);
            HtmlText = sb.ToString();
        }


    }
}
