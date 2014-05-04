using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

using GCAL.Base;

namespace GCAL.Engine
{
    public class CELDownloadFile: CELBase
    {
        public string Address = null;
        public string Error = string.Empty;
        public bool Success = false;

        public String TargetPath = null;
        private String p_fileName = null;

        public String TargetFileName
        {
            get
            {
                if (p_fileName == null)
                {
                    p_fileName = string.Empty;
                    string[] segs = Address.Split('/');
                    string lastSegment = null;
                    if (segs.Length > 0)
                    {
                        lastSegment = segs[segs.Length - 1];
                        int i = lastSegment.LastIndexOf('.');
                        if (i > 0)
                        {
                            string filePrefix = lastSegment.Substring(0, i);
                            string postfix = lastSegment.Substring(i);
                            p_fileName = GPFileHelper.UniqueFile(TargetPath, filePrefix, postfix);
                        }
                    }
                }

                return p_fileName;
            }
        }

        protected override void Execute()
        {
            if (TargetPath == null)
            {
                string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                TargetPath = Path.Combine(pathUser, "Downloads");
                if (!Directory.Exists(TargetPath))
                    Directory.CreateDirectory(TargetPath);
            }

            string fileName = TargetFileName;
            if (File.Exists(fileName))
                File.Delete(fileName);


            WebClient wc = new WebClient();
            try
            {
                wc.DownloadFile(Address, fileName);
                Success = true;
            }
            catch (WebException we)
            {
                Error = we.Message;
                Success = false;
            }            

        }
    }
}
