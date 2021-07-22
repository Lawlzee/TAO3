using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public record TAO3Converters(
        CSharpConverter CSharp,
        CsvConverter Csv,
        CsvConverter Csvh,
        HtmlConverter Html,
        JsonConverter Json,
        LineConverter Line,
        TextConverter Text,
        XmlConverter Xml,
        SqlConverter Sql)
        : IDisposable
    {
        public void Dispose()
        {

        }
    }
}
