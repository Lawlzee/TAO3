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
                .Select(x => new FieldInfo(x.Name, new JsonType(ExcelFormatHelper.GetCellType(x.DataBodyRange))))
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
