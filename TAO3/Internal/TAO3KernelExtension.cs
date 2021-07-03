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
using TAO3.Internal.Commands.Output;
using TAO3.Internal.Commands.Input;
using TAO3.Internal.Commands.Macro;
using TAO3.Services;
using TAO3.Clipboard;
using TAO3.Keyboard;
using TAO3.Notepad;
using TAO3.Toast;
using TAO3.OutputDestinations;
using TAO3.Excel;
using Microsoft.DotNet.Interactive.CSharp;
using TAO3.Cell;
using TAO3.Internal.Commands.Cell;
using TAO3.Internal.Commands.Run;
using TAO3.Windows;
using TAO3.Internal.Commands.Sql;

namespace TAO3.Internal
{
    public class TAO3KernelExtension : IKernelExtension
    {
        public async Task OnLoadAsync(Kernel kernel)
        {
            Debugger.Launch();

            IExcelService excel = new ExcelService((CSharpKernel)kernel.FindKernel("csharp"));
            
            try
            {
                excel.RefreshTypes();
            }
            catch
            {
                //Excel is closed
            }

            INotepadService notepad = new NotepadService();

            IWindowsService windowsService = new WindowsService();
            IKeyboardService keyboard = windowsService.Keyboard;
            IClipboardService clipboard = windowsService.Clipboard;

            IToastService toast = new ToastService();
            IFormatConverterService formatConverter = new FormatConverterService();
            
            IInputSourceService inputSource = new InputSourceService();
            IOutputDestinationService outputDestination = new OutputDestinationService();

            ICellService cellService = new CellService();

            Prelude.Services = new TAO3Services(
                excel,
                notepad,
                keyboard,
                clipboard,
                toast,
                formatConverter,
                inputSource,
                outputDestination,
                cellService,
                windowsService);

            Prelude.Kernel = kernel;

            kernel.RegisterForDisposal(Prelude.Services);

            ((CompositeKernel)kernel).UseKernelClientConnection(new TAO3MsSqlKernelConnection());

            kernel.AddDirective(await MacroCommand.CreateAsync(keyboard, toast));
            kernel.AddDirective(new InputCommand(inputSource, formatConverter));
            kernel.AddDirective(new OutputCommand(outputDestination, formatConverter));
            kernel.AddDirective(new CellCommand(cellService));
            kernel.AddDirective(new RunCommand(cellService));

            formatConverter.Register(new CsvConverter(true));
            formatConverter.Register(new CsvConverter(false));
            formatConverter.Register(new JsonConverter());
            formatConverter.Register(new XmlConverter());
            formatConverter.Register(new LineConverter());
            formatConverter.Register(new TextConveter());
            formatConverter.Register(new HtmlConverter());
            formatConverter.Register(new CSharpConverter());

            inputSource.Register(new ClipboardInputSource(clipboard));
            inputSource.Register(new CellInputSource());
            inputSource.Register(new NotepadInputSource(notepad));

            outputDestination.Register(new ClipboardOutputDestination(clipboard));
            outputDestination.Register(new NotepadOutputDestination(notepad));
        }
    }
}
