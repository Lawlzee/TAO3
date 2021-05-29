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
using TAO3.Internal.Commands.WrapExcel.Generation;

namespace TAO3.Internal.Commands.WrapExcel
{
    internal class WrapExcelCommand : Command
    {
        public WrapExcelCommand(IExcelService excel)
            : base("#!wrapExcel", "Provide a type safe wrapper around IExcelService")
        {
            Add(new Argument<string>("variableName", "The name of the variable containing that the wrapper will be asigned to"));

            Handler = CommandHandler.Create(async (string variableName, KernelInvocationContext context) =>
            {
                CSharpKernel cSharpKernel = (CSharpKernel)context.HandlingKernel.FindKernel("csharp");
                await ExcelTypeSafeGenerator.RefreshGenerationAsync(cSharpKernel, excel, variableName);
            });
        }
    }
}
