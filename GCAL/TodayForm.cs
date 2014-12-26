using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using GCAL.Base;
using GCAL.Dialogs;

namespace GCAL
{
    public partial class TodayForm : Form
    {
        private static TodayForm shared_today_form = null;
        private GPLocationProvider myLocation = null;
        private GPGregorianTime myDate = null;
        private int scaleIndex = 3;
        private int[] scales = new int[] { 8, 9, 10, 12, 14, 17, 20, 25, 30, 40 };

        public static void ShowForm()
        {
            ShowForm(null);
        }

        public void SetUserInterfaceStrings()
        {
            this.toolStripMenuItem1.Text = GPStrings.getString(337);
            this.saveAsToolStripMenuItem.Text = GPStrings.getString(362);
            this.printToolStripMenuItem.Text = GPStrings.getString(363);
            this.editToolStripMenuItem.Text = GPStrings.getString(338);
            this.selectAllToolStripMenuItem.Text = GPStrings.getString(364);
            this.copyToolStripMenuItem.Text = GPStrings.getString(365);
            this.viewToolStripMenuItem.Text = GPStrings.getString(339);
            this.increaseTextSizeToolStripMenuItem.Text = GPStrings.getString(366);
            this.toolStripButton4.Text = GPStrings.getString(366);
            this.decreaseTextSizeToolStripMenuItem.Text = GPStrings.getString(367);
            this.toolStripButton5.Text = GPStrings.getString(367);
            this.navigateToolStripMenuItem.Text = GPStrings.getString(368);
            this.previousDayToolStripMenuItem.Text = GPStrings.getString(369);
            this.toolStripButton1.ToolTipText = GPStrings.getString(369);
            this.todayToolStripMenuItem.Text = GPStrings.getString(43);
            this.toolStripButton3.ToolTipText = GPStrings.getString(371);
            this.nextDayToolStripMenuItem.Text = GPStrings.getString(370);
            this.toolStripButton2.ToolTipText = GPStrings.getString(370);
            this.settingsToolStripMenuItem.Text = GPStrings.getString(372);
            this.myLocationToolStripMenuItem.Text = GPStrings.getString(373);
            this.displaySettingsToolStripMenuItem.Text = GPStrings.getString(374);
            this.windowAutosizeToolStripMenuItem.Text = GPStrings.getString(375);
            this.visibleAtLaunchToolStripMenuItem.Text = GPStrings.getString(376);
            this.toolStripMenuItem3.Text = GPStrings.getString(377);
            this.Text = GPStrings.getString(43);
        }

        /// <summary>
        /// static function for displaying Today screen
        /// </summary>
        public static void ShowForm(GPGregorianTime initDate)
        {
            if (shared_today_form == null)
            {
                shared_today_form = new TodayForm(initDate);
                if (GPUserDefaults.BoolForKey("todayform.bounds.initd", false))
                {
                    shared_today_form.StartPosition = FormStartPosition.Manual;
                    shared_today_form.Location = new Point(GPUserDefaults.IntForKey("todayform.bounds.left", 10), GPUserDefaults.IntForKey("todayform.bounds.top", 10));
                    shared_today_form.Size = new Size(GPUserDefaults.IntForKey("todayform.bounds.width", 200), GPUserDefaults.IntForKey("todayform.bounds.height", 400));
                }
                shared_today_form.Show();
            }
            else
            {
                if (initDate != null)
                    shared_today_form.SetDate(initDate);
                shared_today_form.BringToFront();
                shared_today_form.Activate();
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public TodayForm(GPGregorianTime initDate)
        {
            InitializeComponent();

            InitializeData();

            SetWebBrowserAutoSizeBehaviour(GPDisplays.Today.WindowAutosize());

            if (initDate == null)
                myDate.Today();
            else
                myDate.Copy(initDate);
            RefreshText();

            SupervisorForm.WindowOpened(this);

            //object c = TimeZoneInfo.GetSystemTimeZones();
        }

        public void SetWebBrowserAutoSizeBehaviour(bool autosize)
        {
            webBrowser1.ScrollBarsEnabled = !autosize;
            if (autosize)
            {
                webBrowser1.Dock = DockStyle.None;
                webBrowser1.Size = new Size(1000, 2000);
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            }
            else
            {
                webBrowser1.Dock = DockStyle.Fill;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            }
        }

        public void InitializeData()
        {
            GPLocationProvider loc = GPAppHelper.getMyLocation();

            myLocation = loc;

            myDate = new GPGregorianTime(loc);
            myDate.Today();

            scaleIndex = GPUserDefaults.IntForKey("todayform.scaleindex", 2);
        }

        public void RefreshText()
        {
            StringBuilder sb = new StringBuilder();
            FormaterHtml.WriteTodayInfoHTML(myDate, myLocation, sb, scales[scaleIndex], null);
            webBrowser1.DocumentText = sb.ToString();

        }

        /// <summary>
        /// Sets specific date to display
        /// </summary>
        /// <param name="initDate"></param>
        public void SetDate(GPGregorianTime initDate)
        {
            myDate.Copy(initDate);
            RefreshText();
        }

        /// <summary>
        /// Sets current day to display
        /// </summary>
        public void SetToday()
        {
            myDate.Today();
            RefreshText();
        }

        /// <summary>
        /// Sets previous day relative to currently displayed
        /// (this is not necessary yesterday, it only shifts display date one day back)
        /// </summary>
        public void SetPreviousDay()
        {
            myDate.PreviousDay();
            RefreshText();
        }

        /// <summary>
        /// Sets next day relative to currently displayed
        /// (this is not necessary tomorrow, it shifts date one day forward)
        /// </summary>
        public void SetNextDay()
        {
            myDate.NextDay();
            RefreshText();
        }

        public static bool EditMyLocation()
        {
            GetLocationDlg dlg = new GetLocationDlg("mylocation_get");
            dlg.NextButtonText = "Set";
            dlg.PrevButtonVisible = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
            {
                GPAppHelper.setMyLocation(dlg.SelectedLocation);
                return true;
            }
            return false;
        }

        private void myLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditMyLocation())
            {
                myLocation = GPAppHelper.getMyLocation();
                RefreshText();
            }
        }

        private void previousDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPreviousDay();
        }

        private void todayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetToday();
        }

        private void nextDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNextDay();
        }

        private void increaseTextSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scaleIndex < scales.Length - 1)
            {
                scaleIndex++;
                RefreshText();
            }
        }

        private void decreaseTextSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scaleIndex > 0)
            {
                scaleIndex--;
                RefreshText();
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("SelectAll", true, null);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("Copy", true, null);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sdlg = new SaveFileDialog();
            sdlg.Filter = GPAppHelper.MakeFilterString(FileFormatType.PlainText, FileFormatType.RichText, FileFormatType.HtmlText);

            if (sdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();
                if (sdlg.FilterIndex == 1)
                {
                    FormaterPlain.AvcGetTodayInfo(myDate, myLocation, sb);
                }
                else if (sdlg.FilterIndex == 2)
                {
                    FormaterRtf.FormatTodayInfoRtf(myDate, myLocation, sb);
                }
                else if (sdlg.FilterIndex == 3)
                {
                    FormaterHtml.WriteTodayInfoHTML(myDate, myLocation, sb, 11, null);
                }
                File.WriteAllText(sdlg.FileName, sb.ToString());
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("Print", true, null);
        }

        private void TodayForm_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void TodayForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Down)
                    decreaseTextSizeToolStripMenuItem_Click(sender, new EventArgs());
                else if (e.KeyCode == Keys.Up)
                    increaseTextSizeToolStripMenuItem_Click(sender, new EventArgs());
                else if (e.KeyCode == Keys.Left)
                    previousDayToolStripMenuItem_Click(sender, new EventArgs());
                else if (e.KeyCode == Keys.Right)
                    nextDayToolStripMenuItem_Click(sender, new EventArgs());
                else if (e.KeyCode == Keys.T)
                    todayToolStripMenuItem_Click(sender, new EventArgs());
            }
        }

        private void displaySettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplaySettingsDlg dlg = new DisplaySettingsDlg("today", "calendar");
            dlg.ShowDialog();
            RefreshText();
        }

        private void TodayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            shared_today_form = null;
            GPUserDefaults.SetIntForKey("todayform.bounds.left", Location.X);
            GPUserDefaults.SetIntForKey("todayform.bounds.top", Location.Y);
            GPUserDefaults.SetIntForKey("todayform.bounds.width", Size.Width);
            GPUserDefaults.SetIntForKey("todayform.bounds.height", Size.Height);
            GPUserDefaults.SetBoolForKey("todayform.bounds.initd", true);

            SupervisorForm.WindowClosed(this);

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (GPDisplays.Today.WindowAutosize())
            {
                //HtmlElement elem = webBrowser1.Document.GetElementById("mainContentDiv");

                //this.Size = new Size(elem.OffsetRectangle.Right + (Size.Width - ClientSize.Width + 16), 
                //    elem.OffsetRectangle.Bottom + (Size.Height - ClientSize.Height) + toolStrip1.Height + menuStrip1.Height + 24);
            }
        }

        private void windowAutosizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GPDisplays.Today.WindowAutosize(!GPDisplays.Today.WindowAutosize());
            SetWebBrowserAutoSizeBehaviour(GPDisplays.Today.WindowAutosize());
            RefreshText();
        }

        private void settingsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            windowAutosizeToolStripMenuItem.Checked = GPDisplays.Today.WindowAutosize();
            visibleAtLaunchToolStripMenuItem.Checked = GPDisplays.Today.VisibleAtLaunch();
            toolStripMenuItem3.Checked = this.TopMost;
        }

        private void visibleAtLaunchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GPDisplays.Today.VisibleAtLaunch(!GPDisplays.Today.VisibleAtLaunch());
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            TopMost = !TopMost;
            GPDisplays.Today.TopmostWindow(TopMost);
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.AbsoluteUri;

            if (url == "about:blank")
            {
            }
            else if (url.StartsWith("http://gcal.app/"))
            {
                e.Cancel = true;
                url = url.Substring("http://gcal.app/".Length);
                if (url == "mylocation")
                {
                    if (EditMyLocation())
                    {
                        myLocation = GPAppHelper.getMyLocation();
                        RefreshText();
                    }
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                if (keyData == Keys.F5)
                {
                    Debugger.Log(0, "", string.Format("{0} pressed\n", msg.Msg));
                    SetPreviousDay();
                    return true;
                }
                else if (keyData == Keys.F6)
                {
                    SetToday();
                    return true;
                }
                else if (keyData == Keys.F7)
                {
                    Debugger.Log(0, "", string.Format("{0} pressed\n", msg.Msg));
                    SetNextDay();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
