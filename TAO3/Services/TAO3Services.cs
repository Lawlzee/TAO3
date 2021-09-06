using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TAO3.Avalonia;
using TAO3.Cell;
using TAO3.Clipboard;
using TAO3.Converters;
using TAO3.Excel;
using TAO3.Formatting;
using TAO3.IO;
using TAO3.Keyboard;
using TAO3.Notepad;
using TAO3.Toast;
using TAO3.Translation;
using TAO3.TypeProvider;
using TAO3.VsCode;
using TAO3.Windows;

namespace TAO3.Services
{
    public record TAO3Services(
        IExcelService Excel,
        INotepadService Notepad,
        IKeyboardService Keyboard,
        IClipboardService Clipboard,
        IToastService Toast,
        IConverterService Converter,
        ISourceService SourceService,
        IDestinationService DestinationService,
        ICellService Cells,
        IWindowsService WindowsService,
        HttpClient HttpClient,
        ITranslationService Translation,
        TAO3Converters BuiltInConverters,
        ITypeProviders TypeProviders,
        TAO3Formatters Formatters,
        IVsCodeService VsCode,
        IAvaloniaService Avalonia) : IDisposable
    {
        public void Dispose()
        {
            Excel.Dispose();
            Notepad.Dispose();
            Keyboard.Dispose();
            Clipboard.Dispose();
            Toast.Dispose();
            Converter.Dispose();
            SourceService.Dispose();
            DestinationService.Dispose();
            Cells.Dispose();
            WindowsService.Dispose();
            HttpClient.Dispose();
            Translation.Dispose();
            BuiltInConverters.Dispose();
            TypeProviders.Dispose();
            Formatters.Dispose();
            VsCode.Dispose();
            Avalonia.Dispose();
        }
    }
}
