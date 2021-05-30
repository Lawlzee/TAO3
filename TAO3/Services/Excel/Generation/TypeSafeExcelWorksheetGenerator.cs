using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.CodeGeneration;

namespace TAO3.Excel.Generation
{
    internal static class TypeSafeExcelWorksheetGenerator
    {
        public static async Task<string> GenerateAsync(CSharpKernel cSharpKernel, ExcelWorksheet sheet)
        {
            string className = IdentifierUtils.ToPascalCase(sheet.Name);

            List<string> tables = sheet
                .Tables
                .Select(t => TypeSafeExcelTableGenerator.GenerateAsync(cSharpKernel, t))
                .Select(x => x.Result)
                .Select((name, index) => $"public {name} {name} => new {name}(Tables[{index}].Instance);")
                .ToList();

            string getTablesCode = string.Join(@"
        ", tables);

            string code = $@"using System;
using System.Collections.Generic;
using System.Linq;
using TAO3.Excel;

public class {className} : ExcelWorksheet
{{
    {getTablesCode}

    internal {className}(object worksheet)
        : base(worksheet)
    {{
    }}
}}";

            await cSharpKernel.SubmitCodeAsync(code);

            return className;
        }
    }
}
