using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL
{
    public class HtmlBuildInstructions
    {
        private StringBuilder sb = new StringBuilder();

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
                if (cmd.Equals("h1"))
                {
                    stopScript();
                    stopTable();
                    sb.AppendFormat("<h1>{0}</h1>", cmd.getArg(0));
                }
                else if (cmd.Command.Equals("choice"))
                {
                    if (!bTableStarted)
                    {
                        sb.AppendLine("<table width=100%>");
                        bTableStarted = true;
                    }
                    sb.AppendLine("<tr height=32px><td style='cursor:pointer;border:1px solid black'");
                    sb.AppendFormat(" onmouseover='shh(1,\"t{0}\")' ", cid);
                    sb.AppendFormat(" onmouseout='shh(0,\"t{0}\")'", cid);
                    sb.AppendFormat(" onclick='runAction(\"{0}\")' id='t{1}'>", cmd.getArg(1), cid);
                    sb.AppendLine(cmd.getArg(0));
                    sb.AppendLine("</td></tr>");
//                    sb.AppendFormat("writeChoiceWithAction(\"{0}\", \"t{1}\", 'runAction({2})')", cmd.getArg(0), cid, cmd.getArg(2));
//                    sb.AppendLine(";");
                    cid++;
                }
            }

            stopScript();
            stopTable();

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");


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
