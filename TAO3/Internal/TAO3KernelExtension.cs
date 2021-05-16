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
using TAO3.Services;
using TAO3.Services.Avalonia;
using TAO3.Services.Clipboard;
using TAO3.Services.Keyboard;
using TAO3.Services.Notepad;
using TAO3.Services.Toast;
using WindowsHook;

namespace TAO3.Internal
{
    public class TAO3KernelExtension : IKernelExtension
    {
        public Task OnLoadAsync(Kernel kernel)
        {
            Debugger.Launch();

            IExcelService excel = null!;
            //new ExcelService().GetOrOpenExcel();

            INotepadService notepad = new NotepadService();

            WindowsInterop interop = WindowsInterop.Create();
            IKeyboardService keyboard = interop.Keyboard;
            IClipboardService clipboard = interop.Clipboard;

            IToastService toast = new ToastService();
            IFormatConverterService formatConverter = new FormatConverterService();
            IInputSourceService inputSource = new InputSourceService();

            Prelude.TAO3Services = new TAO3Services(
                excel,
                notepad,
                keyboard,
                clipboard,
                toast,
                formatConverter,
                inputSource);

            Prelude.Kernel = kernel;

            kernel.AddDirective(new MacroCommand(keyboard, toast));
            kernel.AddDirective(new InputCommand(formatConverter, inputSource));
            kernel.AddDirective(new CopyResultCommand(clipboard, formatConverter));
            //kernel.AddDirective(new ConverterCommand(formatConverter));

            formatConverter.Register(new CsvConverter(true));
            formatConverter.Register(new CsvConverter(false));
            formatConverter.Register(new JsonConverter());
            formatConverter.Register(new XmlConverter());
            formatConverter.Register(new LineConverter());
            formatConverter.Register(new TextConveter());

            inputSource.Register(new ClipboardInputSource(clipboard));
            inputSource.Register(new FileInputSource());
            inputSource.Register(new UriInputSource());
            inputSource.Register(new CellInputSource());

            return Task.CompletedTask;
        }
    }
}
