using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GCAL.Base;

namespace GCAL.Controls
{
    public partial class CalendarControl : UserControl
    {
        int currentYear = -1;
        int currentMonth = -1;
        StringFormat centeredFormat = new StringFormat();
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
        Font titleFont;
        Dictionary<DayOfWeek, int> dowToInt = new Dictionary<DayOfWeek, int>();
        Dictionary<int, DayOfWeek> intToDow = new Dictionary<int, DayOfWeek>();
        int dayWidth = 30;
        int dayHeight = 24;

        public CalendarControl()
        {
            InitializeComponent();
            centeredFormat.Alignment = StringAlignment.Center;
            centeredFormat.LineAlignment = StringAlignment.Center;
            titleFont = new Font(SystemFonts.MenuFont.FontFamily, SystemFonts.MenuFont.Size * 1.5f);

            dowToInt.Add(DayOfWeek.Sunday, 0);
            dowToInt.Add(DayOfWeek.Monday, 1);
            dowToInt.Add(DayOfWeek.Tuesday, 2);
            dowToInt.Add(DayOfWeek.Wednesday, 3);
            dowToInt.Add(DayOfWeek.Thursday, 4);
            dowToInt.Add(DayOfWeek.Friday, 5);
            dowToInt.Add(DayOfWeek.Saturday, 6);

            intToDow.Add(0, DayOfWeek.Sunday);
            intToDow.Add(1, DayOfWeek.Monday);
            intToDow.Add(2, DayOfWeek.Tuesday);
            intToDow.Add(3, DayOfWeek.Wednesday);
            intToDow.Add(4, DayOfWeek.Thursday);
            intToDow.Add(5, DayOfWeek.Friday);
            intToDow.Add(6, DayOfWeek.Saturday);

        }

        private void CalendarControl_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DateTime dt = DateTime.Now;
            g.FillRectangle(SystemBrushes.ControlLightLight, Bounds);


            if (currentMonth < 0)
            {
                currentMonth = dt.Month;
                currentYear = dt.Year;
            }
            else
            {
                dt = new DateTime(currentYear, currentMonth, 1);
            }
            int firstDow = dowToInt[mfi.FirstDayOfWeek];
            int cdw = (dowToInt[dt.DayOfWeek] - firstDow + 7) % 7;
            int line = 0;
            int monthDays = GPGregorianTime.GetMonthMaxDays(currentYear, currentMonth);

            Rectangle rect = new Rectangle(0, 0, dayWidth * 7, dayHeight + 8);
            g.DrawString(string.Format("{0} {1}", mfi.GetMonthName(currentMonth), currentYear), titleFont, Brushes.Black, rect, centeredFormat);

            int tr = firstDow;
            for (int i = 0; i < 7; i++)
            {
                rect.Y = dayHeight + 16;
                rect.X = i * dayWidth;
                rect.Width = dayWidth;
                rect.Height = dayHeight;
                g.DrawString(mfi.GetAbbreviatedDayName(intToDow[tr]), SystemFonts.MenuFont, Brushes.Gray, rect, centeredFormat);
                tr = (tr + 1) % 7;
            }
            for (int i = 1; i <= monthDays; i++)
            {
                rect.X = cdw * dayWidth;
                rect.Y = 2*dayHeight + 16 + line * dayHeight;
                g.DrawString(i.ToString(), SystemFonts.MenuFont, Brushes.Black, rect, centeredFormat);
                cdw++;
                if (cdw > 6)
                {
                    cdw = 0;
                    line++;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currentMonth--;
            if (currentMonth < 1)
            {
                currentMonth = 12;
                currentYear--;
            }
            Invalidate();
            Update();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentMonth++;
            if (currentMonth > 12)
            {
                currentMonth = 1;
                currentYear++;
            }
            Invalidate();
            Update();
        }
    }
}
