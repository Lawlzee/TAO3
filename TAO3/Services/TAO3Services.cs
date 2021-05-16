using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Clipboard;
using TAO3.Converters;
using TAO3.Excel;
using TAO3.InputSources;
using TAO3.Keyboard;
using TAO3.Notepad;
using TAO3.OutputDestinations;
using TAO3.Toast;

namespace TAO3.Services
{
    public class TAO3Services
    {
        public IExcelService Excel { get; }
        public INotepadService Notepad { get; }
        public IKeyboardService Keyboard { get; }
        public IClipboardService Clipboard { get; }
        public IToastService Toast { get; }
        public IFormatConverterService FormatConverter { get; }
        public IInputSourceService InputSource { get; }
        public IOutputDestinationService OutputDestination { get; }

        public TAO3Services(IExcelService excel, INotepadService notepad, IKeyboardService keyboard, IClipboardService clipboard, IToastService toast, IFormatConverterService formatConverter, IInputSourceService inputSource, IOutputDestinationService outputDestination)
        {
            Excel = excel;
            Notepad = notepad;
            Keyboard = keyboard;
            Clipboard = clipboard;
            Toast = toast;
            FormatConverter = formatConverter;
            InputSource = inputSource;
            OutputDestination = outputDestination;
        }
    }
}
