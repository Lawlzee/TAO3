using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Excel.Generation
{
    internal static class ExcelTypeSafeGenerator
    {
        public static async Task RefreshGenerationAsync(CSharpKernel cSharpKernel, IExcelService excelService)
        {
            List<string> workbooks = excelService.Workbooks
                .Select(w => TypeSafeExcelWorkbookGenerator.GenerateAsync(cSharpKernel, w))
                .Select(x => x.Result)
                .Select((name, index) => $@"
public static {name} {name}(this IExcelService excelService)
{{
    return new {name}(excelService.Workbooks[{index}].Instance);
}}")
                .ToList();

            string getWorkbooksCode = string.Concat(workbooks);

            string code = $@"using TAO3.Excel;

{getWorkbooksCode}";

            await cSharpKernel.SubmitCodeAsync(code);
        }
    }
}
