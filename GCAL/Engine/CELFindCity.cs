using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GCAL.Base;

namespace GCAL.Engine
{
    public class CELFindCity: CELBase
    {
        public List<GPLocation> Locations = new List<GPLocation>();
        public bool Cancelled = false;
        public string text = string.Empty;
        public bool Valid = true;

        protected override void Execute()
        {
            try
            {
                if (text.Length > 0)
                {
                    foreach (GPLocation loc in GPLocationList.getShared().locations)
                    {
                        if (loc.getCity().IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            Locations.Add(loc);
                        }
                        if (Cancelled)
                        {
                            break;
                        }
                    }

                    Valid = !Cancelled;
                }
            }
            catch
            {
                Valid = false;
            }
        }
    }
}
