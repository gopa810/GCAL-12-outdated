using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GCAL.Base
{
    public class GPObjectXmlListBase: GPObjectListBase
    {
        public override void ReadStream(System.IO.TextReader reader, FileKey key)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            AcceptXml(doc);            
        }

        public virtual void AcceptXml(XmlDocument doc)
        {
        }
    }
}
