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
using TAO3.InputSources;
using TAO3.Internal.Commands.Converter;
using TAO3.Internal.Commands.CopyResult;
using TAO3.Internal.Commands.Input;
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

            new ExcelService().GetOrOpenExcel();

            IInteropOS interop = InteropFactory.Create();
            IFormatConverterService formatConverterService = new FormatConverterService();
            IInputSourceService inputSourceService = new InputSourceService();


            kernel.AddDirective(new MacroCommand(interop));
            kernel.AddDirective(new InputCommand(interop, formatConverterService, inputSourceService));
            kernel.AddDirective(new CopyResultCommand(interop, formatConverterService));
            kernel.AddDirective(new ConverterCommand(formatConverterService));

            formatConverterService.Register(new CsvConverter(true));
            formatConverterService.Register(new CsvConverter(false));
            formatConverterService.Register(new JsonConverter());
            formatConverterService.Register(new XmlConverter());
            formatConverterService.Register(new LineConverter());
            formatConverterService.Register(new TextConveter());

            inputSourceService.Register(new ClipboardInputSource(interop.Clipboard));
            inputSourceService.Register(new FileInputSource());
            inputSourceService.Register(new UriInputSource());
            inputSourceService.Register(new CellInputSource());

            return Task.CompletedTask;
        }
    }
}
