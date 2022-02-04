using Microsoft.DotNet.Interactive.CSharp;
using TAO3.Internal.Extensions;
using TAO3.CodeGeneration;

namespace TAO3.Excel.Generation;

internal class TypeSafeExcelWorkbookGenerator
{
    private readonly TypeSafeExcelWorksheetGenerator _sheetTypeGenerator;

    public TypeSafeExcelWorkbookGenerator(TypeSafeExcelWorksheetGenerator sheetTypeGenerator)
    {
        _sheetTypeGenerator = sheetTypeGenerator;
    }

    public string Generate(CSharpKernel cSharpKernel, ExcelWorkbook workbook)
    {
        string className = IdentifierUtils.ToCSharpIdentifier(workbook.Name);

        List<string> tables = workbook
            .Worksheets
            .Select(t => _sheetTypeGenerator.Generate(cSharpKernel, t))
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
