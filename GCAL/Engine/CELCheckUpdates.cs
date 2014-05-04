using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace GCAL.Engine
{
    public class CELCheckUpdates: CELBase
    {
        public string ErrorString = string.Empty;
        public string Version = string.Empty;
        public string VersionLink = string.Empty;

        public bool Success
        {
            get
            {
                return (Version.Length > 0 && VersionLink.Length > 0);
            }
        }

        protected override void Execute()
        {
            WebClient wc = new WebClient();

            try
            {
                string fileInfo = wc.DownloadString("http://gopal.home.sk/gcal/VersionInfo.txt");
                if (fileInfo != null && fileInfo.Length > 0)
                {
                    string[] lines = fileInfo.Split('\r', '\n');
                    List<string> sts = new List<string>();
                    foreach (string s in lines)
                    {
                        if (s.Length > 0)
                            sts.Add(s);
                    }
                    if (sts.Count > 1)
                    {
                        Version = sts[0];
                        VersionLink = sts[1];
                    }
                }
            }
            catch (WebException wex)
            {
                ErrorString = wex.Message;
            }
        }
    }
}
