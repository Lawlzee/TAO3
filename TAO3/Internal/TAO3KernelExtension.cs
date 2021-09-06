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
using TAO3.IO;
using TAO3.Internal.Commands.Output;
using TAO3.Internal.Commands.Input;
using TAO3.Internal.Commands.Macro;
using TAO3.Services;
using TAO3.Clipboard;
using TAO3.Keyboard;
using TAO3.Notepad;
using TAO3.Toast;
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
using TAO3.Converters.Sql;
using TAO3.TypeProvider;
using TAO3.Converters.Json;
using TAO3.Converters.Csv;
using TAO3.Converters.Html;
using TAO3.Converters.Line;
using TAO3.Converters.Text;
using TAO3.Converters.Xml;
using TAO3.Excel.Generation;
using TAO3.Internal.Kernels.Translate;
using TAO3.Formatting;
using TAO3.Internal.Kernels.Razor;
using RazorLight;
using TAO3.VsCode;
using TAO3.Internal.Commands.GenerateHttpClient;
using TAO3.Avalonia;
using TAO3.Macro;
using TAO3.EventHandlers.Macro;

namespace TAO3.Internal
{
    public class TAO3KernelExtension : IKernelExtension
    {
        public async Task OnLoadAsync(Kernel kernel)
        {
            Debugger.Launch();

            CompositeKernel compositeKernel = (CompositeKernel)kernel;

            INotepadService notepad = new NotepadService();

            IWindowsService windowsService = new WindowsService();
            IKeyboardService keyboard = windowsService.Keyboard;
            IClipboardService clipboard = windowsService.Clipboard;

            IToastService toast = new ToastService();
            IConverterService converterService = new ConverterService();

            ISourceService sourceService = new SourceService();
            IDestinationService destinationService = new DestinationService();

            ICellService cellService = new CellService();

            HttpClient httpClient = new HttpClient();
            ITranslationService translationService = new TranslationService(httpClient);

            IDomSchematizer domSchematizer = IDomSchematizer.Default;

            ICSharpSchemaSerializer cSharpSchemaSerializer = new CSharpSchemaSerializer();
            cSharpSchemaSerializer.AddAnnotator(new JsonPropertyAnnotator());
            cSharpSchemaSerializer.AddAnnotator(new CsvIndexAnnotator());
            cSharpSchemaSerializer.AddAnnotator(new CsvColumnNameAnnotator());
            cSharpSchemaSerializer.AddAnnotator(new ValueToListAnnotator(cSharpSchemaSerializer));
            cSharpSchemaSerializer.AddAnnotator(new TableNameAnnotator());

            ITypeProvider<string> sqlTypeProvider = new TypeProvider<string>(
                "sql",
                new SqlDomParser(),
                domSchematizer,
                cSharpSchemaSerializer);

            ITypeProvider<JsonSource> jsonTypeProvider = new TypeProvider<JsonSource>(
                "json",
                new JsonDomParser(),
                domSchematizer,
                cSharpSchemaSerializer);

            ITypeProvider<JsonSource> xmlTypeProvider = new TypeProvider<JsonSource>(
                "xml",
                new JsonDomParser(),
                domSchematizer,
                cSharpSchemaSerializer);

            ITypeProvider<CsvSource> csvTypeProvider = new TypeProvider<CsvSource>(
                "csv",
                new CsvDomParser(),
                domSchematizer,
                cSharpSchemaSerializer);

            ITypeProvider<ExcelTable> excelTypeProvider = new TypeProvider<ExcelTable>(
                "excel",
                new ExcelDomParser(),
                domSchematizer,
                cSharpSchemaSerializer);

            ITypeProviders typeProviders = new TypeProviders(
                cSharpSchemaSerializer,
                sqlTypeProvider,
                jsonTypeProvider,
                csvTypeProvider);

            JsonConverter jsonConverter = new JsonConverter(jsonTypeProvider);

            TAO3Converters builtInConverters = new TAO3Converters(
                new CSharpConverter(new CSharpObjectSerializer()),
                new CsvConverter(csvTypeProvider, false),
                new CsvConverter(csvTypeProvider, true),
                new HtmlConverter(),
                jsonConverter,
                new LineConverter(),
                new TextConverter(),
                new XmlConverter(jsonConverter, xmlTypeProvider),
                new SqlConverter(sqlTypeProvider, new SqlDeserializer(), new SqlObjectSerializer()));

            CSharpKernel cSharpKernel = (CSharpKernel)compositeKernel.FindKernel("csharp");
            IInteractiveHost interactiveHost = cSharpKernel.TryGetVariable("InteractiveHost", out IInteractiveHost host)
                ? host
                : throw new Exception("Cannot find 'InteractiveHost' in the CSharpKernel");

            
            IExcelService excel = new ExcelService(
                cSharpKernel,
                excelTypeProvider);

            try
            {
                excel.RefreshTypes();
            }
            catch
            {
                //Excel is closed
            }

            TAO3Formatters formatters = new TAO3Formatters(
                new CSharpFormatter(),
                new JsonFormatter(),
                new SqlFormatter(),
                new XmlFormatter());

            IVsCodeService vsCode = new VsCodeService(interactiveHost, converterService);
            IAvaloniaService avalonia = new AvaloniaService();

            IMacroService macroService = new MacroService(keyboard, compositeKernel);

            Prelude.Services = new TAO3Services(
                excel,
                notepad,
                keyboard,
                clipboard,
                toast,
                converterService,
                sourceService,
                destinationService,
                cellService,
                windowsService,
                httpClient,
                translationService,
                builtInConverters,
                typeProviders,
                formatters,
                vsCode,
                avalonia,
                macroService);

            Prelude.Kernel = compositeKernel;

            compositeKernel.RegisterForDisposal(Prelude.Services);

            compositeKernel.RegisterForDisposal(await ShowMacrosInAvalonia.CreateAsync(avalonia, macroService));
            compositeKernel.RegisterForDisposal(new SendToastNotificationOnMacroCompletion(macroService, toast));

            HtmlKernel htmlKernel = (HtmlKernel)compositeKernel.FindKernel("html");
            JavaScriptKernel javascriptKernel = (JavaScriptKernel)compositeKernel.FindKernel("javascript");

            compositeKernel.AddDirective(await MacroCommand.CreateAsync(macroService, javascriptKernel, htmlKernel));
            compositeKernel.AddDirective(new InputCommand(sourceService, converterService, cSharpKernel));
            compositeKernel.AddDirective(new OutputCommand(destinationService, converterService, cSharpKernel));
            compositeKernel.AddDirective(new CellCommand(cellService));
            compositeKernel.AddDirective(new RunCommand(cellService));
            compositeKernel.AddDirective(new ConnectMSSQLCommand());
            compositeKernel.AddDirective(new GenerateHttpClientCommand(cSharpKernel));

            compositeKernel.Add(new TranslateKernel(translationService));

            RazorLightEngine razorEngine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(Prelude))
                .SetOperatingAssembly(typeof(Prelude).Assembly)
                .UseMemoryCachingProvider()
                .Build();

            compositeKernel.Add(new RazorKernel(razorEngine));

            converterService.Register(builtInConverters.Csv);
            converterService.Register(builtInConverters.Csvh);
            converterService.Register(builtInConverters.Json);
            converterService.Register(builtInConverters.Xml);
            converterService.Register(builtInConverters.Line);
            converterService.Register(builtInConverters.Text);
            converterService.Register(builtInConverters.Html);
            converterService.Register(builtInConverters.CSharp);
            converterService.Register(builtInConverters.Sql);

            ClipboardIO clipboardIO = new ClipboardIO(clipboard);
            NotepadIO notepadIO = new NotepadIO(notepad);
            FileIO fileIO = new FileIO();
            HttpIO httpIO = new HttpIO(httpClient);

            sourceService.Register(clipboardIO);
            sourceService.Register(notepadIO);
            sourceService.Register(fileIO);
            sourceService.Register(httpIO);
            sourceService.Register(new CellSource());
            sourceService.Register(new ClipboardFileSource(clipboard));

            destinationService.Register(clipboardIO);
            destinationService.Register(notepadIO);
            destinationService.Register(fileIO);
            destinationService.Register(httpIO);
        }
    }
}
