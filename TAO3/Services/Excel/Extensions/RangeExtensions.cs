using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Excel.Extensions
{
    internal static class RangeExtensions
    {
        public static object[,] GetValues(this Microsoft.Office.Interop.Excel.Range range)
        {
            if (range.Count == 1)
            {
                object[,] array = CreateBase1Array(1, 1);
                array[1, 1] = range.Value2;
                return array;
            }

            return (object[,])range.Value2;
        }

        public static object[,] CreateBase1Array(int rows, int cols)
        {
            int[] lengths = new int[] { rows, cols };
            int[] lowerBounds = new int[] { 1, 1 };

            return (object[,])Array.CreateInstance(typeof(object), lengths, lowerBounds);

        }
    }
}
