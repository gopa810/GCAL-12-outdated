using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GCAL.Dialogs
{
    public class ListItemsComparer: IComparer
    {
        private int column = 0;

        public ListItemsComparer()
        {
        }

        public ListItemsComparer(int iColumn)
        {
            column = iColumn;
        }

        int IComparer.Compare(Object x, Object y)
        {
            if (x is ListViewItem && y is ListViewItem)
            {
                ListViewItem la = x as ListViewItem;
                ListViewItem lb = y as ListViewItem;

                return la.SubItems[column].Text.CompareTo(lb.SubItems[column].Text);
            }

            return 0;
        }
    }
}
