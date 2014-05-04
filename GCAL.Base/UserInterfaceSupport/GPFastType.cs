using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPFastType
    {
        protected int fastType = 0;

        public GPFastType()
        {
        }

        public GPFastType(int ft)
        {
            setFastType(ft);
        }

        public static string getName(int i)
        {
            return GPStrings.getSharedStrings().getString(930 + i);
        }

        public static int count()
        {
            return 6;
        }

        public string name()
        {
            return GPFastType.getName(fastType);
        }

        public void setFastType(int value)
        {
            if (fastType < 0 || fastType > 5)
                throw new Exception("FastType out of range");
            fastType = value;
        }
        public int getFastType()
        {
            return fastType;
        }

        public override string ToString()
        {
            return name();
        }
    }
}
