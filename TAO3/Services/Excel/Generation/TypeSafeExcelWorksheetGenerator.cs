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
    internal class TypeSafeExcelWorksheetGenerator
    {
        private readonly TypeSafeExcelTableGenerator _tableGenerator;

        public TypeSafeExcelWorksheetGenerator(TypeSafeExcelTableGenerator tableGenerator)
        {
            _tableGenerator = tableGenerator;
        }

        public string Generate(CSharpKernel cSharpKernel, ExcelWorksheet sheet)
        {
            string className = IdentifierUtils.ToCSharpIdentifier(sheet.Name);

            List<string> tables = sheet
                .Tables
                .Select(t => _tableGenerator.Generate(cSharpKernel, t))
                .Select((name, index) => $"public {name} {name} => new {name}(Tables[{index}]);")
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

    internal {className}(ExcelWorksheet worksheet)
        : base(worksheet)
    {{
    }}
}}";

            cSharpKernel.ScheduleSubmitCode(code);

            return className;
        }
    }
}
