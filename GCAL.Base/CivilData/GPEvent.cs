using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPEvent
    {
        public static int globalEventId = 1;

        public int eventId = 0;
        public int nClass = 0;
        private int nFastType = 0;
        public int nVisible = 0;
        public int nStartYear = 0;
        public int nUsed = 0;
        public int nDeleted = 0;
        public int nSpec = 0;

        public string strFastSubject = string.Empty;
        public string strText = string.Empty;
        public List<GPEvent> childrenItems = null;

        // offset in days
        // event specification + nOffset is day of notification
        public int nOffset = 0;

        public GPEvent()
        {
            initEventId();
        }

        public GPEvent(string text)
        {
            strText = text;
            initEventId();
        }

        private void initEventId()
        {
            eventId = globalEventId;
            globalEventId++;
        }

        public virtual string getShortDesc()
        {
            return "Event";
        }

        public override string ToString()
        {
            return strText;
        }

        public void setRawFastType(int value)
        {
            nFastType = value;
        }

        public int getRawFastType()
        {
            return nFastType;
        }

        public void addChildrenItem(GPEvent ev)
        {
            if (childrenItems == null)
                childrenItems = new List<GPEvent>();
            childrenItems.Add(ev);
        }

        public bool hasChildrenItems()
        {
            return childrenItems != null && childrenItems.Count > 0;
        }

        public int getChildIndex(int nSpecCode)
        {
            if (childrenItems == null)
                return -1;
            for (int i = 0; i < childrenItems.Count; i++)
            {
                if (childrenItems[i].nSpec == nSpecCode)
                {
                    return i;
                }
            }
            return -1;
        }

        public void removeChildItem(int nSpecCode)
        {
            int i = getChildIndex(nSpecCode);
            if (i >= 0)
                childrenItems.RemoveAt(i);
        }

        public int getFastType()
        {
            if (nFastType == 0)
                return nFastType;
            if (GPUserDefaults.BoolForKey("gen.oldstyle.fasting", false))
            {
                return nFastType;
            }

            if (nClass > 0)
                return GPConstants.FAST_NULL;

            if (nSpec == GPConstants.SPEC_RADHASTAMI)
                return GPConstants.FAST_NULL;

            return GPConstants.FAST_DAY;
        }

        public bool hasFasting()
        {
            return nFastType != 0;
        }
    }
}
