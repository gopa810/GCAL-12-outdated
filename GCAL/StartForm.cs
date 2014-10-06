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

    }
}
