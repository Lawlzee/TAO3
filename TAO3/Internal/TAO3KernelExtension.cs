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
using TAO3.Converters.CSharp;
using TAO3.Internal.Commands.ConnectMSSQL;
using System.Net.Http;
using TAO3.Translation;
using TAO3.Converters.SQL;

namespace TAO3.Internal
{
    public class TAO3KernelExtension : IKernelExtension
    {
        public async Task OnLoadAsync(Kernel kernel)
        {
            Debugger.Launch();

            CompositeKernel compositeKernel = (CompositeKernel)kernel;

            IExcelService excel = new ExcelService((CSharpKernel)compositeKernel.FindKernel("csharp"));
            
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

            HttpClient httpClient = new HttpClient();
            ITranslationService translationService = new TranslationService(httpClient);

            TAO3Converters converters = new TAO3Converters(
                new CSharpConverter(new CSharpObjectSerializer()),
                new CsvConverter(false),
                new CsvConverter(true),
                new HtmlConverter(),
                new JsonConverter(),
                new LineConverter(),
                new TextConverter(),
                new XmlConverter(),
                new SqlConverter(new SqlObjectSerializer()));

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
                windowsService,
                httpClient,
                translationService,
                converters);

            Prelude.Kernel = compositeKernel;

            compositeKernel.RegisterForDisposal(Prelude.Services);

            compositeKernel.AddDirective(await MacroCommand.CreateAsync(keyboard, toast));
            compositeKernel.AddDirective(new InputCommand(inputSource, formatConverter));
            compositeKernel.AddDirective(new OutputCommand(outputDestination, formatConverter));
            compositeKernel.AddDirective(new CellCommand(cellService));
            compositeKernel.AddDirective(new RunCommand(cellService));
            compositeKernel.AddDirective(new ConnectMSSQLCommand());

            formatConverter.Register(converters.Csv);
            formatConverter.Register(converters.Csvh);
            formatConverter.Register(converters.Json);
            formatConverter.Register(converters.Xml);
            formatConverter.Register(converters.Line);
            formatConverter.Register(converters.Text);
            formatConverter.Register(converters.Html);
            formatConverter.Register(converters.CSharp);

            inputSource.Register(new ClipboardInputSource(clipboard));
            inputSource.Register(new CellInputSource());
            inputSource.Register(new NotepadInputSource(notepad));

            outputDestination.Register(new ClipboardOutputDestination(clipboard));
            outputDestination.Register(new NotepadOutputDestination(notepad));
        }
    }
}
