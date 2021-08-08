using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TAO3.Formatting
{
    public class XmlFormatter : IFormatter
    {
        public string Format(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            return doc.ToString();
        }
    }
}
