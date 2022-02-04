using System.Xml.Linq;

namespace TAO3.Formatting;

public class XmlFormatter : IFormatter
{
    public string Format(string xml)
    {
        XDocument doc = XDocument.Parse(xml);
        return doc.ToString();
    }
}
