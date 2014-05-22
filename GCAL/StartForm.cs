using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GCAL
{
    public partial class StartForm : Form
    {
        private ContentServer content = new ContentServer();

        public StartForm()
        {
            InitializeComponent();

            content.ContentDir = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Content");
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
                content.LoadFile(e.Url.AbsolutePath);
            }
        }

    }
}
