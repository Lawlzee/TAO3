using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public class TAO3Converters : IDisposable
    {
        public CSharpConverter CSharp { get; }
        public CsvConverter Csv { get; }
        public CsvConverter Csvh { get; }
        public HtmlConverter Html { get; }
        public JsonConverter Json { get; }
        public LineConverter Line { get; }
        public TextConverter Text { get; }
        public XmlConverter Xml { get; }

        public TAO3Converters(
            CSharpConverter cSharp,
            CsvConverter csv,
            CsvConverter csvh,
            HtmlConverter html,
            JsonConverter json,
            LineConverter line,
            TextConverter text,
            XmlConverter xml)
        {
            CSharp = cSharp;
            Csv = csv;
            Csvh = csvh;
            Html = html;
            Json = json;
            Line = line;
            Text = text;
            Xml = xml;
        }

        public void Dispose()
        {

        }
    }
}
