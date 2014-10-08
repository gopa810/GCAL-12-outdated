using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GCAL.Base;

namespace GCAL
{
    public partial class StartForm : Form
    {
        private ContentServer content = new ContentServer();

        public StartForm()
        {
            InitializeComponent();

            content.ContentDir = Path.Combine(GPFileHelper.getAppExecutableDirectory(), "Content");
            content.LoadFlows();
            content.WebBrowser = webBrowser1;
            content.TopButtons.Add(button1);
            content.TopButtons.Add(button2);
            content.TopButtons.Add(button3);
            content.TopButtons.Add(button4);
            content.TopButtons.Add(button5);
            content.TopButtons.Add(button6);
            content.BottomButtons.Add(buttonBottom1);
            content.BottomButtons.Add(buttonBottom2);
            content.BottomButtons.Add(buttonBottom3);
            content.BottomButtons.Add(buttonBottom4);
            content.MainForm = this;
            content.TopBar = flowLayoutPanel1;
            content.BottomBar = flowLayoutPanel2;
            webBrowser1.ObjectForScripting = content;
            content.LoadStartPage();
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.AbsolutePath;

            if (!url.Equals("blank"))
            {
                e.Cancel = true;
                string filePath = e.Url.AbsolutePath;
                string file = Path.GetFileNameWithoutExtension(filePath);
                content.LoadPage(file, true);
            }
        }

        private void StartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            GPStrings.getSharedStrings().Save();
            GPLocationList.getShared().Save();
            GPEventList.getShared().Save();
            GPCountryList.getShared().Save();
            GPTimeZoneList.sharedTimeZones().Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button b = sender as Button;
                if (b.Tag != null)
                {
                    content.ExecuteCommand(b.Tag.ToString());
                }
            }
        }

    }
}
