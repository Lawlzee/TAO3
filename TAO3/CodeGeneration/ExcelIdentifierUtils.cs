using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.CodeGeneration
{
    internal static class ExcelIdentifierUtils
    {
        internal static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;

            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }
    }
}
