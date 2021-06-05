using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;

namespace TAO3.Excel.Generation
{
    internal class ExcelTypeSafeGenerator
    {
        private readonly CSharpKernel _cSharpKernel;
        private readonly IExcelService _excelService;

        public ExcelTypeSafeGenerator(CSharpKernel cSharpKernel, IExcelService excelService)
        {
            _cSharpKernel = cSharpKernel;
            _excelService = excelService;
        }

        public void ScheduleRefreshGeneration()
        {
            List<string> workbooks = _excelService.Workbooks
                .Select(w => TypeSafeExcelWorkbookGenerator.Generate(_cSharpKernel, w))
                .Select((name, index) => $@"
public static {name} {name}(this IExcelService excelService)
{{
    return new {name}(excelService.Workbooks[{index}]);
}}")
                .ToList();

            string getWorkbooksCode = string.Concat(workbooks);

            string code = $@"using TAO3.Excel;

{getWorkbooksCode}";

            _cSharpKernel.DeferCommand(new SubmitCode(code));
        }
    }
}
