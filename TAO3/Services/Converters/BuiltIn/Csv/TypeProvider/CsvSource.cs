using CsvHelper.Configuration;

namespace TAO3.Converters.Csv;

public class CsvSource
{
    public string RootTypeName { get; }
    public string Csv { get; }
    public CsvConfiguration Configuration { get; }

    public CsvSource(string rootTypeName, string csv, CsvConfiguration configuration)
    {
        RootTypeName = rootTypeName;
        Csv = csv;
        Configuration = configuration;
    }
}
