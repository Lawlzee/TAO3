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
using TAO3.Internal.Kernels;

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
            IFormatConverterService formatConverter = new FormatConverterService();

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

            TAO3Converters converters = new TAO3Converters(
                new CSharpConverter(new CSharpObjectSerializer()),
                new CsvConverter(csvTypeProvider, false),
                new CsvConverter(csvTypeProvider, true),
                new HtmlConverter(),
                jsonConverter,
                new LineConverter(),
                new TextConverter(),
                new XmlConverter(jsonConverter, xmlTypeProvider),
                new SqlConverter(sqlTypeProvider, new SqlDeserializer(), new SqlObjectSerializer()));

            IExcelService excel = new ExcelService(
                (CSharpKernel)compositeKernel.FindKernel("csharp"),
                excelTypeProvider);

            try
            {
                excel.RefreshTypes();
            }
            catch
            {
                //Excel is closed
            }

            Prelude.Services = new TAO3Services(
                excel,
                notepad,
                keyboard,
                clipboard,
                toast,
                formatConverter,
                sourceService,
                destinationService,
                cellService,
                windowsService,
                httpClient,
                translationService,
                converters,
                typeProviders);

            Prelude.Kernel = compositeKernel;

            compositeKernel.RegisterForDisposal(Prelude.Services);

            compositeKernel.AddDirective(await MacroCommand.CreateAsync(keyboard, toast));
            compositeKernel.AddDirective(new InputCommand(sourceService, formatConverter));
            compositeKernel.AddDirective(new OutputCommand(destinationService, formatConverter));
            compositeKernel.AddDirective(new CellCommand(cellService));
            compositeKernel.AddDirective(new RunCommand(cellService));
            compositeKernel.AddDirective(new ConnectMSSQLCommand());

            compositeKernel.Add(new TranslateKernel(translationService));

            formatConverter.Register(converters.Csv);
            formatConverter.Register(converters.Csvh);
            formatConverter.Register(converters.Json);
            formatConverter.Register(converters.Xml);
            formatConverter.Register(converters.Line);
            formatConverter.Register(converters.Text);
            formatConverter.Register(converters.Html);
            formatConverter.Register(converters.CSharp);
            formatConverter.Register(converters.Sql);

            ClipboardIO clipboardIO = new ClipboardIO(clipboard);
            NotepadIO notepadIO = new NotepadIO(notepad);

            sourceService.Register(clipboardIO);
            sourceService.Register(notepadIO);
            sourceService.Register(new CellSource());

            destinationService.Register(clipboardIO);
            destinationService.Register(notepadIO);
        }
    }
}
