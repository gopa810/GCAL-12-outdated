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
    }
}
