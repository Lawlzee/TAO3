using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;
using TAO3.TypeProvider;

namespace TAO3.Excel.Generation
{
    internal class ExcelTypeSafeGenerator
    {
        private readonly CSharpKernel _cSharpKernel;
        private readonly IExcelService _excelService;
        private readonly TypeSafeExcelWorkbookGenerator _typeSafeGenerator;

        public bool RefreshEnable { get; private set; }

        public ExcelTypeSafeGenerator(
            CSharpKernel cSharpKernel, 
            IExcelService excelService, 
            ITypeProvider<ExcelTable> excelTypeProvider)
        {
            _cSharpKernel = cSharpKernel;
            _excelService = excelService;
            _typeSafeGenerator = new TypeSafeExcelWorkbookGenerator(
                new TypeSafeExcelWorksheetGenerator(
                    new TypeSafeExcelTableGenerator(excelTypeProvider)));
            RefreshEnable = true;
        }

        public void DisableRefreshGeneration(Action action)
        {
            ScheduleRefreshGenerationAfter(refresh: false, action);
        }

        public T DisableRefreshGeneration<T>(Func<T> func)
        {
            return ScheduleRefreshGenerationAfter(refresh: false, func);
        }

        public void ScheduleRefreshGenerationAfter(Action action)
        {
            ScheduleRefreshGenerationAfter(refresh: true, action);
        }

        public void ScheduleRefreshGenerationAfter(bool refresh, Action action)
        {
            ScheduleRefreshGenerationAfter(refresh, () => { action(); return true; });
        }

        public T ScheduleRefreshGenerationAfter<T>(Func<T> func)
        {
            return ScheduleRefreshGenerationAfter(refresh: true, func);
        }

        public T ScheduleRefreshGenerationAfter<T>(bool refresh, Func<T> func)
        {
            try
            {
                RefreshEnable = false;
                T result = func();
                RefreshEnable = true;

                if (refresh)
                {
                    ScheduleRefreshGeneration();
                }

                return result;
            }
            finally
            {
                RefreshEnable = true;
            }
        }

        public void ScheduleRefreshGeneration()
        {
            if (!RefreshEnable)
            {
                return;
            }

            List<string> workbooks = _excelService.Workbooks
                .Select(w => _typeSafeGenerator.Generate(_cSharpKernel, w))
                .Select((name, index) => $@"
public static {name} {name}(this IExcelService excelService)
{{
    return new {name}(excelService.Workbooks[{index}]);
}}")
                .ToList();

            string getWorkbooksCode = string.Concat(workbooks);

            string code = $@"using TAO3.Excel;

{getWorkbooksCode}";

            _cSharpKernel.ScheduleSubmitCode(code);
        }
    }
}
