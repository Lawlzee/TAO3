using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Excel
{
    internal static class RangeExtensions
    {
        public static object[,] GetValues(this Microsoft.Office.Interop.Excel.Range range)
        {
            if (range.Count == 1)
            {
                object[,] array = BaseOneArray.Create(1, 1);
                array[1, 1] = range.Value2;
                return array;
            }

            return (object[,])range.Value2;
        }
    }
}
