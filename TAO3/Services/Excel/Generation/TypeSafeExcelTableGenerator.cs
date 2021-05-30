using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TAO3.Internal.CodeGeneration;
using Xamasoft.JsonClassGenerator;

namespace TAO3.Excel.Generation
{
    internal static class TypeSafeExcelTableGenerator
    {
        public static async Task<string> GenerateAsync(CSharpKernel cSharpKernel, ExcelTable table)
        {
            string rowTypeName = await GenerateRowTypeAsync(cSharpKernel, table);
            string tableTypeName = await GenerateTableTypeAsync(cSharpKernel, table, rowTypeName);
            return tableTypeName;
        }

        private static async Task<string> GenerateRowTypeAsync(CSharpKernel cSharpKernel, ExcelTable table)
        {
            List<FieldInfo> fields = table.ListObject.ListColumns
                .Cast<ListColumn>()
                .Select(x => new FieldInfo(x.Name, GetType(x)))
                .ToList();

            JsonType objectType = new JsonType(
                JsonTypeEnum.Object,
                internalType: null,
                assignedName: IdentifierUtils.ToPascalCase(table.Name + "Row"),
                fields,
                isRoot: true);

            string code = JsonClassGenerator.WriteClasses(new List<JsonType> { objectType });

            await cSharpKernel.SubmitCodeAsync(code);

            return objectType.AssignedName;

            JsonType GetType(ListColumn listColumn)
            {
                ListDataFormat format = listColumn.ListDataFormat;
                Microsoft.Office.Interop.Excel.Range bodyRange = listColumn.DataBodyRange;

                string? numberFormat = bodyRange.NumberFormat as string;

                if (numberFormat == null)
                {
                    return new JsonType(JsonTypeEnum.NullableString);
                }

                const string textFormat = "@";
                if (numberFormat == textFormat)
                {
                    return new JsonType(JsonTypeEnum.NullableString);
                }

                object[] values = listColumn.DataBodyRange.GetValues()
                    .Cast<object>()
                    .ToArray();

                bool isNullable = values.Any(x => x == null);

                //We can probably do better than this
                const string dateTimeRegex = @"(^|;)([ymdhms :_\(\),\/\\\-\.]|AM\/PM|am\/pm|A\/P|a\/p|\[.*\])+($|;)";
                if (Regex.IsMatch(numberFormat, dateTimeRegex))
                {
                    bool isTypeNullable = isNullable || values.Any(x => !(x is double));
                    bool isTimeSpan = values
                        .Where(x => x != null)
                        .Cast<double>()
                        .All(x => 0 <= x && x < 1);

                    return new JsonType(
                        (isTimeSpan ? JsonTypeEnum.TimeSpan : JsonTypeEnum.Date)
                        | (isTypeNullable ? JsonTypeEnum.Nullable : 0));
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
                            .Where(x => x != null)
                            .Cast<double>()
                            //https://stackoverflow.com/questions/2751593/how-to-determine-if-a-decimal-double-is-an-integer
                            .All(x => Math.Abs(x % 1) <= (double.Epsilon * 100));

                        return new JsonType(
                            (isInteger ? JsonTypeEnum.Integer : JsonTypeEnum.Float)
                            | (isNullable ? JsonTypeEnum.Nullable : 0));
                    }
                }
                
                return new JsonType(JsonTypeEnum.String | (isNullable ? JsonTypeEnum.Nullable : 0));
            }
        }

        private static async Task<string> GenerateTableTypeAsync(CSharpKernel cSharpKernel, ExcelTable table, string rowTypeName)
        {
            string className = IdentifierUtils.ToPascalCase(table.Name);

            string code = $@"using System;
using System.Collections.Generic;
using System.Linq;
using TAO3.Excel;

public class {className} : ExcelTable
{{
    internal {className}(object worksheet, object listObject)
        : base(worksheet, listObject)
    {{

    }}

    public List<{rowTypeName}> Get() => Get<{rowTypeName}>();

    public void Set(IEnumerable<{rowTypeName}> data) => Set<{rowTypeName}>(data);
}}";
            await cSharpKernel.SubmitCodeAsync(code);

            return className;
        }
    }
}
