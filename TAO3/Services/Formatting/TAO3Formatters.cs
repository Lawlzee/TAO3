namespace TAO3.Formatting;

public record TAO3Formatters(
    CSharpFormatter CSharp,
    JsonFormatter Json,
    SqlFormatter Sql,
    XmlFormatter Xml) : IDisposable
{
    public void Dispose()
    {
    }
}
