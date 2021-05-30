using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExcelRange = Microsoft.Office.Interop.Excel.Range;

namespace TAO3.Excel
{
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

        public ExcelWorksheetCells Cells => new ExcelWorksheetCells(Worksheet);

        internal ExcelWorksheet(Worksheet worksheet)
        {
            Worksheet = worksheet;
        }

        protected ExcelWorksheet(object worksheet)
        {
            Worksheet = (Worksheet)worksheet;
        }

        public object[,] GetUsedRange()
        {
            return Worksheet.UsedRange.GetValues();
        }

        public ExcelTable CreateTable<T>(string name, string cell)
        {
            return CreateTable<T>(name, Worksheet.Range[cell]);
        }

        public ExcelTable CreateTable<T>(string name, int row = 1, int col = 1)
        {
            return CreateTable<T>(name, Worksheet.Cells[row, col]);
        }

        private ExcelTable CreateTable<T>(string name, ExcelRange cell)
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
            newTable.Name = name;

            return new ExcelTable(Worksheet, newTable);
        }
    }
}
