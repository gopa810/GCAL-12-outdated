using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GCAL.Base;

namespace GCAL.Engine
{
    public class CELGenerateHtml : CELBase, IReportProgress
    {
        public static string Template = string.Empty;

        private string html = string.Empty;
        public string SearchText = null;

        public object CalculatedObject = null;

        /// <summary>
        /// when htmltext is set
        /// it checks it we search for some words
        /// if yes, parses string in value parameter
        /// and highlights the found terms
        /// </summary>
        public string HtmlText
        {
            get 
            {
                return html; 
            }
            set 
            {
                if (SearchText == null)
                {
                    html = value;
                    return;
                }
                string spanStart = "<span style='background:yellow'>";
                string spanEnd = "</span>";
                int i = 0;
                int prev = 0;
                StringBuilder sb = new StringBuilder();
                i = value.IndexOf(SearchText, i, value.Length - i, StringComparison.CurrentCultureIgnoreCase);
                while (i >= 0)
                {
                    if (IsTextPosition(value, i))
                    {
                        sb.Append(value.Substring(prev, i - prev));
                        sb.Append(spanStart);
                        sb.Append(value.Substring(i, SearchText.Length));
                        sb.Append(spanEnd);
                        i += SearchText.Length;
                        prev = i;
                    }
                    else
                    {
                        i += SearchText.Length;
                    }
                    i = value.IndexOf(SearchText, i, value.Length - i, StringComparison.CurrentCultureIgnoreCase);
                }
                sb.Append(value.Substring(prev));
                html = sb.ToString();
            }
        }

        /// <summary>
        /// checks is given position in html text is within some tag or
        /// outside of tags
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsTextPosition(string str, int pos)
        {
            for (int i = pos; i >= 0; i--)
            {
                if (str[i] == '<')
                    return false;
                if (str[i] == '>')
                    return true;
            }
            return true;
        }

        public void ReportProgressBase(double d)
        {
            ReportProgress(d);
        }
    }
}
