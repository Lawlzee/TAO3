using CsvHelper;
using System.Globalization;
using System.IO;
using TAO3.Internal.CodeGeneration;
using TAO3.TypeProvider;

namespace TAO3.Converters.Csv;

internal class CsvDomParser : IDomParser<CsvSource>
{
    public IDomType Parse(CsvSource input)
    {
        var settings = input.Configuration;
        bool hasHeader = settings.HasHeaderRecord;
        try
        {
            settings.HasHeaderRecord = false;

            using StringReader reader = new StringReader(input.Csv);
            using CsvReader csvReader = new CsvReader(reader, settings);

            List<DomClass> rows = new List<DomClass>();

            string[]? headers = null;

            while (csvReader.Read())
            {
                string[] row = csvReader.Context.Record;

                if (headers == null)
                {
                    if (hasHeader)
                    {
                        headers = csvReader.Context.Record;
                        continue;
                    }

                    headers = Enumerable.Range(1, row.Length)
                        .Select(ExcelIdentifierUtils.GetExcelColumnName)
                        .ToArray();
                }

                rows.Add(VisitRow(input.RootTypeName, headers, row));
            }

            return new DomCollection(rows.Cast<IDomType>().ToList());
        }
        finally
        {
            settings.HasHeaderRecord = hasHeader;
        }
    }

    private DomClass VisitRow(string className, string[] headers, string[] rowValues)
    {
        return new DomClass(
            className,
            GetProperties().ToList());

        IEnumerable<DomClassProperty> GetProperties()
        {
            int count = Math.Min(headers.Length, rowValues.Length);
            for (int i = 0; i < count; i++)
            {
                yield return new DomClassProperty(
                    headers[i],
                    InferType(rowValues[i]));
            }

            for (int i = count; i < headers.Length; i++)
            {
                yield return new DomClassProperty(
                    headers[i],
                    new DomNullLiteral());
            }

            for (int i = count; i < rowValues.Length; i++)
            {
                yield return new DomClassProperty(
                    ExcelIdentifierUtils.GetExcelColumnName(i + 1),
                    InferType(rowValues[i]));
            }
        }
    }

    private IDomType InferType(string value)
    {
        if (value.Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            return new DomNullLiteral();
        }

        if (value.Equals("", StringComparison.OrdinalIgnoreCase))
        {
            return new DomNullLiteral();
        }

        if (int.TryParse(value, out var _))
        {
            return new DomLiteral(typeof(int));
        }

        if (long.TryParse(value, out var _))
        {
            return new DomLiteral(typeof(long));
        }

        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var _))
        {
            return new DomLiteral(typeof(double));
        }

        if (bool.TryParse(value, out var _))
        {
            return new DomLiteral(typeof(bool));
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _))
        {
            return new DomLiteral(typeof(DateTime));
        }

        if (TimeSpan.TryParse(value, out var _))
        {
            return new DomLiteral(typeof(TimeSpan));
        }

        if (char.TryParse(value, out var _))
        {
            return new DomLiteral(typeof(char));
        }

        return new DomLiteral(typeof(string));
    }
}
