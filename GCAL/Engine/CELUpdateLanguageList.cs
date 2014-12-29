using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using GCAL.Base;

namespace GCAL.Engine
{
    public class CELUpdateLanguageList: CELBase
    {
        private string message = string.Empty;

        public void setMessage(string s)
        {
            lock (message)
            {
                message = s;
            }
        }

        public string getMessage()
        {
            string a = String.Empty;
            lock (message)
            {
                a = message;
            }
            return a;
        }

        protected override void Execute()
        {
            WebResponse response = null;
            WebRequest request = WebRequest.Create("http://gcal.home.sk/languages/langlist.php");
            ((HttpWebRequest)request).UserAgent = "GCAL Application";
            List<GPLanguage> list = new List<GPLanguage>();
            GPLanguageList shared = GPLanguageList.getShared();

            try
            {
                setMessage("msg:" + GPStrings.getString(1285));
                response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string fetchedList = sr.ReadToEnd();
                sr.Close();
                string[] lines = fetchedList.Split('\n');
                foreach(string s in lines)
                {
                    //Debugger.Log(0, "", "LANG: " + s + "\n");
                    string[] langRec = s.Split('\t');
                    if (langRec.Length >= 3)
                    {
                        GPLanguage lang = new GPLanguage();
                        if (!int.TryParse(langRec[0], out lang.LanguageId))
                        {
                            lang.LanguageId = -1;
                        }
                        lang.LanguageName = langRec[1];
                        if (!int.TryParse(langRec[2], out lang.LanguageVersion))
                        {
                            lang.LanguageVersion = -1;
                        }
                        if (shared.IsNewVersion(lang))
                        {
                            list.Add(lang);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                setMessage("msg:" + e.Message);
                System.Threading.Thread.Sleep(2000);
            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            int i = 1;
            int m = list.Count;
            foreach (GPLanguage language in list)
            {
                try
                {
                    setMessage(string.Format("msg:{0} {1}/{2}",GPStrings.getString(1286), i, m));
                    request = WebRequest.Create("http://gcal.home.sk/languages/exportraw.php?langid=" + language.LanguageId);
                    response = request.GetResponse();
                    Stream s = response.GetResponseStream();
                    StreamReader sr = new StreamReader(s);
                    string file = sr.ReadToEnd();
                    sr.Close();
                    string fileName = string.Format("lang{0}.txt", language.LanguageId);
                    string filePath = Path.Combine(Path.Combine(GPFileHelper.getAppExecutableDirectory(),"languages"), fileName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    File.WriteAllText(filePath, file);
                }
                catch
                {
                }
                finally
                {
                    if (response != null)
                        response.Close();
                }
                i++;
            }

            if (list.Count > 0)
            {
                shared.refreshLanguageList();
            }
            else
            {
                setMessage("msg:" + GPStrings.getString(1287));
            }
            System.Threading.Thread.Sleep(1000);
            setMessage("stop:");
        }
    }
}
