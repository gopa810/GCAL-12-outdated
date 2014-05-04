using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GCAL.Base;

namespace GCAL.Engine
{
    public class CELGenerateMasaList: CELGenerateHtml
    {
        public GPLocationProvider location = null;
        public int startYear = 2013;
        public int count = 1;

        public void SetData(GPLocationProvider lo, int sy, int cy)
        {
            location = lo;
            startYear = sy;
            count = cy;
        }

        protected override void Execute()
        {
            GPMasaListResults mas = new GPMasaListResults();

            mas.progressReport = this;

            if (location != null)
            {
                mas.CalcMasaList(location, startYear, count);
            }

            StringBuilder sb = new StringBuilder();
            FormatOutput(sb, mas);

            HtmlText = sb.ToString();
            CalculatedObject = mas;

        }

        public void FormatOutput(StringBuilder sb, GPMasaListResults cal)
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
                    FormaterPlain.FormatMasaListText(cal, fout);
                    sb.Append(fout);

                    sb.AppendLine("</pre></body></html>");
                    done = true;
                }
            }
            if (!done)
            {
                FormaterHtml.WriteMasaListHTML(cal, sb);
            }
        }
    }
}
