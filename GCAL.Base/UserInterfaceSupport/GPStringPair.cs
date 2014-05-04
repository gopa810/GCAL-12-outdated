using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPStringPair
    {
        public string Name = string.Empty;
        public string Value = string.Empty;
        public bool Header = false;

        public GPStringPair()
        {
        }

        public GPStringPair(string a, string b)
        {
            Name = a;
            Value = b;
        }

        public GPStringPair(string a, string b, bool hdr)
        {
            Name = a;
            Value = b;
            Header = hdr;
        }
    }
}
