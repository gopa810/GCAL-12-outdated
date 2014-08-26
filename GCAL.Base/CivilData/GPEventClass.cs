using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPEventClass
    {
        public int group = 0;


        public GPEventClass()
        {
        }

        public GPEventClass(int grp)
        {
            group = grp;
        }

        public static string getName(int i)
        {
            return GPStrings.getString(i + 920);
        }

        public static int count()
        {
            return 7;
        }

        public string name()
        {
            return GPEventClass.getName(group);
        }

        public override string ToString()
        {
            return name();
        }
    }
}
