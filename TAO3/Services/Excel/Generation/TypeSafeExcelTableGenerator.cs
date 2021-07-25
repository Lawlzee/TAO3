using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TAO3.CodeGeneration;
using TAO3.Internal.Extensions;
using Xamasoft.JsonClassGenerator;

namespace TAO3.Excel.Generation
{
    internal static class TypeSafeExcelTableGenerator
    {
        public static string Generate(CSharpKernel cSharpKernel, ExcelTable table)
        {
            string rowTypeName = GenerateRowType(cSharpKernel, table);
            string tableTypeName = GenerateTableType(cSharpKernel, table, rowTypeName);
            return tableTypeName;
        }

        private static string GenerateRowType(CSharpKernel cSharpKernel, ExcelTable table)
        {
            List<FieldInfo> fields = table.ListObject.ListColumns
                .Cast<ListColumn>()
                .Select(x => new FieldInfo(x.Name, new JsonType(ExcelFormatHelper.GetCellType(x.DataBodyRange))))
                .ToList();

            JsonType objectType = new JsonType(
                JsonTypeEnum.Object,
                internalType: null,
                assignedName: IdentifierUtils.ToCSharpIdentifier(table.Name + "Row"),
                fields,
                isRoot: true);

            string code = JsonClassGenerator.WriteClasses(new List<JsonType> { objectType });

            cSharpKernel.ScheduleSubmitCode(code);

            return objectType.AssignedName;
        }

        private static string GenerateTableType(CSharpKernel cSharpKernel, ExcelTable table, string rowTypeName)
        {
            string className = IdentifierUtils.ToCSharpIdentifier(table.Name);

            string code = $@"using System;
using System.Collections.Generic;
using System.Linq;
using TAO3.Excel;

public class {className} : ExcelTable
{{
    internal {className}(ExcelTable table)
        : base(table)
    {{

    }}

    public List<{rowTypeName}> Get() => Get<{rowTypeName}>();

    public void Set(IEnumerable<{rowTypeName}> data) => Set<{rowTypeName}>(data);
}}";
            cSharpKernel.ScheduleSubmitCode(code);

            return className;
        }
    }
}
