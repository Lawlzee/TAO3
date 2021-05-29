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

namespace TAO3.Internal.Commands.WrapExcel.Generation
{
    internal static class TypeSafeExcelWorkbookGenerator
    {
        public static async Task<string> GenerateAsync(CSharpKernel cSharpKernel, ExcelWorkbook workbook)
        {
            string className = IdentifierUtils.ToPascalCase(workbook.Name);

            List<string> tables = workbook
                .Worksheets
                .Select(t => TypeSafeExcelWorksheetGenerator.GenerateAsync(cSharpKernel, t))
                .Select(x => x.Result)
                .Select((name, index) => $"public {name} {name} => new {name}(Worksheets[{index}].Instance);")
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

    internal {className}(object workbook)
        : base(workbook)
    {{
    }}
}}";

            await cSharpKernel.SubmitCodeAsync(code);

            return className;
        }
    }
}
