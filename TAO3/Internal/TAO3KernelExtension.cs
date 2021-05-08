using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters;
using TAO3.Internal.Commands.Converter;
using TAO3.Internal.Commands.CopyResult;
using TAO3.Internal.Commands.GetClipboard;
using TAO3.Internal.Commands.Macro;
using TAO3.Internal.Interop;
using TAO3.Internal.Services;
using WindowsHook;

namespace TAO3.Internal
{
    public class TAO3KernelExtension : IKernelExtension
    {
        public Task OnLoadAsync(Kernel kernel)
        {
            Debugger.Launch();

            IInteropOS interop = InteropFactory.Create();
            IFormatConverterService formatConverterService = new FormatConverterService();

            kernel.AddDirective(new MacroCommand(interop));
            kernel.AddDirective(new GetClipboardCommand(interop, formatConverterService));
            kernel.AddDirective(new CopyResultCommand(interop, formatConverterService));
            kernel.AddDirective(new ConverterCommand(formatConverterService));

            formatConverterService.RegisterConverter(new CsvConverter(true));
            formatConverterService.RegisterConverter(new CsvConverter(false));
            formatConverterService.RegisterConverter(new JsonConverter());
            formatConverterService.RegisterConverter(new XmlConverter());
            formatConverterService.RegisterConverter(new LineConverter());
            formatConverterService.RegisterConverter(new TextConveter());

            return Task.CompletedTask;
        }
    }
}
