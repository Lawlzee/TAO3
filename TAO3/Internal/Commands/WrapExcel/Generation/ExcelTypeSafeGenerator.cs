using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Excel;
using TAO3.Internal.CodeGeneration;

namespace TAO3.Internal.Commands.WrapExcel.Generation
{
    internal static class ExcelTypeSafeGenerator
    {
        public static async Task RefreshGenerationAsync(CSharpKernel cSharpKernel, IExcelService excelService, string name)
        {
            List<string> workbooks = excelService.Workbooks
                .Select(w => TypeSafeExcelWorkbookGenerator.GenerateAsync(cSharpKernel, w))
                .Select(x => x.Result)
                .Select((name, index) => $@"public {name} {name} => new {name}(_excelService.Workbooks[{index}].Instance);")
                .ToList();

            string className = IdentifierUtils.ToPascalCase(name);

            string getWorkbooksCode = string.Join(@"
    ", workbooks);

            string code = $@"using TAO3.Excel;

public class {className}
{{
    private readonly IExcelService _excelService;

    public {className}(IExcelService excelService)
    {{
        _excelService = excelService;
    }}

    {getWorkbooksCode}
}}

{className} {name} = new {className}(TAO3.Prelude.Excel);
";
            await cSharpKernel.SubmitCodeAsync(code);
        }
    }
}
