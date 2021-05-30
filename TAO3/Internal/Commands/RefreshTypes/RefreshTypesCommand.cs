using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Excel;
using TAO3.Excel.Generation;

namespace TAO3.Internal.Commands.RefreshTypes
{
    //Use method on IExcelService instead when this issue is fixed
    //https://github.com/dotnet/interactive/issues/1378
    internal class RefreshTypesCommand : Command
    {
        public RefreshTypesCommand(IExcelService excel)
            : base("#!refreshTypes", "Provide a type safe wrapper around IExcelService")
        {
            Handler = CommandHandler.Create(async (KernelInvocationContext context) =>
            {
                CSharpKernel cSharpKernel = (CSharpKernel)context.HandlingKernel.FindKernel("csharp");
                await ExcelTypeSafeGenerator.RefreshGenerationAsync(cSharpKernel, excel);
            });
        }
    }
}
