using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GCAL
{
    public class HtmlBuildInstructions
    {
        private StringBuilder sb = new StringBuilder();
        private Dictionary<string,string> properties = new Dictionary<string,string>();
        public ContentServer contentServer = null;

        bool bTableStarted = false;
        bool bScriptStarted = false;
        int cid = 1;

        public StringBuilder Builder
        {
            get
            {
                return sb;
            }
        }

        public void Build(List<ContentServer.FlowCommand> pageInstructions)
        {
            sb.Clear();

            sb.AppendLine("<html>");
            sb.AppendLine("<head><title></title>");
            sb.AppendLine("<script type=\"text/javascript\" src=\"{%homedir}/shh.js\"></script>");
            sb.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"{%homedir}/{%current_theme}\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body style='padding-left:32px;padding-right:32px'>");

            foreach (ContentServer.FlowCommand cmd in pageInstructions)
            {
                if (cmd.Command.Equals("h1"))
                {
                    stopScript();
                    stopTable();
                    sb.AppendFormat("<h1>{0}</h1>", cmd.getArg(0));
                }
                else if (cmd.Command.Equals("choice"))
                {
                    if (!bTableStarted)
                    {
                        sb.AppendLine("<table width=100% cellpadding=8 cellspacing=6>");
                        bTableStarted = true;
                    }
                    sb.AppendLine("<tr height=32px><td class='choice'");
                    sb.AppendFormat(" onmouseover='shh(1,\"t{0}\")' ", cid);
                    sb.AppendFormat(" onmouseout='shh(0,\"t{0}\")'", cid);
                    sb.AppendFormat(" onclick='runAction(\"{0}\")' id='t{1}'>", cmd.getArgSubst(1, contentServer.getProperties(), true), cid);
                    sb.AppendLine(cmd.getArg(0));
                    sb.AppendLine("</td></tr>");
                    cid++;
                }
                else if (cmd.Command.Equals("list"))
                {
                    if (cmd.getArg(0).Equals("start"))
                    {
                        sb.AppendFormat("<table style='border:0px' align=center width='66%' cellpadding=4 cellspacing=4>");
                        bTableStarted = true;
                    }
                    else if (cmd.getArg(0).Equals("end"))
                    {
                        stopScript();
                        sb.AppendFormat("</table>\n");
                    }
                }
                else if (cmd.Command.Equals("set"))
                {
                    string a = cmd.getArg(0);
                    string b = cmd.getArg(1);

                    if (a.Length > 0)
                        properties[a] = b;
                }
                else if (cmd.Command.Equals("clickchoice"))
                {
                    string myText = cmd.getArg(0);
                    string myId = cmd.getArg(1);

                    if (!bScriptStarted)
                    {
                        sb.AppendLine("<script>");
                        bScriptStarted = true;
                    }
                    sb.AppendFormat("writeClickChoiceValue(\"");
                    sb.Append(contentServer.evaluate3params("getSelectionText", properties["property"], myId));
                    sb.AppendFormat("\", \"{0}\", \"{1}\", ", myText, myId);
                    sb.Append(contentServer.evaluate3params("isSelectedSetting", properties["property"], myId));
                    sb.Append(", \"");
                    sb.Append(properties["returnpage"]);
                    sb.Append("\", \"");
                    sb.Append(properties["property"]);
                    sb.AppendLine("\");");

                }
            }

            stopScript();
            stopTable();

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            Debugger.Log(0, "", sb.ToString());
        }

        public void stopTable()
        {
            if (bTableStarted)
            {
                sb.AppendLine("</table>");
                bTableStarted = false;
            }
        }

        public void startScript()
        {
            if (!bScriptStarted)
            {
                sb.AppendLine("<script>");
                bScriptStarted = false;
            }
        }

        public void stopScript()
        {
            if (bScriptStarted)
            {
                sb.AppendLine("</script>");
                bScriptStarted = false;
            }
        }

        public string getHtmlText()
        {
            return sb.ToString();
        }
    }
}
