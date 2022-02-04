using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using TAO3.TypeProvider;
using ExcelRange = Microsoft.Office.Interop.Excel.Range;

namespace TAO3.Excel.Generation;

internal class ExcelDomParser : IDomParser<ExcelTable>
{
    //todo: clean
    public IDomType Parse(ExcelTable table)
    {
        List<Field> fields = table.ListObject.ListColumns
            .Cast<ListColumn>()
            .Select(FormatField)
            .ToList();

        DomClass classA = new DomClass(
            table.Name + "Row",
            fields
                .Select(x => new DomClassProperty(
                    x.Name,
                    new DomLiteral(x.Type)))
                .ToList());

        DomClass classB = new DomClass(
            table.Name + "Row",
            fields
                .Select(x => new DomClassProperty(
                    x.Name,
                    x.IsNullable 
                        ? new DomNullLiteral()
                        : new DomLiteral(x.Type)))
                .ToList());


        return new DomCollection(new List<IDomType>
        {
            classA,
            classB
        });
    }

    private record Field(
        string Name,
        Type Type,
        bool IsNullable);

    private Field FormatField(ListColumn listColumn)
    {
        ExcelRange range = listColumn.Range;
        string name = listColumn.Name;

        string? numberFormat = range.NumberFormat as string;

        if (numberFormat == null)
        {
            return new Field(name, typeof(string), true);
        }

        const string textFormat = "@";
        if (numberFormat == textFormat)
        {
            return new Field(name, typeof(string), true);
        }

        object[] values = range.GetValues()
            .Cast<object>()
            .ToArray();

        bool isNullable = values.Any(x => x == null);

        //We can probably do better than this
        const string dateTimeRegex = @"(^|;)([ymdhms :_\(\),\/\\\-\.]|AM\/PM|am\/pm|A\/P|a\/p|\[.*\])+($|;)";
        if (Regex.IsMatch(numberFormat, dateTimeRegex))
        {
            bool isTypeNullable = isNullable || values.Any(x => !(x is double));
            bool isTimeSpan = values
                .OfType<double>()
                .All(x => 0 <= x && x < 1);

            Type type = isTimeSpan ? typeof(TimeSpan) : typeof(DateTime);
            return new Field(name, type, isNullable);
        }

        Type[] columnTypes = values
            .Where(x => x != null)
            .Select(x => x.GetType())
            .Distinct()
            .Take(2)
            .ToArray();

        if (columnTypes.Length == 1)
        {
            Type columnType = columnTypes[0];
            if (columnType == typeof(double))
            {
                bool isInteger = values
                    .OfType<double>()
                    //https://stackoverflow.com/questions/2751593/how-to-determine-if-a-decimal-double-is-an-integer
                    .All(x => Math.Abs(x % 1) <= (double.Epsilon * 100));

                Type type = isInteger ? typeof(int) : typeof(double);
                return new Field(name, type, isNullable);
            }
        }

        return new Field(name, typeof(string), isNullable);
    }
}
