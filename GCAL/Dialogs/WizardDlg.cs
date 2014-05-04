using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GCAL.Dialogs
{
    public partial class WizardDlg : Form
    {
        public string nextButtonTitle = "Next >>";
        public string prevButtonTitle = "<< Prev";
        public string cancelButtonTitle = "Cancel";
        public string okButtonTitle = "OK";

        class PageInfo
        {
            public Control ctrl;
            public string title;
            public string subtitle;

            public PageInfo()
            {
            }

            public PageInfo(Control ictrl, string s, string st)
            {
                ctrl = ictrl;
                title = s;
                subtitle = st;
            }
        }

        List<PageInfo> pages = new List<PageInfo>();
        int currPage = -1;

        public WizardDlg()
        {
            InitializeComponent();
        }

        public string WizardTitle
        {
            get
            {
                return Text;
            }
            set
            {
                Text = value;
            }
        }

        public void AddPage(Control ctrl, string title, string subtitle)
        {
            pages.Add(new PageInfo(ctrl, title, subtitle));
            panel1.Controls.Add(ctrl);

            ctrl.Location = new System.Drawing.Point(19, 36);
            ctrl.Size = new System.Drawing.Size(42, 13);
            ctrl.TabIndex = 5;
            ctrl.TabStop = true;
            ctrl.Dock = DockStyle.Fill;
            ctrl.Visible = false;
        }

        public void ResetPages()
        {
            setPage(0);
        }

        public void setPage(int i)
        {
            if (i < pages.Count)
            {
                foreach (Control c in panel1.Controls)
                {
                    if (c.Visible == true)
                        c.Visible = false;
                }
                pages[i].ctrl.Visible = true;
                label1.Text = string.Format("{0} ({1}/{2})", pages[i].title, i + 1, pages.Count);
                label2.Text = pages[i].subtitle;

                button1.Visible = (i > 0);

                button1.Text = prevButtonTitle;
                button2.Text = (i < (pages.Count - 1) ? nextButtonTitle : okButtonTitle);
            }
            else
            {
                button1.Visible = false;
                button2.Text = okButtonTitle;
            }

            currPage = i;
            pages[i].ctrl.Select();
        }

        /// <summary>
        /// button prev
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (currPage > 0)
                setPage(currPage - 1);
        }

        /// <summary>
        /// button next
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (currPage < (pages.Count - 1))
            {
                setPage(currPage + 1);
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
