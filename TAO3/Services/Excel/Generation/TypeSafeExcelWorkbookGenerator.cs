using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;
using TAO3.CodeGeneration;

namespace TAO3.Excel.Generation
{
    internal static class TypeSafeExcelWorkbookGenerator
    {
        public static string Generate(CSharpKernel cSharpKernel, ExcelWorkbook workbook)
        {
            string className = IdentifierUtils.ToCSharpIdentifier(workbook.Name);

            List<string> tables = workbook
                .Worksheets
                .Select(t => TypeSafeExcelWorksheetGenerator.Generate(cSharpKernel, t))
                .Select((name, index) => $"public {name} {name} => new {name}(Worksheets[{index}]);")
                .ToList();

            string getTablesCode = string.Join(@"
        ", tables);

            string code = $@"using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using TAO3.Excel;

public class {className} : ExcelWorkbook
{{
    {getTablesCode}

    internal {className}(ExcelWorkbook workbook)
        : base(workbook)
    {{
    }}
}}";

            cSharpKernel.ScheduleSubmitCode(code);

            return className;
        }
    }
}
