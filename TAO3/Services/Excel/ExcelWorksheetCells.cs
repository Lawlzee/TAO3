using Microsoft.Office.Interop.Excel;

namespace TAO3.Excel;

public class ExcelWorksheetCells
{
    private readonly Worksheet _worksheet;

    public ExcelWorksheetCells(Worksheet worksheet)
    {
        _worksheet = worksheet;
    }

    public ExcelWorksheetCell this[int row, int col] => new ExcelWorksheetCell(_worksheet.Cells[row, col]);
    public ExcelWorksheetCell this[string position] => new ExcelWorksheetCell(_worksheet.Range[position]);
}
