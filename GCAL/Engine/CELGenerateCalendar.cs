using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GCAL.Base;

namespace GCAL.Engine
{
    public class CELGenerateCalendar: CELGenerateHtml
    {
        public GPLocationProvider location = null;
        public GPGregorianTime startDate = null;
        public int nCount = 1;

        public void SetData(GPLocationProvider lo, GPGregorianTime sd, int cn)
        {
            location = lo;
            startDate = sd;
            nCount = cn;
        }

        protected override void Execute()
        {
            GPCalendarResults cal = new GPCalendarResults();

            cal.progressReport = this;

            if (location != null && startDate != null)
            {
                cal.CalculateCalendar(startDate, nCount);
            }

            StringBuilder sb = new StringBuilder();

            FormatOutput(sb, cal);

            HtmlText = sb.ToString();
            CalculatedObject = cal;
            
        }

        public void FormatOutput(StringBuilder sb, GPCalendarResults cal)
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
                    FormaterPlain.FormatCalendarOld(cal, fout);
                    sb.Append(fout);

                    sb.AppendLine("</pre></body></html>");
                    done = true;
                }
                else if (Template == "default:table")
                {
                    FormaterHtml.WriteCalendarHtmlTable(cal, sb);
                    done = true;
                }
            }
            if (!done)
            {
                FormaterHtml.WriteCalendarHTML(cal, sb);
            }
        }
    }
}
