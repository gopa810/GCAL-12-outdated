using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPSortedIntStringListItem
    {
        public int number;
        public string text;
    }

    public class GPSortedIntStringList
    {
        public bool Flag = false;
        private List<GPSortedIntStringListItem> list = new List<GPSortedIntStringListItem>();

        public void push(int n, string s)
        {
            GPSortedIntStringListItem np = new GPSortedIntStringListItem();
            np.number = n;
            np.text = s;

            if (!Flag)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].number > n)
                    {
                        list.Insert(i, np);
                        return;
                    }
                    if (list[i].number == n)
                        return;
                }
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].text.CompareTo(s) > 0)
                    {
                        list.Insert(i, np);
                        return;
                    }
                    if (list[i].text.CompareTo(s) == 0)
                        return;
                }
            }

            list.Add(np);
        }

        public int length
        {
            get
            {
                return list.Count;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (GPSortedIntStringListItem si in list)
            {
                if (sb.Length > 0)
                    sb.Append("<line>");
                sb.AppendFormat("{0}<br>{1}", si.number, si.text);
            }
            return sb.ToString();
        }
    }
}
