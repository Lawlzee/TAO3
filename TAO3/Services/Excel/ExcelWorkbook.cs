using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Linq;

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
}
