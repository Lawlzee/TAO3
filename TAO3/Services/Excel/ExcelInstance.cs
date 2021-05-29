using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Worksheet _worksheet;
        internal ListObject ListObject { get; }
        public dynamic Instance => ListObject;

        public string Name => ListObject.Name;

        internal ExcelTable(Worksheet worksheet, ListObject listObject)
        {
            _worksheet = worksheet;
            ListObject = listObject;
        }

        protected ExcelTable(object worksheet, object listObject)
        {
            _worksheet = (Worksheet)worksheet;
            ListObject = (ListObject)listObject;
        }

        public object[,] GetRawData()
        {
            int width = ListObject.ListColumns.Count;
            int height = ListObject.ListRows.Count;

            if (height == 0)
            {
                return new object[height, 0];
            }

            ListRow firstRow = ListObject.ListRows[1];
            int col = firstRow.Range.Column;
            int row = firstRow.Range.Row;

            Microsoft.Office.Interop.Excel.Range range = _worksheet.Range[_worksheet.Cells[row, col], _worksheet.Cells[row + height - 1, col + width - 1]];
            object[,] values = (object[,])range.Value2;

            return values;
        }

        public List<T> Get<T>()
        {
            throw new NotImplementedException();
        }
    }
}
