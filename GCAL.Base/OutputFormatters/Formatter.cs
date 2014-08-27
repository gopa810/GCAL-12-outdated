using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class Formatter
    {
        public static string gpszSeparator = "---------------------------------------------------------------------------------------------";
        public static int getShowSettingsValue(int i)
        {
            return GPShowSetting.getValue(i);
        }

        public static string getSharedStringHtml(int index)
        {
            return GPStrings.getString(index);
        }

        public static string getSharedStringRtf(int index)
        {
            return getRtfFromUnicode(getSharedStringPlain(index));
        }

        public static string getRtfFromUnicode(string p)
        {
            StringBuilder sb = new StringBuilder();
            bool wasEscaped = false;
            foreach (char c in p)
            {
                if (Convert.ToInt32(c) >= 128)
                {
                    sb.AppendFormat("\\u{0}", Convert.ToInt32(c));
                    wasEscaped = true;
                }
                else if (c == '\\' || c == '{' || c == '}')
                {
                    sb.AppendFormat("\\{0}", c);
                    wasEscaped = true;
                }
                else
                {
                    if (wasEscaped)
                        sb.Append(' ');
                    sb.Append(c);
                    wasEscaped = false;
                }
            }

            return sb.ToString();
        }

        public static string getSharedStringPlain(int index)
        {
            return getUnicodeFromHtml(GPStrings.getString(index));
        }


        public static string getHtmlFromUnicode(string s)
        {
            if (s == null)
                return "";
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (c == '&')
                {
                    sb.Append("&amp;");
                }
                else if (c == '<')
                {
                    sb.Append("&lt;");
                }
                else if (c == '>')
                {
                    sb.Append("&gt;");
                }
                else if (Convert.ToInt32(c) >= 128)
                {
                    sb.AppendFormat("&#{0};", Convert.ToInt32(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string getUnicodeFromHtml(string s)
        {
            if (s == null)
                return "";
            StringBuilder sb = new StringBuilder();
            int mode = 0;
            for (int i = 0; i < s.Length; i++)
            {
                char u = s[i];
                if (mode == 0)
                {
                    if (u == '&')
                    {
                        int end = s.IndexOf(';', i);
                        if (end >= 0)
                        {
                            string sub = s.Substring(i, end - i + 1);
                            i = end;
                            if (sub == "&amp;")
                            {
                                sb.Append('&');
                            }
                            else if (sub == "&gt;")
                            {
                                sb.Append(">");
                            }
                            else if (sub == "&lt;")
                            {
                                sb.Append("<");
                            }
                            else if (sub.StartsWith("&#"))
                            {
                                int io;
                                int.TryParse(sub.Substring(2, sub.Length - 3), out io);
                                sb.Append(Convert.ToChar(io));
                            }
                        }
                        else
                        {
                            sb.Append(u);
                        }
                    }
                    else
                    {
                        sb.Append(u);
                    }
                }
            }
            return sb.ToString();
        }
    }
}
