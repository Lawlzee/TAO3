using ExcelRange = Microsoft.Office.Interop.Excel.Range;

namespace TAO3.Excel;

public class ExcelWorksheetCell
{
    private readonly ExcelRange _cell;
    public dynamic Instance => _cell;

    public ExcelWorksheetCell(ExcelRange cell)
    {
        _cell = cell;
    }

    public dynamic GetRaw()
    {
        return _cell.Value2;
    }

    /*public dynamic Get()
    {
        Type inferedType = ExcelFormatHelper.GetCellType(_cell).ToClrType();
        return ExcelFormatHelper.ParseCellValue(_cell.Value2, inferedType);
    }*/

    public string? GetString()
    {
        object? value = _cell;
        return value?.ToString();
    }

    public double GetDouble()
    {
        object? value = _cell;
        return (value as double?) ?? 0;
    }

    public double? GetNullableDouble()
    {
        object? value = _cell;
        return value as double?;
    }

    public int GetInt()
    {
        object? value = _cell;
        return (value as int?) ?? 0;
    }

    public int? GetNullableInt()
    {
        object? value = _cell;
        return value as int?;
    }

    public DateTime GetDateTime()
    {
        object? value = _cell;
        if (value is double doubleValue)
        {
            DateTime.FromOADate(doubleValue);
        }
        return default(DateTime);
    }

    public DateTime? GetNullableDateTime()
    {
        object? value = _cell;
        if (value is double doubleValue)
        {
            DateTime.FromOADate(doubleValue);
        }
        return null;
    }

    public TimeSpan GetTimeSpan()
    {
        object? value = _cell;
        if (value is double doubleValue)
        {
            TimeSpan.FromDays(doubleValue);
        }
        return TimeSpan.Zero;
    }

    public TimeSpan? GetNullableTimeSpan()
    {
        object? value = _cell;
        if (value is double doubleValue)
        {
            TimeSpan.FromDays(doubleValue);
        }
        return null;
    }

    public ExcelWorksheetCell Set(object value, bool formatCell = true)
    {
        if (formatCell)
        {
            _cell.NumberFormat = ExcelFormatHelper.GetFormat(value?.GetType());
        }
        _cell.Value2 = value?.ToString();
        return this;
    }
}
