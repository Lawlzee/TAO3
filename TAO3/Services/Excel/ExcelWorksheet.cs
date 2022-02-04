using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System.Reflection;
using TAO3.Excel.Generation;
using ExcelRange = Microsoft.Office.Interop.Excel.Range;

namespace TAO3.Excel;

public class ExcelWorksheet
{
    internal ExcelTypeSafeGenerator TypeGenerator { get; }
    internal Worksheet Worksheet { get; }
    public dynamic Instance => Worksheet;

    public string Name
    {
        get => Worksheet.Name;
        set => TypeGenerator.ScheduleRefreshGenerationAfter(() => Worksheet.Name = value);
    }

    public IReadOnlyList<ExcelTable> Tables => Worksheet
        .ListObjects
        .Cast<ListObject>()
        .Select(x => new ExcelTable(TypeGenerator, Worksheet, x))
        .ToList();

    public ExcelWorksheetCells Cells => new ExcelWorksheetCells(Worksheet);

    internal ExcelWorksheet(ExcelTypeSafeGenerator typeGenerator, Worksheet worksheet)
    {
        TypeGenerator = typeGenerator;
        Worksheet = worksheet;
    }

    protected ExcelWorksheet(ExcelWorksheet worksheet)
    {
        TypeGenerator = worksheet.TypeGenerator;
        Worksheet = worksheet.Worksheet;
    }

    public void Activate()
    {
        TypeGenerator.DisableRefreshGeneration(() => Worksheet.Activate());
    }

    public void Delete(bool refreshTypes = true)
    {
        TypeGenerator.ScheduleRefreshGenerationAfter(refreshTypes, () => Worksheet.Delete());
    }

    public object[,] GetUsedRange()
    {
        return Worksheet.UsedRange.GetValues();
    }

    public ExcelTable CreateTable<T>(IEnumerable<T> data, string position,  string? name = null, bool refreshTypes = true)
    {
        return CreateTable(data, Worksheet.Range[position], name, refreshTypes);
    }

    public ExcelTable CreateTable<T>(IEnumerable<T> data, int row = 1, int col = 1, string? name = null, bool refreshTypes = true)
    {
        return CreateTable(data, Worksheet.Cells[row, col], name, refreshTypes);
    }

    private ExcelTable CreateTable<T>(IEnumerable<T> data, ExcelRange cell, string? name, bool refreshTypes)
    {
        return TypeGenerator.ScheduleRefreshGenerationAfter(refreshTypes, () =>
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            ExcelRange headersRange = Worksheet.Range[cell, Worksheet.Cells[cell.Row, cell.Column + properties.Length - 1]];
            headersRange.NumberFormat = "@";

            ExcelRange range = Worksheet.Range[cell, Worksheet.Cells[cell.Row + 1, cell.Column + properties.Length - 1]];

            object[,] tableCells = BaseOneArray.Create(2, properties.Length);

            for (int i = 1; i <= properties.Length; i++)
            {
                PropertyInfo property = properties[i - 1];
                string columnName = property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName
                    ?? property.Name;

                tableCells[1, i] = columnName;
                ExcelRange bodyCell = Worksheet.Cells[cell.Row + 1, cell.Column + i - 1];

                bodyCell.NumberFormat = ExcelFormatHelper.GetFormat(property.PropertyType);
            }

            range.Value2 = tableCells;

            ListObject newTable = Worksheet.ListObjects.Add(XlListObjectSourceType.xlSrcRange, range, XlListObjectHasHeaders: XlYesNoGuess.xlYes);

            if (name != null)
            {
                newTable.Name = name;
            }

            ExcelTable excelTable = new ExcelTable(TypeGenerator, Worksheet, newTable);

            excelTable.Set(data);

            return excelTable;
        });
    }
}
