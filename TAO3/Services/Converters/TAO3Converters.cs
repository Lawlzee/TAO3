using TAO3.Converters.CSharp;
using TAO3.Converters.Csv;
using TAO3.Converters.Html;
using TAO3.Converters.Json;
using TAO3.Converters.Line;
using TAO3.Converters.Sql;
using TAO3.Converters.Text;
using TAO3.Converters.Xml;

namespace TAO3.Converters;

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
