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

        public static List<string> getSharedStrings()
        {
            return GPStrings.getSharedStrings().gstr;
        }

        public static string getSharedString(int index)
        {
            return GPStrings.getSharedStrings().gstr[index];
        }
    }
}
