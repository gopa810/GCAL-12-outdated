using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using GCAL.Base;

namespace GCAL.Engine
{
    public class CELSendMyLanguageFile: CELBase
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
            setMessage("msg:Send file " + GPStrings.getSharedStrings().getCustomFilePath() + " to email address: " + "translations@gcal.home.sk");
            try
            {
                WebRequest request = WebRequest.Create("http://gcal.home.sk/languages/mail_update.php?langid=2");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                // Set the ContentLength property of the WebRequest.
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("langid={0}&", GPStrings.getSharedStrings().LanguageId);
                sb.Append("strings=");
                sb.Append(GPStrings.getSharedStrings().ToString());
                //string postData = "langid=2&strings=This is a test that posts this string to a Web server.";
                byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());
                request.ContentLength = byteArray.Length;
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                response = request.GetResponse();

                response.Close();
                response = null;
            }
            catch
            {
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
            setMessage("msg:OK");
            System.Threading.Thread.Sleep(1000);
            setMessage("stop:");
        }
    }
}
