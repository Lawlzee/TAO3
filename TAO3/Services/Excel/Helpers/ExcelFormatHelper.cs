using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamasoft.JsonClassGenerator;
using ExcelRange = Microsoft.Office.Interop.Excel.Range;

namespace TAO3.Excel
{
    internal static class ExcelFormatHelper
    {
        public static string GetFormat(Type? cellType)
        {
            if (cellType == typeof(double) || cellType == typeof(double?))
            {
                return "0.00";
            }
            else if (cellType == typeof(int) || cellType == typeof(int?))
            {
                return "0";
            }
            else if (cellType == typeof(DateTime) || cellType == typeof(DateTime?))
            {
                return "yyyy-mm-dd hh:mm:ss";
            }
            else if (cellType == typeof(TimeSpan) || cellType == typeof(TimeSpan?))
            {
                return "hh:mm:ss";
            }
            else
            {
                return "@";
            }
        }

        public static JsonTypeEnum GetCellType(ExcelRange range)
        {
            string? numberFormat = range.NumberFormat as string;

            if (numberFormat == null)
            {
                return JsonTypeEnum.NullableString;
            }

            const string textFormat = "@";
            if (numberFormat == textFormat)
            {
                return JsonTypeEnum.NullableString;
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

                return (isTimeSpan ? JsonTypeEnum.TimeSpan : JsonTypeEnum.Date)
                    | (isTypeNullable ? JsonTypeEnum.Nullable : 0);
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

                    return (isInteger ? JsonTypeEnum.Integer : JsonTypeEnum.Float)
                        | (isNullable ? JsonTypeEnum.Nullable : 0);
                }
            }

            return JsonTypeEnum.String | (isNullable ? JsonTypeEnum.Nullable : 0);
        }

        public static object? ParseCellValue(object? value, Type type)
        {
            return value switch
            {
                double x when type == typeof(DateTime) => DateTime.FromOADate(x),
                double x when type == typeof(DateTime?) => DateTime.FromOADate(x),
                _ when type == typeof(DateTime?) => null,
                double x when type == typeof(TimeSpan) => TimeSpan.FromDays(x),
                double x when type == typeof(TimeSpan?) => TimeSpan.FromDays(x),
                _ when type == typeof(TimeSpan?) => null,
                _ when type == typeof(string) => value?.ToString(),
                double x when type == typeof(int) => (int)x,
                double x when type == typeof(int?) => (int?)x,
                _ => value
            };
        }
    }
}
