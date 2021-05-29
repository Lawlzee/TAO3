using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Excel;
using TAO3.Internal.CodeGeneration;
using Xamasoft.JsonClassGenerator;

namespace TAO3.Internal.Commands.WrapExcel.Generation
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
                .Select(x => new FieldInfo(x.Name, GetType(x.ListDataFormat)))
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

            JsonType GetType(ListDataFormat format)
            {
                //todo implement
                if (format.Type == XlListDataType.xlListDataTypeChoice)
                {
                    //return new JsonType(JsonTypeEnum.Boolean);
                }

                //todo implement
                if (format.Type == XlListDataType.xlListDataTypeChoiceMulti)
                {
                    //return new JsonType(JsonTypeEnum.Boolean);
                }

                //todo implement
                if (format.Type == XlListDataType.xlListDataTypeListLookup)
                {
                    //return new JsonType(JsonTypeEnum.Boolean);
                }

                JsonTypeEnum type = format.Type switch
                {
                    XlListDataType.xlListDataTypeCheckbox => JsonTypeEnum.Boolean,
                    XlListDataType.xlListDataTypeCounter => JsonTypeEnum.Integer,
                    XlListDataType.xlListDataTypeCurrency => JsonTypeEnum.Float,
                    XlListDataType.xlListDataTypeDateTime => JsonTypeEnum.Date,
                    XlListDataType.xlListDataTypeHyperLink => JsonTypeEnum.String,
                    XlListDataType.xlListDataTypeMultiLineRichText => JsonTypeEnum.String,
                    XlListDataType.xlListDataTypeMultiLineText => JsonTypeEnum.String,
                    XlListDataType.xlListDataTypeNone => JsonTypeEnum.Object,
                    XlListDataType.xlListDataTypeNumber => JsonTypeEnum.Float,
                    XlListDataType.xlListDataTypeText => JsonTypeEnum.String,
                    _ => JsonTypeEnum.Object
                };

                if (!format.Required)
                {
                    type |= JsonTypeEnum.Nullable;
                }

                return new JsonType(type);
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
}}";
            await cSharpKernel.SubmitCodeAsync(code);

            return className;
        }
    }
}
