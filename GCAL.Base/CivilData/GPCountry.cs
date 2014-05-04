using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPCountry
    {
        private string name = string.Empty;
        private string code = string.Empty;

        public string getName()
        {
            return name;
        }

        public void setName(String value)
        {
            name = value;
        }

        public string getCode()
        {
            return code;
        }

        public void setCode(string value)
        {
            code = value;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
