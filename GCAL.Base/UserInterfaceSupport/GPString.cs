using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPString
    {
        public int index = 0;
        public String rawHtml = String.Empty;

        public static string plainToHtml(string s)
        {
            return System.Net.WebUtility.HtmlEncode(s);
        }

        public static string htmlToPlain(string s)
        {
            return System.Net.WebUtility.HtmlDecode(s);
        }
    }
}
