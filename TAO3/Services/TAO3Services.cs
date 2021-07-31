using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TAO3.Cell;
using TAO3.Clipboard;
using TAO3.Converters;
using TAO3.Excel;
using TAO3.IO;
using TAO3.Keyboard;
using TAO3.Notepad;
using TAO3.Toast;
using TAO3.Translation;
using TAO3.TypeProvider;
using TAO3.Windows;

namespace TAO3.Services
{
    public record TAO3Services(
        IExcelService Excel,
        INotepadService Notepad,
        IKeyboardService Keyboard,
        IClipboardService Clipboard,
        IToastService Toast,
        IFormatConverterService FormatConverter,
        ISourceService SourceService,
        IDestinationService DestinationService,
        ICellService Cells,
        IWindowsService WindowsService,
        HttpClient HttpClient,
        ITranslationService Translation,
        TAO3Converters Converters,
        ITypeProviders TypeProviders) : IDisposable
    {
        public void Dispose()
        {
            Excel.Dispose();
            Notepad.Dispose();
            Keyboard.Dispose();
            Clipboard.Dispose();
            Toast.Dispose();
            FormatConverter.Dispose();
            SourceService.Dispose();
            DestinationService.Dispose();
            Cells.Dispose();
            WindowsService.Dispose();
            HttpClient.Dispose();
            Translation.Dispose();
            Converters.Dispose();
            TypeProviders.Dispose();
        }
    }
}
