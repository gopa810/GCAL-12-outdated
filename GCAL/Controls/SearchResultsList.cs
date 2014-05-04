using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using GCAL.Base;
using GCAL.Engine;

namespace GCAL.Controls
{
    public partial class SearchResultsList : UserControl
    {
        private Font boldFont = null;
        private Font normalFont = null;
        private Font typeFont = null;

        public delegate void ChangedEventHandler(object sender, EventArgs e);

        public event ChangedEventHandler SelectedItemChanged;

        public SearchResultsList()
        {
            InitializeComponent();

            boldFont = new Font(SystemFonts.CaptionFont, FontStyle.Bold);
            normalFont = SystemFonts.MenuFont;
            typeFont = SystemFonts.SmallCaptionFont;
        }

        public void SetUserInterfaceStrings()
        {
        }



        public ListBox.ObjectCollection Items
        {
            get
            {
                return listBox1.Items;
            }
        }

        public ListBox.SelectedObjectCollection SelectedItems
        {
            get
            {
                return listBox1.SelectedItems;
            }
        }

        public void BeginUpdate()
        {
            listBox1.BeginUpdate();
        }

        public void EndUpdate()
        {
            listBox1.EndUpdate();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            CELSearch.Results res = listBox1.Items[e.Index] as CELSearch.Results;
            e.ItemHeight = 6 + boldFont.Height + (normalFont.Height + 1) * res.Lines.Count + typeFont.Height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            CELSearch.Results res = listBox1.Items[e.Index] as CELSearch.Results;

            e.DrawBackground();
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(Brushes.LightYellow, e.Bounds);
            }


            PointF pt = new PointF(e.Bounds.Left + 4, e.Bounds.Top + 4);
            e.Graphics.DrawString(res.Title, boldFont, Brushes.Black, pt, StringFormat.GenericDefault);
            SizeF siz;
            pt.Y += boldFont.Height;
            e.Graphics.DrawString(res.Type, typeFont, Brushes.Blue, pt);
            pt.Y += typeFont.Height + 2;
            PointF curr = new PointF(pt.X, pt.Y);
            foreach(CELSearch.ResultsLine line in res.Lines)
            {
                siz = e.Graphics.MeasureString(line.Prefix, normalFont);
                e.Graphics.DrawString(line.Prefix, normalFont, Brushes.Black, curr);
                curr.X += siz.Width;
                siz = e.Graphics.MeasureString(line.Term, normalFont);
                e.Graphics.FillRectangle(Brushes.LightGreen, curr.X, curr.Y, siz.Width, siz.Height);
                e.Graphics.DrawString(line.Term, normalFont, Brushes.Black, curr);
                curr.X += siz.Width-2;
                siz = e.Graphics.MeasureString(line.Postfix, normalFont);
                e.Graphics.DrawString(line.Postfix, normalFont, Brushes.Black, curr);
                curr.X = pt.X;
                curr.Y += normalFont.Height + 1;
            }

        }
    }
}
