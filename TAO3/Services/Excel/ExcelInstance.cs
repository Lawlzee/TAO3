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
using TAO3.Excel.Extensions;
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
            .Select(x => new ExcelTable(x))
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
        internal ListObject ListObject { get; }
        public dynamic Instance => ListObject;

        public string Name => ListObject.Name;

        internal ExcelTable(ListObject listObject)
        {
            ListObject = listObject;
        }

        protected ExcelTable(object listObject)
        {
            ListObject = (ListObject)listObject;
        }

        public object[,] GetRawData()
        {
            return ListObject.Range.GetValues();
        }

        public void SetRawData(object[,] data)
        {
            ListObject.Range.Value2 = data;
        }

        //to do: optimise
        public List<T> Get<T>()
            where T : new()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            ListColumn[] columns = ListObject.ListColumns.Cast<ListColumn>().ToArray();

            object[,] data = GetRawData();
            List<T> result = new List<T>();

            for (int row = 2; row <= data.GetLength(0); row++)
            {
                T rowObject = new T();
                for (int col = 1; col <= data.GetLength(1); col++)
                {
                    PropertyInfo? property = GetProperty(col);
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

            PropertyInfo? GetProperty(int col)
            {
                string columnName = columns[col - 1].Name;
                return properties
                    .Where(x => x.Name == columnName
                        || x.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName == columnName
                        || x.GetCustomAttribute<IndexAttribute>()?.Index == col - 1)
                    .FirstOrDefault();
            }
        }
    }
}
