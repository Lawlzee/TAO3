using CsvHelper.Configuration.Attributes;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.CodeGeneration;
using Xamasoft.JsonClassGenerator;

namespace TAO3.Excel
{
    public class ExcelWorkbook
    {
        internal Workbook Workbook { get; }
        public dynamic Instance => Workbook;

        public string Name => Workbook.Name;
        public string FullName => Workbook.FullName;
        public IReadOnlyList<ExcelWorksheet> Worksheets => Workbook
            .Sheets
            .Cast<Worksheet>()
            .Select(x => new ExcelWorksheet(x))
            .ToList();

        internal ExcelWorkbook(Workbook workbook)
        {
            Workbook = workbook;
        }

        protected ExcelWorkbook(object workbook)
        {
            Workbook = (Workbook)workbook;
        }
    }

    public class ExcelWorksheet
    {
        internal Worksheet Worksheet { get; }
        public dynamic Instance => Worksheet;

        public string Name => Worksheet.Name;
        public IReadOnlyList<ExcelTable> Tables => Worksheet
            .ListObjects
            .Cast<ListObject>()
            .Select(x => new ExcelTable(Worksheet, x))
            .ToList();

        internal ExcelWorksheet(Worksheet worksheet)
        {
            Worksheet = worksheet;
        }

        protected ExcelWorksheet(object worksheet)
        {
            Worksheet = (Worksheet)worksheet;
        }
    }

    public class ExcelTable
    {
        internal Worksheet Worksheet { get; }
        internal ListObject ListObject { get; }
        public dynamic Instance => ListObject;

        public string Name => ListObject.Name;

        internal ExcelTable(Worksheet worksheet, ListObject listObject)
        {
            Worksheet = worksheet;
            ListObject = listObject;
        }

        protected ExcelTable(object worksheet, object listObject)
        {
            Worksheet = (Worksheet)worksheet;
            ListObject = (ListObject)listObject;
        }

        public object[,] GetRawData()
        {
            return ListObject.DataBodyRange.GetValues();
        }

        public void SetRawData(object[,] data)
        {
            ListObject.DataBodyRange.Value2 = data;
        }

        //to do: optimise
        public List<T> Get<T>()
            where T : new()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            ListColumn[] columns = ListObject.ListColumns.Cast<ListColumn>().ToArray();

            object[,] data = GetRawData();
            List<T> result = new List<T>();

            for (int row = 1; row <= data.GetLength(0); row++)
            {
                T rowObject = new T();
                for (int col = 1; col <= data.GetLength(1); col++)
                {
                    PropertyInfo? property = GetProperty(properties, columns[col - 1], col - 1);
                    if (property == null)
                    {
                        continue;
                    }

                    object? value = data[row, col];
                    if (value == null)
                    {
                        continue;
                    }

                    object? castedValue = value switch
                    {
                        double x when property.PropertyType == typeof(DateTime) => DateTime.FromOADate(x),
                        double x when property.PropertyType == typeof(DateTime?) => DateTime.FromOADate(x),
                        _ when property.PropertyType == typeof(DateTime?) => null,
                        double x when property.PropertyType == typeof(TimeSpan) => TimeSpan.FromDays(x),
                        double x when property.PropertyType == typeof(TimeSpan?) => TimeSpan.FromDays(x),
                        _ when property.PropertyType == typeof(TimeSpan?) => null,
                        _ when property.PropertyType == typeof(string) => value?.ToString(),
                        double x when property.PropertyType == typeof(int) => (int)x,
                        double x when property.PropertyType == typeof(int?) => (int?)x,
                        _ => value
                    };

                    property.SetValue(rowObject, castedValue);
                }
                result.Add(rowObject);
            }

            return result;


        }

        //to do: optimise
        public void Set<T>(IEnumerable<T> data)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            ListColumn[] columns = ListObject.ListColumns.Cast<ListColumn>().ToArray();

            List<T> dataList = data.ToList();

            Microsoft.Office.Interop.Excel.Range originalRange = ListObject.Range;

            Microsoft.Office.Interop.Excel.Range newRange = Worksheet.Range[
                Worksheet.Cells[originalRange.Row, originalRange.Column],
                Worksheet.Cells[originalRange.Row + Math.Max(1, dataList.Count), originalRange.Column + columns.Length - 1]];

            int originalRowCountNoHeaders = originalRange.Rows.Count - 1;
            if (dataList.Count > originalRowCountNoHeaders)
            {
                ListObject.Resize(newRange);
            }

            object?[,] newBody = BaseOneArray.Create(Math.Max(originalRowCountNoHeaders, dataList.Count), originalRange.Columns.Count);
            for (int col = 1; col <= columns.Length; col++)
            {
                PropertyInfo? property = GetProperty(properties, columns[col - 1], col - 1);

                if (property == null)
                {
                    continue;
                }

                for (int row = 1; row <= dataList.Count; row++)
                {
                    newBody[row, col] = property.GetValue(dataList[row - 1])?.ToString();
                }
            }

            ListObject.DataBodyRange.Value2 = newBody;

            if (dataList.Count < originalRowCountNoHeaders)
            {
                ListObject.Resize(newRange);
            }
        }

        PropertyInfo? GetProperty(PropertyInfo[] properties, ListColumn column, int col)
        {
            string columnName = column.Name;
            return properties
                .Where(x => x.Name == columnName
                    || x.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName == columnName
                    || x.GetCustomAttribute<IndexAttribute>()?.Index == col)
                .FirstOrDefault();
        }
    }
}
