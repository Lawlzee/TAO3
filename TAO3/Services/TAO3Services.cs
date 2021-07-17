using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Cell;
using TAO3.Clipboard;
using TAO3.Converters;
using TAO3.Excel;
using TAO3.InputSources;
using TAO3.Keyboard;
using TAO3.Notepad;
using TAO3.OutputDestinations;
using TAO3.TextSerializer.CSharp;
using TAO3.Toast;
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
        IInputSourceService InputSource,
        IOutputDestinationService OutputDestination,
        ICellService Cells,
        IWindowsService WindowsService,
        ICSharpObjectSerializer CSharpSerializer) : IDisposable
    {
        public void Dispose()
        {
            Excel.Dispose();
            Notepad.Dispose();
            Keyboard.Dispose();
            Clipboard.Dispose();
            Toast.Dispose();
            FormatConverter.Dispose();
            InputSource.Dispose();
            OutputDestination.Dispose();
            Cells.Dispose();
            WindowsService.Dispose();
            CSharpSerializer.Dispose();
        }
    }
}
